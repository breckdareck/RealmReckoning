using System;
using System.Collections.Generic;
using System.Linq;
using Game._Scripts.Runtime.Abilities;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Managers;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game._Scripts.Runtime.Battle
{
    [Serializable]
    public class BattleStateMachine
    { 
        private BattleState _currentState;
        private int _currentUnitIndex;
        private List<BattleUnit> _playerUnits;
        private List<BattleUnit> _enemyUnits;
        private List<BattleUnit> _allUnits;

        private PlayerTurnHandler _playerTurnHandler;
        private EnemyTurnHandler _enemyTurnHandler;
        private AbilityExecutor _abilityExecutor;

        private AbilitySO _selectedAbilitySo;
        private BattleUnit _selectedBattleUnit;
        private BattleUnit _targetedEnemyBattleUnit;
        private BattleUnit _lastTargetedBattleUnit;

        public BattleState CurrentState => _currentState;
        public List<BattleUnit> PlayerUnits => _playerUnits;
        public List<BattleUnit> EnemyUnits => _enemyUnits;


        public BattleStateMachine(List<BattleUnit> playerUnits, List<BattleUnit> enemyUnits, List<BattleUnit> allUnits)
        {
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;
            _allUnits = allUnits;

            _abilityExecutor = new AbilityExecutor();
            _playerTurnHandler = new PlayerTurnHandler(this, _abilityExecutor);
            _enemyTurnHandler = new EnemyTurnHandler(this, _abilityExecutor);
        }


        public void Update()
        {
            if (_targetedEnemyBattleUnit.Model.IsDead && _currentState != BattleState.End)
            {
                _targetedEnemyBattleUnit = EnemyUnits.Find(x => !x.Model.IsDead);
                _targetedEnemyBattleUnit.UIBattleUnit.SetEnemyTargetAnim();
            }

            if (_currentState == BattleState.TurnCycle)
            {
                if (GetActiveUnit().Model.IsTakingTurn) return;
                UpdateUnitsProgress();
            }
        }
        
        private void UpdateUnitsProgress()
        {
            foreach (var x in _allUnits)
            {
                if (x.Model.IsDead) continue;
                x.UpdateTurnProgress(Time.deltaTime * 10);
                if (!(x.Model.TurnProgress >= BattleUnitModel.MAXTURNPROGRESS)) continue;
                _currentUnitIndex = _allUnits.IndexOf(x);
                SetState(GetNextUnitTurn());
                return;
            }
        }

        public void SetState(BattleState newState)
        {
            _currentState = newState;
            EventManager.Instance.InvokeOnStateChanged();

            // Perform state-specific initialization or logic
            switch (_currentState)
            {
                case BattleState.Start:
                    StartBattle();
                    break;

                case BattleState.TurnCycle:
                    break;

                case BattleState.PlayerTurn:
                    _playerTurnHandler.StartPlayerTurn();
                    break;

                case BattleState.EnemyTurn:
                    _enemyTurnHandler.StartEnemyTurn();
                    break;

                case BattleState.EndTurn:
                    EndTurn();
                    break;

                case BattleState.End:
                    EndBattle();
                    break;
            }
        }

        private void StartBattle()
        {
            _targetedEnemyBattleUnit = _enemyUnits[0];
            _targetedEnemyBattleUnit.UIBattleUnit.SetEnemyTargetAnim();
            EventManager.Instance.OnUnitSelectedChangedEvent += OnUnitSelected;
            SetState(BattleState.TurnCycle);
        }

        private void EndTurn()
        {
            //Debug.Log("End of turn");

            GetActiveUnit().EndTurn();

            SetState(IsBattleOver() ? BattleState.End : BattleState.TurnCycle);
        }

        private void EndBattle()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private BattleState GetNextUnitTurn()
        {
            return _playerUnits.Contains(_allUnits[_currentUnitIndex])
                ? BattleState.PlayerTurn
                : BattleState.EnemyTurn;
        }

        private bool IsBattleOver()
        {
            if (PlayerUnits.All(x => x.Model.IsDead))
            {
                Debug.Log("Battle Ended : Enemy Wins");
                return true;
            }
            else if (EnemyUnits.All(x => x.Model.IsDead))
            {
                Debug.Log("Battle Ended : Player Wins");
                return true;
            }

            return false;
        }

        public BattleUnit GetActiveUnit()
        {
            return _allUnits[_currentUnitIndex];
        }

        public void OnUnitSelected(BattleUnit clickedBattleUnit)
        {
            // Set the selected unit in the BattleSystem
            if (_enemyUnits.Contains(clickedBattleUnit))
            {
                _targetedEnemyBattleUnit.UIBattleUnit.SetEnemyTargetAnim(); // Turn off the previous Animation
                _targetedEnemyBattleUnit = clickedBattleUnit;
                _targetedEnemyBattleUnit.UIBattleUnit.SetEnemyTargetAnim();
            }

            if (_abilityExecutor.WaitingForTargetSelection)
            {
                _selectedBattleUnit = clickedBattleUnit;
                _abilityExecutor.SetWaitingForSelectionFalse();
            }
        }

        public BattleUnit GetSelectedUnit()
        {
            return _selectedBattleUnit;
        }

        public void ResetSelectedUnit()
        {
            _selectedBattleUnit = null;
        }

        public BattleUnit GetTargetUnit()
        {
            return _targetedEnemyBattleUnit;
        }
    }
}
using System.Linq;
using Game._Scripts.Runtime.Abilities;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Managers;
using UnityEngine;

namespace Game._Scripts.Runtime.Battle
{
    public class EnemyTurnHandler
    {
        private BattleStateMachine _battleStateMachine;
        private AbilityExecutor _abilityExecutor;

        public EnemyTurnHandler(BattleStateMachine battleStateMachine, AbilityExecutor abilityExecutor)
        {
            _battleStateMachine = battleStateMachine;
            _abilityExecutor = abilityExecutor;
        }

        public async void StartEnemyTurn()
        {
            //Debug.Log("Enemy's turn");

            EventManager.Instance.InvokeOnStepChanged("Wait For Enemy Turn");

            var abilityChosen = Random.Range(0, _battleStateMachine.GetActiveUnit().Model.Unit.currentUnitAbilities.Count);

            var selectedAbility = _battleStateMachine.GetActiveUnit().Model.Unit.currentUnitAbilities.Keys.ToArray()[abilityChosen];

            await _abilityExecutor.ExecuteAbility(selectedAbility, _battleStateMachine.GetActiveUnit());

            _battleStateMachine.SetState(BattleState.EndTurn);
        }
    }
}
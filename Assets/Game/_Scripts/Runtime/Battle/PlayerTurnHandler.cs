using System.Linq;
using Game._Scripts.Runtime.Abilities;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Managers;
using Game._Scripts.Runtime.Scriptables;
using Game._Scripts.Runtime.UI.Battle;
using UnityEngine;

namespace Game._Scripts.Runtime.Battle
{
    public class PlayerTurnHandler
    {
        private BattleStateMachine _battleStateMachine;
        private AbilityExecutor _abilityExecutor;
        
        
        private AbilitySO _currentAbilitySo;

        public PlayerTurnHandler(BattleStateMachine battleStateMachine, AbilityExecutor abilityExecutor)
        {
            _battleStateMachine = battleStateMachine;
            _abilityExecutor = abilityExecutor;
            
            EventManager.Instance.OnAbilitySelectionChangedEvent += OnAbilitySelected;
        }
        
        

        public void StartPlayerTurn()
        {
            //Debug.Log("Player's turn");
            
            EventManager.Instance.InvokeOnStepChanged("Select Ability");

            SetupCurrentUnitAbilityButtons();

            _battleStateMachine.GetActiveUnit().UIBattleUnit.SetActiveUnitAnim();

            //EventManager.Instance.OnAbilitySelectionChangedEvent += OnAbilitySelected;
        }

        private void SetupCurrentUnitAbilityButtons()
        {
            for (var i = 0; i < 4; i++)
                UI_Battle.Instance.SetupAbilityButton(
                    i < _battleStateMachine.GetActiveUnit().Model.Unit.currentUnitAbilities.Count
                        ? _battleStateMachine.GetActiveUnit().Model.Unit.currentUnitAbilities.Keys.ToArray()[i]
                        : null, i);
        }

        private async void OnAbilitySelected(AbilitySO selectedAbilitySo)
        {
            if(_battleStateMachine.GetActiveUnit().IsAbilityOnCooldown(selectedAbilitySo))
            {
                Debug.Log("This ability is still cooling down.");
                return;
            }
            
            // If same button (ability) pressed again and it is currently in progress (waiting for selection)
            if (_currentAbilitySo == selectedAbilitySo && _abilityExecutor.WaitingForTargetSelection)
            {
                //Debug.Log("Ability selection cancelled.");
                _abilityExecutor.CancelAbility();  // Cancel the current ability
                _currentAbilitySo = null; // Reset the currently selected ability
                return;
            }

            if (_abilityExecutor.WaitingForTargetSelection)
            {
                Debug.Log("There is already an ability selected waiting for target confirmation. Please complete or cancel the current ability before selecting another.");
                return;
            }
            

            if (_battleStateMachine.GetTargetUnit() != null && selectedAbilitySo != null)
            {
                // Keep track of the currently selected ability
                _currentAbilitySo = selectedAbilitySo;  
                
                // Execute the selected ability on the selected unit
                await _abilityExecutor.ExecuteAbility(selectedAbilitySo, _battleStateMachine.GetActiveUnit());

                if (_abilityExecutor.AbilityCanceled)
                {
                    _abilityExecutor.SetWaitingForSelectionFalse();
                    EventManager.Instance.InvokeOnStepChanged("Canceled Attack - Reselect an Ability");
                    return;
                }
                
                //EventManager.Instance.OnAbilitySelectionChangedEvent -= OnAbilitySelected;
                
                // Sets all the Ability Buttons to dissapear
                for (var i = 0; i < 4; i++) UI_Battle.Instance.SetupAbilityButton(null, i);

                // Turns off the Active Anim for Players Units
                _battleStateMachine.GetActiveUnit().UIBattleUnit.SetActiveUnitAnim();
                
                _battleStateMachine.GetActiveUnit().StartCooldown(selectedAbilitySo);

                // Notify the end of the player's turn
                _battleStateMachine.SetState(BattleState.EndTurn);
            }
        }
    }
}
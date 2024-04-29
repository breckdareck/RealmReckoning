using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game._Scripts.Runtime.Abilities.Actions;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Game._Scripts.Runtime.Abilities
{
    [Serializable]
    public class AbilityExecutor
    {
        private List<AbilityAction> _remainingActions;
        private bool _waitingForTargetSelection;
        private bool _abilityCanceled = true;

        public bool WaitingForTargetSelection => _waitingForTargetSelection;
        public bool AbilityCanceled => _abilityCanceled;

        public Task ExecuteAbility(AbilitySO ability, BattleUnit source)
        {
            _abilityCanceled = false;
            if (ability == null || source == null)
                return Task.CompletedTask;
            _remainingActions = new List<AbilityAction> { ability.BaseAction };
            _waitingForTargetSelection = false;

            return StartAction(_remainingActions[0], source, ability);
        }

        private async Task StartAction(AbilityAction action, BattleUnit source, AbilitySO ability)
        {
            while (_remainingActions.Count > 0)
            {
                await ExecuteActionOnTargets(action, source, ability);
                _remainingActions.Remove(action);

                if (_remainingActions.Count > 0)
                {
                    action = _remainingActions[0];
                }
            }
        }


        private async Task ExecuteActionOnTargets(AbilityAction action, BattleUnit source, AbilitySO ability)
        {
            List<BattleUnit> targets = await SelectTargets(action, source, ability);
        
            if (targets == null || !targets.Any() || targets.Any(target => target == null)) return;
            
            foreach (var target in targets)
            {
                var command = CommandFactory.CreateCommand(action);
                command.Execute(source, target, ability);
            }

            if (ability.BaseAction is DamageAbilityAction damageAbilityAction)
            {
                damageAbilityAction.ResetAddedDamagePercent();
            }
            
        }

        private async Task<List<BattleUnit>> SelectTargets(AbilityAction action, BattleUnit source, AbilitySO ability)
        {
            return source.Model.IsControlledByAI
                ? await AISelectTarget(ability)
                : await GetTargetForAction(action, source);
        }

        private async Task<List<BattleUnit>> AISelectTarget(AbilitySO ability)
        {
            await Task.Delay(1000);

            if (ability.BaseAction.targetType == TargetType.AllEnemies)
            {
                return BattleSystem.Instance.BattleStateMachine.PlayerUnits;
            }

            var playerUnitIndex = Random.Range(0, BattleSystem.Instance.BattleStateMachine.PlayerUnits.Count);
            return new List<BattleUnit> {BattleSystem.Instance.BattleStateMachine.PlayerUnits[playerUnitIndex]};
        }

        private async Task<List<BattleUnit>> GetTargetForAction(AbilityAction action, BattleUnit source)
        {
            switch (action.targetSelection)
            {
                case TargetSelectionType.Manual:
                    if (action.targetType == TargetType.Self)
                        return new List<BattleUnit>() { source };
                    else
                        return new List<BattleUnit>() { await TargetSelector.SelectManualTarget(action, this) };

                case TargetSelectionType.Auto:
                    if (action.targetType == TargetType.AllEnemies)
                        return TargetSelector.GetAllEnemyUnits();
                    else if (action.targetType == TargetType.AllAllies)
                        return TargetSelector.GetAllAllyUnits();
                    else
                        return new List<BattleUnit>() { TargetSelector.GetAutomaticallySelectedTarget() };

                default:
                    Debug.LogWarning("Unsupported target selection type");
                    return null;
            }
        }
        
        public void CancelAbility()
        {
            // Clear remaining actions.
            _remainingActions?.Clear();

            // Not waiting for target selection anymore.
            _abilityCanceled = true;
            _waitingForTargetSelection = false;
            
            // Log the cancellation.
            Debug.Log("User cancelled the selected ability");
        }

        public void SetWaitingForSelectionFalse()
        {
            _waitingForTargetSelection = false;
            Debug.Log("Set Waiting For Target Selection False");
        }

        public void SetWaitingForSelectionTrue()
        {
            _waitingForTargetSelection = true;
            Debug.Log("Set Waiting For Target Selection True");
        }
    }
}
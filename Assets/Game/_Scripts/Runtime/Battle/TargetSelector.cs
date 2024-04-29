using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Runtime.Abilities;
using Game._Scripts.Runtime.Abilities.Actions;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Managers;

namespace Game._Scripts.Runtime.Battle
{
    public static class TargetSelector
    {
        public static async Task<BattleUnit> SelectManualTarget(AbilityAction action, AbilityExecutor abilityExecutor)
        {
            await WaitForValidTargetSelection(action, abilityExecutor);
            var target = BattleSystem.Instance.BattleStateMachine.GetSelectedUnit();
            BattleSystem.Instance.BattleStateMachine.ResetSelectedUnit();
            return target;
        }

        private static async Task WaitForValidTargetSelection(AbilityAction action, AbilityExecutor abilityExecutor)
        {
            abilityExecutor.SetWaitingForSelectionTrue();

            var targetType = action.targetType == TargetType.Ally
                ? BattleSystem.Instance.BattleStateMachine.PlayerUnits
                : BattleSystem.Instance.BattleStateMachine.EnemyUnits;

            EventManager.Instance.InvokeOnStepChanged(
                action.targetType == TargetType.Ally ? "Select Ally Unit" : "Select Enemy Unit");

            bool isTargetTypeValid = false;
            do
            {
                await Task.Yield();
                isTargetTypeValid = targetType.Contains(BattleSystem.Instance.BattleStateMachine.GetSelectedUnit());

                if (!isTargetTypeValid)
                {
                    abilityExecutor.SetWaitingForSelectionTrue();
                }
            
            } while (abilityExecutor.WaitingForTargetSelection && !isTargetTypeValid && !abilityExecutor.AbilityCanceled);

            if (abilityExecutor.AbilityCanceled)
            {
                BattleSystem.Instance.BattleStateMachine.ResetSelectedUnit();
            }
            EventManager.Instance.InvokeOnStepChanged("");
            //await Task.CompletedTask;
        }

        public static BattleUnit GetAutomaticallySelectedTarget()
        {
            return BattleSystem.Instance.BattleStateMachine.GetTargetUnit();
        }

        public static List<BattleUnit> GetAllAllyUnits()
        {
            return BattleSystem.Instance.BattleStateMachine.PlayerUnits;
        }

        public static List<BattleUnit> GetAllEnemyUnits()
        {
            return BattleSystem.Instance.BattleStateMachine.EnemyUnits;
        }
    }
}
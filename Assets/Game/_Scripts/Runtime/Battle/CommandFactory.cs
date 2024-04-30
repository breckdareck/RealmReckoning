using System.Linq;
using Game._Scripts.Runtime.Abilities.Actions;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Interfaces;
using UnityEngine;

namespace Game._Scripts.Runtime.Battle
{
    public static class CommandFactory
    {
        public static ICommand CreateCommand(AbilityAction action)
        {
            switch (action.actionType)
            {
                case ActionType.Attack:
                    var damageAbilityAction = action as DamageAbilityAction;
                    return new AttackCommand(damageAbilityAction.DamageType);

                case ActionType.Heal:
                    var healAbilityAction = action as HealAbilityAction;
                    return new HealCommand(healAbilityAction.healPercent, healAbilityAction.barrierPercent);

                case ActionType.StatusEffect:
                    var statusEffectAbilityAction = action as StatusEffectAbilityAction;
                    var newStatusEffects = statusEffectAbilityAction.statusEffects.Select(effect => Object.Instantiate(effect)).ToList();
                    return new StatusEffectCommand(newStatusEffects, 0, 0);
                
                case ActionType.Dispell:
                    return new DispellCommand(null);

                default:
                    Debug.LogWarning("Unsupported action type");
                    return null;
            }
        }
    }
}
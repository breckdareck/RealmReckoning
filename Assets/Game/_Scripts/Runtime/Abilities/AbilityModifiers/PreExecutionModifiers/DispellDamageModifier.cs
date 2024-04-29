using System.Collections.Generic;
using Game._Scripts.Runtime.Abilities.Actions;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Interfaces;
using Game._Scripts.Runtime.Managers;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Game._Scripts.Runtime.Abilities.AbilityModifiers.PreExecutionModifiers
{
    public class DispellDamageModifier : PreExecutionAbilityModifier
    {
        public TargetType TargetType;
        public List<StatusEffectSO> StatusEffectsToCheckAgainst;
        [field:SerializeField] public int AddedDamagePercentPerStatusEffectDispelled { get; }

        private int _statusEffectsDispelled;
        
        public override void ApplyPreEffect(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            EventManager.Instance.OnUnitStatusEffectsDispelledEvent += UpdateStatusEffectsDispelled;
            
            DispellCommand dispellCommand;
            dispellCommand = StatusEffectsToCheckAgainst is null ? new DispellCommand(null) : new DispellCommand( new List<StatusEffectSO>( new List<StatusEffectSO>(StatusEffectsToCheckAgainst)));

            if (TargetType == TargetType.Self)
            {
                dispellCommand.Execute(source, source, ability);
            }
            else
            {
                dispellCommand.Execute(source, target, ability);
            }

            for (int i = 0; i < _statusEffectsDispelled; i++)
            {
                if (ability.BaseAction is DamageAbilityAction damageAction)
                {
                    damageAction.AddDamage(AddedDamagePercentPerStatusEffectDispelled);
                }
            }
            
            EventManager.Instance.OnUnitStatusEffectsDispelledEvent -= UpdateStatusEffectsDispelled;
        }

        private void UpdateStatusEffectsDispelled(int amount)
        {
            _statusEffectsDispelled = amount;
        }
    }
}
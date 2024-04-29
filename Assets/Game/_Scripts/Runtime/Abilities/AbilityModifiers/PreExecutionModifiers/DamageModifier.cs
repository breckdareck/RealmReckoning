using Game._Scripts.Runtime.Abilities.Actions;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Game._Scripts.Runtime.Abilities.AbilityModifiers.PreExecutionModifiers
{
    public class DamageModifier : PreExecutionAbilityModifier
    {
        [field:SerializeField] public int AddedDamagePercent { get; }
        
        public override void ApplyPreEffect(BattleUnit source, BattleUnit target, AbilitySO ability)
        {

            if (ability.BaseAction is DamageAbilityAction damageAction)
            {
                damageAction.AddDamage(AddedDamagePercent);
            }
            
        }
    }
}
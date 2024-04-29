using Game._Scripts.Runtime.Abilities.Actions;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Interfaces;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Game._Scripts.Runtime.Abilities.AbilityModifiers.PostExecutionModifiers
{
    public class RepeatedAttackEffectModifier : PostExecutionAbilityModifier
    {
        public int ChanceOnCritHit;
        public int ChanceOnNonCritHit;
        
        
        private bool _isCrit;
        
        public void SetIsCrit(bool value)
        {
            _isCrit = value;
        }
        
        public override void ApplyPostEffect(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            bool repeatAttack;
            if (_isCrit)
            {
                repeatAttack = Random.Range(0f, 100f) <= ChanceOnCritHit;
            }
            else
            {
                repeatAttack = Random.Range(0f, 100f) <= ChanceOnNonCritHit;
            }

            if (!repeatAttack) return;
            
            if (ability.BaseAction is DamageAbilityAction damageAction)
            {
                var newAttackCommand = new AttackCommand(damageAction.DamageType);
                newAttackCommand.Execute(source, target, ability);
            }

        }
    }
}
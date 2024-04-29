using Game._Scripts.Runtime.Abilities.AbilityModifiers.PostExecutionModifiers;
using Game._Scripts.Runtime.Abilities.Actions;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Game._Scripts.Runtime.Interfaces
{
    public class AttackCommand : ICommand
    {
        private readonly DamageType _damageType;

        private bool _attackWasCrit = false;

        public AttackCommand(DamageType damageType)
        {
            _damageType = damageType;
        }

        public void Execute(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            if(target.Model.IsDead) return;
            var damageAction = ability.BaseAction as DamageAbilityAction;
            
            AbilityModifierPreCheck(source, target, ability);
            
            var damage = 0;
            var chanceToHit = CalculateHitChance(source, target);
            
            var hitLanded = Random.Range(0f, 100f) <= chanceToHit;

            if (!hitLanded)
            {
                damage = 0;
                target.ApplyDamage(damage, true);
                return;
            }

            damage = CalculateDamage(source, damageAction);
            
            var chanceToCrit = CalculateCritChance(source, target);
            var isCrit = Random.Range(0f, 100f) <= chanceToCrit;
            if (isCrit)
                damage = (int)(damage * (source.Model.CurrentBattleStats[StatType.CriticalDamage] / 100f));
            
            _attackWasCrit = isCrit;

            
            damage -= CalculateArmorReduction(source, target, damage);

            var sourceDN = source.Model.CurrentBattleStats[StatType.DefenseNegation];
            var guaranteedDamage = (int)(damage * (sourceDN / 100));
            damage += guaranteedDamage;

            // TODO: Implement Barrier Piercing
            // TODO - Implement Health Steal

            target.ApplyDamage(Mathf.Clamp(damage, 0, 999999), false);
            
            AbilityModifierPostCheck(source, target, ability);
            //damageAction.ResetAddedDamagePercent();
        }

        private void AbilityModifierPreCheck(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            foreach (var modifier in ability.PreExecutionModifiers)
            {
                if (source.Model.Unit.currentUnitAbilities[ability] >= modifier.Key)
                {
                    modifier.Value.ForEach(m => m.ApplyPreEffect(source, target, ability));
                }
            }
        }
        
        private void AbilityModifierPostCheck(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            foreach (var modifier in ability.PostExecutionModifiers)
            {
                if (source.Model.Unit.currentUnitAbilities[ability] >= modifier.Key)
                {
                    foreach (var m in modifier.Value)
                    {
                        if (m is RepeatedAttackEffectModifier repeatedAttackEffectModifier)
                        {
                            repeatedAttackEffectModifier.SetIsCrit(_attackWasCrit);
                            m.ApplyPostEffect(source, target, ability);
                        }
                        else
                        {
                            m.ApplyPostEffect(source, target, ability);
                        }
                    }
                }
            }
        }


        private float CalculateHitChance(BattleUnit source, BattleUnit target) 
        {
            var sourceStat = _damageType == DamageType.PhysicalDamage 
                ? source.Model.CurrentBattleStats[StatType.PhysicalAccuracy] 
                : source.Model.CurrentBattleStats[StatType.MagikAccuracy];
            
            var targetStat = _damageType == DamageType.PhysicalDamage 
                ? target.Model.CurrentBattleStats[StatType.PhysicalDodge] 
                : target.Model.CurrentBattleStats[StatType.MagikDodge];

            return 100 - Mathf.Clamp(targetStat - sourceStat, 0, 100);
        }
        
        private int CalculateDamage(BattleUnit source, DamageAbilityAction action) 
        {
            
            var offenseStat = _damageType == DamageType.PhysicalDamage 
                ? source.Model.CurrentBattleStats[StatType.PhysicalOffense] 
                : source.Model.CurrentBattleStats[StatType.MagikOffense];
            
            return (int)((float)(action.DamagePercent + action.AddedDamagePercent) / 100 * offenseStat);
        }
        
        private float CalculateCritChance(BattleUnit source, BattleUnit target)
        {
            var sourceStat = _damageType == DamageType.PhysicalDamage 
                ? source.Model.CurrentBattleStats[StatType.PhysicalCriticalChance] 
                : source.Model.CurrentBattleStats[StatType.MagikCriticalChance];

            var targetStat = _damageType == DamageType.PhysicalDamage 
                ? target.Model.CurrentBattleStats[StatType.PhysicalCriticalAvoidance] 
                : target.Model.CurrentBattleStats[StatType.MagikCriticalAvoidance];

            return Mathf.Clamp(sourceStat - targetStat, 0, 100);
        }

        private int CalculateArmorReduction(BattleUnit source, BattleUnit target, int damage)
        {
            var sourceStat = _damageType == DamageType.PhysicalDamage 
                ? source.Model.CurrentBattleStats[StatType.ArmorPierce] 
                : source.Model.CurrentBattleStats[StatType.MagikArmorPierce];

            var targetStat = _damageType == DamageType.PhysicalDamage 
                ? target.Model.CurrentBattleStats[StatType.Armor] 
                : target.Model.CurrentBattleStats[StatType.MagikArmor];

            var remainingArmorPercent = Mathf.Clamp(targetStat - sourceStat, 0, 100) / 100;
            return (int)(damage * remainingArmorPercent);
        }
        
    }
}
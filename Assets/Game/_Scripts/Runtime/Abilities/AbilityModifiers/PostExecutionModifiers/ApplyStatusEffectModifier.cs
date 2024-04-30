using System.Collections.Generic;
using System.Linq;
using Assets.Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Interfaces;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Game._Scripts.Runtime.Abilities.AbilityModifiers.PostExecutionModifiers
{

    public class ApplyStatusEffectModifier : PostExecutionAbilityModifier
    {
        public UnitRankSO RankToCheckAgainst;
        public UnitTagSO TagToCheckAgainst;
        public StatusEffectSO StatusEffectToCheckAgainst;

        public StatusEffectSO AddedStatusEffect;
        public int EffectedDuration;
        public int ChanceToAfflict;
        public TargetType TargetType;

        public override void ApplyPostEffect(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            List<StatusEffectSO> effects = new() { AddedStatusEffect };
            
            // Create a command with the status effect and chance to afflict
            var command = new StatusEffectCommand(effects, ChanceToAfflict, EffectedDuration);

            // Apply post effect based on the target type
            switch (TargetType)
            {
                case TargetType.Self:
                    ApplyToAllUnits(new []{source}, command, source, ability);
                    break;
                case TargetType.AllEnemies:
                    ApplyToAllUnits(BattleSystem.Instance.BattleStateMachine.EnemyUnits, command, source, ability);
                    break;
                case TargetType.AllAllies:
                    ApplyToAllUnits(BattleSystem.Instance.BattleStateMachine.PlayerUnits, command, source, ability);
                    break;
                default:
                    ApplyToAllUnits(new []{target}, command, source, ability);
                    break;
            }
        }
        
        // Helper method to reduce redundancy when applying effects to all units
        private void ApplyToAllUnits(IEnumerable<BattleUnit> units, StatusEffectCommand command, BattleUnit source,
            AbilitySO ability)
        {
            foreach (var unit in units.Where(u =>
                         (TagToCheckAgainst is null || CheckTag(u)) &&
                         (StatusEffectToCheckAgainst is null ||
                          CheckStatusEffect(u)) &&
                         (RankToCheckAgainst is null || CheckRank(u))))
            {
                command.Execute(source, unit, ability);
            }
        }
        
        private bool CheckTag(BattleUnit unit)
        {
            return unit.Model.Unit.UnitData.unitTags.Contains(TagToCheckAgainst);
        }

        private bool CheckStatusEffect(BattleUnit unit)
        {
            return unit.Model.StatusEffects.Exists(x => x.StatusEffectSO == StatusEffectToCheckAgainst);
        }

        private bool CheckRank(BattleUnit unit)
        {
            return unit.Model.Unit.UnitData.unitRankSo == RankToCheckAgainst;
        }
    }
}

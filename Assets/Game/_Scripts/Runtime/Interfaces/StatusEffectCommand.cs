using System.Collections.Generic;
using Assets.Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Game._Scripts.Runtime.Interfaces
{
    public class StatusEffectCommand : ICommand
    {
        private readonly List<StatusEffectSO> _statusEffects;
        private readonly int _extraChanceToAfflict;

        public StatusEffectCommand(List<StatusEffectSO> statusEffects, int extraChanceToAfflict)
        {
            _statusEffects = statusEffects;
            _extraChanceToAfflict = extraChanceToAfflict;
        }

        public void Execute(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            if (target.Model.IsControlledByAI)
            {
                var sourcePOT = source.Model.CurrentBattleStats[StatType.Potency];
                var targetRES = target.Model.CurrentBattleStats[StatType.Resilience];
                var chanceToHit = 100 - Mathf.Clamp(targetRES - sourcePOT + _extraChanceToAfflict, 0, 100);

                var hitLanded = Random.Range(0f, 100f) <= chanceToHit;
                if (!hitLanded)
                {
                    Debug.Log($"{target.name} Resisted {source.name}'s StatusEffect('s) ");
                    return;
                }
            }

            foreach (var statusEffect in _statusEffects)
            {
                Debug.Log($"{target.name} was Afflicted with {statusEffect.StatusEffectName} by {source.name}");
                target.ApplyStatusEffect(statusEffect);
            }
        }
    }
}
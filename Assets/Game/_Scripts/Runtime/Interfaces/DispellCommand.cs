using System;
using System.Collections.Generic;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Scriptables;

namespace Game._Scripts.Runtime.Interfaces
{
    public class DispellCommand : ICommand
    {
        private readonly List<StatusEffectSO> _specificStatusEffectsToDispell;

        public DispellCommand(List<StatusEffectSO> specificStatusEffectsToDispell)
        {
            _specificStatusEffectsToDispell = specificStatusEffectsToDispell;
        }
        
        public void Execute(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            if (_specificStatusEffectsToDispell is not null && _specificStatusEffectsToDispell.Count > 0)
            {
                target.DispellSpecificStatusEffects(_specificStatusEffectsToDispell);
            }
            else
            {
                target.DispellAllStatusEffects();
            }
        }
    }
}
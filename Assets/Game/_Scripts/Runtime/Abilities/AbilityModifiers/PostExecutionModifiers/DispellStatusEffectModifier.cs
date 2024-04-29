using System.Collections.Generic;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Interfaces;
using Game._Scripts.Runtime.Scriptables;

namespace Game._Scripts.Runtime.Abilities.AbilityModifiers.PostExecutionModifiers
{
    public class DispellStatusEffectModifier : PostExecutionAbilityModifier
    {
        public TargetType TargetType;
        public StatusEffectSO StatusEffectToCheckAgainst;
        
        public override void ApplyPostEffect(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            DispellCommand dispellCommand;
            dispellCommand = StatusEffectToCheckAgainst is null ? new DispellCommand(null) : new DispellCommand( new List<StatusEffectSO>( new List<StatusEffectSO>() {StatusEffectToCheckAgainst}));
            
            if (TargetType == TargetType.Self)
            {
                dispellCommand.Execute(source, source, ability);
            }
            else
            {
                dispellCommand.Execute(source, target, ability);
            }
        }
    }
}
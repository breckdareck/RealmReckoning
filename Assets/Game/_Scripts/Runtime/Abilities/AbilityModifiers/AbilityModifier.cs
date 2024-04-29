using System;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Scriptables;

namespace Game._Scripts.Runtime.Abilities.AbilityModifiers
{
    [Serializable]
    public abstract class PreExecutionAbilityModifier
    {
        public abstract void ApplyPreEffect(BattleUnit source, BattleUnit target, AbilitySO ability);
    }
    
    [Serializable]
    public abstract class PostExecutionAbilityModifier
    { 
        public abstract void ApplyPostEffect(BattleUnit source, BattleUnit target, AbilitySO ability);

    }
}
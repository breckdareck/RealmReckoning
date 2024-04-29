using System;
using Game._Scripts.Runtime.Enums;

namespace Game._Scripts.Runtime.Abilities.Actions
{
    [Serializable]
    public abstract class AbilityAction
    {
        public ActionType actionType;
        public TargetType targetType;
        public TargetSelectionType targetSelection;
    }
}
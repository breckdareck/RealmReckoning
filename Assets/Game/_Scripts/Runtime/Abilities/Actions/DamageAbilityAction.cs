using Game._Scripts.Runtime.Enums;

namespace Game._Scripts.Runtime.Abilities.Actions
{
    public class DamageAbilityAction : AbilityAction
    {
        public DamageType DamageType;
        public bool CanPierceBarrier;
        public bool CanBeDodged = true;
        public bool CanBeCritHit = true;
        public bool IsGuaranteedCrit;
        public int DamagePercent;
        public int AddedDamagePercent;

        public void AddDamage(int addedDamage)
        {
            AddedDamagePercent += addedDamage;
        }

        public void ResetAddedDamagePercent()
        {
            AddedDamagePercent = 0;
        }
    }
}
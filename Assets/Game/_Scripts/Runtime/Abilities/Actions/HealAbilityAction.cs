namespace Game._Scripts.Runtime.Abilities.Actions
{
    public class HealAbilityAction : AbilityAction
    {
        public int healPercent;
        public int barrierPercent;
        public int addedHealPercent;

        public void AddHealAmount(int addedHeal)
        {
            addedHealPercent += addedHeal;
        }

        public void ResetAddedHealPercent()
        {
            addedHealPercent = 0;
        }
    }
}
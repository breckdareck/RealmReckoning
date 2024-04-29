using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Scriptables;

namespace Game._Scripts.Runtime.Interfaces
{
    public class HealCommand : ICommand
    {
        private readonly int _healAmount;
        private readonly int _barrierAmount;

        public HealCommand(int healAmount, int barrierAmount)
        {
            _healAmount = healAmount;
            _barrierAmount = barrierAmount;
        }

        public void Execute(BattleUnit source, BattleUnit target, AbilitySO ability)
        {
            target.ApplyHeal(_healAmount, _barrierAmount);
        }
    }
}
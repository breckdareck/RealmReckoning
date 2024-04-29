using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Scriptables;

namespace Game._Scripts.Runtime.Interfaces
{
    public interface ICommand
    {
        void Execute(BattleUnit source, BattleUnit target, AbilitySO ability);
    }
}
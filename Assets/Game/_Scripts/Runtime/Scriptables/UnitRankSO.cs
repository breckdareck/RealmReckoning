using Game._Scripts.Runtime.Attributes;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [CreateAssetMenu(fileName = "New Unit Rank", menuName = "Custom/Unit Rank")]
    [ManageableData]
    public class UnitRankSO : ScriptableObject
    {
        public string unitRank;
    }
}
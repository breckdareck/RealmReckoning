using Game._Scripts.Runtime.Attributes;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [CreateAssetMenu(fileName = "New Faction", menuName = "Custom/Faction")]
    [ManageableData]
    public class FactionSO : ScriptableObject
    {
        public string factionName;
    }
}
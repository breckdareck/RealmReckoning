using Game._Scripts.Runtime.Attributes;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [CreateAssetMenu(fileName = "New Unit Tag", menuName = "Custom/Unit Tag")]
    [ManageableData]
    public class UnitTagSO : ScriptableObject
    {
        public string unitTag;
    }
}
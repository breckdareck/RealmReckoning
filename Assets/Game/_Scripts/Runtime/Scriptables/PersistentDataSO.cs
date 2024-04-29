using System.Collections.Generic;
using Game._Scripts.Runtime.Attributes;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [ManageableData]
    public class PersistentDataSO : SerializedScriptableObject
    {
        [SerializeField] public Dictionary<StatType, float> stats;

        public List<GearSlot> GearSlots = new(5);
    }
}
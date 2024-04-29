using System;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Game._Scripts.Runtime.Unit
{
    [Serializable]
    public class GearSlot
    {
        [field: SerializeField] public GearSO Gear { get; private set; }
        [field: SerializeField] public GearSlotType GearSlotType { get; private set; }
        [field: SerializeField] public bool IsEquipped { get; private set; }

        public void SetGearType(GearSlotType slotType)
        {
            GearSlotType = slotType;
        }
    }
}
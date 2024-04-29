using System;
using System.Collections.Generic;
using Game._Scripts.Runtime.Attributes;
using Game._Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.Runtime.Scriptables
{
    [CreateAssetMenu(fileName = "New Gear", menuName = "Custom/Gear")]
    [InlineEditor]
    [ManageableData]
    [Serializable]
    public class GearSO : SerializedScriptableObject
    {
        public string gearName;
        [PreviewField, HideLabel] public Image gearImage;
        public GearSlotType gearSlotType;
        public GearClassType gearClassType;
        public GearTierType gearTierType;
        public Dictionary<StatType, float> gearStats;
    }
}
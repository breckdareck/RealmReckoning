using System;
using System.Collections.Generic;
using Game._Scripts.Runtime.Attributes;
using Game._Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [ManageableData]
    [Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Custom/Status Effect")]
    public class StatusEffectSO : ScriptableObject
    {
        [field: SerializeField] public Sprite StatusEffectIcon { get; private set; }
        [field: SerializeField] public string StatusEffectName { get; private set; }
        [field: SerializeField] public StatusEffectType StatusEffectType { get; private set; }
        [field: SerializeField] public StatusEffectCalculationType StatusEffectCalculationType { get; private set; }
        [field: SerializeField] public bool Duplicatable { get; private set; }
        [field: SerializeField] public bool CanStack { get; private set; }

        [field: SerializeField]
        [field: ShowIf("CanStack")]
        public int MaxStackCount { get; private set; } = 1;

        [field: SerializeField] public bool Dispellable { get; private set; }
        [field: SerializeField] public bool Preventable { get; private set; }

        [field: SerializeField] public List<StatusEffectData> StatusEffectDatas { get; private set; }

    }

    [Serializable]
    public class StatusEffectData
    {
        [field: SerializeField] public StatType StatEffected { get; private set; }
        [field: SerializeField, Unit(Units.Percent)] public int EffectAmountPercent { get; private set; }
    }
}
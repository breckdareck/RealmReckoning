using System;
using System.Collections.Generic;
using Game._Scripts.Runtime.Attributes;
using Game._Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [CreateAssetMenu(fileName = "New Base Stats", menuName = "Custom/Base Stats")]
    [InlineEditor]
    [ManageableData]
    // TODO - Separate this into a Base Stats. Put General Stats on Unit for Battle Stats, Currents, etc..
    public class BaseStatsSO : SerializedScriptableObject
    {
        public Dictionary<StatType, float> generalStats = new();
        public Dictionary<StatLevelUpBonus, float> levelUpBonuses = new();

        public float GetStatValue(Enum stat)
        {
            switch (stat)
            {
                case StatType generalStat:
                    return GetStatValueFromDictionary(generalStats, generalStat);

                case StatLevelUpBonus levelUpBonus:
                    return GetStatValueFromDictionary(levelUpBonuses, levelUpBonus);

                default:
                    Debug.LogError($"No Stat Value found for {stat} on {name}");
                    return 0;
            }
        }

        private float GetStatValueFromDictionary<T>(Dictionary<T, float> dictionary, T key)
        {
            if (dictionary.TryGetValue(key, out var value)) return value;

            Debug.LogError($"No Stat Value found for {key} on {name}");
            return 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game._Scripts.Runtime.Attributes;
using Game._Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [CreateAssetMenu(fileName = "New Unit Data", menuName = "Custom/Unit Data")]
    [InlineEditor]
    [ManageableData]
    [Serializable]
    public class UnitDataSO : SerializedScriptableObject
    {
        [FoldoutGroup("Base Unit Data")] public string unitName;
        [FoldoutGroup("Base Unit Data")] public FactionSO unitFactionSo;
        [FoldoutGroup("Base Unit Data")] public UnitRankSO unitRankSo;
        [FoldoutGroup("Base Unit Data")] public UnitTagSO[] unitTags;
        [SerializeField] [FoldoutGroup("Base Unit Data")]
        public Dictionary<StatType, int> baseUnitStats = new();
        [SerializeField] [FoldoutGroup("Base Unit Data")]
        public Dictionary<StatLevelUpBonus, int> unitLevelUpBonus = new();
        [FoldoutGroup("Base Unit Data")] public AbilitySO[] abilities;

        [Button]
        public void SetupDefaultBaseStats()
        {
            baseUnitStats.Add(StatType.StarRating, 0);
            baseUnitStats.Add(StatType.Experience, 0);
            baseUnitStats.Add(StatType.AdherenceToCommand, unitRankSo != null && unitRankSo.unitRank == "General" ? 0 : 50);
            baseUnitStats.Add(StatType.Leadership, unitRankSo != null && unitRankSo.unitRank == "General" ? 60 : 0);
            baseUnitStats.Add(StatType.Potency, 50);
            baseUnitStats.Add(StatType.Resilience, 20);
            baseUnitStats.Add(StatType.Bloodlust, 0);
            baseUnitStats.Add(StatType.CriticalDamage, 150);
            baseUnitStats.Add(StatType.DefenseNegation, 2);
            baseUnitStats.Add(StatType.PhysicalCriticalChance, 5);
            baseUnitStats.Add(StatType.ArmorPierce, 2);
            baseUnitStats.Add(StatType.PhysicalAccuracy, 0);
            baseUnitStats.Add(StatType.PhysicalDodge, 1);
            baseUnitStats.Add(StatType.PhysicalCriticalAvoidance, 0);
            baseUnitStats.Add(StatType.MagikCriticalChance, 5);
            baseUnitStats.Add(StatType.MagikArmorPierce, 3);
            baseUnitStats.Add(StatType.MagikAccuracy, 0);
            baseUnitStats.Add(StatType.MagikDodge, 1);
            baseUnitStats.Add(StatType.MagikCriticalAvoidance, 0);
        }

        [Button]
        public void SetupAbilities()
        {
            abilities = new AbilitySO[0];
            string path = $"Abilities/{unitName}/";
            var abilityAssets = Resources.LoadAll<AbilitySO>(path);

            Debug.Log(abilityAssets.Length);

            abilities = abilityAssets;
        }

    }
}
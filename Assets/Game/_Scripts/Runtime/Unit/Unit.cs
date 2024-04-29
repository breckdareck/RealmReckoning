using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Scriptables;
using Game._Scripts.Runtime.UnityServices;
using Game._Scripts.Runtime.UnityServices.Save;
using Game._Scripts.Runtime.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game._Scripts.Runtime.Unit
{
    [Serializable]
    public class Unit
    {
        [SerializeField] public UnitDataSO UnitData;
        
        [FoldoutGroup("Current Unit Data"), ShowInInspector] 
        public Dictionary<StatType, int> currentUnitStats = new ();
        
        [FoldoutGroup("Current Unit Data"), ShowInInspector] 
        public Dictionary<AbilitySO, int> currentUnitAbilities = new ();

        [FoldoutGroup("Current Unit Data"), ShowInInspector]
        public List<GearSlot> gearSlots = new (5);
        
        [FoldoutGroup("Current Unit Data"), ShowIf("@this.currentUnitStats != null")]
        public int ExperienceRequiredToLevel => Convert.ToInt32(LevelingConstants.BaseExperience *
                                                                Math.Pow(LevelingConstants.Multiplier,
                                                                    currentUnitStats[StatType.Level] - 1));
        
        public static Unit CreateUnit(UnitDataSO unitData)
        {
            var unit = new Unit() { UnitData = unitData };
            unit.InitializeCurrentStats();
            unit.InitializeCurrentAbilities();
            return unit;
        }
        
        public void InitializeCurrentStats()
        {
            currentUnitStats = new Dictionary<StatType, int>(UnitData.baseUnitStats);
            
            // BaseStat + AddStatPerLevel * (CurrentLevel - 1) * (StarModifier^StarRating)
            var str = SetupAttributeValue(StatType.Strength, (int)UnitData.baseUnitStats[StatType.Level]);
            var agi = SetupAttributeValue(StatType.Agility, (int)UnitData.baseUnitStats[StatType.Level]);
            var mag = SetupAttributeValue(StatType.Magik, (int)UnitData.baseUnitStats[StatType.Level]);
            var armor = SetupAttributeValue(StatType.Armor, (int)UnitData.baseUnitStats[StatType.Level]);
            var magikArmor = SetupAttributeValue(StatType.MagikArmor, (int)UnitData.baseUnitStats[StatType.Level]);

            // Setup Str, Agi, Mag
            currentUnitStats[StatType.Strength] = str;
            currentUnitStats[StatType.Agility] = agi;
            currentUnitStats[StatType.Magik] = mag;

            // Setup Physical and Magikal Offense
            currentUnitStats[StatType.PhysicalOffense] = (int)(str * 2.5f + agi);
            currentUnitStats[StatType.MagikOffense] = (int)(mag * 2.5f + agi);

            // TODO : Add Gear Additives to Health Formula
            
            // Formula = (Str*35.6)+(Agi*11.6)+(Mag*17.5) + Gear additives
            var health = (int)Mathf.Round((float)(str * 35.6 + agi * 11.6 + mag * 17.5));
            currentUnitStats[StatType.Health] = health;

            // Setup Potency and Resilience
            currentUnitStats[StatType.Potency] = UnitData.baseUnitStats[StatType.Potency];
            currentUnitStats[StatType.Resilience] = UnitData.baseUnitStats[StatType.Resilience];

            // Setup Physical Accuracy and Dodge
            currentUnitStats[StatType.PhysicalAccuracy] =
                UnitData.baseUnitStats[StatType.PhysicalAccuracy];
            currentUnitStats[StatType.PhysicalDodge] = UnitData.baseUnitStats[StatType.PhysicalDodge];

            // Setup Magikal Accuracy and Dodge
            currentUnitStats[StatType.MagikAccuracy] = UnitData.baseUnitStats[StatType.MagikAccuracy];
            currentUnitStats[StatType.MagikDodge] = UnitData.baseUnitStats[StatType.MagikDodge];

            // Setup CritDmg and Defense Negation
            currentUnitStats[StatType.CriticalDamage] = UnitData.baseUnitStats[StatType.CriticalDamage];
            currentUnitStats[StatType.DefenseNegation] =
                UnitData.baseUnitStats[StatType.DefenseNegation];

            // Setup PhysicalCritChance, PhysicalCritAvoidance
            currentUnitStats[StatType.PhysicalCriticalChance] =
                UnitData.baseUnitStats[StatType.PhysicalCriticalChance];
            currentUnitStats[StatType.PhysicalCriticalAvoidance] =
                UnitData.baseUnitStats[StatType.PhysicalCriticalAvoidance];

            // Setup Armor Pierce and Magik Armor Pierce
            currentUnitStats[StatType.ArmorPierce] = UnitData.baseUnitStats[StatType.ArmorPierce];
            currentUnitStats[StatType.MagikArmorPierce] =
                UnitData.baseUnitStats[StatType.MagikArmorPierce];

            // Setup Armor and MagArmor
            currentUnitStats[StatType.Armor] = armor;
            currentUnitStats[StatType.MagikArmor] = magikArmor;

            // TODO : Add Gear Additives to Speed Formula
            currentUnitStats[StatType.Speed] = (int)UnitData.baseUnitStats[StatType.Speed];
        }

        private void InitializeCurrentAbilities()
        {
            foreach (var ability in UnitData.abilities)
            {
                currentUnitAbilities.Add(Object.Instantiate(ability),1);
            }
        }
        
        public async Task AddExperience(int expToAdd)
        {
            currentUnitStats[StatType.Experience] += expToAdd;
            CheckIfUnitLeveled();
            await SaveService.SaveUnit(this);
        }
        
        private void CheckIfUnitLeveled()
        {
            while (currentUnitStats[StatType.Experience] >= ExperienceRequiredToLevel)
            {
                currentUnitStats[StatType.Experience] -= ExperienceRequiredToLevel;
                OnUnitLevelUp((int)currentUnitStats[StatType.Level] + 1);
            }
        }
        
        private void OnUnitLevelUp(int newUnitLevel)
        {
            currentUnitStats[StatType.Level] = newUnitLevel;
            UpdateStats();
        }
        
        [Button]
        public void UpdateStats()
        {
            // BaseStat + AddStatPerLevel * (CurrentLevel - 1) * (StarModifier^StarRating)
            var str = SetupAttributeValue(StatType.Strength, (int)currentUnitStats[StatType.Level]);
            var agi = SetupAttributeValue(StatType.Agility, (int)currentUnitStats[StatType.Level]);
            var mag = SetupAttributeValue(StatType.Magik, (int)currentUnitStats[StatType.Level]);
            var armor = SetupAttributeValue(StatType.Armor, (int)currentUnitStats[StatType.Level]);
            var magikArmor = SetupAttributeValue(StatType.MagikArmor, (int)currentUnitStats[StatType.Level]);

            // Setup Str, Agi, Mag
            currentUnitStats[StatType.Strength] = (int)str;
            currentUnitStats[StatType.Agility] = (int)agi;
            currentUnitStats[StatType.Magik] = (int)mag;

            // Setup Physical and Magikal Offense
            currentUnitStats[StatType.PhysicalOffense] = (int)(str * 2.5f + agi);
            currentUnitStats[StatType.MagikOffense] = (int)(mag * 2.5f + agi);

            // Setup Armor and Magik Armor
            currentUnitStats[StatType.Armor] = (int)armor;
            currentUnitStats[StatType.MagikArmor] = (int)magikArmor;

            // TODO : Add Gear Additives to Health Formula
            // Formula = (Str*35.6)+(Agi*11.6)+(Mag*17.5) + Gear additives
            var health = (int)Mathf.Round((float)(str * 35.6 + agi * 11.6 + mag * 17.5));
            currentUnitStats[StatType.Health] = health;

            Debug.Log($"{UnitData.unitName} - Stats were updated");
        }
        
        private int SetupAttributeValue(StatType stat, int unitLevel)
        {
            switch (stat)
            {
                case StatType.Strength:
                    return Mathf.RoundToInt(UnitData.baseUnitStats[StatType.Strength] +
                           UnitData.unitLevelUpBonus[StatLevelUpBonus.StrengthPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, currentUnitStats[StatType.StarRating]));
                case StatType.Agility:
                    return Mathf.RoundToInt(UnitData.baseUnitStats[StatType.Agility] +
                           UnitData.unitLevelUpBonus[StatLevelUpBonus.AgilityPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, currentUnitStats[StatType.StarRating]));
                case StatType.Magik:
                    return Mathf.RoundToInt(UnitData.baseUnitStats[StatType.Magik] +
                           UnitData.unitLevelUpBonus[StatLevelUpBonus.MagikPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, currentUnitStats[StatType.StarRating]));
                case StatType.Armor:
                    return UnitData.baseUnitStats[StatType.Armor] +
                           UnitData.unitLevelUpBonus[StatLevelUpBonus.ArmorPerLevel] * (unitLevel - 1);
                case StatType.MagikArmor:
                    return UnitData.baseUnitStats[StatType.MagikArmor] +
                           UnitData.unitLevelUpBonus[StatLevelUpBonus.MagikArmorPerLevel] * (unitLevel - 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        //Debug
        [Button]
        public void ResetUnitToDefault()
        {
            currentUnitStats[StatType.Experience] = 0;
            currentUnitStats[StatType.StarRating] = 0;
            OnUnitLevelUp(1);
        }
        
    }
}
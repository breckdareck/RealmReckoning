using System.Collections.Generic;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Scriptables;
using UnityEditor;
using UnityEngine;

// ReSharper disable StringLiteralTypo

namespace Game._Scripts.Runtime.Tools
{
#if UNITY_EDITOR

    public static class CsvLoaderTool
    {
        public static void LoadCsv(string csvText)
        {
            var lines = csvText.Split('\n');

            // Assuming the first line contains headers
            var headers = lines[0].Trim().Split(',');

            // Loop through the remaining lines (data)
            for (var i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Trim().Split(',');

                // Create new Scriptable Objects
                var unitData = ScriptableObject.CreateInstance<UnitDataSO>();

                unitData.baseUnitStats = new Dictionary<StatType, int>();
                

                FactionSO factionSo;
                UnitRankSO rankSo;
                var tags = new List<UnitTagSO>();

                // Populate the fields based on the headers and values
                for (var j = 0; j < Mathf.Min(headers.Length, values.Length); j++)
                {
                    var header = headers[j].ToLower();
                    var value = values[j];

                    // Update this part based on your specific headers
                    switch (header)
                    {
                        case "name":
                            unitData.unitName = value;
                            break;
                        case "faction":
                            factionSo = LoadOrCreateScriptableObject<FactionSO>(
                                $"Assets/Resources/UnitFactions/{value}.asset");
                            factionSo.factionName = value;
                            unitData.unitFactionSo = factionSo;
                            break;
                        case "rank":
                            rankSo = LoadOrCreateScriptableObject<UnitRankSO>(
                                $"Assets/Resources/UnitRanks/{value}.asset");
                            rankSo.unitRank = value;
                            unitData.unitRankSo = rankSo;
                            break;
                        case "baselevel":
                            unitData.baseUnitStats.Add(StatType.Level, int.Parse(value));
                            break;
                        case "basestr":
                            unitData.baseUnitStats.Add(StatType.Strength, int.Parse(value));
                            break;
                        case "addstrperlevel":
                            unitData.unitLevelUpBonus.Add(StatLevelUpBonus.StrengthPerLevel, float.Parse(value));
                            break;
                        case "baseagi":
                            unitData.baseUnitStats.Add(StatType.Agility, int.Parse(value));
                            break;
                        case "addagiperlevel":
                            unitData.unitLevelUpBonus.Add(StatLevelUpBonus.AgilityPerLevel, float.Parse(value));
                            break;
                        case "basemag":
                            unitData.baseUnitStats.Add(StatType.Magik, int.Parse(value));
                            break;
                        case "addmagperlevel":
                            unitData.unitLevelUpBonus.Add(StatLevelUpBonus.MagikPerLevel, float.Parse(value));
                            break;
                        case "speed":
                            unitData.baseUnitStats.Add(StatType.Speed, int.Parse(value));
                            break;
                        case "basearmor":
                            unitData.baseUnitStats.Add(StatType.Armor, int.Parse(value));
                            break;
                        case "basemagarmor":
                            unitData.baseUnitStats.Add(StatType.MagikArmor, int.Parse(value));
                            break;
                        case "armoraddedperlevel":
                            unitData.unitLevelUpBonus.Add(StatLevelUpBonus.ArmorPerLevel, float.Parse(value));
                            break;
                        case "magarmoraddedperlevel":
                            unitData.unitLevelUpBonus.Add(StatLevelUpBonus.MagikArmorPerLevel, float.Parse(value));
                            break;
                        default:
                            if (header.StartsWith("tag_"))
                            {
                                if (value is null or "") continue;
                                var tag = LoadOrCreateScriptableObject<UnitTagSO>(
                                    $"Assets/Resources/UnitTags/{value}.asset");
                                tag.unitTag = value;
                                tags.Add(tag);
                            }

                            break;
                    }
                }

                // Save the Scriptable Objects
                unitData.unitTags = tags.ToArray();

                unitData.SetupDefaultBaseStats();
                unitData.SetupAbilities();

                AssetDatabase.CreateAsset(unitData,
                    $"Assets/Resources/UnitData/{unitData.unitName}.asset");
                Debug.Log($"Assets Created: {unitData.unitName}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static T LoadOrCreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            var scriptableObject = AssetDatabase.LoadAssetAtPath<T>(path);

            if (scriptableObject == null)
            {
                scriptableObject = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(scriptableObject, path);
                Debug.Log($"Asset Created: {scriptableObject}");
            }

            return scriptableObject;
        }
    }
#endif
}
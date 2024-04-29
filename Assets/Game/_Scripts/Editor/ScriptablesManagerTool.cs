using System;
using System.Linq;
using Game._Scripts.Runtime.Attributes;
using Game._Scripts.Runtime.Scriptables;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Game._Scripts.Editor
{
    public class ScriptablesManagerTool : OdinMenuEditorWindow
    {
        private static Type[] typesToDisplay =
            TypeCache.GetTypesWithAttribute<ManageableDataAttribute>().OrderBy(m => m.Name).ToArray();

        private Type selectedType;

        [MenuItem("Custom Tools/Scriptables Manager")]
        private static void OpenEditor()
        {
            GetWindow<ScriptablesManagerTool>();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            var factions = Resources.FindObjectsOfTypeAll<FactionSO>();
            var unitData = Resources.FindObjectsOfTypeAll<UnitDataSO>();
            var statusEffectData = Resources.FindObjectsOfTypeAll<StatusEffectSO>();

            foreach (var faction in factions)
            {
                tree.Add($"UnitData/{faction.factionName}", null);
            }

            foreach (var type in typesToDisplay)
            {
                if (type.Name == "UnitDataSO")
                {
                    foreach (var data in unitData)
                            tree.AddAssetAtPath($"UnitData/{data.unitFactionSo.factionName}/{data.unitName}",
                                $"Assets/Resources/UnitData/{data.unitName}.asset");
                }
                else if (type.Name == "StatusEffectSO")
                {
                    foreach (var statusEffect in statusEffectData)
                        tree.AddAssetAtPath($"StatusEffectSO/{statusEffect.StatusEffectType}/{statusEffect.StatusEffectName}", 
                            $"Assets/Resources/StatusEffects/{statusEffect.StatusEffectName}.asset");
                    
                }
                else
                {
                    tree.AddAllAssetsAtPath(type.Name, "Assets/Resources", type, true, true);
                }
            }

            tree.SortMenuItemsByName();

            return tree;
        }
    }
}
#endif
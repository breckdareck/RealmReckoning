using System.Collections.Generic;
using Game._Scripts.Runtime.Abilities.AbilityModifiers;
using Game._Scripts.Runtime.Abilities.Actions;
using Game._Scripts.Runtime.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [ManageableData]
    [CreateAssetMenu(fileName = "New Ability", menuName = "Custom/Ability")]
    public class AbilitySO : SerializedScriptableObject
    {
        [field: TabGroup("General")] [field:SerializeField] public string AbilityName { get; private set; }
        [field: TabGroup("General")] [field: TextArea, SerializeField] public string Description { get; private set; }
        [field: TabGroup("General")] [field:SerializeField] public int CooldownTurns { get; private set; }
        [field: TabGroup("Action")] [field:SerializeField] public AbilityAction BaseAction { get; private set; }
        [field: DictionaryDrawerSettings(KeyLabel = "Ability Level", ValueLabel = "Level Modifiers")] 
        [field: TabGroup("Level Modifiers")] [field:SerializeField] public Dictionary<int, List<PreExecutionAbilityModifier>> PreExecutionModifiers { get; private set; } = new () ;
        [field: TabGroup("Level Modifiers")] [field:SerializeField] public Dictionary<int, List<PostExecutionAbilityModifier>> PostExecutionModifiers { get; private set; } = new ();

    }
}
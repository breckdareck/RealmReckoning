using System.Collections.Generic;
using Game._Scripts.Runtime.Attributes;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [ManageableData]
    [CreateAssetMenu(fileName = "New Mission", menuName = "Custom/New Mission")]
    public class MissionSO : ScriptableObject
    {
        public string missionName;
        public List<UnitDataSO> missionUnitDatas;
    }
}
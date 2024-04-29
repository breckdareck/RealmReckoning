using System.Collections.Generic;
using Game._Scripts.Runtime.Attributes;
using UnityEngine;

namespace Game._Scripts.Runtime.Scriptables
{
    [ManageableData]
    [CreateAssetMenu(fileName = "New MissionDatabase", menuName = "Custom/New MissionDatabase")]
    public class MissionDatabaseSO : ScriptableObject
    {
        public List<MissionSO> missions;

        public MissionSO GetMissionByName(string missionName)
        {
            return missions.Find(x => x.missionName == missionName);
        }
    }
}
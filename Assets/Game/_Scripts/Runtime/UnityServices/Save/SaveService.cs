#define LOCAL_TEST

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Models;
using UnityEngine;

namespace Game._Scripts.Runtime.UnityServices.Save
{
    public static class SaveService
    {
#if LOCAL_TEST
        private static ISaveClient _client = new PlayerPrefClient();
#else
        private static ISaveClient _client = new CloudSaveClient();
#endif

        public static void SetISaveClient(bool isLocalSave)
        {
            if (isLocalSave)
            {
                _client = new PlayerPrefClient();
            }
            else
            {
                _client = new CloudSaveClient();
            }
            Debug.Log($"Using {_client.GetType()}");
        }
        
        public static async Task<UnitSaveData> LoadUnit(string key)
        {
            return await _client.Load<UnitSaveData>(key);
        }
        
        public static async Task<List<UnitSaveData>> LoadUnits(params string[] keys)
        {
            var units = await _client.Load<UnitSaveData>(keys);
            
            return units.ToList();
        }

        public static async Task<List<UnitSaveData>> LoadAllUnits()
        {
            var units = await _client.Load<UnitSaveData>();
            return units.ToList();
        }

        public static async Task SaveUnit(Unit.Unit unit)
        {
            var unitSaveData = new UnitSaveData
            {
                unitLevel = (int)unit.currentUnitStats[StatType.Level],
                starRating = (int)unit.currentUnitStats[StatType.StarRating],
                experience = (int)unit.currentUnitStats[StatType.Experience]
            };
            await _client.Save(unit.UnitData.unitName, unitSaveData);
        }

        public static async Task SaveUnits((string key, object value)[] units)
        {
            await _client.Save(units);
        }
    }
}
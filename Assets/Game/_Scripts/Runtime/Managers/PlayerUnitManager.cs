using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Models;
using Game._Scripts.Runtime.Systems;
using Game._Scripts.Runtime.Unit;
using Game._Scripts.Runtime.UnityServices;
using Game._Scripts.Runtime.UnityServices.Save;
using JetBrains.Annotations;
using UnityEngine;

namespace Game._Scripts.Runtime.Managers
{
    public class PlayerUnitManager : MonoBehaviour
    {
        [SerializeField] private Dictionary<string,UnitSaveData> savedUnits = new ();
        
        [SerializeField] private List<Unit.Unit> playerUnlockedUnits = new ();
        [SerializeField] private List<Unit.Unit> playerTeam = new();
        
        public static PlayerUnitManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            await LoadSavedUnits();
        }
        
        private async void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.S)) 
                SaveUnits();

            if (Input.GetKeyDown(KeyCode.L))
                await LoadSavedUnits();
            
        }
        
        private void SaveUnits()
        {
            var units = new (string key, object value)[playerUnlockedUnits.Count];
            for (int i = 0; i < playerUnlockedUnits.Count; i++)
            {
                units[i].key = playerUnlockedUnits[i].UnitData.unitName;
                units[i].value = new UnitSaveData
                {
                    unitLevel = (int)playerUnlockedUnits[i].currentUnitStats[StatType.Level],
                    starRating = (int)playerUnlockedUnits[i].currentUnitStats[StatType.StarRating],
                    experience = (int)playerUnlockedUnits[i].currentUnitStats[StatType.Experience]
                };
            }

            SaveService.SaveUnits(units);
        }

        private async Task LoadSavedUnits()
        {
            ISet<string> keys = new HashSet<string>();
            foreach (var unitData in ResourceSystem.Instance.UnitDatas)
            {
                keys.Add(unitData.unitName.Replace(" ", "_"));
            }

            var keyArray = keys.ToArray();

            var results = await SaveService.LoadUnits(keyArray);

            savedUnits = new();
            for (int j = 0; j < results.Count; j++)
            {
                if(results[j] != null)
                    savedUnits.Add(keyArray[j], results[j]);
            }
            
            await SetPlayerUnlockedUnits();
        }
        
        private async Task SetPlayerUnlockedUnits()
        {
            if (!savedUnits.Any())
            {
                // TODO - If its a new account only give the few main characters
                Debug.Log("Creating Units for new account");
                foreach (var unitData in ResourceSystem.Instance.UnitDatas)
                {
                    if(unitData.unitName is not ("Aethin Rolk" or "Ceve Rolk" or "Almena Creff"))
                        continue;
                    playerUnlockedUnits.Add(Unit.Unit.CreateUnit(unitData));
                }

                await Task.CompletedTask;
            }
            else
            {
                playerUnlockedUnits = new();
                foreach (var unit in savedUnits)
                {
                    var unlockedUnit = Unit.Unit.CreateUnit(ResourceSystem.Instance.GetUnitData(unit.Key.Replace("_", " ")));
                    unlockedUnit.InitializeCurrentStats(); // Do this First because it sets all the defaults
                    unlockedUnit.currentUnitStats[StatType.Level] = unit.Value.unitLevel;
                    unlockedUnit.currentUnitStats[StatType.Experience] = unit.Value.experience;
                    unlockedUnit.currentUnitStats[StatType.StarRating] = unit.Value.starRating;
                    unlockedUnit.UpdateStats();
                    playerUnlockedUnits.Add(unlockedUnit);
                }

                await Task.CompletedTask;
            }
        }
        
        [ItemCanBeNull]
        public List<Unit.Unit> GetPlayerTeam()
        {
            return playerTeam;
        }

        public bool AddUnitToTeam(Unit.Unit unitToAdd)
        {
            if (playerTeam.Contains(unitToAdd))
                return false;
            
            playerTeam.Add(unitToAdd);
            return true;
        }

        public void RemoveUnitFromTeam(int slotNumber)
        {
            playerTeam.RemoveAt(slotNumber);
        }

        public List<Unit.Unit> GetPlayerUnlockedUnits()
        {
            return playerUnlockedUnits;
        }
        
        

        private  void OnApplicationQuit()
        {
            SaveUnits();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SaveUnits();
        }

        private void OnApplicationPause(bool pauseStatus)
        { 
            SaveUnits();
        }
    }
}
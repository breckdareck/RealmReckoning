using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Runtime.Managers
{
    [Serializable]
    public class EnergyManager : MonoBehaviour
    {
        [SerializeField] private int totalMaxEnergy = 1000;
        [SerializeField] private int regenMaxEnergy = 100;
        [SerializeField] private float regenRateInMinutes = 1;
        [SerializeField] private int regenAmount = 1;
        [ShowInInspector] [ReadOnly] public int CurrentEnergy { get; private set; }

        [ShowInInspector] [DisplayAsString] [ReadOnly]
        private DateTime _lastEnergyUpdateTime;

        [ShowInInspector] [DisplayAsString] [ReadOnly]
        private DateTime _nextEnergyUpdateTime;

        [ShowInInspector] [DisplayAsString] [ReadOnly]
        private TimeSpan _timeSinceLastUpdate;

        [ShowInInspector] [DisplayAsString] [ReadOnly]
        private TimeSpan _toMaxEnergy;

        public DateTime LastEnergyUpdateTime => _lastEnergyUpdateTime;
        public DateTime NextEnergyUpdateTime => _nextEnergyUpdateTime;

        public int TotalMaxEnergy => totalMaxEnergy;

        public static EnergyManager Instance { get; private set; }
        

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

            LoadEnergyData();
        }

        private void Start()
        {
            RegenerateMissedEnergy();
            CalculateToMaxEnergy();
        }

        private void Update()
        {
            MainRegenerationLoop();

            // Update the time to reach max energy every second
            if (Time.time % 1f < Time.deltaTime) // Update every second
                CalculateToMaxEnergy();
        }

        private void RegenerateMissedEnergy()
        {
            // Accumulate missed energy during offline period
            _timeSinceLastUpdate = DateTime.Now - _lastEnergyUpdateTime;
            var missedEnergy = (int)(_timeSinceLastUpdate.TotalMinutes / regenRateInMinutes) * regenAmount;
            if (missedEnergy <= 1) return;
            Debug.Log($"Energy Gained while Offline: {missedEnergy}");
            CurrentEnergy = Mathf.Clamp(CurrentEnergy + missedEnergy, 0, regenMaxEnergy);
            _lastEnergyUpdateTime = DateTime.Now;
            SaveEnergyData();
        }

        private void MainRegenerationLoop()
        {
            // Check for energy regeneration
            if (CurrentEnergy < regenMaxEnergy)
            {
                SetNextEnergyUpdateTime();
                _timeSinceLastUpdate = DateTime.Now - _lastEnergyUpdateTime;

                if (_timeSinceLastUpdate.TotalMinutes >= regenRateInMinutes) RegenerateEnergy();
            }
            else
            {
                _lastEnergyUpdateTime = DateTime.Now;
                SaveEnergyData();
            }
        }

        private void SetNextEnergyUpdateTime()
        {
            _nextEnergyUpdateTime = _lastEnergyUpdateTime.Add(TimeSpan.FromMinutes(regenRateInMinutes));
        }

        private void RegenerateEnergy()
        {
            // Regenerate energy based on the regen rate
            CurrentEnergy = Mathf.Clamp(CurrentEnergy + regenAmount, 0, regenMaxEnergy);
            Debug.Log($"Regenerated {regenAmount} Energy");
            EventManager.Instance.InvokeOnEnergyChanged();
            _lastEnergyUpdateTime = DateTime.Now;
            SaveEnergyData();
        }

        public void UpdateEnergyAmount(int amountToChange, bool allowsEnergyOvercap)
        {
            if (CurrentEnergy + amountToChange < 0)
            {
                Debug.LogWarning("Not Enough Energy To Consume");
                return;
            }

            CurrentEnergy = Mathf.Clamp(CurrentEnergy + amountToChange, 0,
                allowsEnergyOvercap ? totalMaxEnergy : regenMaxEnergy);
            EventManager.Instance.InvokeOnEnergyChanged();
            SaveEnergyData();
        }

        private void CalculateToMaxEnergy()
        {
            // Calculate the time it will take to reach the maximum energy based on the regeneration rate
            var minutesToMaxEnergy = (regenMaxEnergy - CurrentEnergy) / (double)regenAmount * regenRateInMinutes;
            _toMaxEnergy = TimeSpan.FromMinutes(minutesToMaxEnergy);
        }

        private void LoadEnergyData()
        {
            CurrentEnergy = PlayerPrefs.GetInt("CurrentEnergy", regenMaxEnergy);
            var lastUpdateTimeString = PlayerPrefs.GetString("LastEnergyUpdateTime", DateTime.Now.ToString());

            DateTime.TryParse(lastUpdateTimeString, out _lastEnergyUpdateTime);
        }

        private void SaveEnergyData()
        {
            PlayerPrefs.SetInt("CurrentEnergy", CurrentEnergy);
            PlayerPrefs.SetString("LastEnergyUpdateTime", _lastEnergyUpdateTime.ToString());
            PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveEnergyData();
        }
    }
}
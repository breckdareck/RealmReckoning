using System;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Game._Scripts.Runtime.Managers
{
    public sealed class EventManager : MonoBehaviour
    {
        public static EventManager Instance;

        // Battle Event
        public event Action OnStateChangedEvent;
        public event Action<string> OnStepChangedEvent;
        public event Action<AbilitySO> OnAbilitySelectionChangedEvent;
        public event Action<BattleUnit> OnUnitSelectedChangedEvent;

        public event Action<int> OnUnitStatusEffectsDispelledEvent;


        // Energy Events
        public event Action OnEnergyChangedEvent;


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

        #region Battle Events

        public void InvokeOnStateChanged()
        {
            //Debug.Log("Invoking OnStateChanged");
            OnStateChangedEvent?.Invoke();
        }

        public void InvokeOnStepChanged(string text)
        {
            //Debug.Log("Invoking OnStepChanged");
            OnStepChangedEvent?.Invoke(text);
        }

        public void InvokeOnAbilitySelectionChanged(AbilitySO abilitySo)
        {
            //Debug.Log("Invoking OnAbilitySelectionChanged");
            OnAbilitySelectionChangedEvent?.Invoke(abilitySo);
        }

        public void InvokeOnUnitSelectedChanged(BattleUnit battleUnit)
        {
            //Debug.Log("Invoking OnUnitSelectedChanged()");
            OnUnitSelectedChangedEvent?.Invoke(battleUnit);
        }
        
        public void InvokeOnUnitStatusEffectsDispelledEvent(int obj)
        {
            OnUnitStatusEffectsDispelledEvent?.Invoke(obj);
        }

        #endregion


        #region Energy Events

        public void InvokeOnEnergyChanged()
        {
            //Debug.Log("Invoking OnEnergyChanged");
            OnEnergyChangedEvent?.Invoke();
        }

        #endregion

        
    }
}
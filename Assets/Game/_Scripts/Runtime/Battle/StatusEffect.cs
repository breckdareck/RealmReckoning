using System;
using System.Collections;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;

namespace Assets.Game._Scripts.Runtime.Battle
{
    [Serializable]
    public class StatusEffect
    {
        public StatusEffect(StatusEffectSO statusEffectSO)
        {
            StatusEffectSO = statusEffectSO;
        }

        public event Action OnStackCountChangedEvent;
        public event Action OnTurnsEffectedChangedEvent;
        public event Action OnDestroyEvent;

        [field: SerializeField] public StatusEffectSO StatusEffectSO { get; private set; }

        [field: SerializeField] public int StackCount { get; private set; } = 1;
        [field: SerializeField] public bool AppliedThisTurn { get; private set; } = true;
        [field: SerializeField] public int RemainingTurnsEffected { get; private set; } = 1;
        [field: SerializeField] public int TurnsEffected { get; private set; } = 1;


        // TODO - Possibly might need an amount to add.
        public void IncreaseStackCount()
        {
            StackCount = Mathf.Clamp(StackCount + 1, 0, StatusEffectSO.MaxStackCount);
            OnStackCountChangedEvent?.Invoke();
        }

        public void SetTurnsEffected(int newTurnsEffected)
        {
            TurnsEffected = newTurnsEffected;
            OnTurnsEffectedChangedEvent?.Invoke();
        }

        public void ResetTurnsEffected()
        {
            RemainingTurnsEffected = TurnsEffected;
            SetAppliedThisTurn(true);
            OnTurnsEffectedChangedEvent?.Invoke();
        }

        public void SetAppliedThisTurn(bool value)
        {
            AppliedThisTurn = value;
        }

        public void TickDownStatusEffect()
        {
            RemainingTurnsEffected -= 1;
            OnTurnsEffectedChangedEvent?.Invoke();
        }

        public void OnDestroy()
        {
            OnDestroyEvent?.Invoke();
        }

    }
}
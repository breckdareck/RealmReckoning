using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Sirenix.OdinInspector;
using Game._Scripts.Runtime.Scriptables;
using Assets.Game._Scripts.Runtime.Battle;

namespace Game._Scripts.Runtime.UI.Battle
{
    public class UI_StatusEffect : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text stackCountText;
        [SerializeField] private TMP_Text turnsRemainingText;

        [ShowInInspector, ReadOnly] private StatusEffect statusEffect;

        private void OnDestroy()
        {
            statusEffect.OnTurnsEffectedChangedEvent -= UpdateIcon;
            statusEffect.OnStackCountChangedEvent -= UpdateIcon;
            statusEffect.OnDestroyEvent -= DestroyIcon;
        }

        private void UpdateIcon()
        {
            stackCountText.text = statusEffect.StackCount.ToString();
            turnsRemainingText.text = statusEffect.RemainingTurnsEffected.ToString();
        }

        public void SetStatusEffect(StatusEffect statusEffect)
        {
            this.statusEffect = statusEffect;
            icon.sprite = statusEffect.StatusEffectSO.StatusEffectIcon;
            statusEffect.OnTurnsEffectedChangedEvent += UpdateIcon;
            statusEffect.OnStackCountChangedEvent += UpdateIcon;
            statusEffect.OnDestroyEvent += DestroyIcon;
            UpdateIcon();
        }

        private void DestroyIcon()
        {
            Destroy(this.gameObject);
        }

    }
}
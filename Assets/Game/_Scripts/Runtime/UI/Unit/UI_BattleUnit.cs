using System.Collections.Generic;
using System.Globalization;
using Assets.Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Managers;
using Game._Scripts.Runtime.Scriptables;
using Game._Scripts.Runtime.UI.Battle;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Game._Scripts.Runtime.UI.Unit
{
    public class UI_BattleUnit : MonoBehaviour
    {
        [Header("Unit Stats")] 
        [SerializeField] private Image healthImage;
        [SerializeField] private Image barrierImage;

        [SerializeField] private Slider healthSlider;

        [Header("Button/Anims")] 
        [SerializeField] private Button unitButton;

        [SerializeField] private Transform activeUnitAnim;
        [SerializeField] private Transform enemyTargetAnim;

        [Header("Damage Text")] 
        [SerializeField] private DamageText damageText;

        [SerializeField] private Transform playerDamageTextLocation;
        [SerializeField] private Transform enemyDamageTextLocation;

        [Header("Status Effects")]
        [SerializeField] private Transform statusEffectsLocation;
        [SerializeField] private UI_StatusEffect statusEffectIconPrefab;

        [Header("Turn Progress")] 
        [SerializeField] private Slider turnProgressSlider;

        [SerializeField] private List<LookAtConstraint> canvasLookAtContraints;

        private BattleUnit _attachedBattleUnit;

        public void InitializeUI(BattleUnit battleUnit)
        {
            _attachedBattleUnit = battleUnit;
            UpdateHealthUI();
            UpdateBarrierUI();
            SetupLookAtContraints();
            unitButton.onClick.AddListener(() => EventManager.Instance.InvokeOnUnitSelectedChanged(_attachedBattleUnit));
        }

        private void SetupLookAtContraints()
        {
            ConstraintSource source = new ConstraintSource
            {
                sourceTransform = Camera.main.transform.GetChild(0).transform,
                weight = 1,
            };
            
            foreach (var constraint in canvasLookAtContraints)
            {
                constraint.SetSource(0, source);
            }
        }

        public void UpdateHealthUI()
        {
            healthSlider.maxValue = _attachedBattleUnit.Model.MaxHealth;
            healthSlider.value = _attachedBattleUnit.Model.CurrentHealth;

            /*if(healthImage != null)
                healthImage.fillAmount = (float)_attachedBattleUnit.CurrentHealth /
                                     _attachedBattleUnit.MaxHealth;*/
        }

        public void UpdateBarrierUI()
        {
            if(barrierImage != null)
                barrierImage.fillAmount = (float)_attachedBattleUnit.Model.CurrentBarrier /
                                      _attachedBattleUnit.Model.MaxBarrier;
        }

        public void UpdateTurnSliderValue(float value)
        {
            if(turnProgressSlider != null)
                turnProgressSlider.value = value;
        }

        public void SetActiveUnitAnim()
        {
            activeUnitAnim.gameObject.SetActive(!activeUnitAnim.gameObject.activeInHierarchy);
        }

        public void SetEnemyTargetAnim()
        {
            if (enemyTargetAnim != null)
                enemyTargetAnim.gameObject.SetActive(!enemyTargetAnim.gameObject.activeInHierarchy);
        }

        public void CreateDamageText(string damageAmount)
        {
            CreateText(damageAmount, false);
        }

        public void CreateHealText(int healAmount)
        {
            CreateText(healAmount.ToString(), true);
        }
        
        private void CreateText(string text, bool isHeal)
        {
            if(damageText == null) return;
            
            var textLocation = playerDamageTextLocation;
            if (_attachedBattleUnit.Model.IsControlledByAI)
            {
                textLocation = enemyDamageTextLocation;
            }

            var damageTextObj = Instantiate(damageText, textLocation.position, quaternion.identity);
            damageTextObj.transform.SetParent(textLocation);
            damageTextObj.transform.localRotation = Quaternion.identity;
            damageTextObj.SetDamageText(text, isHeal, false);
        }

        public void CreateStatusEffectIcon(StatusEffect statusEffect)
        {
            var statusEffectObj = Instantiate(statusEffectIconPrefab, statusEffectsLocation);
            statusEffectObj.SetStatusEffect(statusEffect);
        }
    }
}
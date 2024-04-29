using System.Collections.Generic;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Managers;
using Game._Scripts.Runtime.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.Runtime.UI.UnitCatalog
{
    public class UI_UnitCollection : MonoBehaviour
    {
        [FoldoutGroup("Buttons")] [SerializeField]
        private Button backButton;

        [FoldoutGroup("Buttons")] [SerializeField]
        private Button gain100ExpButton;

        [FoldoutGroup("Buttons")] [SerializeField]
        private Button gain1000ExpButton;
        
        [FoldoutGroup("Buttons")] [SerializeField]
        private List<Button> teamSlotButtons;
        
        [FoldoutGroup("Buttons")] [SerializeField]
        private Button addUnitToTeamButton;

        [FoldoutGroup("Assets")] [SerializeField]
        private UI_CatalogUnit catalogUnitPrefab;

        [FoldoutGroup("Assets")] [SerializeField]
        private Sprite starOffImage;

        [FoldoutGroup("Assets")] [SerializeField]
        private Sprite starOnImage;

        [FoldoutGroup("Unit View")] [SerializeField]
        private Transform contentBox;

        [FoldoutGroup("Unit View")] [SerializeField]
        private Runtime.Unit.Unit selectedUnit;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text unitNameText;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private List<Image> starImages;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private Slider unitExpSlider;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text levelText;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text healthText;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text armorText;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text magikArmorText;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text offenseText;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text critChanceText;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text critDamageText;

        [FoldoutGroup("Stats Box")] [SerializeField]
        private TMP_Text speedText;

        public static UI_UnitCollection Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            InitializeSlotButtons();
        }

        private void OnEnable()
        {
            InitializeUnitSlots();
        }

        private void OnDisable()
        {
            contentBox.DestroyChildren();
        }
        
        private void InitializeSlotButtons()
        {
            for (int i = 0; i < teamSlotButtons.Count; i++)
            {
                var i1 = i;
                teamSlotButtons[i].onClick.AddListener(delegate { RemoveUnitFromTeamSlot(i1); });
            }
        }
        
        private void InitializeUnitSlots()
        {
            foreach (var unitData in PlayerUnitManager.Instance.GetPlayerUnlockedUnits())
            {
                var unit = Instantiate(catalogUnitPrefab, contentBox);
                unit.SetAttachedUnitData(unitData);
                unit.InitializeUI();
            }
            
            ResetTeamSlotUI();

            UpdateSelectedUnit(contentBox.GetChild(0).GetComponent<UI_CatalogUnit>().AttachedUnitData);
        }

        public void AddSelectedUnitToTeamSlot()
        {
            var wasAdded = PlayerUnitManager.Instance.AddUnitToTeam(selectedUnit);
            if (!wasAdded) return;
            foreach (var teamSlot in teamSlotButtons)
            {
                var uiTeamSlot = teamSlot.gameObject.GetComponent<UI_TeamSlot>();
                if (uiTeamSlot.AttachedUnitData == null || uiTeamSlot.AttachedUnitData.UnitData == null)
                {
                    uiTeamSlot.SetAttachedUnitData(selectedUnit);
                    uiTeamSlot.InitializeUI();
                    return;
                }
            }
        }
        
        private void RemoveUnitFromTeamSlot(int slotNumber)
        {
            if (slotNumber+1 <= PlayerUnitManager.Instance.GetPlayerTeam().Count)
            {
                PlayerUnitManager.Instance.RemoveUnitFromTeam(slotNumber);
                ResetTeamSlotUI();
            }
            
        }

        private void ResetTeamSlotUI()
        {
            foreach (var teamSlot in teamSlotButtons)
            {
                var uiTeamSlot = teamSlot.gameObject.GetComponent<UI_TeamSlot>();
                uiTeamSlot.SetAttachedUnitData(null);

                if (teamSlotButtons.IndexOf(teamSlot)+1 > PlayerUnitManager.Instance.GetPlayerTeam().Count)
                {
                    uiTeamSlot.EnableDisableUI(false);
                }
                else
                {
                    uiTeamSlot.SetAttachedUnitData(PlayerUnitManager.Instance.GetPlayerTeam()[teamSlotButtons.IndexOf(teamSlot)]);
                    uiTeamSlot.InitializeUI();
                }
            }
        }

        public void UpdateSelectedUnit(Runtime.Unit.Unit newSelectedUnit)
        {
            selectedUnit = newSelectedUnit;
            UpdateStatsScreen();
        }

        private void UpdateStatsScreen()
        {
            UpdateStatScreenTexts();
            UpdateUnitExperienceSlider();
            UpdateStarImages();
        }

        private void UpdateUnitExperienceSlider()
        {
            unitExpSlider.maxValue = selectedUnit.ExperienceRequiredToLevel;
            unitExpSlider.value = (int)selectedUnit.currentUnitStats[StatType.Experience];
        }

        private void UpdateStarImages()
        {
            for (var i = starImages.Count; i > 0; i--)
                // Count of 10, if starrating is 0 i > starrating i--
                starImages[i - 1].sprite = selectedUnit.currentUnitStats[StatType.StarRating] > starImages.Count - i
                    ? starOnImage
                    : starOffImage;
        }

        private void UpdateStatScreenTexts()
        {
            unitNameText.text = selectedUnit.UnitData.unitName;
            levelText.text = selectedUnit.currentUnitStats[StatType.Level].ToString();
            healthText.text = selectedUnit.currentUnitStats[StatType.Health].ToString();
            armorText.text = selectedUnit.currentUnitStats[StatType.Armor].ToString();
            magikArmorText.text = selectedUnit.currentUnitStats[StatType.MagikArmor].ToString();
            offenseText.text = $"{((int)selectedUnit.currentUnitStats[StatType.PhysicalOffense]).ToString()}";
            critChanceText.text = $"{selectedUnit.currentUnitStats[StatType.PhysicalCriticalChance].ToString()}%";
            critDamageText.text = selectedUnit.currentUnitStats[StatType.CriticalDamage].ToString();
            speedText.text = selectedUnit.currentUnitStats[StatType.Speed].ToString();
        }

        public void CloseUnitCollectionMenu()
        {
            gameObject.SetActive(false);
        }

        public void OnDebugGainExpButtonPressed(int expToGive)
        {
            _ = selectedUnit.AddExperience(expToGive);
            UpdateStatsScreen();
        }

        public void OnDebugResetUnitToDefault()
        {
            selectedUnit.ResetUnitToDefault();
        }
    }
}
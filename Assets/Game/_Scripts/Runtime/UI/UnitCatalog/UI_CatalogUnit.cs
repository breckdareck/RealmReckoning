using System.Collections.Generic;
using Game._Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.Runtime.UI.UnitCatalog
{
    public class UI_CatalogUnit : MonoBehaviour
    {
        [FoldoutGroup("Assets")] [SerializeField]
        private Sprite starOffImage;

        [FoldoutGroup("Assets")] [SerializeField]
        private Sprite starOnImage;

        [SerializeField] private Button unitButton;
        [SerializeField] private Image unitImage;
        [SerializeField] private TMP_Text unitNameText;
        [SerializeField] private List<Image> starImages;

        [SerializeField] private Runtime.Unit.Unit attachedUnitData;
        public Runtime.Unit.Unit AttachedUnitData => attachedUnitData;

        private void OnEnable()
        {
            unitButton.onClick.AddListener(
                delegate { UI_UnitCollection.Instance.UpdateSelectedUnit(attachedUnitData); });
        }

        private void OnDisable()
        {
            unitButton.onClick.RemoveAllListeners();
        }

        public void InitializeUI()
        {
            unitNameText.text = attachedUnitData.UnitData.unitName;
            for (var i = starImages.Count; i > 0; i--)
                // Count of 10, if starrating is 0 i > starrating i--
                starImages[i - 1].sprite =
                    attachedUnitData.currentUnitStats[StatType.StarRating] > starImages.Count - i
                        ? starOnImage
                        : starOffImage;
        }

        public void SetAttachedUnitData(Runtime.Unit.Unit newUnitData)
        {
            attachedUnitData = newUnitData;
        }
    }
}
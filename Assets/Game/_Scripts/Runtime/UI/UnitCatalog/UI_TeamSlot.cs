using System.Collections.Generic;
using Game._Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.Runtime.UI.UnitCatalog
{
    public class UI_TeamSlot : MonoBehaviour
    {
        [FoldoutGroup("Assets")] [SerializeField]
        private Sprite starOffImage;

        [FoldoutGroup("Assets")] [SerializeField]
        private Sprite starOnImage;

        [SerializeField] private Button slotButton;
        [SerializeField] private Image unitImage;
        [SerializeField] private TMP_Text unitNameText;
        [SerializeField] private List<Image> starImages;

        [SerializeField] private Runtime.Unit.Unit attachedUnitData = null;
        public Runtime.Unit.Unit AttachedUnitData => attachedUnitData;
    
        public void InitializeUI()
        {
            EnableDisableUI(true);
        
            unitNameText.text = attachedUnitData.UnitData.unitName;
            for (var i = starImages.Count; i > 0; i--)
                // Count of 10, if starrating is 0 i > starrating i--
                starImages[i - 1].sprite =
                    attachedUnitData.currentUnitStats[StatType.StarRating] > starImages.Count - i
                        ? starOnImage
                        : starOffImage;
        }

        public void EnableDisableUI(bool value)
        {
            unitImage.enabled = value ;
            unitNameText.enabled  = value;
            foreach (var starImage in starImages)
            {
                starImage.enabled = value;
            }
        }

        public void SetAttachedUnitData(Runtime.Unit.Unit newUnitData)
        {
            attachedUnitData = newUnitData;
        }
    }
}

using System.Linq;
using Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Managers;
using Game._Scripts.Runtime.Scriptables;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Scripts.Runtime.UI.Battle
{
    public class UI_Battle : MonoBehaviour
    {
        [SerializeField] private TMP_Text currentStateText;
        
        [SerializeField] private Button endBattleButton;
        
        [SerializeField] private TMP_Text nextStepText;
        
        [SerializeField] private Button[] abilityButtons;

        [SerializeField] private TMP_Text debugUnitStatsText;


        public static UI_Battle Instance { get; private set; }


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            endBattleButton.onClick.AddListener(
                () => BattleSystem.Instance.BattleStateMachine.SetState(BattleState.End));
        }
        
        private void OnEnable()
        {
            EventManager.Instance.OnStateChangedEvent += UpdateStateText;
            EventManager.Instance.OnStateChangedEvent += UpdateDebugUnitStatsText;

            EventManager.Instance.OnStepChangedEvent += UpdateStepText;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnStateChangedEvent -= UpdateStateText;
            EventManager.Instance.OnStateChangedEvent -= UpdateDebugUnitStatsText;

            EventManager.Instance.OnStepChangedEvent -= UpdateStepText;
        }

        private void UpdateStateText()
        {
            currentStateText.text =
                $"Current State: {BattleSystem.Instance.BattleStateMachine.CurrentState.ToString()}";
        }
        
        private void UpdateStepText(string text)
        {
            nextStepText.text = $"Next Step: {text}";
        }
        
        private void UpdateDebugUnitStatsText()
        {

                var unit = BattleSystem.Instance.BattleStateMachine.GetActiveUnit();

                var sortedStats = unit.Model.Unit.currentUnitStats.OrderBy(x => x.Key.ToString()).ToDictionary(x => x.Key, x => x.Value);

            debugUnitStatsText.text =
                $"{BattleSystem.Instance.BattleStateMachine.GetActiveUnit().name} Persistent Stats + Bonus Battle Stats = Current \n";
            sortedStats
                .ForEach(x =>
                    debugUnitStatsText.text +=
                        $"{x.Key} : {x.Value} + {(unit.Model.BattleBonusStats.TryGetValue(x.Key, out var stat) ? stat : 0)} = {(unit.Model.CurrentBattleStats.TryGetValue(x.Key, out var value) ? value : 0)} \n");
        }
        
        public void SetupAbilityButton(AbilitySO abilitySo, int buttonIndex)
        {
            abilityButtons[buttonIndex].gameObject.SetActive(abilitySo != null);

            abilityButtons[buttonIndex].onClick.RemoveAllListeners();
            abilityButtons[buttonIndex].onClick.AddListener(() => OnAbilityButtonClick(abilitySo));
            abilityButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text =
                abilitySo != null ? abilitySo.AbilityName : "";
        }
        
        private void OnAbilityButtonClick(AbilitySO abilitySo)
        {
            EventManager.Instance.InvokeOnAbilitySelectionChanged(abilitySo);
        }
    }
}
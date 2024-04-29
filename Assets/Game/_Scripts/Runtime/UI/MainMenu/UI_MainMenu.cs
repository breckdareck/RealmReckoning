using System;
using System.Collections;
using Game._Scripts.Runtime.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game._Scripts.Runtime.UI.MainMenu
{
    public class UI_MainMenu : MonoBehaviour
    {
        public TMP_Text currentTimeText;
        public TMP_Text lastUpdateTimeText;
        public TMP_Text nextUpdateTimeText;
        public TMP_Text energyText;

        public Button use10Button;
        public Button gain10Button;
        public Button gain10OvercapButton;
        public Button unitCollectionButton;

        public Canvas UnitCatalogCanvas;

        public static UI_MainMenu Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            use10Button.onClick.AddListener(delegate { EnergyManager.Instance.UpdateEnergyAmount(-10, true); });
            gain10Button.onClick.AddListener(delegate { EnergyManager.Instance.UpdateEnergyAmount(10, false); });
            gain10OvercapButton.onClick.AddListener(delegate { EnergyManager.Instance.UpdateEnergyAmount(10, true); });

            // Initialize the UI with the current stamina value
            UpdateEnergyText();
        }

        private void OnEnable()
        {
            EventManager.Instance.OnEnergyChangedEvent += UpdateEnergyText;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnEnergyChangedEvent -= UpdateEnergyText;
        }

        private void Update()
        {
            currentTimeText.text = $"Current Time: {DateTime.Now.ToString()}";
            lastUpdateTimeText.text = $"Last Update: {EnergyManager.Instance.LastEnergyUpdateTime.ToString()}";
            nextUpdateTimeText.text = $"Next Update: {EnergyManager.Instance.NextEnergyUpdateTime.ToString()}";
        }

        private void UpdateEnergyText()
        {
            energyText.text = $"{EnergyManager.Instance.CurrentEnergy}/{EnergyManager.Instance.TotalMaxEnergy}";
        }

        public void OpenUnitCollectionMenu()
        {
            if (!UnitCatalogCanvas.gameObject.activeInHierarchy) UnitCatalogCanvas.gameObject.SetActive(true);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // Start loading the scene asynchronously
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            // Wait until the scene is fully loaded
            while (!asyncOperation.isDone)
            {
                var progress = Mathf.Clamp01(asyncOperation.progress / 0.9f); // 0.9 is the completion threshold
                Debug.Log($"Loading progress: {progress * 100}%");

                // You can update a loading bar or display progress here

                yield return null; // Wait for the next frame
            }

            // Scene is fully loaded
            Debug.Log("Scene loaded successfully");

            // Check if the loaded scene is the Battle Scene
            if (sceneName == "BattleScene") Debug.Log("BattleScene Loaded: Initializing Battle");
        }
    }
}
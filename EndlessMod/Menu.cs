using TDTN.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirthos.EndlessMod
{
    public class Menu
    {
        public static float Diamond
        {
            get => PlayerPrefs.GetFloat("EndlessMod_Diamond", 0f);
            set
            {
                PlayerPrefs.SetFloat("EndlessMod_Diamond", value);
                UpdateDiamondText();
            }
        }

        private static TextMeshProUGUI _diamondCountText;
        private Button _startButton;
        
        public void Init()
        {
            Transform rootPanel = GameObject.Find("/EndlessModMenu/Panel").transform;
            _diamondCountText = rootPanel.Find("DiamondText").GetComponent<TextMeshProUGUI>();
            _startButton = rootPanel.Find("Start").GetComponent<Button>();
            _startButton.onClick.AddListener(OnClickStart);
            rootPanel.Find("Reset").GetComponent<Button>().onClick.AddListener(OnClickReset);
            rootPanel.Find("Back").GetComponent<Button>().onClick.AddListener(OnClickBack);
            UpdateDiamondText();
            UpgradeManager upgradeManager = new UpgradeManager();
            upgradeManager.Init(rootPanel);
        }
        
        private static void UpdateDiamondText()
        {
            if (_diamondCountText == null) return;
            _diamondCountText.text = "Diamond: " + Mathf.FloorToInt(Diamond);
        }

        private void OnClickStart()
        {
            SaveManager.ClearLevelDatas();
            GameObject levelObject = new GameObject("EndlessLevelManager");
            EndlessLevel endlessLevel = levelObject.AddComponent<EndlessLevel>();
            Object.DontDestroyOnLoad(levelObject);
            endlessLevel.LoadLevel();
        }

        private void OnClickBack()
        {
            GameManager.Instance.LoadGameMenu();
        }

        private void OnClickReset()
        {
            Diamond = 0f;
            UpgradeManager.ClearSave();
            UpdateDiamondText();
        }
    }
}
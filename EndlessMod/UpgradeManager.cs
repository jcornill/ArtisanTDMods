using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirthos.EndlessMod
{
    public class UpgradeManager
    {
        public static bool HasArbalete
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Arbalete", 0) == 1;
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Arbalete", value ? 1 : 0);
        }

        public static bool HasGel
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Gel", 0) == 1;
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Gel", value ? 1 : 0);
        }

        public static bool HasFlechette
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Flechette", 0) == 1;
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Flechette", value ? 1 : 0);
        }

        public static bool HasCatapulte
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Catapulte", 0) == 1;
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Catapulte", value ? 1 : 0);
        }

        public static bool HasBlizzard
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Blizzard", 0) == 1;
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Blizzard", value ? 1 : 0);
        }

        public static bool HasPieux
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Pieux", 0) == 1;
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Pieux", value ? 1 : 0);
        }

        public static bool HasBaliste
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Baliste", 0) == 1;
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Baliste", value ? 1 : 0);
        }

        public static bool HasRayon
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Rayon", 0) == 1;
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Rayon", value ? 1 : 0);
        }

        public static int MapUp
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_MapUp", 0);
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_MapUp", value);
        }

        public static int MapLeft
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_MapLeft", 0);
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_MapLeft", value);
        }

        public static int MapDown
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_MapDown", 0);
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_MapDown", value);
        }

        public static int MapRight
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_MapRight", 0);
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_MapRight", value);
        }

        private static int mapUpgrades => MapDown + MapRight + MapLeft + MapUp;

        private static int mapUpgradeCost
        {
            get
            {
                int result = 5;
                for (int i = 0; i < mapUpgrades; i++)
                {
                    result *= 2;
                }

                return result;
            }
        }
        
        public static int StartingGoldUpgrade
        {
            get => PlayerPrefs.GetInt("EndlessMod_Upgrade_Gold", 0);
            set => PlayerPrefs.SetInt("EndlessMod_Upgrade_Gold", value);
        }

        public static int GoldStart => 50 + StartingGoldUpgrade * 5;
        
        public static void ClearSave()
        {
            HasArbalete = false;
            HasGel = false;
            HasFlechette = false;
            HasCatapulte = false;
            HasBlizzard = false;
            HasPieux = false;
            HasBaliste = false;
            HasRayon = false;
            MapDown = 0;
            MapUp = 0;
            MapLeft = 0;
            MapRight = 0;
            StartingGoldUpgrade = 0;
            UpdateButtons();
        }

        private static Button buyArbalete;
        private static Button buyGel;
        private static Button buyFlechette;
        private static Button buyCatapulte;
        private static Button buyBlizzard;
        private static Button buyPieux;
        private static Button buyBaliste;
        private static Button buyRayon;

        private static TextMeshProUGUI buyMapCostText;
        private static Button buyMapTop;
        private static Button buyMapDown;
        private static Button buyMapLeft;
        private static Button buyMapRight;
        
        private static TextMeshProUGUI startingGoldText;
        private static Button buyStartingGold;

        public void Init(Transform rootPanel)
        {
            buyArbalete = rootPanel.Find("UnlockArbalete").GetComponent<Button>();
            buyGel = rootPanel.Find("UnlockGel").GetComponent<Button>();
            buyFlechette = rootPanel.Find("UnlockFlechette").GetComponent<Button>();
            buyCatapulte = rootPanel.Find("UnlockCatapulte").GetComponent<Button>();
            buyBlizzard = rootPanel.Find("UnlockBlizzard").GetComponent<Button>();
            buyPieux = rootPanel.Find("UnlockPieux").GetComponent<Button>();
            buyBaliste = rootPanel.Find("UnlockBaliste").GetComponent<Button>();
            buyRayon = rootPanel.Find("UnlockRayon").GetComponent<Button>();

            buyArbalete.onClick.AddListener(OnClickBuyArbalete);
            buyGel.onClick.AddListener(OnClickBuyGel);
            buyFlechette.onClick.AddListener(OnClickBuyFlechette);
            buyCatapulte.onClick.AddListener(OnClickBuyCatapulte);
            buyBlizzard.onClick.AddListener(OnClickBuyBlizzard);
            buyPieux.onClick.AddListener(OnClickBuyPieux);
            buyBaliste.onClick.AddListener(OnClickBuyBaliste);
            buyRayon.onClick.AddListener(OnClickBuyRayon);

            buyMapLeft = rootPanel.Find("UpgradeLeft").GetComponent<Button>();
            buyMapTop = rootPanel.Find("UpgradeTop").GetComponent<Button>();
            buyMapRight = rootPanel.Find("UpgradeRight").GetComponent<Button>();
            buyMapDown = rootPanel.Find("UpgradeDown").GetComponent<Button>();
            buyMapCostText = rootPanel.Find("UpgradeSizeText").GetComponent<TextMeshProUGUI>();

            buyMapLeft.onClick.AddListener(OnClickUpgradeMapLeft);
            buyMapTop.onClick.AddListener(OnClickUpgradeMapUp);
            buyMapRight.onClick.AddListener(OnClickUpgradeMapRight);
            buyMapDown.onClick.AddListener(OnClickUpgradeMapDown);

            buyStartingGold = rootPanel.Find("BuyStartingGold").GetComponent<Button>();
            startingGoldText = rootPanel.Find("StartingGoldText").GetComponent<TextMeshProUGUI>();
            
            buyStartingGold.onClick.AddListener(OnClickBuyStartingGold);
            
            UpdateButtons();
        }

        private static void UpdateButtons()
        {
            float diamond = Menu.Diamond;

            buyArbalete.gameObject.SetActive(!HasArbalete);
            buyGel.gameObject.SetActive(!HasGel);
            buyFlechette.gameObject.SetActive(!HasFlechette);
            buyCatapulte.gameObject.SetActive(!HasCatapulte);
            buyBlizzard.gameObject.SetActive(!HasBlizzard);
            buyPieux.gameObject.SetActive(!HasPieux);
            buyBaliste.gameObject.SetActive(!HasBaliste);
            buyRayon.gameObject.SetActive(!HasRayon);

            buyArbalete.interactable = diamond >= 2;
            buyGel.interactable = diamond >= 5;
            buyFlechette.interactable = diamond >= 10;
            buyCatapulte.interactable = diamond >= 50;
            buyBlizzard.interactable = diamond >= 75;
            buyPieux.interactable = diamond >= 100;
            buyBaliste.interactable = diamond >= 150;
            buyRayon.interactable = diamond >= 500;
            
            buyMapCostText.text = $"Upgrade map size.\nCost: {mapUpgradeCost} Diamond";
            buyMapLeft.interactable = diamond >= mapUpgradeCost;
            buyMapTop.interactable = diamond >= mapUpgradeCost;
            buyMapRight.interactable = diamond >= mapUpgradeCost;
            buyMapDown.interactable = diamond >= mapUpgradeCost;

            startingGoldText.text = $"Increase starting gold by 5\nCurrent gold at start: {GoldStart}\nCost: {5 + StartingGoldUpgrade * 5} diamond";
            buyStartingGold.interactable = diamond >= 5 + StartingGoldUpgrade * 5;
        }

        private void OnClickBuyStartingGold()
        {
            Menu.Diamond -= 5 + StartingGoldUpgrade * 5;
            StartingGoldUpgrade++;
            UpdateButtons();
        }

        private void OnClickUpgradeMapLeft()
        {
            Menu.Diamond -= mapUpgradeCost;
            MapLeft++;
            UpdateButtons();
        }
        
        private void OnClickUpgradeMapUp()
        {
            Menu.Diamond -= mapUpgradeCost;
            MapUp++;
            UpdateButtons();
        }
        
        private void OnClickUpgradeMapRight()
        {
            Menu.Diamond -= mapUpgradeCost;
            MapRight++;
            UpdateButtons();
        }
        
        private void OnClickUpgradeMapDown()
        {
            Menu.Diamond -= mapUpgradeCost;
            MapDown++;
            UpdateButtons();
        }

        private void OnClickBuyArbalete()
        {
            Menu.Diamond -= 2;
            HasArbalete = true;
            UpdateButtons();
        }

        private void OnClickBuyGel()
        {
            Menu.Diamond -= 5;
            HasGel = true;
            UpdateButtons();
        }

        private void OnClickBuyFlechette()
        {
            Menu.Diamond -= 10;
            HasFlechette = true;
            UpdateButtons();
        }

        private void OnClickBuyCatapulte()
        {
            Menu.Diamond -= 50;
            HasCatapulte = true;
            UpdateButtons();
        }

        private void OnClickBuyBlizzard()
        {
            Menu.Diamond -= 75;
            HasBlizzard = true;
            UpdateButtons();
        }

        private void OnClickBuyPieux()
        {
            Menu.Diamond -= 100;
            HasPieux = true;
            UpdateButtons();
        }

        private void OnClickBuyBaliste()
        {
            Menu.Diamond -= 150;
            HasBaliste = true;
            UpdateButtons();
        }

        private void OnClickBuyRayon()
        {
            Menu.Diamond -= 500;
            HasRayon = true;
            UpdateButtons();
        }
    }
}
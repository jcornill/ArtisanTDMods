using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirthos.ArtisanTDModLoader
{
    public class CustomGameMenu
    {
        private Transform PopupsContainer;
        private Transform VictoryPopupContent;
        private Transform PausePopupContent;
        private Transform DefeatPopupContent;

        private TextMeshProUGUI VictoryPopupTitle;
        private TextMeshProUGUI PausePopupTitle;
        private TextMeshProUGUI DefeatPopupTitle;
        
        private Transform VictoryButtonRewind;
        private Transform VictoryButtonMenu;
        
        private Transform PauseButtonRewind;
        private Transform PauseButtonMenu;
        
        private Transform DefeatButtonRewind;
        private Transform DefeatButtonMenu;

        private string PopupTitle;

        public CustomGameMenu()
        {
            PopupsContainer = GameObject.Find("LevelManager")?.transform.Find("LevelMenu").Find("Popups");
            VictoryPopupContent = PopupsContainer.Find("Popup_Victory").Find("Panel").Find("Content");
            PausePopupContent = PopupsContainer.Find("Popup_Pause").Find("Panel").Find("Content");
            DefeatPopupContent = PopupsContainer.Find("Popup_Defeat").Find("Panel").Find("Content");
            
            VictoryPopupTitle = PopupsContainer.Find("Popup_Victory").Find("Panel").Find("Background").Find("Header").GetComponentInChildren<TextMeshProUGUI>();
            PausePopupTitle = PopupsContainer.Find("Popup_Pause").Find("Panel").Find("Background").Find("Header").GetComponentInChildren<TextMeshProUGUI>();
            DefeatPopupTitle = PopupsContainer.Find("Popup_Defeat").Find("Panel").Find("Background").Find("Header").GetComponentInChildren<TextMeshProUGUI>();

            VictoryButtonRewind = VictoryPopupContent.Find("Buttons").Find("Button_Rewind-Carousel");
            VictoryButtonMenu = VictoryPopupContent.Find("Buttons").Find("Button_MapMenu");
            
            PauseButtonRewind = PausePopupContent.Find("Buttons").Find("Button_Rewind-Carousel");
            PauseButtonMenu = PausePopupContent.Find("Buttons").Find("Button_MapMenu");
            
            DefeatButtonRewind = DefeatPopupContent.Find("Buttons").Find("Button_Rewind-Carousel");
            DefeatButtonMenu = DefeatPopupContent.Find("Buttons").Find("Button_MapMenu");
        }
        
        public void SetPopupsTitle(string newTitle)
        {
            PopupTitle = newTitle;
            VictoryPopupTitle.GetComponent<Localize>().enabled = false;
            VictoryPopupTitle.text = newTitle;
            PausePopupTitle.GetComponent<Localize>().enabled = false;
            PausePopupTitle.text = newTitle;
            DefeatPopupTitle.GetComponent<Localize>().enabled = false;
            DefeatPopupTitle.text = newTitle;
        }

        public void ToggleButtonRewind(bool enabled)
        {
            DefeatButtonRewind.gameObject.SetActive(enabled);
            PauseButtonRewind.gameObject.SetActive(enabled);
            VictoryButtonRewind.gameObject.SetActive(enabled);
        }
        
        public void UpdateButtonRewind(string text, Action onClick)
        {
            
        }
        
        public void UpdateButtonCarte(string text, Action onClick)
        {
            if (text != null)
            {
                DefeatButtonMenu.GetComponentInChildren<TextMeshProUGUI>().text = text;
                PauseButtonMenu.GetComponentInChildren<TextMeshProUGUI>().text = text;
                VictoryButtonMenu.GetComponentInChildren<TextMeshProUGUI>().text = text;
            }

            DefeatButtonMenu.GetComponent<Button>().onClick.AddListener(() => { onClick?.Invoke(); });
            PauseButtonMenu.GetComponent<Button>().onClick.AddListener(() => { onClick?.Invoke(); });
            VictoryButtonMenu.GetComponent<Button>().onClick.AddListener(() => { onClick?.Invoke(); });
        }
    }
}
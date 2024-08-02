using System;
using Kirthos.ArtisanTDModLoader;
using TDTN.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Kirthos.EndlessMod
{
    public class EndlessMod : ModBase
    {
        public override string DisplayName => "Endless Mod";
        public override string Version => "v0.1";
        public override string Creator => "Kirthos";

        private void Start()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            EventHandlerData.OnMapMenuLoaded += OnMapMenuLoaded;
        }

        private void OnMapMenuLoaded()
        {
            BundleSceneLoader.LoadSceneFromModBundle("EndlessMod", "Map", () =>
            {
                // get the button then add event on click to load the endless menu scene
                Button endlessModButton = GameObject.Find("/EndlessModMap/Button").GetComponent<Button>();
                endlessModButton.onClick.AddListener(OnClickEndlessMod);
            });
        }

        private void OnClickEndlessMod()
        {
            BundleSceneLoader.LoadSceneFromModBundle("EndlessMod", "Menu", () =>
            {
                Menu modMenu = new Menu();
                modMenu.Init();
            });
        }
    }
}
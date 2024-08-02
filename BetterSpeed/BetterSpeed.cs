using System.Linq;
using System.Reflection;
using Kirthos.ArtisanTDModLoader;
using TDTN.DataResources;
using TDTN.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Kirthos.BetterSpeed
{
    public class BetterSpeed : ModBase
    {
        private ScriptableIntValue _speedRef;
        private Button _x3;
        private Button _x5;

        private void Start()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            EventHandlerData.OnLevelLoaded += OnLevelLoaded;
        }

        private void Update()
        {
            if (_x3 == null || _x5 == null) return;
            bool disabled = LevelManager.Instance.CurrentWaveState == LevelManager.WaveState.Disable;
            _x3.interactable = !disabled;
            _x5.interactable = !disabled;
            if (_speedRef == null) return;
            if (_speedRef.Value == 3)
                _x3.interactable = false;
            else if (_speedRef.Value == 5)
                _x5.interactable = false;
        }

        private void OnLevelLoaded(string levelName)
        {
            BundleSceneLoader.LoadSceneFromModBundle("BetterSpeed", "BetterSpeed", () =>
            {
                GameObject buttonX3 = GameObject.Find("/BetterSpeed/x3");
                GameObject buttonX5 = GameObject.Find("/BetterSpeed/x5");
                _x3 = buttonX3.GetComponent<Button>();
                _x3.onClick.AddListener(Speedx3);
                _x5 = buttonX5.GetComponent<Button>();
                _x5.onClick.AddListener(Speedx5);
                UpdateSpeedRef();
            });
        }

        private void UpdateSpeedRef()
        {
            _speedRef = typeof(LevelManager).GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance).FirstOrDefault(x => x.Name == "gameSpeedRef").GetValue(LevelManager.Instance) as ScriptableIntValue;
        }

        private void Speedx3()
        {
            if (LevelManager.Instance.CurrentWaveState == LevelManager.WaveState.Disable) return;
            _speedRef.SetValue(3);
            Debug.Log("Speedx3");
        }

        private void Speedx5()
        {
            if (LevelManager.Instance.CurrentWaveState == LevelManager.WaveState.Disable) return;
            _speedRef.SetValue(5);
            Debug.Log("Speedx5");
        }

        public override string DisplayName => "More Speed";
        public override string Version => "v0.1";
        public override string Creator => "Kirthos";
    }
}
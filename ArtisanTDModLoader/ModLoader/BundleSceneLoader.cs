using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kirthos.ArtisanTDModLoader
{
    public static class BundleSceneLoader
    {
        private static bool isInit;
        private static List<string> loadedBundle = new List<string>();
        
        private static void Init()
        {
            isInit = true;
        }
        
        public static void LoadSceneFromModBundle(string modName, string sceneName, Action onComplete)
        {
            if (isInit == false)
                Init();
            if (sceneName.Contains("_"))
                return;
            if (loadedBundle.Contains(modName))
            {
                AsyncOperation op = SceneManager.LoadSceneAsync($"Assets/Mods/{modName}/{sceneName}.unity", LoadSceneMode.Additive);
                op.completed += __ =>
                {
                    onComplete?.Invoke();
                };
                return;
            }
            AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(ModLoader.ModsFolder + modName + "/" + modName.ToLower());
            loadedBundle.Add(modName);
            bundle.completed += _ =>
            {
                AsyncOperation op = SceneManager.LoadSceneAsync($"Assets/Mods/{modName}/{sceneName}.unity", LoadSceneMode.Additive);
                op.completed += __ =>
                {
                    onComplete?.Invoke();
                };
            };
        }
    }
}
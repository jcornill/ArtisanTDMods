using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kirthos.ArtisanTDModLoader
{
    public static class ModLoader
    {
        public static string ModsFolder = Application.dataPath + "/../Mods/";


        private static string Version => "v0.1";
        private static List<ModBase> _loadedMods = new List<ModBase>();
        
        public static void EntryPoint()
        {
            GameObject modObject = new GameObject("EventsCaller");
            modObject.AddComponent<EventsCaller>();
            Object.DontDestroyOnLoad(modObject);
            EventHandlerData.OnMainMenuLoaded += OnMainMenuLoaded;
            LoadMods();
        }

        private static void OnMainMenuLoaded()
        {
            BundleSceneLoader.LoadSceneFromModBundle("ModLoader", "ModLoader", () =>
            {
                GameObject modList = GameObject.Find("/ModLoader/ModList");
                TextMeshProUGUI modListText = modList.GetComponent<TextMeshProUGUI>();
                modListText.text = "Mod Loader " + Version + " by Kirthos\n";
                modListText.text += "Loaded Mods:\n";
                foreach (ModBase loadedMod in _loadedMods)
                {
                    modListText.text += loadedMod.DisplayName + " " + loadedMod.Version + " by " + loadedMod.Creator + "\n";
                }
            });
        }

        private static void AddModToList(ModBase mod)
        {
            _loadedMods.Add(mod);
        }

        private static void LoadMods()
        {
            if (Directory.Exists(ModsFolder) == false)
            {
                Directory.CreateDirectory(ModsFolder);
            }

            foreach (string directory in Directory.GetDirectories(ModsFolder))
            {
                try
                {
                    Debug.Log("Loading mod at path " + directory);
                    if (LoadMod(directory))
                    {
                        Debug.Log("Mod " + Path.GetFileNameWithoutExtension(directory) + " correctly loaded.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            //GameObject modObject = new GameObject("WaveSelector");
            //modObject.AddComponent<WaveSelector>();
            //Object.DontDestroyOnLoad(modObject);

            //GameObject modObject = new GameObject("BetterSpeed");
            //modObject.AddComponent<BetterSpeed>();
            //Object.DontDestroyOnLoad(modObject);

            //GameObject modObject = new GameObject("MapEditor");
            //modObject.AddComponent<MapEditor>();
            //Object.DontDestroyOnLoad(modObject);
        }

        private static bool LoadMod(string modPath)
        {
            string modName = Path.GetFileNameWithoutExtension(modPath);
            string modDllPath = modPath + "/" + modName + ".dll";
            if (File.Exists(modDllPath) == false)
            {
                Debug.LogError("Can't find dll mod file at path " + modDllPath);
                return false;
            }

            Assembly modAssembly = Assembly.LoadFile(modDllPath);
            foreach (Type type in modAssembly.GetExportedTypes())
            {
                if (type.IsClass == false) continue;
                if (type.IsSubclassOf(typeof(ModBase)) == false)
                {
                    continue;
                }

                GameObject modObject = new GameObject(modName);
                modObject.AddComponent(type);
                Object.DontDestroyOnLoad(modObject);
                ModBase modBase = modObject.GetComponent(type) as ModBase;
                AddModToList(modBase);
                return true;
            }
            Debug.LogError("Can't find any class that inherit ModBase inside mod dll at path " + modDllPath);
            return false;
        }
    }
}
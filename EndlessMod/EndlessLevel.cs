using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kirthos.ArtisanTDModLoader;
using TDTN.DataResources;
using TDTN.Managers;
using TMPro;
using UnityEngine;

namespace Kirthos.EndlessMod
{
    public class EndlessLevel : MonoBehaviour
    {
        private bool _levelStarted = false;
        private TextMeshProUGUI _waveNumber;
        private CustomGameMenu _gameMenu;
        
        public void LoadLevel()
        {
            _levelStarted = true;
            RegisterEvents();
            GameManager.Instance.LoadLevel(29);
        }

        private void RegisterEvents()
        {
            EventHandlerData.OnLevelLoaded += OnLevelLoaded;
        }

        private void OnLevelLoaded(string levelName)
        {
            if (_levelStarted == false) return;
            if (levelName.StartsWith("M29") == false) return;
            InitLevel();
        }

        private void InitLevel()
        {
            _levelStarted = false;
            _waveNumber = GameObject.Find("/LevelManager/LevelMenu/HUD/Anchor - Top").transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            ScriptableIntValue diffRef = typeof(GameManager).GetField("difficultyRef", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GameManager.Instance) as ScriptableIntValue;
            diffRef.SetValue((int)GameManager.GameDifficulty.Normal);
            SetupGameMenu();
            SetupMap();
        }

        private void SetupGameMenu()
        {
            _gameMenu = new CustomGameMenu();
            _gameMenu.SetPopupsTitle("Endless");
            _gameMenu.ToggleButtonRewind(false);
        }

        private void SetupMap()
        {
            TileMapManager tileMapEditor = new TileMapManager(LevelManager.Level.Tilemap);
            tileMapEditor.ClearFences();
            tileMapEditor.RemoveTurretAndWalls();
            tileMapEditor.RemoveAllTilesExceptPortals();
            CreateLevel(tileMapEditor);
            tileMapEditor.EndEdit();
            SetupWave();
        }

        private void SetupWave()
        {
            LevelEditor levelEditor = new LevelEditor(LevelManager.Instance);
            int waveToGenerate = 50;
            levelEditor.SetNumWave(waveToGenerate);
            EndlessWaveManager waveManager = new EndlessWaveManager();
            for (int i = 0; i < waveToGenerate; i++)
            {
                Debug.Log($"Generating wave {i}");
                EndlessWaveManager.EndlessWaveData data = waveManager.GetEndlessWaveData(i + 1);
                List<MonsterDataOnWave> monsters = waveManager.GenerateWave(data, i);
                levelEditor.SetWaveMonsters(i, monsters, (int)data.WaveDuration);
            }

            SetupTowers(levelEditor);
            levelEditor.SetStartGold(UpgradeManager.GoldStart);
            levelEditor.EndEdit();
        }

        private void SetupTowers(LevelEditor levelEditor)
        {
            levelEditor.ClearAllAvailableBuilding();
            levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Wall));
            levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Archer));
            if (UpgradeManager.HasArbalete)
                levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Arbaletrier));
            if (UpgradeManager.HasGel)
                levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Gel));
            if (UpgradeManager.HasFlechette)
                levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Flechette));
            if (UpgradeManager.HasCatapulte)
                levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Catapulte));
            if (UpgradeManager.HasBlizzard)
                levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Blizzard));
            if (UpgradeManager.HasPieux)
                levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Pieux));
            if (UpgradeManager.HasBaliste)
                levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Baliste));
            if (UpgradeManager.HasRayon)
                levelEditor.AddAvailableBuilding(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Rayon));
/*
            typeof(BuildingArchetype).GetField("_fireSpeed", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Archer), 50f);
            
            typeof(BuildingArchetype).GetField("_splashArea", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Catapulte), 5000f);
            
            typeof(BuildingArchetype).GetField("_splashDamages", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Catapulte), 5000);
            
            typeof(BuildingArchetype).GetField("_damages", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(TowerHelper.GetArchetypeFromType(TowerHelper.BasicTowerType.Catapulte), 5000f);
                */
            
        }

        private void CreateLevel(TileMapManager tileMapEditor)
        {
            int left = -2 - UpgradeManager.MapLeft;
            int right = 8 + UpgradeManager.MapRight;
            int down = -3 - UpgradeManager.MapDown;
            int up = -1 + UpgradeManager.MapUp;
            
            for (int i = left; i <= right; i++)
            {
                for (int j = down; j <= up; j++)
                {
                    tileMapEditor.CreateTile(new Vector3Int(i, j, 0));
                }
            }
        }
        
        private void LateUpdate()
        {
            if (_waveNumber == null) return;
            _waveNumber.text = _waveNumber.text.Split('/')[0];
            if (_gameMenu != null)
                _gameMenu.SetPopupsTitle("Endless");
        }
    }
}
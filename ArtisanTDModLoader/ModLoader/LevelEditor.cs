using System.Collections.Generic;
using System.Reflection;
using TDTN.DataResources;
using TDTN.Managers;
using TDTN.UI;
using UnityEngine;

namespace Kirthos.ArtisanTDModLoader
{
    public class LevelEditor
    {
        private LevelManager _level;
        private MethodInfo _initPortalMethod;
        private MethodInfo _initWeatherMethod;
        private FieldInfo _portalsData;
        private LevelInfos _levelInfos;

        public LevelEditor(LevelManager level)
        {
            _level = level;
            _initPortalMethod = typeof(LevelManager).GetMethod("InitPortals", BindingFlags.Instance | BindingFlags.NonPublic);
            _initWeatherMethod = typeof(LevelManager).GetMethod("InitWeather", BindingFlags.Instance | BindingFlags.NonPublic);
            _portalsData = typeof(LevelWavesArchetype).GetField("_levelPortalDatas", BindingFlags.Instance | BindingFlags.NonPublic);
            LevelList.Datas.GetCurrentLevelInfos(out _levelInfos);
            foreach (var buildings in _levelInfos.availableBuildings)
            {
                Debug.Log("Building: " + buildings.Key.ItemName + " x " + buildings.Value);
            }
            foreach (var elites in _levelInfos.availableElites)
            {
                Debug.Log("Elite: " + elites.Key.ItemName + " x " + elites.Value);
            }
            UpdateLevelPortalDatas(_levelInfos.levelWavesDatas, new LevelPortalDatas[] { _levelInfos.levelWavesDatas.LevelPortals[0] });
        }

        public void SetNumWave(int waveCount)
        {
            typeof(LevelWavesArchetype).GetField("_numWaves", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_levelInfos.levelWavesDatas, waveCount);
            _levelInfos.levelWavesDatas.LevelPortals[0].waves = new LevelWaveDatas[waveCount];
            _levelInfos.levelWavesDatas._levelWeatherDatas = new LevelWeatherDatas[waveCount];
            for (int i = 0; i < waveCount; i++)
            {
                _levelInfos.levelWavesDatas._levelWeatherDatas[i].weatherSpawningArray = new WeatherDataOnWave[0];
            }
            ScriptableGaugeIntValue waveRef = typeof(LevelManager).GetField("wavesRef", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(LevelManager.Instance) as ScriptableGaugeIntValue;
            waveRef.SetValue(0, 99);
        }

        public void SetWaveMonsters(int wave, List<MonsterDataOnWave> monsters, int waveDuration)
        {
            LevelWaveDatas waveData = _levelInfos.levelWavesDatas.LevelPortals[0].waves[wave];
            waveData.difficulties = new MonsterDatasByDifficulty[3];
            waveData.difficulties[0].monsterSpawningArray = new MonsterDataOnWave[0];
            waveData.difficulties[1].monsterSpawningArray = new MonsterDataOnWave[0];
            waveData.difficulties[2].monsterSpawningArray = new MonsterDataOnWave[0];
            
            waveData.SetWaveDatas(
                monsters.ToArray(),
                0,
                new WaveInfos(null, WaveType.Diverse, WaveColor.Grey),
                waveDuration,
                0
            );
            _levelInfos.levelWavesDatas.LevelPortals[0].waves[wave] = waveData;
        }

        public void SetStartGold(int gold)
        {
            _levelInfos.startGolds = gold;
            //ScriptableIntValue goldRef = typeof(LevelManager).GetField("goldRef", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(LevelManager.Instance) as ScriptableIntValue;
            //goldRef.SetValue(gold);
        }
        
        private void UpdateLevelPortalDatas(LevelWavesArchetype wave, LevelPortalDatas[] newPortalData)
        {
            _portalsData.SetValue(wave, newPortalData);
        }

        public void AddAvailableBuilding(BuildingArchetype building, int count = 0)
        {
            if (_levelInfos.availableBuildings.ContainsKey(building))
                _levelInfos.availableBuildings[building] += count;
            else
                _levelInfos.availableBuildings.Add(building, count);
        }
        
        public void AddAvailableElite(BuildingArchetype building, int count = 0)
        {
            if (_levelInfos.availableElites.ContainsKey(building))
                _levelInfos.availableElites[building] += count;
            else
                _levelInfos.availableElites.Add(building, count);
        }

        public void ClearAllAvailableBuilding()
        {
            ClearAllAvailableElites();
            _levelInfos.availableBuildings.Clear();
        }

        public void ClearAllAvailableElites()
        {
            _levelInfos.availableElites.Clear();
        }

        public void EndEdit()
        {
            typeof(LevelList).GetField("currentLevelName", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(LevelList.Datas, string.Empty);
            LevelList.Datas.Levels[29] = _levelInfos;
            _initPortalMethod.Invoke(_level, new object[] { _levelInfos });
            _initWeatherMethod.Invoke(_level, new object[] { _levelInfos });
            MethodInfo initDeck = typeof(BuildingDeck).GetMethod("InitDeck", BindingFlags.Instance | BindingFlags.NonPublic);
            initDeck.Invoke(GameObject.FindObjectOfType<BuildingDeck>().GetComponent<BuildingDeck>(), new object[] { });
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TDTN.DataResources;
using TDTN.Levels.Monsters;
using TDTN.Levels.Pathfinding;
using TDTN.Managers;
using TDTN.UI.Monsters;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Kirthos.ArtisanTDModLoader
{
    public class EventsCaller : MonoBehaviour
    {
        private FieldInfo waveRef;
        private int lastWave;
        private EntityManager _entityManager;
        private List<Entity> _monsters = new List<Entity>();
        private Dictionary<Entity, MonsterArchetype> _monsterDict = new Dictionary<Entity, MonsterArchetype>();
        private Dictionary<Entity, BaseMoveComponent> _monsterDictMove = new Dictionary<Entity, BaseMoveComponent>();
        private List<MonsterArchetype> _monstersArch = new List<MonsterArchetype>();
        
        private void Start()
        {
            _monstersArch = Resources.LoadAll<MonsterArchetype>("Monsters").ToList();
            RegisterUnityEvent();
            InitializeFieldInfo();
        }

        private void RegisterUnityEvent()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void InitializeFieldInfo()
        {
            FieldInfo[] fields = typeof(LevelManager).GetFields(
                BindingFlags.NonPublic | 
                BindingFlags.Instance);
            waveRef = fields.FirstOrDefault(x => x.Name == "wavesRef");
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Update()
        {
            UpdateLevel();
            UpdateMonstersEvents();
        }

        private void UpdateMonstersEvents()
        {
            foreach (Entity e in _entityManager.GetAllEntities())
            {
                if (_entityManager.HasComponent<MonsterComponent>(e))
                {
                    OnMonsterSpawn(e);
                }
            }

            foreach (Entity entity in _monsters.ToArray())
            {
                if (ECSUtils.EntityExists(entity) == false)
                {
                    OnMonsterKilled(entity);
                    continue;
                }

                if (_entityManager.HasComponent<MonsterComponent>(entity) == false)
                {
                    OnMonsterKilled(entity);
                    continue;
                }
                
                if (_entityManager.HasComponent<MoveOnPathComponent>(entity))
                    _monsterDictMove[entity] = _entityManager.GetComponentData<MoveOnPathComponent>(entity).baseMove;
                if (_entityManager.HasComponent<MoveFlyingComponent>(entity))
                    _monsterDictMove[entity] = _entityManager.GetComponentData<MoveFlyingComponent>(entity).baseMove;
                if (_entityManager.HasComponent<MoveCrawlingComponent>(entity))
                    _monsterDictMove[entity] = _entityManager.GetComponentData<MoveCrawlingComponent>(entity).baseMove;
                /*
                MonsterComponent monster = _entityManager.GetComponentData<MonsterComponent>(entity);
                if (monster.health <= 0)
                {
                    OnMonsterKilled(entity);
                }
                */
            }
        }

        private void OnMonsterKilled(Entity entity)
        {
            BaseMoveComponent baseMove = _monsterDictMove[entity];
            if (baseMove.distanceToDo > 4)
                EventHandlerData.MonsterKilled(_monsterDict[entity]);
            else
                EventHandlerData.MonsterReachPortal(_monsterDict[entity]);
            _monsterDict.Remove(entity);
            _monsterDictMove.Remove(entity);
            _monsters.Remove(entity);
        }

        private void OnMonsterSpawn(Entity entity)
        {
            MonsterComponent monster = _entityManager.GetComponentData<MonsterComponent>(entity);
            if (_monsters.Contains(entity)) return;
            if (monster.health <= 0) return;
            _monsters.Add(entity);
            _monsterDict.Add(entity, _monstersArch.FirstOrDefault(x => x.GetInstanceID() == monster.archetypeID));
            EventHandlerData.MonsterSpawn(_monsterDict[entity]);
        }

        private void UpdateLevel()
        {
            if (LevelManager.Instance == null) return;
            ScriptableGaugeIntValue wave = (waveRef.GetValue(LevelManager.Instance) as ScriptableGaugeIntValue);
            if (wave.Current != lastWave)
            {
                Debug.Log("Wave " + wave.Current + " start.");
                EventHandlerData.WaveStart(wave);
                lastWave = wave.Current;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            string sceneName = scene.name;
            Debug.Log(sceneName + " scene is loaded");
            if (sceneName == "MainMenu")
                EventHandlerData.MainMenuLoaded();
            else if (sceneName == "MapMenu")
                EventHandlerData.MapMenuLoaded();
            else if ((sceneName.StartsWith("M") || sceneName.StartsWith("B")) && (sceneName.Contains("_") || (sceneName.Length == 3 && char.IsNumber(sceneName[1]) && char.IsNumber(sceneName[2]))))
                LevelLoaded(sceneName);
        }

        private void LevelLoaded(string levelName)
        {
            EventHandlerData.LevelLoaded(levelName);
        }
    }
}
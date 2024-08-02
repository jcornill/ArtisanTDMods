using System;
using TDTN.DataResources;
using TDTN.Levels.Monsters;

namespace Kirthos.ArtisanTDModLoader
{
    public static class EventHandlerData
    {
        public static event Action OnMainMenuLoaded;
        internal static void MainMenuLoaded() => OnMainMenuLoaded?.Invoke();
        
        public static event Action OnMapMenuLoaded;
        internal static void MapMenuLoaded() => OnMapMenuLoaded?.Invoke();
        
        public static event Action<string> OnLevelLoaded;
        internal static void LevelLoaded(string levelName) => OnLevelLoaded?.Invoke(levelName);
        
        public static event Action<ScriptableGaugeIntValue> OnWaveStart;
        internal static void WaveStart(ScriptableGaugeIntValue waveCount) => OnWaveStart?.Invoke(waveCount);

        public static event Action<MonsterArchetype> OnMonsterSpawn;
        internal static void MonsterSpawn(MonsterArchetype monster) => OnMonsterSpawn?.Invoke(monster);
        
        public static event Action<MonsterArchetype> OnMonsterKilled;
        internal static void MonsterKilled(MonsterArchetype monster) => OnMonsterKilled?.Invoke(monster);
        
        public static event Action<MonsterArchetype> OnMonsterReachPortal;
        internal static void MonsterReachPortal(MonsterArchetype monster) => OnMonsterReachPortal?.Invoke(monster);
    }
}
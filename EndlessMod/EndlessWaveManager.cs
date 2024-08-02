using System.Collections.Generic;
using System.Linq;
using Kirthos.ArtisanTDModLoader;
using TDTN.DataResources;
using UnityEngine;

namespace Kirthos.EndlessMod
{
    public class EndlessWaveManager
    {
        public struct EndlessWaveData
        {
            public float WaveScore;
            public float WaveDuration;
        }

        private List<MonsterArchetype> _monsters = new List<MonsterArchetype>();
        private Dictionary<MonsterArchetype, float> _monstersCorruptedScore = new Dictionary<MonsterArchetype, float>();
        private Dictionary<MonsterArchetype, float> _monstersGoldScore = new Dictionary<MonsterArchetype, float>();

        public EndlessWaveManager()
        {
            _monsters = Resources.LoadAll<MonsterArchetype>("Monsters").ToList();
            foreach (MonsterArchetype monster in _monsters)
            {
                if (monster.MonsterType != MoveOnPathType.Normal)
                    continue;
                if (monster.Summoner.Enabled)
                    continue;
                if (monster.Pinata.Enabled)
                    continue;
                AssignScoreToMonster(monster);
            }

            _monstersCorruptedScore = _monstersCorruptedScore.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            _monstersGoldScore = _monstersGoldScore.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            EventHandlerData.OnMonsterKilled += OnMonsterKilled;
        }

        private void OnMonsterKilled(MonsterArchetype killedMonster)
        {
            float score = 0;
            if (killedMonster.GoldDrop == 0)
            {
                score = _monstersCorruptedScore[killedMonster];
            }
            else
            {
                score = _monstersGoldScore[killedMonster];
            }

            score /= 300f;
            float finalScore = score + Menu.Diamond;
            Menu.Diamond = finalScore;
        }


        private void AssignScoreToMonster(MonsterArchetype monster)
        {
            float score = 0;
            score += monster.Health;
            score += monster.Armor;
            if (monster.Dodge.Enabled)
                score *= 1.15f;

            if (monster.Doctor.Enabled)
                score *= 2f;

            if (monster.Tired.Enabled)
                score *= 0.75f;

            if (monster.Speed > 5)
                score *= 1f + 0.05f * (monster.Speed - 5f);
            else
                score *= 1f - 0.1f * (5f - monster.Speed);

            if (monster.GoldDrop == 0)
            {
                _monstersCorruptedScore.Add(monster, score);
            }
            else
            {
                _monstersGoldScore.Add(monster, score);
            }

            Debug.Log(monster.ItemName + ":" + monster.name + " -> " + score + " g:" + monster.GoldDrop);
        }

        public EndlessWaveData GetEndlessWaveData(int waveCount)
        {
            EndlessWaveData result = new EndlessWaveData()
            {
                WaveDuration = 10 + waveCount * 3
            };
            if (waveCount > 20)
            {
                result.WaveDuration = 70;
            }

            if (waveCount < 3)
            {
                result.WaveScore = 150 + waveCount * 50;
            }
            else if (waveCount < 6)
            {
                result.WaveScore = waveCount * waveCount * 35;
            }
            else
            {
                result.WaveScore = waveCount * waveCount * waveCount * 5;
            }

            return result;
        }

        private float GetScore(MonsterArchetype monster, float score, int waveIndex)
        {
            return score;
        }

        private KeyValuePair<MonsterArchetype, float> GetRandomGold(IEnumerable<KeyValuePair<MonsterArchetype, float>> list, float remainingScore, int waveIndex, int gold)
        {
            if (list.Any() == false) return default;
            List<KeyValuePair<MonsterArchetype, float>> finalList = new List<KeyValuePair<MonsterArchetype, float>>();

            foreach (var rng in list)
            {
                if (rng.Value < remainingScore / (10f + waveIndex * 3))
                    continue;
                if (rng.Value / rng.Key.GoldDrop < remainingScore / 30f)
                    finalList.Add(rng);
            }
            
            if (finalList.Any())
                return finalList.ElementAt(Random.Range(0, finalList.Count));

            Debug.Log("Rng: " + remainingScore);
            if (remainingScore / 6f > gold)
            {
                IEnumerable<KeyValuePair<MonsterArchetype, float>> list2 = list.Where(x => x.Value > remainingScore / 10f);
                if (list2.Any())
                    return list2.ElementAt(Random.Range(0, list2.Count()));
                return list.ElementAt(Random.Range(0, list.Count()));
            }

            return list.OrderBy(x => x.Value).First();
        }

        private KeyValuePair<MonsterArchetype, float> GetRandomCorr(IEnumerable<KeyValuePair<MonsterArchetype, float>> list, float remainingScore, int waveIndex)
        {
            IEnumerable<KeyValuePair<MonsterArchetype, float>> list2 = list.Where(x => x.Value < remainingScore / 10f);
            if (list2.Any())
                return list2.ElementAt(Random.Range(0, list2.Count()));
            
            return list.ElementAt(Random.Range(0, list.Count()));
        }

        public List<MonsterDataOnWave> GenerateWave(EndlessWaveData data, int index)
        {
            List<MonsterDataOnWave> waveData = new List<MonsterDataOnWave>();
            float scorePerSecond = data.WaveScore / data.WaveDuration;
            float totalScore = data.WaveScore;
            int goldAvailable = 20 + index * 8;

            var gold = _monstersGoldScore.Where(x => GetScore(x.Key, x.Value, index) < totalScore);
            while (goldAvailable > 0)
            {
                KeyValuePair<MonsterArchetype, float> rng = GetRandomGold(gold.Where(x => x.Key.GoldDrop <= goldAvailable), totalScore, index, goldAvailable);
                if (rng.Key == null)
                {
                    Debug.Log("nothing");
                    break;
                }

                float monsterScore = GetScore(rng.Key, rng.Value, index);
                totalScore -= monsterScore;
                goldAvailable -= rng.Key.GoldDrop;
                waveData.Add(new MonsterDataOnWave()
                {
                    Archetype = rng.Key,
                    Line = Random.Range(0, 5),
                    SpawningTime = Random.Range(0f, data.WaveDuration)
                });
            }

            for (int i = 0; i < data.WaveDuration; i++)
            {
                scorePerSecond = data.WaveScore / data.WaveDuration;
                while (true)
                {
                    var tmp = _monstersCorruptedScore.Where(x => GetScore(x.Key, x.Value, index) < totalScore);
                    if (!tmp.Any())
                        break;
                    var rng = GetRandomCorr(tmp, totalScore, index);
                    float monsterScore = GetScore(rng.Key, rng.Value, index);
                    scorePerSecond -= monsterScore;
                    totalScore -= monsterScore;
                    waveData.Add(new MonsterDataOnWave()
                    {
                        Archetype = rng.Key,
                        Line = Random.Range(0, 5),
                        SpawningTime = Random.Range(i, i + 1f)
                    });
                }
            }

            while (true)
            {
                var tmp2 = _monstersCorruptedScore.Where(x => GetScore(x.Key, x.Value, index) < totalScore);
                if (!tmp2.Any())
                    break;
                var rng = GetRandomCorr(tmp2, totalScore, index);
                float monsterScore = GetScore(rng.Key, rng.Value, index);
                totalScore -= monsterScore;
                waveData.Add(new MonsterDataOnWave()
                {
                    Archetype = rng.Key,
                    Line = Random.Range(0, 5),
                    SpawningTime = Random.Range(0f, data.WaveDuration)
                });
            }

            Debug.Log("RemainingGold: " + goldAvailable);
            Debug.Log("Monsters: " + waveData.Count());
            foreach (MonsterDataOnWave wave in waveData)
            {
                Debug.Log(wave.Archetype.ItemName + ":" + wave.Archetype.name);
            }

            return waveData;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TDTN.DataResources;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Kirthos.ArtisanTDModLoader
{
    public static class TowerHelper
    {
        public enum BasicTowerType
        {
            Wall,
            Archer,
            Arbaletrier,
            Gel,
            Flechette,
            Catapulte,
            Pieux,
            Baliste,
            Blizzard,
            Rayon
        }
        
        public enum EliteTowerType
        {
            Archer,
            Arbaletrier,
            Gel,
            Flechette,
            Catapulte,
            Pieux,
            Baliste,
            Blizzard
        }

        private static bool IsInit;
        private static List<BuildingArchetype> _buildings;
        
        private static void Init()
        {
            IsInit = true;
            _buildings = Resources.LoadAll<BuildingArchetype>("Buildings").ToList();
            foreach (BuildingArchetype building in _buildings)
            {
                Debug.Log(building.ItemName + " - " + building.name);
            }
        }

        public static BuildingArchetype GetArchetypeFromType(BasicTowerType towerType)
        {
            if (IsInit == false)
                Init();
            switch (towerType)
            {
                case BasicTowerType.Wall:
                    return _buildings.First(x => x.name == "Mur");
                case BasicTowerType.Archer:
                    return _buildings.First(x => x.name == "Archer_L1");
                case BasicTowerType.Arbaletrier:
                    return _buildings.First(x => x.name == "Arbalete_L1");
                case BasicTowerType.Gel:
                    return _buildings.First(x => x.name == "Gel_L1");
                case BasicTowerType.Flechette:
                    return _buildings.First(x => x.name == "Flechette_L1");
                case BasicTowerType.Catapulte:
                    return _buildings.First(x => x.name == "Catapulte_L1");
                case BasicTowerType.Pieux:
                    return _buildings.First(x => x.name == "Pieux_L1");
                case BasicTowerType.Baliste:
                    return _buildings.First(x => x.name == "Baliste_L1");
                case BasicTowerType.Blizzard:
                    return _buildings.First(x => x.name == "Spray_L1");
                case BasicTowerType.Rayon:
                    return _buildings.First(x => x.name == "Rayon_L1");
            }
            return null;
        }

        public static BuildingArchetype GetArchetypeFromType(EliteTowerType towerType)
        {
            if (IsInit == false)
                Init();
            switch (towerType)
            {
                case EliteTowerType.Archer:
                    return _buildings.First(x => x.name == "Archer_L4");
                case EliteTowerType.Arbaletrier:
                    return _buildings.First(x => x.name == "Arbalete_L4");
                case EliteTowerType.Gel:
                    return _buildings.First(x => x.name == "Gel_L4");
                case EliteTowerType.Flechette:
                    return _buildings.First(x => x.name == "Flechette_L4");
                case EliteTowerType.Catapulte:
                    return _buildings.First(x => x.name == "Catapulte_L4");
                case EliteTowerType.Pieux:
                    return _buildings.First(x => x.name == "Pieux_L4");
                case EliteTowerType.Baliste:
                    return _buildings.First(x => x.name == "Baliste_L4");
                case EliteTowerType.Blizzard:
                    return _buildings.First(x => x.name == "Spray_L4");
            }
            return null;
        }
    }
}
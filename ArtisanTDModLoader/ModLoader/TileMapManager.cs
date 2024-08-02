using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TDTN.DataResources;
using TDTN.Levels;
using TDTN.Levels.Pathfinding;
using TDTN.Levels.Tiles;
using TDTN.Levels.Tiles.Effects;
using TDTN.Managers;
using UnityEngine;

namespace Kirthos.ArtisanTDModLoader
{
    public class TileMapManager
    {
        private Tilemap3D _map;
        private MethodInfo _instantiateMethod;
        private MethodInfo _pathfinderStart;
        private FieldInfo _tileField;
        private GroundTileArchetype[] _groundTileArchetypes;

        private GameObject _groundTilePrefab;
        
        public TileMapManager(Tilemap3D map)
        {
            _map = map;
            _groundTileArchetypes = Resources.LoadAll<GroundTileArchetype>("Tiles");
            foreach (GroundTileArchetype tileArchetype in _groundTileArchetypes)
            {
                Debug.Log(tileArchetype);
                Debug.Log(tileArchetype.Prefab);
                Debug.Log(tileArchetype.ItemName);
                if (tileArchetype.ToString().Split(' ')[0].Length == 2)
                {
                    _groundTilePrefab = tileArchetype.Prefab;
                }
            }
            _instantiateMethod = _map.GetType().GetMethods(
                BindingFlags.NonPublic |
                BindingFlags.Instance).FirstOrDefault(x => x.Name == "InstantiateGroundTile");
            _pathfinderStart = typeof(Pathfinder).GetMethods(
                BindingFlags.NonPublic |
                BindingFlags.Instance).FirstOrDefault(x => x.Name == "Start");
            _tileField = typeof(GroundTile).GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance).FirstOrDefault(x => x.Name == "_tile");
        }

        public void ClearFences()
        {
            foreach (Tile3D tile in _map.TileMap.Values)
            {
                tile.Ground.GetFenceTr.gameObject.SetActive(false);
                tile.Ground.GetBlockTr.gameObject.SetActive(false);
            }
        }
        
        public void RemoveTurretAndWalls()
        {
            List<Tile3D> toRemove = new List<Tile3D>();
            foreach (Tile3D tile in LevelManager.Level.Tilemap.TileMap.Values)
            {
                if (tile.Superstructure != null)
                {
                    GameObject.Destroy(tile.Superstructure.gameObject);
                    tile.RemoveObjectOnTile(tile.Superstructure);
                }

                if (tile.Wall != null)
                {
                    GameObject.Destroy(tile.Wall.gameObject);
                    tile.RemoveObjectOnTile(tile.Wall);
                }

                if (tile.Ground.Archetype.Effect.effectType != GroundEffectType.None || tile.Ground.Archetype.IsRoad)
                {
                    GameObject.Destroy(tile.GetHigherTileGameObject());
                    toRemove.Add(tile);
                }
            }

            foreach (Tile3D tile3D in toRemove)
            {
                LevelManager.Level.Tilemap.TileMap.Remove(tile3D.GetHigherTileObject().TilePosition);
            }
        }
        
        public void CreateTile(Vector3Int pos)
        {
            if (_map.GetTile(pos, out Tile3D existingTile)) return;
            Vector3 objectWorldPosition = _map.GetTileObjectWorldPosition(pos);
            GameObject go = GameObject.Instantiate(_groundTilePrefab, objectWorldPosition, Quaternion.identity);
            //GameObject gameObject = Object.Instantiate<GameObject>(groundTile.gameObject, objectWorldPosition, rotation, this.transform);
            //GroundTile tile = go1.GetComponent<GroundTile>();
            //GameObject go = _instantiateMethod.Invoke(_map, new object[] { tile, pos, Quaternion.identity }) as GameObject;
            //var temp = go.name.Split(' ').ToList();
            //temp.RemoveAt(1);
            //go.name = String.Join(" ", temp.ToArray());
            GroundTile newTile = go.GetComponent<GroundTile>();
            newTile.TilePosition = pos;
            Tile3D createdTile = new Tile3D(newTile);
            newTile.Init(createdTile);
            //_tileField.SetValue(newTile, createdTile);
            _map.TileMap.Add(pos, createdTile);
        }

        public void RemoveAllTilesExceptPortals()
        {
            foreach (Tile3D tile in LevelManager.Level.Tilemap.TileMap.Values.ToArray())
            {
                if (tile.Portal == null)
                {
                    GameObject.Destroy(tile.GetHigherTileGameObject());
                    GameObject.Destroy(tile.Ground);
                    LevelManager.Level.Tilemap.TileMap.Remove(tile.GetHigherTileObject().TilePosition);
                }
            }
        }

        public void EndEdit()
        {
            foreach (MonoBehaviour go in GameObject.FindObjectsOfType<MonoBehaviour>())
            {
                GroundTile tile = go.GetComponent<GroundTile>();
                if (tile != null)
                {
                    if (tile.Tile == null)
                    {
                        if (LevelManager.Level.Tilemap.GetTile(tile.TilePosition, out Tile3D tile2))
                        {
                            if (tile2.Portal != null)
                                continue;
                        }
                        //Debug.Log("Removing object");
                        GameObject.Destroy(tile.gameObject);
                    }
                }
            }
            LevelManager.Level.Heightmap.Init(_map);
            _pathfinderStart?.Invoke(LevelManager.Instance.Pathfinder, new object[] { });
        }
    }
}
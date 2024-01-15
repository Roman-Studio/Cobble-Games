using System;
using System.Collections;
using System.Collections.Generic;
using CobbleGames.Grid;
using CobbleGames.PathFinding;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CobbleGames.Map
{
    public class CGMapGenerator : MonoBehaviour
    {
        [field: SerializeField, Expandable]
        public CGMapGeneratorPreset MapGeneratorPreset { get; set; }
        
        public bool IsGenerating { get; private set; }
        public event Action EventMapGenerationFinished;

        private Unity.Mathematics.Random _MapGeneratorRandom;

        private readonly List<AsyncOperationHandle<CGMapTile>> _QueuedTilesToSpawn = new();
        
        public void GenerateNewMap(uint seed, out CGGrid<CGMapTile> generatedMapGrid)
        {
            IsGenerating = true;
            _MapGeneratorRandom = new Unity.Mathematics.Random(seed);
            generatedMapGrid = new CGGrid<CGMapTile>(MapGeneratorPreset.MapSizeX, MapGeneratorPreset.MapSizeY);

            if (MapGeneratorPreset.MapTilesPrefabs.Count == 0)
            {
                IsGenerating = false;
                Debug.LogError($"[{nameof(CGMapGenerator)}.{nameof(GenerateNewMap)}] There are no map tiles prefabs assigned!", MapGeneratorPreset);
                return;
            }

            for (var x = 0; x < MapGeneratorPreset.MapSizeX; x++)
            {
                for (var y = 0; y < MapGeneratorPreset.MapSizeY; y++)
                {
                    SpawnRandomTile(x, y, generatedMapGrid);
                }
            }
            
            var mapPosition = transform;
            mapPosition.position = new Vector3(MapGeneratorPreset.MapSizeX / 2f, mapPosition.position.y, MapGeneratorPreset.MapSizeY / -2f);

            StartCoroutine(WaitForGenerationComplete(generatedMapGrid, OnGenerationCompleted));
        }

        private void SpawnRandomTile(int x, int y, CGGrid<CGMapTile> targetGrid)
        {
            var randomTilePrefabIndex = _MapGeneratorRandom.NextInt(0, MapGeneratorPreset.MapTilesPrefabs.Count);
            var spawnTileAsyncOperation = MapGeneratorPreset.MapTilesPrefabs[randomTilePrefabIndex].InstantiateAsync(transform);
            
            spawnTileAsyncOperation.Completed += (handle) =>
            {
                var spawnedMapTile = handle.Result;

                var mapOriginPosition = transform.position;
                spawnedMapTile.transform.position = new Vector3(mapOriginPosition.x - y * MapGeneratorPreset.TileSize, mapOriginPosition.y, mapOriginPosition.z + x * MapGeneratorPreset.TileSize);
                targetGrid.TrySetElement(x, y, spawnedMapTile);

                _QueuedTilesToSpawn.Remove(handle);
            };
            
            _QueuedTilesToSpawn.Add(spawnTileAsyncOperation);
        }

        private IEnumerator WaitForGenerationComplete(CGGrid<CGMapTile> generatedGrid, Action<CGGrid<CGMapTile>> callback)
        {
            if (_QueuedTilesToSpawn.Count > 0)
            {
                yield return new WaitUntil(() => _QueuedTilesToSpawn.Count <= 0);
            }

            IsGenerating = false;
            callback?.Invoke(generatedGrid);
            EventMapGenerationFinished?.Invoke();
        }

        private static void OnGenerationCompleted(CGGrid<CGMapTile> generatedMapGrid)
        {
            AssignNeighbourMapTiles(generatedMapGrid);
            InitializePathFinding(generatedMapGrid);
        }

        private static void AssignNeighbourMapTiles(CGGrid<CGMapTile> generatedGrid)
        {
            foreach (var mapTile in generatedGrid.GridElements)
            {
                mapTile.GetNeighbourMapTiles(generatedGrid);
            }
        }

        private static void InitializePathFinding(CGGrid<CGMapTile> generatedGrid)
        {
            generatedGrid.CastGrid<ICGPathFindingNode>(out var pathFindingNodesGrid);
            CGPathFindingManager.Instance.Initialize(pathFindingNodesGrid);
        }
    }
}
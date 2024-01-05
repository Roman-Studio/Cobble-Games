using CobbleGames.Grid;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Map
{
    public class CGMapGenerator : MonoBehaviour
    {
        [field: SerializeField, Expandable]
        public CGMapGeneratorPreset MapGeneratorPreset { get; set; }
        
        public void GenerateNewMap(out CGGrid<CGMapTile> generatedMapGrid)
        {
            generatedMapGrid = new CGGrid<CGMapTile>(MapGeneratorPreset.MapSizeX, MapGeneratorPreset.MapSizeY);

            if (MapGeneratorPreset.MapTilesPrefabs.Count == 0)
            {
                Debug.LogError($"[{nameof(CGMapGenerator)}.{nameof(GenerateNewMap)}] There are no map tiles prefabs assigned!");
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
        }

        private void SpawnRandomTile(int x, int y, CGGrid<CGMapTile> targetGrid)
        {
            var randomTilePrefabIndex = Random.Range(0, MapGeneratorPreset.MapTilesPrefabs.Count);
            var spawnedMapTile = Instantiate(MapGeneratorPreset.MapTilesPrefabs[randomTilePrefabIndex], transform).GetComponent<CGMapTile>();

            var mapOriginPosition = transform.position;
            spawnedMapTile.transform.position = new Vector3(mapOriginPosition.x - y * MapGeneratorPreset.TileSize, mapOriginPosition.y, mapOriginPosition.z + x * MapGeneratorPreset.TileSize);
            targetGrid.TrySetElement(x, y, spawnedMapTile);
        }
    }
}
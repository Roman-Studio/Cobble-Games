using System.Collections.Generic;
using UnityEngine;

namespace CobbleGames.Map
{
    [CreateAssetMenu(fileName = "MapGeneratorPreset", menuName = "Cobble Games/Map/Generator Preset")]
    public class CGMapGeneratorPreset : ScriptableObject
    {
        [SerializeField]
        private List<CGMapTile> _MapTilesPrefabs = new();
        public IReadOnlyList<CGMapTile> MapTilesPrefabs => _MapTilesPrefabs;

        [field: SerializeField]
        public int MapSizeX { get; private set; } = 50;

        [field:SerializeField]
        public int MapSizeY { get; private set; } = 50;

        [field:SerializeField]
        public float TileSize { get; private set; } = 1f;
    }
}
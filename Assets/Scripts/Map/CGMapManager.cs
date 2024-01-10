using CobbleGames.Core;
using CobbleGames.Grid;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Map
{
    public class CGMapManager : CGManager<CGMapManager>
    {
        [SerializeField]
        private CGMapGenerator _MapGenerator;

        [field: SerializeField, ReadOnly]
        public CGGrid<CGMapTile> GeneratedGrid { get; private set; }
        
        public override void Initialize()
        {
            GenerateRandomMap();
        }

        private bool IsPlaying => Application.isPlaying;

        [Button, EnableIf(nameof(IsPlaying))]
        private void GenerateRandomMap()
        {
            GeneratedGrid.ForEach(DestroyTile);
            _MapGenerator.GenerateNewMap(out var generatedMapGrid);
            GeneratedGrid = generatedMapGrid;
        }

        private static void DestroyTile(CGMapTile mapTile)
        {
            if (mapTile == null)
            {
                return;
            }
            
            Destroy(mapTile.gameObject);
        }
    }
}
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

        [SerializeField, ReadOnly]
        private CGGrid<CGMapTile> _GeneratedGrid;
        
        public override void Initialize()
        {
            GenerateRandomMap();
        }

        private bool IsPlaying => Application.isPlaying;

        [Button, EnableIf(nameof(IsPlaying))]
        private void GenerateRandomMap()
        {
            _GeneratedGrid.ForEach(DestroyTile);
            _MapGenerator.GenerateNewMap(out _GeneratedGrid);
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
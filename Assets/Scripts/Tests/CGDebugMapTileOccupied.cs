using System;
using CobbleGames.Map;
using UnityEngine;

namespace CobbleGames.Tests
{
    public class CGDebugMapTileOccupied : MonoBehaviour
    {
        [SerializeField]
        private CGMapTile _TargetMapTile;

        [SerializeField]
        private Renderer _TileRenderer;
        
        #if PATHFINDING_DEBUG
        private Color _OriginalTileColor;

        private void Awake()
        {
            _OriginalTileColor = _TileRenderer.material.color;
        }

        private void Update()
        {
            _TileRenderer.material.color = _TargetMapTile.IsOccupied ? Color.red : _OriginalTileColor;
        }
        #endif
    }
}
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
            _TargetMapTile.EventCurrentlyAssignedObjectChanged += UpdateTileColor;
        }

        private void OnDestroy()
        {
            _TargetMapTile.EventCurrentlyAssignedObjectChanged -= UpdateTileColor;
        }

        private void UpdateTileColor()
        {
            _TileRenderer.material.color = _TargetMapTile.IsOccupied ? Color.red : _OriginalTileColor;
        }
        #endif
    }
}
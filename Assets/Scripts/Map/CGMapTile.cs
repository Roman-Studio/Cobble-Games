using System.Collections.Generic;
using CobbleGames.Grid;
using CobbleGames.PathFinding;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Map
{
    public class CGMapTile : MonoBehaviour, ICGPathFindingNode
    {
        [field: SerializeField]
        public Transform TileSurfaceCenter { get; private set; }
        
        [SerializeField]
        private bool _IsWalkable = true;

        [SerializeField]
        private float _TileMovementCost = 1f;

        [field: SerializeField, ReadOnly, Foldout("Debug")]
        public int X { get; set; }
        
        [field: SerializeField, ReadOnly, Foldout("Debug")]
        public int Y { get; set; }

        public Vector3 NodePosition => TileSurfaceCenter.position;
        public float WalkingCost => _TileMovementCost;
        public bool IsWalkable => _IsWalkable;

        [SerializeField, ReadOnly, Foldout("Debug")]
        private List<CGMapTile> _NeighbourMapTiles = new ();

        public void GetNeighbourMapTiles(CGGrid<CGMapTile> mapTilesGrid)
        {
            _NeighbourMapTiles.Clear();
            
            if (X - 1 >= 0)
            {
                // Left
                GetNeighbourNode(X - 1, Y, mapTilesGrid);
                
                // Left Down
                if (Y - 1 >= 0)
                {
                    GetNeighbourNode(X - 1, Y - 1, mapTilesGrid);
                }
                
                // Left Up
                if (Y + 1 < mapTilesGrid.SizeY)
                {
                    GetNeighbourNode(X - 1, Y + 1, mapTilesGrid);
                }
            }
            
            if (X + 1 < mapTilesGrid.SizeX) 
            {
                // Right
                GetNeighbourNode(X + 1, Y, mapTilesGrid);
                
                // Right Down
                if (Y - 1 >= 0)
                {
                    GetNeighbourNode(X + 1, Y - 1, mapTilesGrid);
                }
                
                // Right Up
                if (Y + 1 < mapTilesGrid.SizeY)
                {
                    GetNeighbourNode(X + 1, Y + 1, mapTilesGrid);
                }
            }
            
            // Down
            if (Y - 1 >= 0)
            {
                GetNeighbourNode(X, Y - 1, mapTilesGrid);
            }

            // Up
            if (Y + 1 < mapTilesGrid.SizeY)
            {
                GetNeighbourNode(X, Y + 1, mapTilesGrid);
            }
        }

        private void GetNeighbourNode(int x, int y, CGGrid<CGMapTile> mapTilesGrid)
        {
            if (!mapTilesGrid.TryGetElement(x, y, out var foundMapTile))
            {
                Debug.LogError($"[{nameof(CGMapTile)}.{nameof(GetNeighbourNode)}] Failed to get neighbour node at ({x}, {y})!", this);
                return;
            }
            
            _NeighbourMapTiles.Add(foundMapTile);
        }
    }
}
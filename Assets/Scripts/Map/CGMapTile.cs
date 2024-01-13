using System;
using System.Collections.Generic;
using CobbleGames.Characters;
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

        private Bounds _NodeBounds;
        public Bounds NodeBounds
        {
            get
            {
                if (_NodeBounds != default)
                {
                    return _NodeBounds;
                }
                
                var boundsSize = CGPathFindingManager.Instance.GridFromWorldPositionDetectionTolerance;
                _NodeBounds = new Bounds(NodePosition, new Vector3(boundsSize, boundsSize, boundsSize));
                return _NodeBounds;
            }
        }
        
        public float WalkingCost => _TileMovementCost;
        public bool IsWalkable => _IsWalkable && !IsOccupied;

        [SerializeField, ReadOnly, Foldout("Debug")]
        private List<CGMapTile> _NeighbourMapTiles = new ();
        public IReadOnlyList<CGMapTile> NeighbourMapTiles => _NeighbourMapTiles;
        
        public ICGTileAssignable CurrentlyAssignedObject { get; private set; }
        public bool IsOccupied => CurrentlyAssignedObject != default;

        public void AssignObject(ICGTileAssignable tileAssignable)
        {
            if (tileAssignable == null)
            {
                CurrentlyAssignedObject = null;
                CGMapManager.Instance.InvokeEventAnyGridStateChanged();
                return;
            }
            
            if (tileAssignable.CurrentMapTile != null)
            {
                tileAssignable.CurrentMapTile.AssignObject(null);
            }
            
            if (CurrentlyAssignedObject != default)
            {
                CurrentlyAssignedObject.CurrentMapTile = null;
            }

            CurrentlyAssignedObject = tileAssignable;
            tileAssignable.CurrentMapTile = this;
            CGMapManager.Instance.InvokeEventAnyGridStateChanged();
        }

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(NodeBounds.center, NodeBounds.size);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using CobbleGames.Core;
using CobbleGames.Grid;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CobbleGames.PathFinding
{
    public class CGPathFindingManager : CGManager<CGPathFindingManager>
    {
        public const float MoveStraightCostModifier = 1f;
        public const float MoveDiagonalCostModifier = 1.4f;

        [SerializeField]
        private float _GridFromWorldPositionDetectionTolerance = 0.5f;
        
        private CGGrid<ICGPathFindingNode> _CurrentNodes;

        private readonly List<ICGPathFindingNode> _OpenList = new();
        private readonly List<ICGPathFindingNode> _ClosedList = new();
        
        public void Initialize(CGGrid<ICGPathFindingNode> nodesGrid)
        {
            _CurrentNodes = nodesGrid;
        }

        public bool TryGetPathFindingNodeFromWorldPosition(Vector3 worldPosition, out ICGPathFindingNode foundPathFindingNode)
        {
            foundPathFindingNode = default;

            foreach (var pathFindingNode in _CurrentNodes.GridElements)
            {
                if (!(worldPosition - pathFindingNode.NodePosition).magnitude.IsInRangeInclusive(-_GridFromWorldPositionDetectionTolerance, _GridFromWorldPositionDetectionTolerance))
                {
                    continue;
                }
                
                foundPathFindingNode = pathFindingNode;
                return true;
            }

            return false;
        }
        
        public bool FindPath(Vector3 startPosition, Vector3 targetPosition, out List<ICGPathFindingNode> foundPath)
        {
            foundPath = default;
            
            if (!TryGetPathFindingNodeFromWorldPosition(startPosition, out var startNode))
            {
                Debug.LogError($"[{nameof(CGPathFindingManager)}.{nameof(FindPath)}] Requested start position is outside of the navigation area!", this);
                return false;
            }
            
            if (!TryGetPathFindingNodeFromWorldPosition(targetPosition, out var targetNode))
            {
                Debug.LogError($"[{nameof(CGPathFindingManager)}.{nameof(FindPath)}] Requested target position is outside of the navigation area!", this);
                return false;
            }

            return FindPath(startNode, targetNode, out foundPath);
        }
        
        public bool FindPath(int startX, int startY, int endX, int endY, out List<ICGPathFindingNode> foundPath)
        {
            foundPath = default;
            
            if (!_CurrentNodes.TryGetElement(startX, startY, out var startNode))
            {
                Debug.LogError($"[{nameof(CGPathFindingManager)}.{nameof(FindPath)}] Failed to get start path node at: ({startX}, {startY})!", this);
                return false;
            }
            
            if (!_CurrentNodes.TryGetElement(endX, endY, out var endNode))
            {
                Debug.LogError($"[{nameof(CGPathFindingManager)}.{nameof(FindPath)}] Failed to get end path node at: ({endX}, {endY})!", this);
                return false;
            }

            return FindPath(startNode, endNode, out foundPath);
        }

        public bool FindPath(ICGPathFindingNode startNode, ICGPathFindingNode targetNode, out List<ICGPathFindingNode> foundPath)
        {
            foundPath = new List<ICGPathFindingNode>();
            
            if (!targetNode.IsWalkable)
            {
                return false;
            }

            if (startNode == targetNode)
            {
                return false;
            }

            var jobNodes = new NativeArray<CGPathFindingNodeData>(_CurrentNodes.SizeX * _CurrentNodes.SizeY, Allocator.TempJob);

            for (var i = 0; i < _CurrentNodes.GridElements.Count; i++)
            {
                var node = _CurrentNodes.GridElements[i];
                
                var newJobNode = new CGPathFindingNodeData()
                {
                    X = node.X,
                    Y = node.Y,
                    Index = i,
                    WalkingCost = float.MaxValue,
                    WalkingCostModifier = node.WalkingCost,
                    DistanceToTarget = 0,
                    IsWalkable = node.IsWalkable,
                    ParentNodeIndex = -1
                };

                jobNodes[i] = newJobNode;
            }

            var findPathJob = new CGPathFindingJob
            (
                new int2(startNode.X, startNode.Y),
                new int2(targetNode.X, targetNode.Y), 
                new int2(_CurrentNodes.SizeX, _CurrentNodes.SizeY), 
                jobNodes
            );
            
            var jobHandle = findPathJob.Schedule();
            jobHandle.Complete();

            var result = findPathJob.IsSuccessful(out var nodesIndexes);

            if (result)
            {
                foundPath = CalculatePath(nodesIndexes);
            }
            
            findPathJob.Dispose();
            nodesIndexes.Dispose();
            jobNodes.Dispose();
            return result;
        }
        
        private List<ICGPathFindingNode> CalculatePath(NativeList<int> nodeIndexes)
        {
            var result = new List<ICGPathFindingNode>();

            for (var i = nodeIndexes.Length - 1; i >= 0; i--)
            {
                result.Add(_CurrentNodes.GridElements[nodeIndexes[i]]);
            }
            
            return result;
        }
    }
}
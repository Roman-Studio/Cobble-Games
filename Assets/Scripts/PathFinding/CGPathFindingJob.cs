using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CobbleGames.PathFinding
{
    [BurstCompile]
    internal struct CGPathFindingJob : IJob, IDisposable
    {
        public int2 StartNodePosition { get; }
        public int2 EndNodePosition { get; }
        public int2 GridSize { get; }

        private NativeArray<CGPathFindingNodeData> _PathFindingNodes;

        private NativeList<int> _OpenList;
        private NativeList<int> _ClosedList;
        private NativeArray<int2> _NeighbourOffsetArray;

        private NativeList<int> _ResultingPath;

        public CGPathFindingJob(int2 startNodePosition, int2 endNodePosition, int2 gridSize, NativeArray<CGPathFindingNodeData> pathFindingNodes)
        {
            StartNodePosition = startNodePosition;
            EndNodePosition = endNodePosition;
            GridSize = gridSize;
            _PathFindingNodes = pathFindingNodes;
            _OpenList = new NativeList<int>(Allocator.TempJob);
            _ClosedList = new NativeList<int>(Allocator.TempJob);
            _ResultingPath = new NativeList<int>(Allocator.TempJob);
            
            _NeighbourOffsetArray = new NativeArray<int2>(8, Allocator.TempJob);
            _NeighbourOffsetArray[0] = new int2(-1, 0); //Left
            _NeighbourOffsetArray[1] = new int2(1, 0); //Right
            _NeighbourOffsetArray[2] = new int2(0, 1); //Up
            _NeighbourOffsetArray[3] = new int2(0, -1); //Down
            _NeighbourOffsetArray[4] = new int2(-1, -1); //Left Down
            _NeighbourOffsetArray[5] = new int2(-1, 1); //Left Up
            _NeighbourOffsetArray[6] = new int2(1, -1); //Right Down
            _NeighbourOffsetArray[7] = new int2(1, 1); //Right Up
        }

        public bool IsSuccessful(out NativeList<int> foundPath)
        {
            foundPath = _ResultingPath;
            return _ResultingPath.Length > 0;
        }
        
        public void Execute()
        {
            if (!PreparePathFinding(out var endNodeIndex))
            {
                return;
            }

            RunAStar(_PathFindingNodes[endNodeIndex]);
            
            if (_PathFindingNodes[endNodeIndex].ParentNodeIndex == -1)
            {
                return;
            }
            
            CalculatePath(_PathFindingNodes[endNodeIndex]);
        }

        private bool PreparePathFinding(out int endNodeIndex)
        {
            endNodeIndex = GetNodeIndex(EndNodePosition.x, EndNodePosition.y, GridSize.x);
            var endNode = _PathFindingNodes[endNodeIndex];

            if (!endNode.IsWalkable)
            {
                return false;
            }
            
            var startNode = _PathFindingNodes[GetNodeIndex(StartNodePosition.x, StartNodePosition.y, GridSize.x)];
            startNode.WalkingCost = 0;
            startNode.DistanceToTarget = CalculateDistanceCost(new int2(startNode.X, startNode.Y), EndNodePosition);

            _PathFindingNodes[startNode.Index] = startNode;
            _OpenList.Add(startNode.Index);
            
            return true;
        }

        private void RunAStar(CGPathFindingNodeData endNode)
        {
            while (_OpenList.Length > 0)
            {
                var currentNodeIndex = GetLowestCostNode(_OpenList, _PathFindingNodes);
                
                if (currentNodeIndex == endNode.Index)
                {
                    break;
                }

                for (var i = _OpenList.Length - 1; i >= 0; i--)
                {
                    if (_OpenList[i] != currentNodeIndex)
                    {
                        continue;
                    }
                    
                    _OpenList.RemoveAtSwapBack(i);
                    break;
                }
                
                _ClosedList.Add(currentNodeIndex);
                CheckNeighbourNodes(_PathFindingNodes[currentNodeIndex]);
            }
        }

        private void CheckNeighbourNodes(CGPathFindingNodeData currentNode)
        {
            foreach (var neighbourOffset in _NeighbourOffsetArray)
            {
                var neighbourPosition = new int2(currentNode.X + neighbourOffset.x, currentNode.Y + neighbourOffset.y);

                if (!IsInsideGrid(neighbourPosition, GridSize))
                {
                    continue;
                }

                var neighbourNodeIndex = GetNodeIndex(neighbourPosition.x, neighbourPosition.y, GridSize.x);

                if (_ClosedList.Contains(neighbourNodeIndex))
                {
                    continue;
                }

                var neighbourNode = _PathFindingNodes[neighbourNodeIndex];

                if (!neighbourNode.IsWalkable)
                {
                    _ClosedList.Add(neighbourNode.Index);
                    continue;
                }
                    
                var tentativeWalkingCost = currentNode.WalkingCost + CalculateDistanceCost(new int2(currentNode.X, currentNode.Y), neighbourPosition);

                if (!(tentativeWalkingCost < neighbourNode.WalkingCost))
                {
                    continue;
                }
                    
                neighbourNode.ParentNodeIndex = currentNode.Index;
                neighbourNode.WalkingCost = tentativeWalkingCost;
                neighbourNode.DistanceToTarget = CalculateDistanceCost(neighbourPosition, EndNodePosition);
                _PathFindingNodes[neighbourNodeIndex] = neighbourNode;

                if (!_OpenList.Contains(neighbourNodeIndex))
                {
                    _OpenList.Add(neighbourNodeIndex);
                }
            }
        }
        
        private void CalculatePath(CGPathFindingNodeData endNode)
        {
            if (endNode.ParentNodeIndex == -1) 
            {
                return;
            }

            _ResultingPath.Add(endNode.Index);

            var currentNode = endNode;
            
            while (currentNode.ParentNodeIndex != -1) 
            {
                var parentNode = _PathFindingNodes[currentNode.ParentNodeIndex];
                _ResultingPath.Add(parentNode.Index);
                currentNode = parentNode;
            }
        }

        public void Dispose()
        {
            _OpenList.Dispose();
            _ClosedList.Dispose();
            _NeighbourOffsetArray.Dispose();
        }

        private static int GetNodeIndex(int x, int y, int gridSizeX)
        {
            return gridSizeX * y + x;
        }
        
        private static float CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            var xDistance = math.abs(aPosition.x - bPosition.x);
            var yDistance = math.abs(aPosition.y - bPosition.y);
            var remaining = math.abs(xDistance - yDistance);
            return CGPathFindingManager.MoveDiagonalCostModifier * math.min(xDistance, yDistance) + CGPathFindingManager.MoveStraightCostModifier * remaining;
        }
        
        private static int GetLowestCostNode(NativeList<int> openList, NativeArray<CGPathFindingNodeData> pathFindingNodes) 
        {
            var lowestCostNode = pathFindingNodes[openList[0]];
            
            for (var i = 1; i < openList.Length; i++)
            {
                var testedPathFindingNode = pathFindingNodes[openList[i]];
                
                if (testedPathFindingNode.TotalCost < lowestCostNode.TotalCost) 
                {
                    lowestCostNode = testedPathFindingNode;
                }
            }
            
            return lowestCostNode.Index;
        }
        
        private static bool IsInsideGrid(int2 gridPosition, int2 gridSize) 
        {
            return gridPosition is {x: >= 0, y: >= 0} &&
                   gridPosition.x < gridSize.x &&
                   gridPosition.y < gridSize.y;
        }
    }
}
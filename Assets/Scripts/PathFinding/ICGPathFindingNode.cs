using System.Collections.Generic;
using CobbleGames.Grid;
using UnityEngine;

namespace CobbleGames.PathFinding
{
    public interface ICGPathFindingNode : ICGGridElement
    {
        Vector3 NodePosition { get; }
        
        float GCost { get; set; }
        float HCost { get; set; }
        float FCost => GCost + HCost;
        
        bool IsWalkable { get; }
        
        IReadOnlyCollection<ICGPathFindingNode> NeighbourNodes { get; }
        ICGPathFindingNode ParentNode { get; set; }
    }
}
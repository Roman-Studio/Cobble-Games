using CobbleGames.Grid;
using UnityEngine;

namespace CobbleGames.PathFinding
{
    public interface ICGPathFindingNode : ICGGridElement
    {
        Vector3 NodePosition { get; }
        float WalkingCost { get; }
        bool IsWalkable { get; }
    }
}
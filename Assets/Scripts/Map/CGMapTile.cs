using CobbleGames.Grid;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Map
{
    public class CGMapTile : MonoBehaviour, ICGGridElement
    {
        [SerializeField]
        private bool _IsWalkable = true;

        [SerializeField]
        private float _TileMovementCost = 1f;

        [field: SerializeField, ReadOnly]
        public int X { get; set; }
        
        [field: SerializeField, ReadOnly]
        public int Y { get; set; }
    }
}
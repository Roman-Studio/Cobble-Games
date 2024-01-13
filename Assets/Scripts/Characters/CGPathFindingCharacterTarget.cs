using CobbleGames.Map;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Characters
{
    [System.Serializable]
    public class CGPathFindingCharacterTarget
    {
        [field: SerializeField, ReadOnly]
        public CGCharacter TargetCharacter { get; private set; }
        
        [field: SerializeField, ReadOnly]
        public CGMapTile TargetTile { get; private set; }

        public CGPathFindingCharacterTarget(CGCharacter targetCharacter, CGMapTile targetTile)
        {
            TargetCharacter = targetCharacter;
            TargetTile = targetTile;
        }
    }
}
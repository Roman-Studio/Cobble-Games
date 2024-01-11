using UnityEngine;

namespace CobbleGames.Characters.Presets
{
    [CreateAssetMenu(fileName = "CharacterSpawnPositionPreset", menuName = "Cobble Games/Characters/Spawn Position Preset")]
    public class CGCharacterSpawnPositionPreset : ScriptableObject
    {
        [field: SerializeField] 
        public int MinX { get; private set; } = 15;

        [field: SerializeField] 
        public int MaxX { get; private set; } = 30;

        [field: SerializeField] 
        public int MinY { get; private set; } = 15;

        [field: SerializeField] 
        public int MaxY { get; private set; } = 30;
    }
}
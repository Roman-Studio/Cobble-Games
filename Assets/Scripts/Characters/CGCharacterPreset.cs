using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Characters
{
    [CreateAssetMenu(fileName = "CharacterPreset", menuName = "Cobble Games/Characters/Preset")]
    public class CGCharacterPreset : ScriptableObject
    {
        [field: SerializeField] 
        public float MinMovementSpeed { get; private set; } = 1f;

        [field: SerializeField] 
        public float MaxMovementSpeed { get; private set; } = 5f;

        public float GetRandomMovementSpeed()
        {
            return Random.Range(MinMovementSpeed, MaxMovementSpeed);
        }

        [field: SerializeField] 
        public float MinRotationSpeed { get; private set; } = 1f;

        [field: SerializeField] 
        public float MaxRotationSpeed { get; private set; } = 5f;
        
        public float GetRandomRotationSpeed()
        {
            return Random.Range(MinRotationSpeed, MaxRotationSpeed);
        }

        [field: SerializeField] 
        public int MinStamina { get; private set; } = 50;
        
        [field: SerializeField]
        public int MaxStamina { get; private set; } = 150;
        
        public float GetRandomStamina()
        {
            return Random.Range(MinStamina, MaxStamina + 1);
        }
        
        [field: SerializeField, Expandable]
        public CGCharacterColorPreset CharacterRandomColorPreset { get; private set; }
    }
}
using UnityEngine;

namespace CobbleGames.Characters
{
    public class CGCharacter : MonoBehaviour
    {
        [field: SerializeField]
        public float MovementSpeed { get; private set; }
        
        [field: SerializeField]
        public float RotationSpeed { get; private set; }
        
        [field: SerializeField]
        public float Stamina { get; private set; }
        
        [field: SerializeField]
        public Color CharacterColor { get; private set; }

        [SerializeField]
        private Renderer _CharacterRenderer;

        public void Initialize(CGCharacterPreset characterPreset)
        {
            Initialize(characterPreset.GetRandomMovementSpeed(), characterPreset.GetRandomRotationSpeed(),
            characterPreset.GetRandomStamina(), characterPreset.CharacterRandomColorPreset.GetRandomColor());
        }

        public void Initialize(float movementSpeed, float rotationSpeed, float stamina, Color characterColor)
        {
            MovementSpeed = movementSpeed;
            RotationSpeed = rotationSpeed;
            Stamina = stamina;
            CharacterColor = characterColor;
            _CharacterRenderer.material.color = CharacterColor;
        }
    }
}
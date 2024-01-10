using UnityEngine;

namespace CobbleGames.Characters
{
    [CreateAssetMenu(fileName = "CharacterColorPreset", menuName = "Cobble Games/Characters/Color Preset")]
    public class CGCharacterColorPreset : ScriptableObject
    {
        [field: SerializeField, Range(0f, 1f)] 
        public float MinHue { get; private set; }

        [field: SerializeField, Range(0f, 1f)] 
        public float MaxHue { get; private set; } = 1f;

        [field: SerializeField, Range(0f, 1f)] 
        public float MinSaturation { get; private set; } = 0.75f;

        [field: SerializeField, Range(0f, 1f)] 
        public float MaxSaturation { get; private set; } = 1f;

        [field: SerializeField, Range(0f, 1f)] 
        public float MinColorValue { get; private set; } = 0.5f;

        [field: SerializeField, Range(0f, 1f)] 
        public float MaxColorValue { get; private set; } = 1f;

        public Color GetRandomColor()
        {
            return Random.ColorHSV(MinHue, MaxHue, MinSaturation, MaxSaturation, MinColorValue, MaxColorValue);
        }
    }
}
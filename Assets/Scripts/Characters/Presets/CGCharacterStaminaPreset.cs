using UnityEngine;

namespace CobbleGames.Characters.Presets
{
    [CreateAssetMenu(fileName = "CharacterStaminaPreset", menuName = "Cobble Games/Characters/Stamina Preset")]
    public class CGCharacterStaminaPreset : ScriptableObject
    {
        [field: SerializeField] 
        public float StaminaCostMultiplier { get; private set; } = 5f;

        [field: SerializeField, Range(0f, 100f)]
        public float StaminaRestingThreshold { get; private set; } = 50f;

        [field: SerializeField] 
        public float StaminaRegenerationTimeInterval { get; private set; } = 1f;

        [field: SerializeField] 
        public float StaminaRegenerationValue { get; private set; } = 5f;
    }
}
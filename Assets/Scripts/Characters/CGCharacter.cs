using CobbleGames.Characters.Presets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CobbleGames.Characters
{
    public class CGCharacter : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField]
        public float MovementSpeed { get; private set; }
        
        [field: SerializeField]
        public float RotationSpeed { get; private set; }
        
        [field: SerializeField]
        public float Stamina { get; private set; }

        public bool IsSelected => CGCharactersManager.Instance.SelectedCharacter == this;
        
        [field: SerializeField]
        public Color CharacterColor { get; private set; }

        [SerializeField]
        private Renderer _CharacterRenderer;
        
        [field: SerializeField]
        public UnityEvent EventCharacterSelected { get; private set; }
        
        [field: SerializeField]
        public UnityEvent EventCharacterDeselected { get; private set; }

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
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            if (CGCharactersManager.Instance == null)
            {
                return;
            }
            
            CGCharactersManager.Instance.EventSelectedCharacterChanged += OnSelectedCharacterChanged;
        }

        private void UnregisterEvents()
        {
            if (CGCharactersManager.Instance == null)
            {
                return;
            }
            
            CGCharactersManager.Instance.EventSelectedCharacterChanged -= OnSelectedCharacterChanged;
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsSelected)
            {
                DeselectCharacter();
            }
            else
            {
                SelectCharacter();
            }
        }

        public void SelectCharacter()
        {
            CGCharactersManager.Instance.SetSelectedCharacter(this);
        }

        public void DeselectCharacter()
        {
            CGCharactersManager.Instance.SetSelectedCharacter(null);
        }

        private void OnSelectedCharacterChanged()
        {
            if (IsSelected)
            {
                OnSelectCharacter();
            }
            else
            {
                OnDeselectCharacter();
            }
        }

        private void OnSelectCharacter()
        {
            EventCharacterSelected?.Invoke();
        }

        private void OnDeselectCharacter()
        {
            EventCharacterDeselected?.Invoke();
        }
    }
}
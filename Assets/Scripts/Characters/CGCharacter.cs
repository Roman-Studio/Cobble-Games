using System;
using System.Collections.Generic;
using CobbleGames.Characters.Presets;
using CobbleGames.PathFinding;
using NaughtyAttributes;
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

        [SerializeField]
        private float _CurrentStamina;
        public float CurrentStamina
        {
            get => _CurrentStamina;
            private set
            {
                _CurrentStamina = Mathf.Clamp(value, 0f, Stamina);
                EventCharacterCurrentStaminaChanged?.Invoke();
            }
        }
        
        public event Action EventCharacterCurrentStaminaChanged;

        [SerializeField, ReadOnly]
        private bool _IsResting;
        
        public bool IsResting
        {
            get => _IsResting;
            private set
            {
                _IsResting = value;
                EventIsRestingChanged?.Invoke();
            }
        }
        
        public event Action EventIsRestingChanged;

        public bool IsSelected => CGCharactersManager.Instance.SelectedCharacter == this;
        
        [field: SerializeField]
        public Color CharacterColor { get; private set; }
        
        private List<ICGPathFindingNode> _CurrentCharacterPath = new();

        [SerializeField]
        private Renderer _CharacterRenderer;

        [SerializeField]
        private CGCharacterController _CharacterController;
        
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
            CurrentStamina = Stamina;
            CharacterColor = characterColor;
            _CharacterRenderer.material.color = CharacterColor;
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _CharacterController.EventNextMovementTargetPositionChanged += UpdateCharacterSpeed;
            
            if (CGCharactersManager.Instance == null)
            {
                return;
            }
            
            CGCharactersManager.Instance.EventSelectedCharacterChanged += OnSelectedCharacterChanged;
            CGCharactersManager.Instance.EventRegenerateCharacterEnergy += RegenerateCharacterStamina;
        }

        private void UnregisterEvents()
        {
            _CharacterController.EventNextMovementTargetPositionChanged -= UpdateCharacterSpeed;
            
            if (CGCharactersManager.Instance == null)
            {
                return;
            }
            
            CGCharactersManager.Instance.EventSelectedCharacterChanged -= OnSelectedCharacterChanged;
            CGCharactersManager.Instance.EventRegenerateCharacterEnergy -= RegenerateCharacterStamina;
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

        public void MoveCharacter(Vector3 targetPosition)
        {
            if (!CGPathFindingManager.Instance.FindPath(transform.position, targetPosition, out var foundPath))
            {
                return;
            }
            
            _CurrentCharacterPath = foundPath;
            _CharacterController.SetPathVectors(foundPath);
        }

        private void UpdateCharacterSpeed()
        {
            if (_CurrentCharacterPath.Count == 0)
            {
                return;
            }

            if (IsResting && CurrentStamina / Stamina * 100f >= CGCharactersManager.Instance.CharacterStaminaPreset.StaminaRestingThreshold)
            {
                IsResting = false;
            }

            if (CurrentStamina == 0f || IsResting)
            {
                _CharacterController.MovementSpeed = 0f;
                _CharacterController.RotationSpeed = 0f;
                IsResting = true;
                return;
            }
            
            var nextPathNode = _CurrentCharacterPath[0];
            CurrentStamina -= nextPathNode.WalkingCost * CGCharactersManager.Instance.CharacterStaminaPreset.StaminaCostMultiplier;
            _CharacterController.MovementSpeed = MovementSpeed / nextPathNode.WalkingCost;
            _CharacterController.RotationSpeed = RotationSpeed / nextPathNode.WalkingCost;
            _CurrentCharacterPath.RemoveAt(0);
        }
        
        private void RegenerateCharacterStamina(float valueToAdd)
        {
            CurrentStamina += valueToAdd;
            UpdateCharacterSpeed();
        }
    }
}
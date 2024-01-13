using System;
using System.Collections.Generic;
using System.Linq;
using CobbleGames.Characters.Presets;
using CobbleGames.Characters.UI;
using CobbleGames.Map;
using CobbleGames.PathFinding;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CobbleGames.Characters
{
    public class CGCharacter : MonoBehaviour, IPointerClickHandler, ICGTileAssignable
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
        
        [SerializeField, ReadOnly]
        private CGMapTile _CurrentMapTile;

        public CGMapTile CurrentMapTile
        {
            get => _CurrentMapTile; 
            set
            {
                if (_CurrentMapTile != null)
                {
                    _PreviousMapTiles.Add(_CurrentMapTile);

                    if (_PreviousMapTiles.Count > CGCharactersManager.Instance.SpawnedCharacters.Count)
                    {
                        _PreviousMapTiles.RemoveAt(0);
                    }
                }

                _CurrentMapTile = value;
            }
        }

        [SerializeField, ReadOnly]
        private List<CGMapTile> _PreviousMapTiles = new();
        
        private List<ICGPathFindingNode> _CurrentCharacterPath = new();
        public ICGPathFindingNode NextPathNode
        {
            get
            {
                if (_CurrentCharacterPath == null)
                {
                    return null;
                }

                return _CurrentCharacterPath.Count == 0 ? null : _CurrentCharacterPath[0];
            }
        }
        
        [field: SerializeField, ReadOnly]
        public CGCharacter FollowedCharacter { get; private set; }
        
        [SerializeField, ReadOnly]
        private List<CGCharacter> _FollowingCharacters = new();

        [SerializeField]
        private Renderer _CharacterRenderer;

        [SerializeField]
        private CGCharacterController _CharacterController;

        [SerializeField]
        private CGPathDrawer _CharacterPathDrawer;

        [SerializeField]
        private CGCharacterTracker _CharacterTracker;
        
        [field: SerializeField]
        public UnityEvent EventCharacterSelected { get; private set; }
        
        [field: SerializeField]
        public UnityEvent EventCharacterDeselected { get; private set; }

        public void Initialize(CGCharacterPreset characterPreset, CGMapTile startMapTile)
        {
            Initialize(characterPreset.GetRandomMovementSpeed(), characterPreset.GetRandomRotationSpeed(),
            characterPreset.GetRandomStamina(), characterPreset.CharacterRandomColorPreset.GetRandomColor(), startMapTile);
        }

        public void Initialize(float movementSpeed, float rotationSpeed, float stamina, Color characterColor, CGMapTile startMapTile)
        {
            MovementSpeed = movementSpeed;
            RotationSpeed = rotationSpeed;
            Stamina = stamina;
            CurrentStamina = Stamina;
            
            CharacterColor = characterColor;
            _CharacterRenderer.material.color = CharacterColor;
            _CharacterPathDrawer.UpdateLineColor(CharacterColor);
            
            _CharacterTracker.Set(this);
            
            startMapTile.AssignObject(this);
            UpdateCharacterPathDrawer();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _CharacterController.EventNextMovementTargetPositionChanged += OnNextMovementTargetPositionChanged;
            _CharacterController.EventCurrentPathChanged += OnCurrentPathChanged;

            if (CGCharactersManager.Instance != null)
            {
                CGCharactersManager.Instance.EventSelectedCharacterChanged += OnSelectedCharacterChanged;
                CGCharactersManager.Instance.EventRegenerateCharacterEnergy += RegenerateCharacterStamina;
            }

            if (CGMapManager.Instance != null)
            {
                CGMapManager.Instance.EventAnyGridStateChanged += UpdateFollowingCharacters;
            }
        }

        private void UnregisterEvents()
        {
            _CharacterController.EventNextMovementTargetPositionChanged -= OnNextMovementTargetPositionChanged;
            _CharacterController.EventCurrentPathChanged -= OnCurrentPathChanged;

            if (CGCharactersManager.Instance != null)
            {
                CGCharactersManager.Instance.EventSelectedCharacterChanged -= OnSelectedCharacterChanged;
                CGCharactersManager.Instance.EventRegenerateCharacterEnergy -= RegenerateCharacterStamina;
            }
            
            if (CGMapManager.Instance != null)
            {
                CGMapManager.Instance.EventAnyGridStateChanged -= UpdateFollowingCharacters;
            }
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
            ClearFollowingCharacters();
            EventCharacterDeselected?.Invoke();
        }

        public void MoveCharacter(Vector3 targetPosition)
        {
            MoveCharacter(transform.position, targetPosition);
        }
        
        public void MoveCharacter(Vector3 startPosition, Vector3 targetPosition)
        {
            if (!CGPathFindingManager.Instance.FindPath(startPosition, targetPosition, out var foundPath))
            {
                return;
            }
            
            _CurrentCharacterPath = foundPath;
            _CharacterController.SetPathVectors(foundPath);
        }

        private void OnNextMovementTargetPositionChanged()
        {
            if (_CurrentCharacterPath == null || _CurrentCharacterPath.Count == 0)
            {
                return;
            }

            if (NextPathNode is CGMapTile mapTile)
            {
                mapTile.AssignObject(this);
            }
            
            CurrentStamina -= NextPathNode.WalkingCost * CGCharactersManager.Instance.CharacterStaminaPreset.StaminaCostMultiplier;
            _CurrentCharacterPath.RemoveAt(0);
            UpdateCharacterPathDrawer();
            UpdateCharacterSpeed();
            UpdateFollowingCharacters();
        }

        private void OnCurrentPathChanged()
        {
            UpdateCharacterPathDrawer();
            UpdateCharacterSpeed();
        }

        private void UpdateCharacterSpeed()
        {
            if (_CurrentCharacterPath == null || _CurrentCharacterPath.Count == 0)
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
            
            var nextPathNode = NextPathNode;
            _CharacterController.MovementSpeed = MovementSpeed / nextPathNode.WalkingCost;
            _CharacterController.RotationSpeed = RotationSpeed / nextPathNode.WalkingCost;
        }
        
        private void RegenerateCharacterStamina(float valueToAdd)
        {
            CurrentStamina += valueToAdd;
            UpdateCharacterSpeed();
        }
        
        private void UpdateCharacterPathDrawer()
        {
            #if PATHFINDING_DEBUG
            _CharacterPathDrawer.UpdateLine(_CurrentCharacterPath);
            #endif
        }

        public void FollowCharacter(CGCharacter followedCharacter)
        {
            if (FollowedCharacter != null)
            {
                FollowedCharacter._FollowingCharacters.Remove(this);
            }
            
            FollowedCharacter = followedCharacter;

            if (followedCharacter == null)
            {
                return;
            }
            
            FollowedCharacter._FollowingCharacters.Add(this);
        }

        private void UpdateFollowingCharacters()
        {
            var followingCharactersByDistance = _FollowingCharacters.OrderBy(character => character._CurrentCharacterPath.Count).ToList();
            
            for (var i = 0; i < followingCharactersByDistance.Count; i++)
            {
                if (!TryGetTargetNodeForFollowingCharacter(i, out var foundPathFindingNode))
                {
                    continue;
                }
                
                var characterToMove = followingCharactersByDistance[i];
                characterToMove.MoveCharacter(characterToMove.NextPathNode?.NodePosition ?? characterToMove.transform.position, foundPathFindingNode.NodePosition);
            }
        }

        private bool TryGetTargetNodeForFollowingCharacter(int followingCharacterIndex, out ICGPathFindingNode foundPathFindingNode)
        {
            foundPathFindingNode = default;
            
            if (followingCharacterIndex < _PreviousMapTiles.Count)
            {
                foundPathFindingNode = _PreviousMapTiles[^(followingCharacterIndex + 1)];
                return true;
            }

            var oldestPathNode = _PreviousMapTiles.Count > 0 ? _PreviousMapTiles[^1] : _CurrentMapTile;
            foundPathFindingNode = oldestPathNode.NeighbourMapTiles.FirstOrDefault(node => node.IsWalkable);
            return foundPathFindingNode != default;
        }

        private void ClearFollowingCharacters()
        {
            foreach (var character in _FollowingCharacters)
            {
                character.FollowedCharacter = null;
            }
            
            _FollowingCharacters.Clear();
        }
    }
}
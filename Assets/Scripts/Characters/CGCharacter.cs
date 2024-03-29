﻿using System;
using System.Collections.Generic;
using System.Linq;
using CobbleGames.Characters.Presets;
using CobbleGames.Characters.UI;
using CobbleGames.Core;
using CobbleGames.Map;
using CobbleGames.PathFinding;
using CobbleGames.SaveSystem;
using CobbleGames.SaveSystem.Data;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CobbleGames.Characters
{
    public class CGCharacter : MonoBehaviour, IPointerClickHandler, ICGTileAssignable, ICGGameSaveObject
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

        [SerializeField]
        private Color _CharacterColor;

        public Color CharacterColor
        {
            get => _CharacterColor;
            private set
            {
                _CharacterColor = value;
                _CharacterRenderer.material.color = CharacterColor;
                _CharacterPathDrawer.UpdateLineColor(CharacterColor);
            }
        }
        
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
            if (CurrentMapTile != null)
            {
                CurrentMapTile.AssignObject(null);
            }

            Addressables.ReleaseInstance(gameObject);
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
            
            CurrentStamina -= NextPathNode.WalkingCost * CGCharactersManager.Instance.CharacterStaminaPreset.StaminaCostMultiplier;
            _CurrentCharacterPath.RemoveAt(0);
            UpdateCharacterPathDrawer();
            UpdateCharacterSpeed();
            
            if (!IsResting && NextPathNode is CGMapTile mapTile)
            {
                mapTile.AssignObject(this);
            }
            
            UpdateFollowingCharacters();
        }

        private void OnCurrentPathChanged()
        {
            UpdateCharacterPathDrawer();
            UpdateCharacterSpeed();
        }

        private void UpdateCharacterSpeed()
        {
            if (IsResting && CurrentStamina / Stamina * 100f >= CGCharactersManager.Instance.CharacterStaminaPreset.StaminaRestingThreshold)
            {
                IsResting = false;
            }
            
            if (_CurrentCharacterPath == null || _CurrentCharacterPath.Count == 0)
            {
                return;
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

    #region Save System

        private const string PositionSaveKey = "Position";
        private const string RotationSaveKey = "Rotation";
        
        public CGSaveDataEntryDictionary GetSaveData()
        {
            var saveData = new CGSaveDataEntryDictionary();

            SaveCharacterTransform(saveData);
            SaveCharacterStats(saveData);
            SaveCharacterColor(saveData);
            SaveCurrentTile(saveData);
            return saveData;
        }

        private void SaveCharacterTransform(CGSaveDataEntryDictionary saveData)
        {
            saveData.TryAddDataEntry(PositionSaveKey, transform.position.GetSaveData());
            saveData.TryAddDataEntry(RotationSaveKey, transform.eulerAngles.GetSaveData());
        }

        private void SaveCharacterStats(CGSaveDataEntryDictionary saveData)
        {
            saveData.TryAddDataEntry(nameof(MovementSpeed), new CGSaveDataEntryFloat(MovementSpeed));
            saveData.TryAddDataEntry(nameof(RotationSpeed), new CGSaveDataEntryFloat(RotationSpeed));
            saveData.TryAddDataEntry(nameof(Stamina), new CGSaveDataEntryFloat(Stamina));
            saveData.TryAddDataEntry(nameof(CurrentStamina), new CGSaveDataEntryFloat(CurrentStamina));
            saveData.TryAddDataEntry(nameof(IsResting), new CGSaveDataEntryBool(IsResting));
        }

        private void SaveCharacterColor(CGSaveDataEntryDictionary saveData)
        {
            saveData.TryAddDataEntry(nameof(CharacterColor), CharacterColor.GetSaveData());
        }

        private void SaveCurrentTile(CGSaveDataEntryDictionary saveData)
        {
            var currentTileSaveData = new CGSaveDataEntryList();
            currentTileSaveData.AddListEntry(new CGSaveDataEntryInt(CurrentMapTile.X));
            currentTileSaveData.AddListEntry(new CGSaveDataEntryInt(CurrentMapTile.Y));
            
            saveData.TryAddDataEntry(nameof(CurrentMapTile), currentTileSaveData);
        }

        public void LoadDataFromSave(CGSaveDataEntryDictionary saveData)
        {
            LoadCharacterTransform(saveData);
            LoadCharacterStats(saveData);
            LoadCharacterColor(saveData);
            LoadCurrentTile(saveData);
        }

        private void LoadCharacterTransform(CGSaveDataEntryDictionary saveData)
        {
            if (saveData.TryGetDataEntry(PositionSaveKey, out CGSaveDataEntryList positionDataList))
            {
                if (positionDataList.TryLoadFromSaveData(out Vector3 position))
                {
                    transform.position = position;
                }
            }

            if (!saveData.TryGetDataEntry(RotationSaveKey, out CGSaveDataEntryList rotationDataList))
            {
                return;
            }
            
            if (rotationDataList.TryLoadFromSaveData(out Vector3 rotation))
            {
                transform.eulerAngles = rotation;
            }
        }

        private void LoadCharacterStats(CGSaveDataEntryDictionary saveData)
        {
            if (saveData.TryGetDataValue(nameof(MovementSpeed), out float movementSpeed))
            {
                MovementSpeed = movementSpeed;
            }
            
            if (saveData.TryGetDataValue(nameof(RotationSpeed), out float rotationSpeed))
            {
                RotationSpeed = rotationSpeed;
            }
            
            if (saveData.TryGetDataValue(nameof(Stamina), out float stamina))
            {
                Stamina = stamina;
            }
            
            if (saveData.TryGetDataValue(nameof(CurrentStamina), out float currentStamina))
            {
                CurrentStamina = currentStamina;
            }
            
            if (saveData.TryGetDataValue(nameof(IsResting), out bool isResting))
            {
                IsResting = isResting;
            }
        }

        private void LoadCharacterColor(CGSaveDataEntryDictionary saveData)
        {
            if (!saveData.TryGetDataEntry(nameof(CharacterColor), out CGSaveDataEntryList characterColorDataList))
            {
                return;
            }
            
            if (characterColorDataList.TryLoadFromSaveData(out Color characterColor))
            {
                CharacterColor = characterColor;
            }
        }

        private void LoadCurrentTile(CGSaveDataEntryDictionary saveData)
        {
            if (!saveData.TryGetDataEntry(nameof(CurrentMapTile), out CGSaveDataEntryList currentMapTileDataList))
            {
                return;
            }
            
            var gridX = (int)currentMapTileDataList.DataList[0].GetData();
            var gridY = (int)currentMapTileDataList.DataList[1].GetData();
                
            if(!CGMapManager.Instance.GeneratedGrid.TryGetElement(gridX, gridY, out var foundTile))
            {
                Debug.LogError($"[{nameof(CGCharacter)}.{nameof(LoadCurrentTile)}] Failed to get map tile at ({gridX}, {gridY})!", this);
                return;
            }

            CurrentMapTile.AssignObject(null);
            CurrentMapTile = foundTile;
            CurrentMapTile.AssignObject(this);
        }
        
    #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CobbleGames.Characters.Presets;
using CobbleGames.Core;
using CobbleGames.Map;
using CobbleGames.PathFinding;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CobbleGames.Characters
{
    public class CGCharactersManager : CGManager<CGCharactersManager>
    {
        [SerializeField]
        private int _CharactersToSpawn = 3;

        [SerializeField, Expandable]
        private List<CGCharacterPreset> _CharacterPresets = new();

        [SerializeField]
        private CGCharacter _CharacterPrefab;

        [SerializeField, Expandable]
        private CGCharacterSpawnPositionPreset _CharacterSpawnPositionPreset;
        
        [field: SerializeField]
        public CGCharacterStaminaPreset CharacterStaminaPreset { get; private set; }
        
        [SerializeField, ReadOnly]
        private List<CGCharacter> _SpawnedCharacters = new();
        public IReadOnlyList<CGCharacter> SpawnedCharacters => _SpawnedCharacters;

        public event Action EventSpawnedCharactersChanged;
        
        [field: SerializeField, ReadOnly]
        public CGCharacter SelectedCharacter { get; private set; }

        public event Action EventSelectedCharacterChanged;

        private Coroutine _EnergyRegenerationCoroutine;

        public event Action<float> EventRegenerateCharacterEnergy;
        
        private readonly Dictionary<CGCharacter, ICGPathFindingNode> _CharacterPathFindingTargets = new();
        public IReadOnlyDictionary<CGCharacter, ICGPathFindingNode> CharacterPathFindingTargets => _CharacterPathFindingTargets;
        
        public void Initialize()
        {
            SpawnRandomCharacters(_CharactersToSpawn);
            StartEnergyRegenerator();
        }

        private void SpawnRandomCharacters(int charactersToSpawn)
        {
            for (var i = 0; i < charactersToSpawn; i++)
            {
                SpawnRandomCharacter();
            }
        }

        [Button]
        private void SpawnRandomCharacter()
        {
            if (_CharacterPresets.Count == 0)
            {
                Debug.LogError($"[{nameof(CGCharactersManager)}.{nameof(SpawnRandomCharacter)}] There are no character presets assigned in the {nameof(CGCharactersManager)}!", this);
                return;
            }
            
            var randomPreset = _CharacterPresets[Random.Range(0, _CharacterPresets.Count)];

            if (!TryGetRandomMapTile(out var randomMapTile))
            {
                Debug.LogError($"[{nameof(CGCharactersManager)}.{nameof(SpawnRandomCharacter)}] Failed to find random map tile!", this);
                return;
            }

            var newCharacter = Instantiate(_CharacterPrefab, transform).GetComponent<CGCharacter>();
            newCharacter.transform.position = randomMapTile.TileSurfaceCenter.position;
            newCharacter.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            newCharacter.Initialize(randomPreset, randomMapTile);
            _SpawnedCharacters.Add(newCharacter);
            EventSpawnedCharactersChanged?.Invoke();
        }
        
        private bool TryGetRandomMapTile(out CGMapTile randomMapTile)
        {
            randomMapTile = default;

            if (_CharacterSpawnPositionPreset == null)
            {
                return false;
            }
            
            var availableMapTiles = CGMapManager.Instance.GeneratedGrid.GridElements.Where(mapTile =>
                mapTile.IsWalkable && _SpawnedCharacters.All(character => character.transform.position != mapTile.TileSurfaceCenter.position) &&
                mapTile.X >= _CharacterSpawnPositionPreset.MinX && mapTile.X <= _CharacterSpawnPositionPreset.MaxX &&
                mapTile.Y >= _CharacterSpawnPositionPreset.MinY && mapTile.Y <= _CharacterSpawnPositionPreset.MaxY).ToList();

            if (availableMapTiles.Count == 0)
            {
                return false;
            }

            randomMapTile = availableMapTiles[Random.Range(0, availableMapTiles.Count)];
            return true;
        }

        public void SetSelectedCharacter(CGCharacter selectedCharacter)
        {
            var hasChanged = SelectedCharacter != selectedCharacter;
            SelectedCharacter = selectedCharacter;

            if (hasChanged)
            {
                EventSelectedCharacterChanged?.Invoke();
            }
        }

        private void StartEnergyRegenerator()
        {
            if (_EnergyRegenerationCoroutine != null)
            {
                StopCoroutine(_EnergyRegenerationCoroutine);
            }

            _EnergyRegenerationCoroutine = StartCoroutine(EnergyRegenerator());
        }

        private IEnumerator EnergyRegenerator()
        {
            var delay = new WaitForSeconds(CharacterStaminaPreset.StaminaRegenerationTimeInterval);
            
            while (true)
            {
                yield return delay;
                EventRegenerateCharacterEnergy?.Invoke(CharacterStaminaPreset.StaminaRegenerationValue);
            }
        }

        public void AddCharacterPathFindingTarget(CGCharacter character, ICGPathFindingNode targetMapTile)
        {
            _CharacterPathFindingTargets[character] = targetMapTile;
        }

        public void RemoveCharacterPathFindingTarget(CGCharacter character)
        {
            _CharacterPathFindingTargets.Remove(character);
        }
    }
}
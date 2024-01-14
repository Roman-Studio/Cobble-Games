using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CobbleGames.Characters.Presets;
using CobbleGames.Core;
using CobbleGames.Map;
using CobbleGames.PathFinding;
using CobbleGames.SaveSystem;
using CobbleGames.SaveSystem.Data;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CobbleGames.Characters
{
    public class CGCharactersManager : CGManager<CGCharactersManager>, ICGGameSaveClient, IComparable<CGCharactersManager>
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
        private ICGGameSaveClient _GameSaveClientImplementation;

        public event Action<float> EventRegenerateCharacterEnergy;

        public void Initialize()
        {
            SpawnRandomCharacters(_CharactersToSpawn);
            StartEnergyRegenerator();
        }

        private void SpawnRandomCharacters(int charactersToSpawn)
        {
            for (var i = 0; i < charactersToSpawn; i++)
            {
                TrySpawnRandomCharacter(out _);
            }
        }

        [Button]
        private bool TrySpawnRandomCharacter(out CGCharacter spawnedCharacter)
        {
            spawnedCharacter = null;
            
            if (_CharacterPresets.Count == 0)
            {
                Debug.LogError($"[{nameof(CGCharactersManager)}.{nameof(TrySpawnRandomCharacter)}] There are no character presets assigned in the {nameof(CGCharactersManager)}!", this);
                return false;
            }

            if (!TryGetRandomMapTile(out var randomMapTile))
            {
                Debug.LogError($"[{nameof(CGCharactersManager)}.{nameof(TrySpawnRandomCharacter)}] Failed to find random map tile!", this);
                return false;
            }

            var randomPreset = _CharacterPresets[Random.Range(0, _CharacterPresets.Count)];
            
            spawnedCharacter = Instantiate(_CharacterPrefab, transform).GetComponent<CGCharacter>();
            spawnedCharacter.transform.position = randomMapTile.TileSurfaceCenter.position;
            spawnedCharacter.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            spawnedCharacter.Initialize(randomPreset, randomMapTile);
            
            _SpawnedCharacters.Add(spawnedCharacter);
            EventSpawnedCharactersChanged?.Invoke();
            return true;
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

        private void DestroyAllSpawnedCharacters()
        {
            foreach (var character in _SpawnedCharacters)
            {
                Destroy(character.gameObject);
            }
            
            _SpawnedCharacters.Clear();
        }

        public string ClientID => nameof(CGCharactersManager);
        public int LoadOrder => 1;

        public CGSaveDataEntryDictionary GetSaveData()
        {
            var result = new CGSaveDataEntryDictionary();

            result.TryAddDataEntry(nameof(SpawnedCharacters), SpawnedCharacters.GetSaveData());

            return result;
        }

        public void LoadDataFromSave(CGSaveDataEntryDictionary saveData)
        {
            LoadSpawnedCharacters(saveData);
        }

        private void LoadSpawnedCharacters(CGSaveDataEntryDictionary saveData)
        {
            if (!saveData.TryGetDataEntry(nameof(SpawnedCharacters), out CGSaveDataEntryList spawnedCharactersDataList))
            {
                return;
            }

            DestroyAllSpawnedCharacters();

            foreach (var characterDataEntry in spawnedCharactersDataList.DataList.OfType<CGSaveDataEntryDictionary>())
            {
                if (!TrySpawnRandomCharacter(out var spawnedCharacter))
                {
                    continue;
                }
                
                spawnedCharacter.LoadDataFromSave(characterDataEntry);
            }
        }

        public int CompareTo(CGCharactersManager other)
        {
            throw new NotImplementedException();
        }
    }
}
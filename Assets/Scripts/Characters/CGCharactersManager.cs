using System.Collections.Generic;
using System.Linq;
using CobbleGames.Core;
using CobbleGames.Map;
using NaughtyAttributes;
using UnityEngine;

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
        
        [SerializeField, ReadOnly]
        private List<CGCharacter> _SpawnedCharacters = new();
        
        public override void Initialize()
        {
            SpawnRandomCharacters(_CharactersToSpawn);
        }

        private void SpawnRandomCharacters(int charactersToSpawn)
        {
            for (var i = 0; i < charactersToSpawn; i++)
            {
                SpawnRandomCharacter();
            }
        }

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
            newCharacter.Initialize(randomPreset);
            _SpawnedCharacters.Add(newCharacter);
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
    }
}
using System;
using CobbleGames.Core;
using CobbleGames.Grid;
using CobbleGames.SaveSystem;
using CobbleGames.SaveSystem.Data;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CobbleGames.Map
{
    public class CGMapManager : CGManager<CGMapManager>, ICGGameSaveClient
    {
        [SerializeField]
        private CGMapGenerator _MapGenerator;

        [field: SerializeField, ReadOnly]
        public CGGrid<CGMapTile> GeneratedGrid { get; private set; }

        [ShowNonSerializedField, ReadOnly]
        private int _CurrentMapSeed;

        public bool IsGeneratingMap => _MapGenerator.IsGenerating;

        public event Action EventAnyGridStateChanged;
        public void InvokeEventAnyGridStateChanged() => EventAnyGridStateChanged?.Invoke();
        
        public void Initialize()
        {
            GenerateRandomMap();
        }

        private bool IsPlaying => Application.isPlaying;

        [Button, EnableIf(nameof(IsPlaying))]
        private void GenerateRandomMap()
        {
            GenerateMapFromSeed(DateTime.Now.GetHashCode());
        }

        private void GenerateMapFromSeed(int seed)
        {
            GeneratedGrid.ForEach(DestroyTile);
            _CurrentMapSeed = seed;
            _MapGenerator.GenerateNewMap((uint)_CurrentMapSeed, out var generatedMapGrid);
            GeneratedGrid = generatedMapGrid;
        }

        private static void DestroyTile(CGMapTile mapTile)
        {
            if (mapTile == null)
            {
                return;
            }

            Addressables.ReleaseInstance(mapTile.gameObject);
            Destroy(mapTile.gameObject);
        }

        public string ClientID => nameof(CGMapManager);
        public int LoadOrder => 0;
        public bool IsLoading { get; private set; }

        public CGSaveDataEntryDictionary GetSaveData()
        {
            var saveData = new CGSaveDataEntryDictionary();
            saveData.TryAddDataEntry(nameof(_CurrentMapSeed), new CGSaveDataEntryInt(_CurrentMapSeed));
            return saveData;
        }

        public void LoadDataFromSave(CGSaveDataEntryDictionary saveData)
        {
            IsLoading = true;
            _MapGenerator.EventMapGenerationFinished += OnMapGenerationFinished;
            
            if (saveData.TryGetDataValue(nameof(_CurrentMapSeed), out _CurrentMapSeed))
            {
                GenerateMapFromSeed(_CurrentMapSeed);
            }
            else
            {
                IsLoading = false;
            }
        }

        private void OnMapGenerationFinished()
        {
            _MapGenerator.EventMapGenerationFinished -= OnMapGenerationFinished;
            IsLoading = false;
        }
    }
}
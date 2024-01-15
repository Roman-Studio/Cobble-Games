using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CobbleGames.Core;
using CobbleGames.SaveSystem.Data;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CobbleGames.SaveSystem
{
    public class CGSaveManager : CGManager<CGSaveManager>
    {
        [SerializeField]
        private Environment.SpecialFolder _SaveTargetSpecialFolder = Environment.SpecialFolder.MyDocuments;

        [field: SerializeField] 
        public string SaveFileExtension { get; private set; } = ".sav";

        public string SaveFileDirectoryPath => Path.Combine(Environment.GetFolderPath(_SaveTargetSpecialFolder), Application.productName, "Saves");
        
        private readonly List<ICGGameSaveClient> _GameSaveClients = new();
        public IReadOnlyCollection<ICGGameSaveClient> GameSaveClients => _GameSaveClients;

        private List<CGGameSave> _CurrentGameSaves = new ();
        public IReadOnlyList<CGGameSave> CurrentGameSaves => _CurrentGameSaves;

        public event Action EventCurrentGameSavesChanged;

        public event Action EventStartGameSave;
        public event Action EventEndGameSave;
        
        public event Action EventStartGameLoad;
        public event Action EventEndGameLoad;

        public bool AddGameSaveClient(ICGGameSaveClient gameSaveClient)
        {
            if (_GameSaveClients.Contains(gameSaveClient))
            {
                return false;
            }
            
            _GameSaveClients.AddSorted(gameSaveClient);
            return true;
        }

        public bool TryRemoveGameSaveClient(ICGGameSaveClient gameSaveClient)
        {
            return _GameSaveClients.Remove(gameSaveClient);
        }

        public async void LoadAvailableSaves()
        {
            if (!EnsureSaveDirectoryExists())
            {
                return;
            }

            var saveFileDirectoryPath = SaveFileDirectoryPath;
            var saveFileExtension = SaveFileExtension;
            var getSavesListTask = Task.Run(() => GetSavesList(saveFileDirectoryPath, saveFileExtension));
            
            await getSavesListTask;
            
            _CurrentGameSaves = getSavesListTask.Result;
            EventCurrentGameSavesChanged?.Invoke();
        }
        
        private bool EnsureSaveDirectoryExists()
        {
            try
            {
                var saveFileDirectory = SaveFileDirectoryPath;
                
                if (!Directory.Exists(saveFileDirectory))
                {
                    Directory.CreateDirectory(saveFileDirectory);
                }

                return true;
            } 
            catch (Exception exception)
            {
                Debug.LogError($"[{nameof(CGSaveManager)}.{nameof(EnsureSaveDirectoryExists)}] Save localization unavailable! Exception:\n{exception}", this);
                return false;
            }
        }

        private static List<CGGameSave> GetSavesList(string saveFileDirectoryPath, string saveFileExtension)
        {
            var saveFilePaths = Directory.GetFiles(saveFileDirectoryPath, $"*{saveFileExtension}");
            return saveFilePaths.Select(saveFilePath => new CGGameSave(saveFilePath)).ToList();
        }

        public async void SaveGame(string saveName)
        {
            if (!EnsureSaveDirectoryExists())
            {
                return;
            }
            
            EventStartGameSave?.Invoke();
            
            var savePath = Path.Combine(SaveFileDirectoryPath, $"{saveName}{SaveFileExtension}");
            Debug.Log($"[{nameof(CGSaveManager)}.{nameof(SaveGame)}] Saving game at: {savePath}", this);

            var isOverride = File.Exists(savePath);
            var saveData = GetSaveDataFromRegisteredClients();
            var newGameSave = new CGGameSave(savePath);

            await Task.Run(() => SerializeToFile(newGameSave.SavePath, saveData));

            if (!isOverride)
            {
                _CurrentGameSaves.Add(newGameSave);
            }

            EventCurrentGameSavesChanged?.Invoke();
            EventEndGameSave?.Invoke();
            Debug.Log($"[{nameof(CGSaveManager)}.{nameof(SaveGame)}] Game saved!", this);
        }

        private static void SerializeToFile(string saveFilePath, CGSaveDataEntryDictionary saveData)
        {
            var serializedSaveData = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(saveFilePath, serializedSaveData);
        }

        private CGSaveDataEntryDictionary GetSaveDataFromRegisteredClients()
        {
            var saveData = new CGSaveDataEntryDictionary();
            
            foreach (var saveClient in _GameSaveClients)
            {
                if (!saveData.TryAddDataEntry(saveClient.ClientID, saveClient.GetSaveData()))
                {
                    Debug.LogError($"[{nameof(CGSaveManager)}.{nameof(GetSaveDataFromRegisteredClients)}] Failed to save data from client with ID: {saveClient.ClientID}!", saveClient as Object);
                }
            }

            return saveData;
        }

        public async void LoadGame(string saveName)
        {
            if (!EnsureSaveDirectoryExists())
            {
                return;
            }

            var savePath = Path.Combine(SaveFileDirectoryPath, $"{saveName}{SaveFileExtension}");

            if (!File.Exists(savePath))
            {
                return;
            }

            EventStartGameLoad?.Invoke();
            Debug.Log($"[{nameof(CGSaveManager)}.{nameof(LoadGame)}] Loading game save file at: {savePath}", this);
            
            var getSaveDataTask = Task.Run(() => GetSaveData(savePath));
            await getSaveDataTask;

            SetSaveDataForRegisteredClients(getSaveDataTask.Result);
            EventEndGameLoad?.Invoke();
            Debug.Log($"[{nameof(CGSaveManager)}.{nameof(SaveGame)}] Game loaded!", this);
        }

        private static CGSaveDataEntryDictionary GetSaveData(string savePath)
        {
            var serializedData = File.ReadAllText(savePath);
            return JsonConvert.DeserializeObject<CGSaveDataEntryDictionary>(serializedData);
        }

        private void SetSaveDataForRegisteredClients(CGSaveDataEntryDictionary saveData)
        {
            foreach (var saveClient in _GameSaveClients)
            {
                if (!saveData.TryGetDataEntry<CGSaveDataEntryDictionary>(saveClient.ClientID, out var saveDataEntry))
                {
                    Debug.LogError($"[{nameof(CGSaveManager)}.{nameof(GetSaveDataFromRegisteredClients)}] Failed to find data for client with ID: {saveClient.ClientID}!", saveClient as Object);
                    continue;
                }
                
                saveClient.LoadDataFromSave(saveDataEntry);
            }
        }
    }
}
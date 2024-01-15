using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace CobbleGames.SaveSystem.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class CGSaveDataEntryDictionary : CGSaveDataEntry
    {
        [JsonProperty]
        private Dictionary<string, CGSaveDataEntry> _DataByID;
        
        public CGSaveDataEntryDictionary() : this(new Dictionary<string, CGSaveDataEntry>())
        {
            
        }
        
        public CGSaveDataEntryDictionary(Dictionary<string, CGSaveDataEntry> dictionary) : base(ECGSaveDataType.Dictionary)
        {
            _DataByID = dictionary;
        }
        
        public override object GetData() => _DataByID;
        
        public bool TryAddDataEntry(string dataID, CGSaveDataEntry dataEntry)
        {
            var result = _DataByID.TryAdd(dataID, dataEntry);

            #if SAVE_DEBUG
            if (!result)
            {
                Debug.LogError($"[{nameof(CGSaveDataEntryDictionary)}.{nameof(TryAddDataEntry)}] {dataID} entry is already present in the save data!");
            }
            #endif
            
            return result;
        }

        public bool TryGetDataEntry(string dataID, out CGSaveDataEntry saveDataEntry)
        {
            var result = _DataByID.TryGetValue(dataID, out saveDataEntry);

            #if SAVE_DEBUG
            if (!result)
            {
                Debug.LogError($"[{nameof(CGSaveDataEntryDictionary)}.{nameof(TryGetDataEntry)}] Can't find {dataID} entry in the save data!");
            }
            #endif
            
            return result;
        }

        public bool TryGetDataEntry<TDataType>(string dataID, out TDataType saveDataEntry)
            where TDataType : CGSaveDataEntry
        {
            saveDataEntry = default;
            var searchResult = TryGetDataEntry(dataID, out var foundDataEntry);

            if (!searchResult)
            {
                return false;
            }

            saveDataEntry = foundDataEntry as TDataType;
            var result = saveDataEntry != default;
            
            #if SAVE_DEBUG
            if (!result)
            {
                Debug.LogError($"[{nameof(CGSaveDataEntryDictionary)}.{nameof(TryGetDataEntry)}] Can't cast {dataID} entry to {typeof(TDataType).Name}!");
            }
            #endif
            
            return result;
        }

        public bool TryGetDataValue(string dataID, out object foundDataValue)
        {
            foundDataValue = default;
            var result = TryGetDataEntry(dataID, out var foundDataEntry);

            if (!result)
            {
                return false;
            }

            foundDataValue = foundDataEntry.GetData();
            return true;
        }

        public bool TryGetDataValue<TDataType>(string dataID, out TDataType foundDataValue)
        {
            foundDataValue = default;
            var result = TryGetDataValue(dataID, out var foundValue);

            if (!result)
            {
                return false;
            }

            if (foundValue is TDataType castedType)
            {
                foundDataValue = castedType;
                return true;
            }

            #if SAVE_DEBUG
            Debug.LogError($"[{nameof(CGSaveDataEntryDictionary)}.{nameof(TryGetDataEntry)}] Can't cast {dataID} data to {typeof(TDataType).Name}!");
            #endif
            foundDataValue = default;
            return false;
        }
    }
}
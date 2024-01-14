using System.Collections.Generic;
using CobbleGames.SaveSystem.Data;
using UnityEngine;

namespace CobbleGames.SaveSystem
{
    public static class CGSaveSystemExtensionMethods
    {
        public static CGSaveDataEntryList GetSaveData(this Vector3 position)
        {
            var result = new CGSaveDataEntryList();
            result.AddListEntry(new CGSaveDataEntryFloat(position.x));
            result.AddListEntry(new CGSaveDataEntryFloat(position.y));
            result.AddListEntry(new CGSaveDataEntryFloat(position.z));
            return result;
        }

        public static bool TryLoadFromSaveData(this CGSaveDataEntryList positionsList, out Vector3 result)
        {
            if (positionsList.DataList.Count < 3)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Given data list contains less than 3 elements!");
                #endif
                return false;
            }

            if (positionsList.DataList[0].GetData() is not float positionX)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Data list element at index: 0 is not a float!");
                #endif
                return false;
            }
            
            if (positionsList.DataList[1].GetData() is not float positionY)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Data list element at index: 1 is not a float!");
                #endif
                return false;
            }
            
            if (positionsList.DataList[2].GetData() is not float positionZ)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Data list element at index: 2 is not a float!");
                #endif
                return false;
            }

            result = new Vector3(positionX, positionY, positionZ);
            return true;
        }
        
        public static CGSaveDataEntryList GetSaveData(this Color color)
        {
            var result = new CGSaveDataEntryList();
            result.AddListEntry(new CGSaveDataEntryFloat(color.r));
            result.AddListEntry(new CGSaveDataEntryFloat(color.g));
            result.AddListEntry(new CGSaveDataEntryFloat(color.b));
            result.AddListEntry(new CGSaveDataEntryFloat(color.a));
            return result;
        }
        
        public static bool TryLoadFromSaveData(this CGSaveDataEntryList positionsList, out Color result)
        {
            if (positionsList.DataList.Count < 4)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Given data list contains less than 4 elements!");
                #endif
                return false;
            }

            if (positionsList.DataList[0].GetData() is not float red)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Data list element at index: 0 is not a float!");
                #endif
                return false;
            }
            
            if (positionsList.DataList[1].GetData() is not float green)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Data list element at index: 1 is not a float!");
                #endif
                return false;
            }
            
            if (positionsList.DataList[2].GetData() is not float blue)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Data list element at index: 2 is not a float!");
                #endif
                return false;
            }
            
            if (positionsList.DataList[3].GetData() is not float alpha)
            {
                result = default;
                #if SAVE_DEBUG
                Debug.LogError($"[{nameof(CGSaveSystemExtensionMethods)}.{nameof(TryLoadFromSaveData)}] Data list element at index: 3 is not a float!");
                #endif
                return false;
            }

            result = new Color(red, green, blue, alpha);
            return true;
        }

        public static CGSaveDataEntryList GetSaveData<TDataType>(this IEnumerable<TDataType> listToSave)
            where TDataType : ICGGameSaveObject
        {
            var result = new CGSaveDataEntryList();

            foreach (var listElement in listToSave)
            {
                result.AddListEntry(listElement.GetSaveData());
            }

            return result;
        }
    }
}
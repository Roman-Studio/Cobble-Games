using CobbleGames.SaveSystem.Data;

namespace CobbleGames.SaveSystem
{
    public interface ICGGameSaveObject
    {
        CGSaveDataEntryDictionary GetSaveData();
        void LoadDataFromSave(CGSaveDataEntryDictionary saveData);
    }
}
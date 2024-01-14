using System;
using System.IO;

namespace CobbleGames.SaveSystem
{
    public class CGGameSave
    {
        public string SavePath { get; }
        public string SaveName => Path.GetFileNameWithoutExtension(SavePath);
        public DateTime SaveTime => File.GetLastWriteTime(SavePath);

        public CGGameSave(string savePath)
        {
            SavePath = savePath;
        }

        public void OverrideSave()
        {
            CGSaveManager.Instance.SaveGame(SaveName);
        }

        public void LoadGame()
        {
            CGSaveManager.Instance.LoadGame(SaveName);
        }
    }
}
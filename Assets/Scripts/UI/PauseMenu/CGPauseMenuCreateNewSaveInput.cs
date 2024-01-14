using System;
using System.IO;
using CobbleGames.SaveSystem;
using TMPro;
using UnityEngine;

namespace CobbleGames.UI.PauseMenu
{
    public class CGPauseMenuCreateNewSaveInput : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _SaveNameInputField;

        private void OnEnable()
        {
            _SaveNameInputField.text = GetAutoSaveName();
        }
        
        private static string GetAutoSaveName()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");
        }

        public void OnEditSaveNameInputField(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                Debug.LogWarning($"[{nameof(CGPauseMenuCreateNewSaveInput)}.{nameof(OnEditSaveNameInputField)}] Entered save name cannot be empty!", this);
                _SaveNameInputField.text = GetAutoSaveName();
                return;
            }

            if (inputText.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
            {
                Debug.LogWarning($"[{nameof(CGPauseMenuCreateNewSaveInput)}.{nameof(OnEditSaveNameInputField)}] Entered save name contains not allowed characters!", this);
                _SaveNameInputField.text = GetAutoSaveName();
            }
        }

        public void CreateNewSave()
        {
            CGSaveManager.Instance.SaveGame(_SaveNameInputField.text);
        }
    }
}
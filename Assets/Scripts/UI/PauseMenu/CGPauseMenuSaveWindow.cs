using CobbleGames.SaveSystem;
using UnityEngine;
using UnityEngine.Events;

namespace CobbleGames.UI.PauseMenu
{
    public class CGPauseMenuSaveWindow : MonoBehaviour
    {
        [field: SerializeField]
        public UnityEvent EventOpenSaveWindow { get; private set; }
        
        [field: SerializeField]
        public UnityEvent EventCloseSaveWindow { get; private set; }

        [SerializeField]
        private GameObject _CreateNewSaveButton;
        
        [SerializeField]
        private CGPauseMenuSaveList _PauseMenuSaveList;

        private bool _IsOpen;
        private bool _IsSaving;

        public void OpenForSaveGame()
        {
            if (_IsOpen && _IsSaving)
            {
                CloseWindow();
                return;
            }

            _IsSaving = true;
            _IsOpen = true;
            EventOpenSaveWindow?.Invoke();
            LoadWindowContent();
        }

        public void OpenForLoadGame()
        {
            if (_IsOpen && !_IsSaving)
            {
                CloseWindow();
                return;
            }

            _IsSaving = false;
            _IsOpen = true;
            EventOpenSaveWindow?.Invoke();
            LoadWindowContent();
        }

        public void CloseWindow()
        {
            _IsOpen = false;
            EventCloseSaveWindow?.Invoke();
        }

        private void LoadWindowContent()
        {
            _CreateNewSaveButton.gameObject.SetActive(_IsSaving);
            CGSaveManager.Instance.LoadAvailableSaves();

            if (_IsSaving)
            {
                _PauseMenuSaveList.SetSavingMode();
            }
            else
            {
                _PauseMenuSaveList.SetLoadingMode();
            }
        }
    }
}
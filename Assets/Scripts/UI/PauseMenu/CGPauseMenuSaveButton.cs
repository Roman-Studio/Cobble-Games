using CobbleGames.Core;
using CobbleGames.SaveSystem;
using TMPro;
using UnityEngine;

namespace CobbleGames.UI.PauseMenu
{
    public class CGPauseMenuSaveButton : CGObserverMonoBehaviour<CGGameSave>
    {
        [SerializeField]
        private TextMeshProUGUI _SavePath;

        public bool IsSaving { get; private set; }
        
        protected override void RegisterEvents()
        {
            
        }

        protected override void UnregisterEvents()
        {
            
        }
        
        protected override void OnReactToChanges()
        {
            _SavePath.text = ObservedObject.SaveName;
        }
        
        public void SetSavingMode()
        {
            IsSaving = true;
        }

        public void SetLoadingMode()
        {
            IsSaving = false;
        }

        public void OnSaveButtonClick()
        {
            if (IsSaving)
            {
                ObservedObject.OverrideSave();
            }
            else
            {
                ObservedObject.LoadGame();
            }
        }
    }
}
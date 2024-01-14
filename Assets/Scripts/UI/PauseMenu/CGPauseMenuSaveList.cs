using System.Linq;
using CobbleGames.Core;
using CobbleGames.SaveSystem;
using UnityEngine;

namespace CobbleGames.UI.PauseMenu
{
    public class CGPauseMenuSaveList : CGMonoBehaviourPool<CGPauseMenuSaveButton, CGGameSave>
    {
        public bool IsSaving { get; private set; }
        
        protected override void Start()
        {
            base.Start();
            
            if (CGSaveManager.Instance == null)
            {
                return;
            }

            CGSaveManager.Instance.EventCurrentGameSavesChanged += ReactToChanges;
            ReactToChanges();
        }

        private void OnDestroy()
        {
            if (CGSaveManager.Instance == null)
            {
                return;
            }

            CGSaveManager.Instance.EventCurrentGameSavesChanged -= ReactToChanges;
        }

        protected override void OnReactToChanges()
        {
            BindCollection(CGSaveManager.Instance.CurrentGameSaves.OrderByDescending(save => save.SaveTime));
        }

        protected override void OnGetPoolElement(CGPauseMenuSaveButton poolElement)
        {
            if (IsSaving)
            {
                poolElement.SetSavingMode();
            }
            else
            {
                poolElement.SetLoadingMode();
            }
            
            base.OnGetPoolElement(poolElement);
        }

        public void SetSavingMode()
        {
            IsSaving = true;
            ReactToChanges();
        }

        public void SetLoadingMode()
        {
            IsSaving = false;
            ReactToChanges();
        }
    }
}
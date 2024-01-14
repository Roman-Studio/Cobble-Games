using CobbleGames.SaveSystem;
using UnityEngine;
using UnityEngine.Events;

namespace CobbleGames.UI.LoadingScreen
{
    public class CGLoadingScreen : MonoBehaviour
    {
        [field: SerializeField]
        public UnityEvent EventShowSavingScreen { get; private set; }
        
        [field: SerializeField]
        public UnityEvent EventHideSavingScreen { get; private set; }
        
        [field: SerializeField]
        public UnityEvent EventShowLoadingScreen { get; private set; }
        
        [field: SerializeField]
        public UnityEvent EventHideLoadingScreen { get; private set; }

        private void Start()
        {
            if (CGSaveManager.Instance == null)
            {
                return;
            }
            
            CGSaveManager.Instance.EventStartGameSave += ShowSavingScreen;
            CGSaveManager.Instance.EventEndGameSave += HideSavingScreen;
            CGSaveManager.Instance.EventStartGameLoad += ShowLoadingScreen;
            CGSaveManager.Instance.EventEndGameLoad += HideLoadingScreen;
        }

        private void OnDestroy()
        {
            if (CGSaveManager.Instance == null)
            {
                return;
            }
            
            CGSaveManager.Instance.EventStartGameSave -= ShowSavingScreen;
            CGSaveManager.Instance.EventEndGameSave -= HideSavingScreen;
            CGSaveManager.Instance.EventStartGameLoad -= ShowLoadingScreen;
            CGSaveManager.Instance.EventEndGameLoad -= HideLoadingScreen;
        }

        private void ShowSavingScreen()
        {
            EventShowSavingScreen?.Invoke();
        }

        private void HideSavingScreen()
        {
            EventHideSavingScreen?.Invoke();
        }

        private void ShowLoadingScreen()
        {
            EventShowLoadingScreen?.Invoke();
        }

        private void HideLoadingScreen()
        {
            EventHideLoadingScreen?.Invoke();
        }
    }
}
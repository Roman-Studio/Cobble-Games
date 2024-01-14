using CobbleGames.SaveSystem;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CobbleGames.UI.PauseMenu
{
    public class CGPauseMenu : MonoBehaviour
    {
        [field: SerializeField, ReadOnly]
        public bool IsEnabled { get; private set; }
        
        [SerializeField]
        private InputActionReference _TogglePauseMenuInputAction;

        [field: SerializeField]
        public UnityEvent EventEnablePauseMenu { get; private set; }
        
        [field: SerializeField]
        public UnityEvent EventDisablePauseMenu { get; private set; }

        private void Start()
        {
            _TogglePauseMenuInputAction.action.Enable();

            if (CGSaveManager.Instance == null)
            {
                return;
            }
            
            CGSaveManager.Instance.EventEndGameLoad += ClosePauseMenu;
        }

        private void OnDestroy()
        {
            _TogglePauseMenuInputAction.action.Disable();
            
            if (CGSaveManager.Instance == null)
            {
                return;
            }
            
            CGSaveManager.Instance.EventEndGameLoad -= ClosePauseMenu;
        }

        private void Update()
        {
            if (_TogglePauseMenuInputAction.action.WasPressedThisFrame())
            {
                TogglePauseMenu();
            }
        }

        private void TogglePauseMenu()
        {
            if (IsEnabled)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }

        private void OpenPauseMenu()
        {
            IsEnabled = true;
            Time.timeScale = 0f;
            EventEnablePauseMenu?.Invoke();
        }

        public void ClosePauseMenu()
        {
            IsEnabled = false;
            Time.timeScale = 1f;
            EventDisablePauseMenu?.Invoke();
        }

        public void OnExitButtonClick()
        {
            Application.Quit();

            #if UNITY_EDITOR
            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            #endif
        }
    }
}
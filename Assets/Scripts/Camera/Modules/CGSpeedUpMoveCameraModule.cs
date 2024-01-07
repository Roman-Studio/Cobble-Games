using UnityEngine;
using UnityEngine.InputSystem;

namespace CobbleGames.Camera.Modules
{
    public class CGSpeedUpMoveCameraModule : CGMoveCameraModuleBase
    {
        [SerializeField]
        private InputActionReference _SpeedUpCameraInputAction;
        
        public override void Initialize()
        {
            
        }

        public override void RegisterInputDelegates()
        {
            _SpeedUpCameraInputAction.action.Enable();
        }

        public override void UnregisterInputDelegates()
        {
            _SpeedUpCameraInputAction.action.Disable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_SpeedUpCameraInputAction.action.IsPressed())
            {
                CurrentSpeedModifier = MainCamera.CameraConfig.OnButtonSpeedModifier;
                return;
            }
            
            CurrentSpeedModifier = 1f;
        }

        public override void OnCalculateCamera()
        {
            
        }
    }
}
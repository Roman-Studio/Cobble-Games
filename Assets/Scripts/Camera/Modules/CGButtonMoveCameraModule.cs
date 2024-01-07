using UnityEngine;
using UnityEngine.InputSystem;

namespace CobbleGames.Camera.Modules
{
    public class CGButtonMoveCameraModule : CGMoveCameraModuleBase
    {
        [SerializeField]
        private InputActionReference _MoveForwardInputAction;
        
        [SerializeField]
        private InputActionReference _MoveBackwardsInputAction;
        
        [SerializeField]
        private InputActionReference _MoveLeftInputAction;
        
        [SerializeField]
        private InputActionReference _MoveRightInputAction;
        
        public override void Initialize()
        {
            
        }

        public override void RegisterInputDelegates()
        {
            _MoveForwardInputAction.action.Enable();
            _MoveBackwardsInputAction.action.Enable();
            _MoveLeftInputAction.action.Enable();
            _MoveRightInputAction.action.Enable();
        }

        public override void UnregisterInputDelegates()
        {
            _MoveForwardInputAction.action.Disable();
            _MoveBackwardsInputAction.action.Disable();
            _MoveLeftInputAction.action.Disable();
            _MoveRightInputAction.action.Disable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_MoveForwardInputAction.action.IsPressed())
            {
                MoveCameraForward(MainCamera.CameraConfig.MovementSpeed);
            }

            if (_MoveBackwardsInputAction.action.IsPressed())
            {
                MoveCameraBackward(MainCamera.CameraConfig.MovementSpeed);
            }
            
            if (_MoveLeftInputAction.action.IsPressed())
            {
                MoveCameraLeft(MainCamera.CameraConfig.MovementSpeed);
            }
            
            if (_MoveRightInputAction.action.IsPressed())
            {
                MoveCameraRight(MainCamera.CameraConfig.MovementSpeed);
            }
        }

        public override void OnCalculateCamera()
        {
            
        }
    }
}
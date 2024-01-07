using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CobbleGames.Camera.Modules
{
    public class CGZoomOrthographicCameraModule : CGCameraModuleBase
    {
        private static CGCamera MainCamera => CGCamera.Instance;
        
        [SerializeField]
        private InputActionReference _ZoomInputAction;
        
        public override void Initialize()
        {
            
        }

        public override void RegisterInputDelegates()
        {
            _ZoomInputAction.action.Enable();
        }

        public override void UnregisterInputDelegates()
        {
            _ZoomInputAction.action.Disable();
        }

        public override void OnUpdate()
        {
            CalculateZoom();
        }

        public override void OnCalculateCamera()
        {
            
        }

        private void CalculateZoom()
        {
            if (MainCamera.BlockCamera || EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var zoomDelta = _ZoomInputAction.action.ReadValue<float>() * MainCamera.CameraConfig.ZoomSpeed * Time.deltaTime;
            var zoomValue = Mathf.Clamp(MainCamera.UnityCamera.orthographicSize + zoomDelta, 
                MainCamera.CameraConfig.MovementBounds.MinCameraHeight, MainCamera.CameraConfig.MovementBounds.MaxCameraHeight);
            
            MainCamera.UnityCamera.orthographicSize = zoomValue;
        }
    }
}
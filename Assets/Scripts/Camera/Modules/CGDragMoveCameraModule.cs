using System;
using CobbleGames.Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CobbleGames.Camera.Modules
{
    public class CGDragMoveCameraModule : CGMoveCameraModuleBase
    {
        [SerializeField]
        private InputActionReference _EnableCameraDragInputAction;

        [SerializeField]
        private InputActionReference _CameraDragDeltaInputAction;

        [SerializeField]
        private InputActionReference _CameraDragPositionInputAction;

        [ShowNonSerializedField, ReadOnly]
        private float CachedDragPlaneHeight;

        [ShowNonSerializedField, ReadOnly]
        private Vector3 PreviousScreenPoint;

        [ShowNonSerializedField, ReadOnly]
        private Vector3 PreviousWorldPoint;

        [ShowNonSerializedField, ReadOnly]
        private bool DragActive;

        private Plane DragPlane = new (Vector3.up, Vector3.zero);

        public override void Initialize()
        {
            SetDragPlaneHeight(MainCamera.CameraConfig.DragPlaneHeight);
        }

        public override void RegisterInputDelegates()
        {
            _EnableCameraDragInputAction.action.Enable();
            _CameraDragDeltaInputAction.action.Enable();
            _CameraDragPositionInputAction.action.Enable();
        }

        public override void UnregisterInputDelegates()
        {
            _EnableCameraDragInputAction.action.Disable();
            _CameraDragDeltaInputAction.action.Disable();
            _CameraDragPositionInputAction.action.Disable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            DragCamera();
        }

        public override void OnCalculateCamera()
        {

        }

        private void DragCamera()
        {
            if (!MainCamera.CameraConfig.EnableDragMovement || MainCamera.BlockCamera)
            {
                return;
            }

            var buttonDown = _EnableCameraDragInputAction.action.WasPerformedThisFrame();

            if (buttonDown)
            {
                DragActive = true;
            }

            if (_EnableCameraDragInputAction.action.WasReleasedThisFrame())
            {
                DragActive = false;
            }
            
            var cameraDragDelta = _CameraDragDeltaInputAction.action.ReadValue<Vector2>();

            if (cameraDragDelta.x.IsInRangeInclusive(-MainCamera.CameraConfig.DragDeadzoneSize, MainCamera.CameraConfig.DragDeadzoneSize) &&
                cameraDragDelta.y.IsInRangeInclusive(-MainCamera.CameraConfig.DragDeadzoneSize, MainCamera.CameraConfig.DragDeadzoneSize))
            {
                return;
            }

            CalculateDragCamera(buttonDown, DragActive);
            MainCamera.ClampTransformToCameraLimits(MainCamera.transform);
        }

        private void CalculateDragCamera(bool buttonDown, bool buttonHeld)
        {
            if (Math.Abs(MainCamera.CameraConfig.DragPlaneHeight - CachedDragPlaneHeight) > float.Epsilon)
            {
                SetDragPlaneHeight(MainCamera.CameraConfig.DragPlaneHeight);
            }

            var screenPoint = _CameraDragPositionInputAction.action.ReadValue<Vector2>();
            var ray = MainCamera.UnityCamera.ScreenPointToRay(screenPoint);

            if (DragPlane.Raycast(ray, out var distance))
            {
                var worldPoint = ray.GetPoint(distance);

                if (buttonDown)
                {
                    PreviousWorldPoint = worldPoint;
                }

                if (buttonHeld)
                {
                    ray = MainCamera.UnityCamera.ScreenPointToRay(PreviousScreenPoint);
                    DragPlane.Raycast(ray, out distance);
                    PreviousWorldPoint = ray.GetPoint(distance);
                    var worldDelta = worldPoint - PreviousWorldPoint;
                    MainCamera.UnityCamera.transform.position -= worldDelta;
                }

                PreviousWorldPoint = worldPoint;
            }

            PreviousScreenPoint = screenPoint;
        }

        private void SetDragPlaneHeight(float planeHeight)
        {
            DragPlane = new Plane(Vector3.up, new Vector3(0f, planeHeight, 0f));
            CachedDragPlaneHeight = planeHeight;
        }
    }
}
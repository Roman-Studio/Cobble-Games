using CobbleGames.Core;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Camera.Modules
{
    public abstract class CGMoveCameraModuleBase : CGCameraModuleBase
    {
        protected static CGCamera MainCamera => CGCamera.Instance;
        
        [ShowNonSerializedField, ReadOnly]
        protected float CameraZoom;
        
        protected float NormalizedCameraZoom => CameraZoom.RemapValue(MainCamera.CameraConfig.MovementBounds.MinCameraHeight, MainCamera.CameraConfig.MovementBounds.MaxCameraHeight, 0f, 1f);

        protected static float CurrentSpeedModifier = 1f;

        protected float MovementSpeedWithZoomModifier(float speed) => speed * MainCamera.CameraConfig.GetZoomCameraMovementSpeedModifier(NormalizedCameraZoom);
        protected float GetMovementSpeedWithZoomModifier(float speed) => MovementSpeedWithZoomModifier(speed) < speed ? MovementSpeedWithZoomModifier(speed) * CurrentSpeedModifier : speed * CurrentSpeedModifier;

        public override void OnUpdate()
        {
            CameraZoom = MainCamera.UnityCamera.orthographicSize;
        }

        protected void MoveCameraForward(float speed)
        {
            if (!MainCamera.CameraConfig.EnableMovement || MainCamera.BlockCamera)
            {
                return;
            }

            var cameraTransform = MainCamera.transform;
            var cameraForward = cameraTransform.forward;
            
            MainCamera.transform.position +=  new Vector3(
                cameraForward.x * MainCamera.CameraConfig.DeltaTime * GetMovementSpeedWithZoomModifier(speed),
                0,
                cameraForward.z * MainCamera.CameraConfig.DeltaTime * GetMovementSpeedWithZoomModifier(speed));

            MainCamera.ClampTransformToCameraLimits(cameraTransform);
        }

        protected void MoveCameraBackward(float speed)
        {
            if (!MainCamera.CameraConfig.EnableMovement || MainCamera.BlockCamera)
            {
                return;
            }

            var cameraTransform = MainCamera.transform;
            var cameraForward = cameraTransform.forward;
            
            MainCamera.transform.position -=  new Vector3(
                cameraForward.x * MainCamera.CameraConfig.DeltaTime * GetMovementSpeedWithZoomModifier(speed),
                0,
                cameraForward.z * MainCamera.CameraConfig.DeltaTime * GetMovementSpeedWithZoomModifier(speed));

            MainCamera.ClampTransformToCameraLimits(cameraTransform);
        }

        protected void MoveCameraLeft(float speed)
        {
            if (!MainCamera.CameraConfig.EnableMovement || MainCamera.BlockCamera)
            {
                return;
            }

            var cameraTransform = MainCamera.transform;
            var cameraRight = cameraTransform.right;
            
            MainCamera.transform.position -= new Vector3(
                cameraRight.x * MainCamera.CameraConfig.DeltaTime * GetMovementSpeedWithZoomModifier(speed),
                0,
                cameraRight.z * MainCamera.CameraConfig.DeltaTime * GetMovementSpeedWithZoomModifier(speed));

            MainCamera.ClampTransformToCameraLimits(cameraTransform);
        }

        protected void MoveCameraRight(float speed)
        {
            if (!MainCamera.CameraConfig.EnableMovement || MainCamera.BlockCamera)
            {
                return;
            }

            var cameraTransform = MainCamera.transform;
            var cameraRight = cameraTransform.right;
            
            MainCamera.transform.position += new Vector3(
                cameraRight.x * MainCamera.CameraConfig.DeltaTime * GetMovementSpeedWithZoomModifier(speed),
                0,
                cameraRight.z * MainCamera.CameraConfig.DeltaTime * GetMovementSpeedWithZoomModifier(speed));

            MainCamera.ClampTransformToCameraLimits(cameraTransform);
        }
    }
}
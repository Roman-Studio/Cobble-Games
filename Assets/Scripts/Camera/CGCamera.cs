using System.Collections.Generic;
using CobbleGames.Camera.Modules;
using CobbleGames.Core;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Camera
{
    public class CGCamera : CGSingletonMonoBehaviour<CGCamera>
    {
        [field: SerializeField]
        public UnityEngine.Camera UnityCamera { get; private set; }

        [SerializeField]
        private List<CGCameraModuleBase> _CameraModules;
        
        [field: SerializeField, Expandable]
        public CGCameraConfig CameraConfig { get; private set; }
        
        [field: SerializeField]
        public bool BlockCamera { get; set; }
        
        private void Start()
        {
            foreach (var module in _CameraModules)
            {
                module.Initialize();
            }

            RegisterInputDelegates();
        }

        private void Update()
        {
            foreach (var module in _CameraModules)
            {
                module.OnUpdate();
            }
        }

        private void LateUpdate()
        {
            CalculateCamera();
        }

        private void OnDestroy()
        {
            UnregisterInputDelegates();
        }

        private void RegisterInputDelegates()
        {
            foreach (var module in _CameraModules)
            {
                module.RegisterInputDelegates();
            }
        }

        private void UnregisterInputDelegates()
        {
            foreach (var module in _CameraModules)
            {
                module.UnregisterInputDelegates();
            }
        }

        private void CalculateCamera()
        {
            foreach (var module in _CameraModules)
            {
                module.OnCalculateCamera();
            }
        }
        
        public void ClampTransformToCameraLimits(Transform transformToClamp)
        {
            var transformPosition = transformToClamp.position;

            var horizontalLimit = CameraConfig.MovementBounds.GetDynamicHorizontalMovementLimit(UnityCamera.orthographicSize);
            var verticalLimit = CameraConfig.MovementBounds.GetDynamicVerticalMovementLimit(UnityCamera.orthographicSize);

            transformPosition = new Vector3(
                Mathf.Clamp(transformPosition.x, horizontalLimit.x, horizontalLimit.y),
                transformPosition.y,
                Mathf.Clamp(transformPosition.z, verticalLimit.x, verticalLimit.y)
            );

            transformToClamp.position = transformPosition;
        }
    }
}
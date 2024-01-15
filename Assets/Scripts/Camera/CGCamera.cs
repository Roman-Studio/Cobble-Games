using System.Collections.Generic;
using CobbleGames.Camera.Modules;
using CobbleGames.Core;
using CobbleGames.SaveSystem;
using CobbleGames.SaveSystem.Data;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Camera
{
    public class CGCamera : CGSingletonMonoBehaviour<CGCamera>, ICGGameSaveClient
    {
        [field: SerializeField]
        public UnityEngine.Camera UnityCamera { get; private set; }

        [SerializeField]
        private List<CGCameraModuleBase> _CameraModules;
        
        [field: SerializeField, Expandable]
        public CGCameraConfig CameraConfig { get; private set; }
        
        [field: SerializeField]
        public bool BlockCamera { get; set; }
        
        protected override void Start()
        {
            base.Start();
            
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

        protected override void OnDestroy()
        {
            UnregisterInputDelegates();
            base.OnDestroy();
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

        public string ClientID => nameof(CGCamera);
        public int LoadOrder => 0;
        public bool IsLoading { get; private set; }

        private const string CameraPositionSaveKey = "CameraPosition";
        private const string CameraZoomSaveKey = "CameraZoom";
        
        public CGSaveDataEntryDictionary GetSaveData()
        {
            var saveData = new CGSaveDataEntryDictionary();

            saveData.TryAddDataEntry(CameraPositionSaveKey, transform.position.GetSaveData());
            saveData.TryAddDataEntry(CameraZoomSaveKey, new CGSaveDataEntryFloat(UnityCamera.orthographicSize));

            return saveData;
        }

        public void LoadDataFromSave(CGSaveDataEntryDictionary saveData)
        {
            IsLoading = true;
            
            if (saveData.TryGetDataEntry(CameraPositionSaveKey, out CGSaveDataEntryList cameraPositionDataList))
            {
                if (cameraPositionDataList.TryLoadFromSaveData(out Vector3 cameraPosition))
                {
                    transform.position = cameraPosition;
                }
            }

            if (saveData.TryGetDataValue(CameraZoomSaveKey, out float cameraZoom))
            {
                UnityCamera.orthographicSize = cameraZoom;
            }

            IsLoading = false;
        }
    }
}
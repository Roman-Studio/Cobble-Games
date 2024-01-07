using CobbleGames.Core;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Camera
{
    [CreateAssetMenu(fileName = "CameraBounds", menuName = "Cobble Games/Camera/Bounds")]
    public class CGCameraBounds : ScriptableObject
    {
        [field: SerializeField, Foldout("Camera Height")]
        public float MinCameraHeight { get; private set; } = 0;

        [field: SerializeField, Foldout("Camera Height")]
        public float MaxCameraHeight { get; private set; } = 100;

        [field: SerializeField, Foldout("Movement Limit")]
        public Vector2 HorizontalMovementLimit { get; private set; } = new Vector3(-25, 25);

        [field: SerializeField, Foldout("Movement Limit")]
        public Vector2 VerticalMovementLimit { get; private set; } = new Vector3(-25, 25);

        [field: SerializeField, Foldout("Movement Limit")]
        public Vector2 MaxZoomHorizontalMovementLimit { get; private set; } = new Vector3(-50, 50);

        [field: SerializeField, Foldout("Movement Limit")]
        public Vector2 MaxZoomVerticalMovementLimit { get; private set; } = new Vector3(-50, 50);
        
        public Vector2 GetDynamicHorizontalMovementLimit(float cameraHeight)
        {
            var normalizedCameraHeightInLevel = cameraHeight.RemapValue(MinCameraHeight, MaxCameraHeight, 0f, 1f);
            return Vector2.Lerp(MaxZoomHorizontalMovementLimit, HorizontalMovementLimit, normalizedCameraHeightInLevel);
        }

        public Vector2 GetDynamicVerticalMovementLimit(float cameraHeight)
        {
            var normalizedCameraHeightInLevel = cameraHeight.RemapValue(MinCameraHeight, MaxCameraHeight, 0f, 1f);
            return Vector2.Lerp(MaxZoomVerticalMovementLimit, VerticalMovementLimit, normalizedCameraHeightInLevel);
        }
    }
}
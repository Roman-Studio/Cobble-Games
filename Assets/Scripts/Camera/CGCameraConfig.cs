using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Camera
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Cobble Games/Camera/Config")]
    public class CGCameraConfig : ScriptableObject
    {
        [SerializeField]
        private bool _UseUnscaledTime;
        
        [SerializeField, Foldout("Zoom")]
        private float _ZoomSpeed = 1f;
        
        [SerializeField, Foldout("Zoom")]
        private AnimationCurve _ZoomCameraMovementSpeedModifier;

        [SerializeField, Foldout("Movement")]
        private bool _EnableMovement = true;

        [SerializeField, Expandable, ShowIf(nameof(_EnableMovement)), Foldout("Movement")]
        private CGCameraBounds _MovementBounds;

        [SerializeField, ShowIf(nameof(_EnableMovement)), Foldout("Movement")]
        private float _MovementSpeed = 10f;

        [SerializeField, ShowIf(nameof(_EnableMovement)), Foldout("Movement")]
        private float _OnButtonSpeedModifier = 2f;

        [SerializeField, ShowIf(nameof(_EnableMovement)), Foldout("Movement")]
        private bool _EnableDragMovement = true;

        [SerializeField, ShowIf(nameof(EnableDragMovement)), Foldout("Movement")]
        private float _DragDeadzoneSize;

        [SerializeField, ShowIf(nameof(EnableDragMovement)), Foldout("Movement")]
        private float _DragPlaneHeight;

        public float DeltaTime => _UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        public float ZoomSpeed => _ZoomSpeed;
        public float GetZoomCameraMovementSpeedModifier(float currentZoom) => _ZoomCameraMovementSpeedModifier.Evaluate(currentZoom);
        
        public float MovementSpeed => _MovementSpeed;
        public float OnButtonSpeedModifier => _OnButtonSpeedModifier;
        public bool EnableMovement => _EnableMovement;
        public CGCameraBounds MovementBounds => _MovementBounds;
        public bool EnableDragMovement => EnableMovement && _EnableDragMovement;
        public float DragDeadzoneSize => _DragDeadzoneSize;
        public float DragPlaneHeight => _DragPlaneHeight;
    }
}
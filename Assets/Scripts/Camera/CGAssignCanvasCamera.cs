using UnityEngine;

namespace CobbleGames.Camera
{
    [RequireComponent(typeof(Canvas))]
    public class CGAssignCanvasCamera : MonoBehaviour
    {
        private Canvas _TargetCanvas;
        private Canvas TargetCanvas
        {
            get
            {
                if (_TargetCanvas == null)
                {
                    _TargetCanvas = GetComponent<Canvas>();
                }

                return _TargetCanvas;
            }
        }

        [SerializeField]
        private float _PlaneDistance = 1f;

        private void Start()
        {
            TargetCanvas.worldCamera = CGCamera.Instance.UnityCamera;
            TargetCanvas.planeDistance = _PlaneDistance;
        }
    }
}
using CobbleGames.Camera;
using CobbleGames.Characters;
using CobbleGames.PathFinding;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CobbleGames.Player
{
    public class CGPlayerController : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _GetGridPositionInputAction;

        [SerializeField]
        private InputActionReference _GetGridUnderMouseInputAction;

        [SerializeField]
        private LayerMask _GridDetectionLayerMask;

        private void Update()
        {
            TryMoveSelectedCharacter();
        }
        
        private void OnEnable()
        {
            _GetGridPositionInputAction.action.Enable();
            _GetGridUnderMouseInputAction.action.Enable();
        }

        private void OnDisable()
        {
            _GetGridPositionInputAction.action.Disable();
            _GetGridUnderMouseInputAction.action.Disable();
        }

        private void TryMoveSelectedCharacter()
        {
            if (CGCharactersManager.Instance.SelectedCharacter == null)
            {
                return;
            }
            
            if (!_GetGridUnderMouseInputAction.action.WasPressedThisFrame())
            {
                return;   
            }

            var mousePosition = _GetGridPositionInputAction.action.ReadValue<Vector2>();
            var ray = CGCamera.Instance.UnityCamera.ScreenPointToRay(mousePosition);

            if (!Physics.Raycast(ray, out var hit, CGCamera.Instance.UnityCamera.farClipPlane, _GridDetectionLayerMask))
            {
                return;
            }

            var pathFindingNode = hit.transform.GetComponent<ICGPathFindingNode>();

            if (pathFindingNode == null)
            {
                return;
            }
            
            CGCharactersManager.Instance.SelectedCharacter.MoveCharacter(pathFindingNode.NodePosition);

            foreach (var spawnedCharacter in CGCharactersManager.Instance.SpawnedCharacters)
            {
                if (spawnedCharacter == CGCharactersManager.Instance.SelectedCharacter)
                {
                    continue;
                }
                
                spawnedCharacter.FollowCharacter(CGCharactersManager.Instance.SelectedCharacter);
            }
        }
    }
}
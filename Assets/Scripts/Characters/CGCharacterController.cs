using System.Collections.Generic;
using UnityEngine;

namespace CobbleGames.Characters
{
    public class CGCharacterController : MonoBehaviour
    {
        [SerializeField]
        private float _MovementSpeed = 2f;

        [SerializeField]
        private float _RotationSpeed = 4f;

        [SerializeField]
        private float _TargetTolerance = 0.1f;
        
        private readonly List<Vector3> _PathVectors = new();
        
        public Vector3 CurrentMovementDirection { get; private set; }

        private void Update()
        {
            MoveCharacter();
            RotateCharacter();
        }

        private void MoveCharacter()
        {
            if (_PathVectors.Count <= 0)
            {
                CurrentMovementDirection = Vector3.zero;
                return;
            }

            var nextTargetPosition = _PathVectors[0];
            var currentPosition = transform.position;

            if (Vector3.Distance(currentPosition, nextTargetPosition) <= _TargetTolerance)
            {
                _PathVectors.RemoveAt(0);
                return;
            }
            
            CurrentMovementDirection = (nextTargetPosition - currentPosition).normalized;
            var newPosition = currentPosition + CurrentMovementDirection * (_MovementSpeed * Time.deltaTime);
            transform.position = newPosition;
        }

        private void RotateCharacter()
        {
            if (CurrentMovementDirection == Vector3.zero)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(CurrentMovementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _RotationSpeed * Time.deltaTime);
        }

        public void SetPathVectorsFromTransforms(IEnumerable<Transform> pathTransforms)
        {
            _PathVectors.Clear();
            
            foreach (var pathTransform in pathTransforms)
            {
                _PathVectors.Add(pathTransform.position);
            }
        }
    }
}
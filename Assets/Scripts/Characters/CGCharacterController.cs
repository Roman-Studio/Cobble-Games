﻿using System;
using System.Collections.Generic;
using CobbleGames.Core;
using CobbleGames.PathFinding;
using UnityEngine;

namespace CobbleGames.Characters
{
    public class CGCharacterController : MonoBehaviour
    {
        [SerializeField]
        private float _MovementSpeed = 2f;

        public float MovementSpeed
        {
            get => _MovementSpeed;
            set => _MovementSpeed = value;
        }

        [SerializeField]
        private float _RotationSpeed = 4f;
        
        public float RotationSpeed
        {
            get => _RotationSpeed;
            set => _RotationSpeed = value;
        }

        [SerializeField]
        private float _TargetTolerance = 0.1f;
        
        private readonly List<Vector3> _PathVectors = new();
        
        public Vector3 CurrentMovementDirection { get; private set; }

        public event Action EventNextMovementTargetPositionChanged;
        public event Action EventCurrentPathChanged;

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

            if (Vector3.Distance(currentPosition, nextTargetPosition).IsInRangeInclusive(-_TargetTolerance, _TargetTolerance))
            {
                _PathVectors.RemoveAt(0);
                EventNextMovementTargetPositionChanged?.Invoke();
                return;
            }
            
            CurrentMovementDirection = (nextTargetPosition - currentPosition).normalized;
            var newPosition = currentPosition + CurrentMovementDirection * (MovementSpeed * Time.deltaTime);
            transform.position = newPosition;
        }

        private void RotateCharacter()
        {
            if (CurrentMovementDirection == Vector3.zero)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(CurrentMovementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }

        public void SetPathVectors(IEnumerable<Transform> pathTransforms)
        {
            _PathVectors.Clear();
            
            foreach (var pathTransform in pathTransforms)
            {
                _PathVectors.Add(pathTransform.position);
            }
            
            EventCurrentPathChanged?.Invoke();
        }

        public void SetPathVectors(IEnumerable<Vector3> pathVectors)
        {
            _PathVectors.Clear();

            foreach (var pathTransform in pathVectors)
            {
                _PathVectors.Add(pathTransform);
            }
            
            EventCurrentPathChanged?.Invoke();
        }

        public void SetPathVectors(IEnumerable<ICGPathFindingNode> pathFindingNodes)
        {
            _PathVectors.Clear();

            foreach (var pathFindingNode in pathFindingNodes)
            {
                _PathVectors.Add(pathFindingNode.NodePosition);
            }
            
            EventCurrentPathChanged?.Invoke();
        }
    }
}
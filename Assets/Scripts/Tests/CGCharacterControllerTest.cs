using System.Collections.Generic;
using CobbleGames.Characters;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Tests
{
    public class CGCharacterControllerTest : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _PathTransforms;

        [SerializeField]
        private CGCharacterController _CharacterController;

        private bool IsPlaying => Application.isPlaying;

        [Button, EnableIf(nameof(IsPlaying))]
        private void TestPath()
        {
            _CharacterController.SetPathVectorsFromTransforms(_PathTransforms);
        }
    }
}
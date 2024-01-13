using UnityEngine;

namespace CobbleGames.Camera
{
    public class CGLookAtCamera : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = CGCamera.Instance.UnityCamera.transform.rotation;
        }
    }
}
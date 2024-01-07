using UnityEngine;

namespace CobbleGames.Camera.Modules
{
    public abstract class CGCameraModuleBase : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void RegisterInputDelegates();
        public abstract void UnregisterInputDelegates();
        public abstract void OnUpdate();
        public abstract void OnCalculateCamera();
    }
}
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Core
{
    public abstract class CGObserverMonoBehaviour<TObservedType> : CGObserverMonoBehaviour
    {
        [field: SerializeField, ReadOnly]
        public TObservedType ObservedObject { get; protected set; }

        public virtual void Set(TObservedType newObservedObject)
        {
            if (ReferenceEquals(ObservedObject, newObservedObject))
            {
                return;
            }
            
            if (ObservedObject != null)
            {
                UnregisterEvents();
            }

            ObservedObject = newObservedObject;
            OnReactToChanges();
            RegisterEvents();
        }

        protected virtual void OnDestroy()
        {
            if (ObservedObject != null)
            {
                UnregisterEvents();
            }
        }

        protected abstract void RegisterEvents();
        protected abstract void UnregisterEvents();
    }

    public abstract class CGObserverMonoBehaviour : MonoBehaviour
    {
        protected bool RequestedUpdateInThisFrame;
        
        protected void OnDisable()
        {
            RequestedUpdateInThisFrame = false;
        }
        
        protected void ReactToChanges()
        {
            if (RequestedUpdateInThisFrame)
            {
                return;
            }

            RequestedUpdateInThisFrame = true;

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ReactToChangesCoroutine());
            }
            else
            {
                OnReactToChanges();
                RequestedUpdateInThisFrame = false;
            }
        }

        private IEnumerator ReactToChangesCoroutine()
        {
            yield return null;
            OnReactToChanges();
            RequestedUpdateInThisFrame = false;
        }

        protected abstract void OnReactToChanges();
    }
}
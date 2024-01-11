using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CobbleGames.Core
{
    public abstract class CGMonoBehaviourPool<TPoolElement, TPoolObservedType> : CGObserverMonoBehaviour
        where TPoolElement : CGObserverMonoBehaviour<TPoolObservedType>
    {
        [SerializeField]
        protected TPoolElement _PoolElementPrefab;

        [SerializeField]
        protected Transform _TargetTransform;

        protected ObjectPool<TPoolElement> _ObjectPool;
        protected ObjectPool<TPoolElement> ObjectPool => _ObjectPool ??= new ObjectPool<TPoolElement>(CreatePoolElementInstance, OnGetPoolElement, OnReleasePoolElement, OnDestroyPoolElement);

        protected List<PooledObject<TPoolElement>> _CurrentlyActiveObjects = new();
        protected bool _ScenePrefabPresentInPool;

        protected virtual TPoolElement CreatePoolElementInstance()
        {
            if (string.IsNullOrEmpty(_PoolElementPrefab.gameObject.scene.name) || _ScenePrefabPresentInPool)
            {
                return Instantiate(_PoolElementPrefab, _TargetTransform);
            }
            
            _ScenePrefabPresentInPool = true;
            _PoolElementPrefab.gameObject.SetActive(false);
            return _PoolElementPrefab;
        }

        protected virtual void OnGetPoolElement(TPoolElement poolElement)
        {
            poolElement.gameObject.SetActive(true);
        }

        protected virtual void OnReleasePoolElement(TPoolElement poolElement)
        {
            poolElement.gameObject.SetActive(false);
        }

        protected virtual void OnDestroyPoolElement(TPoolElement poolElement)
        {
            Destroy(poolElement.gameObject);
        }

        public void BindCollection(IEnumerable<TPoolObservedType> collectionToBind)
        {
            ClearActive();

            foreach (var objectToBind in collectionToBind)
            {
                _CurrentlyActiveObjects.Add(ObjectPool.Get(out var pooledElement));
                pooledElement.Set(objectToBind);
            }
        }

        protected void ClearActive()
        {
            foreach (var currentlyActiveObject in _CurrentlyActiveObjects)
            {
                ((IDisposable)currentlyActiveObject).Dispose();
            }
            
            _CurrentlyActiveObjects.Clear();
        }
    }
}
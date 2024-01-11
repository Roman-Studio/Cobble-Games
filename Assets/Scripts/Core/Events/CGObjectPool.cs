using System;
using UnityEngine.Pool;

namespace CobbleGames.Core.Events
{
    public class CGObjectPool<TPoolElement> : ObjectPool<TPoolElement> where TPoolElement : class
    {
        public CGObjectPool(Func<TPoolElement> createFunc, Action<TPoolElement> actionOnGet = null, Action<TPoolElement> actionOnRelease = null, 
            Action<TPoolElement> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) 
            : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {
            
        }
    }
}
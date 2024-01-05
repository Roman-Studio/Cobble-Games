using UnityEngine;

namespace CobbleGames.Core
{
    public abstract class CGSingletonMonoBehaviour<TFinalType> : MonoBehaviour
        where TFinalType : Component
    {
        private static TFinalType instance;
        public static TFinalType Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                
                instance = FindObjectOfType<TFinalType>();

                if (instance != null)
                {
                    return instance;
                }
                
                var newObject = new GameObject
                {
                    name = typeof(TFinalType).Name
                };
                
                instance = newObject.AddComponent<TFinalType>();
                return instance;
            }
        }

        [SerializeField] 
        private bool _DontDestroyOnLoad = true;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as TFinalType;

                if (_DontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
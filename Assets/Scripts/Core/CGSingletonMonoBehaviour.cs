﻿using CobbleGames.SaveSystem;
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
            else if(instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        protected virtual void Start()
        {
            if (this is ICGGameSaveClient gameSaveClient && CGSaveManager.Instance != null)
            {
                CGSaveManager.Instance.AddGameSaveClient(gameSaveClient);
            }
        }

        protected virtual void OnDestroy()
        {
            if (this is ICGGameSaveClient gameSaveClient && CGSaveManager.Instance != null)
            {
                CGSaveManager.Instance.TryRemoveGameSaveClient(gameSaveClient);
            }
            
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
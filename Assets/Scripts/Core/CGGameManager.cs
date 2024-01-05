using System;
using CobbleGames.Map;

namespace CobbleGames.Core
{
    public class CGGameManager : CGSingletonMonoBehaviour<CGGameManager>
    {
        public bool GameInitialized { get; private set; }
        
        private event Action _EventGameInitialized;
        public event Action EventGameInitialized
        {
            add
            {
                if (GameInitialized)
                {
                    value?.Invoke();
                    return;
                }

                _EventGameInitialized += value;
            }
            remove
            {
                if (GameInitialized)
                {
                    return;
                }

                _EventGameInitialized -= value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            CGMapManager.Instance.Initialize();
            FinishInitialization();
        }

        private void FinishInitialization()
        {
            _EventGameInitialized?.Invoke();
            GameInitialized = true;
            _EventGameInitialized = null;
        }
    }
}
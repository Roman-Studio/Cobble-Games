using UnityEngine;
using UnityEngine.Events;

namespace CobbleGames.Core.Events
{
    public class CGOnStartEvent : MonoBehaviour
    {
        [field: SerializeField]
        public UnityEvent EventOnStart { get; private set; }

        private void Start()
        {
            EventOnStart?.Invoke();
        }
    }
}
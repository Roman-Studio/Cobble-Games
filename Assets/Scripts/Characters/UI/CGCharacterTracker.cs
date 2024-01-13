using CobbleGames.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CobbleGames.Characters.UI
{
    public class CGCharacterTracker : CGObserverMonoBehaviour<CGCharacter>
    {
        [SerializeField]
        private Slider _StaminaSlider;

        [SerializeField]
        private GameObject _RestingInfo;

        protected override void RegisterEvents()
        {
            ObservedObject.EventCharacterCurrentStaminaChanged += ReactToChanges;
            ObservedObject.EventIsRestingChanged += ReactToChanges;
        }

        protected override void UnregisterEvents()
        {
            ObservedObject.EventCharacterCurrentStaminaChanged -= ReactToChanges;
            ObservedObject.EventIsRestingChanged -= ReactToChanges;
        }
        
        protected override void OnReactToChanges()
        {
            _StaminaSlider.maxValue = ObservedObject.Stamina;
            _StaminaSlider.value = ObservedObject.CurrentStamina;
            _RestingInfo.SetActive(ObservedObject.IsResting);
        }
    }
}
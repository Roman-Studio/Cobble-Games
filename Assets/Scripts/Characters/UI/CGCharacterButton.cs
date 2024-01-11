using CobbleGames.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CobbleGames.Characters.UI
{
    public class CGCharacterButton : CGObserverMonoBehaviour<CGCharacter>
    {
        [SerializeField]
        private Image _ButtonFill;

        [SerializeField]
        private Image _ButtonBorder;

        [SerializeField]
        private Color _SelectedBorderColor;

        [SerializeField]
        private Color _DeselectedBorderColor;
        
        protected override void RegisterEvents()
        {
            ObservedObject.EventCharacterSelected.AddListener(ReactToChanges);
            ObservedObject.EventCharacterDeselected.AddListener(ReactToChanges);
        }

        protected override void UnregisterEvents()
        {
            ObservedObject.EventCharacterSelected.RemoveListener(ReactToChanges);
            ObservedObject.EventCharacterDeselected.RemoveListener(ReactToChanges);
        }

        protected override void OnReactToChanges()
        {
            _ButtonBorder.color = ObservedObject.IsSelected ? _SelectedBorderColor : _DeselectedBorderColor;
            _ButtonFill.color = ObservedObject.CharacterColor;
        }

        public void OnCharacterButtonClick()
        {
            if (ObservedObject.IsSelected)
            {
                ObservedObject.DeselectCharacter();
            }
            else
            {
                ObservedObject.SelectCharacter();
            }
        }
    }
}
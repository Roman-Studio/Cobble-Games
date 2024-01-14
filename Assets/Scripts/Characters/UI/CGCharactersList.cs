using CobbleGames.Core;

namespace CobbleGames.Characters.UI
{
    public class CGCharactersList : CGMonoBehaviourPool<CGCharacterButton, CGCharacter>
    {
        protected override void Start()
        {
            base.Start();
            
            if (CGCharactersManager.Instance == null)
            {
                return;
            }
            
            CGCharactersManager.Instance.EventSpawnedCharactersChanged += ReactToChanges;
            ReactToChanges();
        }

        private void OnDestroy()
        {
            if (CGCharactersManager.Instance == null)
            {
                return;
            }
            
            CGCharactersManager.Instance.EventSpawnedCharactersChanged -= ReactToChanges;
        }

        protected override void OnReactToChanges()
        {
            BindCollection(CGCharactersManager.Instance.SpawnedCharacters);
        }
    }
}
using System;

namespace CobbleGames.SaveSystem
{
    public interface ICGGameSaveClient : ICGGameSaveObject, IComparable<ICGGameSaveClient>
    {
        string ClientID { get; }
        int LoadOrder { get; }

        int IComparable<ICGGameSaveClient>.CompareTo(ICGGameSaveClient otherClient)
        {
            if (LoadOrder > otherClient.LoadOrder)
            {
                return 1;
            }
            
            return LoadOrder < otherClient.LoadOrder ? -1 : 0;
        }
    }
}
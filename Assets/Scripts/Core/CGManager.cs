using UnityEngine;

namespace CobbleGames.Core
{
    public abstract class CGManager<TFinalType> : CGSingletonMonoBehaviour<TFinalType> 
        where TFinalType : Component
    {
        
    }
}
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CobbleGames.Editor
{
    public static class CGScriptingDefines
    {
        [MenuItem("Cobble Games/Path Finding/Enable Debug Mode")]
        public static void EnablePathFindingDebug() => SetScriptingDefine("PATHFINDING_DEBUG", true);
    
        [MenuItem("Cobble Games/Path Finding/Disable Debug Mode")]
        public static void DisablePathFindingDebug() => SetScriptingDefine("PATHFINDING_DEBUG", false);
        
        private static void SetScriptingDefine(string scriptingDefine, bool enabled)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, out var currentScriptingDefines);
            var scriptingDefines = currentScriptingDefines.ToList();

            if (enabled)
            {
                if (!scriptingDefines.Contains(scriptingDefine))
                {
                    scriptingDefines.Add(scriptingDefine);
                }

                Debug.Log($"[{nameof(CGScriptingDefines)}.{nameof(SetScriptingDefine)}] Scripting defines changed! {scriptingDefine} is enabled.");
            }
            else
            {
                if (scriptingDefines.Contains(scriptingDefine))
                {
                    scriptingDefines.Remove(scriptingDefine);
                }
            
                Debug.Log($"[{nameof(CGScriptingDefines)}.{nameof(SetScriptingDefine)}] Scripting defines changed! {scriptingDefine} is disabled.");
            }
        
            var newScriptingDefines = string.Join(";", scriptingDefines);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newScriptingDefines);
        }
    }
}
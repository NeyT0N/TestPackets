using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MultiplatformAds.Editor
{
    [InitializeOnLoad]
    public class DefineManager
    {
        public const string BRIDGE_NAME = "Bridge";
        private const string BRIDGE_DEFINE = "PLAYGAMA_BRIDGE";
    
        static DefineManager()
        {
            if (IsScriptPresent(BRIDGE_NAME)) Add(BRIDGE_DEFINE);
            else Remove(BRIDGE_DEFINE);
        }

        static bool IsScriptPresent(string scriptName)
        {
            string[] scripts = AssetDatabase.FindAssets(scriptName);
            return scripts.Length > 0;
        }

        static void Add(string symbol)
        {
#if UNITY_STANDALONE
        NamedBuildTarget currentTarget = NamedBuildTarget.Standalone;
#elif UNITY_IPHONE
        NamedBuildTarget currentTarget = NamedBuildTarget.iOS;
#elif UNITY_ANDROID
        NamedBuildTarget currentTarget = NamedBuildTarget.Android;
#elif UNITY_WEBGL
            NamedBuildTarget currentTarget = NamedBuildTarget.WebGL;
#endif

            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(currentTarget);
            if (!currentDefines.Contains(symbol))
            {
                currentDefines += ";" + symbol;
                PlayerSettings.SetScriptingDefineSymbols(currentTarget, currentDefines);
                Debug.Log($"Module added: {symbol}");
            }
        }

        static void Remove(string symbol)
        {
#if UNITY_STANDALONE
        NamedBuildTarget currentTarget = NamedBuildTarget.Standalone;
#elif UNITY_IPHONE
        NamedBuildTarget currentTarget = NamedBuildTarget.iOS;
#elif UNITY_ANDROID
        NamedBuildTarget currentTarget = NamedBuildTarget.Android;
#elif UNITY_WEBGL
            NamedBuildTarget currentTarget = NamedBuildTarget.WebGL;
#endif
        
            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone);
            if (currentDefines.Contains(symbol))
            {
                currentDefines = currentDefines.Replace(";" + symbol, "").Replace(symbol + ";", "");
                PlayerSettings.SetScriptingDefineSymbols(currentTarget, currentDefines);
                Debug.Log($"Module deleted: {symbol}");
            }
        }
    }
}

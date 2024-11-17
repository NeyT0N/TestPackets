#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
#if !UNITY_EDITOR
using InstantGamesBridge.Common;
using System.Runtime.InteropServices;
#endif

namespace InstantGamesBridge.Modules.RemoteConfig
{
    public class RemoteConfigModule : MonoBehaviour
    {
        public bool isSupported
        {
            get
            {
#if !UNITY_EDITOR
                return InstantGamesBridgeIsRemoteConfigSupported() == "true";
#else
                return false;
#endif
            }
        }
        
#if !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string InstantGamesBridgeIsRemoteConfigSupported();

        [DllImport("__Internal")]
        private static extern void InstantGamesBridgeRemoteConfigGet(string options);
#endif
        private Action<bool, Dictionary<string, string>> _getCallback;

        
        public void Get(Dictionary<string, object> options, Action<bool, Dictionary<string, string>> onComplete)
        {
            _getCallback = onComplete;

#if !UNITY_EDITOR
            InstantGamesBridgeRemoteConfigGet(options.ToJson());
#else
            OnRemoteConfigGetFailed();
#endif
        }


        // Called from JS
        private void OnRemoteConfigGetSuccess(string result)
        {
            var values = new Dictionary<string, string>();
            
            try
            {
                values = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            
            _getCallback?.Invoke(true, values);
        }

        private void OnRemoteConfigGetFailed()
        {
            _getCallback?.Invoke(false, null);
        }
    }
}
#endif
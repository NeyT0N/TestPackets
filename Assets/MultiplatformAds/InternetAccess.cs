using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityWebRequest = UnityEngine.Networking.UnityWebRequest;

namespace MultiPlatformAds
{
    public static class InternetAccess
    {
        private static string[] _urls = new []{"google.com", "facebook.com", "wikipedia.org", "yahoo.com", "x.com"};

        private const string HTTPS = "https://";

        [DllImport("__Internal")]
        private static extern string Check();
        /// <summary>
        /// Checks if there is an internet connection
        /// </summary>
        /// <param name="callback">Returns a true callback if there is a network connection</param>
        public static IEnumerator CheckConnection(Action<bool> callback)
        {
#if UNITY_WEBGL
            var result = Check();
            
            bool isConnectionAccess = bool.Parse(result);

            if (isConnectionAccess)
            {
                callback?.Invoke(true);
                yield break; 
            }
#else
            foreach (var url in _urls)
            {
                
                UnityWebRequest request = new UnityWebRequest(HTTPS + url);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError) continue;
                
                callback?.Invoke(true);
                yield break;
            }
#endif
            
            callback?.Invoke(false);
        }
    }
}
using System;
using Debug = UnityEngine.Debug;

namespace MultiPlatformAds.State
{
    public class LoadingAdsState : IAdsState
    {
        public static event Action StateExit;

        public void Enter()
        {
            Debug.Log("Ads loading");
        }

        public void Exit()
        {
            StateExit?.Invoke();
            Debug.Log("Ads loaded");
        }
    }
}
using System;
using Debug = UnityEngine.Debug;

namespace MultiPlatformAds.State
{
    public class NoAdsState : IAdsState
    {
        public static event Action StateExit;

        public void Enter()
        {
            Debug.Log("Ads not working correctly");
        }

        public void Exit()
        {
            StateExit?.Invoke();
            Debug.Log("Ads loading");
        }
    }
}
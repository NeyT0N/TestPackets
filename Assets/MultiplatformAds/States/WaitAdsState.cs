using System;
using Debug = UnityEngine.Debug;

namespace MultiPlatformAds.State
{
    public class WaitAdsState : IAdsState
    {
        public static event Action StateExit;

        public void Enter()
        {
            Debug.Log("Waiting ads");
        }

        public void Exit()
        {
            StateExit?.Invoke();
            Debug.Log("Ads waited");
        }
    }
}
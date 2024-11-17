using System;
using Debug = UnityEngine.Debug;

namespace MultiPlatformAds.State
{
    public class OpeningAdsState : IAdsState
    {
        public static event Action StateExit;

        public void Enter()
        {
            Debug.Log("Ads opening");
        }

        public void Exit()
        {
            StateExit?.Invoke();
            Debug.Log("Ads opened");
        }
    }
}
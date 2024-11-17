using System;
using Debug = UnityEngine.Debug;

namespace MultiPlatformAds.State
{
    public class ClosingAdsState : IAdsState
    {
        public static event Action StateExit;

        public void Enter()
        {
            Debug.Log("Ads closing");
        }

        public void Exit()
        {
            StateExit?.Invoke();
            Debug.Log("Ads closed");
        }
    }
}
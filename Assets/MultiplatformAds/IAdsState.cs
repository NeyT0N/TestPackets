using Action = System.Action<MultiPlatformAds.State.AdsStateEnums>;

namespace MultiPlatformAds.State
{
    public interface IAdsState
    {
        public static event Action StateExit;
        
        public void Enter();
        public void Exit();
    }
}
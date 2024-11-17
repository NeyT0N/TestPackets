using System;
using System.Collections.Generic;
using MultiPlatformAds.State;

namespace MultiPlatformAds
{
    public class AdsState
    {
        public static event Action<AdsStateEnums> StateChanged;

        private Dictionary<Type, IAdsState> _states = new Dictionary<Type, IAdsState>();
        private IAdsState _currentState;

        public AdsState()
        {
            InitStates();
        }

        public void SetWaitState()
        {
            IAdsState state = GetState<WaitAdsState>();
            SetState(state);
            StateChanged?.Invoke(AdsStateEnums.Waiting);
        }

        public void SetLoadingState()
        {
            IAdsState state = GetState<LoadingAdsState>();
            SetState(state);
            StateChanged?.Invoke(AdsStateEnums.Loading);
        }

        public void SetClosingState()
        {
            IAdsState state = GetState<ClosingAdsState>();
            SetState(state);
            StateChanged?.Invoke(AdsStateEnums.Closed);
        }

        public void SetNoAdsState()
        {
            IAdsState state = GetState<NoAdsState>();
            SetState(state);
            StateChanged?.Invoke(AdsStateEnums.NoAds);
        }

        public void SetOpeningState()
        {
            IAdsState state = GetState<OpeningAdsState>();
            SetState(state);
            StateChanged?.Invoke(AdsStateEnums.Opening);
        }
        
        private void InitStates()
        {
            _states = new Dictionary<Type, IAdsState>
            {
                [typeof(NoAdsState)] = new NoAdsState(),
                [typeof(WaitAdsState)] = new WaitAdsState(),
                [typeof(LoadingAdsState)] = new LoadingAdsState(),
                [typeof(ClosingAdsState)] = new ClosingAdsState(),
                [typeof(OpeningAdsState)] = new OpeningAdsState()
            };
        }

        private void SetState(IAdsState state)
        {
            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        private IAdsState GetState<T>() where T : IAdsState
        {
            var type = typeof(T);
            return _states[type];
        }
    }
}
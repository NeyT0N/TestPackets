#region UsingRegion
using UnityEngine;
using Action = System.Action;
using AdblockAction = System.Action<bool>;
using ConnectAction = System.Action<bool>;
using IEnumerator = System.Collections.IEnumerator;

#if UNITY_WEBGL && PLAYGAMA_BRIDGE
using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
#endif
#endregion

namespace MultiPlatformAds
{
    public class AdsController : MonoBehaviour
    {
        #region Parameter

        public static AdsController Instance;

        public static event AdblockAction EnabledAdblock;
        public static event ConnectAction EnabledNetworkAccess;
        public static event Action InterShowingReady;
        
        [SerializeField] private int timerValue;
        [SerializeField] private int interInMinutes;
        [SerializeField] private bool isRepeatable;
        [SerializeField] private bool isAdWithTimer;
        [SerializeField] private bool isInterRepeatingAlternate;

        private const int TIME_VALUE = 60;

        private event Action InterFailAction;
        private event Action InterSuccessAction;
        private event Action RewardFailAction;
        private event Action RewardSuccessAction;

        private AdsState _adsState;
        private InfoPanel _infoPanel;
        private bool _isConnectionFailed;
        private bool _isRewarded;

        #endregion

        /// <summary>
        /// Displays interstitial ads, with or without delay
        /// </summary>
        /// <param name="isWithTimer">Show with timer</param>
        /// <param name="success">Action on success</param>
        /// <param name="fail">Action in case of failure</param>
        public void ShowInterstitial(bool isWithTimer = false, Action success = null, Action fail = null)
        {
            InterSuccessAction = success;
            InterFailAction = fail;

            if (_isConnectionFailed) FailedInternetCheck();
            
#if UNITY_WEBGL && PLAYGAMA_BRIDGE
            if (isWithTimer || isAdWithTimer) StartCoroutine(_infoPanel.Timer(timerValue, CallInterstitial));
            else CallInterstitial();
            
            
            if (isRepeatable) StartCoroutine(InterstitialRepeater());
#endif
        }

        /// <summary>
        /// Show banner ads if there is such an opportunity
        /// </summary>
        public void ShowBanner()
        {
#if UNITY_WEBGL && PLAYGAMA_BRIDGE
            if (Bridge.advertisement.isBannerSupported == false) return;

            Bridge.advertisement.ShowBanner();
#endif
        }

        /// <summary>
        /// Hide banner ads
        /// </summary>
        public void HideBanner()
        {
#if UNITY_WEBGL && PLAYGAMA_BRIDGE
            if (Bridge.advertisement.isBannerSupported == false) return;

            Bridge.advertisement.HideBanner();
#endif
        }

        /// <summary>
        /// Shows an advertisement followed by an award
        /// </summary>
        /// <param name="success">Action on success</param>
        /// <param name="fail">Action in case of failure</param>
        public void ShowReward(Action success = null, Action fail = null)
        {
            RewardSuccessAction = success;
            RewardFailAction = fail;

            if (_isConnectionFailed) FailedInternetCheck();
#if UNITY_WEBGL && PLAYGAMA_BRIDGE
            Bridge.advertisement.ShowRewarded();
#endif
        }
        
        public void FailedInternetCheck()
        {
            StartCoroutine(InternetAccess.CheckConnection(isConnected =>
            {
                EnabledNetworkAccess?.Invoke(isConnected);
                _isConnectionFailed = !isConnected;
            }));
        }

        private void Awake()
        {
            InstanceThis();
#if UNITY_WEBGL && PLAYGAMA_BRIDGE
            SetupWebActions();
            CheckAdblock();
#endif
            InitStates();
            SetupPanel();
            
            if (isRepeatable) StartCoroutine(InterstitialRepeater());
        }
        
        private void InstanceThis()
        {
            if (Instance) Destroy(this);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
#if UNITY_WEBGL && PLAYGAMA_BRIDGE
        private void SetupWebActions()
        {
            Bridge.advertisement.rewardedStateChanged += AdvertisementOnRewardedStateChanged;
            Bridge.advertisement.interstitialStateChanged += AdvertisementOnInterstitialStateChanged;
            Bridge.advertisement.bannerStateChanged += AdvertisementOnBannerStateChanged;
            Bridge.advertisement.SetMinimumDelayBetweenInterstitial(TIME_VALUE);
        }

        private void CheckAdblock() => Bridge.advertisement.CheckAdBlock(EnabledAdblock);

        private void CallInterstitial() => Bridge.advertisement.ShowInterstitial();
#endif

        private void InitStates() => _adsState = new AdsState();

        private void SetupPanel()
        {
            _infoPanel = gameObject.GetComponent<InfoPanel>();
            _infoPanel.Setup();
        }
        
        private void CheckReward()
        {
            if (_isRewarded) _isRewarded = false;
            else
            {
                RewardFailAction?.Invoke();
                _adsState.SetNoAdsState();
                FailedInternetCheck();
            }
        }

        private IEnumerator InterstitialRepeater()
        {
            yield return new WaitForSeconds(interInMinutes * TIME_VALUE);
            if (isInterRepeatingAlternate)
            {
                InterShowingReady?.Invoke();
            }
            else
            {
                if (_isConnectionFailed == false) ShowInterstitial(isAdWithTimer);
            }
        }

        #region WebAdsStates;

#if UNITY_WEBGL && PLAYGAMA_BRIDGE
        /// <summary>
        /// Interstitial state
        /// </summary>
        /// <param name="state">Current state</param>
        private void AdvertisementOnInterstitialStateChanged(InterstitialState state)
        {
            switch (state)
            {
                case InterstitialState.Loading:
                    _adsState.SetLoadingState();
                    break;
                case InterstitialState.Opened:
                    _adsState.SetOpeningState();
                    break;
                case InterstitialState.Closed:
                    InterSuccessAction?.Invoke();
                    goto default;
                case InterstitialState.Failed:
                    InterFailAction?.Invoke();
                    _adsState.SetNoAdsState();
                    FailedInternetCheck();
                    goto default;
                default:
                    _adsState.SetClosingState();
                    _adsState.SetWaitState();
                    break;
            }

            _adsState.SetWaitState();
        }

        /// <summary>
        /// Reward state
        /// </summary>
        /// <param name="state">Current state</param>
        private void AdvertisementOnRewardedStateChanged(RewardedState state)
        {
            switch (state)
            {
                case RewardedState.Loading:
                    _adsState.SetLoadingState();
                    break;
                case RewardedState.Opened:
                    _adsState.SetOpeningState();
                    break;
                case RewardedState.Rewarded:
                    RewardSuccessAction?.Invoke();
                    _isRewarded = true;
                    break;
                case RewardedState.Closed:
                    CheckReward();
                    _adsState.SetClosingState();
                    _adsState.SetWaitState();
                    break;
                case RewardedState.Failed:
                    RewardFailAction?.Invoke();
                    _adsState.SetNoAdsState();
                    FailedInternetCheck();
                    goto case RewardedState.Closed;
                default:
                    break;
            }
        }

        /// <summary>
        /// Banner state
        /// </summary>
        /// <param name="state">Current state</param>
        private void AdvertisementOnBannerStateChanged(BannerState state)
        {
            switch (state)
            {
                case BannerState.Loading:
                    Debug.Log("Banner loading");
                    break;
                case BannerState.Shown:
                    Debug.Log("Banner shown");
                    break;
                case BannerState.Hidden:
                    Debug.Log("Banner hidden");
                    break;
                case BannerState.Failed:
                    HideBanner();
                    break;
                default:
                    break;
            }
        }
#endif

        #endregion
    }
}
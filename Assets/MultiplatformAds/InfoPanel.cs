using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Xml;
using MultiPlatformAds.State;
using Text = TMPro.TMP_Text;
using Button = UnityEngine.UI.Button;
using CultureInfo = System.Globalization.CultureInfo;

namespace MultiPlatformAds
{
    public class InfoPanel : MonoBehaviour
    {
        public event Action TimerEnd;
        
        [Header("Components")]
        [SerializeField] private CanvasGroup timerBg;
        [SerializeField] private Text timerTMPro;
        [SerializeField] private Button button;
        
        [Header("Setups")] 
        [Tooltip("Notify that the Internet connection is lost")]
        [SerializeField] private bool isBlockingWithFailedConnection = false;
        [Tooltip("Notify that the ad blocker is enabled")]
        [SerializeField] private bool isBlockingWithAdBlock = false;
        [Tooltip("Is it possible to close the panel if the ad blocker is enabled or the Internet connection is lost")]
        [SerializeField] private bool isTheAbilityToCloseAds = false;
        [Tooltip("Expect to click on the panel after viewing the interstitial advertisement")]
        [SerializeField] private bool isWaitingAfterTheInterstitial = false;
        [Tooltip("Enabling internal localization")]
        [SerializeField] private bool isLocalizationEnabled = false;
        
        private const int SECOND = 1;
        private const string TRANSLATE_DOCUMENT_NAME = "Translations.xml";
        
        private XmlDocument xmlDoc;

        private string _tapToContinueText = "Tap to continue";
        private string _connectionText = "No connection";
        private string _adblockText = "Ad block enabled";
        private string _waitText = "Waiting ads";
        private string _failText = "Fail";
        private string _timerText = "Advertising in {0} seconds";

        public void Setup()
        {
            InitLocalization();

            if (isBlockingWithFailedConnection) AdsController.EnabledNetworkAccess += ShowNotConnection;
            if (isBlockingWithAdBlock) AdsController.EnabledAdblock += ShowAdBlock;

            AdsState.StateChanged += AdsStateOnStateChanged;

            button.onClick.AddListener(Hide);
        }

        public IEnumerator Timer(int timerValue, Action onEndTimerCallback)
        {
            var value = timerValue;
            Show();

            while (value > 0)
            {
                timerTMPro.text = string.Format(_timerText, value);
                value--;
                yield return new WaitForSecondsRealtime(SECOND);
            }
            
            TimerEnd?.Invoke();
            onEndTimerCallback?.Invoke();
        }
        
        private void ShowAdsBackground()
        {
            if (isWaitingAfterTheInterstitial == false) return;
         
            Show();
            
            button.interactable = true;
            timerTMPro.text = _tapToContinueText;
        }

        private void ShowNotConnection(bool isConnectAccessed)
        {
            if (isConnectAccessed) return;
            
            Show();
            
            if (isTheAbilityToCloseAds == false) button.interactable = true;
            
            timerTMPro.text = _connectionText;
        }

        private void ShowAdBlock(bool isAdBlockEnabled)
        {
            if (isAdBlockEnabled) return;
            if (isTheAbilityToCloseAds == false) button.interactable = true;
            
            Show();
            
            timerTMPro.text = _adblockText;
        }
        
        private void AdvertisementOpen()
        {
            Show();
            
            timerTMPro.text = _waitText;
        }

        private void ShowFail()
        {
            Show();
            if (IsInvoking(nameof(Hide))) 
                CancelInvoke(nameof(Hide));

            button.interactable = true;
            timerTMPro.text = _failText;
            
            Invoke(nameof(Hide), 2f);
        }

        private void InitLocalization()
        {
            if (isLocalizationEnabled == false) return;
            
            xmlDoc = new XmlDocument();
            xmlDoc.Load(TRANSLATE_DOCUMENT_NAME);
            XmlElement xmlRoot = xmlDoc.DocumentElement;

            var language =
#if UNITY_WEBGL
                InstantGamesBridge.Bridge.platform.language.ToLower();
#else
                CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
#endif

            foreach (XmlElement xmlNode in xmlRoot)
            {
                if (xmlNode.Name == language)
                {
                    var translate = xmlNode.SelectNodes("translate");
                        
                    _timerText = translate?[0].InnerText;
                    _tapToContinueText = translate?[1].InnerText;
                    _connectionText = translate?[2].InnerText;
                    _adblockText = translate?[3].InnerText;
                    _waitText = translate?[4].InnerText;
                    _failText = translate?[5].InnerText;
                    break;
                }
            }
        }
        
        private void AdsStateOnStateChanged(AdsStateEnums stateType)
        {
            switch (stateType)
            {
                case AdsStateEnums.Waiting:
                    if (isWaitingAfterTheInterstitial == false && IsInvoking(nameof(Hide)) == false) Hide();
                    break;
                case AdsStateEnums.Opening:
                    break;
                case AdsStateEnums.Closed:
                    ShowAdsBackground();
                    break;
                case AdsStateEnums.Loading:
                    AdvertisementOpen();
                    break;
                case AdsStateEnums.NoAds:
                    ShowFail();
                    break;
            }
        }

        private void Show()
        {
            timerBg.alpha = 1;
            timerBg.blocksRaycasts = true;
            button.interactable = false;
        }

        private void Hide()
        {
            timerBg.alpha = 0;
            timerBg.blocksRaycasts = false;
            button.interactable = false;
        }
    }
}
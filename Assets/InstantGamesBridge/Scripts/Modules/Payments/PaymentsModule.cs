#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
#if !UNITY_EDITOR
using InstantGamesBridge.Common;
using System.Runtime.InteropServices;
#endif

namespace InstantGamesBridge.Modules.Payments
{
    public class PaymentsModule : MonoBehaviour
    {
        public bool isSupported
        {
            get
            {
#if !UNITY_EDITOR
                return InstantGamesBridgeIsPaymentsSupported() == "true";
#else
                return false;
#endif
            }
        }
        
#if !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string InstantGamesBridgeIsPaymentsSupported();
        
        [DllImport("__Internal")]
        private static extern void InstantGamesBridgePaymentsPurchase(string options);

        [DllImport("__Internal")]
        private static extern void InstantGamesBridgePaymentsConsumePurchase(string options);
        
        [DllImport("__Internal")]
        private static extern void InstantGamesBridgePaymentsGetPurchases();
        
        [DllImport("__Internal")]
        private static extern void InstantGamesBridgePaymentsGetCatalog();
#endif
        
        private Action<bool> _purchaseCallback;
        private Action<bool> _consumePurchaseCallback;
        private Action<bool, List<Dictionary<string, string>>> _getPurchasesCallback;
        private Action<bool, List<Dictionary<string, string>>> _getCatalogCallback;


        public void Purchase(Dictionary<string, object> options, Action<bool> onComplete = null)
        {
            _purchaseCallback = onComplete;

#if !UNITY_EDITOR
            InstantGamesBridgePaymentsPurchase(options.ToJson());
#else
            OnPaymentsPurchaseCompleted("false");
#endif
        }
        
        public void ConsumePurchase(Dictionary<string, object> options, Action<bool> onComplete = null)
        {
            _consumePurchaseCallback = onComplete;

#if !UNITY_EDITOR
            InstantGamesBridgePaymentsConsumePurchase(options.ToJson());
#else
            OnPaymentsConsumePurchaseCompleted("false");
#endif
        }
        
        public void GetPurchases(Action<bool, List<Dictionary<string, string>>> onComplete = null)
        {
            _getPurchasesCallback = onComplete;

#if !UNITY_EDITOR
            InstantGamesBridgePaymentsGetPurchases();
#else
            OnPaymentsGetPurchasesCompletedFailed();
#endif
        }
        
        public void GetCatalog(Action<bool, List<Dictionary<string, string>>> onComplete = null)
        {
            _getCatalogCallback = onComplete;

#if !UNITY_EDITOR
            InstantGamesBridgePaymentsGetCatalog();
#else
            OnPaymentsGetCatalogCompletedFailed();
#endif
        }


        // Called from JS
        private void OnPaymentsPurchaseCompleted(string result)
        {
            var isSuccess = result == "true";
            _purchaseCallback?.Invoke(isSuccess);
            _purchaseCallback = null;
        }
        
        private void OnPaymentsConsumePurchaseCompleted(string result)
        {
            var isSuccess = result == "true";
            _consumePurchaseCallback?.Invoke(isSuccess);
            _consumePurchaseCallback = null;
        }
        
        private void OnPaymentsGetPurchasesCompletedSuccess(string result)
        {
            var purchases = new List<Dictionary<string, string>>();

            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    purchases = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            _getPurchasesCallback?.Invoke(true, purchases);
            _getPurchasesCallback = null;
        }

        private void OnPaymentsGetPurchasesCompletedFailed()
        {
            _getPurchasesCallback?.Invoke(false, null);
            _getPurchasesCallback = null;
        }
        
        private void OnPaymentsGetCatalogCompletedSuccess(string result)
        {
            var items = new List<Dictionary<string, string>>();

            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            _getCatalogCallback?.Invoke(true, items);
            _getCatalogCallback = null;
        }

        private void OnPaymentsGetCatalogCompletedFailed()
        {
            _getCatalogCallback?.Invoke(false, null);
            _getCatalogCallback = null;
        }
    }
}
#endif
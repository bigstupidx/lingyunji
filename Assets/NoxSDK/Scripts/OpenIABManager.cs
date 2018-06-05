using UnityEngine;
using OnePF;
using System.Collections.Generic;

/**
 * Example of OpenIAB usage
 */ 
public class OpenIABManager : MonoBehaviour
{
	public string googlePublicKey;
	public string onestoreKey;
    bool _isInitialized = false;
    Inventory _inventory = null;
	List<string> listSkus;
	public IapSample iapcore;
	public bool DEBUG;

    private void OnEnable()
    {
        // Listen to all events for illustration purposes
        OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
        OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
        OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
        OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
        OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
        OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
        OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
        OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
    }

    private void OnDisable()
    {
        // Remove all event handlers
        OpenIABEventManager.billingSupportedEvent -= billingSupportedEvent;
        OpenIABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
        OpenIABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
        OpenIABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
        OpenIABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
        OpenIABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
        OpenIABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
        OpenIABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
    }

    private void Start()
    {

	}

	public void Init(List<string> skus)
	{
		if (skus == null) {
			Debug.Log ("IAP Init Error skus = null");
			//if (TestCase.instance != null) {
			//	TestCase.instance._label = "IAP Init Error skus = null";
			//}
			return;
		}
		listSkus = skus;

		if (NOXSDK.Instance.StoreType == STORETYPE.GOOGLE) {
			// Map skus

			for (int i = 0; i < skus.Count; i++) {
				OpenIAB.mapSku (skus [i], OpenIAB_Android.STORE_GOOGLE, skus [i]);
			}
			
			// Application public 		
			var options = new Options ();
			options.checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS * 2;
			options.discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS * 2;
			options.checkInventory = false;
			options.verifyMode = OptionsVerifyMode.VERIFY_SKIP;
			options.prefferedStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE };
			options.availableStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE };
			options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, googlePublicKey} };
			options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;
			
			// Transmit options and start the service
			OpenIAB.init (options);
			//if (TestCase.instance != null) {
			//	TestCase.instance._label = "Finish public void Init(List<string> skus)";
			//}
		} else if (NOXSDK.Instance.StoreType == STORETYPE.ONESTORE) {
			
		}
	}

	public void checkLostPurchase()
	{
		OpenIAB.queryInventory ();
	}

	public void Purchase(string sku)
	{
		if (NOXSDK.Instance.StoreType == STORETYPE.GOOGLE) {
			if (_isInitialized == false) {
				Debug.Log("Error : Not Initialized!");				 
				return;
			}
			Debug.Log("Purchase : " + sku);		 
			OpenIAB.purchaseProduct (sku);
		} else if (NOXSDK.Instance.StoreType == STORETYPE.ONESTORE) {
			iapcore.RequestPaymenet(sku);
		}
	}

    private void billingSupportedEvent()
    {
        _isInitialized = true;
        Debug.Log("billingSupportedEvent");
    }

    private void billingNotSupportedEvent(string error)
    {
        Debug.Log("billingNotSupportedEvent: " + error);
    }

    private void queryInventorySucceededEvent(Inventory inventory)
    {
        Debug.Log("queryInventorySucceededEvent: " + inventory);
        if (inventory != null)
        {
            _inventory = inventory;
			for (int i = 0; i < listSkus.Count; i++) {
				Purchase pur = _inventory.GetPurchase(listSkus[i]);
				if(pur != null)
				{
					OpenIAB.consumeProduct(pur);
				}
			}
        }
    }

    private void queryInventoryFailedEvent(string error)
    {
        Debug.Log("queryInventoryFailedEvent: " + error);
    }

    private void purchaseSucceededEvent(Purchase purchase)
    {
		OpenIAB.consumeProduct (purchase);
        Debug.Log("purchaseSucceededEvent: " + purchase);
    }

    private void purchaseFailedEvent(int errorCode, string errorMessage)
    {
        Debug.Log("purchaseFailedEvent: " + errorMessage);
    }

    private void consumePurchaseSucceededEvent(Purchase purchase)
    {
		NOXSDK.Instance.PurchaseSucceede (purchase);
        Debug.Log("consumePurchaseSucceededEvent: " + purchase);
    }

    private void consumePurchaseFailedEvent(string error)
    {
        Debug.Log("consumePurchaseFailedEvent: " + error);
    }
}
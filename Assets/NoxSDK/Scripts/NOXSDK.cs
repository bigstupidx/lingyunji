using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OnePF;
using IgaworksUnityAOS;
using Toast.Analytics;
using IapResponse;
using MoTianJi.Json;

public enum STORETYPE
{
	GOOGLE = 0,
	ONESTORE = 1,
}

public class NOXSDK : MonoBehaviour {
	private static NOXSDK _instance = null;
	// Use this for initialization
	public STORETYPE StoreType;
	public PlayService PlayServiceObject;
	public FaceBookMgr FaceBookObject;
	public Adbrix ADBrixObject;
	public ToastMgr ToastObject;
	public GoogleAnalyticsV4 GoogleAnalyticsObject;
	public Cafe NaverCafeObject;
	public OpenIABManager OpenIABObject;

	public bool OnPlayService;
	public bool OnFaceBook;
	public bool OnAdbrix;
	public bool OnToast;
	public bool OnGoogleAnalytics;
	public bool OnNaverCafe;
	public bool OnIAP;

	public static NOXSDK Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType(typeof(NOXSDK)) as NOXSDK;
				if(_instance == null)
				{
					Debug.LogError("There's no active NOXSDK object");
				}
			}
			return _instance;
		}
	}

	private NOXSDK()
	{

	}

	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		DontDestroyOnLoad (this);

	}

	public void Init()
	{
		if (StoreType == STORETYPE.GOOGLE) {
			GoogleAnalyticsV4.TrackingCode = "UA-76844519-9";
			GoogleAnalyticsV4.PackageName = "g.com.noxgames.ma";
		} else if (StoreType == STORETYPE.ONESTORE) {
			GoogleAnalyticsV4.TrackingCode = "UA-76844519-10";
			GoogleAnalyticsV4.PackageName = "o.com.noxgames.ma";
		}

		ADBrixObject.gameObject.SetActive(OnAdbrix);
		ToastObject.gameObject.SetActive(OnToast);
		GoogleAnalyticsObject.gameObject.SetActive(OnGoogleAnalytics);
		NaverCafeObject.gameObject.SetActive(OnNaverCafe);
		OpenIABObject.gameObject.SetActive(OnIAP);

		//Commented out because of two choices:
		//PlayServiceObject.gameObject.SetActive(OnPlayService);
		//FaceBookObject.gameObject.SetActive(OnFaceBook);
	}

	public void OnGoogleLogin()
	{
		OnPlayService = true;
		PlayServiceObject.gameObject.SetActive(OnPlayService);
	}

	public void OnFacebookLogin()
	{
		OnFaceBook = true;
		FaceBookObject.gameObject.SetActive(OnFaceBook);
	}

	public string GetGoogleID()
	{
		return PlayServiceObject.GetIDToken ();
	}

	public string GetFaceBookID()
	{
		return FaceBookObject.GetIDToken ();
	}

	void OnDestroy()
	{
		if (OnGoogleAnalytics == true) {
			if (GoogleAnalyticsV4.instance != null)
				GoogleAnalyticsV4.instance.StopSession();
		}
		if (OnAdbrix == true) {
			IgaworksUnityPluginAOS.Common.endSession();
			IgaworksUnityPluginAOS.LiveOps.pause();
		}
		if (OnToast == true) {
			GameAnalytics.traceDeactivation();
		}
	}

	public void InitIAP(List<string> skus)
	{
		OpenIABObject.Init (skus);
	}

	public void Purchase(string sku)
	{
		if (OnIAP == true && OpenIABObject != null) {
			OpenIABObject.Purchase (sku);
		} else {
			Debug.Log("Error : Check IAP Object!");
		}
	}

	public void PurchaseSucceede(Purchase purchase)
	{
        Debug.Log("充值成功");
		// Succeede Purchase!
		string orderID = purchase.OrderId;
		string token = purchase.Token;
        SDKHelper.PayCallBackParameter parameter = new SDKHelper.PayCallBackParameter();
        parameter.token = token;
        parameter.orderId = orderID;
        parameter.rid = SDKHelper.instance.ChangeStoreIdToGameId(purchase.Sku);
        SDKHelper.instance.PaySuccess(JsonMapper.ToJson(parameter));
		// add your code!
		//if (TestCase.instance != null) {
		//	TestCase.instance._label = "OrderID : " + orderID;
		//	TestCase.instance._label += "\nToken : " + token;
		//}
	}

	public void PurchaseFailed()
	{
        SDKHelper.instance.PayFaild("Purchase Failed");

        // add your code!
        //if (TestCase.instance != null) {
        //	TestCase.instance._label = "Purchase Failed";
        //}
    }

    public void CheckLostPurchase()
	{
		if (OnIAP == true && OpenIABObject != null) {
			OpenIABObject.checkLostPurchase ();
		} else {
			Debug.Log("Error : Check IAP Object!");
		}
	}
	
	public void LogTransaction(string product_id, string item_name, string purchase_id, double price, int level)
	{		
		if (OnGoogleAnalytics == true && GoogleAnalyticsV4.instance != null)
		{
			GoogleAnalyticsV4.instance.LogTransaction(product_id, "NOX Games INC.", price, 0.0f, 0.0f, "KRW");
			GoogleAnalyticsV4.instance.LogItem(product_id, item_name, purchase_id, "InappItem", price, 1, "KRW");
		}

		if (OnAdbrix == true) {
			IgaworksUnityPluginAOS.Adbrix.purchase(purchase_id, product_id, item_name, price, 1, IgaworksUnityPluginAOS.Adbrix.Currency.KR_KRW, item_name);
			IgaworksUnityPluginAOS.Adbrix.setCustomCohort(IgaworksUnityPluginAOS.CohortVariable.COHORT_2, product_id);
			IgaworksUnityPluginAOS.LiveOps.setTargetingData("Purchase", product_id);
		}

		if (OnToast == true) {
			GameAnalytics.tracePurchase(product_id, (float)price, (float)price, "KRW", level);
		}
	}

	public void LogFriendsNumber(int number)
	{
		if (OnToast == true) {
			GameAnalytics.traceFriendCount (number);
		}
	}

	public void LogScreen(string screen_name)
	{
		if (OnGoogleAnalytics == true && GoogleAnalyticsV4.instance != null)
			GoogleAnalyticsV4.instance.LogScreen(screen_name);
	}

	public void LogPopup(string screen_name, string Popup_name)
	{
		if (OnGoogleAnalytics == true && GoogleAnalyticsV4.instance != null)
			GoogleAnalyticsV4.instance.LogScreen(screen_name + " " + Popup_name);
	}

	public void LogLevel(string job, int level)
	{
		if (Application.isEditor == true)
		{
			return;
		}

		if (OnGoogleAnalytics == true && GoogleAnalyticsV4.instance != null)
		{
			GoogleAnalyticsV4.instance.LogEvent("Level", job, level.ToString("000"), 1);
			GoogleAnalyticsV4.instance.LogEvent("Level", "Total", level.ToString("000"), 1);
		}

		GameAnalytics.traceLevelUp(level);
		IgaworksUnityPluginAOS.Adbrix.retention("Level_" + level.ToString("00000"));
		IgaworksUnityPluginAOS.Adbrix.setCustomCohort(IgaworksUnityPluginAOS.CohortVariable.COHORT_1, "Level_" + level);
		IgaworksUnityPluginAOS.LiveOps.setTargetingData("Level", level);
		IgaworksUnityPluginAOS.Common.endSession();
		IgaworksUnityPluginAOS.Common.startSession();
	}

	public void LogNewUserActivity(string code)
	{
		IgaworksUnityPluginAOS.Adbrix.firstTimeExperience(code);
	}

	private void InitExtraService()
	{
		TnkAd.Plugin.Instance.applicationStarted();
		CashslideWrapper.AppFirstLaunched("g35d5317");
		Nas.NASRunPlugin.Instance.run("5412312beafcd4bc8a346512fe0a5d6e");
	}

	private IEnumerator InitFB()
	{
		while (true)
		{
			if (FB.IsInitialized == true)
			{
				FB.GetDeepLink(DeepLinkCallback);
				break;
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void DeepLinkCallback(FBResult result)
	{
		if (!string.IsNullOrEmpty(result.Text))
		{
			GameAnalytics.traceFacebookInstall(result.Text);
		}
		if (result != null && !string.IsNullOrEmpty(result.Text))
		{
			try
			{
				Dictionary<string, object> jsonObjects = MiniJson.Deserialize(result.Text) as Dictionary<string, object>;
				string extras = (string)jsonObjects["extras"];
				Dictionary<string, object> extrasJSON = MiniJson.Deserialize(extras) as Dictionary<string, object>;
				string nativeURL = (string)extrasJSON["com.facebook.platform.APPLINK_NATIVE_URL"];
				IgaworksUnityPluginAOS.Common.setReferralUrlForFacebook(nativeURL);
			}
			catch (UnityException e)
			{

			}			
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause == true)
		{
			if (GoogleAnalyticsV4.instance != null)
			{
				GoogleAnalyticsV4.instance.StopSession();
			}
		}
		else
		{
			if (GoogleAnalyticsV4.instance != null)
			{
				GoogleAnalyticsV4.instance.StartSession();
			}
		}
	}
}

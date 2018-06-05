using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace Toast.Analytics {
		
	public class GameAnalyticsUnityPluginController : MonoBehaviour 
	{
		
		const string DELIMITER = "##";
		
		public static CampaignListener listener;
		
#if !UNITY_EDITOR && UNITY_ANDROID
		private static AndroidJavaClass _activityClass;
#endif
		
		public void Awake() 
		{
			Debug.Log ("GameAnalyticsUnityPluginController Awake");
			DontDestroyOnLoad(gameObject);
			GameAnalyticsUnityPlugin.instance.controller = this;
#if !UNITY_EDITOR && UNITY_ANDROID
			_activityClass = new AndroidJavaClass("com.toast.android.analytics.unity.UnityActivity");
#endif
		}
		
		void OnDestroy()
		{
			if (GameAnalyticsUnityPlugin.instance.controller == this)
				GameAnalyticsUnityPlugin.instance.controller = null;
		}
		
		
#if !UNITY_EDITOR && UNITY_ANDROID
		
		// Configuration API
		public static void setDebugMode(bool enable)
		{
			_activityClass.CallStatic("setDebugMode", enable);
		}

		public static int setUserId(string userId, bool useCampaignOrPromotion)
		{
			return _activityClass.CallStatic<int>("setUserId", userId, useCampaignOrPromotion);
		}
		
		public static string getDeviceInfo(string key)
		{
			string deviceInfo = _activityClass.CallStatic<string>("getDeviceInfo", key);
			return deviceInfo;
		}
		
		public static void setCampaignListener(CampaignListener campaignListener)
		{
			listener = campaignListener;
			_activityClass.CallStatic("setCampaignListener", GameAnalyticsUnityPlugin.instance.controller.name);
		}
		
		// Trace API
		public static int initializeSdk(string appId, string companyId, string appVersion, bool useLoggingUserId)
		{
			return _activityClass.CallStatic<int>("initializeSdk", appId, companyId, appVersion, useLoggingUserId);
		}
		
		public static int traceActivation()
		{
			return _activityClass.CallStatic<int>("traceActivation");
		}
		
		public static int traceDeactivation()
		{
			return _activityClass.CallStatic<int>("traceDeactivation");
		}
		
		public static int traceFriendCount(int friendCount)
		{
			return _activityClass.CallStatic<int>("traceFriendCount", friendCount);
		}
		
		public static int tracePurchase(string itemCode, float payment, float unitCost, string currency, int level)
		{
			return _activityClass.CallStatic<int>("tracePurchase", itemCode, payment, unitCost, currency, level);
		}
		
		public static int traceMoneyAcquisition(string usageCode, string type, double acquistionAmount, int level)
		{
			return _activityClass.CallStatic<int>("traceMoneyAcquisition", usageCode, type, acquistionAmount, level);
		}
		
		public static int traceMoneyConsumption(string usageCode, string type, double consumptionAmount, int level)
		{
			return _activityClass.CallStatic<int>("traceMoneyConsumption", usageCode, type, consumptionAmount, level);
		}
		
		public static int traceLevelUp(int level)
		{
			return _activityClass.CallStatic<int>("traceLevelUp", level);
		}
		
		public static int traceEvent(string eventType, string eventCode, string param1, string param2, double value, int level)
		{
			return _activityClass.CallStatic<int>("traceEvent", eventType, eventCode, param1, param2, value, level);
		}
		
		public static int traceStartSpeed(string intervalName)
		{
			return _activityClass.CallStatic<int>("traceStartSpeed", intervalName);
		}
		
		public static int traceEndSpeed(string intervalName)
		{
			return _activityClass.CallStatic<int>("traceEndSpeed", intervalName);
		}

		public static int traceFacebookInstall(string deeplinkUri)
		{
			return _activityClass.CallStatic<int>("traceFacebookInstall", deeplinkUri);
		}
		
		// Campaign API
		public static int showCampaign(string adspaceName)
		{
			return _activityClass.CallStatic<int>("showCampaign", adspaceName);
		}
		
		public static int showCampaign(string adspaceName, int animation, int lifeTime)
		{
			return _activityClass.CallStatic<int>("showCampaign", adspaceName, animation, lifeTime);
		}
		
		public static int hideCampaign(string adspaceName)
		{
			return _activityClass.CallStatic<int>("hideCampaign", adspaceName);
		}
		
		public static int hideCampaign(string adspaceName, int animation)
		{
			return _activityClass.CallStatic<int>("hideCampaign", adspaceName, animation);
		}

		public static Dictionary<string, CampaignInfo> getCampaignInfos()
		{
			string rtnStr = _activityClass.CallStatic<string>("getCampaignInfos");
			return makeCampaignInfosUsingJson(rtnStr);
		}

		// Toast Promotion API
		public static bool isPromotionAvailable() 
		{
			return _activityClass.CallStatic<bool>("isPromotionAvailable");
		}

		public static string getPromotionButtonImagePath()
		{
			string uri = _activityClass.CallStatic<string>("getPromotionButtonImagePath");
			return uri;
		}

		public static int launchPromotionPage()
		{
			return _activityClass.CallStatic<int>("launchPromotionPage");
		}

#elif !UNITY_EDITOR && UNITY_IPHONE

		// Common
		[DllImport("__Internal")] public static extern string _getDeviceInfo(string key);
		[DllImport("__Internal")] public static extern void _setDebugMode(bool enable);
		[DllImport("__Internal")] public static extern int _setUserId(string userId, bool useCampaignOrPromotion);

		public static string getDeviceInfo(string key) 
		{ 
			return _getDeviceInfo(key);
		}
		
		public static void setDebugMode(bool enable) 
		{ 
			_setDebugMode(enable);
		}

		public static int setUserId(string userId, bool useCampaignOrPromotion)
		{
			return _setUserId(userId, useCampaignOrPromotion);
		}
		
		// Analytics
		[DllImport("__Internal")] public static extern int _initializeSdk(string appId, string companyId, string appVersion, bool useLoggingUserId);
		[DllImport("__Internal")] public static extern int _traceActivation();
		[DllImport("__Internal")] public static extern int _traceDeactivation();
		[DllImport("__Internal")] public static extern int _traceFriendCount(int friendCount);
		[DllImport("__Internal")] public static extern int _tracePurchase(string itemCode, float payment, float unitCost, string currency, int level);
		[DllImport("__Internal")] public static extern int _traceMoneyAcquisition(string usageCode, string type, double amount, int level);
		[DllImport("__Internal")] public static extern int _traceMoneyConsumption(string usageCode, string type, double amount, int level);
		[DllImport("__Internal")] public static extern int _traceLevelUp(int level);
		[DllImport("__Internal")] public static extern int _traceEvent(string eventType, string eventCode, string param1, string param2, double value, int level);
		[DllImport("__Internal")] public static extern int _traceStartSpeed(string intervalName);
		[DllImport("__Internal")] public static extern int _traceEndSpeed(string intervalName);
		[DllImport("__Internal")] public static extern int _traceFacebookInstall(string deeplinkUri);

		public static int initializeSdk(string appId, string companyId, string appVersion, bool useLoggingUserId) 
		{ 
			return _initializeSdk(appId, companyId, appVersion, useLoggingUserId);
		}

		public static int traceActivation() 
		{
			return _traceActivation();
		}

		public static int traceDeactivation() 
		{ 
			return _traceDeactivation();
		}
		
		public static int traceFriendCount(int friendCount) 
		{ 
			return _traceFriendCount(friendCount);
		}
		
		public static int tracePurchase(string itemCode, float payment, float unitCost, string currency, int level) 
		{ 
			return _tracePurchase(itemCode, payment, unitCost, currency, level);
		}
		
		public static int traceMoneyAcquisition(string usageCode, string type, double acquistionAmount, int level) 
		{ 
			return _traceMoneyAcquisition(usageCode, type, acquistionAmount, level);
		}
		
		public static int traceMoneyConsumption(string usageCode, string type, double consumptionAmount, int level) 
		{ 
			return _traceMoneyConsumption(usageCode, type, consumptionAmount, level);
		}
		
		public static int traceLevelUp(int level) 
		{ 
			return _traceLevelUp(level);
		}
		
		public static int traceEvent(string eventType, string eventCode, string param1, string param2, double value, int level) 
		{ 
			return _traceEvent(eventType, eventCode, param1, param2, value, level);
		}
		
		public static int traceStartSpeed(string intervalName) 
		{ 
			return _traceStartSpeed(intervalName);
		}
		
		public static int traceEndSpeed(string intervalName) 
		{
			return _traceEndSpeed(intervalName);;
		}

		public static int traceFacebookInstall(string deeplinkUri)
		{
			return _traceFacebookInstall(deeplinkUri);
		}
		
		// Campaign
		[DllImport("__Internal")] public static extern int _showCampaign(string adspaceName);
		[DllImport("__Internal")] public static extern int _showCampaignAnimation(string adspaceName, int animation, int lifeTime);
		[DllImport("__Internal")] public static extern int _hideCampaign(string adspaceName);
		[DllImport("__Internal")] public static extern int _hideCampaignAnimation(string adspaceName, int animation);
		[DllImport("__Internal")] public static extern int _setOnCampaignListener(string objectName);
		[DllImport("__Internal")] public static extern string _getCampaignInfos();

		public static int showCampaign(string adspaceName) 
		{ 
			return _showCampaign(adspaceName);
		}
		
		public static int showCampaign(string adspaceName, int animation, int lifeTime) 
		{ 
			return _showCampaignAnimation(adspaceName, animation, lifeTime);
		}
		
		public static int hideCampaign(string adspaceName) 
		{ 
			return _hideCampaign(adspaceName);
		}
		
		public static int hideCampaign(string adspaceName, int animation) 
		{ 
			return _hideCampaignAnimation(adspaceName, animation);
		}

		public static Dictionary<string, CampaignInfo> getCampaignInfos()
		{
			string rtnStr = _getCampaignInfos();
			return makeCampaignInfosUsingJson(rtnStr);
		}

		public static int setCampaignListener(CampaignListener campaignListener) 
		{ 
			listener = campaignListener;
			return _setOnCampaignListener(GameAnalyticsUnityPlugin.instance.controller.name);
		}

		// Toast Promotion API - Android Only
		public static bool isPromotionAvailable() 
		{
			return false;
		}

		public static string getPromotionButtonImagePath()
		{
			return "";
		}
		
		public static int launchPromotionPage()
		{
			return 0;
		}
#else
		
		// Configuration API
		public static void setDebugMode(bool enable) { }
		public static int setUserId(string userId, bool useCampaignOrPromotion) { return 0; }
		public static string getDeviceInfo(string key) { return ""; }
		public static void setCampaignListener(CampaignListener campaignListener) { }

		// Trace API
		public static int initializeSdk(string appId, string companyId, string appVersion, bool useLoggingUserId) { return 0; }
		public static int traceActivation() { return 0; }
		public static int traceDeactivation() { return 0; }
		public static int traceFriendCount(int friendCount) { return 0; }
		public static int tracePurchase(string itemCode, float payment, float unitCost, string currency,  int level) { return 0; }
		public static int traceMoneyAcquisition(string usageCode, string type, double acquistionAmount, int level) { return 0; }
		public static int traceMoneyConsumption(string usageCode, string type, double consumptionAmount, int level) { return 0; }
		public static int traceLevelUp(int level) { return 0; }
		public static int traceEvent(string eventType, string eventCode, string param1, string param2, double value, int level) { return 0; }
		public static int traceStartSpeed(string intervalName) { return 0; }
		public static int traceEndSpeed(string intervalName) { return 0; }
		public static int traceFacebookInstall(string deeplinkUri) { return 0; }
		
		// Campaign API
		public static int showCampaign(string adspaceName) { return 0; }
		public static int showCampaign(string adspaceName, int animation, int lifeTime) { return 0; }
		public static int hideCampaign(string adspaceName) { return 0; }
		public static int hideCampaign(string adspaceName, int animation) { return 0; }
		public static Dictionary<string, CampaignInfo> getCampaignInfos() { return makeCampaignInfosUsingJson(""); }

		// Toast Promotion API - Android Only
		public static bool isPromotionAvailable() { return false; }
		public static string getPromotionButtonImagePath() { return ""; }
		public static int launchPromotionPage() { return 0; }
		
#endif

		private static Dictionary<string, CampaignInfo> makeCampaignInfosUsingJson(string json)
		{
			Dictionary<string, CampaignInfo> rtnData = new Dictionary<string, CampaignInfo> ();
			var dict = Json.Deserialize(json) as Dictionary<string,object>;

			if (dict == null) {
				return rtnData;
			}

			foreach (string key in dict.Keys)
			{
				Dictionary<string, object> info = (Dictionary<string, object>)dict[key];
				CampaignInfo campaignInfo = new CampaignInfo(info);
				rtnData.Add(key, campaignInfo);
			}

			return rtnData;
		}

		public void OnCampaignListener_OnCampaignVisibilityChanged(string param)
		{
			string[] callbackData = param.Split(new string[] {DELIMITER}, StringSplitOptions.RemoveEmptyEntries);

			if (callbackData.Length == 2) 
			{
				string adspaceName = callbackData[0];
				bool show = callbackData[1] == "true"?true:false;

				if (listener != null) 
				{
					listener.OnCampaignVisibilityChanged(adspaceName, show);
				}
			}
		}
		
		public void OnCampaignListener_OnCampaignLoadSuccess(string param)
		{
			if (listener != null) 
			{
				listener.OnCampaignLoadSuccess(param);
			}
		}
		
		public void OnCampaignListener_OnCampaignLoadFail(string param)
		{
			string[] callbackData = param.Split(new string[] {DELIMITER}, StringSplitOptions.RemoveEmptyEntries);

			if (callbackData.Length == 3) 
			{
				string adspaceName = callbackData[0];
				int errorCode = int.Parse(callbackData[1]);
				string errorMessage = callbackData[2];

				if (listener != null) 
				{
					listener.OnCampaignLoadFail(adspaceName, errorCode, errorMessage);
				}
			}
		}
		
		public void OnCampaignListener_OnMissionCompleted(string param)
		{
			string[] callbackData = param.Split(new string[] {DELIMITER}, StringSplitOptions.RemoveEmptyEntries);

			if (callbackData.Length > 0) 
			{
				List<string> rewardIds = new List<string>(callbackData);

				if (listener != null) 
				{
					listener.OnMissionComplete(rewardIds);
				}
			}
		}

		public void OnCampaignListener_OnPromotionVisibilityChanged(string param)
		{
			if (param == "true")
			{
				listener.OnPromotionVisibilityChanged(true);
			}
			else if (param == "false")
			{
				listener.OnPromotionVisibilityChanged(false);
			}
		}

		public void OnCampaignListener_OnCampaignClick(string param)
		{
			if (listener != null) 
			{
				listener.OnCampaignClick(param);
			}
		}
	}
}
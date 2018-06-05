using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toast.Analytics {
		
	public class GameAnalytics {
		
		public enum AnimationType {
			NONE = 0, 
			SILDE = 1, 
			FADE = 2
		};

		public const string DEVICE_INFO_DEVICEID 			= "deviceId";
		public const string DEVICE_INFO_PUSH_USERID 		= "pushUserId";         
		public const string DEVICE_INFO_CAMPAIGN_USERID 	= "campaignUserId";
		
		
		// Configuration APIS
		public static void setDebugMode(bool enable) 
		{ 
			GameAnalyticsUnityPluginController.setDebugMode (enable);
		}

		public static int setUserId(string userId, bool useCampaignOrPromotion)
		{
			return GameAnalyticsUnityPluginController.setUserId (userId, useCampaignOrPromotion);
		}

		
		public static string getDeviceInfo(string key)
		{
			return GameAnalyticsUnityPluginController.getDeviceInfo (key);
		}
		
		public static void setCampaignListener(CampaignListener campaignListener)
		{
			GameAnalyticsUnityPluginController.setCampaignListener (campaignListener);
		}
		
		// Trace API
		public static int initializeSdk(string appId, string companyId, string appVersion, bool useLoggingUserId)
		{
			return GameAnalyticsUnityPluginController.initializeSdk (appId, companyId, appVersion, useLoggingUserId);
		}
		
		public static int traceActivation()
		{
			return GameAnalyticsUnityPluginController.traceActivation ();
		}
		
		public static int traceDeactivation()
		{
			return GameAnalyticsUnityPluginController.traceDeactivation ();
		}
		
		public static int traceFriendCount(int friendCount) 
		{
			return GameAnalyticsUnityPluginController.traceFriendCount (friendCount);
		}
		
		public static int tracePurchase(string itemCode, float payment, float unitCost, string currency, int level) 
		{
			return GameAnalyticsUnityPluginController.tracePurchase (itemCode, payment, unitCost, currency, level);
		}
		
		public static int traceMoneyAcquisition(string usageCode, string type, double acquistionAmount, int level) 
		{
			return GameAnalyticsUnityPluginController.traceMoneyAcquisition (usageCode, type, acquistionAmount, level);
		}
		
		public static int traceMoneyConsumption(string usageCode, string type, double consumptionAmount, int level) 
		{
			return GameAnalyticsUnityPluginController.traceMoneyConsumption(usageCode, type, consumptionAmount, level);
		}
		
		public static int traceLevelUp(int level)
		{
			return GameAnalyticsUnityPluginController.traceLevelUp (level);
		}
		
		public static int traceEvent(string eventType, string eventCode, string param1, string param2, double value, int level) 
		{
			return GameAnalyticsUnityPluginController.traceEvent (eventType, eventCode, param1, param2, value, level);
		}
		
		public static int traceStartSpeed(string intervalName)
		{
			return GameAnalyticsUnityPluginController.traceStartSpeed (intervalName);
		}
		
		public static int traceEndSpeed(string intervalName) 
		{
			return GameAnalyticsUnityPluginController.traceEndSpeed (intervalName);
		}

		public static int traceFacebookInstall(string deeplinkUri)
		{
			return GameAnalyticsUnityPluginController.traceFacebookInstall (deeplinkUri);
		}
		
		
		// Campaign API
		public static int showCampaign(string adspaceName)
		{
			return GameAnalyticsUnityPluginController.showCampaign (adspaceName);
		}
		
		public static int showCampaign(string adspaceName, AnimationType animation, int lifeTime) 
		{
			return GameAnalyticsUnityPluginController.showCampaign (adspaceName, (int)animation, lifeTime);
		}
		
		public static int hideCampaign(string adspaceName) 
		{
			return GameAnalyticsUnityPluginController.hideCampaign (adspaceName);
		}
		
		public static int hideCampaign(string adspaceName, AnimationType animation) 
		{
			return GameAnalyticsUnityPluginController.hideCampaign(adspaceName, (int)animation);
		}

		public static Dictionary<string, CampaignInfo> getCampaignInfos()
		{
			return GameAnalyticsUnityPluginController.getCampaignInfos ();
		}


		// Toast Promotion API
		public static bool isPromotionAvailable()
		{
			return GameAnalyticsUnityPluginController.isPromotionAvailable ();
		}

		public static string getPromotionButtonImagePath()
		{
			return GameAnalyticsUnityPluginController.getPromotionButtonImagePath ();
		}
		
		public static int launchPromotionPage()
		{
			return GameAnalyticsUnityPluginController.launchPromotionPage ();
		}
	}
}

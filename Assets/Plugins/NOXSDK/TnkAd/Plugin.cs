using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace TnkAd {
	public class Plugin
	{
		private static Plugin _instance;
		
		public static Plugin Instance {
			get {
				if(_instance == null) {
					_instance = new Plugin();
				}
				return _instance;
			}
		}
	#if UNITY_ANDROID
		private AndroidJavaClass pluginClass;
		
		public Plugin()
		{
			pluginClass = new AndroidJavaClass("com.tnkfactory.ad.unity.TnkUnityPlugin");
		}

		public void initInstance()
		{
			pluginClass.CallStatic ("initInstance");
		}

		public void initInstance(string appId) 
		{
			pluginClass.CallStatic ("initInstance", appId);
		}

		public void prepareVideoAd()
		{
			pluginClass.CallStatic ("prepareVideoAd");
		}

		public void prepareVideoAd(string logicName)
		{
			pluginClass.CallStatic ("prepareVideoAd", logicName);
		}

		public void prepareVideoAdOnce(string logicName, string handlerName)
		{
			pluginClass.CallStatic ("prepareVideoAdOnce", logicName, handlerName);
		}

		public void prepareVideoAd(string logicName, string handlerName)
		{
			pluginClass.CallStatic ("prepareVideoAd", logicName, handlerName);
		}

		public void showVideoAd()
		{
			pluginClass.CallStatic ("showVideoAd");
		}

		public void showVideoCloseButton(bool show) 
		{
			if (show) {
				pluginClass.CallStatic ("showVideoCloseButton");
			} 
			else {
				pluginClass.CallStatic ("hideVideoCloseButton");
			}
		}

		public void showVideoAd(string logicName)
		{
			pluginClass.CallStatic ("showVideoAd", logicName);
		}

		public bool hasVideoAd(string logicName)
		{
			return pluginClass.CallStatic<bool>("hasVideoAd", logicName);
		}

		public void prepareInterstitialAdForPPI()
		{
			pluginClass.CallStatic ("prepareInterstitialAdForPPI");
		}

		public void prepareInterstitialAdForCPC()
		{
			pluginClass.CallStatic ("prepareInterstitialAdForCPC");
		}

		public void prepareInterstitialAd(string logicName)
		{
			pluginClass.CallStatic ("prepareInterstitialAd", logicName);
		}

		public void prepareInterstitialAd(string logicName, string handlerName)
		{
			pluginClass.CallStatic ("prepareInterstitialAd", logicName, handlerName);
		}

		public void showInterstitialAd() {
			pluginClass.CallStatic ("showInterstitialAd");
		}

		public void showInterstitialAd(string logicName) {
			pluginClass.CallStatic ("showInterstitialAd", logicName);
		}

		public void closeButtonAlignRight(bool alignRight) {

		    if (alignRight) {
		        pluginClass.CallStatic ("closeButtonAlignRight");
		    }
		    else {
		        pluginClass.CallStatic ("closeButtonAlignLeft");
		    }
		}

		public void setLeftButtonLabel(string label) {
		    pluginClass.CallStatic ("setLeftButtonLabel", label);
		}

		public void setRightButtonLabel(string label) {
            pluginClass.CallStatic ("setRightButtonLabel", label);
        }

		public void onBackPressed() {
			pluginClass.CallStatic ("onBackPressed");
		}

		public bool isAdViewVisible() {
			return pluginClass.CallStatic<bool> ("isAdViewVisible");
		}

		public bool isInterstitialAdVisible(string logicName) {
			return pluginClass.CallStatic<bool>("isInterstitialAdVisible", logicName);
		}

		public bool isInterstitialAdVisible() {
			return pluginClass.CallStatic<bool> ("isInterstitialAdVisible");
		}

		public void showAdList() {
			pluginClass.CallStatic ("showAdList");
		}

		public void showAdList(string title) {
			pluginClass.CallStatic ("showAdList", title);
		}

		public void popupAdList() {
			pluginClass.CallStatic ("popupAdList");
		}
		
		public void popupAdList(string title) {
			pluginClass.CallStatic ("popupAdList", title);
		}

		public void popupAdList(string title, string handlerName) {
			pluginClass.CallStatic ("popupAdList", title, handlerName);
		}

		public void applicationStarted() {
			pluginClass.CallStatic ("applicationStarted");
		}

		public void actionCompleted() {
			pluginClass.CallStatic ("actionCompleted");
		}

		public void actionCompleted(string actionName) {
			pluginClass.CallStatic ("actionCompleted", actionName);
		}

		public void buyCompleted(string itemName) {
			pluginClass.CallStatic ("buyCompleted", itemName);
		}

		public void queryPoint(string handlerName) {
			pluginClass.CallStatic ("queryPoint", handlerName);
		}

		public void withdrawPoints(string desc, string handlerName) {
			pluginClass.CallStatic ("withdrawPoints", desc, handlerName);
		}
		                                            
		public void purchaseItem(int cost, string itemName, string handlerName) {
			pluginClass.CallStatic ("purchaseItem", cost, itemName, handlerName);
		}

		public void queryPublishState(string handlerName) {
			pluginClass.CallStatic ("queryPublishState", handlerName);
		}

		public void setUserName(string userName) {
			pluginClass.CallStatic ("setUserName", userName);
		}

		public void setUserAge(int age) {
			pluginClass.CallStatic ("setUserAge", age);
		}

		// 0 for male, 1 for female
		public void setUserGender(int gender) {
			pluginClass.CallStatic ("setUserGender", gender);
		}

		public void popupMoreApps() {
			pluginClass.CallStatic ("popupMoreApps");
		}
		public void popupMoreApps(string title) {
			pluginClass.CallStatic ("popupMoreApps", title);
		}
		public void popupMoreApps(string title, string handlerName) {
			pluginClass.CallStatic ("popupMoreApps", title, handlerName);
		}
		public void popupMoreAppsWithButtons(string title, string closeText, string exitText, string handlerName) {
			pluginClass.CallStatic ("popupMoreAppsWithButtons", title, closeText, exitText, handlerName);
		}
	#elif UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void applicationStarted_TnkUnityBridge();
		[DllImport("__Internal")]
		private static extern void actionCompleted_TnkUnityBridge(string actionName);
		[DllImport("__Internal")]
		private static extern void buyCompleted_TnkUnityBridge(string itemName);
		[DllImport("__Internal")]
		private static extern void prepareInterstitialAd_TnkUnityBridge(string logicName, string handlerName);
		[DllImport("__Internal")]
		private static extern void showInterstitialAd_TnkUnityBridge();

		[DllImport("__Internal")]
		private static extern void setCloseButtonAlignLeft_TnkUnityBridge ();
		[DllImport("__Internal")]
		private static extern void setCloseButtonAlignRight_TnkUnityBridge ();
		[DllImport("__Internal")]
		private static extern void setLeftButtonLabel_TnkUnityBridge(string label);
		[DllImport("__Internal")]
		private static extern void setRightButtonLabel_TnkUnityBridge(string label);

		[DllImport("__Internal")]
		private static extern bool isInterstitialAdVisible_TnkUnityBridge();
		[DllImport("__Internal")]
		private static extern void showAdList_TnkUnityBridge(string title, string handlerName);

		[DllImport("__Internal")]
		private static extern void prepareVideoAd_TnkUnityBridge(string logicName, string handlerName);
		[DllImport("__Internal")]
		private static extern void prepareVideoAdOnce_TnkUnityBridge(string logicName, string handlerName);
		[DllImport("__Internal")]
		private static extern void showVideoAd_TnkUnityBridge(string logicName);
		[DllImport("__Internal")]
		private static extern bool hasVideoAd_TnkUnityBridge(string logicName);
		[DllImport("__Internal")]
		private static extern void showVideoClose_TnkUnityBridge();
		[DllImport("__Internal")]
		private static extern void hideVideoClose_TnkUnityBridge();

		[DllImport("__Internal")]
		private static extern void queryPoint_TnkUnityBridge(string handlerName);
		[DllImport("__Internal")]
		private static extern void withdrawPoints_TnkUnityBridge(string desc, string handlerName);
		[DllImport("__Internal")]
		private static extern void purchaseItem_TnkUnityBridge (int cost, string itemName, string handlerName);
		[DllImport("__Internal")]
		private static extern void queryPublishState_TnkUnityBridge (string handlerName);
		[DllImport("__Internal")]
		private static extern void setUserName_TnkUnityBridge (string userName);
		[DllImport("__Internal")]
		private static extern void setUserAge_TnkUnityBridge (int age);
		[DllImport("__Internal")]
		private static extern void setUserGender_TnkUnityBridge (int gender);

		public Plugin() {
		}

		public void initInstance()
		{}
		public void initInstance(string appId) 
		{}

		public void prepareVideoAd()
		{
			prepareVideoAd_TnkUnityBridge ("__tnk_cpc__", null);
		}
		public void prepareVideoAd(string logicName)
		{
			prepareVideoAd_TnkUnityBridge (logicName, null);
		}
		public void prepareVideoAd(string logicName, string handlerName)
		{
			prepareVideoAd_TnkUnityBridge (logicName, handlerName);
		}
		
		public void prepareVideoAdOnce(string logicName, string handlerName)
		{
			prepareVideoAdOnce_TnkUnityBridge (logicName, handlerName);
		}

		public void showVideoAd()
		{
			showVideoAd_TnkUnityBridge (null);
		}
		public void showVideoAd(string logicName)
		{
			showVideoAd_TnkUnityBridge (logicName);
		}
		public bool hasVideoAd(string logicName)
		{ 
			return hasVideoAd_TnkUnityBridge (logicName); 
		}

		public void showVideoCloseButton(bool show) 
		{
			if (show) {
				showVideoClose_TnkUnityBridge();
			} 
			else {
				hideVideoClose_TnkUnityBridge();
			}
		}

		public void prepareInterstitialAdForPPI()
		{
			prepareInterstitialAd_TnkUnityBridge ("__tnk_ppi__", null);
		}
		public void prepareInterstitialAdForCPC()
		{
			prepareInterstitialAd_TnkUnityBridge ("__tnk_cpc__", null);
		}
		public void prepareInterstitialAd(string logicName)
		{
			prepareInterstitialAd_TnkUnityBridge (logicName, null);
		}
		public void prepareInterstitialAd(string logicName, string handlerName) 
		{
			prepareInterstitialAd_TnkUnityBridge (logicName, handlerName);
		}
		public void showInterstitialAd() 
		{
			showInterstitialAd_TnkUnityBridge ();
		}
		public void showInterstitialAd(string logicName) 
		{
			showInterstitialAd_TnkUnityBridge ();
		}

		public void closeButtonAlignRight(bool alignRight) {
			if (alignRight) {
				setCloseButtonAlignRight_TnkUnityBridge();
			}
			else {
				setCloseButtonAlignLeft_TnkUnityBridge();
			}
		}

		public void setLeftButtonLabel(string label) {
			setLeftButtonLabel_TnkUnityBridge(label);
		}

		public void setRightButtonLabel(string label) {
			setRightButtonLabel_TnkUnityBridge(label);
		}


		public void onBackPressed() 
		{}
		public bool isAdViewVisible() {
			return isInterstitialAdVisible_TnkUnityBridge();
		}
		public bool isInterstitialAdVisible(string logicName) {
			return false;
		}
		public bool isInterstitialAdVisible() 
		{ 
			return isInterstitialAdVisible_TnkUnityBridge(); 
		}

		public void showAdList() 
		{
			showAdList_TnkUnityBridge (null, null);
		}
		public void showAdList(string title) 
		{
			showAdList_TnkUnityBridge (title, null);
		}
		public void popupAdList() 
		{
			showAdList_TnkUnityBridge (null, null);
		}
		public void popupAdList(string title) 
		{
			showAdList_TnkUnityBridge (title, null);
		}
		public void popupAdList(string title, string handlerName) 
		{
			showAdList_TnkUnityBridge (title, handlerName);
		}
		public void applicationStarted() 
		{
			applicationStarted_TnkUnityBridge ();
		}
		public void actionCompleted() 
		{
			actionCompleted_TnkUnityBridge (null);
		}
		public void actionCompleted(string actionName) 
		{
			actionCompleted_TnkUnityBridge (actionName);
		}
		public void buyCompleted(string itemName) 
		{
			buyCompleted_TnkUnityBridge (itemName);
		}
		public void queryPoint(string handlerName) 
		{
			queryPoint_TnkUnityBridge (handlerName);
		}
		public void withdrawPoints(string desc, string handlerName) 
		{
			withdrawPoints_TnkUnityBridge (desc, handlerName);
		}
		public void purchaseItem(int cost, string itemName, string handlerName) 
		{
			purchaseItem_TnkUnityBridge (cost, itemName, handlerName);
		}
		public void queryPublishState(string handlerName) 
		{
			queryPublishState_TnkUnityBridge (handlerName);
		}
		public void setUserName(string userName) 
		{
			setUserName_TnkUnityBridge (userName);
		}
		public void setUserAge(int age) 
		{
			setUserAge_TnkUnityBridge (age);
		}
		public void setUserGender(int gender) 
		{
			setUserGender_TnkUnityBridge (gender);
		}

		public void popupMoreApps() 
		{}
		public void popupMoreApps(string title) 
		{}
		public void popupMoreApps(string title, string handlerName) 
		{}
		public void popupMoreAppsWithButtons(string title, string closeText, string exitText, string handlerName) 
		{}
	#else
		public Plugin() {
		}

		public void initInstance()
		{}
		public void initInstance(string appId) 
		{}

		public void prepareVideoAd()
		{}
		public void prepareVideoAd(string logicName)
		{}
		public void prepareVideoAd(string logicName, string handlerName)
		{}
		public void prepareVideoAdOnce(string logicName, string handlerName)
		{}
		public void showVideoAd(string logicName)
		{}
		public void showVideoAd()
		{}
		public bool hasVideoAd(string logicName)
		{ return false; }
		public void showVideoCloseButton(bool show) 
		{}
		public void prepareInterstitialAdForPPI()
		{}
		public void prepareInterstitialAdForCPC()
		{}
		public void prepareInterstitialAd(string logicName)
		{}
		public void prepareInterstitialAd(string logicName, string handlerName) 
		{}
		public void showInterstitialAd() 
		{}
		public void showInterstitialAd(string logicName) 
		{}
		public void onBackPressed() 
		{}
		public bool isAdViewVisible() 
		{ return false; }
		public bool isInterstitialAdVisible(string logicName) 
		{ return false; }
		public bool isInterstitialAdVisible() 
		{ return false; }
		public void showAdList() 
		{}
		public void showAdList(string title) 
		{}
		public void popupAdList() 
		{}
		public void popupAdList(string title) 
		{}
		public void popupAdList(string title, string handlerName) 
		{}
		public void applicationStarted() 
		{}
		public void actionCompleted() 
		{}
		public void actionCompleted(string actionName) 
		{}
		public void buyCompleted(string itemName) 
		{}
		public void queryPoint(string handlerName) 
		{}
		public void withdrawPoints(string handlerName)
		{}
		public void purchaseItem(int cost, string itemName, string handlerName) 
		{}
		public void queryPublishState(string handlerName) 
		{}
		public void setUserName(string userName) 
		{}
		public void setUserAge(int age) 
		{}
		public void setUserGender(int gender) 
		{}
		public void popupMoreApps() 
		{}
		public void popupMoreApps(string title) 
		{}
		public void popupMoreApps(string title, string handlerName) 
		{}
		public void popupMoreAppsWithButtons(string title, string closeText, string exitText, string handlerName) 
		{}
	#endif
	}
}

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class CashslideWrapper  {

#if UNITY_ANDROID
	public static void AppFirstLaunched(string appId) {
		AndroidJavaObject cashslide = GetCashslide(appId);
		cashslide.Call("appFirstLaunched");
	}

	public static void MissionCompleted(string appId) {
		AndroidJavaObject cashslide = GetCashslide(appId);
		cashslide.Call("missionCompleted");
	}

	public static void Recommend(string appId) {
		AndroidJavaObject cashslide = GetCashslide(appId);
		cashslide.Call("recommend");
	}

	public static void SetRetentionTracking(string appId, bool retentionTracking) {
		AndroidJavaObject cashslide = GetCashslide(appId);
		cashslide.Call("setRetentionTracking", retentionTracking);
	}

	private static AndroidJavaObject GetCashslide(string appId) {
		AndroidJavaObject context = GetContext();

		AndroidJavaObject cashslide = new AndroidJavaObject(
			"kr.co.cashslide.Cashslide", 
			context,
			appId
		);

		return cashslide;
	}

	private static AndroidJavaObject GetContext() {
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject app = activity.Call<AndroidJavaObject>("getApplicationContext");

		return app;
	}
#else
	public static void AppFirstLaunched(string appId) {}
	public static void MissionCompleted(string appId) {}
	public static void Recommend(string appId) {}
	public static void SetRetentionTracking(string appId, bool retentionTracking) {}
#endif
}


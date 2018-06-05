using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Toast.Analytics;

public class ToastMgr : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int result = 0;

		if (NOXSDK.Instance.StoreType == STORETYPE.GOOGLE) {
			result = GameAnalytics.initializeSdk("vl2j02jJqExQMhwz", "f9yD4ZWIT64Dam9B", "1.0", true);
		} else if (NOXSDK.Instance.StoreType == STORETYPE.ONESTORE) {
			result = GameAnalytics.initializeSdk("mLJ35yJwFgT8PPN6", "f9yD4ZWIT64Dam9B", "1.0", true);
		}

		if (result != 0)
		{
			// 토스트 에러..
			Debug.Log("Error GameAnalytics.initializeSdk");
		}
		GameAnalytics.setUserId(SystemInfo.deviceUniqueIdentifier, true);
		GameAnalytics.traceActivation();
	}

	public void OnDestroy()
	{
		GameAnalytics.traceDeactivation();
	}

	// 앱 복귀 및 일시 정지시 처리
	public void OnApplicationPause(bool pause)
	{
		if (pause == true)
		{
			GameAnalytics.traceDeactivation();
		}
		else
		{
			GameAnalytics.traceActivation();
		}
	}
}

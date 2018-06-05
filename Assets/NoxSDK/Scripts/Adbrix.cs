using UnityEngine;
using System.Collections;
using IgaworksUnityAOS;

public class Adbrix : MonoBehaviour {
	// Use this for initialization
	IEnumerator Start () {
		//유니티 엔진이 초기화될 때, IGAW 플러그인을 초기화 합니다.
		IgaworksUnityPluginAOS.InitPlugin();
		//네이티브 SDK를 초기화 합니다.
		IgaworksUnityPluginAOS.Common.startApplication();
		IgaworksUnityPluginAOS.Common.startSession();
		IgaworksUnityPluginAOS.Common.setUserId(SystemInfo.deviceUniqueIdentifier);
		//  팝업을 위한 리소스 불러오기
		yield return null;
		if (Application.isEditor == false) {
			IgaworksUnityPluginAOS.LiveOps.initialize ();
			IgaworksUnityPluginAOS.LiveOps.requestPopupResource ();
			IgaworksUnityPluginAOS.LiveOps.enableService (true);
			IgaworksUnityPluginAOS.LiveOps.setNotificationIconStyle("ic_stat_white_icon_g", "app_icon", "ffa2a2a2");
		}
	}

	public void OnDestroy()
	{
		IgaworksUnityPluginAOS.Common.endSession();
		IgaworksUnityPluginAOS.LiveOps.pause();
	}

	// 앱 복귀 및 일시 정지시 처리
	public void OnApplicationPause(bool pause)
	{
		if (pause == true)
		{
			IgaworksUnityPluginAOS.Common.endSession();
		}
		else
		{
			IgaworksUnityPluginAOS.Common.startSession();
			IgaworksUnityPluginAOS.LiveOps.resume();
		}
	}

	public void SetPushService(bool value)
	{
		IgaworksUnityPluginAOS.LiveOps.enableService(value);
	}

	public void SetNormalClientPushEvent(long second, string contenttext, int eventid)
	{
		IgaworksUnityPluginAOS.LiveOps.setNormalClientPushEvent(second, contenttext, eventid, false);
	}

	public void CancelNormalClientPushEvent(int eventID)
	{
		IgaworksUnityPluginAOS.LiveOps.cancelClientPushEvent(eventID);
	}

	public void OpenLobbyPopupWindow()
	{
		string popupSpace = "FirstScreen";
		IgaworksUnityPluginAOS.LiveOps.showPopUp(popupSpace);
	}
}

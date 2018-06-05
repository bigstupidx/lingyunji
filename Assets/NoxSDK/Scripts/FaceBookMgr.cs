using UnityEngine;
using System.Collections;
using Facebook;

public class FaceBookMgr : MonoBehaviour {
	
	public string IdToken;

	// Use this for initialization
	void Start () {
		#if !UNITY_EDITOR
		if (FB.IsInitialized == false) {
			FB.Init (InitCallback, OnHideUnity);
		}
		#else
		IdToken = "PCID" + SystemInfo.deviceUniqueIdentifier;
		#endif
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			FB.Login("public_profile, email", AuthCallback);
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}
	private void AuthCallback (FBResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			string aToken = FB.AccessToken;
			// Print current access token's User ID
			Debug.Log(FB.UserId);
			IdToken = FB.UserId;
//debug 报错临时处理            ((KrAndriodSDKHelper)(SDKHelper.instance.sdkParent)).SetLoginType(KrAndriodSDKHelper.LoginType.facebook);
            SDKHelper.instance.OnMessage(SDKHelper.MSG_LOGIN_SUCCESS);
			// Print current access token's granted permissions
		} else {
			Debug.Log("User cancelled login");
		}
	}

	public string GetIDToken()
	{
		return IdToken;
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}
}

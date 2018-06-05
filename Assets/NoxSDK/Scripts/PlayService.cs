using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;


public class PlayService : MonoBehaviour
{

    public string IdToken;

    // Use this for initialization
    void Start()
    {
//#if !UNITY_EDITOR
//		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
//			// enables saving game progress.
//			//.EnableSavedGames()
//			// registers a callback to handle game invitations received while the game is not running.
//			.Build();
		
//		PlayGamesPlatform.InitializeInstance(config);
//		// recommended for debugging:
//		PlayGamesPlatform.DebugLogEnabled = true;
//		// Activate the Google Play Games platform
//		PlayGamesPlatform.Activate();
//#endif
        if (Social.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                // handle success or failure
                if (success == true)
                {
                    Debug.Log("Google user ID : " + Social.localUser.id);
                    Debug.Log("Google Login Success");
                    StartCoroutine(_GetIDToken());
                }
                else
                {
                    Debug.Log("Google Login Failed");
                    SDKHelper.instance.LoginFail();
                }
            });
        }
    }

    public string GetIDToken()
    {
        return IdToken;
    }

    IEnumerator _GetIDToken()
    {
#if !UNITY_EDITOR
		while (Social.localUser.id == "0" && Social.localUser.id == "" && Social.localUser.id == null) {
			yield return null;
		}
		IdToken = Social.localUser.id;
#else
        yield return null;
        IdToken = "PCID" + (SystemInfo.deviceUniqueIdentifier);
#endif
        //debug 报错临时处理            ((KrAndriodSDKHelper)(SDKHelper.instance.sdkParent)).SetLoginType(KrAndriodSDKHelper.LoginType.google);
        SDKHelper.instance.OnMessage(SDKHelper.MSG_LOGIN_SUCCESS);
        Debug.Log("SetIDToken : " + IdToken);
    }
}

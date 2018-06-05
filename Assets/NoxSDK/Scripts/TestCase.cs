using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class TestCase : MonoBehaviour {
	
	public static TestCase instance;
	const float X_OFFSET = 10.0f;
	const float Y_OFFSET = 10.0f;
	const int SMALL_SCREEN_SIZE = 800;
	const int LARGE_FONT_SIZE = 34;
	const int SMALL_FONT_SIZE = 24;
	const int LARGE_WIDTH = 380;
	const int SMALL_WIDTH = 160;
	const int LARGE_HEIGHT = 100;
	const int SMALL_HEIGHT = 40;
	
	int _column = 0;
	int _row = 0;
	
	public string _label = "";
	
	// Use this for initialization
	void Start () {
		instance = this;
		NOXSDK.Instance.Init ();
		/*string json = "{\"api_version\":\"4\",\"identifier\":\"1477984099815\",\"method\":\"purchase\",\"result\":{\"code\":\"0000\",\"message\":\"요청이 성공했습니다.\",\"count\":1,\"txid\":\"TSTOREXXXX_..\",\"receipt\":\".......\"}}";
		JsonData info = JsonMapper.ToObject<JsonData>(json);
		string api_version = "";
		string identifier = "";
		string method = "";
		string code = "";
		string message = "";
		string count = "";
		string txid = "";
		string receipt = "";
		if (info.Keys.Contains ("api_version")) {
			api_version = info["api_version"].ToString();
		}
		if (info.Keys.Contains ("identifier")) {
			identifier = info["identifier"].ToString();
		}
		if (info.Keys.Contains ("method")) {
			method = info["method"].ToString();
		}
		if (info.Keys.Contains ("result")) {
			if(info["result"].Keys.Contains("code"))
			{
				code = info["result"]["code"].ToString();
			}
			if(info["result"].Keys.Contains("message"))
			{
				message = info["result"]["message"].ToString();
			}
			if(info["result"].Keys.Contains("count"))
			{
				count = info["result"]["count"].ToString();
			}
			if(info["result"].Keys.Contains("txid"))
			{
				txid = info["result"]["txid"].ToString();
			}
			if(info["result"].Keys.Contains("receipt"))
			{
				receipt = info["result"]["receipt"].ToString();
			}
		}
		Debug.Log (api_version);
		Debug.Log (identifier);
		Debug.Log (method);
		Debug.Log (code);
		Debug.Log (message);
		Debug.Log (count);
		Debug.Log (txid);
		Debug.Log (receipt);*/
	}
	
	private bool Button(string text)
	{
		float width = Screen.width / 2.0f - X_OFFSET * 2;
		float height = (Screen.width >= SMALL_SCREEN_SIZE || Screen.height >= SMALL_SCREEN_SIZE) ? LARGE_HEIGHT : SMALL_HEIGHT;
		
		bool click = GUI.Button(new Rect(
			X_OFFSET + _column * X_OFFSET * 2 + _column * width, 
			Y_OFFSET + _row * Y_OFFSET + _row * height, 
			width, height),
		                        text);
		
		++_column;
		if (_column > 1)
		{
			_column = 0;
			++_row;
		}
		
		return click;
	}
	
	private bool BigButton(string text)
	{
		float width = Screen.width / 2.0f - X_OFFSET * 2;
		float height = (Screen.width >= SMALL_SCREEN_SIZE || Screen.height >= SMALL_SCREEN_SIZE) ? LARGE_HEIGHT : SMALL_HEIGHT;
		
		bool click = GUI.Button(new Rect(
			X_OFFSET + _column * X_OFFSET * 2 + _column * width, 
			Y_OFFSET + _row * Y_OFFSET + _row * height, 
			width * 2, height * 2),
		                        text);
		
		_row += 2;
		
		return click;
	}
	
	private void Reset_label()
	{
		_label = "";
	}
	
	
	private void OnGUI()
	{
		_column = 0;
		_row = 0;
		
		GUI.skin.button.fontSize = (Screen.width >= SMALL_SCREEN_SIZE || Screen.height >= SMALL_SCREEN_SIZE) ? LARGE_FONT_SIZE : SMALL_FONT_SIZE;
		
		if (BigButton(_label))
		{
			
		}
		
		if (Button("GPS Login"))
		{
			_label = "Google Login......\n";
			NOXSDK.Instance.OnGoogleLogin ();
			StartCoroutine(waitGoogleLogin());
		}
		
		if (Button("FB Login"))
		{
			_label = "FaceBook Login......\n";
			NOXSDK.Instance.OnFacebookLogin ();
			StartCoroutine(waitFaceBookLogin());	
		}
		
		if (Button("G IAP Init"))
		{
			List<string> listSKU = new List<string> ();
			listSKU.Add ("test_product");
			NOXSDK.Instance.InitIAP(listSKU);
		}		
		
		if (Button("G IAP"))
		{
			NOXSDK.Instance.Purchase("test_product");
		}

		if (Button("G LOST"))
		{
			NOXSDK.Instance.CheckLostPurchase();
		}

		if (Button("O IAP"))
		{
			NOXSDK.Instance.Purchase("0910088865");
		}
	}
	
	IEnumerator waitGoogleLogin()
	{
		while (true) {
			if(NOXSDK.Instance.GetGoogleID() != "0" && NOXSDK.Instance.GetGoogleID() != "")
			{
				_label = "Google Login......\n" + NOXSDK.Instance.GetGoogleID();
				break;
			}
			yield return null;
		}
	}
	
	IEnumerator waitFaceBookLogin()
	{
		while (true) {
			if(!string.IsNullOrEmpty(NOXSDK.Instance.GetFaceBookID()))
			{
				_label = "FaceBook Login......\n" + NOXSDK.Instance.GetFaceBookID();
				break;
			}
			yield return null;
		}
	}
}

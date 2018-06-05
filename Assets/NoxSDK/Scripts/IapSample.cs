//#define IAP_GOOGLE
using UnityEngine;
using System.Collections;
using IapResponse;
using IapVerifyReceipt;
using IapError;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using OnePF;
using LitJson;

public class IapSample : MonoBehaviour {
	//#if UNITY_ANDROID
	private AndroidJavaClass unityPlayerClass = null;
	private AndroidJavaObject currentActivity = null;
	private AndroidJavaObject iapRequestAdapter = null;
	string PID;
	
	IEnumerator Start () 
	{
		yield return null;
		if(Application.isEditor == false)
		{
			unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			yield return null;
			currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
			yield return null;
			if (currentActivity != null)
			{
				iapRequestAdapter = new AndroidJavaObject("causallink.assets.RequestAdapter", "IapSample", currentActivity, NOXSDK.Instance.OpenIABObject.DEBUG);
			}
			if (iapRequestAdapter == null)
			{
				Debug.Log("Error : iapRequestAdapter == null");
			}
		}
	}
	
	void Destroy () 
	{
		if (unityPlayerClass != null)
			unityPlayerClass.Dispose ();
		if (currentActivity != null)
			currentActivity.Dispose ();
		if (iapRequestAdapter != null)
			iapRequestAdapter.Dispose ();
	}
	
	public void RequestProductInfo() 
	{
		iapRequestAdapter.Call ("requestProductInfo", true, NOXSDK.Instance.OpenIABObject.onestoreKey);
	}
	
	public void RequestCheckPurchasability(string pID) 
	{
		iapRequestAdapter.Call ("requestCheckPurchasability", true, NOXSDK.Instance.OpenIABObject.onestoreKey, pID);
	}
	
	public void RequestSubtractPoints(string pID) 
	{
		iapRequestAdapter.Call ("requestChangeProductProperties", true, "subtract_points", NOXSDK.Instance.OpenIABObject.onestoreKey, pID);
	}
	
	public void RequestCancelSubscription(string pID) 
	{
		iapRequestAdapter.Call ("requestChangeProductProperties", true, "cancel_subscription", NOXSDK.Instance.OpenIABObject.onestoreKey, pID);
	}
	
	
	//------------------------------------------------
	//
	// Command - Callback
	//
	//------------------------------------------------
	
	public void CommandResponse(string response) 
	{
		Debug.Log ("--------------------------------------------------------");
		Debug.Log ("[UNITY] CommandResponse >>> " + response);
		Debug.Log ("--------------------------------------------------------");
	}
	
	public void CommandError(string message) 
	{
		Debug.Log ("--------------------------------------------------------");
		Debug.Log ("[UNITY] CommandError >>> " + message);
		Debug.Log ("--------------------------------------------------------");
	}
	
	public void RequestPaymenet(string pid)
	{
		string tid = "NOXTID"; //getTid();
		PlayerPrefs.SetString("TID", tid);
		if (iapRequestAdapter != null) {
			iapRequestAdapter.Call ("requestPayment", NOXSDK.Instance.OpenIABObject.onestoreKey, pid, "", tid, "");
		} else if (Application.isEditor == true) {
			Purchase pur = new Purchase ();
			pur.Sku = PID;
			pur.OrderId = "TEST:ORDERID";
			pur.Token = "TEST:TOKEN";
			pur.OriginalJson = "TEST:JSON";
			pur.ItemType = "ITEM_TYPE_INAPP";
			pur.AppstoreName = "ONE_STORE";
			pur.PurchaseTime = 0;
			pur.PurchaseState = 0;
			pur.Receipt = "TEST:RECEIPT";
			NOXSDK.Instance.PurchaseSucceede (pur);
		}
		PID = pid;
	}
	
	/*public string getTid()
	{
		string result = "";
		Socket m_Socket;
		
		string iPAdress = "169.56.70.55";
		int kPort = 11950;
		
		m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
		m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
		
		try
		{
			IPAddress ipAddr = System.Net.IPAddress.Parse(iPAdress);
			IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, kPort);
			m_Socket.Connect(ipEndPoint);
			
			try
			{
				
				byte[] data_size = new byte[4];
				m_Socket.Receive(data_size);
				int PacketSize = BitConverter.ToInt32(data_size, 0);
				byte[] MainPacket = new byte[PacketSize];
				int offset = 0;
				while (true)
				{
					int bytes = m_Socket.Receive(MainPacket, offset, PacketSize, SocketFlags.None);
					offset += bytes;
					PacketSize -= bytes;
					
					if (PacketSize <= 0)
					{
						break;
					}
				}
				string tid = Encoding.UTF8.GetString(MainPacket);
				result = tid;
			}
			catch (SocketException err)
			{
				Debug.Log("Socket send or receive error! : " + err.ToString());
			}
			
		}
		catch (SocketException SCE)
		{
			Debug.Log("Socket connect error! : " + SCE.ToString());
			
		}
		
		m_Socket.Close();
		m_Socket = null;
		
		Debug.Log("TID : " + result);
		return result;
	}*/
	
	
	public string RequestOldPaymenet()
	{
		string result = PlayerPrefs.GetString("TID", "");
		return result;
	}
	
	public void ResetOldPaymenet()
	{
		PlayerPrefs.SetString("TID", "");
	}

	public void PaymentResponse(string response) 
	{
		Debug.Log ("--------------------------------------------------------");
		Debug.Log ("[UNITY] PaymentResponse >>> " + response);
		Debug.Log ("--------------------------------------------------------");

		JsonData info = JsonMapper.ToObject<JsonData>(response);
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

		Purchase pur = new Purchase ();
		pur.Sku = PID;
		pur.OrderId = txid;
		pur.Token = receipt;
		pur.OriginalJson = response;
		pur.ItemType = "ITEM_TYPE_INAPP";
		pur.AppstoreName = "ONE_STORE";
		pur.PurchaseTime = 0;
		pur.PurchaseState = 0;
		pur.Receipt = receipt;
		NOXSDK.Instance.PurchaseSucceede (pur);
		PID = "";
	}
	
	public void PaymentError(string message) 
	{
		Debug.Log ("--------------------------------------------------------");
		Debug.Log ("[UNITY] PaymentError >>> " + message);
		Debug.Log ("--------------------------------------------------------");
		//OneStoreIapManager.Instance.PurchaseFailed(message);
		PID = "";
	}

	public void ReceiptVerificationResponse(string result) 
	{
		Debug.Log ("--------------------------------------------------------");
		Debug.Log ("[UNITY] ReceiptVerificationResponse >>> " + result);
		Debug.Log ("--------------------------------------------------------");
	}

	public void ReceiptVerificationError(string message) 
	{
		Debug.Log ("--------------------------------------------------------");
		Debug.Log ("[UNITY] ReceiptVerificationError >>> " + message);
		Debug.Log ("--------------------------------------------------------");
	}
	//#endif
}

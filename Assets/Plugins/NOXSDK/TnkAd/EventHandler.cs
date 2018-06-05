using UnityEngine;
using System.Collections;
using System;

namespace TnkAd {
	public class EventHandler : MonoBehaviour {
		// publishing state 
		public const int PUB_STAT_NO = 0; // not publishing yet
		public const int PUB_STAT_YES = 1; // publising state
		public const int PUB_STAT_TEST = 2; // testing state

		// onClose(int type)
		public const int CLOSE_SIMPLE = 0; // users simply closed ad view.
		public const int CLOSE_CLICK = 1; // users clicked ad view.
		public const int CLOSE_EXIT = 2; // users clicked exit app button.

		// onFailure(int errCode)
		public const int FAIL_NO_AD = -1;  // no ad available
		public const int FAIL_NO_IMAGE = -2; // ad image not available
		public const int FAIL_TIMEOUT = -3;  // ad not arrived in 5 secs.
		public const int FAIL_CANCELED = -4; // ad frequency setting
		public const int FAIL_NOT_PREPARED = -5; // prepare not invoked.

		public const int FAIL_SYSTEM = -9;

		// Set 'Handler Name' in Unity Inspector
		public string handlerName;
		
		void Awake() {
			//Debug.Log("##### Awake " + handlerName);
			gameObject.name = handlerName;
			DontDestroyOnLoad( gameObject );
		}
		
		public void onReturnQueryPointBinding(string point) {
			//Debug.Log("##### onReturnQueryPointBinding " + point);
			int pnt = int.Parse (point);
			onReturnQueryPoint (pnt);
		}

		public void onReturnWithdrawPointsBinding(string point) {
			int pnt = int.Parse (point);
			onReturnWithdrawPoints (pnt);
		}

		public void onReturnPurchaseItemBinding(string point) {
			//Debug.Log("##### onReturnPurcaseItemBinding " + point);
			char[] delimiterChars = {','};
			string[] str = point.Split (delimiterChars);
			long pnt = long.Parse (str[0]);
			long seq = long.Parse (str[1]);
			onReturnPurchaseItem (pnt, seq);
		}

		public void onReturnQueryPublishStateBinding(string state) {
			//Debug.Log("##### onReturnQueryPublishStateBinding " + state);
			int stat = int.Parse (state);
			onReturnQueryPublishState (stat);
		}

		public void onCloseBinding(string type) {
			//Debug.Log("##### onCloseBinding " + type);
			int typeCode = int.Parse (type);
			onClose (typeCode);
		}
		
		public void onFailureBinding(string err) {
			//Debug.Log("##### onFailureBinding " + err);
			int errCode = int.Parse (err);
			onFailure (errCode);
		}
		
		public void onLoadBinding(string dummy) {
			//Debug.Log("##### onLoadBinding ");
			onLoad ();
		}
		
		public void onShowBinding(string dummy) {
			//Debug.Log("##### onShowBinding ");
			onShow ();
		}

		public void onVideoCompletedBinding(string skipped) {
			//Debug.Log("##### onVideoCompletedBinding " + skipped);
			if (String.Compare (skipped, "Y") == 0) {
				onVideoCompleted(true);
			} 
			else {
				onVideoCompleted (false);
			}
		}

		// TnkAdListener
		public virtual void onClose (int type) {}
		public virtual void onFailure (int errCode) {}
		public virtual void onLoad() {}
		public virtual void onShow() {}

		// VideoAdListener
		public virtual void onVideoCompleted(bool skipped) {}

		// ServiceCallback
		public virtual void onReturnQueryPoint (int point) {}
		public virtual void onReturnWithdrawPoints (int point) {}
		public virtual void onReturnPurchaseItem(long curPoint, long seqId) {}
		public virtual void onReturnQueryPublishState(int state) {}
	}
}
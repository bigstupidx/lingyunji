using UnityEngine;
using System;
using System.Collections.Generic;

namespace Toast.Analytics {
	public interface CampaignListener {
		void OnCampaignVisibilityChanged(string adspaceName, bool show);
		void OnCampaignLoadSuccess(string adspaceName);
		void OnCampaignLoadFail(string adspaceName, int errorCode, string errorMessage);
		void OnMissionComplete(List<string> missionList);
		void OnPromotionVisibilityChanged(bool show);
		void OnCampaignClick(string callbackInfo);
	}
}
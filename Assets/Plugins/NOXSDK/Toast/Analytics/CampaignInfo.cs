using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Toast.Analytics 
{
	public class CampaignInfo
	{
		private string missionKey;
		public string MissionKey {
			get { return missionKey; }
		}

		private int missionVal;
		public int MissionVal {
			get { return missionVal; }
		}

		private DateTime promotionDateBegin;
		public DateTime PromotionDateBegin {
			get { return promotionDateBegin; }
		}

		private DateTime promotionDateEnd;
		public DateTime PromotionDateEnd {
			get { return promotionDateEnd; }
		}

		public CampaignInfo (Dictionary<string, object> campaignInfo)
		{
			this.missionKey = (string)campaignInfo ["missionKey"];
			this.missionVal = Convert.ToInt32(campaignInfo ["missionVal"]);
		
			this.promotionDateBegin = DateTime.ParseExact ((string)campaignInfo ["promotionDateBegin"], 
			                                               "yyyy-MM-dd HH:mm:ss.fff",
			                                               System.Globalization.CultureInfo.InvariantCulture);

			this.promotionDateEnd = DateTime.ParseExact ((string)campaignInfo ["promotionDateEnd"], 
			                                             "yyyy-MM-dd HH:mm:ss.fff",
			                                             System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}


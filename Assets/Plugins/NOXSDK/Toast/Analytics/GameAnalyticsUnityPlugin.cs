using UnityEngine;
using System.Collections;

namespace Toast.Analytics {

	public class GameAnalyticsUnityPlugin {

		static public GameAnalyticsUnityPlugin _instance;
		static public GameAnalyticsUnityPlugin instance
		{
			get
			{ 
				if(_instance == null)
				{
					_instance = new GameAnalyticsUnityPlugin();
				}
				return _instance;
			}
		}

		public GameAnalyticsUnityPluginController _controller;
		public GameAnalyticsUnityPluginController controller
		{
			get
			{
				if(_controller == null)
					((GameAnalyticsUnityPluginController)UnityEngine.Object.FindObjectOfType(typeof(GameAnalyticsUnityPluginController))).Awake();
				return _controller;
			}
			set
			{
				if(_controller == value)
					return;
				
				_controller = value;
			}
		}

	}
}
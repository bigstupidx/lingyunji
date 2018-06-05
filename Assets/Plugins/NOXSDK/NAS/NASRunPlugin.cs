using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Nas {

	public class NASRunPlugin
	{
		private static NASRunPlugin _instance;
		
		public static NASRunPlugin Instance {
			get {
				if(_instance == null) {
					_instance = new NASRunPlugin();
				}
				return _instance;
			}
		}
			
	#if UNITY_ANDROID
		private AndroidJavaClass pluginClass;

		public NASRunPlugin()
		{
			pluginClass = new AndroidJavaClass("com.nas.run.unity.NasrunUnityPlugin");
		}

		public void run(string adKey)
		{
			pluginClass.CallStatic ("run", adKey);
		}

	#elif UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void run_UnityBridge (string adKey);

		public NASRunPlugin()
		{}

		public void run(string adKey) 
		{
			run_UnityBridge (adKey);
		}
	#else
		public NASRunPlugin()
		{}
		public void run(string adKey)
		{}
	#endif

	}
}
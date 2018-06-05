using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using WellFired.Shared;
using System.Reflection;

namespace WellFired.Shared
{
	public abstract class WelcomeScreenWindow : EditorWindow
	{
		#region Member Variables
		private VersionInformation versionInformation;

		public abstract string ProductName { get; }
		public abstract string ProductDescription { get; }
		public abstract string ReadableProductName { get; }
		public abstract string DocumentationURL { get; }
		public abstract string DocumentationText { get; }
		
		private static bool forceShow;

		private string WindowPref
		{
			get
			{
				return string.Format("WellFired.{0}.{1}", ProductName, versionInformation.FullVersionString);
			}
		}

		private Texture DocumentationLogo = null;
		private Texture VideosLogo = null;
		private Texture CommunityLogo = null;
		private Texture FeedbackLogo = null;
		private Texture WebsiteLogo = null;
		private Texture BlogLogo = null;
		
		private Texture TwitterLogo = null;
		private Texture YoutubeLogo = null;
		private Texture FacebookLogo = null;
		private Texture GoogleLogo = null;
		
		private GUISkin LoadedSkin = null;
		private GUIStyle Heading1Style = null;
		private GUIStyle SubHeading1Style = null;
		private GUIStyle SubHeading2Style = null;
		private GUIStyle NormalStyle = null;
		#endregion
		
		public static void OpenWindow<T>(bool force) where T : EditorWindow
		{
			var window = EditorWindow.GetWindow<T>(true, "Welcome", true) as T;
			window.position = new Rect(350, 150, 400, 620);
			forceShow = force;
		}

		private void OnEnable()
		{
			if(versionInformation == null)
				versionInformation = VersionInformationUtilities.GetVersionInformationForProduct(ProductName);
		}
		
		private void OnGUI()
		{
			if(!forceShow)
			{
				if(!EditorPrefs.GetBool(WindowPref, true))
				{
					Close();
					return;
				}
			}

			if(!LoadAssets())
			{
				Debug.LogWarning("There was a problem loading assets, the welcome screen will not display correctly.");
				return;
			}
				
			using(new WellFired.Shared.GUIBeginVertical())	
			{
				int distanceFromEdge = 100;
			
				Rect area = new Rect(0, 0, position.width, 107);
				using(new WellFired.Shared.GUIBeginArea(area, ""))
				{
					using(new WellFired.Shared.GUIBeginHorizontal())
					{
						using(new WellFired.Shared.GUIBeginVertical())
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label(string.Format("Welcome to {0}!", ReadableProductName), Heading1Style, GUILayout.MinWidth (position.width));
							GUILayout.Space(6);
							GUILayout.Label(ProductDescription, SubHeading1Style, GUILayout.MinWidth(position.width));
					
							GUILayout.FlexibleSpace();
						}
						GUILayout.FlexibleSpace();
					}
				}
			
				distanceFromEdge = 150;

				area.y = 100;
				area.height = 80;
				using(new WellFired.Shared.GUIBeginArea(area, ""))
				{
					using(new WellFired.Shared.GUIBeginHorizontal())
					{
						GUILayout.FlexibleSpace();
						if(GUILayout.Button(TwitterLogo, GUILayout.Width(60.0f), GUILayout.Height(60.0f)))
							OpenTwitterWindow();
						if(GUILayout.Button(FacebookLogo, GUILayout.Width(60.0f), GUILayout.Height(60.0f)))
							OpenFacebookWindow();
						if(GUILayout.Button(GoogleLogo, GUILayout.Width(60.0f), GUILayout.Height(60.0f)))
							OpenGoogleWindow();
						if(GUILayout.Button(YoutubeLogo, GUILayout.Width(60.0f), GUILayout.Height(60.0f)))
							OpenYoutubeWindow();
						GUILayout.FlexibleSpace();
					}
				}
			
				area.y = 180;
				area.height = 80;
				using(new WellFired.Shared.GUIBeginArea(area, ""))
				{
					using(new WellFired.Shared.GUIBeginHorizontal())
					{
						GUILayout.Space(10);
						if(GUILayout.Button(WebsiteLogo, GUILayout.Width(75.0f), GUILayout.Height(75.0f)))
							OpenWebsiteWindow();
						GUILayout.Space(10);
						using(new WellFired.Shared.GUIBeginVertical())
						{
							GUILayout.Space(10);
							GUILayout.Label("Website", SubHeading2Style, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Space(6);
							GUILayout.Label("The official Well Fired website is packed with ", NormalStyle, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Label("information.", NormalStyle, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.FlexibleSpace();
						}
						GUILayout.FlexibleSpace();
					}
				}
				
				area.y += area.height;
				using(new WellFired.Shared.GUIBeginArea(area, ""))
				{
					using(new WellFired.Shared.GUIBeginHorizontal())
					{
						GUILayout.Space(10);
						if(GUILayout.Button(BlogLogo, GUILayout.Width(75.0f), GUILayout.Height(75.0f)))
							OpenBlogWindow();
						GUILayout.Space(10);
						using(new WellFired.Shared.GUIBeginVertical())
						{
							GUILayout.Space(4);
							GUILayout.Label("Blog", SubHeading2Style, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Space(6);
							GUILayout.Label("For lots of cool tips and tricks, including technical", NormalStyle, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Label("write ups, be sure to check out our development blog!", NormalStyle, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.FlexibleSpace();
						}
						GUILayout.FlexibleSpace();
					}
				}
				
				area.y += area.height;
				using(new WellFired.Shared.GUIBeginArea(area, ""))
				{
					using(new WellFired.Shared.GUIBeginHorizontal())
					{
						GUILayout.Space(10);
						if(GUILayout.Button(DocumentationLogo, GUILayout.Width(75.0f), GUILayout.Height(75.0f)))
							OpenDocumentationWindow();
						GUILayout.Space(10);
						using(new WellFired.Shared.GUIBeginVertical())
						{
							GUILayout.Space(4);
							GUILayout.Label("Documentation", SubHeading2Style, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Space(6);
							GUILayout.Label(DocumentationText, NormalStyle, GUILayout.MinWidth (position.width - distanceFromEdge));
							GUILayout.FlexibleSpace();
						}
						GUILayout.FlexibleSpace();
					}
				}
				
				area.y += area.height;
				using(new WellFired.Shared.GUIBeginArea(area, ""))
				{
					using(new WellFired.Shared.GUIBeginHorizontal())
					{
						GUILayout.Space(10);
						if(GUILayout.Button(CommunityLogo, GUILayout.Width(75.0f), GUILayout.Height(75.0f)))
							OpenCommunityWindow();
						GUILayout.Space(10);
						using(new WellFired.Shared.GUIBeginVertical())
						{
							GUILayout.Space(10);
							GUILayout.Label("Community", SubHeading2Style, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Space(6);
							GUILayout.Label("The community is growing daily, visit the", NormalStyle, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Label("forum and join in with the conversation", NormalStyle, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.FlexibleSpace();
						}
						GUILayout.FlexibleSpace();
					}
				}
			
				area.y += area.height;
				using(new WellFired.Shared.GUIBeginArea(area, ""))
				{
					using(new WellFired.Shared.GUIBeginHorizontal())
					{
						GUILayout.Space(10);
						if(GUILayout.Button(FeedbackLogo, GUILayout.Width(75.0f), GUILayout.Height(75.0f)))
							OpenFeedbackWindow();
						GUILayout.Space(10);
						using(new WellFired.Shared.GUIBeginVertical())
						{
							GUILayout.Space(10);
							GUILayout.Label("Feedback", SubHeading2Style, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Space(6);
							GUILayout.Label("Your feedback is very important to use. If you want a", NormalStyle, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.Label("new feature, you can request it here.", NormalStyle, GUILayout.MinWidth(position.width - distanceFromEdge));
							GUILayout.FlexibleSpace();
						}
						GUILayout.FlexibleSpace();
					}
				}
			
				GUILayout.FlexibleSpace();

				using(new WellFired.Shared.GUIBeginHorizontal())
				{
					bool show = EditorGUILayout.Toggle("Show Welcome Screen?", EditorPrefs.GetBool(WindowPref, true), GUILayout.MinWidth(minSize.x));
					EditorPrefs.SetBool(WindowPref, show);

					GUILayout.FlexibleSpace();

					GUILayout.Label(string.Format("V{0}", versionInformation.FullVersionString));
				}
			}
		}
		
		private bool LoadAssets()
		{
			// Already Loaded.
			if(DocumentationLogo && VideosLogo && FeedbackLogo && WebsiteLogo)
				return true;

            DocumentationLogo   = USEditorUtility.EditorResourcesLoad("IconDocumentation") as Texture;
            VideosLogo          = USEditorUtility.EditorResourcesLoad("IconVideos") as Texture;
			FeedbackLogo 		= USEditorUtility.EditorResourcesLoad("IconFeedback") as Texture;
			CommunityLogo 		= USEditorUtility.EditorResourcesLoad("IconCommunity") as Texture;
			WebsiteLogo 		= USEditorUtility.EditorResourcesLoad("IconWebsite") as Texture;
			BlogLogo			= USEditorUtility.EditorResourcesLoad("IconBlog") as Texture;
			TwitterLogo			= USEditorUtility.EditorResourcesLoad("IconTwitter") as Texture;
			FacebookLogo		= USEditorUtility.EditorResourcesLoad("IconFacebook") as Texture;
			GoogleLogo			= USEditorUtility.EditorResourcesLoad("IconGoogle") as Texture;
			YoutubeLogo			= USEditorUtility.EditorResourcesLoad("IconYoutube") as Texture;

			var Skin = "ProSkin";
			if(!EditorGUIUtility.isProSkin)
				Skin = "FreeSkin";
			LoadedSkin = USEditorUtility.EditorResourcesLoad(Skin) as GUISkin;

			if(!DocumentationLogo || !VideosLogo || !FeedbackLogo || !WebsiteLogo || !LoadedSkin || !TwitterLogo || !FacebookLogo || !GoogleLogo || ! YoutubeLogo)
				return false;
			
			Heading1Style = LoadedSkin.GetStyle("Heading1");
			if(Heading1Style == null)
				return false;
			
			SubHeading1Style = LoadedSkin.GetStyle("SubHeading1");
			if(SubHeading1Style == null)
				return false;
			
			SubHeading2Style = LoadedSkin.GetStyle("SubHeading2");
			if(SubHeading2Style == null)
				return false;
			
			NormalStyle = LoadedSkin.GetStyle("Normal");
			if(NormalStyle == null)
				return false;
			
			return true;
		}
		
		private void OpenDocumentationWindow()
		{
			Application.OpenURL(DocumentationURL);
		}
		
		private void OpenCommunityWindow()
		{
			Application.OpenURL("http://www.wellfired.com/blog/forum");
		}

		private void OpenBlogWindow()
		{
			Application.OpenURL("http://www.wellfired.com/blog");
		}
		
		private void OpenFeedbackWindow()
		{
			Application.OpenURL("http://usequencer.uservoice.com/");
		}
		
		private void OpenWebsiteWindow()
		{
			Application.OpenURL("http://www.wellfired.com/usequencer.html");
		}

		private void OpenTwitterWindow()
		{
			Application.OpenURL("https://www.twitter.com/wellfired");
		}

		private void OpenFacebookWindow()
		{
			Application.OpenURL("https://www.facebook.com/wellfired");
		}

		private void OpenGoogleWindow()
		{
			Application.OpenURL("https://plus.google.com/+WellfiredDevelopment/posts");
		}

		private void OpenYoutubeWindow()
		{
			Application.OpenURL("https://www.youtube.com/channel/UC94MXAjuJT-dSfS03wrZW9Q");
		}
	}
}
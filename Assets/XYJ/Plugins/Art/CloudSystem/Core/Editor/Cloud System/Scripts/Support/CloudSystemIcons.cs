//
// CloudSystemIcons.cs: Static access to all icons.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

namespace Edelweiss.CloudSystemEditor {

	public class CloudSystemIcons {
		
		private const string c_IconPath = "Assets/Art/ThirdPartyPlugins/CloudSystem/Core/Editor/Cloud System/Res/";
		
		private const string c_CloudIconName = "cloud.png";
		private const string c_ShapeIconName = "shape.png";
		private const string c_ParticleTypeIconName = "particleType.png";
		private const string c_ShadingIconName = "shading.png";
		private const string c_VerticalColorIconName = "verticalColor.png";
		private const string c_ParticleIconName = "particle.png";
		private const string c_SettingIconName = "setting.png";
		
		private static Texture2D m_CloudIcon;
		private static Texture2D m_ShapeIcon;
		private static Texture2D m_ParticleTypeIcon;
		private static Texture2D m_ShadingIcon;
		private static Texture2D m_VerticalColorIcon;
		private static Texture2D m_ParticleIcon;
		private static Texture2D m_SettingIcon;
		
		public static Texture2D CloudIcon {
			get {
				LoadIconsIfNeeded ();
				return (m_CloudIcon);
			}
		}
		public static Texture2D ShapeIcon {
			get {
				LoadIconsIfNeeded ();
				return (m_ShapeIcon);
			}
		}
		public static Texture2D ParticleTypeIcon {
			get {
				LoadIconsIfNeeded ();
				return (m_ParticleTypeIcon);
			}
		}
		public static Texture2D ShadingIcon {
			get {
				LoadIconsIfNeeded ();
				return (m_ShadingIcon);
			}
		}
		public static Texture2D VerticalColorIcon {
			get {
				LoadIconsIfNeeded ();
				return (m_VerticalColorIcon);
			}
		}
		public static Texture2D ParticleIcon {
			get {
				LoadIconsIfNeeded ();
				return (m_ParticleIcon);
			}
		}
		public static Texture2D SettingIcon {
			get {
				LoadIconsIfNeeded ();
				return (m_SettingIcon);
			}
		}
		
		private static void LoadIconsIfNeeded () {
			if (!AreIconsLoaded ()) {
				LoadIcons ();
			}
		}
		
		private static bool AreIconsLoaded () {
			bool l_Result = (m_CloudIcon != null);
			return (l_Result);
		}
		
		private static void LoadIcons () {
			m_CloudIcon = IconLoader.LoadIcon (c_IconPath, c_CloudIconName);
			m_ShapeIcon = IconLoader.LoadIcon (c_IconPath, c_ShapeIconName);
			m_ParticleTypeIcon = IconLoader.LoadIcon (c_IconPath, c_ParticleTypeIconName);
			m_ShadingIcon = IconLoader.LoadIcon (c_IconPath, c_ShadingIconName);
			m_VerticalColorIcon = IconLoader.LoadIcon (c_IconPath, c_VerticalColorIconName);
			m_ParticleIcon = IconLoader.LoadIcon (c_IconPath, c_ParticleIconName);
			m_SettingIcon = IconLoader.LoadIcon (c_IconPath, c_SettingIconName);
		}
		
		private static void DestroyIcons () {
			if (IconLoader.IsIconFromAssembly (c_CloudIconName)) {
				Object.DestroyImmediate (CloudIcon);
			}
			if (IconLoader.IsIconFromAssembly (c_ShapeIconName)) {
				Object.DestroyImmediate (ShapeIcon);
			}
			if (IconLoader.IsIconFromAssembly (c_ParticleTypeIconName)) {
				Object.DestroyImmediate (ParticleTypeIcon);
			}
			if (IconLoader.IsIconFromAssembly (c_ShapeIconName)) {
				Object.DestroyImmediate (ShapeIcon);
			}
			if (IconLoader.IsIconFromAssembly (c_VerticalColorIconName)) {
				Object.DestroyImmediate (VerticalColorIcon);
			}
			if (IconLoader.IsIconFromAssembly (c_ParticleIconName)) {
				Object.DestroyImmediate (ParticleIcon);
			}
			if (IconLoader.IsIconFromAssembly (c_SettingIconName)) {
				Object.DestroyImmediate (SettingIcon);
			}
		}
	}
}
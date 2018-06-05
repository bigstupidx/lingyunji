using System.Collections;
using System.IO;
using UnityEngine;

namespace WellFired.Shared
{
	public static class VersionInformationUtilities
	{
		private static readonly string ANCHOR_FILE = "WellFired.Project.Anchor.File";
		private static readonly string VERSION_PATH = "/version.txt";
		private static string installationDirectory = string.Empty;

		public static VersionInformation GetVersionInformationForProduct(string productName)
		{
			DumpAnchorFileIfNeeded();

#if WELLFIRED_INTERNAL
			var versionFileInfo = string.Format("{0}{1}{2}/Installer Data{3}", DirectoryExtensions.ProjectPath, DirectoryExtensions.DirectoryOf(ANCHOR_FILE), productName, VERSION_PATH);
#else
			var versionFileInfo = string.Format("{0}{1}{2}{3}", DirectoryExtensions.ProjectPath, DirectoryExtensions.DirectoryOf(ANCHOR_FILE), productName, VERSION_PATH);
#endif

			if(!File.Exists(versionFileInfo))
			{
				Debug.LogWarning("Cannot find version file for product " + productName);
				return VersionInformation.InvalidVersion();
			}

			return VersionInformation.BuildFrom(File.ReadAllText(versionFileInfo));
		}

		private static void DumpAnchorFileIfNeeded()
		{
			installationDirectory = DirectoryExtensions.DirectoryOf(ANCHOR_FILE);
			if(installationDirectory != string.Empty)
			{
				installationDirectory = string.Format("{0}{1}", (new DirectoryInfo(DirectoryExtensions.ProjectPath)), installationDirectory);
				return;
			}
			
			var baseDirectory = (new DirectoryInfo(DirectoryExtensions.ProjectPath + "/Assets")).FullName;
			var wellFiredDirectories = Directory.GetDirectories(baseDirectory, "WellFired", SearchOption.AllDirectories);
			if(wellFiredDirectories.Length != 0)
				installationDirectory = wellFiredDirectories[0];
			
			if(installationDirectory == string.Empty)
			{
				installationDirectory = DirectoryExtensions.DefaultInstallationDirectory;
				Directory.CreateDirectory(installationDirectory);
			}
			
 			DumpAnchorFile();
		}
		
		private static void DumpAnchorFile()
		{
			var anchorFile = string.Format("{0}/{1}", installationDirectory, ANCHOR_FILE);
			if(File.Exists(anchorFile))
				return;
			
			File.WriteAllBytes(anchorFile, new byte[] {});
		}
	}
}
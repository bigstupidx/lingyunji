using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using xys.battle;

namespace behaviac
{
    class BehaviacWorkspace
    {
        static public void Behavic_Init(string path)
        {
            behaviac.Workspace.Instance.FilePath = path;
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;

#if !COM_SERVER
            BehaviacFileManager.Create();
#endif
        }

        static public void Behavic_Update()
        {
            behaviac.Workspace.Instance.DebugUpdate();
            behaviac.Workspace.Instance.TimeSinceStartup = BattleHelp.timePass;
        }

        static public void Behavic_Cleanup()
        {
            behaviac.Workspace.Instance.Cleanup();
        }
    }

#if !COM_SERVER
    public class BehaviacFileManager : behaviac.FileManager
    {
        static BehaviacFileManager ms_fileSystem = null;
        public static void Create()
        {
            ms_fileSystem = new BehaviacFileManager();
        }

        public override void FileClose(string filePath, string ext, byte[] fileHandle)
        {
            //base.FileClose(filePath, ext, fileHandle);
        }

        public override bool FileExist(string filePath, string ext)
        {
            return PackTool.TextLoad.has(filePath + ext);
        }

        public override byte[] FileOpen(string filePath, string ext)
        {
            int lenght = 0;
            byte[] bytes = null;
            PackTool.TextLoad.GetBytes(filePath + ext, ref bytes, out lenght);
            return bytes;
        }
    }
#endif

    //热更新
}

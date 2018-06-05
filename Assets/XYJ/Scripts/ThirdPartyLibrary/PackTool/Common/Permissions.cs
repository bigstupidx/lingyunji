using UnityEngine;
using System.IO;

namespace PackTool
{
    public class Permissions
    {
        // 是否拥有写入的权限
        static public bool hasWriteAccess
        {
            get
            {
                if (!Directory.Exists(ResourcesPath.LocalTempPath))
                {
                    Directory.CreateDirectory(ResourcesPath.LocalTempPath);
                }

                string testfile = ResourcesPath.LocalTempPath + "writeaccess"; // 测试是否拥有写入的权限
                try
                {
                    FileStream fs = new FileStream(testfile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    byte[] bytes = new byte[4];
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                    File.Delete(testfile);
                    return true;
                }
                catch(System.Exception )
                {
                    return false; // 没有写入的权限
                }
            }
        }
    }
}
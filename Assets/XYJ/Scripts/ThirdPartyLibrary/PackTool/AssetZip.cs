using System.IO;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;

namespace XTools
{
    public class AssetZip
    {
        public const string suffix = ".awb";
        public const string DefaultName = "Data.awb";
        public const string patchname = "patchs.awb";
        public const string extendedname = "extended.wp";
        public const string patchsuffix = ".wp"; // 补丁包的后缀名

#if !UNITY_IPHONE || UNITY_EDITOR 
        ZipFile mZipfile = null;

        // 释放资源
        public void Release()
        {
            if (mZipfile != null)
            {
                mZipfile.Close();
                mZipfile = null;
            }
        }

        public void Init(string filepath, string password = "")
        {
            try
            {
                mZipfile = new ZipFile(filepath);
                mZipfile.Password = password;
                InitFileList();
            }
            catch (System.Exception e)
            {
                Debuger.LogError("AssetZip.init error!" + e.Message + ",StackTrace:" + e.StackTrace);
                return;
            }
        }

        Dictionary<string, long> mZipEntrys = new Dictionary<string, long>();

        // 初始化文件列表
        void InitFileList()
        {
            // zip包当中的所有文件列表
            IEnumerator itor = mZipfile.GetEnumerator();
            while (itor.MoveNext())
            {
                ZipEntry entry = itor.Current as ZipEntry;
                if (entry.IsFile)
                    mZipEntrys.Add(entry.Name.ToLower(), entry.ZipFileIndex);
            }
        }

        public Stream FindFileStream(string file)
        {
            long entryIndex = -1;
            if (!mZipEntrys.TryGetValue(file.ToLower(), out entryIndex))
            {
                Debuger.LogError(string.Format("file: {0} not find!", file));
                return null;
            }

            return mZipfile.GetInputStream(entryIndex);
        }

        public void EachAllFile(System.Action<string> fun)
        {
            foreach (KeyValuePair<string, long> itor in mZipEntrys)
                fun(itor.Key);
        }
#endif
    }
}
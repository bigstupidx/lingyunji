#if USE_RESOURCESEXPORT || USE_ABL
using System.IO;
using System.Collections.Generic;

namespace PackTool
{
    // 资源包，内部包+外部的补丁合并包,外部包优先于内部包
    internal class ResUnite : IPackRead
    {
        public ResUnite(List<Archive> zips)
        {
            Init(zips);
        }

        List<Archive> mZips = null; // zip文件

        internal class Res
        {
            public Archive zip;
            public Archive.FileItem entry;

            public int filesize { get { return (int)entry.size; } }

            public int crc32 { get { return (int)entry.crc32; } }

            public Res(Archive z, Archive.FileItem e)
            {
                zip = z;
                entry = e;
            }

            public void Reset(Archive z, Archive.FileItem e)
            {
                zip = z;
                entry = e;
            }

            public Archive.PartialInputStream GetStream()
            {
                return entry.GetStream(zip);
            }
        }

        Dictionary<string, Res> mResList;

        Res GetRes(string filename)
        {
            Res res = null;
            if (mResList.TryGetValue(filename, out res))
                return res;

            return null;
        }
        
        public Dictionary<string, Res> GetFileList()
        {
            return mResList;
        }

        public readonly static string fullpath = ResourcesPath.LocalTempPath + "unwz/";

        // 初始化
        public void Init(List<Archive> zips)
        {
            mZips = zips;
            Res res = null;
            string filename;
            string suffix = "Data/";

            int total = 0;
            for (int i = 0; i < mZips.Count; ++i)
                total += mZips[i].fileItems.Length;

            mResList = new Dictionary<string, Res>(total);
            Archive zip = null;
            for (int m = 0; m < mZips.Count; ++m)
            {
                zip = mZips[m];
                for (int i = 0; i < zip.fileItems.Length; ++i)
                {
                    filename = zip.fileItems[i].name;
                    ResourcesGroup.AddFile(filename);

                    if (filename.StartsWith(suffix))
                    {
                        filename = filename.ToLower();
                    }

                    if (!mResList.TryGetValue(filename, out res))
                    {
                        res = new Res(zip, zip.fileItems[i]);
                        mResList.Add(filename, res);

                        StringHashCode.Add(filename);
                    }
                    else
                    {
                        res.Reset(zip, zip.fileItems[i]);
                    }
                }

                zip.Release();
            }
        }

        Stream IPackRead.FindBaseStream(string file, out int lenght)
        {
            return FindBaseStream(GetRes(file), out lenght);
        }

        string IPackRead.Find(string file, out int offset, out int lenght)
        {
            offset = 0;
            lenght = 0;
            Res res = GetRes(file);
            if (res == null)
                return null;

            lenght = res.entry.size;
            offset = res.zip.data_offset + res.entry.offset;
            return res.zip.data_file;
        }

        Stream FindBaseStream(Res res, out int lenght)
        {
            if (res != null)
            {
                lenght = res.filesize;
                return res.GetStream();
            }

            lenght = 0;
            return null;
        }

        // 是否存在此文件
        bool IPackRead.IsExistFile(string file)
        {
            return mResList.ContainsKey(file);
        }

        void IPackRead.Release()
        {
            if (mResList != null)
                mResList.Clear();

            if (mZips != null)
            {
                foreach (Archive zip in mZips)
                    zip.Close();

                mZips = null;
            }
        }

        void IPackRead.EachAllFile(System.Action<string> files)
        {
            foreach (KeyValuePair<string, Res> itor in mResList)
            {
                files(itor.Value.entry.name);
            }
        }
    }
}
#endif
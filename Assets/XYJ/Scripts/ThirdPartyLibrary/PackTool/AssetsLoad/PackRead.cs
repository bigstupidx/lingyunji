#if USE_RESOURCESEXPORT || USE_ABL
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Core;

namespace PackTool
{
    // 资源包的读取
    internal interface IPackRead
    {
        Stream FindBaseStream(string file, out int lenght);
        string Find(string file, out int offset, out int lenght);

        void Release();

        // 是否存在此文件
        bool IsExistFile(string file);

        // 遍历所有的文件
        void EachAllFile(System.Action<string> files);
    }

    class EmptyPackRead : IPackRead
    {
        Stream IPackRead.FindBaseStream(string file, out int lenght)
        {
            lenght = 0;
            return null;
        }

        string IPackRead.Find(string file, out int offset, out int lenght)
        {
            offset = 0;
            lenght = 0;
            return null;
        }

        void IPackRead.Release() { }

        // 是否存在此文件
        bool IPackRead.IsExistFile(string file) { return false; }
        
        // 遍历所有的文件
        void IPackRead.EachAllFile(System.Action<string> files) { }
    }

    // 目录资源读取
    internal class DirectoryPackRead : IPackRead
    {
        List<string> AllFiles = new List<string>();

        public DirectoryPackRead(string root)
        {
            lenght = root.Length;
            root.CopyTo(0, bufs, 0, root.Length);
            Debuger.Log("DirectoryPackRead:" + root);
            FileSystemScanner scanner = new FileSystemScanner("", "");
            string filepath;
            scanner.ProcessFile = 
                (object sender, ScanEventArgs e) => 
                {
                    filepath = e.Name.Substring(lenght).Replace('\\', '/');
                    AllFiles.Add(filepath);
                    ResourcesGroup.AddFile(filepath);
                    StringHashCode.Add(filepath);
                };

            scanner.Scan(root, true);
        }

        char[] bufs = new char[512];
        int lenght = 0;

        string GetDstFile(string file)
        {
            file.CopyTo(0, bufs, lenght, file.Length);
            return new string(bufs, 0, lenght + file.Length);
        }

        public Stream FindBaseStream(string file, out int lenght)
        {
            string dstfile = GetDstFile(file);
            if (File.Exists(dstfile))
            {
                FileStream fs = File.Open(dstfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                lenght = (int)fs.Length;
                return fs;
            }
            
            lenght = 0;
            return null;
        }

        // 查找文件
        // file 查找的文件名，返回值为真实的文件名
        // 此文件在文件包当中的偏移与长度
        public string Find(string file, out int offset, out int lenght)
        {
            lenght = 0;
            offset = 0;
            string dstfile = GetDstFile(file);
            if (File.Exists(dstfile))
            {
                FileStream fs = File.Open(dstfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                lenght = (int)fs.Length;
                return dstfile;
            }

            return null;
        }

        // 是否存在此文件
        public bool IsExistFile(string file)
        {
            string dstfile = GetDstFile(file);
            return File.Exists(dstfile);
        }

        // 清除掉所有缓存的资源
        public void ClearCache()
        {

        }

        public void Release()
        {
            AllFiles.Clear();
            AllFiles = null;
        }

        // 遍历所有的文件
        public void EachAllFile(System.Action<string> files)
        {
            for (int i =  0; i < AllFiles.Count; ++i)
            {
                files(AllFiles[i]);
            }
        }
    }
}
#endif
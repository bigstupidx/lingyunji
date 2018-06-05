#if USE_RESOURCESEXPORT
using System;
using System.IO;
using XTools;

public partial class Archive
{
#if UNITY_EDITOR
    // 解压资源包内的目录到指定硬盘目录
    public void Unarchive(string src, string dst)
    {
        if (fileItems == null || fileItems.Length == 0)
        {
            Debuger.WarningLog("Unarchive({0},{1}) fileItems is Empty!", src, dst);
            return;
        }

        byte[] temp_buf = new byte[1024 * 1024];
        int total = 0;
        //lzma.lzma_init_alloc_root();
        FileItem fileitem = null;
        for (int i = 0; i < fileItems.Length; ++i)
        {
            fileitem = fileItems[i];
            if (fileitem.name.StartsWith(src))
            {
                // 可以开始拷贝
                string filepath = System.IO.Path.Combine(dst, fileitem.name.Substring(src.Length)).Replace('\\', '/');
                Directory.CreateDirectory(filepath.Substring(0, filepath.LastIndexOf('/')));

                FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                fs.SetLength(fileitem.size);
                Utility.CopyStream(fileitem.GetStream(this), fs, temp_buf);
                fs.Close();

                //if (Utility.isLzma(fileitem.name))
                //{
                //    lzma.DecodeFile(filepath, 0, filepath + ".lzma");
                //    File.Delete(filepath);
                //    File.Move(filepath + ".lzma", filepath);
                //}

                ++total;
            }
        }

        Debuger.DebugLog("Unarchive({0},{1}), total:{2}", src, dst, total);
    }
#endif
}
#endif
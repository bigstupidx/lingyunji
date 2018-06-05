#if USE_RESOURCESEXPORT || USE_ABL
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using PackTool;
using XTools;

// 只加密数据头
public partial class Archive
{
    public class FileItem
    {
        public string name; // 文件名
        public int crc32; // crc效验码
        public int offset; // 数据的偏移量
        public int size; // 文件大小

        internal static FileItem Create(BinaryReader reader)
        {
            FileItem item = new FileItem();
            item.crc32 = reader.ReadInt32();
            item.name = reader.ReadString();
            item.offset = reader.ReadInt32();
            item.size = reader.ReadInt32();
            //item.saveType = reader.ReadByte();
            return item;
        }

        public Archive.PartialInputStream GetStream(Archive archive)
        {
            return new Archive.PartialInputStream(archive.BaseStream, offset + archive.data_offset, size);
        }

        internal void Writer(BinaryWriter reader)
        {
            reader.Write(crc32);
            reader.Write(name);
            reader.Write(offset);
            reader.Write(size);
        }
    }

    Stream baseStream = null;
    int data_position = 0; // 数据区域

    public Stream BaseStream { get { return baseStream; } }

    public int data_offset { get { return data_position; } }

    public string data_file { get; protected set; }

    public void Close()
    {
        if (baseStream != null)
            baseStream.Close();
        baseStream = null;
        fileItems = null;
    }

    public void ReadInfo(System.Action<FileItem> once, System.Action<int> onheadend = null)
    {
        int startpos = (int)baseStream.Position;
        StreamOffset so = new StreamOffset(baseStream, 0, 8);
        Encrypt.TeaStream teaStream = new Encrypt.TeaStream(so);
        BinaryReader reader = new BinaryReader(teaStream);
        int headsize = reader.ReadInt32(); // 数据区偏移量
        data_position = startpos + headsize;
        int nums = reader.ReadInt32();
        so.Reset(8, headsize - 8);

        if (onheadend != null)
        {
            onheadend(nums);
        }

        for (int i = 0; i < nums; ++i)
        {
            once(FileItem.Create(reader));
        }

        reader.Close();
        teaStream.Close();
    }

    // 默认初始化
    public void InitDefault()
    {
        string versionfile = ResourceConfig.filename;

        fileItems = null;
        int index = 0;
        FileItem fi_version = null;
        ReadInfo(
            (FileItem item) =>
            {
                fileItems[index] = item;
                ++index;
                if (fi_version == null && item.name == versionfile)
                    fi_version = item;
            },

            (int num) => 
            {
                fileItems = new FileItem[num];
            });
        
        if (fi_version != null)
        {
            ResourceConfig rc = new ResourceConfig();
            StreamReader sr = new StreamReader(fi_version.GetStream(this));
            rc.Load(sr);
            sr.Close();
            current_version = rc.version;
        }
        else
        {
            current_version = null;
        }
    }

    public System.Version current_version { get; protected set; }

    public FileItem[] fileItems { get; protected set; }

    public void Release()
    {
        fileItems = null;
    }

    public Archive(string file, int offset)
    {
        data_file = file;
        baseStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 512);
        baseStream.Seek(offset, SeekOrigin.Begin);
    }

    static int WriteFileItems(BinaryWriter writer, List<FileItem> fileItems)
    {
        int pos = (int)writer.BaseStream.Position;
        writer.Write((int)0);
        writer.Write(fileItems.Count); // 头部偏移量
        for (int i = 0; i < fileItems.Count; ++i)
        {
            fileItems[i].crc32 = (int)writer.BaseStream.Position;
            fileItems[i].Writer(writer);
        }

        int endpos = (int)writer.BaseStream.Position;

        writer.BaseStream.Position = pos;
        writer.Write(endpos - pos);
        writer.BaseStream.Position = endpos;

        return endpos - pos;
    }

    public static void Generate(string file, Dictionary<string, Stream> items, byte[] bytes)
    {
        List<Pair<string, Stream>> files = new List<Pair<string, Stream>>();
        foreach (KeyValuePair<string, Stream> itor in items)
            files.Add(new Pair<string, Stream>(itor.Key, itor.Value));

        Generate(file, files, bytes);
    }

    public static void Generate(string file, List<Pair<string, Stream>> items, byte[] bytes)
    {
        if (File.Exists(file))
            File.Delete(file);
        else
            Directory.CreateDirectory(file.Substring(0, file.LastIndexOf('/')));

        // 生成文档
        List<FileItem> fileItems = new List<FileItem>(items.Count);
        {
            long offset = 0;
            for (int i = 0; i < items.Count; ++i)
            {
                FileItem item = new FileItem();
                item.name = items[i].first.Replace('\\', '/');
                item.crc32 = 0;
                item.offset = (int)offset;
                item.size = (int)items[i].second.Length;
                fileItems.Add(item);

                offset += items[i].second.Length;
            }
        }

        BinaryWriter writer = new BinaryWriter(File.Open(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite));
        int size = WriteFileItems(writer, fileItems); // 写入文件头信息
        Logger.LogDebug("包头大小:{0}", Utility.ToMb(size));

        // 再写入所有的流数据
        for (int i = 0; i < items.Count; ++i)
        {
            WriteCrc32AndData(writer, items[i].second, fileItems[i], bytes);
        }
        
        // 加密消息头
        Encrypt.Tea.EncryptStream(writer.BaseStream, 0, size, bytes);

        // 写入完成
        writer.BaseStream.Close();
        writer.Close();
    }

    static void WriteCrc32AndData(BinaryWriter writer, Stream stream, FileItem fi, byte[] bytes)
    {
        Crc32 crc32 = new Crc32();
        int readLenght = 0;
        while ((readLenght = stream.Read(bytes, 0, bytes.Length)) > 0)
        {
            crc32.Update(bytes, 0, readLenght);
            writer.Write(bytes, 0, readLenght);
        }
        writer.Flush();
        int endPos = (int)(writer.BaseStream.Position);
        writer.Seek(fi.crc32, SeekOrigin.Begin);
        fi.crc32 = (int)crc32.Value;
        writer.Write(fi.crc32);

        writer.Seek(endPos, SeekOrigin.Begin);

        stream.Close();
    }

#if USE_RESOURCESEXPORT || USE_ABL
    static public void AssetPackArchive(string srcpath, string dstfile, string prefix)
    {
        if (srcpath[srcpath.Length - 1] == '/' || srcpath[srcpath.Length - 1] == '\\')
            srcpath = srcpath.Substring(0, srcpath.Length - 1);

        srcpath = srcpath.Replace('\\', '/');

        if (string.IsNullOrEmpty(dstfile))
            dstfile = srcpath + AssetZip.suffix;

        dstfile = dstfile.Replace('\\', '/');
        if (File.Exists(dstfile))
            File.Delete(dstfile);
        else
        {
            Directory.CreateDirectory(dstfile.Substring(0, dstfile.LastIndexOfAny(new char[] { '\\', '/' })));
        }

        //string title = string.Format("正在压缩文件目录:{0}", srcpath);
        string[] allfiles = System.IO.Directory.GetFiles(srcpath, "*", SearchOption.AllDirectories);
        if (prefix == null)
            prefix = srcpath.Substring(srcpath.LastIndexOf('/') + 1);

        int srcindex = srcpath.Length;
        TimeCheck tc = new TimeCheck(true);
        string name;
        List<Pair<string, Stream>> itemfiles = new List<Pair<string, Stream>>();
        int totalsize = 0;
        foreach (string file in allfiles)
        {
            if (!string.IsNullOrEmpty(prefix))
                name = prefix + "/" + file.Substring(srcindex + 1);
            else
                name = file.Substring(srcindex + 1);

            name = name.Replace('\\', '/');

            // 不压缩
            itemfiles.Add(new Pair<string, Stream>(name, File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)));
        }

        Archive.Generate(dstfile, itemfiles, new byte[1024 * 1024]);
        Debuger.DebugLog("生成文件包:{0}用时:{1} md5:{2} {3} size",
            Path.GetFileName(dstfile),
            tc.delay,
            Md5.GetFileMd5(dstfile),
            Md5.GetFileSize(dstfile));

        Utility.DeleteFolder(ResourcesPath.LocalBasePath + "LzmaTemp");
    }
#endif
}
#endif
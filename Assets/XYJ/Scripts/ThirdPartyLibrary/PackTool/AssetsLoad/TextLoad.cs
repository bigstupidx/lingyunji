using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class TextLoad
    {
        static string RootPath { get; set; }

        public static void Init()
        {
            RootPath = Application.dataPath + "/../";
        }

#if UNITY_EDITOR
        static string GetCopyFile(string file)
        {
            int index = file.LastIndexOf('.');
            if (index <= -1)
                return file + "__copy__";

            return file.Insert(index, "__copy__");
        }

        static bool HasFileByEditor(string key)
        {
            if (string.IsNullOrEmpty(RootPath))
                Init();
            string filepath = RootPath + key;
            return File.Exists(filepath);
        }

        static Stream GetByEditor(string key)
        {
            if (string.IsNullOrEmpty(RootPath))
                Init();
            string filepath = RootPath + key;
            if (!System.IO.File.Exists(filepath))
            {
                return null;
            }

            try
            {
                return new MemoryStream(System.IO.File.ReadAllBytes(filepath));
            }
            catch (System.Exception)
            {

            }

            string copyFilePath;
            string copyFileName = key;
            do
            {
                copyFileName = GetCopyFile(copyFileName);
                copyFilePath = RootPath + copyFileName;
            } while (File.Exists(copyFilePath));

            File.Copy(filepath, copyFilePath);
            Stream stream = GetTextByPath(copyFileName);
            File.Delete(copyFilePath);
            return stream;
        }
#endif
        static Stream GetTextByPath(string key)
        {
#if UNITY_EDITOR
            return GetByEditor(key);
#else
    #if UNITY_IPHONE
            string dst = StreamingAssetLoad.FindFile(key);
            return StreamingAssetLoad.GetFile(string.IsNullOrEmpty(dst) ? key : dst);
    #else
            return StreamingAssetLoad.GetFile(key);
    #endif
#endif
        }

        static byte[] temp_bytes = null;

        public static byte[] GetBytes(string key, out int lenght)
        {
            if (GetBytes(key, ref temp_bytes, out lenght))
                return temp_bytes;

            return null;
        }

        public static string GetString(string key)
        {
            Stream s = GetStream(key);
            if (s == null)
                return "";
            
            using (StreamReader streamReader = new StreamReader(s, System.Text.Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static bool GetBytes(string key, ref byte[] bytes, out int lenght)
        {
            lenght = 0;
            Stream s = GetStream(key);
            if (s == null)
                return false;

            lenght = (int)s.Length;
            if (bytes == null || bytes.Length < lenght)
                bytes = new byte[lenght];
            else
            {
                System.Array.Clear(temp_bytes, 0, temp_bytes.Length);
            }

            s.Read(bytes, 0, lenght);
            return true;
        }

#if UNITY_EDITOR
        public static string GetEditorPath()
        {
            if (string.IsNullOrEmpty(RootPath))
                Init();
            return RootPath;
        }
#endif

        public static Stream GetStream(string key)
        {
#if USE_RESOURCESEXPORT || USE_ABL
            return ResourcesPack.FindBaseStream(key.ToLower());
#else
            return GetTextByPath(key);
#endif
        }

        public static bool has(string key)
        {
#if USE_RESOURCESEXPORT || USE_ABL
            return ResourcesPack.IsExistFile(key.ToLower());
#elif UNITY_EDITOR
            return HasFileByEditor(key);
#else
            return StreamingAssetLoad.FileExist(key);      
#endif
        }
    }
}
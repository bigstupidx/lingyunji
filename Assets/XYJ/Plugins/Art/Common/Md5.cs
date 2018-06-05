using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

public class Md5
{
#if UNITY_EDITOR
    [MenuItem("PackTool/Md5File", false, 9)]
    [MenuItem("Assets/PackTool/Md5File", false, 0)]
    static void CaclFileMd5Value()
    {
        string fn = EditorUtility.OpenFilePanel("读取文件", "", "");
        if (string.IsNullOrEmpty(fn))
            return;

        Debug.Log(string.Format("file:{0} md5:{1} size:{2}", fn, Md5.GetFileMd5(fn), GetFileSize(fn)));
    }

#endif

    static MD5 MD5 = null;

    public static int GetFileSize(string file)
    {
        if (!File.Exists(file))
            return 0;

        FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        int lenght = (int)fileStream.Length;
        fileStream.Close();
        return lenght;
    }

    public static string GetFileMd5(string file)
    {
        if (!File.Exists(file))
            return null;

        if (MD5 == null)
            MD5 = MD5.Create();

        FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        byte[] values = MD5.ComputeHash(fileStream);
        fileStream.Close();
        return ToMd5(values);
    }

    static string ToMd5(byte[] md5)
    {
        string OutString = "";
        for (int i = 0; i < md5.Length; i++)
        {
            OutString += md5[i].ToString("x2");
        }

        return OutString;
    }

	public static string MD5Encode(string encodeTxt)
	{
		if(string.IsNullOrEmpty(encodeTxt)) 
            return string.Empty;

        MD5 md5 = MD5.Create();
		byte[] encodeData = System.Text.Encoding.Default.GetBytes(encodeTxt);
		byte[] md5Data = md5.ComputeHash(encodeData);
        md5.Clear();
        md5 = null;

		return ToMd5(md5Data);
	}
}
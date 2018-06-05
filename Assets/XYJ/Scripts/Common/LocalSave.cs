using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
/// <summary>
/// ********注意事项*******
/// 慎重使用PlayerPrefs.DeleteAll()函数，会被所有记录都清空
/// 如需要保存复杂类型的数据可自行组织数据格式，可参考ChatLocalSave
/// </summary>
public class LocalSave
{
    #region Field
    // 密钥
    private const string PRIVATE_KEY = "9ETrEsWaFRach3gexaDr";

    // 加密前先给这个数组添加数值,用于随机加密
    private static string[] keys;

    #endregion

    #region Local Function

    private static void InitSaveKeys()
    {
        int keycount = 50;
        keys = new string[keycount];
        int startnum = 7;
        int addrate = 7;
        for(int i = 0 ; i < keycount ; i++)
        {
            keys[i] = startnum.ToString();
            startnum += addrate++;
        }
    }
    private static string Md5(string strToEncrypt)
    {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        string hashString = "";

        for(int i = 0 ; i < hashBytes.Length ; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
    private static void SaveEncryption(string key, string type, string value)
    {
        if(keys == null)
        {
            InitSaveKeys();
        }
        int keyIndex = (int)Mathf.Floor(Random.value * keys.Length);
        string secretKey = keys[keyIndex];
        string check = Md5(string.Format("{0}_{1}_{2}_{3}",type,PRIVATE_KEY,secretKey,value));
        PlayerPrefs.SetString(key + "_encryption_check", check);
        PlayerPrefs.SetInt(key + "_used_key", keyIndex);
    }

    private static bool CheckEncryption(string key, string type, string value)
    {
        if(keys == null)
        {
            InitSaveKeys();
        }
        int keyIndex = PlayerPrefs.GetInt(key + "_used_key");
        string secretKey = keys[keyIndex];
        string check = Md5(string.Format("{0}_{1}_{2}_{3}", type, PRIVATE_KEY, secretKey, value));
        if(!PlayerPrefs.HasKey(key + "_encryption_check"))
            return false;
        string storedCheck = PlayerPrefs.GetString(key + "_encryption_check");
        return storedCheck == check;
    }

    #endregion

    #region Interface

    #region Set

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        SaveEncryption(key, "int", value.ToString());
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        SaveEncryption(key, "float", Mathf.Floor(value * 1000).ToString());
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        SaveEncryption(key, "string", value);
    }
    public static void SetBool(string key, bool value)
    {
        int val = value ? 1 : 0;
        PlayerPrefs.SetInt(key, val);
        SaveEncryption(key, "int", val.ToString());
    }

    #endregion

    #region Get

    public static int GetInt(string key, int defaultValue = 0)
    {
        int value = PlayerPrefs.GetInt(key);
        if(!CheckEncryption(key, "int", value.ToString()))
            return defaultValue;
        return value;
    }

    public static float GetFloat(string key, float defaultValue = 0f)
    {
        float value = PlayerPrefs.GetFloat(key);
        if(!CheckEncryption(key, "float", Mathf.Floor(value * 1000).ToString()))
            return defaultValue;
        return value;
    }

    public static string GetString(string key, string defaultValue = "")
    {
        string value = PlayerPrefs.GetString(key);
        if(!CheckEncryption(key, "string", value))
            return defaultValue;
        return value;
    }
    public static bool GetBool(string key, bool defaultValue = false)
    {
        int value = PlayerPrefs.GetInt(key);
        if(!CheckEncryption(key, "int", value.ToString()))
            return defaultValue;
        return value == 1;
    } 
    #endregion

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey(key + "_encryption_check");
        PlayerPrefs.DeleteKey(key + "_used_key");
    }
    /// <summary>
    /// 清空所有记录，慎用
    /// </summary>
    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion

}

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class IniFile
{
    private class KeyPair
    {
        public string key;
        public string value;
        public string comment;

        public KeyPair(string key, string value, string comment)
        {
            this.key = key;
            this.value = value;
            this.comment = comment;
        }
    }

    private Dictionary<string, KeyPair> mKeysMap;
    private List<KeyPair> mKeysList;

    public IniFile()
    {
        Init();
    }

    public IniFile(string file)
    {
        Init();
        load(file);
    }

    public IniFile(Stream stream)
    {
        Init();
        load(stream);
    }

    private void Init()
    {
        mKeysMap = new Dictionary<string, KeyPair>();
        mKeysList = new List<KeyPair>();
    }

    public bool HasKey(string key)
    {
        if (mKeysMap.ContainsKey(key))
            return true;

        return false;
    }

    public void Set(string key, int value)
    {
        Set(key, value, "");
    }

    public void Set(string key, int value, string comment)
    {
        Set(key, value.ToString(), comment);
    }

    public void Set(string key, float value)
    {
        Set(key, value, "");
    }

    public void Set(string key, float value, string comment)
    {
        Set(key, value.ToString(), comment);
    }

    public void Set(string key, double value)
    {
        Set(key, value, "");
    }

    public void Set(string key, double value, string comment)
    {
        Set(key, value.ToString(), comment);
    }

    public void Set(string key, bool value)
    {
        Set(key, value, "");
    }

    public void Set(string key, bool value, string comment)
    {
        Set(key, value.ToString(), comment);
    }

    public void Set(string key, string value)
    {
        Set(key, value, "");
    }

    public void Set(string key, string value, string comment)
    {
        KeyPair outKeyPair = null;

        if (mKeysMap.TryGetValue(key, out outKeyPair))
        {
            outKeyPair.value = value;
            outKeyPair.comment = comment;

            return;
        }

        outKeyPair = new KeyPair(key, value, comment);

        mKeysMap.Add(key, outKeyPair);
        mKeysList.Add(outKeyPair);
    }

    public int Get(string key, int defaultValue)
    {
        string value = Get(key);

        try
        {
            return Convert.ToInt32(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public float Get(string key, float defaultValue)
    {
        string value = Get(key);

        try
        {
            return Convert.ToSingle(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public double Get(string key, double defaultValue)
    {
        string value = Get(key);

        try
        {
            return Convert.ToDouble(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public bool Get(string key, bool defaultValue)
    {
        string value = Get(key);

        try
        {
            return Convert.ToBoolean(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public string Get(string key)
    {
        return Get(key, "");
    }

    public string Get(string key, string defaultValue)
    {
        KeyPair outKeyPair = null;

        if (mKeysMap.TryGetValue(key, out outKeyPair))
        {
            return outKeyPair.value;
        }

        return defaultValue;
    }

    public void Remove(string key)
    {
        KeyPair outKeyPair = null;

        if (mKeysMap.TryGetValue(key, out outKeyPair))
        {
            mKeysList.Remove(outKeyPair);
            mKeysMap.Remove(key);
        }
    }

    public void RenameKey(string key, string newKey)
    {
        if (key.Equals(newKey))
        {
            return;
        }

        KeyPair outKeyPair = null;

        if (mKeysMap.TryGetValue(key, out outKeyPair))
        {
            outKeyPair.key = newKey;

            mKeysMap.Add(newKey, outKeyPair);
            mKeysMap.Remove(key);
        }
    }

    public void Save(Stream stream)
    {
        try
        {
            StreamWriter writer = new StreamWriter(stream);
            for (int i = 0; i < mKeysList.Count; ++i)
            {
                if (!mKeysList[i].comment.Equals(""))
                {
                    writer.WriteLine("; " + mKeysList[i].comment);
                }

                writer.WriteLine(mKeysList[i].key + "=" + mKeysList[i].value);
            }

            writer.Flush();
        }
        catch (IOException e)
        {
            Debuger.Log("Impossible to save file2");
            Debuger.LogWarning(e);
        }

    }

    public void Save(string fileName)
    {
        fileName = fileName.Replace("\\", "/");
        int pos = fileName.LastIndexOf('/');
        if (pos != -1)
            Directory.CreateDirectory(fileName.Substring(0, pos));

        try
        {
            StreamWriter stream = new StreamWriter(fileName);
            Save(stream.BaseStream);
            stream.BaseStream.Close();
            stream.Close();
        }
        catch (IOException e)
        {
            Debuger.Log("Impossible to save file: " + fileName + ".ini");
            Debuger.LogWarning(e);
        }
    }

    public void load(Stream baseStream)
    {
        StreamReader stream = new StreamReader(baseStream);

        mKeysMap.Clear();
        mKeysList.Clear();

        string line = "";
        string currentComment = "";

        while ((line = stream.ReadLine()) != null)
        {
            if (line.StartsWith(";"))
            {
                currentComment = line.Substring(1).Trim();
            }
            else
            {
                int index = line.IndexOf("=");

                if (index > 0)
                {
                    Set(line.Substring(0, index), line.Substring(index + 1), currentComment);
                    currentComment = "";
                }
            }
        }

        baseStream.Close();
        stream.Close();
    }

    public bool load(string fileName)
    {
        if (File.Exists(fileName))
        {
            StreamReader stream = null;
            try
            {
                stream = new StreamReader(fileName);
                load(stream.BaseStream);
                stream.Close();
                return true;
            }
            catch (IOException e)
            {
                Debuger.Log("Impossible to open file: " + fileName + ".ini");
                Debuger.LogWarning(e);
            }
        }

        return false;
    }

    public string[] keys()
    {
        string[] res = new string[mKeysList.Count];

        for (int i = 0; i < mKeysList.Count; ++i)
        {
            res[i] = mKeysList[i].key;
        }

        return res;
    }

    public int count()
    {
        return mKeysList.Count;
    }
}

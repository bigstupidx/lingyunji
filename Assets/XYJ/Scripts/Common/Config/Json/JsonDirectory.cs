using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


namespace xys.json
{
    public class JsonDictionary<T>
    {
        // Json文件后缀
        public const string JsonFileSuffix = ".json";

        protected Dictionary<string, T> _jsonObjectDict = new Dictionary<string, T>();

        protected string m_fileDirectory = string.Empty;// 文件目录

        public JsonDictionary(string fileDir)
        {
            m_fileDirectory = fileDir;
            _jsonObjectDict = new Dictionary<string, T>();
        }

        public bool TryGet (string fileName, out T value)
        {
            if (_jsonObjectDict.ContainsKey(fileName))
            {
                value = _jsonObjectDict[fileName];
                return true;
            }
            else
            {
                string filePath = GetFilePath(fileName);
                string json = PackTool.TextLoad.GetString(filePath);
                if (string.IsNullOrEmpty(json))
                {
                    Debuger.LogError(string.Format("type={1}, 找不到配置文件：{0}", fileName, typeof(T)));
                    value = default(T);
                    return false;
                }

                try
                {
                    value = JsonUtility.FromJson<T>(json);
                    _jsonObjectDict.Add(fileName, value);
                    return true;
                }
                catch (System.Exception ex)
                {
                    Debuger.LogException(ex);
                    value = default(T);
                    return false;
                }
            }
        }

        public void Release ()
        {
            _jsonObjectDict.Clear();
        }

        /// <summary>
        /// 获取某文件的路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetFilePath(string fileName)
        {
            if (!fileName.EndsWith(JsonFileSuffix))
                fileName += JsonFileSuffix;
            if (m_fileDirectory.EndsWith("/"))
                return string.Format("{0}{1}", m_fileDirectory, fileName);
            else
                return string.Format("{0}/{1}", m_fileDirectory, fileName);
        }

    }

    /// <summary>
    /// 编辑器用
    /// 加载某目录下的所有Json文件，目录下所有文件应该为同一对象类型。
    /// 注意：需要注意是否给编辑工具使用
    /// 支持功能：
    /// 1. 编辑器下可支持读取，保存，删除，重名Json文件等操作
    /// 2. 运行时只能读取加载Json文件，并转为对象数据
    /// </summary>
    /// <typeparam name="ValueT"></typeparam>
    public class JsonDirectory<ValueT> where ValueT : IJsonFile, new()
    {
        // Json文件后缀
        public const string JsonFileSuffix = ".json";
        public const string JsonFilePattern = "*.json";

        // FileName, JsonFile
        protected Dictionary<string, JsonFile<ValueT>> _jsonFileDict = new Dictionary<string, JsonFile<ValueT>>();

        protected bool m_toolUsed = false;// 是否给编辑工具使用
        protected string m_fileDirectory = string.Empty;// 文件目录

        /// <summary>
        /// 加载某目录下的Json文件
        /// </summary>
        /// <param name="fileDir"></param>
        /// <param name="toolUsed">true:只在编辑工具下有效，不会管是否打包模式</param>
        public JsonDirectory(string fileDir, bool toolUsed = false)
        {
            m_fileDirectory = fileDir;
            m_toolUsed = toolUsed;
        }

        /// <summary>
        /// 获取文件目录
        /// </summary>
        /// <returns></returns>
        public string GetFileDirectory()
        {
            return m_fileDirectory;
        }

        /// <summary>
        /// 获取某文件的路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFilePath(string fileName)
        {
            if (!fileName.EndsWith(JsonFileSuffix))
                fileName += JsonFileSuffix;
            if (m_fileDirectory.EndsWith("/"))
                return string.Format("{0}{1}", m_fileDirectory, fileName);
            else
                return string.Format("{0}/{1}", m_fileDirectory, fileName);
        }

        public IEnumerator<string> GetKeysEnumerator()
        {
            return _jsonFileDict.Keys.GetEnumerator();
        }

        /// <summary>
        /// 获取目录文件列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetDirectoryFileList()
        {
            if (_jsonFileDict.Count == 0)
                return null;
            List<string> files = new List<string>();
            foreach (string name in _jsonFileDict.Keys)
            {
                files.Add(name);
            }
            return files;
        }

        /// <summary>
        /// 获取文件配置数量
        /// </summary>
        /// <returns></returns>
        public int GetDictCount()
        {
            return _jsonFileDict.Count;
        }

        /// <summary>
        /// 是否存在文件配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsFileExists(string fileName)
        {
            return _jsonFileDict.ContainsKey(fileName);
        }

        /// <summary>
        /// 获取文件配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Get(string fileName, out ValueT value)
        {
            if (IsFileExists(fileName))
            {
                value = _jsonFileDict[fileName].ObjectValue;
                return true;
            }
            else
            {
                value = new ValueT();
                return false;
            }
        }

        /// <summary>
        /// 跟Add方法相同
        /// </summary>
        /// <param name="value"></param>
        public void Set(string fileName, ValueT value)
        {
            if (IsFileExists(fileName))
            {
                _jsonFileDict[fileName].Write(value);
            }
            else
            {
                string filePath = GetFilePath(fileName);
                JsonFile<ValueT> jsonObj = new JsonFile<ValueT>(filePath);
                jsonObj.Write(value);
                _jsonFileDict.Add(fileName, jsonObj);
            }
        }

        /// <summary>
        /// 添加文件配置，并保存
        /// </summary>
        /// <param name="value"></param>
        public bool Add(string fileName, ValueT value)
        {
            if (IsFileExists(fileName))
            {
                // 已存在配置
                Debug.LogWarning("添加文件失败！Josn文件已存在：" + fileName);
                return false;
            }
            else
            {
                if (value == null)
                {
                    value = new ValueT();
                }
                value.SetKey(fileName);
                string filePath = GetFilePath(fileName);
                JsonFile<ValueT> jsonObj = new JsonFile<ValueT>(filePath);
                jsonObj.Write(value);
                _jsonFileDict.Add(fileName, jsonObj);
                return true;
            }
        }

        public void RemoveAll()
        {
            _jsonFileDict.Clear();
            DeleteAll();
        }

        /// <summary>
        /// 删除文件配置
        /// </summary>
        /// <param name="fileName"></param>
        public void Remove(string fileName)
        {
            if (IsFileExists(fileName))
            {
                if (_jsonFileDict[fileName].Delete())
                {
                    _jsonFileDict.Remove(fileName);
                }
            }
        }
        public void Remove(ValueT value)
        {
            string fileName = value.GetKey();
            Remove(fileName);
        }


        /// <summary>
        /// 加载目录下所有配置
        /// </summary>
        /// <returns></returns>
        public bool LoadAll()
        {
            _jsonFileDict.Clear();

            if (!Directory.Exists(GetFileDirectory()))
            {
                Directory.CreateDirectory(GetFileDirectory());
                return false;
            }

            string[] files = Directory.GetFiles(GetFileDirectory(), JsonFilePattern);
            for (int i = 0; i < files.Length; ++i)
            {
                JsonFile<ValueT> jsonObj = new JsonFile<ValueT>(files[i]);
                if (jsonObj.Read())
                {
                    string fileName = jsonObj.FileName;
                    if (IsFileExists(fileName))
                    {
                        Debuger.LogError(string.Format("目录={0}，获取类={1}存在相同名字的文件：{2}", GetFileDirectory(), typeof(ValueT), fileName));
                    }
                    else
                    {
                        _jsonFileDict.Add(fileName, jsonObj);
                    }
                }
            }
            Debug.LogWarning(string.Format("目录={0}，获取类={1}的Json配置数量：{2}", GetFileDirectory(), typeof(ValueT), GetDictCount()));
            return true;
        }

        /// <summary>
        /// 重载文件配置
        /// </summary>
        /// <param name="fileName"></param>
        public void Reload(string fileName)
        {
            if (IsFileExists(fileName))
            {
                _jsonFileDict[fileName].Read();
            }
            else
            {
                string filePath = GetFilePath(fileName);
                JsonFile<ValueT> jsonObj = new JsonFile<ValueT>(filePath);
                if (jsonObj.Read())
                {
                    _jsonFileDict.Add(fileName, jsonObj);
                }
            }
        }

        /// <summary>
        /// 重命名文件配置
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="newFileName"></param>
        public bool Rename(string sourceFileName, string newFileName)
        {
            if (IsFileExists(sourceFileName))
            {
                if (IsFileExists(newFileName))
                {
                    Debug.LogError(string.Format("Josn文件重命名失败！存在Json文件：{0}", newFileName));
                    return false;
                }
                JsonFile<ValueT> jsonObj = _jsonFileDict[sourceFileName];
                if (jsonObj.Move(GetFilePath(sourceFileName), GetFilePath(newFileName)))
                {
                    _jsonFileDict.Remove(sourceFileName);
                    _jsonFileDict.Add(newFileName, jsonObj);
                    return true;
                }
            }
            Debug.LogError(string.Format("Josn文件重命名失败！不存在Json文件：{0}!", sourceFileName));
            return false;
        }

#region Helper Methods

        /// <summary>
        /// 删除路径下所有Json文件
        /// </summary>
        /// <returns></returns>
        bool DeleteAll()
        {
            if (!Directory.Exists(GetFileDirectory()))
            {
                Debug.LogWarning("删除失败！不存在文件目录：" + GetFileDirectory());
                return false;
            }

            string[] files = Directory.GetFiles(GetFileDirectory(), JsonFilePattern);
            if (files == null || files.Length == 0)
            {
                Debug.LogWarning("该目录下没有可删除的json文件，目录：" + GetFileDirectory());
                return false;
            }
            for (int i = 0; i < files.Length; ++i)
            {
                File.Delete(files[i]);
                Debug.Log("已删除Json文件：" + files[i]);
            }
            return true;
        }

#endregion

    }
}


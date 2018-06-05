using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace xys.json
{
    using PackTool;

    public class JsonObject<T>
    {
        string m_filePath = string.Empty;// json文件路径

        bool _hasInit = false;
        T _objValue;// 对象值
        public T ObjectValue
        {
            get { return _objValue; }
            set { _objValue = value; }
        }

        public JsonObject()
        {
            _hasInit = false;
        }

        /// <summary>
        /// 路径初始化
        /// </summary>
        /// <param name="filePath"></param>
        public JsonObject(string filePath)
        {
            _hasInit = false;
            m_filePath = filePath;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public bool TryGet(out T objValue)
        {
            if (!_hasInit)
            {
                string json = PackTool.TextLoad.GetString(m_filePath);
                if (string.IsNullOrEmpty(json))
                {
                    Debuger.LogError(string.Format("type={1}, 找不到配置文件：{0}", m_filePath, typeof(T)));
                    objValue = default(T);
                    return false;
                }

                try
                {
                    objValue = JsonUtility.FromJson<T>(json);
                    _objValue = objValue;
                    _hasInit = true;
                    return true;
                }
                catch (System.Exception ex)
                {
                    Debuger.LogException(ex);
                    objValue = default(T);
                    return false;
                }
            }
            else
            {
                objValue = _objValue;
                return true;
            }
        }

        public void Release()
        {
            _objValue = default(T);
            _hasInit = false;
        }
    }

    /// <summary>
    /// Json配置文件接口，主要用来支持读写文件名
    /// </summary>
    public interface IJsonFile
    {
        /// <summary>
        /// key值保存文件名
        /// </summary>
        /// <returns></returns>
        string GetKey();

        /// <summary>
        /// 修改文件key值，即文件名
        /// </summary>
        /// <returns></returns>
        void SetKey(string key);

    }

    /// <summary>
    /// 只编辑器下用
    /// 用来加载指定Json文件
    /// 注意：需要注意是否给编辑工具使用
    /// 支持功能：
    /// 1. 编辑器下可支持读取，保存，删除，重名Json文件等操作
    /// 2. 运行时只能读取加载Json文件，并转为对象数据
    /// 未支持
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonFile<ValueT> where ValueT : IJsonFile, new()
    {

        public const string JsonFileSuffix = ".json";

        string m_filePath = string.Empty;// json文件路径


        string m_fileName = string.Empty;// json文件名key
        public string FileName
        {
            get { return m_fileName; }
        }

        ValueT _objValue;// 对象值
        public ValueT ObjectValue
        {
            get { return _objValue; }
            set { _objValue = value; }
        }

        public JsonFile() {
        }

        /// <summary>
        /// 路径初始化
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="toolUsed">true:只在编辑工具下有效，不会管是否打包模式</param>
        public JsonFile(string filePath)
        {
            SetFilePath(filePath);
        }

        void SetFilePath(string filePath)
        {
            m_filePath = filePath;
            string[] files = m_filePath.Split('/');
            m_fileName = files[files.Length - 1];
            if (m_fileName.EndsWith(JsonFileSuffix))
                m_fileName = m_fileName.Substring(0, m_fileName.Length - JsonFileSuffix.Length);

            if (_objValue != null)
                _objValue.SetKey(m_fileName);
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <returns></returns>
        public string GetFilePath()
        {
            return m_filePath;
        }

        /// <summary>
        /// 检查key值是否与文件名一致
        /// </summary>
        void CheckFileName()
        {
            if (_objValue != null && !string.IsNullOrEmpty(m_fileName))
            {
                if (!_objValue.GetKey().Equals(m_fileName))
                {
                    Debug.LogWarning(string.Format("Json文件名与key值不一致，把key={0}改为：{1}", _objValue.GetKey(), m_fileName));
                    _objValue.SetKey(m_fileName);
                }
            }
        }


#region public Methods

        /// <summary>
        /// 读入配置文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Read()
        {
            string filePath = GetFilePath();
            if (!filePath.EndsWith(JsonFileSuffix))
                filePath += JsonFileSuffix;
            if (!File.Exists(filePath))
            {
                Debug.LogError("加载Json文件失败！路径：" + filePath);
                _objValue = new ValueT();
                return false;
            }
            string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            _objValue = JsonUtility.FromJson<ValueT>(json);
            CheckFileName();
            return true;
        }

        /// <summary>
        /// 保存，如果需要修改值
        /// </summary>
        /// <param name="val"></param>
        public void Write()
        {
            Write(_objValue);
        }

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        public void Write(ValueT value)
        {
            _objValue = value;
            CheckFileName();
            string filePath = GetFilePath();
            if (!filePath.EndsWith(JsonFileSuffix))
                filePath += JsonFileSuffix;
            string json = JsonUtility.ToJson(value);
            File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Delete()
        {
            string filePath = GetFilePath();
            if (!filePath.EndsWith(JsonFileSuffix))
                filePath += JsonFileSuffix;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.LogWarning(string.Format("Json文件: {0}已删除!", filePath));
                return true;
            }
            else
            {
                Debug.LogWarning(string.Format("Josn文件删除失败！不存在Json文件：{0}!", filePath));
                return false;
            }
        }

        /// <summary>
        /// 文件改名
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public bool Move(string sourceFilePath, string newFilePath)
        {
            string sourcePath = sourceFilePath;
            if (!sourcePath.EndsWith(JsonFileSuffix))
                sourcePath += JsonFileSuffix;

            if (File.Exists(sourcePath))
            {
                if (!newFilePath.EndsWith(JsonFileSuffix))
                    newFilePath += JsonFileSuffix;
                SetFilePath(newFilePath);// 重新指定目录
                Write();
                File.Delete(sourceFilePath);
                Debug.LogWarning(string.Format("Josn文件: {0}重命名为：{1}!", sourceFilePath, newFilePath));
                return true;
            }
            else
            {
                Debug.LogWarning(string.Format("Josn文件重命名失败！不存在Json文件：{0}!", sourceFilePath));
                return false;
            }
        }

#endregion

    }
}


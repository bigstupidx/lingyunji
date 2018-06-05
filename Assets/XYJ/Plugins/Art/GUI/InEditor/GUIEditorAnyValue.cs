#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

namespace GUIEditor
{
    public class AnyValue
    {
        public AnyValue(DataType t, string str, System.Type st)
        {
            SetValue(t, str, st);
        }

        public DataType type { get; set; }

        // 真实的数据类型
        public System.Type type_ { get; protected set; }

        // 类型参数
        public string type_name { get; set; }

        public object value { get; protected set; }

        void SetNewValue(DataType t, object v, System.Type st)
        {
            type_ = st;
            type = t;
            value = v;
        }

        public AnyValue clone()
        {
            return new AnyValue(type, (value != null ? value.ToString() : ""), type_);
        }

        public void SetValue(DataType t, string v, System.Type st = null)
        {
            type = t;
            try
            {
                switch (type)
                {
                case DataType.Null: type_ = null; value = null; break;
                case DataType.Bool:
                    type_ = typeof(bool);
                    value = (v.Length < 4 ? (int.Parse(v) == 0 ? false : true) : bool.Parse(v));
                    break;
                case DataType.Float:
                    type_ = typeof(float);
                    float tmpF = 0.0f;
                    float.TryParse(v, out tmpF);
                    value = tmpF;
                    break;
                case DataType.Int:
                    type_ = typeof(int);
                    int tmpI = 0;
                    int.TryParse(v, out tmpI);
                    value = tmpI;
                    break;
                case DataType.String: type_ = typeof(string); value = v; break;
                case DataType.Enum: if (st != null) type_ = st; value = System.Enum.Parse(type_, v); break;
                default: Debuger.LogError("type:" + type + " error!"); break;
                }
            }
            catch (System.Exception ex)
            {
                Debuger.LogError("DataType:" + t + " v:" + v);
                Debug.LogException(ex);
            }
        }

        public int GetInt(int defv)
        {
            return GetValue<int>(DataType.Int, defv);
        }

        public float GetFloat(float defv)
        {
            return GetValue<float>(DataType.Float, defv);
        }

        public bool GetBool(bool defv)
        {
            return GetValue<bool>(DataType.Bool, defv);
        }

        public string GetString(string defv)
        {
            return GetValue<string>(DataType.String, defv);
        }

        public System.Enum GetEnum(System.Enum defV)
        {
            if (type_ != defV.GetType())
            {
                SetNewValue(DataType.Enum, defV, defV.GetType());
            }

            return (System.Enum)value;
        }

        T GetValue<T>(DataType t, T defV)
        {
            if (type != t)
            {
                SetNewValue(t, defV, typeof(T));
            }

            return (T)value;
        }

        public void SetInt(int defv)
        {
            SetValue(DataType.Int, defv.ToString());
        }

        public void SetFloat(float defv)
        {
            SetValue(DataType.Float, defv.ToString());
        }

        public void SetBool(bool defv)
        {
            SetValue(DataType.Bool, defv.ToString());
        }

        public void SetString(string defv)
        {
            SetValue(DataType.String, defv);
        }

        public void SetEnum(System.Enum defv)
        {
            SetValue(DataType.Enum, defv.ToString(), defv.GetType());
        }
    }
}
#endif
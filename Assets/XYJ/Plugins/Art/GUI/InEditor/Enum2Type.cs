using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Reflection;

namespace GUIEditor
{
    public class Enum2Type
    {
        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class EnumExpel : System.Attribute
        {

        }

        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class EnumValue : System.Attribute
        {
            public EnumValue(string name)
            {
#if UNITY_EDITOR
                Name = name;
                Priority = 0;
#endif
            }

            public EnumValue(string name, int priority)
            {
#if UNITY_EDITOR
                Name = name;
                Priority = priority;
#endif
            }

#if UNITY_EDITOR
            public string Name { get; protected set; }
            public int Priority { get; protected set; }// 扩展枚举值显示顺序用，默认都为0，不影响原来的枚举顺序
#endif
        }

        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
        public class ParamValue : System.Attribute
        {
            // key值，显示文本，数据类型，初始值
            public ParamValue(string key, string tx, DataType type, string initValue, float text_width = -1, float value_width = -1, string st = "")
            {
#if UNITY_EDITOR
                Key = key;
                text = tx;
                dataType = type;
                InitValue = initValue;
                showType = st;
                Text_Width = text_width;
                Value_Width = value_width;
#endif
            }

            public ParamValue(string key, string tx, Type type, string initValue, float text_width = -1, float value_width = -1, string st = "")
            {
#if UNITY_EDITOR
                Key = key;
                text = tx;
                dataType = DataType.Enum;
                InitValue = initValue.ToString();
                RealType = type;

                Text_Width = text_width;
                Value_Width = value_width;
#endif
            }
#if UNITY_EDITOR

            public string Key { get; protected set; }

            public string text { get; protected set; }

            public DataType dataType { get; protected set; }

            public string showType { get; protected set; }

            public Type RealType { get; protected set; }

            public string InitValue { get; protected set; }

            public float Text_Width { get; protected set; }

            public float Value_Width { get; protected set; }

            public Value CloneValue
            {
                get
                {
                    Value value = new Value();
                    value.key = Key;
                    value.text = text;
                    value.showType = showType;
                    value.text_width = Text_Width;
                    value.value_width = Value_Width;
                    value.value.SetValue(dataType, InitValue, RealType);

                    return value;
                }
            }
#endif
        }
#if UNITY_EDITOR

        public class E
        {
            public Enum value; // 枚举值
            public string src; // 源值
            public string text; // 枚举描述
            public int priority;// 枚举位置顺序

            public List<ParamValue> paramList = new List<ParamValue>(); // 参数值
        }

        public class ET
        {
            public ET()
            {
                valueList = new List<E>();
                intValue = new List<int>();
                srcTextList = new List<string>();
                showTextList = new List<string>();
            }

            // 值类型
            public List<E> valueList { get; protected set; }

            public List<int> intValue { get; protected set; }

            public List<string> srcTextList { get; protected set; }

            public List<string> showTextList { get; protected set; }

            public int FindIndex(Enum value)
            {
                for (int i = 0; i < valueList.Count; ++i)
                {
                    if (value.Equals(valueList[i].value))
                    {
                        return i;
                    }
                }

                return -1;
            }

            public E FindE(int value)
            {
                for (int i = 0; i < valueList.Count; ++i)
                {
                    if (((int)((object)valueList[i].value)) == value)
                        return valueList[i];
                }

                return null;
            }

            public E FindE(Enum value)
            {
                for (int i = 0; i < valueList.Count; ++i)
                {
                    if (((int)(object)valueList[i].value) == ((int)(object)value))
                        return valueList[i];
                }

                return null;
            }
        }

        // 参数列表
        public class ParamList
        {
            public ParamList()
            {
                valueList = new List<ParamValue>();
            }

            public List<ParamValue> valueList { get; protected set; }
        }

        public class Etype
        {
            public Etype(Type t)
            {
                type = t;
                etype = new ET();
            }

            // 类型
            public System.Type type { get; protected set; }

            public ET etype { get; protected set; }
        }

        static Dictionary<Type, Etype> TypeList = new Dictionary<Type, Etype>();

        public static Etype To(Type type)
        {
            if (!type.IsEnum)
                return null; // 非枚举

            Etype etype = null;
            if (TypeList.TryGetValue(type, out etype))
                return etype;

            etype = new Etype(type);
            foreach (MemberInfo info in type.GetMembers())
            {
                if (info.MemberType == MemberTypes.Field && info.Name != "value__")
                {
                    object[] os = info.GetCustomAttributes(typeof(EnumExpel), false);
                    if (os != null && os.Length != 0)
                        continue;

                    E e = new E();
                    e.src = info.Name;
                    e.value = (System.Enum)System.Enum.Parse(type, e.src);
                    e.text = e.src;
                    os = info.GetCustomAttributes(typeof(EnumValue), false);
                    if (os != null && os.Length > 0)
                    {
                        EnumValue ev = os[0] as EnumValue;
                        e.text = ev.Name;
                        e.priority = ev.Priority;
                    }

                    os = info.GetCustomAttributes(typeof(ParamValue), false);
                    foreach (object o in os)
                        e.paramList.Add((ParamValue)o);

                    etype.etype.valueList.Add(e);
                    //etype.etype.intValue.Add((int)(object)e.value);
                    //etype.etype.srcTextList.Add(e.src);
                    //etype.etype.showTextList.Add(e.text);
                    
                }
            }
            etype.etype.valueList.Sort(EnumPriorityCompare);
            for (int i = 0; i < etype.etype.valueList.Count; ++i)
            {
                E e = etype.etype.valueList[i];
                etype.etype.intValue.Add((int)(object)e.value);
                etype.etype.srcTextList.Add(e.src);
                etype.etype.showTextList.Add(e.text);
            }

            TypeList.Add(type, etype);
            return etype;
        }
        static int EnumPriorityCompare(E x, E y)
        {
            int r = x.priority.CompareTo(y.priority);
            if (r == 0)
                return x.value.CompareTo(y.value);
            else
                return r;
        }

        public static Etype To<T>()
        {
            Type type = typeof(T);
            return To(type);
        }
#endif
    }
}
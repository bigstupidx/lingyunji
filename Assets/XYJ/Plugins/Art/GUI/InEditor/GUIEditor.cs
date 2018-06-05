using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GUIEditor
{
    // 数据类型
    // 字符串，整型 ，浮点数，布尔
    public enum DataType
    {
        Null, // 无效的数据类型
        String, // 字符串
        Int, // 整数
        Float, // 浮点数
        Bool, // 布尔值
        Enum, // 枚举值
    }

#if UNITY_EDITOR
    public class Value
    {
        public string key; // key值
        public string text; // 显示的文本
        public string showType; // 显示类型
        public bool isEnum = false; // 是否使用默认的枚举类型

        public bool isNewLine = true; // 是否单独一行
        public float text_width = -1;
        public float value_width = -1;

        public AnyValue value = new AnyValue(DataType.Null, null, null);
    }

    public class Misc
    {
        public static string ReadAllText(string filename)
        {
            return ReadAllText(filename, System.Text.Encoding.Default);
        }

        public static string ReadAllText(string filename, System.Text.Encoding encoding)
        {
            if (!System.IO.File.Exists(filename))
                return "";

            try
            {
                return System.IO.File.ReadAllText(filename, encoding);
            }
            catch (System.Exception)
            {
                string copy_file = filename + "___copy___";
                if (System.IO.File.Exists(copy_file))
                    System.IO.File.Delete(copy_file);

                System.IO.File.Copy(filename, copy_file);

                string text = System.IO.File.ReadAllText(copy_file, encoding);

                System.IO.File.Delete(copy_file);

                return text;
            }
        }
    }
#endif
}

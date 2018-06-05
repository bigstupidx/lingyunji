using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PackTool
{
    public partial class MonoScriptAutoGen
    {
        public partial class MonoText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<{0}>(com.{1}, writer, mgr, __CollectMono__<{0}>);
";

            public const string LoadTextList = @"
            __LoadMonoList__({1}, data, reader, PrefabLoadEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = go.GetComponent<{2}>();
                break;
";
        }

        static System.Type GetArrayElementType(System.Type type)
        {
            if (type.IsGenericType)//判断是否是泛型
            {
                // 为模版的
                List<System.Type> genericTypes = new List<System.Type>(type.GetGenericArguments());
                if (genericTypes.Count != 0)
                    return genericTypes[0];
            }
            else
            {
                return type.GetElementType();
            }

            return null;
        }

        static void Gen_MonoList(FieldInfo info, Data data, int index)
        {
            string elementName = GetTypeName(GetArrayElementType(info.FieldType));

            // 收集接口
            data.collect.Append(string.Format(MonoText.CollectTextList, elementName, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(MonoText.LoadTextList, elementName, index));

            // 设置
            data.prefabloadendlist.Append(string.Format(MonoText.LoadendTextList, index, info.Name, elementName));
        }
    }
}
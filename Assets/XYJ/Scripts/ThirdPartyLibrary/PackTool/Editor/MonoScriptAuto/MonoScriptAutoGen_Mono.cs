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
            public const string CollectText = @"
            has |= __CollectMono__<{0}>(ref com.{1}, writer, mgr);
";

            public const string LoadText = @"
            __LoadMono__(data, reader, PrefabLoadEnd, new object[]{{data, {1}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = go.GetComponent<{2}>();
                break;
";
        }

        static string GetTypeName(System.Type type)
        {
            string ns = type.Namespace;
            if (string.IsNullOrEmpty(ns))
                ns = type.Name;
            else
            {
                if (ns == "UnityEngine")
                    return type.Name;

                ns = ns + "." + type.Name;
            }

            return ns;
        }

        static void Gen_MonoPrefab(FieldInfo info, Data data, int index)
        {
            string ns = GetTypeName(info.FieldType);

            // 收集接口
            data.collect.Append(string.Format(MonoText.CollectText, ns, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(MonoText.LoadText, ns, index));

            // 设置
            data.prefabloadend.Append(string.Format(MonoText.LoadendText, index, info.Name, ns));
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PackTool
{
    public partial class MonoScriptAutoGen
    {
        const string AutoFile =
@"#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{{
    public partial class ComponentSave
    {{
        // 注册自动生成的组件
        static void RegAuto()
        {{
{0}
        }}
    }}
}}
#endif";

        const string RegAutoOne =
@"
            Register<{0}>(new {1}PackData());
";

        // 重新生成
        [MenuItem("Assets/PackTool/AnewBuild", false, 0)]
        static void AnewBuild()
        {
            // 遍历目录下的文件，然后对文件名分析
            List<string> autoFile = GetAllCSFileList(Application.dataPath + "/Scripts/ThirdPartyLibrary/PackTool/Component/Auto");
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            Dictionary<string, MonoScript> scripts = FindAllMonoScript();
            foreach (string file in autoFile)
            {
                string typeName = file.Substring(0, file.Length - "PackData.cs".Length);
                string packname = typeName;
                MonoScript s = null;
                if (scripts.TryGetValue(typeName, out s))
                    typeName = GetTypeName(s.GetClass());

                builder.AppendFormat(RegAutoOne, typeName, packname);
            }

            string filepath = Application.dataPath + "/Scripts/ThirdPartyLibrary/PackTool/Component/ComponentSave_RegAuto.cs";
            System.IO.File.WriteAllText(filepath, string.Format(AutoFile, builder.ToString()));
        }

        static bool isPackType(System.Type type)
        {
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(PackAttribute), false))
                    return true;                    
            }

            return false;
        }

        // 查找所有的脚本
        static Dictionary<string, MonoScript> FindAllMonoScript()
        {
            Dictionary<string, MonoScript> scripts = new Dictionary<string, MonoScript>();
            foreach (MonoScript mono in Resources.FindObjectsOfTypeAll<MonoScript>())
            {
                var type = mono.GetClass();
                if (type != null && isPackType(type))
                {
                    if (scripts.ContainsKey(type.Name))
                    {
                        Debug.LogErrorFormat("type:{0} repate!", type.Name);
                    }
                    else
                    {
                        scripts.Add(type.Name, mono);
                    }
                }
            }

            return scripts;
        }
    }
}
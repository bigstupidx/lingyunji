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
        public partial class MeshText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<Mesh>(com.{0}, writer, mgr, __CollectMesh__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<Mesh>({0}, data, reader, __LoadMesh__, LoadMeshEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = mesh;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadMeshEndList(Mesh mesh, object p)
        {{
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            {0} com = data.mComponent as {0};
            
            switch(type)
            {{
{1}
            }}

            data.OnEnd();
        }}
";
        }
        static void Gen_MeshList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(MeshText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(MeshText.LoadTextList, index));

            // 设置
            data.meshloadendlist.Append(string.Format(MeshText.LoadendTextList, index, info.Name));
        }

        static void Gen_MeshFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(MeshText.LoadEndFunList, GetTypeName(type), data.meshloadendlist);
        }
    }
}
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
            public const string CollectText = @"
            has |= __CollectMesh__(ref com.{1}, writer, mgr);
";

            public const string LoadText =
    @"
            __LoadMesh__(data, reader, LoadMeshEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = mesh;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadMeshEnd(Mesh mesh, object p)
        {{
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            {0} com = data.mComponent as {0};
            switch (index)
            {{
{1}
            }}
            data.OnEnd();
        }}
";
        }

        static void Gen_Mesh(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(MeshText.CollectText, index, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(MeshText.LoadText, index));

            // 设置
            data.meshloadend.Append(string.Format(MeshText.LoadendText, index, info.Name));
        }

        static void Gen_MeshFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(MeshText.LoadEndFun, GetTypeName(type), data.meshloadend);
        }
    }
}
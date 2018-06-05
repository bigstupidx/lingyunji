using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GUIEditor;

namespace PackTool
{
    public partial class BuiltinResourceEditor : EditorWindow
    {
        [MenuItem("PackTool/资源工具/BuiltinResource查看", false, 9)]
        static public void OpenPackEditorWindow()
        {
            GetWindow<BuiltinResourceEditor>(false, "BuiltinResourceEditor", true);
        }

        enum ShowType
        {
            Shader, // 
            Material,            
        }

        ShowType showType = ShowType.Shader;
        public BuiltinResource builtin { get; private set; }
        //public ResourcesUsedInfo usedInfo { get; private set; }

        private void OnEnable()
        {
            builtin = AssetDatabase.LoadAssetAtPath<BuiltinResource>("Assets/__copy__/BuiltinResource.prefab");
            showShader = new ShowShader(this);
        }

        ShowShader showShader;
        ParamList paramList = new ParamList();

        void OnGUI()
        {
            //if (GUILayout.Button("查看引用", GUILayout.Width(200f), GUILayout.Height(50f)))
            //{
            //    if (usedInfo == null)
            //        usedInfo = new ResourcesUsedInfo();
            //    usedInfo.Init();
            //}

            showType = (ShowType)GuiTools.EnumPopup(false, "查看类别", showType);
            switch (showType)
            {
            case ShowType.Shader:
                ShowShaderGUI(paramList);
                break;
            }
        }
    }
}
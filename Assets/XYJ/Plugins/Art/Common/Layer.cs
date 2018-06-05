using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif
public class Layer
{
    // UI层
    static public int ui { get; protected set; }
    static public int uiMask { get; protected set; }
    static public int passUI { get; protected set; }
    static public int passMask { get; protected set; }

    // UI层
    //static public int uiBlurExcep { get; protected set; }
    //static public int uiBlurExcepMask { get; protected set; }

    static public int ui3d { get; protected set; }
    static public int ui3dMask { get; protected set; }
    static public int terrain { get; protected set; }
    static public int terrainMask { get; protected set; }
    static public int wall { get; private set; }
    static public int wallMask { get; private set; }

    static public int no { get; private set; }
    static public int noMask { get; private set; }
    static Layer()
    {
        ui = LayerMask.NameToLayer("UI");
        passUI = LayerMask.NameToLayer("PassUI");
        uiMask = 1 << ui | 1 << passUI;
        passMask = 1 << passUI;

        terrain = LayerMask.NameToLayer("Terrains");
        terrainMask = 1 << terrain;

        wall = LayerMask.NameToLayer("Wall");
        wallMask = 1 << wall;

#if UNITY_EDITOR 
        RemoveLayer("UIBlurExcep");
        AddLayer("UI3D"); // 3D UI层，主要用在人物头顶名字，飘血之类的东东
        AddLayer("no"); // 不渲染
#endif
        //        uiBlurExcep = LayerMask.NameToLayer("UIBlurExcep");
        //        uiBlurExcepMask = 1 << ui;

        ui3d = LayerMask.NameToLayer("UI3D");
        ui3dMask = 1 << ui3d;

        no = LayerMask.NameToLayer("no");
        noMask = 1 << no;
    }

#if UNITY_EDITOR
    static void AddLayer(string layer)
    {
        if (!isHasLayer(layer))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "layers")
                {
                    //层默认是32个，只能从第8个开始写入自己的层  
                    for (int i = 8; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);//获取层信息  
                        if (string.IsNullOrEmpty(dataPoint.stringValue))//如果制定层内为空，则可以填写自己的层名称
                        {
                            dataPoint.stringValue = layer;//设置名字  
                            tagManager.ApplyModifiedProperties();//保存修改的属性  
                            Debug.LogFormat("添加Layer:{0}", layer);
                            return;
                        }
                    }
                }
            }
        }
    }

    static void RemoveLayer(string layer)
    {
        if (isHasLayer(layer))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "layers")
                {
                    //层默认是32个，只能从第8个开始写入自己的层  
                    for (int i = 8; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);//获取层信息  
                        if (dataPoint.stringValue == layer)
                        {
                            dataPoint.stringValue = string.Empty;
                            tagManager.ApplyModifiedProperties();
                            Debug.LogFormat("移除Layer:{0}", layer);
                            return;
                        }
                    }
                }
            }
        }
    }

    static bool isHasLayer(string layer)
    {
        string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
        for (int i = 0; i < layers.Length; ++i)
        {
            if (layers[i] == layer)
                return true;
        }

        return false;
    }
 
#endif
}
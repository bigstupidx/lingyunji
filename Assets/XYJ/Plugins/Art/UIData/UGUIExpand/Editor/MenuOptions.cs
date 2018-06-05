using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UnityEditor.UI
{
    /// <summary>
    /// This script adds the UI menu options to the Unity Editor.
    /// </summary>

    static class UIMenuOptions
    {
        [MenuItem("GameObject/UI/面板", false)]
        static public void AddUIPanelBase(MenuCommand menuCommand)
        {
            var root = GameObject.Find("UIRoot");
            if (root == null)
            {
                root = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/UIData/Data/Prefabs/Template/ResourcesExport/UIRoot.prefab");
                root = Object.Instantiate(root);
                root.name = "UIRoot";
            }
        }

        [MenuItem("GameObject/UI/复制路径", false)]
        static public void CopyPath(MenuCommand menuCommand)
        {
            GUIUtility.systemCopyBuffer = XTools.Utility.GetPath(null, Selection.activeGameObject);
        }
    }
}

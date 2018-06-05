using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class UIClanTest : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(400, 50, 200, 200));
        GUILayout.BeginVertical();

        if (GUILayout.Button("打开氏族创建界面"))
        {
            xys.App.my.uiSystem.ShowPanel("UIClanCreatePanel", new object() { }, true);
        }

        if (GUILayout.Button("关闭氏族创建面板"))
        {
            xys.App.my.uiSystem.HidePanel("UIClanCreatePanel");
        }


        if (GUILayout.Button("打开氏族界面"))
        {
            xys.App.my.uiSystem.ShowPanel("UIClanPanel", new object() { }, true);
        }

        if (GUILayout.Button("关闭氏族面板"))
        {
            xys.App.my.uiSystem.HidePanel("UIClanPanel");
        }


        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
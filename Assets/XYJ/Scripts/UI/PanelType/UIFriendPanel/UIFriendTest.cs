using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class UIFriendTest : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(400, 50, 200, 200));
        GUILayout.BeginVertical();

        if (GUILayout.Button("打开好友界面"))
        {
            xys.App.my.uiSystem.ShowPanel("UIFriendPanel", new object() { }, true);
        }
     
        if (GUILayout.Button("关闭好友面板"))
        {
            xys.App.my.uiSystem.HidePanel("UIFriendPanel");
        }


        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
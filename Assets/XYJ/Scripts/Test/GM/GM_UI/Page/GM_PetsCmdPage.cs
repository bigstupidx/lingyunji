using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.gm
{
    public class GM_PetsCmdPage : GM_IPage
    {
        string cmd = string.Empty;
        public string GetTitle()
        {
            return "宠物";
        }

        public void OnOpen()
        {
            cmd = PlayerPrefs.GetString("gmCmd_Pets",string.Empty);
        }
        public void OnClose()
        {
            PlayerPrefs.SetString("gmCmd_Pets", cmd);
        }

        public void OnGUI(Rect beginArea)
        {
            GUILayout.BeginArea(beginArea);

            if (!Application.isPlaying)
                return;
            if (GUILayout.Button("打开宠物面板"))
            {
                App.my.uiSystem.ShowPanel(UI.PanelType.UIPetsPanel, 0);
            }
            if (GUILayout.Button("关闭宠物面板"))
            {
                App.my.uiSystem.HidePanel(UI.PanelType.UIPetsPanel);
            }
            GUILayout.BeginHorizontal();
            cmd = GUILayout.TextField(cmd, GUILayout.Width(200));
            if (GUILayout.Button("执行", GUILayout.Width(100)))
            {
                int petid;
                if(int.TryParse(cmd,out petid))
                    xys.App.my.eventSet.FireEvent(xys.EventID.Pets_Create, petid);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

    }
}

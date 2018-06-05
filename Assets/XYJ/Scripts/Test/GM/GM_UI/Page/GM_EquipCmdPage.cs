using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.gm
{
    public class GM_EquipCmdPage : GM_IPage
    {
        string cmd = string.Empty;
        public string GetTitle()
        {
            return "装备";
        }

        public void OnOpen()
        {
            cmd = PlayerPrefs.GetString("gmCmd_Equip",string.Empty);
        }
        public void OnClose()
        {
            PlayerPrefs.SetString("gmCmd_Equip", cmd);
        }

        public void OnGUI(Rect beginArea)
        {
            GUILayout.BeginArea(beginArea);
            if (!Application.isPlaying)
                return;
            
            if (GUILayout.Button("打开装备"))
            {
                App.my.uiSystem.ShowPanel(UI.PanelType.UIEquipPanel, 0);
            }
            if (GUILayout.Button("关闭装备"))
            {
                App.my.uiSystem.HidePanel(UI.PanelType.UIEquipPanel);
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("重置装备操作次数", GUILayout.Width(250)))
            {
                App.my.eventSet.fireEvent(EventID.Equip_ResetTimes);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("启用装备操作次数", GUILayout.Width(200)))
            {
                App.my.eventSet.FireEvent(EventID.Equip_SetOpTimesActive, true);
            }
            if (GUILayout.Button("禁用装备操作次数", GUILayout.Width(200)))
            {
                App.my.eventSet.FireEvent(EventID.Equip_SetOpTimesActive, false);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

    }
}

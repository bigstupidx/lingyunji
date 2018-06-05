using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys;
using xys.gm;
using xys.UI;

public class GM_SKillCmdPage : GM_IPage
{
    
    string cmd = string.Empty;
    public string GetTitle()
    {
        return "技能";
    }

    public void OnClose()
    {
        PlayerPrefs.SetString("gmCmd_Skill", cmd);
    }

    public void OnGUI(Rect beginArea)
    {
        GUILayout.BeginArea(beginArea);
        if (!Application.isPlaying)
        {
            return;
        }
        if (GUILayout.Button("打开技能面板"))
        {

            App.my.uiSystem.ShowPanel(PanelType.UISkillPanel, 1);
        }
        if (GUILayout.Button("关闭技能面板"))
        {
            App.my.uiSystem.HidePanel(PanelType.UISkillPanel);
        }
        GUILayout.BeginHorizontal();
        cmd = GUILayout.TextField(cmd, GUILayout.Width(200));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void OnOpen()
    {
        cmd = PlayerPrefs.GetString("gmCmd_Skill", string.Empty);
    }
}

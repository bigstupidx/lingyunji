using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.gm
{
    public class GM_WelfareCmdPage : GM_IPage
    {
        string cmd = string.Empty;
        public string GetTitle()
        {
            return "福利";
        }

        public void OnOpen()
        {
            cmd = PlayerPrefs.GetString("gmCmd_Welfare",string.Empty);
        }
        public void OnClose()
        {
            PlayerPrefs.SetString("gmCmd_Welfare", cmd);
        }

        public void OnGUI(Rect beginArea)
        {
            GUILayout.BeginArea(beginArea);

            if (!Application.isPlaying)
                return;
            if (GUILayout.Button("打开福利面板"))
            {
                App.my.uiSystem.ShowPanel(UI.PanelType.UIWelfarePanel, 0);
            }
            if (GUILayout.Button("关闭福利面板"))
            {
                App.my.uiSystem.HidePanel(UI.PanelType.UIWelfarePanel);
            }
            GUILayout.BeginHorizontal();
            cmd = GUILayout.TextField(cmd, GUILayout.Width(100));
            if (GUILayout.Button("设定登陆天数", GUILayout.Width(100)))
            {
                int days;
                if (int.TryParse(cmd, out days))
                {
                    NetProto.Int32 msg = new NetProto.Int32();
                    msg.value = days;
                    App.my.eventSet.FireEvent<NetProto.Int32>(EventID.Welfare_Test, msg);
                }
            }
            //if (GUILayout.Button("重置数据"))
            //{
            //    NetProto.Int32 msg = new NetProto.Int32();
            //    msg.value = -1;
            //    App.my.eventSet.FireEvent<NetProto.Int32>(EventID.Welfare_Test, msg);
            //}
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

    }
}

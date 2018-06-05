using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.gm
{
    public class GM_InputCmdPage : GM_IPage
    {
        const int CMD_CNT = 15;

        GM_CmdInputRecord[] m_cmds = new GM_CmdInputRecord[CMD_CNT];

        public void OnOpen()
        {
            for (int i = 0; i < CMD_CNT; i++)
                m_cmds[i] = new GM_CmdInputRecord("gmCmd" + i);
        }
        public void OnClose()
        {

        }
        public string GetTitle()
        {
            return "输入";
        }

        public void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);

            GUILayout.Label("自定义命令", GUILayout.Height(40));
            // GM命令输入
            for(int i=0;i<CMD_CNT;i++)
            {
                m_cmds[i].OnGUI();
            }


            GUILayout.EndArea();
        }
    }

}

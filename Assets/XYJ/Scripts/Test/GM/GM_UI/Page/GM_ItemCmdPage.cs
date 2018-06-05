using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.gm
{
    public class GM_ItemCmdPage : GM_IPage
    {
        const int CMD_CNT = 5;

        string[] m_cmds = new string[CMD_CNT];

        public void OnOpen()
        {
            for (int i = 0; i < CMD_CNT; i++)
                m_cmds[i] = GetCmdSave(i);
        }
        public void OnClose()
        {

        }
        public string GetTitle()
        {
            return "道具";
        }

        string GetCmdSave(int i)
        {
            return PlayerPrefs.GetString("gmItem" + i);
        }

        public void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);

            GUILayout.Label("自定义命令", GUILayout.Height(40));
            // GM命令输入
            for (int i = 0; i < CMD_CNT; i++)
            {
                GUILayout.BeginHorizontal();
                m_cmds[i] = GUILayout.TextField(m_cmds[i], GUILayout.Width(200));

                //每个命令都记录下来
                if (m_cmds[i] != GetCmdSave(i))
                    PlayerPrefs.SetString("gmCmd" + i, m_cmds[i]);

                if (GUILayout.Button("执行", GUILayout.Width(100)))
                    GM_Cmd.instance.ParseCmd(m_cmds[i]);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();
        }

    }

}

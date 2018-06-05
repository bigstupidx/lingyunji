using UnityEngine;
using System.Collections;
using UI;
using System;

namespace xys.gm
{
    public class GM_GUIHelper
    {
        public static string TextField( string title,string text )
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title);
            string temStr = GUILayout.TextField(text);
            GUILayout.EndHorizontal();
            return temStr;
        }
    }


    //会记录输入的命令，默认回调是执行gm命令
    public class GM_CmdInputRecord
    {
        string m_key;
        string m_title;
        string m_button;
        string m_input = "";
        Action<string> m_actin;
        public GM_CmdInputRecord(string key,Action<string> action = null, string title = "",string button="执行")
        {
            m_key = key;
            m_title = title;
            m_button = button;
            if (action == null)
                m_actin = (p) => { GM_Cmd.instance.ParseCmd(p); };
            else
                m_actin = action;
            m_input = PlayerPrefs.GetString(m_key);
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(m_title))
                GUILayout.Label(m_title);

            m_input = GUILayout.TextField(m_input, GUILayout.Width(200));

            //每个命令都记录下来
            if (m_input != m_key)
                PlayerPrefs.SetString(m_key, m_input);

            if (GUILayout.Button(m_button, GUILayout.Width(100)))
                m_actin(m_input);
            GUILayout.EndHorizontal();
        }
    }

}

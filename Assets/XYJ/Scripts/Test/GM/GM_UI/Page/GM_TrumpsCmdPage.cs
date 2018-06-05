using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.gm
{
    public class GM_TrumpsCmdPage : GM_IPage
    {
        string cmd = string.Empty;
        public string GetTitle()
        {
            return "法宝";
        }
        public void OnOpen()
        {
        }

        public void OnClose()
        {
        }

        public void OnGUI(Rect beginArea)
        {
            GUILayout.BeginArea(beginArea);

            if (!Application.isPlaying)
                return;

            GUILayout.BeginHorizontal();
            cmd = GUILayout.TextField(cmd, GUILayout.Width(200));
            if (GUILayout.Button("执行", GUILayout.Width(100)))
            {
                int trumpId;
                if (int.TryParse(cmd, out trumpId))
                    xys.App.my.eventSet.FireEvent(xys.EventID.Trumps_Create, trumpId);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
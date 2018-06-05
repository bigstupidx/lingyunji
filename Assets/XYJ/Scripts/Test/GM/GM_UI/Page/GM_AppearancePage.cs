using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;
using System;

namespace xys.gm
{
    class GM_AppearancePage : GM_IPage
    {
        
        public string GetTitle()
        {
            return "外观";
        }

        public void OnClose()
        {
            
        }

        public void OnGUI(Rect beginArea)
        {
            GUILayout.BeginArea(beginArea);
            if (GUILayout.Button("天-秒"))
            {
                AppearanceModule.TimeRatio = 24 * 60 * 60;
            }
            if(GUILayout.Button("天-分"))
            {
                AppearanceModule.TimeRatio = 24 * 60 ;
            }
            if(GUILayout.Button("天-时"))
            {
                AppearanceModule.TimeRatio = 24;
            }
            GUILayout.EndArea();
        }

        public void OnOpen()
        {
           
        }
    }
}

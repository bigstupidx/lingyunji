using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace xys.gm
{
    public class GM_ExStoreCmdPage : GM_IPage
    {
        string cmd = string.Empty;
        public string GetTitle()
        {
            return "商会";
        }

        public void OnClose()
        {
            PlayerPrefs.SetString("gmCmd_ExStore", cmd);
        }

        public void OnGUI(Rect beginArea)
        {
            GUILayout.BeginArea(beginArea);
            if (!Application.isPlaying) {
                return;
            }
            if (GUILayout.Button("重置商会库存，次数")) {
                App.my.eventSet.fireEvent(EventID.TradeStore_Rest);
            }
            if (GUILayout.Button("还原商会配置表"))
            {
                App.my.eventSet.fireEvent(EventID.TradeStore_Recover);
            }
            if (GUILayout.Button("重置积分商城每日次数")) {
                App.my.eventSet.FireEvent<int>(EventID.ExchangeStore_RestDay,1024);
            }
            if (GUILayout.Button("重置积分商城每周次数")) {
                App.my.eventSet.FireEvent<int>(EventID.ExchangeStore_RestDay, 1024);
            }
            GUILayout.BeginHorizontal();
            cmd = GUILayout.TextField(cmd, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public void OnOpen()
        {
            cmd = PlayerPrefs.GetString("gmCmd_ExStore", string.Empty);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.gm
{
    public class GM_LevelCmdPage : GM_IPage
    {
        string cmd = string.Empty;
        Vector2 m_listScollPos = Vector2.zero;
        public string GetTitle()
        {
            return "关卡";
        }

        public void OnOpen()
        {
            cmd = PlayerPrefs.GetString("gmCmd_Level", string.Empty);
        }
        public void OnClose()
        {
            PlayerPrefs.SetString("gmCmd_Level", cmd);
        }

        public void OnGUI(Rect beginArea)
        {
            GUILayout.BeginArea(beginArea);

            if (!Application.isPlaying)
                return;
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("切换关卡");
            cmd = GUILayout.TextField(cmd, GUILayout.Width(200));
            if (GUILayout.Button("执行", GUILayout.Width(100)))
            {
                int levelId;
                if (int.TryParse(cmd, out levelId))
                {
                    //App.my.levelLogicMgr.ExitTheLevel();
                    //App.my.localPlayer.GetModule<LevelModule>().request.StartLevel(new NetProto.Int32() { value = levelId }, null);
                    //App.my.localPlayer.GetModule<LevelModule>().request.GMChange(new NetProto.Int32() { value = levelId }, null);
                    //App.my.eventSet.FireEvent<int>(EventID.Level_Change, levelId);
                    App.my.sceneMgr.ChangeScene(levelId);
                }
            }
            GUILayout.EndHorizontal();


            if(GUILayout.Button("打开界面"))
            {
                App.my.uiSystem.ShowPanel(UI.PanelType.UIConvenientPanel, null);
            }

            //绘制当前关卡的事件相关
#if UNITY_EDITOR
            if (GUILayout.Button("打开事件监视", GUILayout.Width(300)))
            {
                LevelMonitorEditor.OpenLevelMonitor();
            }
#endif
            GUILayout.EndArea();
        }
    }
}

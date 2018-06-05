using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.gm
{
    public class GM_AttributePage : GM_IPage
    {
        GM_CmdInputRecord m_attack; 
        public void OnOpen()
        {
            m_attack = new GM_CmdInputRecord("attack");
        }
        public void OnClose()
        {

        }
        public string GetTitle()
        {
            return "属性";
        }


        public void OnGUI(Rect rect)
        {
            if (!App.my.localPlayer.isAlive)
                return;

            IObject target = App.my.localPlayer.GetUIChooseTarget();
            GUILayout.BeginArea(rect);

            if (GUILayout.Button("目标属性"))
                if(target!=null)
                    GM_Cmd.instance.ParseCmd("getAttribute " + target.charSceneId);

            if (GUILayout.Button("目标buff"))
                GM_Cmd.instance.ParseCmd("getBuff " + target.charSceneId);
            

            if (GUILayout.Button("自己属性"))
                   GM_Cmd.instance.ParseCmd("getAttribute " + App.my.localPlayer.charSceneId);

            if (GUILayout.Button("自己buff"))
                GM_Cmd.instance.ParseCmd("getBuff " + App.my.localPlayer.charSceneId);
            


            m_attack.OnGUI();

            GUILayout.EndArea();
        }

        public static void ReloadConfig()
        {
            //服务器重载配置
            GM_Cmd.instance.ParseCmd("reloadconfig");
            CsvLoadAdapter.All();

            foreach (var p in App.my.sceneMgr.GetObjs())
                ((ObjectBase)p.Value).GM_Reload();
        }

    }

}

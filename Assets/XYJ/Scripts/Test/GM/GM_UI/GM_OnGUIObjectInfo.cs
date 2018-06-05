using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace xys.gm
{
    public partial class GM_UI
    {
        public static bool s_showObjectInfo = false;//显示角色数据

        void OnGUIAllRoleInfo()
        {
            if (s_showObjectInfo && App.my.sceneMgr!=null)
            {
                foreach(var p in App.my.sceneMgr.GetObjs())
                {
                    DrawRoleInfo(p.Value);
                }
            }
        }

        void DrawRoleInfo(IObject obj)
        {
            Camera camera = App.my.cameraMgr.m_mainCamera;

            if (camera == null || obj == null || !obj.isAlive)
                return;

            Vector3 bottomPos = camera.WorldToScreenPoint(obj.root.position);
            Vector3 topPos = camera.WorldToScreenPoint(obj.battle.actor.m_titile.position);


            //太远不显示
            if (Vector3.Distance(obj.position, App.my.localPlayer.position) > 20)
                return;

            float dh = 20;
            Rect topRect = new Rect(topPos.x, Screen.height - topPos.y, 500, 100);
            Rect bottomRect = new Rect(bottomPos.x, Screen.height - bottomPos.y, 500, 100);
            GUI.skin.label.normal.textColor = Color.red;

            GUI.Label(bottomRect, "charid:" + obj.charSceneId); bottomRect.y += dh;
            GUI.Label(bottomRect, "hp:" + obj.hpValue); bottomRect.y += dh;
            GUI.Label(bottomRect, "真气:" + obj.zhenQiValue); bottomRect.y += dh;
            GUI.Label(bottomRect, "速度:" + obj.speedValue); bottomRect.y += dh;
            GUI.Label(bottomRect, string.Format("护体:{0} {1}", obj.huTiValue, obj.huTiStateValue)); bottomRect.y += dh;

        
        }
    }
}

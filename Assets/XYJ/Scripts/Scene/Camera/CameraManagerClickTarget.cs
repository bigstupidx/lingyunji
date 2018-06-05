using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;
using xys.Test;

namespace xys
{
    public partial class CameraManager : MonoBehaviour
    {
        //更新鼠标点击选择目标
        void UpdateSelectedTarget()
        {
            if (m_mainCamera == null) 
                return;

            LocalPlayer localPlayer = App.my.localPlayer;
            if (Input.GetMouseButtonDown(0))//如果鼠标左键按下，切换锁定目标
            {
                if (localPlayer == null || !localPlayer.isAlive || localPlayer.battle.m_skillMgr.IsPlaying() )
                    return;

                //点击ui
                if ( App.my.uiSystem.isContain())
                    return;

                //特写镜头的时候禁止点选怪
                if (m_isFeature)
                    return;

                //不计算主角
                int layerMask = 1 << ComLayer.Layer_LocalPlayer;
                Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                //点击距离不能太远
                if (Physics.Raycast(ray, out hit, kvBattle.lockTargetMaxDistance, ~layerMask))
                {
                    GameObject clickGO = hit.transform.gameObject;
                    //角色选中框
                    if (clickGO.tag != ComLayer.TAG_ROLE)
                        return;
                    ObjectMono objMono = clickGO.GetComponent<ObjectMono>();
                    if (objMono == null || objMono.m_obj == null || !objMono.m_obj.isAlive || objMono.m_obj == localPlayer)
                        return;
                    IObject target = objMono.m_obj;

                    ////判断角色当前是否禁止点击
                    //if (roleMono.role.m_objPrototype.forbidClick)
                    //    return;

                    //选中敌人
                    if (BattleHelp.IsEnemy(localPlayer, target))
                    {
                        localPlayer.battle.m_targetMgr.SetTarget(target);
                    }
                    else
                    {
                        //选中非敌人
                        localPlayer.battle.m_targetMgr.SetTarget(null);
                        ((BattleManagerLocal)localPlayer.battle).m_autoChooseTarget.SetChooseTarget(target);

                        NpcBase npc = target as NpcBase;
                        if (npc != null && npc.canClick)
                        {
                            npc.OnClickNpc();
                        }
                    }
                }
            }
        }
    }

}

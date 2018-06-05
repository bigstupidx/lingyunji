using NetProto;
using xys.battle;
using System;
using System.Collections.Generic;
using UnityEngine;
using Config;


namespace xys.battle
{
    public class AIManagerBase : AIManager
    {
        //位置同步
        LocalObjectSysPos m_sysPos;

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (m_sysPos != null)
                m_sysPos.Clear();
        }


        public override void OnEnterScene()
        {
            base.OnEnterScene();

            if (m_obj.type == NetProto.ObjectType.Player)
            {
                if (m_obj is LocalPlayer)
                {
                    m_obj.battle.SetAiByClient(true);
                    SetIdleDefaultAI(SimpleAIType.Client_LocalPlayerInput, null, true);
                }
            }
            else if (m_obj.type == NetProto.ObjectType.Pet)
            {
                //宠物有对应玩家驱动
                Pet pet = m_obj as Pet;
                if (pet.m_masterId == App.my.localPlayer.charSceneId)
                {
                    m_obj.battle.SetAiByClient(true);
                    SetIdleDefaultAI(SimpleAIType.Client_Pet, null, true);
                }
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //同步坐标
            if (m_obj.battle.m_isAiByLocal)
            {
                if (m_sysPos == null)
                    m_sysPos = new LocalObjectSysPos(m_obj);
                m_sysPos.OnUpdate();
            }
        }
    }

}


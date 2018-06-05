using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;

namespace xys.battle
{
    class SingState : IState
    {
        public class SingInfo
        {
            public int singid;
            public Action<object> callback;
            public object para;
        }

        SingInfo m_info;
        ProgressBase m_progress;
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            m_info = (SingInfo)para;
            SingConfig cfg = SingConfig.Get(m_info.singid);

            float ani1Time = 0;
            obj.battle.m_aniMgr.ClearQueued();
            if (!string.IsNullOrEmpty(cfg.anmation1))
            {
                ani1Time = obj.battle.m_aniMgr.GetAniLenght(cfg.anmation1);
                obj.battle.m_aniMgr.PlayQueued(cfg.anmation1);
            }
            if (!string.IsNullOrEmpty(cfg.anmation2))
                obj.battle.m_aniMgr.PlayQueued(cfg.anmation2, 1.0f, cfg.singTime-ani1Time);
            if (!string.IsNullOrEmpty(cfg.anmation3))
                obj.battle.m_aniMgr.PlayQueued(cfg.anmation3);

            if(BattleHelp.IsMe(obj))
            {
                xys.UI.ProgressData barData = new xys.UI.ProgressData();
                barData.timeLenght = cfg.singTime;
                barData.timeBegin = 0;
                barData.rightToLeft = true;
                m_progress = App.my.uiSystem.progressMgr.PlayItemCasting(barData);
            }
        }

        public override void OnExit(IObject obj, StateType nextState)
        {
            if (m_progress!=null)
            {
                m_progress.Stop();
                m_progress = null;
            }
        }

        public override void OnUpdate(IObject obj)
        {
            if (obj.battle.m_aniMgr.IsAniFinish())
            {
                if (m_info.callback != null)
                    m_info.callback(m_info.para);
                obj.battle.m_stateMgr.SetStateFinish();
            }   
        }
    }
}

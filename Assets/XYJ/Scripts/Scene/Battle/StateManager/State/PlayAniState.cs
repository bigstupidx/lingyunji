using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;

namespace xys.battle
{
    class PlayAniState : IState
    {
        public class PlayAniInfo
        {
            public string aniName;
            public float timeLen;   //持续时间
            public Action<object> callback;
            public object para;
        }

        PlayAniInfo m_info;
        ProgressBase m_progress;
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            m_info = (PlayAniInfo)para;

            obj.battle.m_aniMgr.PlayAni(m_info.aniName, 1.0f, m_info.timeLen);
        }

        public override void OnExit(IObject obj, StateType nextState)
        {
            if (m_progress != null)
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

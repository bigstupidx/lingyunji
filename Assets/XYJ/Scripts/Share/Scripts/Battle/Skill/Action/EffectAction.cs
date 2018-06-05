using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class EffectAction : IAction<EffectActionConfig>
    {
        public override RunType GetRunType()
        {
            return RunType.ClientOnly;
        }


        public override bool OnExcute(ActionInfo info)
        {
#if !COM_SERVER
            IObject addToTarget;
            if (cfg.targetType == EffectTarget.Self)
                addToTarget = info.source;
            else
                addToTarget = info.target;


            if (info.skill == null || !info.skill.IsPlaying() || addToTarget == null)
                return false;

            //延时多长时间销毁
            float timeLenght = 0;
            if (cfg.lastFrame > 0)
                timeLenght = info.skill.m_aniGroup.GetSkillEventGroup().m_speedMul * cfg.lastFrame / AniConst.AnimationFrameRate;
       
            ILoadReference m_load;
            if (!string.IsNullOrEmpty(cfg.warningSearchId))
            {
                m_load = EffectManager.PlayWarning(info.source, addToTarget, SearchActionConfig.Get(cfg.warningSearchId), timeLenght);
                //特效id为空，使用searchid来代替
                addToTarget.battle.m_effectMgr.AddEffect(cfg.warningSearchId, m_load, timeLenght);
            }
    
            else
                m_load = addToTarget.battle.m_effectMgr.AddEffect(cfg.fxid, timeLenght);

            if (m_load == null)
                return false;

             
            //动作结束就销毁
            if(cfg.destroyType == EffectActionConfig.DestroyType.BySkillAni)
            {
                EffectActionLogic p = new EffectActionLogic(m_load);
                info.skill.m_aniGroup.AddActionUpdate(p);
            }
#endif
            return true;
        }

#if !COM_SERVER
        class EffectActionLogic : IActionUpdate
        {
            ILoadReference m_load;
            public EffectActionLogic(ILoadReference load)
            {
                m_load = load;
            }

            //返回true表示行为结束，需要结束动画
            public bool Update()
            {
                return false;
            }

            public void Stop()
            {
                m_load.SetDestroy();
            }
        }
#endif

    }
}

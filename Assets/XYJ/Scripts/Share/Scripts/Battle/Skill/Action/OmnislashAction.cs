using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    //无敌斩
    class OmnislashAction : IAction<OmnislashActionConfig>
    {

        public override RunType GetRunType()
        {
            return RunType.Both;
        }


        public override bool OnExcute(ActionInfo info)
        {
            if (info.skill == null)
                return false;

            OmnislashActionLogic actionUpdate = new OmnislashActionLogic();
            actionUpdate.Begin(info.skill, info.source, info.target, cfg);
            info.skill.m_aniGroup.AddActionUpdate(actionUpdate);
            return true;
        }
    }

    public class OmnislashActionLogic : IActionUpdate
    {
        public OmnislashActionConfig cfg { get; private set; }
        //目标列表
        List<int> m_targetList;
        //当前目标下标
        int m_curTargetIndex;
        IObject m_source;
        float n_nextActiontime;
        int m_curActionCnt;
        SkillLogic m_skill;
        Vector3 m_initPos;


        public bool Begin(SkillLogic skill, IObject source, IObject target, OmnislashActionConfig cfg)
        {
            this.cfg = cfg;
            m_targetList = skill.m_skillTargets;
            m_source = source;
            n_nextActiontime = BattleHelp.timePass;
            m_curActionCnt = 0;
            m_curTargetIndex = 0;
            m_skill = skill;
            m_initPos = source.position;
            //设置结束时间
            skill.SetAniFinishTime(BattleHelp.timePass+ 100);
#if !COM_SERVER
            ActionClient.OmnislashActionBegin(this, source);
#endif
            return true;
        }
        public void Stop()
        {
#if !COM_SERVER
            ActionClient.OmnislashActionStop(this, m_source);
#endif

            //恢复原来坐标
            m_source.SetPosition(m_initPos);
            m_source.battle.m_moveMgr.StopMove();

            m_skill = null;
            m_source = null;
            m_targetList = null;
        }

        IObject GetNextObject()
        {
            if (m_curTargetIndex >= m_targetList.Count)
                m_curTargetIndex = 0;
            int objid = m_targetList[m_curTargetIndex++];
            return BattleHelp.GetAOIObj(m_source, objid);
        }

        public bool Update()
        {
            if(m_targetList == null || m_targetList.Count == 0)
                return true;
            
            //执行下次action
            if (BattleHelp.timePass >= n_nextActiontime)
            {
                n_nextActiontime += cfg.actionInterval;

                IObject target = GetNextObject();
                if (target != null && target.isAlive)
                {
#if !COM_SERVER
                    ActionClient.OmnislashAction(this, m_source, target, target.position);
#else
                    m_source.SetPosition(target.position);
#endif
                    ActionManager.HandleActionListAndSendMsg(m_skill, m_source, target, cfg.actionList,m_source.position,m_source.rotateAngle);
                }

                //达到最大次数
                if (++m_curActionCnt >= cfg.actionMaxCnt)
                    return true;
            }

            return false;
        }
    }
}

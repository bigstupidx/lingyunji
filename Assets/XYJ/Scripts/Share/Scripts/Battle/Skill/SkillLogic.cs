using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.battle
{
    public class SkillLogic
    {
        //播放动画回调
        public Action<SkillAniGroup.AniInfo> m_aniFun;

        public SkillConfig m_cfg { get; private set; }
        public IObject m_source { get; private set; }
        public IObject m_target { get; private set; }
        //释放技能时的目标坐标
        public Vector3 m_skillPoint { get; private set; }

        public List<int> m_skillTargets { get; private set; }

        public SkillAniGroup m_aniGroup { get; private set; }
        bool m_isPlaying;
        //技能等级伤害值
        public SkillDamageConfig skillDamageCfg { get; private set; }

        public SkillLogic()
        {
            m_aniGroup = new SkillAniGroup();
            m_isPlaying = false;
        }

        public void Play(SkillConfig cfg, IObject source, IObject target, List<int> skillTargets, int skilllevel = 1)
        {
            m_skillTargets = skillTargets;
            m_source = source;
            m_target = target;
            m_cfg = cfg;
            m_isPlaying = true;
            if (target != null)
                m_skillPoint = target.position;

            //执行要最后，因为执行里面包含了update
            m_aniGroup.Play(this);

            skillDamageCfg = SkillDamageConfig.Get(skilllevel);

        }

        public SkillConfig GetCurSkill()
        {
            return m_cfg;
        }

        public bool IsPlaying()
        {
            return m_isPlaying;
        }

        //部分action可以设置结束时间
        public void SetAniFinishTime(float finishTime)
        {
#if !COM_SERVER
            //客户端加多一些时间
            finishTime += 0.05f;
#endif
            m_aniGroup.SetAniFinishTime(finishTime);
        }

        //获得动画类型
        public SkillAniConfig.AniType GetAniType()
        {
            if (m_isPlaying)
                return m_aniGroup.GetAniType(this);
            else
                return SkillAniConfig.AniType.Normal;
        }

        public void Stop()
        {
            if (!m_isPlaying)
                return;
            m_skillTargets = null;
            m_isPlaying = false;
            m_aniGroup.Stop(this);
            m_source = null;
            m_target = null;
        }

        public void Update()
        {
            if (!m_isPlaying)
                return;
            if (!m_aniGroup.Update(this))
                m_isPlaying = false;
        }

        public float GetAniTimePass()
        {
            return m_aniGroup.GetAniTimePass();
        }

        public float GetAniTimeFinish()
        {
            return m_aniGroup.m_aniFinishTime;
        }

    }
}

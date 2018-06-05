using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    /// <summary>
    /// 子弹逻辑
    /// </summary>
    public class BulletLogic
    {
        protected enum State
        {
            FireDelay,
            Fire,
            Stop,
        };



        protected Vector3 m_fromPos;
        protected Vector3 m_toPos;
        protected float m_lifeTime;
        protected IObject m_source;
        protected IObject m_target;
        protected BulletActionConfig cfg;
        protected Vector3 m_curPos;
        protected float m_curAngle;
        protected State m_state;

        protected float m_timePass;
        float m_actionInterval;

        public bool Init(BulletActionConfig cfg, IAction.ActionInfo info)
        {
            this.cfg = cfg;

            Vector3 firePos;
            float fireAngle;
            Vector3 toPos;

            float timeLenght;

            if (cfg.firePos == BulletActionConfig.FirePos.BindSource)
            {
                firePos = toPos = info.source.position;
                fireAngle = info.source.rotateAngle;
                timeLenght = cfg.lifeTime;
            }
            else if (cfg.firePos == BulletActionConfig.FirePos.BindTarget)
            {
                firePos = toPos = info.target.position;
                fireAngle = info.target.rotateAngle;
                timeLenght = cfg.lifeTime;
            }
            else
            {
                //确定发射坐标和角度
                switch (cfg.firePos)
                {
                    case BulletActionConfig.FirePos.Target:
                        fireAngle = info.fireAngle;
                        firePos = info.target.position;
                        break;
                    case BulletActionConfig.FirePos.Source:
                        fireAngle = info.fireAngle;
                        firePos = info.firePos;
                        break;
                    case BulletActionConfig.FirePos.SkillTargetPos:
                        if (info.skill == null)
                            return false;
                        fireAngle = info.fireAngle;
                        firePos = info.skill.m_skillPoint;
                        break;
                    default:
                        firePos = Vector3.zero;
                        fireAngle = 0;
                        XYJLogger.LogError(" 子弹发射位置没有定义 " + cfg.firePos);
                        break;
                }

                //发射偏移,偏移应该是相对施法者朝向来的
                if (cfg.firePosOff.Length == 3)
                {
                    firePos += BattleHelp.RotateAngle(new Vector3(cfg.firePosOff[0], cfg.firePosOff[1], cfg.firePosOff[2]), fireAngle);
                }

                //目的地坐标和子弹时间
                switch (cfg.targetPos)
                {
                    //没有目标
                    case BulletActionConfig.TargetPos.NoTarget:
                        int angle = ((int)(fireAngle + cfg.flyAngle)) % 360;
                        Vector3 forward = BattleHelp.Angle2Vector(angle);
                        toPos = firePos + forward * cfg.speed * cfg.lifeTime;
                        timeLenght = cfg.lifeTime;
                        break;

                    default:
                        if (cfg.targetPos == BulletActionConfig.TargetPos.SkillTargetPos)
                        {
                            if (info.skill == null)
                                return false;
                            toPos = info.skill.m_skillPoint;
                        }
                        else
                            toPos = info.target.position;
                        timeLenght = GetMoveTimeLenght(firePos, toPos);
                        break;
                }
            }


            m_source = info.source;
            m_target = info.target;
            m_curPos = m_fromPos = firePos;
            m_toPos = toPos;
            m_lifeTime = timeLenght;
            m_curAngle = fireAngle;

            ChangeState(State.FireDelay);
            m_timePass = 0;
            m_actionInterval = 0;

            return OnPlay();
        }

        float GetMoveTimeLenght(Vector3 firePos, Vector3 toPos)
        {
            float dis = BattleHelp.GetDistance(firePos, toPos);
            float timeLenght;

            //跟随子弹在发射的时候就确定了命中时间
            if (cfg.follow != 0)
            {
                if (cfg.speed == 0)
                    timeLenght = 0;
                else
                    timeLenght = dis / cfg.speed;
            }
            //有速度
            else if (cfg.speed != 0)
            {
                //曲线运动 2*a*s=vt*vt-v0*v0
                if (cfg.moveType == BulletActionConfig.MoveType.Parabolic)
                {
                    float a = cfg.paraAcc;
                    if (a != 0)
                    {
                        float v0 = cfg.speed;
                        float vt = Mathf.Sqrt(2 * a * dis + v0 * v0);
                        timeLenght = (vt - v0) / a;
                    }
                    else
                        timeLenght = dis / cfg.speed;
                    if (timeLenght <= 0)
                    {
                        XYJLogger.LogError("曲线运动时间小于0 id=" + cfg.id);
                        timeLenght = 0;
                    }
                }
                //直线
                else
                    timeLenght = dis / cfg.speed;
            }
            else
                timeLenght = cfg.lifeTime;
            return timeLenght;
        }


        protected virtual bool OnPlay() { return true; }
        protected virtual void OnFinish() { }
        protected virtual void OnUpdate() { }


        public bool IsFinish()
        {
            return m_state == State.Stop;
        }
        void ChangeState(State state)
        {
            if (state == State.Fire)
            {
                if (cfg.createActionList.Count > 0)
                    ActionManager.HandleActionListAndSendMsg(null, m_source, m_target, cfg.createActionList, m_curPos, m_curAngle);
            }
            else if (state == State.Stop)
            {
                OnFinish();
                if (cfg.destroyActionList.Count > 0)
                    ActionManager.HandleActionListAndSendMsg(null, m_source, m_target, cfg.destroyActionList, m_curPos, m_curAngle);
            }

            m_state = state;
        }


        public void Update(float deltaTime)
        {
            m_timePass += deltaTime;
            OnUpdate();
            switch (m_state)
            {
                //发射延时
                case State.FireDelay:
                    if (m_timePass >= cfg.fireDelay)
                    {
                        ChangeState(State.Fire);
                        m_timePass = m_timePass - cfg.fireDelay;
                    }
                    break;
                case State.Fire:
                    if (m_timePass >= m_lifeTime)
                    {
                        ChangeState(State.Stop);
                        return;
                    }
                    else if (cfg.intervalActionList.Count > 0)
                    {
                        //触发事件
                        m_actionInterval += deltaTime;
                        if (m_actionInterval > cfg.interval)
                        {
                            m_actionInterval = 0;

                            if (cfg.firePos == BulletActionConfig.FirePos.BindSource)
                                m_curPos = m_source.position;
                            else if (cfg.firePos == BulletActionConfig.FirePos.BindTarget)
                                m_curPos = m_target.position;
                            //移动
                            else if (cfg.speed != 0)
                            {
                                //跟随目标
                                if (cfg.targetPos == BulletActionConfig.TargetPos.Target && cfg.follow != 0 && m_target != null && m_target.isAlive)
                                    m_toPos = m_target.position;

                                m_curPos = m_fromPos + (m_toPos - m_fromPos) * m_timePass / m_lifeTime;
                            }

                            ActionManager.HandleActionListAndSendMsg(null, m_source, m_target, cfg.intervalActionList, m_curPos, m_curAngle);
                        }
                    }
                    break;
            }
        }
    }
}

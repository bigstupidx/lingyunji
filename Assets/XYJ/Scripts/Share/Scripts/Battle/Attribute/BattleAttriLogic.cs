
using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    //远程控制逻辑
    public class BattleAttriLogic
    {
        protected IObject m_obj;


        public BattleAttriLogic(IObject obj)
        {
            m_obj = obj;
        }
        public virtual void OnUpdate()
        {

        }

        //初始化数据
        public virtual void InitData()
        {

        }

        public virtual void AddHp(int value)
        {
            XYJLogger.LogError("非本地控制玩家不能调用");
        }

        public virtual bool Huti_Damage(int damage)
        {
            XYJLogger.LogError("非本地控制玩家不能调用");
            return false;
        }

        public virtual void ChangeToNextPosture()
        {
            XYJLogger.LogError("非本地控制玩家不能调用");
        }
        public virtual void SetBattleState(bool state)
        {
            XYJLogger.LogError("非本地控制玩家不能调用");
        }

        public virtual void SetState(ObjectState state)
        {
            XYJLogger.LogError("非本地控制玩家不能调用");
        }

        public virtual bool SetPosture(int toPosture, float timeLenght = -1)
        {
            XYJLogger.LogError("非本地控制玩家不能调用");
            return false;
        }
        public virtual void Zhenqi_Add(int value)
        {
            XYJLogger.LogError("非本地控制玩家不能调用");
        }

        //设置的速度是基础速度，会受buff影响
        public virtual float SetSpeed(float speed)
        {
            XYJLogger.LogError("非本地控制玩家不能调用");
            return 0;
        }

    }

    //本地控制逻辑
    public class BattleAttriLogicLocal : BattleAttriLogic
    {
        CheckInterval m_interval = new CheckInterval();
        PostureConfig postureCfg { get { return m_obj.battle.m_attrMgr.postureCfg; } }

        //进入破体后有一段免疫破体伤害时间
        float m_hutiNoiDamageTime;
        //姿态结束时间
        float m_postureTimeFinish = float.MaxValue;
        //战斗状态结束时间
        float m_battleStateFinishTime;

        public BattleAttriLogicLocal(IObject obj) : base(obj) { }

        public override void OnUpdate()
        {
            //每秒恢复一次
            if (m_interval.Check(1.0f))
            {
                //姿态结束
                if (BattleHelp.timePass > m_postureTimeFinish)
                    ChangeToNextPosture();

                //玩家才有
                if (m_obj.type == NetProto.ObjectType.Player)
                {
                    //非战斗状态
                    if (!m_obj.battle.m_attrMgr.battleState)
                    {
                        //真气恢复
                        int zhenqi = m_obj.zhenQiValue;
                        if (zhenqi < kvBattle.zhenqiMax)
                            Zhenqi_Add(kvBattle.zhenqiRecoverSpeed);
                    }
                    //战斗状态
                    else
                    {
                        //只有玩家才会根据时间结束战斗状态
                        if (BattleHelp.timePass > m_battleStateFinishTime && !postureCfg.logic.forceInBattle) //强制战斗状态时不能退出战斗状态
                            SetBattleState(false);
                    }
                }


                Huti_Update();
            }
        }

        //设置的速度是基础速度，会受buff影响
        public override float SetSpeed(float speed)
        {
            if (speed == 0)
                speed = m_obj.cfgInfo.speed;
            m_obj.speedValue = (int)(speed * 100);
            return speed;
        }
        public override void SetState(ObjectState state)
        {
            m_obj.stateValue = (int)state;
        }
        public override void AddHp(int value)
        {
            if (value == 0)
                return;
            int cur = m_obj.hpValue + value;
            if (cur <= 0)
            {
                //角色死亡
                m_obj.hpValue = 0;
                m_obj.SetDead();
                return;
            }
            else
            {
                if (cur > m_obj.maxHpValue)
                    cur = m_obj.maxHpValue;
                m_obj.hpValue = cur;
            }
        }


        #region 护体
        public override bool Huti_Damage(int damage)
        {
            //没有护体
            if (m_obj.maxHuTiValue == 0)
                return false;
            //免疫护体伤害
            if (BattleHelp.timePass < m_hutiNoiDamageTime)
                return false;

            int curhuTi = m_obj.huTiValue;
            curhuTi -= damage;
            if (curhuTi <= 0)
                curhuTi = 0;
            m_obj.huTiValue = (ushort)curhuTi;
            //当前是护体状态,护体减少到0，进入破体状态
            if (m_obj.huTiStateValue == (ushort)HutiState.Huti)
            {
                //进入破体
                if (curhuTi <= 0)
                {
                    m_hutiNoiDamageTime = BattleHelp.timePass + kvBattle.hutiBreakNoDamageTime;
                    m_obj.huTiStateValue = (ushort)HutiState.Poti;
                    return true;
                }
            }
            return false;
        }

        void Huti_Update()
        {
            //恢复护体值
            int curHuti = m_obj.huTiValue;
            int maxHuti = m_obj.maxHuTiValue;
            if (curHuti != maxHuti)
            {
                if (m_obj.huTiStateValue == (ushort)HutiState.Poti)
                {
                    curHuti += (int)kvBattle.hutiRecoverSpeedWhenBreak;
                    //进入护体状态
                    if (curHuti >= (int)(kvBattle.hutiEnterStatusMul * m_obj.maxHuTiValue))
                        m_obj.huTiStateValue = (ushort)HutiState.Huti;
                }

                else
                {
                    if (m_obj.battle.m_attrMgr.battleState)
                        curHuti += (int)kvBattle.hutiRecoverSpeedWhenBattle;
                    else
                        curHuti += (int)kvBattle.hutiRecoverSpeedWhenIdle;
                }
                if (curHuti >= maxHuti)
                    curHuti = maxHuti;
                m_obj.huTiValue = (ushort)curHuti;

            }
        }
        #endregion

        #region 姿态


        //切换后续姿态
        public override void ChangeToNextPosture()
        {
            if (postureCfg.logic.endToPosture == 0)
            {
                XYJLogger.LogError(string.Format("姿态{0} 时间到了没有后续姿态 ", postureCfg.id));
                return;
            }

            SetPosture(postureCfg.logic.endToPosture);
        }

        public override void SetBattleState(bool state)
        {
            if (m_obj.stateValue != (int)ObjectState.Battle && m_obj.stateValue != (int)ObjectState.Idle)
                return;

            m_battleStateFinishTime = BattleHelp.timePass + kvBattle.breakBattleTime;
            if (m_obj.battle.m_attrMgr.battleState != state)
            {
                if (state)
                    m_obj.stateValue = (int)ObjectState.Battle;
                else
                {
                    m_obj.stateValue = (int)ObjectState.Idle;
                    //该姿态只在战斗状态
                    if (postureCfg.logic.isInBattle)
                        ChangeToNextPosture();
                }
            }
        }

        public override bool SetPosture(int toPosture, float timeLenght = -1)
        {
            if (postureCfg.id != toPosture)
            {
                PostureConfig oldCfg = postureCfg;
                PostureLogicConfig newLogicCfg = PostureLogicConfig.GetKey(toPosture);

                //部分姿态真气不够,不能切换
                if (newLogicCfg.mpOutEnd && m_obj.zhenQiValue <= 0)
                    return false;

                m_obj.postureValue = toPosture;

                if (timeLenght < 0)
                    m_postureTimeFinish = float.MaxValue;
                else
                    m_postureTimeFinish = BattleHelp.timePass + timeLenght;
                return true;
            }
            return false;
        }
        #endregion

        #region 真气
        public override void Zhenqi_Add(int value)
        {
            if (value == 0)
                return;
            int cur = m_obj.zhenQiValue;
            cur += value;
            if (cur <= 0)
            {
                cur = 0;
                if (postureCfg.logic.mpOutEnd)
                    ChangeToNextPosture();
            }
            else if (cur > kvBattle.zhenqiMax)
                cur = kvBattle.zhenqiMax;
            m_obj.zhenQiValue = (ushort)cur;
        }
        #endregion
    }

}
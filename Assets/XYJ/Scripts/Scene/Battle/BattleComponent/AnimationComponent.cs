using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class AnimationCompoment : IBattleComponent, IBattleUpdate
    {
        class AniInfo
        {
            public string m_name;
            public float m_timeLenght;
            public float m_speed = 1;
            public bool m_crossFade;
            public float m_normalizedTime = 0;
            public Action<object> m_finishCall;
            public object m_finishCallPara;
            public float m_endNormalizedTime = 1.0f;
        }

        //播放队列
        Queue<AniInfo> m_playQueue = new Queue<AniInfo>();
        AniInfo m_curAniInfo = new AniInfo();
        //播放的动画时间记录
        float m_curPlayAniTimeRecord;
        bool m_curAniIsFinish;
        bool m_idleStateNoPlayAni;
        IObject m_obj;


        public void OnAwake(IObject obj)
        {
            m_obj = obj;
        }

        public void OnStart()
        {
            m_obj.eventSet.Subscribe<AttributeChange>(NetProto.AttType.AT_Speed, (p) => { RefreshMoveAniRate(); });
        }


        public void OnEnterScene()
        {

        }
        public void OnExitScene()
        {

        }

        public void OnDestroy()
        {
            m_obj = null;
        }


        public void OnUpdate()
        {
            if (!m_curAniIsFinish)
            {
                //动画结束
                if (Time.realtimeSinceStartup - m_curPlayAniTimeRecord >= m_curAniInfo.m_timeLenght)
                {
                    if (m_curAniInfo.m_finishCall != null)
                        m_curAniInfo.m_finishCall(m_curAniInfo.m_finishCallPara);
                    if (m_playQueue.Count > 0)
                        PlayNextQueued();
                    else
                        m_curAniIsFinish = true;
                }
            }
        }

        //播放动画,调用该接口会清除之前的Queue;
        public void PlayAni(string name, float speed = 1.0f, float playLenght = 0, float normalizedTime = 0, bool crossfade = false, Action<object> finishFun = null, object para = null, float endNormalizedTime = -1.0f)
        {
            ClearQueued();
            PlayQueued(name, speed, playLenght, normalizedTime, crossfade, finishFun, para, endNormalizedTime);
        }

        //获得移动动作速率
        float GetMoveAniRate( string aniName )
        {
            float moveSpeed = m_obj.battle.speed;
            float cfgSpeed = MoveRateConfig.GetMoveRate(m_obj.cfgInfo.model, aniName);
            if (cfgSpeed == 0)
                return 1;
            return moveSpeed / cfgSpeed;
        }

        //刷新动画速度
        public void RefreshMoveAniRate( )
        {
            if(m_obj.battle.m_stateMgr.m_curStType == StateType.Move)
            {
                SetCurSpeed(GetMoveAniRate(m_curAniInfo.m_name));
            }
        }

        //先插入inserAni动画,再根据状态播放动画
        public void PlayStateAni(string inserAni = null)
        {
            StateType state = m_obj.battle.m_stateMgr.m_curStType;
            bool battleState = m_obj.battle.m_attrMgr.battleState;
            string stName;
            float stNameSpeed = 1.0f;
            if (state == StateType.Idle)
                stName = battleState ? m_obj.battle.m_attrMgr.postureCfg.battleIdle : m_obj.battle.m_attrMgr.postureCfg.normalIdle;
            else if (state == StateType.Move)
            {
                if(m_obj.battle.m_stateMgr.isFastRun)
                    stName = m_obj.battle.m_attrMgr.postureCfg.fastRun;
                else
                    stName = battleState ? m_obj.battle.m_attrMgr.postureCfg.battleRun : m_obj.battle.m_attrMgr.postureCfg.normalRun;
                stNameSpeed = GetMoveAniRate(stName);
            }
            else
                return;

            //先播切换动作，再接待机
            if (!string.IsNullOrEmpty(inserAni) && state == StateType.Idle)
            {
                m_obj.battle.m_aniMgr.PlayAni(inserAni);
                m_obj.battle.m_aniMgr.PlayQueued(stName, stNameSpeed);
            }
            else
            {
                //播放相同动画就不需要了
                if (m_curAniInfo != null && m_curAniInfo.m_name == stName && m_curAniInfo.m_speed == stNameSpeed)
                    return;

                m_obj.battle.m_aniMgr.PlayAni(stName, stNameSpeed);
            }
        }

        public int GetCurTrueAniFrame()
        {
            return m_obj.battle.actor.m_partManager.GetCurTrueAniFrame();
        }

        public void SetCurSpeed(float speed)
        {
            if (m_obj.battle.actor.rootAni == null)
                return;
            m_obj.battle.actor.m_partManager.SetAnimSpeed(speed);
        }

        public bool IsAniLoop(string name)
        {
            AnimationWrap ani = m_obj.battle.actor.rootAni;
            if (ani == null)
                return false;
            else
                return ani.IsLoop(name);
        }

        //根据时长获取动画速度
        public float GetSpeedByLenght(string name, float lenght)
        {
            float aniLenght = GetAniLenght(name);
            return aniLenght / lenght;
        }


        //playLenght=0表示时长为动画长度
        //先调用ClearQueued，再调用该接口会立马播放动画
        public void PlayQueued(string name, float speed = 1.0f, float timeLenght = 0, float normalizedTime = 0, bool crossfade = false, Action<object> finishFun = null, object para = null, float endNormalizedTime = 1.0f)
        {
            if (speed == 0)
            {
                Debug.LogError("动画速度不能为0 " + name);
                speed = 0.00001f;
            }

            if (timeLenght == 0)
                timeLenght = GetAniLenght(name) / speed;
            m_playQueue.Enqueue(new AniInfo() { m_name = name, m_speed = speed, m_timeLenght = timeLenght, m_normalizedTime = normalizedTime, m_finishCall = finishFun, m_finishCallPara = para, m_endNormalizedTime = endNormalizedTime });
            if (IsAniFinish())
                PlayNextQueued();
        }

        //格挡时插入一个动画，动画结束后恢复格挡动画
        public void PlayBlockAni(string name)
        {
            AniInfo curAniInfo = m_curAniInfo;
            PlayAni(name);
            curAniInfo.m_timeLenght -= GetAniLenght(name);
            m_playQueue.Enqueue(curAniInfo);
        }

        //
        public void ClearQueued()
        {
            m_curAniIsFinish = true;
            m_playQueue.Clear();
        }

        public float GetAniLenght(string name)
        {
            if (m_obj.battle.actor.rootAni == null)
                return 0;
            return m_obj.battle.actor.m_partManager.GetAnimLength(name);
        }

        public bool IsAniFinish()
        {
            return m_curAniIsFinish;
        }

        //待机状态是否需要播放动画
        public bool IsIdleStateNeedPlayAni()
        {
            if (m_idleStateNoPlayAni)
            {
                m_idleStateNoPlayAni = false;
                return false;
            }
            else
                return true;
        }

        //设置待机状态不播放动画
        public void SetIdleStateNoPlayAni()
        {
            m_idleStateNoPlayAni = true;
        }

        void PlayNextQueued()
        {
            if (m_obj.battle.actor.rootAni == null)
                return;
            AniInfo p = m_curAniInfo = m_playQueue.Dequeue();
            m_curPlayAniTimeRecord = Time.realtimeSinceStartup;
            m_curAniIsFinish = false;
            m_obj.battle.actor.m_partManager.PlayAnim(p.m_name, p.m_speed, p.m_normalizedTime, false, p.m_crossFade, 0, -1, p.m_endNormalizedTime);

            if (m_obj.battle.actor.mono.m_testAni)
                Debug.LogError(string.Format("name={0} normalizedTime={1}", p.m_name, p.m_normalizedTime));
        }
    }
}
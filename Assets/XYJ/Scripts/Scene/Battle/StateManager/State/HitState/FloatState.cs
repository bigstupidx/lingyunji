using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class FloatState : IState
    {
        enum State
        {
            Rise,       //上升
            SkyStay,    //浮空
            SkyHit,     //空中受击
            Falling,    //下降
            Stop,
        }

        const float RiseTime = 0.267f;
        const float FallingTime = 0.267f;


        //空中停留时间
        float m_skyStayTime;
        State m_state;
        float m_stateFnishTime;



        void SetState(State state, float time)
        {
            m_state = state;
            m_stateFnishTime = Time.timeSinceLevelLoad + time;
        }

        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            m_skyStayTime = obj.battle.m_stateMgr.stateTimeLeft;
            float toSkyTime;

            obj.battle.actor.m_modelOffset.SetTrans(obj.battle.actor.m_modelTrans.transform);
            //空中再浮空
            if (lastState == StateType.Float)
            {
                toSkyTime = 0.1f;
                obj.battle.m_aniMgr.PlayQueued(AniConst.SkyHit);
                SetState(State.SkyHit, toSkyTime);
                //受击时会再拉到空中
                obj.battle.actor.m_modelOffset.HitToSky(obj, toSkyTime);
            }
            else
            {
                toSkyTime = RiseTime;
                float aniSpeed = obj.battle.m_aniMgr.GetSpeedByLenght(AniConst.UpToSky, toSkyTime);
                obj.battle.m_aniMgr.PlayAni(AniConst.UpToSky, aniSpeed);
                SetState(State.Rise, toSkyTime);
                obj.battle.actor.m_modelOffset.HitToSky(obj, toSkyTime);
            }

            //空中停留时间,需要扣除上升和下降时间
            m_skyStayTime = m_skyStayTime - toSkyTime - FallingTime;
        }

        public override void OnExit(IObject obj, StateType nextState)
        {
            if (nextState != StateType.Float
             && nextState != StateType.KnockDown)
                obj.battle.actor.m_modelOffset.ResetHeight(obj);
        }

        public override void OnUpdate(IObject obj)
        {
            if (Time.timeSinceLevelLoad - m_stateFnishTime >= 0)
            {
                //上升 或者 空中受击结束
                if (m_state == State.Rise || m_state == State.SkyHit)
                {
                    float aniSpeed = obj.battle.m_aniMgr.GetSpeedByLenght(AniConst.Sky, m_skyStayTime);
                    obj.battle.m_aniMgr.PlayAni(AniConst.Sky, aniSpeed, m_skyStayTime);
                    SetState(State.SkyStay, m_skyStayTime);
                }
                //空中停留
                else if (m_state == State.SkyStay)
                {
                    float fallTime = FallingTime;
                    float aniSpeed = obj.battle.m_aniMgr.GetSpeedByLenght(AniConst.Sky, fallTime);
                    obj.battle.m_aniMgr.PlayAni(AniConst.FallFromSky, aniSpeed);
                    SetState(State.Falling, fallTime);
                    obj.battle.actor.m_modelOffset.FallToGround(fallTime);
                }

                //下降
                else if (m_state == State.Falling)
                {
                    SetState(State.Stop, 0);
                }
            }
        }
    }

}


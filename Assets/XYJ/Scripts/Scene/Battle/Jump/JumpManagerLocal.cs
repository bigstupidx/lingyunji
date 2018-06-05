using UnityEngine;
using System.Collections;
using Config;
using System.Collections.Generic;


//轻功移动逻辑
namespace xys.battle
{
    class JumpManagerLocal : JumpManagerBase
    {
        enum FovState
        {
            Default,
            Change,
            Recover,
        }
        FovState m_fovState;

        //有请求下一段轻功
        protected SkillConfig m_requestNextJump;
        //轻功开始时间
        float m_jumpBeginTime;
        public void JumpCache(SkillConfig skillCfg)
        {
            if (!IsCanJump())
                return;

            //开启轻功
            if (m_state == State.Stop)
            {
                m_requestNextJump = null;
            }
            //已经在轻功中了
            else
            {
                //记录下段轻功,timeBetinInput小于0表示该段动作不能接输入
                if (cfg.timeBetinInput >= 0 && BattleHelp.timePass - m_jumpBeginTime >= cfg.timeBetinInput)
                    m_requestNextJump = skillCfg;
            }
        }

        protected override void OnUpdateLogic()
        {
            //开启下一段轻功,需要在设置着地前，不然卡了会切不出来
            if (m_requestNextJump != null && m_curAniFrame >= cfg.responseFrame && m_state == State.Fly)
            {
                App.my.battleProtocol.Jump_Request(m_requestNextJump, m_obj);
                m_requestNextJump = null;
                return;
            }

            //fov修改
            UpdateFov();

            //移动
            UpdateMove();

            //落地结束
            if (m_state == State.Land )
            {
                //落地动作若干时间后可以移动
                if (m_curAniFrame >= cfg.responseOperationFrame && App.my.input.IsMoving())
                {
                    m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
                    if (cfg.landToFastRun)
                        m_obj.battle.m_stateMgr.SetFastRun(true);                    
                    return;
                }

                //没有操作则动作完成就进入待机
                if (m_obj.battle.m_aniMgr.IsAniFinish())
                {
                    m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
                    return;
                }
            }

            //摇杆操作
            if ( m_state == State.Land || !App.my.input.IsMoving())
                return;
            //设置朝向
            float toAngle = BattleHelp.Vector2Angle(App.my.input.GetMoveWay());
            if (cfg.glidingRotation != 0)
                toAngle = Mathf.LerpAngle(m_obj.rotateAngle, toAngle, cfg.glidingRotation * Time.deltaTime);
            m_obj.SetRotate(toAngle);
            //设置摇杆速度
            if (cfg.inputSpeedZ > 0 && m_speed.z == 0)
            {
                m_speed.z = cfg.inputSpeedZ;
                //广播速度
                App.my.battleProtocol.Jump_SendSpeed(m_obj, m_speed.z);
            }
        }

        protected void UpdateMove()
        {
            //移动
            Vector3 move = BattleHelp.RotateAngle(m_speed * Time.deltaTime, m_obj.rotateAngle);
            CollisionFlags flg = m_obj.battle.actor.CCMove(move);
            //下降碰到障碍则停止
            if (m_state == State.Fly)
            {
                //动作过程中不着地
                if (cfg.noFallingWhenAni && m_curAniFrame < cfg.endFrame)
                    return;

                if (m_speed.y < 0 && (flg&CollisionFlags.CollidedBelow)!=0)
                {
                    PlayJumpLand();
                    App.my.battleProtocol.Request_JumpSendLand(m_obj);
                }
            }
        }


        protected override bool IsCanJump() 
        {
            if (m_obj.battle.m_buffMgr.IsFlag(BuffManager.Flag.NoSkill))
                return false;
            if(m_state == State.Stop)
                return true;
            
            return true;
        }

        protected override void OnPlayNextAni()
        {
            //非强制速度时按着摇杆有速度
            if (!cfg.forceSpeedZ && App.my.input.IsMoving())
            {
                m_speed.z = cfg.speedZ;
                //广播速度
                App.my.battleProtocol.Jump_SendSpeed(m_obj, cfg.speedZ);
            }
                
            m_fovState = FovState.Default;
            App.my.cameraMgr.SetPlayerHeight(cfg.changeCameraHeight + App.my.cameraMgr.m_defaultPlayerHeight);
        }

        protected override void OnStop()
        {
            m_requestNextJump = null;
            ChangeFov(App.my.cameraMgr.m_defaultFov, Mathf.Min(15,cfg.fovRecoverLastFrame));
            App.my.cameraMgr.SetPlayerHeight(App.my.cameraMgr.m_defaultPlayerHeight);
        }

        protected override void OnPlay()
        {
            m_requestNextJump = null;
            m_jumpBeginTime = BattleHelp.timePass;
        }

        void UpdateFov()
        {
            int curFrame = m_curAniFrame;
            if (curFrame >= cfg.fovStarFrame && m_fovState == FovState.Default)
            {
                ChangeFov( cfg.toFov,cfg.fovLastFrame);
                m_fovState = FovState.Change;
            }
            else if (curFrame >= cfg.fovRecoverStarFrame && m_fovState == FovState.Change)
            {
                ChangeFov(App.my.cameraMgr.m_defaultFov, cfg.fovRecoverLastFrame);
                m_fovState = FovState.Recover;
            }
        }

        //修改Fov
        private void ChangeFov( float tofov,int lastFrame)
        {
            if (tofov == 0 || lastFrame == 0)
                return;

            float tempTime = (float)lastFrame / AniConst.AnimationFrameRate;
            float tempSpeed = Mathf.Abs((tofov - App.my.cameraMgr.m_mainCamera.fieldOfView) / tempTime);
            App.my.cameraMgr.ChangeCameraFov(tofov, tempSpeed, float.MaxValue);
        }

        void SetInputSpeed( float speed )
        {

        }
    }
}

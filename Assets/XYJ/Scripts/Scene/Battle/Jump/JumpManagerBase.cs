using UnityEngine;
using System.Collections;
using Config;
using System.Collections.Generic;


//轻功移动逻辑
namespace xys.battle
{
    public class JumpManagerBase : IBattleComponent, IBattleUpdate
    {
        public enum State
        {
            Stop,
            Fly,        //飞行
            Land,       //着地
        }


        protected IObject m_obj;
        protected State m_state;
        //每段动画时间
        protected float m_aniBeginTime;
        //是否播放了下落动画
        bool m_isPlayFallingAni;
        //当前运动速度
        protected Vector3 m_speed;
        protected JumpConfig cfg;

        List<JumpConfig> m_cfgList;
        int m_aniCfgId;


        //当前动画帧数
        protected int m_curAniFrame { get { return cfg.beginFrame + (int)((BattleHelp.timePass - m_aniBeginTime) * AniConst.AnimationFrameRate); } }


        protected virtual void OnUpdateLogic() { }
        protected virtual bool IsCanJump() { return true; }
        protected virtual void OnPlayNextAni() { }
        protected virtual void OnStop() { }
        protected virtual void OnPlay() { }


        public void OnAwake(IObject obj) { m_obj = obj; }
        public void OnStart() { }
        public void OnDestroy() { m_obj = null; }
        public void OnEnterScene()
        {

        }
        public void OnExitScene()
        {

        }
        public void OnUpdate()
        {
            if (m_state == State.Stop)
                return;
            if (m_obj.battle.m_stateMgr.m_curStType != StateType.Jump)
                return;

            if (m_state == State.Fly)
            {
                //动画播放结束,没有下个动画,接下落动作
                if (m_curAniFrame >= cfg.endFrame && !PlayNextAni() && !m_isPlayFallingAni)
                {
                    m_isPlayFallingAni = true;
                    m_obj.battle.m_aniMgr.PlayAni(cfg.fallingAni);
                }
            }

            SpeedUpdate(Time.deltaTime);
            OnUpdateLogic();
        }


        public void PlayJump(SkillConfig skillCfg)
        {
            m_cfgList = JumpConfig.GetGroupBykey(skillCfg.aniid);
            m_aniCfgId = -1;
            OnPlay();
            PlayNextAni();
            ChangeState(State.Fly);
        }

        public void Stop()
        {
            ChangeState(State.Stop);
            OnStop();
        }

        //着地
        public void PlayJumpLand()
        {
            ChangeState(State.Land);
                PlayNextAni(true);
        }

        protected void ChangeState(State state)
        {
            m_state = state;
        }

        //播放下一个动画
        bool PlayNextAni(bool playLand = false)
        {
            if (playLand)
            {
                if(cfg.landAnis == null || cfg.landAnis.Length == 0)
                {
                    Debug.LogError(string.Format("找不到着地动画 key={0} ani={1}=",cfg.key,cfg.aniFly));
                    return false;
                }

                cfg = JumpConfig.GetGroupBykey(cfg.landAnis[0])[0];
                m_speed = Vector3.zero;
            }
            else
            {
                if (m_aniCfgId >= m_cfgList.Count - 1)
                    return false;
                cfg = m_cfgList[++m_aniCfgId];
            }

            SpeedInit();

  
            //如果是重复的动画则不用重新播放了，策划保证每段动画帧数是连贯的
            if (m_aniCfgId == 0 || cfg.aniFly != m_cfgList[m_aniCfgId - 1].aniFly)
                m_obj.battle.m_aniMgr.PlayAni(cfg.aniFly);
            m_isPlayFallingAni = false;
            m_aniBeginTime = BattleHelp.timePass;
            OnPlayNextAni();
            return true;
        }


        //速度初始化
        void SpeedInit()
        {
            m_speed.y = cfg.speedY;

            if (cfg.forceSpeedZ)
                m_speed.z = cfg.speedZ;
        }

        //更新速度
        void SpeedUpdate(float deltaTime)
        {
            if (cfg.accY != 0)
                m_speed.y -= cfg.accY * deltaTime;
            if (cfg.accZ != 0)
            {
                m_speed.z += cfg.accZ * deltaTime;
                if (m_speed.z < 0)
                    m_speed.z = 0;
            }
        }
    }
}
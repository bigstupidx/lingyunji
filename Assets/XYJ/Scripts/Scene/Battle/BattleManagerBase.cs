using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class BattleManagerBase : BattleManager
    {
        public Transform m_root { get { return actor == null ? null : actor.m_rootTrans; } }


        public ActorComponent       actor { get; private set; }
        public AnimationCompoment   m_aniMgr { get; private set; }
        public JumpManagerBase      m_jumpMgr { get; private set; }
        //特效管理
        public ObjectEffectManager  m_effectMgr { get; private set; }

        public BattleManagerBase(IObject obj):base(obj)
        {

        }

        //设置由客户端控制ai
        public void SetAiByClient(bool isAIByLocal)
        {
            m_isAiByLocal = isAIByLocal;
        }

 
        protected override void InitComponent()
        {
            m_attrMgr = AddCompoment(new BattleAttriMgr());
            m_effectMgr = AddCompoment(new ObjectEffectManager());
            m_moveMgr = AddCompoment(ManagerCreate.CreateMoveManager(m_obj));
            actor = AddCompoment(new ActorComponent());
            m_aniMgr = AddCompoment(new AnimationCompoment());
            m_stateMgr = AddCompoment(new StateManagerBase());
            m_skillMgr = AddCompoment(ManagerCreate.CreateSkillManager(m_obj));
            m_targetMgr = AddCompoment(ManagerCreate.CreateTargetManager(m_obj));
            m_buffMgr = AddCompoment(new BuffManagerBase());
            m_jumpMgr = AddCompoment(ManagerCreate.CreateJumpManager(m_obj));
            m_ai = AddCompoment( new AIManagerBase());
        }


        public override void Destroy()
        {
            if (m_stateMgr.m_curStType == StateType.Destroy)
                return;

            //正在播放死亡动作不要删除模型
            if (m_obj.root != null && m_stateMgr.m_curStType == StateType.Dead)
                GameObject.Destroy(m_obj.root.gameObject);

            base.Destroy();
        }

        //能否被选中(点击或者攻击)
        public bool IsCanSelect()
        {
            return true;
        }

        public bool IsCanMove()
        {
            return !m_buffMgr.IsFlag(BuffManager.Flag.NoMove);
        }

        public virtual IObject GetTarget()
        {
            return m_targetMgr.target;
        }

        public void OnFinishSkill()
        {
            if (actor.m_partManager != null && actor.m_partManager.m_aniEffect != null)
                actor.m_partManager.m_aniEffect.FinishSkill();
        }

        #region 提供给外部的接口，会改变状态
        //寻路到制定位置
        public bool State_PathToPos(Vector3 toPos, Action<object> callback = null, object para = null, float speed = 0)
        {
            m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
            m_moveMgr.SetMoveByPath(toPos, callback, para, speed);
            return true;
        }


        //吟唱行为
        public void State_Sing(int singid, Action<object> callback = null, object para = null)
        {
            m_obj.battle.m_stateMgr.ChangeHitState(battle.StateType.Sing, new SingState.SingInfo() { singid = 1, callback=callback,para=para});
        }

        //播放动画,timeLen默认0表示动画播放完就结束
        public void State_PlayAni(string ani,float timeLen=0, Action<object> callback = null, object para = null)
        {
            m_obj.battle.m_stateMgr.ChangeHitState(battle.StateType.PlayAni, new PlayAniState.PlayAniInfo() { aniName = ani, timeLen = timeLen, callback = callback, para = para });
        }
        #endregion
    }

}

using xys.battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
namespace xys.battle
{

   public static class BulletLogicCreate
   {
       public static BulletLogicClient Create(BulletActionConfig cfg)
       {
           if (cfg.firePos == BulletActionConfig.FirePos.BindSource || cfg.firePos == BulletActionConfig.FirePos.BindTarget)
               return new BulletMoveBind();
           else if (cfg.moveType == BulletActionConfig.MoveType.Line )
               return new BulletMoveLine();
           else
               return new BulletMoveParabolic();
       }
   }


    public class BulletLogicClient : BulletLogic
    {
       protected  PrefabsLoadReference m_loadRef = new PrefabsLoadReference(true);
       protected Vector3 m_goFromPos;
       protected Vector3 m_goToPos;


       protected GameObject m_go { get { return m_loadRef.m_go; } }
       Transform m_followBone;
        protected override bool OnPlay()
        {         
            Transform trans = null;
           
            if(!string.IsNullOrEmpty(cfg.FirePosBone))
            {
                Transform fireRoot = null;
                //只要填了骨骼名字就会取发射角色的骨骼,所以子弹发射的子弹不能填骨骼名字
                if (cfg.firePos == BulletActionConfig.FirePos.Source || cfg.firePos == BulletActionConfig.FirePos.BindSource)
                {
                    fireRoot = m_source.root;
                    if (BattleHelp.GetDistance(m_fromPos, fireRoot.position) > 3.0f)
                        Debug.LogError("发射坐标相差太远，是否子弹产生的子弹填了发射骨骼 " + cfg.id);
                }

                else if (cfg.firePos == BulletActionConfig.FirePos.Target || cfg.firePos == BulletActionConfig.FirePos.BindTarget)
                {
                    if (m_target == null)
                    {
                        Debug.LogError("客户端子弹找不到目标 id=" + cfg.id);
                        return false;
                    }
                    fireRoot = m_target.root;
                }

                m_followBone = trans = BoneManage.GetBone(fireRoot, cfg.FirePosBone);
                if (m_followBone == null)
                    m_followBone = fireRoot;
            }

            if (trans != null)
                m_goFromPos = trans.position;
            else
                m_goFromPos = m_fromPos;

            //初始化目标点坐标,如果是直线运动则y坐标与起点相同
            m_goToPos = m_toPos;
            if (cfg.targetPos == BulletActionConfig.TargetPos.NoTarget)
                m_goToPos.y = m_goFromPos.y;

            m_goToPos = GetToPos();
            m_loadRef.Load(cfg.fxName, OnLoadFinish, null, m_goFromPos, m_source.root.rotation);

            OnBeginMove();
            return true;
        }

        protected Vector3 GetToPos()
        {
            if( m_followBone!=null && (cfg.firePos == BulletActionConfig.FirePos.BindTarget || cfg.firePos == BulletActionConfig.FirePos.BindSource))
            {
                return m_followBone.position;
            }
            else if (cfg.targetPos == BulletActionConfig.TargetPos.SkillTargetPos)
            {
                return m_goToPos;
            }
            else if (cfg.targetPos == BulletActionConfig.TargetPos.Target)
            {
                if (m_target == null || !m_target.isAlive)
                    return m_goToPos;

                 //目标受击点
                if (cfg.follow == 2)
                    return m_target.battle.actor.hurtFxTrans.position;
                //目标点
                else                 
                    return m_target.position;
            }            
            else
                return m_goToPos;
        }

        protected override void OnFinish()
        {
            if (!string.IsNullOrEmpty(cfg.destroyEffect))
                XYJObjectPool.LoadPrefab(cfg.destroyEffect, null, null, m_goToPos, Quaternion.Euler(0, m_curAngle, 0));

            if (!cfg.notDestroyModel)
            {
                m_loadRef.SetDestroy();
            }
           
        }

        protected override void OnUpdate()
        {
            if (!m_loadRef.IsLoad())
                return;
            if (m_state == State.Fire)
                OnMove();
        }

        

        protected virtual void OnBeginMove() { }
        protected virtual void OnMove() { }

        void OnLoadFinish(GameObject go, object para)
        {
            //如果特效配置销毁时间，
            ObjectPoolDestroy p = go.GetComponent<ObjectPoolDestroy>();

            //美术销毁
            if (cfg.notDestroyModel)
            {
                if (p == null)
                    Debug.LogError(string.Format("子弹模型不需要销毁，美术需要配置销毁时间 id={0} model={1}",cfg.id,go.name));
            }
            //代码销毁
            else
            {
                if (p !=null)
                    Debug.LogError(string.Format("子弹模型由配置表销毁，美术不应该配置销毁脚本 id={0} model={1}",cfg.id,go.name));
            }
        }
    }
}


using Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.battle
{
    public enum SimpleAIType
    {
        NULL,
        SimpleAttack=1,               //简单攻击
        IdleCheckEnemy,             //待机搜索敌人 
        MoveToBornPos,              //返回出生点
        Patrol,                     //巡逻
        PlayAni,                    //播放动画ai
        BehaviorTree,               //行为树
        TestMove=99,                 //测试移动
        Client_LocalPlayerInput = 100,   //客户端本地玩家
        Client_Pet,                 //宠物ai
    }


    public class  ISimpleAI
    {
        protected IObject m_obj;
        public virtual void OnCreate(IObject obj) { m_obj = obj; }
        public virtual void OnDestroy() { m_obj = null; }
        public virtual void OnEnter(object para) { }
        public virtual void OnExit() { }
        public virtual void OnUpdate() { }
        //ai暂停不会再调用OnUpdate，击倒，击飞等都会暂停ai
        public virtual void OnPause(bool pause) { }
    }

    public class SimpleAICreate
    {
        public static ISimpleAI CreateSimpleAI(SimpleAIType aiid)
        {
            switch (aiid)
            {
                case SimpleAIType.SimpleAttack:
                    return new SimpleAttackAI();
                case SimpleAIType.TestMove:
                    return new TestMoveAI();
                case SimpleAIType.BehaviorTree:
                    return new BehaviorTreeAI();
                case SimpleAIType.IdleCheckEnemy:
                    return new IdleCheckEnemyAI();
                case SimpleAIType.MoveToBornPos:
                    return new MoveToBornPosAI();
                case SimpleAIType.Patrol:
                    return new PatrolAI();
                case SimpleAIType.PlayAni:
                    return new PlayAniAI();
                case SimpleAIType.NULL:
                    return new ISimpleAI();
#if !COM_SERVER
                case SimpleAIType.Client_LocalPlayerInput:
                    return new LocalPlayerInputAI();
                case SimpleAIType.Client_Pet:
                    return new PetAI();
#endif
                default:
                    return null;
            }
        }
    }
}
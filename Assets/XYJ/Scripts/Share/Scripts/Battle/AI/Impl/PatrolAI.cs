using Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;
using wTimer;

namespace xys.battle
{
    class PatrolAI : ISimpleAI
    {
        PatrolData m_patrolData;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        public override void OnCreate(IObject obj)
        {
            base.OnCreate(obj);
        }

        /// <summary>
        /// 进入巡逻ai
        /// </summary>
        /// <param name="para"></param>
        public override void OnEnter(object para)
        {
            m_patrolData = (PatrolData)para;
            DoPatrol();
        }

        /// <summary>
        /// 执行巡逻
        /// </summary>
        void DoPatrol()
        {
            if(m_patrolData == null || m_patrolData.pathList == null)
            {
                Log.Error("巡逻数据为空");
                return;
            }

            //巡逻列表走完，需要重置列表
            if (m_patrolData.pathList.Count == 0 && m_patrolData.tempList.Count > 0)
            {
                m_patrolData.pathList = new List<PathPointVo>(m_patrolData.tempList.ToArray());
                m_patrolData.tempList = new List<PathPointVo>();
                if (m_patrolData.loopType)
                {
                    //若是正序，则设置npc坐标至第一个点
#if COM_SERVER
                    m_obj.battle.m_moveMgr.SetPosAddSend(m_patrolData.pathList[0].pos, m_obj.rotateAngle);
#else
                    m_obj.SetPosition(m_patrolData.pathList[0].pos);
#endif
                }
            }

            PathPointVo vo = m_patrolData.pathList[0];
            m_patrolData.pathList.RemoveAt(0);
            if (m_patrolData.loopType)
            {
                //正序
                m_patrolData.tempList.Add(vo);
            }
            else
            {
                //倒叙，插入到最前
                m_patrolData.tempList.Insert(0, vo);
            }
            //先把旋转清空
            m_obj.battle.State_MoveToPos(vo.pos, OnPatrolPoint, vo, vo.speed);
        }

        /// <summary>
        /// 到达关键点
        /// </summary>
        /// <param name="param"></param>
        void OnPatrolPoint(object param)
        {
            PathPointVo vo = (PathPointVo)param;
            if (null != vo)
            {
                float stayTime = vo.stayTime;
                string eventId = vo.eventId;
                
                if (stayTime == 0)
                {
                    stayTime = 0.1f;
                }
                else
                {
                    m_obj.battle.m_stateMgr.ChangeState(battle.StateType.Idle);
                }
                if (!string.IsNullOrEmpty(eventId))
                {
                    //执行一个事件
#if COM_SERVER
                    m_obj.zone.gameLevel.DoEvent(eventId);
#else
                    App.my.localPlayer.GetModule<LevelModule>().DoEvent(eventId);
#endif
                }

                //在寻路点上等待一段时间
            }
        }

        public override void OnUpdate()
        {
            //检测是否有打断的事件

        }
    }

    public class PatrolData
    {
        public string patrolId;
        public bool loopType;
        public List<PathPointVo> pathList = new List<PathPointVo>();
        public List<PathPointVo> tempList = new List<PathPointVo>();
    }

    public class PathPointVo
    {
        public Vector3 pos;
        public float stayTime;
        public float speed;
        public string eventId;
    }
}

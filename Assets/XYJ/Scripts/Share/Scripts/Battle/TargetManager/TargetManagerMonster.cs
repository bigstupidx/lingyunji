using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Config;


namespace xys.battle
{
    public class TargetManagerMonster : TargetManager, IBattleUpdate
    {
        Dictionary<IObject, int> m_hatredValues = new Dictionary<IObject, int>();

        CheckInterval m_interval = new CheckInterval();

        //出生点
        public Vector3 m_bornPos { get; private set; }
        public float m_bornAngle { get; private set; }
        public bool m_isBackToBornPos { get; private set; }

        public override void OnEnterScene()
        {
            base.OnEnterScene();
            //坐标必须在添加到场景前设置
            m_bornPos = m_obj.position;
            m_bornAngle = m_obj.rotateAngle;

            if (m_obj.m_refreshData != null)
                m_isBackToBornPos = m_obj.m_refreshData.backToBorn;
        }

        public override void AddValue(IObject obj, int value)
        {
            if (!m_active)
                return;

            int tempValue;
            if (m_hatredValues.TryGetValue(obj, out tempValue))
            {
                value = tempValue + value;
                m_hatredValues[obj] = value;
            }
            else
                m_hatredValues.Add(obj, value);

            IObject curTarget = this.target;
            if (curTarget == null)
                SetTarget(obj);
            else if (obj != curTarget)
            {
                //仇恨当前目标一定值，则转化
                if (!m_hatredValues.TryGetValue(curTarget, out tempValue))
                {
                    m_hatredValues.Add(curTarget, 0);
                    tempValue = 0;
                    XYJLogger.LogError("不在仇恨列表里面，不应该走到这里 obj={0} target={1}", obj.charSceneId, curTarget.charSceneId);
                }
                if (value > (int)(tempValue * (1 + kvBattle.hatredChangeMul)))
                    SetTarget(obj);
            }
        }

        public override void SetActive(bool active)
        {
            if(!active)
            {
                m_hatredValues.Clear();
                SetTarget(null);
            }
            base.SetActive(active);
        }

        //回出生点,射过后就必定会回去
        void SetBackToBorn()
        {
            m_obj.battle.m_ai.ChangeAI(SimpleAIType.MoveToBornPos,new object[] { m_bornPos, m_bornAngle });
        }

        void SetNoTarget()
        {
            if(m_isBackToBornPos)
            {
                SetBackToBorn();
                return;
            }

            m_obj.battle.m_ai.ChangeIdleAI();
        }

        public void OnUpdate()
        {
            if (!m_interval.Check(1.0f))
                return;

            if (!m_obj.battle.m_isAiByLocal || !m_obj.isAlive || m_obj.battleCamp == BattleCamp.NeutralCamp)
                return;

            if (!m_active)
                return;

            bool isBornPosAi = m_isBackToBornPos;
            //回出生点
            if (isBornPosAi && m_obj.battle.m_ai.m_curAiType != SimpleAIType.IdleCheckEnemy)
            {
                //距离超过一定值回出生点
                if (BattleHelp.GetDistance(m_bornPos, m_obj.position) > m_obj.battle.m_attrMgr.trackDistance)
                {
                    SetBackToBorn();
                    return;
                }
            }

            //当前仇恨列表没有目标，第一个进入视野的选为仇恨目标
            if (m_hatredValues.Count == 0)
            {
                //被动怪不会主动添加目标
                if (m_obj.battle.m_attrMgr.fieldOfView > 0)
                {
                    foreach (var p in BattleHelp.GetAOIObj(m_obj))
                    {
                        if (p.Value.isAlive && BattleHelp.IsEnemy(m_obj, p.Value) && BattleHelp.GetDistance(m_obj, p.Value) < m_obj.battle.m_attrMgr.fieldOfView)
                        {
                            SetTarget(p.Value);
                            m_hatredValues.Add(p.Value, kvBattle.hatredInitValue);
                            return;
                        }
                    }
                }

                return;
            }
            //移除，已经死亡或者走出追击距离的
            else
            {
                IObject deadObj = null;
                foreach (var p in m_hatredValues)
                {
                    if (!p.Key.isAlive 
                        || (isBornPosAi && BattleHelp.GetDistance(m_bornPos, p.Key.position) > m_obj.battle.m_attrMgr.trackDistance))
                    {
                        deadObj = p.Key;
                        break;
                    }
                }
                if (deadObj != null)
                    m_hatredValues.Remove(deadObj);
            }

            //当前目标死亡，重新选目标
            if (m_target != null && !m_target.isAlive)
            {
                int hatredValue = -1;
                IObject changeTarget = null;
                //仇恨值大于hatredValue中最大的
                foreach (var p in m_hatredValues)
                {
                    if (p.Key.isAlive && BattleHelp.IsEnemy(m_obj, p.Key))
                    {
                        if (p.Value > hatredValue)
                        {
                            hatredValue = p.Value;
                            changeTarget = p.Key;
                        }
                    }
                }
                //切换目标
                if (changeTarget != null)
                    SetTarget(changeTarget);
                //目标死亡，回到出生点
                else
                {
                    SetNoTarget();
                    return;
                }

            }
        }
    }
}

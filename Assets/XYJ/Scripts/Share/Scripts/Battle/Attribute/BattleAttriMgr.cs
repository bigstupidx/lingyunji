using Config;
using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class BattleAttriMgr : IBattleComponent, IBattleUpdate
    {
        public PostureConfig postureCfg { get; protected set; }
        //战斗状态
        public bool battleState { get { return m_obj.stateValue == (int)ObjectState.Battle; } }
        //视野
        public float fieldOfView { get; private set; }
        //追踪距离
        public float trackDistance { get; private set; }
        //数值改变
        public BattleAttriLogic logic { get; private set; }

        #region 内部实现

        //总属性
        protected BattleAttri m_totalAttri = new BattleAttri();
        //原始属性和
        protected BattleAttri m_originalAttriSum = new BattleAttri();
        protected BattleAttri m_buffAttriSum = new BattleAttri();
        //添加buff属性时不重新计算属性
        protected bool m_onlyAddBuffAttri = false;
        protected IObject m_obj;
        public virtual void OnAwake(IObject obj)
        {
            m_obj = obj;
            BattleHelp.Subscribe(obj, NetProto.AttType.AT_Posture, OnChangePosture);
        }
        public virtual void OnDestroy() { m_obj = null; }

        public virtual void OnStart()
        {
            if (BattleHelp.IsRunBattleLogic())
                InitDataByLocalAI();

            SetPostureImpl(m_obj.postureValue);
        }

        public void OnEnterScene()
        {
            if (BattleHelp.IsRunBattleLogic())
                logic = new BattleAttriLogicLocal(m_obj);
            else
                logic = new BattleAttriLogic(m_obj);

            //根据刷新点初始化数据
            NpcBase npc = m_obj as NpcBase;
            if (npc != null)
            {
                fieldOfView = m_obj.cfgInfo.fieldOfView;
                trackDistance = m_obj.cfgInfo.trackDisance;

            }
        }
        public virtual void OnExitScene()
        {
            logic = null;
        }

        public void OnUpdate()
        {
            logic.OnUpdate();
        }
        void OnChangePosture(CommonBase.IAttribute<NetProto.AttType> args)
        {
            SetPostureImpl(args.intValue);
        }

        void SetPostureImpl(int id)
        {
            PostureConfig newCfg = PostureConfig.Get(id);
            //设置空姿态,避免后续使用判断空指针
            if (newCfg == null)
                newCfg = PostureConfig.Get(0);

            //移除buff
            if (postureCfg != null && postureCfg.logic != null)
                m_obj.battle.m_buffMgr.RemoveBuff(postureCfg.logic.buffid);
            //添加buff
            if (newCfg != null && newCfg.logic != null)
                m_obj.battle.m_buffMgr.AddBuff(m_obj, newCfg.logic.buffid);
            postureCfg = newCfg;
        }

        //刷新血量
        protected void RefreshHP()
        {
            //测试代码，进入场景血量恢复满,生命值不小于1
            int hp = BattleAttri.RoundInt((m_totalAttri.Get(AttributeDefine.iHp) * (1 + m_totalAttri.Get(AttributeDefine.fHpAddition))));
            if (hp == 0)
                hp = 5;

            if (m_obj.type == NetProto.ObjectType.Player)
            {
                hp = 10000;
                m_obj.zhenQiValue = (ushort)kvBattle.zhenqiMax;
            }

            float mulHp = m_obj.hpValue / (float)m_obj.maxHpValue;
            m_obj.maxHpValue = hp;

            //血量按百分比修改
            m_obj.hpValue = (int)(mulHp * m_obj.maxHpValue);
        }

        //初始化数据
        void InitDataByLocalAI()
        {
            m_obj.hpValue = (int)m_obj.maxHpValue;

            if (m_totalAttri.Get(AttributeDefine.iHuti) > 0)
            {
                m_obj.maxHuTiValue = m_obj.huTiValue = (ushort)m_totalAttri.Get(AttributeDefine.iHuti);
                m_obj.huTiStateValue = (ushort)HutiState.Huti;
            }
            else
                m_obj.huTiStateValue = (ushort)HutiState.Poti;

            //设置姿态
            m_obj.postureValue = m_obj.cfgInfo.posture;
            //设置速度
            m_obj.speedValue = (int)(m_obj.cfgInfo.speed*100);
        }

        protected virtual void SendChangeToClient()
        {
        }

        #endregion

        #region 对外接口
        //fov=0会使用默认值
        public void SetFieldOfView(float fov)
        {
            if (fov == 0)
            {
                fieldOfView = m_obj.cfgInfo.fieldOfView;
            }

            else
                fieldOfView = fov;
        }
        //distance=0会使用默认值
        public void SetTrackDistance(float distance)
        {
            if (distance == 0)
            {
                trackDistance = m_obj.cfgInfo.trackDisance;
            }

            else
                trackDistance = distance;
        }

        public float GetFloat(int index)
        {
            return (float)m_totalAttri.Get(index);
        }

        public int GetInt(int index)
        {
            return (int)m_totalAttri.Get(index);
        }


        public BattleAttri GetTotalAttri()
        {
            return m_totalAttri;
        }

        //根据buff重新刷新属性
        public void RefreshAttributeByBuff(BattleAttri attri, bool isAdd, bool refreshAll)
        {
            if (attri != null)
            {
                if (isAdd)
                    m_buffAttriSum.Add(attri);
                else
                    m_buffAttriSum.Sub(attri);
            }

            //添加buff属性时不重新计算属性
            if (m_onlyAddBuffAttri)
                return;

            //修改了一级属性
            if (refreshAll)
            {
                m_totalAttri.Clear();
                //属性的一级转化
                if (m_obj.type == NetProto.ObjectType.Player)
                    m_totalAttri = BattleAttriCaculate.GetPlayerAttributeFix(m_originalAttriSum, m_buffAttriSum, m_obj.job, m_obj.levelValue);
                else if (m_obj.type == NetProto.ObjectType.Npc)
                    m_totalAttri = BattleAttriCaculate.GetMonsterAttributeFix(m_originalAttriSum, m_buffAttriSum, m_obj.cfgInfo, m_obj.levelValue);
                else if (m_obj.type == NetProto.ObjectType.Pet)
                {
#if COM_SERVER
                    m_totalAttri = BattleAttriCaculate.GetPetsAttributeFix(m_originalAttriSum, m_obj.levelValue,((Pet)m_obj).GetQualification(), m_buffAttriSum);
#endif
                }
            }
            RefreshHP();
            //同步属性改变给客户端
            SendChangeToClient();
        }


        public string GM_GetUIShowText()
        {
            BattleAttri ui = new BattleAttri();
            BattleAttriCaculate.GetUIShowAttribute(m_totalAttri, ui, m_obj.job, m_obj.levelValue);
            string text = string.Format("角色属性 id={0} \r\n", m_obj.charSceneId);
            return text + ui.GetAllValueString();
        }
        #endregion
    }
}

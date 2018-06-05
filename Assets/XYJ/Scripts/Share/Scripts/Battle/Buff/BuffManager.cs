using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public abstract partial class BuffManager : IBattleComponent, IBattleUpdate
    {
        //所有buff
        List<Buff> m_buffList = new List<Buff>();

        //有重复key，所以不能用map
        Dictionary<int, List<Buff>> m_buffMap = new Dictionary<int, List<Buff>>();

        public abstract Buff Create(BuffType id);
        protected IObject m_obj;
        CheckInterval m_interval = new CheckInterval();
        public void OnAwake(IObject obj)
        {
            m_obj = obj;
        }
        public void OnStart()
        {

        }
        public void OnDestroy()
        {
            m_obj = null;
        }

        public void OnEnterScene()
        {

        }
        public void OnExitScene()
        {

        }

        public void OnUpdate()
        {
            //500毫秒tick一次
            if (!m_interval.Check(kvBattle.buffTickInterval))
                return;

            for (int i = m_buffList.Count - 1; i >= 0; i--)
            {
                Buff buff = m_buffList[i];
                if (buff.IsFinish())
                {
                    RemoveBuffImpl(i);
                }
                //有持续事件
                else
                    buff.UpdateActionsTick();
            }
        }


        //获得buff叠加层数
        public int GetBuffAddCnt(int buffid)
        {
            List<Buff> list;
            if (m_buffMap.TryGetValue(buffid, out list))
            {
                if (list.Count > 0)
                    return list[0].m_addCnt;
            }
            return 0;
        }
        //添加buff
        public bool AddBuff(IObject source, int buffid)
        {
            if (buffid == 0)
                return false;
            BuffConfig cfg = BuffConfig.Get(buffid);
            if (cfg == null)
                return false;

            List<Buff> mapList;
            if (m_buffMap.TryGetValue(buffid, out mapList))
            {
                foreach(var p in mapList)
                {
                    //相同id的常规只会有一个Buff,如果区分施法者则每个施法者都可以有一个,叠加层数由配置表配置
                    if (cfg.addType == BuffConfig.AddType.Normal || p.m_source == source)
                    {
                        p.AddCnt(source);
                        return true;
                    }
                }
            }
            else
            {
                mapList = new List<Buff>();
                m_buffMap.Add(buffid, mapList);
                //相同id的buff只会加一个特效
                OnAddBuffEffect(cfg);
            }


            //可以添加
            Buff newBuff = Create(cfg.type);
            newBuff.Enter(source, m_obj, cfg);
            m_buffList.Add(newBuff);
            mapList.Add(newBuff);
            return true;
        }

        void RemoveBuffImpl(int index)
        {
            Buff buff = m_buffList[index];
            buff.Exit();
            m_buffList.RemoveAt(index);

            List<Buff> mapList;
            if (m_buffMap.TryGetValue(buff.m_cfg.id, out mapList))
            {
                mapList.Remove(buff);
                if (mapList.Count == 0)
                {
                    m_buffMap.Remove(buff.m_cfg.id);
                    //相同id的buff只会加一个特效
                    OnRemoveBuffEffect(buff.m_cfg);
                }
            }
        }

        public void RemoveBuff(int buffid)
        {
            if (buffid == 0)
                return;
            for (int i = m_buffList.Count - 1; i >= 0; i--)
            {
                if (m_buffList[i].m_cfg.id == buffid)
                {
                    RemoveBuffImpl(i);
                }
            }
        }

        protected virtual void OnRemoveBuffEffect(BuffConfig cfg)
        {

        }
        protected virtual void OnAddBuffEffect(BuffConfig cfg)
        {

        }

        public string GM_GetUIShowText()
        {
            string text = string.Format("角色buff id={0} \r\n", m_obj.charSceneId);
            foreach(var p in m_buffList)
            {
                text += string.Format("id={0} type={1} cnt={2}\r\n", p.m_cfg.id, p.m_cfg.typename,p.m_addCnt);
            }
            return text;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace xys.battle
{
    //对象池对象基类
    public abstract class NewObject
    {
        public virtual void DestroyObj() { }
    }

    public abstract class FactoryCreatePool
    {
        public abstract string GetCntTip();
    }

    //对象工厂
    public abstract class FactoryCreatePool<U, T> : FactoryCreatePool where T : NewObject, new()
    {
        Dictionary<U, NewObjectPool> m_pools = new Dictionary<U, NewObjectPool>();

        protected abstract T CreateFactory(U id);

        public T Get(U id)
        {
            NewObjectPool list;
            T obj;
            if (!m_pools.TryGetValue(id, out list))
            {
                list = new NewObjectPool();
                m_pools.Add(id, list);
            }

            obj = list.Get(id, CreateFactory);

            return obj;
        }

        public void Release(U id, T obj)
        {
            NewObjectPool list;
            if (m_pools.TryGetValue(id, out list))
            {
                list.Release(obj);
            }
        }

        public void Clear()
        {
            m_pools.Clear();
        }

        public override string GetCntTip()
        {
            string tip = "";
            int totalCreate = 0;
            int totalIdle = 0;
            foreach (KeyValuePair<U, NewObjectPool> p in m_pools)
            {
                tip += p.Value.objCreateCnt + " " + p.Value.objIdleCnt + " " + p.Key.ToString() + "\r\n";
                totalCreate += p.Value.objCreateCnt;
                totalIdle += p.Value.objIdleCnt;
            }
            tip = totalCreate + " " + totalIdle + " \r\n" + tip;
            return tip;
        }


        //对象池
        public class NewObjectPool
        {
            List<T> m_list = new List<T>();
            int m_objCreate = 0;
            //获取一个对象
            public T Get(U id, Func<U, T> createFactory)
            {
                if (m_list.Count == 0)
                {
                    T obj = createFactory(id);
                    m_objCreate++;
                    return obj;
                }
                else
                {
                    T obj = m_list[0];
                    m_list.RemoveAt(0);
                    return obj;
                }
            }

            //释放一个对象
            public void Release(T obj)
            {
                if (obj == null)
                    return;

                if (!m_list.Contains(obj))
                {
                    m_list.Add(obj);
                    obj.DestroyObj();
                }
                else
                    XYJLogger.LogError("重复删除对象 " + obj.GetType());
            }

            public void Clear()
            {
                m_list.Clear();
            }

            public int objCreateCnt
            {
                get { return m_objCreate; }
            }

            public int objIdleCnt
            {
                get { return m_list.Count; }
            }
        }
    }
}

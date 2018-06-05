using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

namespace xys.UI
{
    public class UIGroup : MonoBehaviour
    {
        public enum enType
        {
            none,
            tab,
            tab2,//再点一下可以取消选中状态
        }
        public enType m_type = enType.none;

        State.StateRoot m_cur;//当前选中的
        Action<int> m_onSel;  

        List<GameObject> m_items = new List<GameObject>();
        
        bool m_cached = false;


        public int Count { get { Cache(); return m_items.Count!=1? m_items.Count:((m_items[0].activeSelf)?1:0); } }
        public GameObject this[int index] { get { Cache(); return m_items[index]; } }
        public int CurIdx { get { return m_items.IndexOf(m_cur.gameObject); } }

        void Awake()
        {
            Cache();
        }

        public IEnumerator GetEnumerator()
        {
            Cache();
            return this.m_items.GetEnumerator();
        }


        void Cache()//有时候别的函数的执行可能先于Start()函数，这时候Cache()下就相当于先执行了Start()
        {
            if (m_cached)
                return;

            Transform t = this.transform;
            for (int i = 0; i < t.childCount; ++i)
            {
                GameObject go = t.GetChild(i).gameObject;
                go.name = "item" + i;

                var sr=go.GetComponent<UI.State.StateRoot>();
                if (sr != null && sr.uCurrentButton!=null)
                {
                    int idx = i;
                    sr.uCurrentButton.onClick.RemoveAllListeners();
                    sr.uCurrentButton.onClick.AddListener(() => OnSel(idx));

                }
                m_items.Add(go);
            }

            //类型状态机
            switch (m_type)
            {
                case enType.none: break;
                case enType.tab: OnCacheTab(); break;//确保有且只有一个处于选中状态
                case enType.tab2: OnCacheTab2(); break;//这里不做任何处理了，上层自己在初始化后设置
                default: Debuger.LogError(string.Format("未作cache的控件类型", m_type)); break;
            }


            m_cached = true;
        }

        //提供设置大小的功能
        public void SetCount(int count)
        {
            Cache();
            if (count == 0)
            {
                this.gameObject.SetActive(false);
                return;
            }
            else
                this.gameObject.SetActive(true);


            //多退少补
            int curCount = m_items.Count;
            GameObject s;
            if (count < curCount)
            {
                for (int i = count; i < curCount; ++i)
                {
                    s = m_items[i];
                    //这里不要把m_itemTemplate对应的对象销毁了
                    //if (m_itemTemplate == s)
                    //    s.gameObject.SetActive(false);
                    //else
                        UnityEngine.Object.Destroy(s);
                }
                m_items.RemoveRange(count, curCount - count);
            }
            else if (count > curCount)
            {
                GameObject go;
                Transform t;
                for (int i = curCount; i < count; ++i)
                {
                    GameObject template = m_items[m_items.Count - 1];//m_itemTemplate != null ? m_itemTemplate : 
                    go = GameObject.Instantiate(template.gameObject) as GameObject;
                    go.gameObject.name = "item" + i;
                    t = go.transform;
                    t.SetParent(this.transform, false);
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = m_items[m_items.Count - 1].gameObject.transform.localScale;
                    if (go.layer != this.gameObject.layer) go.layer = this.gameObject.layer;
                    if (go.activeSelf == false) go.SetActive(true);
                    m_items.Add(go);

                    var sr = go.GetComponent<UI.State.StateRoot>();
                    if (sr != null && sr.uCurrentButton != null)
                    {
                        int idx = i;
                        sr.uCurrentButton.onClick.RemoveAllListeners();
                        sr.uCurrentButton.onClick.AddListener(() => OnSel(idx));
                    }
                 }
            }
        }

        public GameObject Get(int idx)
        {
            Cache();
            if (idx >= m_items.Count || idx < 0)
                return null;
            return m_items[idx];
        }

        public T Get<T>(int idx) where T : Component
        {
            Cache();
            if (idx >= m_items.Count || idx < 0)
                return null;
            return m_items[idx].GetComponent<T>();
        }

        //选中第几个，如果是-1就是全部不选中
        public void SetSel(int idx)
        {
            Cache();
            if (idx < -1 || idx >= m_items.Count)
            {
                Debuger.LogError(string.Format("索引不存在!{0}/{1}", idx, m_items.Count));
                return;
            }

            OnSel(idx);
        }

        public void AddSel(Action<int> cb, bool reset = false)
        {
            Cache();
            if (m_onSel == null || reset)
            {
                m_onSel = cb;
                return;
            }

            //如果重复添加，那么就不添加了
            Delegate[] inlist = m_onSel.GetInvocationList();
            foreach (Delegate d in inlist)
            {
                if (d == cb)
                    return;
            }

            m_onSel += cb;
        }

        void OnSel(int idx)
        {
            if (m_cached == false)//初始化中的话不用处理
                return;

            //类型状态机
            bool selSuccess = false;
            switch (m_type)
            {
                case enType.none:  break;
                case enType.tab: selSuccess = OnSelTab(ref idx); break;
                case enType.tab2: selSuccess = OnSelTab2(ref idx); break;
                default: Debuger.LogError(string.Format("未作处理的控件类型", m_type)); break;
            }

            if (selSuccess && m_onSel != null)
            {
                m_onSel(idx);
            }
        }

        #region 不同的控件类型的特殊处理，如果以后类型很多可以拆到别的类里或者用partial
        void OnCacheTab()
        {
            //如果是tab控件，确保有且只有一个处于选中状态
            UI.State.StateRoot sNeedSet = null;//需要设置到提起状态的控件
            bool needSet = true;
            foreach (var s in m_items)
            {
                if (s == null)
                    continue;

                var sr = s.GetComponent<State.StateRoot>();
                if(sr == null)
                {
                    Debug.LogError("UIGroup的子节点必须有StateRoot组件才可以用tab功能");
                    return;
                }

                
                if (sr.CurrentState== 0 && needSet == true)//找到需要设置为提起状态的控件
                    sNeedSet = sr;
                if (sr.CurrentState == 1 && needSet == false)//只有一个控件能为提起状态，其他控件要设置为0
                    sr.CurrentState = 0;
                if (sr.CurrentState == 1 && needSet == true)//找到一个已经设置为提起状态的控件的话，记录下
                {
                    needSet = false;
                    sNeedSet = sr;
                }
            }
            if (needSet && sNeedSet != null)//设置为选中状态
                sNeedSet.CurrentState = 1;
            m_cur = sNeedSet;//当前选中的状态控件
        }

        bool OnSelTab(ref int idx)
        {
            State.StateRoot sr;
            if (idx == -1)
                sr = null;
            else
                sr = m_items[idx].GetComponent<State.StateRoot>();
            
            if (sr == m_cur)
                return true;//也触发回调

            //老的置0，新的置1
            if (m_cur!=null)
                m_cur.CurrentState = 0;
            m_cur = sr;
            if(m_cur!=null)
                m_cur.CurrentState = 1;
            
            return true;
        }


        void OnCacheTab2()
        {
            m_cur = null;
        }

        bool OnSelTab2(ref int idx)
        {
            State.StateRoot sr;
            if (idx == -1)
                sr = null;
            else
                sr = m_items[idx].GetComponent<State.StateRoot>();
            
            if (m_cur == sr && m_cur != null)//同一个的话取消选中
            {
                m_cur = null;
                idx = -1;
            }
            else
            {
                m_cur = sr;   
            }
                

            foreach (var s in m_items)
            {
                if (s == null)
                    continue;

                sr = s.GetComponent<State.StateRoot>();
                if (sr == null)
                {
                    Debug.LogError("UIGroup的子节点必须有StateRoot组件才可以用tab2功能");
                    continue;
                }

                int st = sr == m_cur?1:0;

                if (sr.CurrentState != st)//找到需要设置为提起状态的控件
                    sr.CurrentState = st;
            }
           
            return true;
        }

        #endregion
    }


}
#if !USE_HOT
//#define DEFDEBUG 
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using UnityEngine;
using xys.UI.State;
using NetProto;
using Config;
using xys.UI;
using NetProto.Hot;
using xys.hot.Event;

namespace xys.hot.UI
{
    [AutoILMono]
    class OLScrollView
    {
        enum BtnState
        {
            STATE_ENABLE,
            STATE_RECEIVERD,
            STATE_DISABLE
        }
        [SerializeField]
        GameObject m_SOLAwardItem;
        [SerializeField]
        Transform m_Grid;

        //public Transform transform { get { return m_Transform; } }

        int m_TotalCountDown = 0; //当前栏总倒数时间

        int m_TimeDif = 0; //时间差，用于判断倒数是否完成
        int m_CurCountDown = 0;//当前栏的当前倒数时间，每帧变动
        bool m_IsCounting = false; //是否有奖励栏处于倒数之中，倒数过程中为TRUE，倒数完成为false
        bool m_isAwardEnable = false;  //奖励是否可领取，倒数完成后为true
        int m_CountingIndex = 0; //当前倒数栏的INDEX

        List<GameObject> m_ColumnGoList = new List<GameObject>();
        List<WelfareRwdColumn> m_ColumnInstanceList = new List<WelfareRwdColumn>();
        HotObjectEventAgent m_Event;
        WelfareDB m_WelfareData;
        bool m_hasReward = true;
        bool m_MessageFired = false;
        public void Awake()
        {
            //m_Page.parent.Event.Subscribe(EventID.Welfare_OLRwdReceived, DisableColumn);
            WelfareMgr mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
            m_WelfareData = mgr.m_WelfareDB;

            Dictionary<int, NewbieRewardDefine> dataConf = Config.NewbieRewardDefine.GetAll();
            Dictionary<int, NewbieRewardDefine>.Enumerator itr = dataConf.GetEnumerator();
            while (itr.MoveNext())
            {
                ItemCount[] items = Config.RewardDefine.Get(itr.Current.Value.award).item.items;

                GameObject go = GameObject.Instantiate(m_SOLAwardItem);
                go.SetActive(true);
                go.transform.SetParent(m_Grid);
                go.transform.localScale = Vector3.one;
                m_ColumnGoList.Add(go);

                WelfareRwdColumn column = (WelfareRwdColumn)go.GetComponent<ILMonoBehaviour>().GetObject();
                column.SetInfoText("第" + StrUtil.intToZH(itr.Current.Value.id) + "次");
                column.SetData(WelfareRwdColumn.ColumnType.TYPE_OLRWD, itr.Current.Value.id - 1, items);
                m_ColumnInstanceList.Add(column);
            }
            m_CountingIndex = m_WelfareData.onlineRwdID - 1;
            if (m_CountingIndex<m_ColumnGoList.Count)
            {
                m_TotalCountDown = Config.NewbieRewardDefine.Get(m_WelfareData.onlineRwdID).time
                               - (m_WelfareData.onlineAwardTime < 0 ? 0 : m_WelfareData.onlineAwardTime);
            }
            else
            {
                m_hasReward = false;
                Debug.LogError("m_WelfareData.onlineRwdID out of range");
            }

            
        }
        
        public void OnEnable()
        {
            if (m_Event != null)
            {
                m_Event.Release();
                m_Event = null;
            }
            m_Event = new HotObjectEventAgent(App.my.localPlayer.eventSet);
            StartCountDown(m_CountingIndex, m_TotalCountDown);
            RefreshUI();
        }
        public void OnDisable()
        {
            if (m_Event != null)
            {
                m_Event.Release();
                m_Event = null;
            }
        }
        public void DisableColumn()
        {
            if (m_IsCounting)
            {
                return;
            }
            //show rwd(move to package)
            //m_ColumnInstanceList[m_CountingIndex].ShowObtainItem();
            //指向下个index
            m_CountingIndex = m_WelfareData.onlineRwdID-1;

            if (m_WelfareData.onlineRwdID <= Config.NewbieRewardDefine.GetAll().Count)
            {
                m_hasReward = true;
                m_TotalCountDown = Config.NewbieRewardDefine.Get(m_WelfareData.onlineRwdID).time-1;
                StartCountDown(m_CountingIndex, m_TotalCountDown);
            }
            else
                m_hasReward = false;

            //重拍所有子节点
            RefreshUI();
        }
        //设置倒计时数据
        void StartCountDown(int index, int time)
        {
            Debug.Log("Counting down-- "+time);
            m_CurCountDown = time;
            m_IsCounting = true;
            m_ColumnInstanceList[m_CountingIndex].SetBtnText(Convert.ToString(m_CurCountDown));
            m_Event.FireEvent<int>(EventID.Welfare_PageRewardNotReady, (int)WelfarePageType.TYPE_OL);
        }
        public void RefreshUI()
        {
            WelfareMgr mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
            m_WelfareData = mgr.m_WelfareDB;

            m_Grid.DetachChildren();
            for (int i = m_CountingIndex; i < m_ColumnGoList.Count; i++)
            {
                m_ColumnGoList[i].transform.SetParent(m_Grid);
                m_ColumnInstanceList[i].SetReceiveBtnDisable();
            }
            //加载领取的
            for (int i = 0; i < m_CountingIndex; i++)
            {
                m_ColumnGoList[i].transform.SetParent(m_Grid);
                m_ColumnInstanceList[i].SetReceiveBtnReceived();
            }
            if (!m_hasReward)
            {
                m_Event.FireEvent<int>(EventID.Welfare_NoPageReward, (int)WelfarePageType.TYPE_OL);
                m_Event.FireEvent<int>(EventID.Welfare_PageRewardNotReady, (int)WelfarePageType.TYPE_OL);
            }
            m_MessageFired = false;
        }
        void Update()
        {
            if (!m_hasReward)
            {
                return;
            }
            if (m_IsCounting)
            {
                m_isAwardEnable = false;
#if DEFDEBUG
                m_TimeDif += Time.deltaTime ;
                if (m_TimeDif >= 1.0f)
                {
                    m_CurCountDown--;
                    m_TimeDif = 0.0f;
                }
                if (m_CurCountDown > 0)
                {
                    m_ColumnInstanceList[m_CountingIndex].SetBtnText(Convert.ToString(Convert.ToString(m_CurCountDown)));
                }
                else
                {
                    m_CurCountDown = 0;
                    m_isAwardEnable = true;
                    m_IsCounting = false;
                }


#else
                //当前计时器时间差=当前栏倒数时间-当前时间与该栏开始倒数日期的差值
                m_TimeDif = m_CurCountDown - (int)DateTime.Now.Subtract(HotWelfareModule.countDownDate).TotalSeconds;
                if (m_TimeDif > 0)
                {
                    m_ColumnInstanceList[m_CountingIndex].SetBtnText(GetTimeInfo(m_TimeDif));
                }
                else
                {
                    Debug.Log("count down--"+m_CurCountDown+"--end");
                    m_CurCountDown = 0;
                    m_isAwardEnable = true;
                    m_IsCounting = false;
                }
#endif
            }

            if (m_isAwardEnable)
            {
                m_ColumnInstanceList[m_CountingIndex].SetReceiveBtnEnable();
                if(!m_MessageFired)
                {
                    m_Event.FireEvent<int>(EventID.Welfare_PageRewardReady, (int)WelfarePageType.TYPE_OL);
                    m_MessageFired = true;
                }
            }
        }
        //获取时间信息
        string GetTimeInfo(int time)
        {
            int min = 0, sec = 0,hour = 0;
            SecondsToMinsAndSeconds(time, out min, out sec,out hour);
            string str;
            if (min != 0 && sec != 0)
            {
                str = string.Format("{0}分{1}秒", min, sec);
            }
            else
            {
                if (min != 0)
                {
                    str = string.Format("{0}分", min);
                }
                else
                {
                    str = string.Format("{0}秒", sec);
                }
            }
            return str;
        }
        //秒转时分秒
        private void SecondsToMinsAndSeconds(int time,out int min,out int sec, out int hour)
        {
            sec = time % 60;
            min = time / 60;
            hour = (time /= 60) / 60;
        }
    }
}

#endif
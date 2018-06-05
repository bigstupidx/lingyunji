#if !USE_HOT
using Config;
using NetProto;
using NetProto.Hot;
using System.Collections.Generic;
using UnityEngine;
using xys.hot.Event;
using xys.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    class DayOLScrollView
    {
        [SerializeField]
        GameObject m_SOLAwardItem;
        [SerializeField]
        Transform m_Grid;

        //public Transform transform { get { return m_Transform; } }



        List<GameObject> m_ColumnGoList = new List<GameObject>();
        List<WelfareRwdColumn> m_ColumnInstanceList = new List<WelfareRwdColumn>();
        List<int> m_ReceivedAwdList = new List<int>(); //已领取的奖励条的index列表
        List<int> m_UnreceivedAwdList = new List<int>();//未领取的奖励条的index列表
        Dictionary<int, ItemCount[]> m_RwdTable = new Dictionary<int, ItemCount[]>();

        WelfareDB m_WelfareData;
        IntBit m_RwdStatus;
        bool m_RwdReady = false;
        HotObjectEventAgent m_Event;
        //初始化
        public void Awake()
        {
            //m_Page.parent.Event.Subscribe<int>(EventID.Welfare_DayOLRwdReceived, DisableColumn);
            Dictionary<int, SevenRewardDefine> dataConf = Config.SevenRewardDefine.GetAll();
            Dictionary<int, SevenRewardDefine>.Enumerator itr = dataConf.GetEnumerator();
            //初始化奖励条目
            while (itr.MoveNext())
            {
                ItemCount[] items = Config.RewardDefine.Get(itr.Current.Value.award).item.items;
                m_RwdTable.Add(itr.Current.Value.day, items);

                GameObject go = GameObject.Instantiate(m_SOLAwardItem);
                go.SetActive(true);
                go.transform.SetParent(m_Grid);
                go.transform.localScale = Vector3.one;
                m_ColumnGoList.Add(go);

                WelfareRwdColumn column = (WelfareRwdColumn)go.GetComponent<ILMonoBehaviour>().GetObject();
                column.SetInfoText("第" + StrUtil.intToZH(itr.Current.Value.id) + "天");
                column.SetData(WelfareRwdColumn.ColumnType.TYPE_DAYOLRWD, itr.Current.Value.id - 1, items);
                m_ColumnInstanceList.Add(column);
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
        //刷新数据
        public void RefreshData()   //assume the input data format is Dic<lv,AwdList>
        {

            m_ReceivedAwdList.Clear();
            m_UnreceivedAwdList.Clear();

            WelfareMgr mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
            m_WelfareData = mgr.m_WelfareDB;
            m_RwdStatus = new IntBit(m_WelfareData.dayOLRwdStatus);
            for (int i = 0; i < m_ColumnInstanceList.Count; i++)
            {
                //该INDEX对应的奖励是否已经领取过
                if (!m_RwdStatus.Get(i))
                {
                    //将奖励放入可领取列表
                    m_UnreceivedAwdList.Add(i);
                    //如果当前的登录奖励天数小于奖励表中对应奖励ID中的天数要求
                    if (m_WelfareData.loginDays >= Config.SevenRewardDefine.Get(i + 1).day)
                    {
                        //显示可领取
                        m_ColumnInstanceList[i].SetReceiveBtnEnable();
                    }
                    else
                    {
                        m_ColumnInstanceList[i].SetReceiveBtnDisable();
                    }
                }
                else
                {
                    m_ReceivedAwdList.Add(i);
                }
            }
            m_ReceivedAwdList.Sort();
        }

        //隐藏领取奖励的项目
        public void DisableColumn(int rwdIndex)
        {
            //change status and write back to local
            m_RwdStatus.Set(rwdIndex, true);
            m_WelfareData.dayOLRwdStatus = m_RwdStatus.value;
            RefreshUI();
        }

        public void RefreshUI()
        {
            RefreshData();

            m_Grid.DetachChildren();
            m_RwdReady = false;

            for (int i = 0; i < m_UnreceivedAwdList.Count; i++)
            {
                m_ColumnGoList[m_UnreceivedAwdList[i]].transform.SetParent(m_Grid);
                //检测是否有奖励可领取
                //如果i之前已有奖励可领取，无需再判断
                if (!m_RwdReady&&(m_ColumnInstanceList[m_UnreceivedAwdList[i]].GetReceiveBtnStatus()== (int)WelfareRwdColumn.BtnState.STATE_ENABLE))
                {
                    m_RwdReady = true;
                    m_Event.FireEvent<int>(EventID.Welfare_PageRewardReady, (int)WelfarePageType.TYPE_DAYOL);
                }
            }
            
            //after checking the unreceived ,no avaliables
            if (!m_RwdReady)
            {
                m_Event.FireEvent<int>(EventID.Welfare_PageRewardNotReady, (int)WelfarePageType.TYPE_DAYOL);
            }

            for (int i = 0; i < m_ReceivedAwdList.Count; i++)
            {
                m_ColumnGoList[m_ReceivedAwdList[i]].transform.SetParent(m_Grid);
                m_ColumnInstanceList[m_ReceivedAwdList[i]].SetReceiveBtnReceived();
            }
            if (m_UnreceivedAwdList.Count == 0)
            {
                m_Event.FireEvent<int>(EventID.Welfare_NoPageReward, (int)WelfarePageType.TYPE_DAYOL);
            }
        }
    }
}

#endif
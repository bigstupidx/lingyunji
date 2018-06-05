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
    class LVScrollView
    {

        [SerializeField]
        GameObject m_SOLAwardItem;
        [SerializeField]
        Transform m_Grid;

        List<GameObject> m_ColumnGoList = new List<GameObject>();
        List<WelfareRwdColumn> m_ColumnInstanceList = new List<WelfareRwdColumn>();

        List<int> m_ReceivedAwdList = new List<int>(); //已领取的奖励条的index列表
        List<int> m_UnreceivedAwdList = new List<int>();//未领取的奖励条的index列表
        Dictionary<int, ItemCount[]> m_RwdTable = new Dictionary<int, ItemCount[]>();  // <LV,List<rwdID>>
        HotObjectEventAgent m_Event;
        IntBit m_RwdStatus;
        bool m_RwdReady = false;
        WelfareDB m_WelfareData;
        public void Awake()
        {
            Dictionary<int, UpgradeRewardDefine> dataConf = Config.UpgradeRewardDefine.GetAll();
            var cfgItr = dataConf.GetEnumerator();
            while (cfgItr.MoveNext())
            {
                ItemCount[] items = Config.RewardDefine.Get(cfgItr.Current.Value.award).item.items;
                m_RwdTable.Add(cfgItr.Current.Value.level, items);

                GameObject go = GameObject.Instantiate(m_SOLAwardItem);
                go.SetActive(true);
                go.transform.SetParent(m_Grid);
                go.transform.localScale = Vector3.one;
                m_ColumnGoList.Add(go);

                WelfareRwdColumn column = (WelfareRwdColumn)go.GetComponent<ILMonoBehaviour>().GetObject();
                column.SetInfoText(cfgItr.Current.Value.level.ToString() + "级");
                column.SetData(WelfareRwdColumn.ColumnType.TYPE_LVRWD, cfgItr.Current.Value.id - 1, items);
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
        void RefreshData()
        {
            m_UnreceivedAwdList.Clear();
            m_ReceivedAwdList.Clear();

            WelfareMgr mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
            m_WelfareData = mgr.m_WelfareDB;

            m_RwdStatus = new IntBit(m_WelfareData.lvRwdStatus);
            for (int i = 0; i < m_ColumnInstanceList.Count; i++)
            {
                if (!m_RwdStatus.Get(i))
                {
                    m_UnreceivedAwdList.Add(i);
                }
                else
                {
                    m_ReceivedAwdList.Add(i);
                }
            }
            m_ReceivedAwdList.Sort();
        }
        public void DisableColumn(int rwdIndex)
        {
            //m_ColumnInstanceList[rwdIndex].ShowObtainItem();
            m_RwdStatus.Set(rwdIndex,true);
            m_WelfareData.lvRwdStatus = m_RwdStatus.value;
            RefreshUI();
        }
        //根据等级更新奖励是否可领取状态
        public void RefreshRwdStatus()
        {
            
            int playerLv = App.my.localPlayer.levelValue;
            //finalindex表示0到finalindex都可以领取,先默认移动到数组尾部
            int finalIndex = WelfareMgr.GetRwdUpLimit();
            //遍历未领取列表，如果位于finalIndex中,则说明可以领取，将按钮启用
            m_RwdReady = false;
            for (int i = 0; i < m_UnreceivedAwdList.Count; i++)
            {
                //如果当前未领取列表中的奖励index比final
                if ((m_UnreceivedAwdList[i]<=finalIndex))
                {
                    m_ColumnInstanceList[m_UnreceivedAwdList[i]].SetReceiveBtnEnable();
                    m_Event.FireEvent<int>(EventID.Welfare_PageRewardReady, (int)WelfarePageType.TYPE_LV);
                    HotWelfareModule.rwdReadyStatus.Set((int)WelfarePageType.TYPE_LV,true);
                    m_RwdReady = true;
                }
                else
                {
                    m_ColumnInstanceList[m_UnreceivedAwdList[i]].SetReceiveBtnDisable();
                }
            }
            //no avaliables
            if (!m_RwdReady)
            {
                //set false
                m_RwdReady = false;
                m_Event.FireEvent<int>(EventID.Welfare_PageRewardNotReady, (int)WelfarePageType.TYPE_LV);
            }
        }

        public void RefreshUI()
        {
            RefreshData();
            m_Grid.DetachChildren();
            for (int i = 0; i < m_UnreceivedAwdList.Count; i++)
            {
                m_ColumnGoList[m_UnreceivedAwdList[i]].transform.SetParent(m_Grid);
            }
            for (int i = 0; i < m_ReceivedAwdList.Count; i++)
            {
                m_ColumnGoList[m_ReceivedAwdList[i]].transform.SetParent(m_Grid);
                m_ColumnInstanceList[m_ReceivedAwdList[i]].SetReceiveBtnReceived();
            }
            RefreshRwdStatus();
            if (m_UnreceivedAwdList.Count == 0)
            {
                m_Event.FireEvent<int>(EventID.Welfare_NoPageReward, (int)WelfarePageType.TYPE_LV);
            }
        }


    }
}

#endif
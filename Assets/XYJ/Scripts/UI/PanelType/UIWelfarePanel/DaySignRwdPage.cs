#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using Config;
using NetProto.Hot;

namespace xys.hot.UI
{
    class DaySignRwdPage : HotTablePageBase
    {
        enum SignBtnState
        {
            STATE_ENABLE,
            STATE_DISABLE
        }
        enum SubSignBtnState
        {
            STATE_HIDE,
            STATE_ENABLE,
            STATE_DISABLE
        }
        enum BtnType
        {
            TYPE_SIGN,
            TYPE_SUBSIGN
        }
        [SerializeField]
        Text m_SignInfo;

        [SerializeField]
        Button m_SignBtn;
        StateRoot m_SignBtnState;

        [SerializeField]
        Button m_SubSignBtn;
        StateRoot m_SubSignBtnState;

        [SerializeField]
        Transform m_Grid;
        [SerializeField]
        ScrollRect m_Scroll;
        [SerializeField]
        GameObject m_RewardItem;


        List<DaySignRwdItem> m_ItemInstanceList = new  List<DaySignRwdItem>();
        List<int> m_AwdIDList = new List<int>();
        Dictionary<int, ItemCount[]> m_RwdTable = new Dictionary<int, ItemCount[]>();  // <LV,List<rwdID>>

        BtnType m_SignBtnType;

        WelfareDB m_WelfareData;
        DaySignRwdPage() : base(null) { }
        DaySignRwdPage(HotTablePage page) : base(page) { }

        protected override void OnInit()
        {
            WelfareMgr mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
            m_WelfareData = mgr.m_WelfareDB;
            m_SignBtnState = m_SignBtn.GetComponent<StateRoot>();
            m_SubSignBtnState = m_SubSignBtn.GetComponent<StateRoot>();

            //Read rewardTable
            Dictionary<int, SignRewardDefine> dataConf = Config.SignRewardDefine.GetAll();
            Dictionary<int, SignRewardDefine>.Enumerator itr = dataConf.GetEnumerator();
            while (itr.MoveNext())
            {
                RewardDefine cfg = Config.RewardDefine.Get(itr.Current.Value.award);
                ItemCount[] items = cfg.item.items;
                m_RwdTable.Add(itr.Current.Value.id, items);
            }

            InitGrid();

            //关联点击事件
            m_SignBtn.onClick.AddListener(OnSignBtn);
            m_SubSignBtn.onClick.AddListener(OnSubSignBtn);
        }

        protected override void OnShow(object args)
        {
            Event.Subscribe(EventID.Welfare_DaySignRwdReceived, RefreshUI);
            Event.Subscribe<int>(EventID.Welfare_OnSignItem, OnSignItem);
            Event.Subscribe(EventID.Welfare_RefreshUI,RefreshUI);
            RefreshUI();
            m_Scroll.content.localPosition = Vector3.zero;
        }

        public void RefreshUI()
        {
            WelfareMgr mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
            m_WelfareData = mgr.m_WelfareDB;
            if (m_WelfareData.isSubSignEnable)
            {
                if (m_SubSignBtnState != null)
                {
                    m_SignInfo.gameObject.SetActive(true);
                    m_SubSignBtnState.gameObject.SetActive(true);
                    m_SubSignBtnState.SetCurrentState((int)SubSignBtnState.STATE_ENABLE, false);
                }
                ResetSubSignTimes();
            }
            else
            {
                m_SubSignBtnState.SetCurrentState((int)SubSignBtnState.STATE_DISABLE, false);
                m_SubSignBtnState.gameObject.SetActive(false);
                m_SignInfo.gameObject.SetActive(false);
            }
            ResetSignBtn();
            for (int i = 0 ;i < m_WelfareData.signDay;i++)
            {
                m_ItemInstanceList[i].SetSigned(true);
                m_ItemInstanceList[i].SetHighLight(false);
            }
            for (int i = m_WelfareData.signDay; i < m_ItemInstanceList.Count;i++)
            {
                m_ItemInstanceList[i].SetSigned(false);
                m_ItemInstanceList[i].SetHighLight(false);
            }
            if (m_WelfareData.signDay < m_WelfareData.signableDayNum)
            {
                m_ItemInstanceList[m_WelfareData.signDay].SetHighLight(true);
            }
        }

        protected override void OnHide()
        {
            Debug.Log("OnHide Called");
        }
        //初始化奖励表格
        void InitGrid()
        {
            Dictionary<int, ItemCount[]>.Enumerator itr = m_RwdTable.GetEnumerator();
            while(itr.MoveNext())
            {
                GameObject go = GameObject.Instantiate(m_RewardItem);
                go.SetActive(true);
                go.transform.SetParent(m_Grid);
                go.transform.localScale = Vector3.one;

                DaySignRwdItem item  = (DaySignRwdItem)go.GetComponent<ILMonoBehaviour>().GetObject();
                item.SetData(itr.Current.Key-1,itr.Current.Value);
                m_ItemInstanceList.Add(item);

                //如果小于已签到天数，则说明该奖励已经领取，设置已签状态
                if (itr.Current.Key <= m_WelfareData.signDay)
                {
                    item.SetSigned(true);
                }
            }
        }
        //签到回调
        void OnSignBtn()
        {
            m_SignBtnType = BtnType.TYPE_SIGN;
            if ((m_WelfareData.signDay < m_WelfareData.signableDayNum) && !m_WelfareData.isTodaySigned)
            {
                Event.fireEvent(EventID.Welfare_OnSign);
            }
        }
        //补签回调
        void OnSubSignBtn()
        {
            m_SignBtnType = BtnType.TYPE_SUBSIGN;
            if ((m_WelfareData.signDay < m_WelfareData.signableDayNum))
            {
                Event.fireEvent(EventID.Welfare_OnSubsign);
            }
        }
        //奖励图标回调
        void OnSignItem(int index)
        {
            m_SignBtnType = BtnType.TYPE_SIGN;
            if ((index==m_WelfareData.signDay) &&(m_WelfareData.signDay < m_WelfareData.signableDayNum) && !m_WelfareData.isTodaySigned)
            {
                Event.fireEvent(EventID.Welfare_OnSign);
            }
            else
            {
                m_ItemInstanceList[index].ShowItemTip();
            }
        }
        //重置补签次数
        void ResetSubSignTimes()
        {
            //可补签次数=当月可签到次数-当月已签到次数+当日是否已签到-1
            int subsignNum = m_WelfareData.signableDayNum - m_WelfareData.signDay + (m_WelfareData.isTodaySigned ? 1 : 0) - 1;
            m_SignInfo.text = string.Format("补签次数   <color=#{0}>{1}</color>", Config.ColorConfig.GetIndexByName("Y3"), subsignNum.ToString()) ;
            if (0==subsignNum)
            {
                m_SubSignBtnState.SetCurrentState((int)SubSignBtnState.STATE_DISABLE, false);
                m_SubSignBtn.enabled = false;
            }
            else
            {
                m_SubSignBtnState.SetCurrentState((int)SubSignBtnState.STATE_ENABLE, false);
                m_SubSignBtn.enabled = true;
            }
        }
        //重置签到按钮状态
        void ResetSignBtn()
        {
            if (m_WelfareData.isTodaySigned)
            {
                m_SignBtnState.SetCurrentState((int)SignBtnState.STATE_DISABLE,false);
                m_SignBtn.enabled=false;
                Event.FireEvent<int>(EventID.Welfare_PageRewardNotReady, (int)WelfarePageType.TYPE_SIGN);
            }
            else
            { 
                m_SignBtnState.SetCurrentState((int)SignBtnState.STATE_ENABLE, false);
                m_SignBtn.enabled = true;
                Event.FireEvent<int>(EventID.Welfare_PageRewardReady, (int)WelfarePageType.TYPE_SIGN);
            }
        }

    }
}

#endif
#if !USE_HOT
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using NetProto;
using System;
using NetProto.Hot;

namespace xys.hot.UI
{
    class UIWelfarePanel : HotTablePanelBase
    {
        [SerializeField]
        Text m_PageInfo;
        [SerializeField]
        Image m_SignReadySp;
        [SerializeField]
        Image m_OLReadySp;
        [SerializeField]
        Image m_LvReadySp;
        [SerializeField]
        Image m_DayOLReadySp;
        [SerializeField]
        ILMonoBehaviour m_ILScrollView;
        WelfareItemScrollView m_ScrollView;

        HotTablePanel m_Parent;
        Dictionary<string, string> pageInfoList;
        WelfareMgr m_Mgr;
        Color defaulColor;
        UIWelfarePanel() : base(null) { }
        UIWelfarePanel(HotTablePanel parent) : base(parent)
        {
            m_Parent = parent;
            pageInfoList = new Dictionary<string,string>() {
                {"LevelRwdPage","角色达到限定等级可获得等级奖励"},
                {"DayOLRwdPage","新手登录七天可领取丰厚奖励"},
                {"OnlineRwdPage","累计在线达到限定时间可领取丰厚奖励"}
            };
        }
        //初始化
        protected override void OnInit()
        {
            m_Mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
            if (m_ILScrollView != null)
                m_ScrollView = (WelfareItemScrollView)m_ILScrollView.GetObject();
            defaulColor = m_PageInfo.color;
        }
        protected override void OnShow(object args)
        {
            
            Event.Subscribe<int>(EventID.Welfare_PageRewardReady, SetPageReady);
            Event.Subscribe<int>(EventID.Welfare_PageRewardNotReady, SetPageNotReady);
            Event.Subscribe(EventID.Welfare_RefreshUI,RefreshUI);
            Event.Subscribe(EventID.Welfare_RefreshPageInfo,()=> { RefreshPageInfoText("DaySignRwdPage"); });
            Event.Subscribe<int>(EventID.Welfare_NoPageReward, (type) =>
            {
                HotWelfareModule.rwdReadyStatus.Set(type, false);
                HotWelfareModule.rwdHideStatus.Set(type, true);
                RestoreStatusToLocal();
            });
            RefreshUI();

            RefreshPageInfoText("DaySignRwdPage");
        }
        //刷新UI
        private void RefreshUI()
        {
            RefreshPageInfoText("DaySignRwdPage");
            //根据储存的奖励状态和奖励领取状态设置页面的初始状态
            CheckPageStatus(HotWelfareModule.rwdReadyStatus, SetPageReady, SetPageNotReady);
            CheckPageStatus(HotWelfareModule.rwdHideStatus, HidePage, null);
        }
        //选择页面之前调用
        protected override bool OnPreChange(xys.UI.HotTablePage page)
        {
            tableParent.ShowType(page.pageType, null);
            RefreshPageInfoText(page.pageType);
            return false;
        }
        void CheckPageStatus(IntBit statusBit,Action<int> statusFunc, Action<int> notStatusFunc)
        {
            for (int i = 0; i < tableParent.GetPageList().Count; i++)
            {
                if (statusBit.Get(i))
                {
                    if (statusFunc!=null)
                    {
                        statusFunc(i);
                    }
                    
                }
                else
                {
                    if (notStatusFunc!=null)
                    {
                        notStatusFunc(i);
                    }
                    
                }
            }
        }
        //刷新页面信息
        void RefreshPageInfoText(string pageType)
        {
            if (pageType != "DaySignRwdPage")
            {
                m_PageInfo.text = pageInfoList[pageType];
                m_PageInfo.color = defaulColor;
            }
            else
            {
                WelfareMgr mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
                
                m_PageInfo.text = string.Format("<color=#{0}>{1}</color>  月      累计签到  <color=#{2}>{3}</color>  天",
                                                Config.ColorConfig.GetIndexByName("Y3"),
                                                mgr.m_WelfareDB.playerLoginDate/100%100,
                                                Config.ColorConfig.GetIndexByName("Y3"),
                                                mgr.m_WelfareDB.signDay
                                                );
            }
        }
        //设置页面红点生效
        void SetPageReady(int pageType)
        {
            WelfarePageType type = (WelfarePageType)pageType;
            Debug.Log("Page Ready:" + type.ToString());
            switch (type)
            {
                case WelfarePageType.TYPE_SIGN:
                    {
                    	m_SignReadySp.gameObject.SetActive(true);
                    }
                    break;
                case WelfarePageType.TYPE_DAYOL:
                    {   
                        m_DayOLReadySp.gameObject.SetActive(true);
                    }
                    break;
                case WelfarePageType.TYPE_LV:
                    {
                        m_LvReadySp.gameObject.SetActive(true);
                    }
                    break;
                case WelfarePageType.TYPE_OL:
                    {
                        m_OLReadySp.gameObject.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
            HotWelfareModule.rwdReadyStatus.Set((int)type, true);
            if (HotWelfareModule.rwdReadyStatus.value >0)
                MainPanel.SetItemReadyActive((int)PanelType.UIWelfarePanel, true);
        }
        //设置页面红点失效
        void SetPageNotReady(int pageType)
        {
            WelfarePageType type = (WelfarePageType)pageType;
            Debug.Log("Page NotReady:" + type.ToString());
            switch (type)
            {
                case WelfarePageType.TYPE_SIGN:
                    {
                        m_SignReadySp.gameObject.SetActive(false);
                    }
                    break;
                case WelfarePageType.TYPE_DAYOL:
                    {
                        m_DayOLReadySp.gameObject.SetActive(false);
                    }
                    break;
                case WelfarePageType.TYPE_LV:
                    {
                        m_LvReadySp.gameObject.SetActive(false);
                    }
                    break;
                case WelfarePageType.TYPE_OL:
                    {
                        m_OLReadySp.gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
            HotWelfareModule.rwdReadyStatus.Set((int)type, false);
            if (HotWelfareModule.rwdReadyStatus.value == 0)
                MainPanel.SetItemReadyActive((int)PanelType.UIWelfarePanel, false);
        }
        void HidePage(int pageType)
        {
            WelfarePageType type = (WelfarePageType)pageType;
            Debug.Log("Hide Page:"+type.ToString());
            m_ScrollView.HideChild(type);
            //OnSelectChange((int)WelfarePageType.TYPE_SIGN);
        }
        //将缓存的位数据写回模块
        void RestoreStatusToLocal()
        {
            m_Mgr.m_WelfareDB.rwdReadyStatus = HotWelfareModule.rwdReadyStatus.value;
            m_Mgr.m_WelfareDB.rwdHideStatus = HotWelfareModule.rwdHideStatus.value;
        }
    }
}

#endif
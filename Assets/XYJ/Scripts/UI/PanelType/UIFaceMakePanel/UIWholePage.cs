#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIWholePage : HotTablePageBase
    {
        UIWholePage() : base(null) { }
        UIWholePage(HotTablePage _page) : base(_page)
        {

        }

        //配置与角色数据部分
        UIFaceMakePanel m_FaceMakeModule;

        //
        public static string[] s_tabs = new string[] { "全身", "换脸", "皮肤", "发型" };
        public static int[][] s_tabKeys = new int[][]
        {
            new int[] { 5 },
            new int[] { 4 },
            new int[] { 3 },
            new int[] { 1 , 2 },
        };
        [SerializeField]
        Transform m_TopBtnRoot;
        [SerializeField]
        GameObject m_TopBtnPref;
        public List<UITopBtnItem> m_TopBtnList = new List<UITopBtnItem>();

        [SerializeField]
        ILMonoBehaviour m_ILPrefTab;
        UIPrefTab m_PresTab;
        [SerializeField]
        ILMonoBehaviour m_ILBaseFaceTab;
        UIBaseFaceTab m_BaseFaceTab;

        RTTModelPart m_RttModelPart = null;

        public int m_currentTab = 0;
        protected override void OnInit()
        {
            if(m_ILPrefTab!=null)
            {
                m_PresTab = (UIPrefTab)m_ILPrefTab.GetObject();
                m_ILPrefTab.gameObject.SetActive(false);
            }
            if(m_ILBaseFaceTab!=null)
            {
                m_BaseFaceTab = (UIBaseFaceTab)m_ILBaseFaceTab.GetObject();
            }
        }
        protected override void OnShow(object args)
        {
            if(args==null)
            {
                Debug.Log("args==null");
                return;
            }
            Event.Subscribe(EventID.AP_RefreshUI, RefreshUI);
            Debug.Log("Show" + parent.name);
            m_FaceMakeModule = (args as PanelInfo).m_Panel;
            m_currentTab = 0;
            AddBtn(m_currentTab);
        }

        void AddBtn(int _curTab)
        {
            m_TopBtnList.Clear();
            if (m_FaceMakeModule != null)
            {
                int btnCount = s_tabs.Length;

                for (int i = 0; i < btnCount; i++)
                {
                    if (m_TopBtnRoot == null) return;
                    int count = m_TopBtnRoot.childCount;
                    GameObject tempObj = null;
                    if (i < count)
                    {
                        Transform tempTran = m_TopBtnRoot.GetChild(i);
                        if (tempTran != null)
                        {
                            tempObj = tempTran.gameObject;
                        }
                    }
                    else
                    {
                        tempObj = GameObject.Instantiate(m_TopBtnPref);
                    }
                    if (tempObj == null) return;
                    tempObj.SetActive(true);
                    tempObj.transform.SetParent(m_TopBtnRoot, false);
                   
                    tempObj.transform.localScale = Vector3.one;
                    ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                    if (ILObj == null) return;
                    UITopBtnItem tempBtnItem = (UITopBtnItem)ILObj.GetObject();
                    if (tempBtnItem == null) return;
                    tempBtnItem.Set(i, OnClickItem, OnItemStateChange, s_tabs[i]);

                    m_TopBtnList.Add(tempBtnItem);
                }

            }
            m_TopBtnList[_curTab].m_StateRoot.SetCurrentState(1, true);

         
            if (m_FaceMakeModule != null)
            {
                m_RttModelPart = m_FaceMakeModule.GetRttModlePart();
                if (m_RttModelPart != null)
                {
                    if(_curTab==0)
                    {
                        m_RttModelPart.SetCamState(0, false);
                    }
                    else
                    {
                        m_RttModelPart.SetCamState(2, false);
                    }       
                }
            } 
        }

        void OnClickItem(UITopBtnItem item)
        {
            if(m_currentTab==item.m_Index)
            {
                return;
            }
            else
            {
                if(m_FaceMakeModule.GetDisguiseHandle().m_sex==1)
                {
                    if(item.m_Index==1)
                    {
                        SystemHintMgr.ShowHint("此功能暂未开放！");
                        return;
                    }
                }
                m_TopBtnList[m_currentTab].m_StateRoot.SetCurrentState(0, true);
                m_currentTab = item.m_Index;
                item.m_StateRoot.SetCurrentState(1, true);
            }
        }
        void OnItemStateChange(UITopBtnItem item)
        {
            if(item.m_StateRoot.CurrentState==0)
            {
                //关闭tab
                if (item.m_Index == 0)
                {
                    m_BaseFaceTab.CloseTab();
                    m_ILBaseFaceTab.gameObject.SetActive(false); 
                }
                else
                {
                    m_PresTab.CloseTab();
                    m_ILPrefTab.gameObject.SetActive(false);
                }
                
            }
            else
            {
                //OpenTab;
                if (m_currentTab == 0)
                {
                    m_ILBaseFaceTab.gameObject.SetActive(true);
                    m_BaseFaceTab.OpenTab(m_FaceMakeModule);
                }
                else
                {
                    m_ILPrefTab.gameObject.SetActive(true);
                    m_PresTab.OpenTab(m_currentTab, m_FaceMakeModule);
                }
                
            }
        }
        protected override void OnHide()
        {
            Debug.Log("HidePage");
            m_PresTab.CloseTab();
            m_BaseFaceTab.CloseTab();
            if(m_TopBtnRoot!=null)
            {
                for(int i=0;i<m_TopBtnRoot.childCount;i++)
                {
                    m_TopBtnRoot.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        void RefreshUI()
        {
            m_BaseFaceTab.RefreshUI();
        }




    }
}
#endif
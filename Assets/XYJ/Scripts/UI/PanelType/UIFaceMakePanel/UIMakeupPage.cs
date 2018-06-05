#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIMakeupPage : HotTablePageBase
    {
        UIMakeupPage() : base(null) { }
        UIMakeupPage(HotTablePage _page) : base(_page)
        {

        }


        UIFaceMakePanel m_FaceMakeModule;

        List<UITopBtnItem> m_TopBtnList = new List<UITopBtnItem>();

        [SerializeField]
        GameObject m_TopBtnPrefab;
        [SerializeField]
        Transform m_TopBtnRoot;

        [SerializeField]
        ILMonoBehaviour m_ILSkinPartTab;
        UISkinPartTab m_SkinPartTab;


        RTTModelPart m_RttModelPart = null;
        public int m_currentTab = 0;
        protected override void OnInit()
        {
            if(m_ILSkinPartTab!=null)
            {
                m_SkinPartTab = (UISkinPartTab)m_ILSkinPartTab.GetObject();
            }

        }

        protected override void OnShow(object args)
        {
            if (args == null)
            {
                Debug.LogError("args == null");
                return;
            }

            Debug.Log("args:" + args.GetType().Name);
            m_FaceMakeModule = (args as PanelInfo).m_Panel;
            if(m_FaceMakeModule!=null)
            {
                m_RttModelPart=m_FaceMakeModule.GetRttModlePart();
                if(m_RttModelPart!=null)
                {
                    m_RttModelPart.SetCamState(2, false);
                }
            }
            AddBtn(m_currentTab);
        }
        void AddBtn(int _curTab)
        {
            m_TopBtnList.Clear();
            if(m_FaceMakeModule!=null)
            {
                RoleSkinPart data = m_FaceMakeModule.GetSkinPartConfig();

                string[] keys= data.tabsName;
                int btnCount = keys.Length;

                for(int i=0;i<btnCount;i++)
                {
                    if (m_TopBtnRoot == null) return;
                    int count = m_TopBtnRoot.childCount;
                    GameObject tempObj = null;
                    if (i<count)
                    {
                        Transform tempTran = m_TopBtnRoot.GetChild(i);
                        if (tempTran != null)
                        {
                            tempObj = tempTran.gameObject; 
                        }
                    }
                    else
                    {
                        tempObj = GameObject.Instantiate(m_TopBtnPrefab);
                    }                       
                    if (tempObj == null) return;

                    tempObj.SetActive(true);                   
                    tempObj.transform.SetParent(m_TopBtnRoot,false);
      
                    tempObj.transform.localScale = Vector3.one;
                    ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                    if (ILObj == null) return;
                    UITopBtnItem tempBtnItem = (UITopBtnItem)ILObj.GetObject();
                    if (tempBtnItem == null) return;
                    tempBtnItem.Set(i, OnClickItem, OnItemStateChange, data.tabsName[i]);
                       
                    m_TopBtnList.Add(tempBtnItem);
                }
                    
            }
            m_TopBtnList[_curTab].m_StateRoot.SetCurrentState(1, true);
                   
        }
        void OnClickItem(UITopBtnItem item)
        {
            if(m_currentTab==item.m_Index)
            {
                return;
            }
            else
            {
                m_TopBtnList[m_currentTab].m_StateRoot.SetCurrentState(0, true);
                m_currentTab = item.m_Index;
                item.m_StateRoot.SetCurrentState(1, true);
            }
        }

        void OnItemStateChange(UITopBtnItem item)
        {
            //打开或关闭相应的选项卡
            if(item.m_StateRoot.CurrentState==0)
            {
                //关闭
                Debug.Log("Close tab " + item.m_Index);
                if(m_SkinPartTab!=null)
                {
                    m_SkinPartTab.CloseTab();
                }
            }
            else
            {
                //打开
                Debug.Log("Open tab " + item.m_Index);
                if (m_SkinPartTab != null)
                {
                    m_SkinPartTab.OpenTab(item.m_Index, m_FaceMakeModule);
                }
            }
        }
        protected override void OnHide()
        {
            Debug.Log("HidePage");
            m_SkinPartTab.CloseTab();
            if (m_TopBtnRoot != null)
            {
                for (int i = 0; i < m_TopBtnRoot.childCount; i++)
                {
                    m_TopBtnRoot.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
#endif
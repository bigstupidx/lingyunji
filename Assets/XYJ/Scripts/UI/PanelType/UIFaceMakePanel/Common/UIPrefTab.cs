#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIPrefTab
    {
        [SerializeField]
        GameObject m_PrefPartItemPref;

        [SerializeField]
        Transform m_TabRoot;

        [SerializeField]
        Button m_ResetBtn;

        public int m_curPartIndex = 0;
        public int m_TabIndex = 0;

        private List<UIPrefPartItem> m_PrefPartItemList = new List<UIPrefPartItem>();
        private UIFaceMakePanel m_FaceMakeModule = null;
        RTTModelPart m_RttModelPart = null;

        string[] name = { "发型", "发色" ,"皮肤","换脸","默认"};
        public void OpenTab(int index , UIFaceMakePanel faceMakeModule)
        {
            InitTab();
            
            m_FaceMakeModule = faceMakeModule;
            if (m_FaceMakeModule != null)
            {
                m_RttModelPart = m_FaceMakeModule.GetRttModlePart();
                if (m_RttModelPart != null)
                {
                    if(index==1)
                    {
                        m_RttModelPart.SetCamState(2, false);
                    }
                    else if(index == 2)
                    {
                        m_RttModelPart.SetCamState(0, false);
                    }
                    else if(index==3)
                    {
                        m_RttModelPart.SetCamState(1, false);
                    }
                }
            }
           
            RoleDisguiseHandle prefHandle= faceMakeModule.GetDisguiseHandle();
            RoleDisguiseCareer config = prefHandle.GetOverallConfig();

            m_TabIndex = index;

            if(m_ResetBtn!=null)
            {
                m_ResetBtn.gameObject.SetActive(true);
                m_ResetBtn.GetComponentInChildren<Text>().text = "重置" + UIWholePage.s_tabs[m_TabIndex];

                m_ResetBtn.onClick.AddListener(OnClickReset);
            }
            int[] tabsKeys = UIWholePage.s_tabKeys[index];
            int partCount = tabsKeys.Length;  
            for (int i=0;i<partCount;i++)
            {
                var type = tabsKeys[i];
                RoleDisguiseType partData = config.GetType(type);
                if (partData == null) return;

                if (m_TabRoot == null) return;
                int curChildCount = m_TabRoot.childCount;
                GameObject tempObj = null;
                if(i< curChildCount)
                {
                    tempObj = m_TabRoot.GetChild(i).gameObject;
                }
                else
                {
                    tempObj = GameObject.Instantiate(m_PrefPartItemPref);
                }
                if (tempObj == null) return;
                tempObj.SetActive(true);
                tempObj.transform.SetParent(m_TabRoot, false);
                tempObj.transform.localScale = Vector3.one;
                ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                UIPrefPartItem item = (UIPrefPartItem)ILObj.GetObject();

                item.Set(i, name[type-1], OnClickItem, OnItemStateChange);
                m_PrefPartItemList.Add(item);
            }
            m_PrefPartItemList[m_curPartIndex].m_StateRoot.SetCurrentState(1, true);
            
            if(partCount == 1)
            {
                m_PrefPartItemList[0].m_StateRoot.gameObject.SetActive(false);
            }
        }
        //初始化类的数据
        void InitTab()
        {
            m_PrefPartItemList.Clear();
            m_curPartIndex = 0;
            m_TabIndex = 0;
            m_FaceMakeModule = null;
        }
        //还原游戏对象的设置
        public void CloseTab()
        {
            if (m_TabRoot!=null)
            {
                int count = m_TabRoot.childCount;
                for (int i = 0; i < count; i++)
                {
                    m_TabRoot.GetChild(i).gameObject.SetActive(false);
                }
            }
            for(int i=0; i<m_PrefPartItemList.Count;i++)
            {
                m_PrefPartItemList[i].ClosePart();
            }
            if(m_ResetBtn!=null)
            {
                m_ResetBtn.gameObject.SetActive(false);
                m_ResetBtn.onClick.RemoveAllListeners();
            }
        }
        //点击Item回调
        void OnClickItem(UIPrefPartItem item)
        {
            if(m_curPartIndex == item.m_PartIndex)
            {
                if(item.m_StateRoot.CurrentState==0)
                {
                    item.m_StateRoot.SetCurrentState(1, true);
                }
                else
                {
                    item.m_StateRoot.SetCurrentState(0, true);
                }
            }
            else
            {
                m_PrefPartItemList[m_curPartIndex].m_StateRoot.SetCurrentState(0, true);            
                item.m_StateRoot.SetCurrentState(1, true);
            }
            
        }
        //item的state状态变化时回调
        void OnItemStateChange(UIPrefPartItem item)
        {
            if(item.m_StateRoot.CurrentState==0)
            {
                item.ClosePart();
            }
            else
            {
                m_curPartIndex = item.m_PartIndex;
                item.OpenPart(m_TabIndex, m_FaceMakeModule);
            }
        }
        //点击重置
        void OnClickReset()
        {
            int[] tab = UIWholePage.s_tabKeys[m_TabIndex];
            RoleDisguiseHandle prefHandle = m_FaceMakeModule.GetDisguiseHandle();
            for (int i=0;i<tab.Length;i++)
            {
                var type = tab[i]; 
                prefHandle.ResetType(type);
            }
            m_PrefPartItemList[m_curPartIndex].ResetUI();
        }
    }
}
#endif
#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIModifyPage : HotTablePageBase
    {
        UIModifyPage() : base(null) { }

        UIModifyPage(HotTablePage _page) : base(_page)
        {

        }

        UIFaceMakePanel m_FaceMakeModule=null;
        public int m_curTab = 0;
        List<UITopBtnItem> m_TopBtnList = new List<UITopBtnItem>();

        [SerializeField]
        GameObject m_TopBtnPrefab;
        [SerializeField]
        Transform m_TopBtnRoot;

        [SerializeField]
        ILMonoBehaviour m_ILShapeTab;
        UIShapeTab m_ShapeTab;

        RTTModelPart m_RttModelPart = null;
        protected override void OnInit()
        {
            if(m_ILShapeTab!=null)
            {
                m_ShapeTab = (UIShapeTab)m_ILShapeTab.GetObject();
            }
        }

        protected override void OnShow(object arg)
        {
            if (arg == null)
            {
                Debug.LogError("arg == null");
                return;
            }

            m_FaceMakeModule = (arg as PanelInfo).m_Panel;
            if (m_FaceMakeModule != null)
            {
                m_RttModelPart = m_FaceMakeModule.GetRttModlePart();
                if (m_RttModelPart != null)
                {
                    m_RttModelPart.SetCamState(2, false);
                }
            }
            AddTopBtn(m_curTab);
        }
        void AddTopBtn(int tabIndex)
        {
            m_TopBtnList.Clear();
            if (m_FaceMakeModule == null) return;
            RoleShapeConfig shapeConfig = m_FaceMakeModule.GetShapeConfig();
            int topBtnCount = shapeConfig.faceParts.Count;
            for(int i=0;i<topBtnCount;i++)
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
                    tempObj = GameObject.Instantiate(m_TopBtnPrefab);
                }
                if (tempObj == null) return;
                tempObj.SetActive(true);
                tempObj.transform.SetParent(m_TopBtnRoot, false);
       
                tempObj.transform.localScale = Vector3.one;
                ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                if (ILObj == null) return;
                UITopBtnItem tempBtnItem = (UITopBtnItem)ILObj.GetObject();
                if (tempBtnItem == null) return;
                tempBtnItem.Set(i, OnClickItem, OnItemStateChange, shapeConfig.faceParts[i].name);

                m_TopBtnList.Add(tempBtnItem);
            }
            m_TopBtnList[m_curTab].m_StateRoot.SetCurrentState(1, true);
        }
        void OnClickItem(UITopBtnItem item)
        {
            if (m_curTab == item.m_Index)
            {
                return;
            }
            else
            {
                m_TopBtnList[m_curTab].m_StateRoot.SetCurrentState(0, true);   
                item.m_StateRoot.SetCurrentState(1, true);
            }
        }
        void OnItemStateChange(UITopBtnItem item)
        {
            if (item.m_StateRoot.CurrentState == 0)
            {
                //关闭tab
                m_ShapeTab.CloseTab();
            }
            else
            {
                //OpenTab;
                m_curTab = item.m_Index;
                m_ShapeTab.OpenTab(m_curTab, m_FaceMakeModule);
            }
        }
        protected override void OnHide()
        {
            Debug.Log("HidePage");
            m_ShapeTab.CloseTab();
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
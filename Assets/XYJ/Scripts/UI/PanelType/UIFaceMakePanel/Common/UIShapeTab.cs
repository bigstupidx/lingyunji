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
    class UIShapeTab
    {
        [SerializeField]
        GameObject m_ShapePartPref;

        [SerializeField]
        Transform m_TabRoot;

        [SerializeField]
        Button m_ResetBtn;


        public int m_curPartIndex = 0;
        public int m_TabIndex = 0;

        private List<UIShapePart> m_ShapePartList = new List<UIShapePart>();
        private UIFaceMakePanel m_FaceMakeModule = null;

        public void OpenTab(int tabIndex, UIFaceMakePanel faceMakeModule)
        {
            InitTab();
            m_FaceMakeModule = faceMakeModule;
            m_TabIndex = tabIndex;
            RoleShapeConfig shapeConfig = faceMakeModule.GetShapeConfig();
            RoleShapePart shapePart = shapeConfig.faceParts[tabIndex];
            if (m_ResetBtn != null)
            {
                m_ResetBtn.gameObject.SetActive(true);
                m_ResetBtn.GetComponentInChildren<Text>().text = "重置" + shapePart.name;
                m_ResetBtn.onClick.AddListener(OnClickReset);
            }

            int subPartCount = shapePart.subParts.Count;
            for(int i=0;i<subPartCount;i++)
            {
                if (m_TabRoot == null) return;
                int curChildCount = m_TabRoot.childCount;
                GameObject tempObj = null;
                if(i<curChildCount)
                {
                    Transform tempTran = m_TabRoot.GetChild(i);
                    if (tempTran == null) return;
                    tempObj = tempTran.gameObject;
                }
                else
                {
                    tempObj = GameObject.Instantiate(m_ShapePartPref);
                }
                if (tempObj == null) return;
                tempObj.SetActive(true);
                tempObj.transform.SetParent(m_TabRoot,false);
                tempObj.transform.localScale = Vector3.one;
                ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                if (ILObj == null) return;
                UIShapePart item = (UIShapePart)ILObj.GetObject();
                item.Set(m_TabIndex,i, shapePart.subParts[i].name,m_FaceMakeModule, OnClickItem, OnItemStateChange);               
                m_ShapePartList.Add(item);
            }
            m_ShapePartList[m_curPartIndex].m_StateRoot.SetCurrentState(1, true);
            if(m_ShapePartList.Count==1)
            {
                m_ShapePartList[0].m_StateRoot.gameObject.SetActive(false);
            }
        }

        void InitTab()
        {
            m_curPartIndex = 0;
            m_TabIndex = 0;
            m_ShapePartList.Clear();
            m_FaceMakeModule = null;
        }
        public void CloseTab()
        {
            if(m_TabRoot!=null)
            {
                for(int i=0;i<m_TabRoot.childCount;i++)
                {
                    m_TabRoot.GetChild(i).gameObject.SetActive(false);
                }
            }
            for(int i=0;i<m_ShapePartList.Count;i++)
            {
                m_ShapePartList[i].ClosePart();
            }
            if (m_ResetBtn != null)
            {
                m_ResetBtn.gameObject.SetActive(false);
                m_ResetBtn.onClick.RemoveAllListeners();

            }
        }

        void OnClickItem(UIShapePart item)
        {
            if(m_curPartIndex==item.m_PartIndex)
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
                if(m_ShapePartList[m_curPartIndex].m_StateRoot.CurrentState==1)
                {
                    m_ShapePartList[m_curPartIndex].m_StateRoot.SetCurrentState(0, true);
                }
                
                item.m_StateRoot.SetCurrentState(1, true);
            }
        }
        void OnItemStateChange(UIShapePart item)
        {
            if(item.m_StateRoot.CurrentState==0)
            {
                item.ClosePart();
            }
            else
            {
                m_curPartIndex = item.m_PartIndex;
                item.OpenPart();
            }
        }

        void OnClickReset()
        {
            RoleShapeHandle shapeHandle = m_FaceMakeModule.GetShapeHandle();
            shapeHandle.ResetPart(m_TabIndex);
            UIFaceMakePanel.m_NeedComfirm = true;
            //刷新UI
            m_ShapePartList[m_curPartIndex].Reset();
        }
    }
}
#endif 
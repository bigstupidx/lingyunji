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
    class UIShapePart
    {
        [SerializeField]
        public StateRoot m_StateRoot;
        [SerializeField]
        Text m_TitleText;

        [SerializeField]
        Transform m_SliderRoot;
        [SerializeField]
        GameObject m_SliderItemPref;

        UIFaceMakePanel m_FaceMakeModule = null;
        public int m_PartIndex = 0;
        public int m_CurTabIndex = 0;
        private List<UIShapeSliderItem> m_SliderItemList = new List<UIShapeSliderItem>();

        private System.Action<UIShapePart> m_ClickEvent = null;
        private System.Action<UIShapePart> m_StateChangeEvent = null;

        public void Set(int tabIndex,int partIndex,string partName,UIFaceMakePanel faceMakeModule, System.Action<UIShapePart> clickEvent, System.Action<UIShapePart> stateChangeEvent)
        {
            InitPart();
            m_CurTabIndex = tabIndex;
            m_PartIndex = partIndex;
            m_TitleText.text = partName;
            m_ClickEvent = clickEvent;
            m_StateChangeEvent = stateChangeEvent;
            m_FaceMakeModule = faceMakeModule;
        }
        void InitPart()
        {
            m_StateRoot.SetCurrentState(0, false);
            m_StateRoot.gameObject.SetActive(true);
            m_PartIndex = 0;
            m_CurTabIndex = 0;
            m_ClickEvent = null;
            m_StateChangeEvent = null;
            if (m_StateRoot != null)
            {
                m_StateRoot.onClick.RemoveAllListeners();
                m_StateRoot.onStateChange.RemoveAllListeners();

                m_StateRoot.onClick.AddListener(OnClick);
                m_StateRoot.onStateChange.AddListener(OnStateChange);

            }
        }
        public void OpenPart()
        {
            RoleShapeHandle shapeHandle = m_FaceMakeModule.GetShapeHandle();
            RoleShapeConfig shapeConfig = m_FaceMakeModule.GetShapeConfig();
            RoleShapePart shapePart = shapeConfig.faceParts[m_CurTabIndex];
            RoleShapeSubPart shapeSubPart = shapePart.subParts[m_PartIndex];
            if(m_SliderRoot!=null)
            {
                m_SliderRoot.gameObject.SetActive(true);
            }
            int subPartCount = shapeSubPart.units.Count;
            for(int i=0;i<subPartCount;i++)
            {
                var data = shapeSubPart.units[i];
                int curChildCount = m_SliderRoot.childCount;
                GameObject tempObj = null;
                if(i<curChildCount)
                {
                    Transform tempTran = m_SliderRoot.GetChild(i);
                    if (tempTran == null) return;
                    tempObj = tempTran.gameObject;
                }
                else
                {
                    tempObj = GameObject.Instantiate(m_SliderItemPref);
                }
                tempObj.SetActive(true);
                tempObj.transform.SetParent(m_SliderRoot,false);
                tempObj.transform.localScale = Vector3.one;

                ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                UIShapeSliderItem item = (UIShapeSliderItem)ILObj.GetObject();
                float value = shapeHandle.GetValue01(data);
                item.Set(i, value, data.name, OnValueChange);
                m_SliderItemList.Add(item);
            }
        }
        
        public void ClosePart()
        {
            m_SliderItemList.Clear();
            if (m_SliderRoot != null)
            {
                for (int i = 0; i < m_SliderRoot.childCount; i++)
                {
                    m_SliderRoot.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        void OnValueChange(UIShapeSliderItem item, System.Single value)
        {

            RoleShapeConfig shapeConfig = m_FaceMakeModule.GetShapeConfig();
            RoleShapePart shapePart = shapeConfig.faceParts[m_CurTabIndex];
            RoleShapeSubPart shapeSubPart = shapePart.subParts[m_PartIndex];
            var data = shapeSubPart.units[item.m_Index];
            RoleShapeHandle shapeHandle = m_FaceMakeModule.GetShapeHandle();
            shapeHandle.SetValue01(data, value);

            UIFaceMakePanel.m_NeedComfirm = true;

        }
        void OnClick()
        {
            if (m_ClickEvent != null)
            {
                m_ClickEvent(this);
            }
        }
        void OnStateChange()
        {
            if(m_StateChangeEvent!=null)
            {
                m_StateChangeEvent(this);
            }
        }

        public void Reset()
        {
            if(m_FaceMakeModule==null)
            {
                Debug.Log("找不到数据模块");
                return;
            }
            //重置UI
            if(m_StateRoot.CurrentState==1)
            {
                RoleShapeHandle shapeHandle = m_FaceMakeModule.GetShapeHandle();
                RoleShapeConfig shapeConfig = m_FaceMakeModule.GetShapeConfig();
                RoleShapePart shapePart = shapeConfig.faceParts[m_CurTabIndex];
                RoleShapeSubPart shapeSubPart = shapePart.subParts[m_PartIndex];

                for (int i = 0; i < m_SliderItemList.Count; i++)
                {
                    var data = shapeSubPart.units[i];
                    m_SliderItemList[i].m_Slider.value = shapeHandle.GetValue01(data);;
                }
            }
            
        }
    }
}
#endif
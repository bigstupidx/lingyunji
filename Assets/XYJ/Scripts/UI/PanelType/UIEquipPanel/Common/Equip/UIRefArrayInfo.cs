#if !USE_HOT
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using xys.UI.State;
using NetProto;
using NetProto.Hot;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
namespace xys.hot.UI
{
    [System.Serializable]
    class UIRefArrayInfo
    {

        [SerializeField]
        Transform m_OriginAttrsInfo;
        [SerializeField]
        Transform m_NewAttrsInfo;
        [SerializeField]
        Transform m_LeftEmptyInfo;
        [SerializeField]
        Transform m_RightEmptyInfo;

        List<RefInfo> m_OriginAttrList = new List<RefInfo>();
        List<RefInfo> m_NewAttrList = new List<RefInfo>();

        EquipMgr m_EquipMgr;
        const int refInfoMaxLv = 80;
        public  enum propType
        {
            enRefProp = 0,
            enRefBackProp = 1, 
        }

        public UIRefArrayInfo()
        {
        }

        public void OnInit()
        {
            for (int i = 0; i < m_OriginAttrsInfo.childCount; i++)
            {
                RefInfo info = new RefInfo(m_OriginAttrsInfo.GetChild(i));
                m_OriginAttrList.Add(info);
            }
            for (int i = 0; i < m_NewAttrsInfo.childCount; i++)
            {
                RefInfo info = new RefInfo(m_NewAttrsInfo.GetChild(i));
                m_NewAttrList.Add(info);
            }

        }
        public void ClearPropertyInfo()
        {
            m_RightEmptyInfo.gameObject.SetActive(true);
            m_NewAttrsInfo.gameObject.SetActive(false);

            m_LeftEmptyInfo.gameObject.SetActive(true);
            m_OriginAttrsInfo.gameObject.SetActive(false);
        }
        public void RefreshPropInfo(ItemData data)
        {
            EquipBasicData equipBasicData = data.equipdata.equipBasicData;
            int subType = equipBasicData.nSubType;

            bool isShow = equipBasicData.tempRefAtts.Count > 0;
            m_RightEmptyInfo.gameObject.SetActive(!isShow);
            m_NewAttrsInfo.gameObject.SetActive(isShow);

            isShow = equipBasicData.refAtts.Count > 0;
            m_LeftEmptyInfo.gameObject.SetActive(!isShow);
            m_OriginAttrsInfo.gameObject.SetActive(isShow);
            

            SetDataToAttrList(equipBasicData.tempRefAtts,m_NewAttrList);
            SetDataToAttrList(equipBasicData.refAtts, m_OriginAttrList);
        }

        void SetDataToAttrList(List<RefAttr> attrList,List<RefInfo> infoList)
        {
            string attrName;
            double value = 0;
            double upLimit = 0;
            int quality = 0;
            int attrID = 0;
            for (int i = 0; i < attrList.Count; i++)
            {
                attrID = attrList[i].key;
                attrName = Config.AttributeDefine.Get(attrID).attrName;
                infoList[i].SetName(attrID,attrName, infoList[i].defaultColor);

                value = attrList[i].value;
                quality = attrList[i].quality;
                upLimit = Config.RefinePropertyValueLibrary.Get(refInfoMaxLv).battleAttri.Get(attrID);
                infoList[i].SetValue(value, upLimit, quality);
                infoList[i].Show();
            }
            for (int i = attrList.Count; i < infoList.Count; i++)
            {
                infoList[i].Hide();
            }
        }
        public void SetActive(bool active)
        {
            m_OriginAttrsInfo.gameObject.SetActive(active);
            m_NewAttrsInfo.gameObject.SetActive(active);
            m_LeftEmptyInfo.gameObject.SetActive(active);
            m_RightEmptyInfo.gameObject.SetActive(active);
        }
    }
}
#endif

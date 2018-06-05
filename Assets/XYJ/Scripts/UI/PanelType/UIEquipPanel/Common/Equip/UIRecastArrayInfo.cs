#if !USE_HOT
using UnityEngine;
using System.Collections.Generic;
using NetProto.Hot;
using Config;
using WXB;
using NetProto;
using UnityEngine.UI;
using System.Text;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
namespace xys.hot.UI
{
    [System.Serializable]
    class UIRecastArrayInfo
    {
        public enum ShowType
        {
            TYPE_RECAST,
            TYPE_CONCISE
        }
        [SerializeField]
        Transform m_OriginAttrsInfo;
        [SerializeField]
        Transform m_NewAttrsInfo;
        [SerializeField]
        Transform m_LeftEmptyInfo;
        [SerializeField]
        Transform m_RightEmptyInfo;
        [SerializeField]
        Transform m_EffectTrans;
        [SerializeField]
        Transform m_tempEffectTrans;

        Text m_Effect1;
        Text m_Effect2;
        Text m_tempEffect1;
        Text m_tempEffect2;

        List<RecastInfo> m_OriginAttrList = new List<RecastInfo>();
        List<RecastInfo> m_NewAttrList = new List<RecastInfo>();
        Vector3 m_DefaultEffectTransPos = new Vector3();
        Vector3 m_DefaultTempEffectTransPos = new Vector3();
        EquipMgr m_EquipMgr;

        enum PropType
        {
            TYPE_BASE,
            TYPE_CUSTOMS,
            TYPE_RANDOM
        }

        public UIRecastArrayInfo()
        {
        }

        public void OnInit()
        {
            for (int i = 0; i < m_OriginAttrsInfo.childCount; i++)
            {
                RecastInfo info = new RecastInfo(m_OriginAttrsInfo.GetChild(i));
                m_OriginAttrList.Add(info);
            }
            for (int i = 0; i < m_NewAttrsInfo.childCount; i++)
            {
                RecastInfo info = new RecastInfo(m_NewAttrsInfo.GetChild(i));
                m_NewAttrList.Add(info);
            }
            if (m_EffectTrans != null)
            {
                m_Effect1 = m_EffectTrans.Find("Text").GetComponent<Text>();
                m_Effect2 = m_EffectTrans.Find("Text1").GetComponent<Text>();
            }
            if (m_tempEffectTrans != null)
            {
                m_tempEffect1 = m_tempEffectTrans.Find("Text").GetComponent<Text>();
                m_tempEffect2 = m_tempEffectTrans.Find("Text1").GetComponent<Text>();
            }
            m_DefaultEffectTransPos = m_EffectTrans.localPosition;
            m_DefaultTempEffectTransPos = m_tempEffectTrans.localPosition;
        }

        public void ClearPropertyInfo()
        {
            m_RightEmptyInfo.gameObject.SetActive(true);
            m_NewAttrsInfo.gameObject.SetActive(false);
            m_EffectTrans.gameObject.SetActive(false);
            m_LeftEmptyInfo.gameObject.SetActive(true);
            m_OriginAttrsInfo.gameObject.SetActive(false);
            m_tempEffectTrans.gameObject.SetActive(false);
        }
        public void RefreshPropInfo(ItemData data, ShowType showType)
        {

            bool isShow = false;
            switch (showType)
            {
                case ShowType.TYPE_RECAST:
                    isShow = data.equipdata.equipBasicData.tempBaseAttsByRecast.Count > 0;
                    break;
                case ShowType.TYPE_CONCISE:
                    isShow = data.equipdata.equipBasicData.tempBaseAttsByConcise.Count > 0;
                    break;
                default:
                    break;
            }

            m_RightEmptyInfo.gameObject.SetActive(!isShow);
            m_NewAttrsInfo.gameObject.SetActive(isShow);
            m_tempEffectTrans.gameObject.SetActive(isShow);

            isShow = data.equipdata.equipBasicData.baseAtts.Count > 0;
            m_LeftEmptyInfo.gameObject.SetActive(!isShow);
            m_OriginAttrsInfo.gameObject.SetActive(isShow);
            m_EffectTrans.gameObject.SetActive(isShow);
            m_EffectTrans.localPosition  = m_DefaultEffectTransPos;
            m_tempEffectTrans.localPosition  = m_DefaultTempEffectTransPos;
            SetEffectActive(false);
            SetDataToAttrList(data, showType);
        }

        void SetDataToAttrList(ItemData data, ShowType showType)
        {
            //data.equipdata.baseAtts, data.equipdata.randomAtts;
            //    data.equipdata.tempBaseAtts, data.equipdata.tempRandomAtts;
            EquipBasicData equipBasicData = data.equipdata.equipBasicData;
            for (int i = 0; i < m_OriginAttrList.Count; i++)
            {
                m_OriginAttrList[i].Hide();
                m_NewAttrList[i].Hide();
            }
            int index = 0;
            SetDataToAttrListCore(data, equipBasicData.baseAtts, m_OriginAttrList, PropType.TYPE_BASE,ref index);
            if (showType == ShowType.TYPE_RECAST)
            {
                //set originAttr info
                SetDataToAttrListCore(data, equipBasicData.customAtts, m_OriginAttrList, PropType.TYPE_CUSTOMS, ref index);
                SetDataToAttrListCore(data, equipBasicData.randomAtts, m_OriginAttrList, PropType.TYPE_RANDOM, ref index);
                //set effect&temp effect
                SetEffectData(data);
                //adjust pos of effect trans
                var yPos = GetFirstInactiveInfoPosInY(m_OriginAttrList);
                if(yPos!=Vector3.zero)
                    m_EffectTrans.position = new Vector3(m_EffectTrans.position.x, yPos.y, 0) ;
            }
            index = 0;
            
            if (showType == ShowType.TYPE_RECAST)
            {
                SetDataToAttrListCore(data, equipBasicData.tempBaseAttsByRecast, m_NewAttrList, PropType.TYPE_BASE, ref index);
                SetDataToAttrListCore(data, equipBasicData.tempCustomAtts, m_NewAttrList, PropType.TYPE_CUSTOMS, ref index);
                SetDataToAttrListCore(data, equipBasicData.tempRandomAtts, m_NewAttrList, PropType.TYPE_RANDOM, ref index);
                var yPos = GetFirstInactiveInfoPosInY(m_NewAttrList);
                if (yPos != Vector3.zero)
                    m_tempEffectTrans.position = new Vector3(m_tempEffectTrans.position.x, yPos.y, 0);
            }
            if (showType == ShowType.TYPE_CONCISE)
                SetDataToAttrListCore(data, equipBasicData.tempBaseAttsByConcise, m_NewAttrList, PropType.TYPE_BASE, ref index);

            if (equipBasicData.tempBaseAttsByRecast.Count!=0|| equipBasicData.tempBaseAttsByConcise.Count != 0)
            {
                AdjustAttrInfoList(data, showType);
            }
        }
        void SetDataToAttrListCore(ItemData data, Dictionary<int,double> attrs, List<RecastInfo> infoList, PropType type, ref int startIndex)
        {
            var itr = attrs.GetEnumerator();
            string attrName;
            double value = 0;
            double min=0,max = 0;
            int attrID = 0;
            UnityEngine.Color color ;
            EquipBasicData equipBasicData = data.equipdata.equipBasicData;
            while (itr.MoveNext()&& (startIndex < infoList.Count))
            {

                attrID = itr.Current.Key;
                attrName = Config.AttributeDefine.Get(attrID).attrName;

                color = infoList[startIndex].defaultColor;
                value = EquipCfgMgr.GetPropertValInTotal(data.id, equipBasicData.nSubType, itr.Current.Value, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus);
                switch (type)
                {
                    case PropType.TYPE_BASE:
                        EquipCfgMgr.GetBaseAttrRange(data.id, equipBasicData.nSubType, attrID, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus,out min,out max);
                        break;
                    case PropType.TYPE_CUSTOMS:
                        EquipCfgMgr.GetCustomAttrRange(data.id, equipBasicData.nSubType, attrID, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus, out min, out max);
                        color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName("G3"));
                        break;
                    case PropType.TYPE_RANDOM:
                        EquipCfgMgr.GetRandomAttrRange(data.id, equipBasicData.nSubType, attrID, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus, out min, out max);
                        color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName("G3"));
                        break;
                    default:
                        break;
                }
                infoList[startIndex].SetName(attrID, attrName, color);
                infoList[startIndex].SetValue(value, min, max);
                infoList[startIndex].Show();
                startIndex++;
            }
        }

        void SetEffectData(ItemData data)
        {
            SetEffectDataCore(data.equipdata.equipBasicData.tempEffectValues, m_tempEffect1,m_tempEffect2,m_tempEffectTrans);
            SetEffectDataCore(data.equipdata.equipBasicData.effectValues, m_Effect1, m_Effect2, m_EffectTrans);
        }
        void SetEffectDataCore(List<EffectValue> values,Text text1,Text text2,Transform trans)
        {
            if (values.Count > 0)
            {
                StringBuilder str = new StringBuilder();
                str.Append(Config.EquipEffectPrototype.Get(values[0].id).effectName);
                str.AppendFormat(" +{0}", values[0].level);
                text1.text = str.ToString();
                trans.gameObject.SetActive(true);
                text1.gameObject.SetActive(true);
                if (values.Count > 1)
                {
                    str.Remove(0, str.Length);
                    str.Append(Config.EquipEffectPrototype.Get(values[1].id).effectName);
                    str.AppendFormat(" +{0}", values[1].level);
                    text2.text = str.ToString();
                    text2.gameObject.SetActive(true);
                }
                else
                {
                    text2.gameObject.SetActive(false);
                }
            }
            else
            {
                trans.gameObject.SetActive(false);
            }
        }
        //检测属性升降
        /// <summary>
        /// 检测属性升降
        /// </summary>
        /// <param name="subType"></param>
        /// <param name="showType"></param>
        void AdjustAttrInfoList(ItemData data, ShowType showType)
        {
            EquipBasicData equipBasicData = data.equipdata.equipBasicData;
            //base 
            for (int i = 0; i < equipBasicData.baseAtts.Count; i++)
            {
                if (m_NewAttrList[i].val > m_OriginAttrList[i].val)
                {
                    m_NewAttrList[i].SetArrowUp();
                }
                else
                {
                    if (m_NewAttrList[i].val < m_OriginAttrList[i].val)
                    {
                        m_NewAttrList[i].SetArrowDown();
                    }
                    else
                    {
                        m_NewAttrList[i].HideArrow();
                    }
                }
            }

            if (showType== ShowType.TYPE_RECAST)
            {
                //custom
                for (int i = equipBasicData.baseAtts.Count; i < (equipBasicData.baseAtts.Count + equipBasicData.customAtts.Count); i++)
                {
                    if (m_NewAttrList[i].val > m_OriginAttrList[i].val)
                    {
                        m_NewAttrList[i].SetArrowUp();
                    }
                    else
                    {
                        if (m_NewAttrList[i].val < m_OriginAttrList[i].val)
                        {
                            m_NewAttrList[i].SetArrowDown();
                        }
                        else
                        {
                            m_NewAttrList[i].HideArrow();
                        }
                    }
                }
                //randomAttrs
                int originIndex = equipBasicData.baseAtts.Count + equipBasicData.customAtts.Count, newIndex = originIndex;

                UnityEngine.Color color = new Color();
                color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName("G3"));
                while (originIndex < m_OriginAttrList.Count && m_OriginAttrList[originIndex].IsActive())
                {
                    for (int i = newIndex; i < m_NewAttrList.Count; i++)
                    {
                        if (m_NewAttrList[i].propertyID == m_OriginAttrList[originIndex].propertyID)
                        {
                            //swap
                            string attrName = m_NewAttrList[originIndex].GetName();
                            double value = m_NewAttrList[originIndex].val;
                            float percentage = m_NewAttrList[originIndex].GetPercentage();
                            int attrID = m_NewAttrList[originIndex].propertyID;
                            
                            m_NewAttrList[originIndex].SetName(m_NewAttrList[i].propertyID, m_NewAttrList[i].GetName(), color);
                            m_NewAttrList[originIndex].SetValue(m_NewAttrList[i].val, m_NewAttrList[i].GetPercentage());
                            m_NewAttrList[i].SetName(attrID, attrName, color);
                            m_NewAttrList[i].SetValue(value, percentage);

                            //compare
                            if (m_NewAttrList[originIndex].val > m_OriginAttrList[originIndex].val)
                            {
                                m_NewAttrList[originIndex].SetArrowUp();
                            }
                            else
                            {
                                if (m_NewAttrList[originIndex].val < m_OriginAttrList[originIndex].val)
                                {
                                    m_NewAttrList[originIndex].SetArrowDown();
                                }
                                else
                                {
                                    m_NewAttrList[originIndex].HideArrow();
                                }
                            }
                        }
                        else
                        {
                            m_NewAttrList[originIndex].HideArrow();
                        }
                    }
                    originIndex++;
                }
            }
        }
        public void SetActive(bool active)
        {
            m_OriginAttrsInfo.gameObject.SetActive(active) ;
            m_NewAttrsInfo.gameObject.SetActive(active);
            m_LeftEmptyInfo.gameObject.SetActive(active);
            m_RightEmptyInfo.gameObject.SetActive(active);
        }

        void SetEffectActive(bool active)
        {
            m_EffectTrans.gameObject.SetActive(active);
            m_tempEffectTrans.gameObject.SetActive(active);
        }

        Vector3 GetFirstInactiveInfoPosInY(List<RecastInfo> infoList)
        {
            Vector3 ret = new Vector3();
            var itr = infoList.GetEnumerator();
            while (itr.MoveNext())
            {
                if (!itr.Current.IsActive())
                {
                    ret.y = itr.Current.GetLocalPosition().y;
                    break;
                }
            }
            return ret;
        }
    }
}
#endif

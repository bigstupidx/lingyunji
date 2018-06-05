#if !USE_HOT
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using xys.UI.State;
using WXB;
using Config;
using NetProto;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
namespace xys.hot.UI
{
    [System.Serializable]
    class UIArrayInfo
    {
        List<EnforceInfo> m_PropertyInfoList = new List<EnforceInfo>();
        List<StateRoot> m_LvIconTransList = new List<StateRoot>();
        [SerializeField]
        Transform m_PropTrans;
        [SerializeField]
        Transform m_LvIconTrans;
        [SerializeField]
        Text m_EquipName;
        [SerializeField]
        Text m_LvNum;
        [SerializeField]
        Text m_MaxLvNum;



        EquipMgr m_EquipMgr;
        Config.EquipPrototype m_EquipCfg;
        public UIArrayInfo()
        {

        }

        public void OnInit()
        {
            for (int i = 0; i < m_PropTrans.childCount; i++)
            {
                Transform Prop = m_PropTrans.GetChild(i);
                m_PropertyInfoList.Add(new EnforceInfo(Prop) );
            }
            for (int i = 0; i < m_LvIconTrans.childCount; i++)
            {
                StateRoot trans = m_LvIconTrans.GetChild(i).GetComponent<StateRoot>();
                m_LvIconTransList.Add(trans);
            }

            
        }

        public  void RefreshPropInfo(ItemData data)
        {
            int subType = data.equipdata.equipBasicData.nSubType;
            m_EquipCfg = Config.EquipPrototype.Get(data.id);
            EquipBasicData equipBasicData = data.equipdata.equipBasicData;
            #region 刷新属性
            var baseAttrs = data.equipdata.equipBasicData.baseAtts;
            var randomAttrs = data.equipdata.equipBasicData.randomAtts;
            var customAttrs = data.equipdata.equipBasicData.customAtts;
            var itrBase = baseAttrs.GetEnumerator();
            var itrRd = randomAttrs.GetEnumerator();
            var itrCus = customAttrs.GetEnumerator();
            //get attrs num
            int totalPropertyNum = baseAttrs.Count + randomAttrs.Count + customAttrs.Count;

            string attrName = "";
            double attrValue = 0;
            double addValue = 0;
            int attrId = 0;
            //if lv is out of range,percentage is 0
            float percentage = 0.0f;
            if (equipBasicData.enforceLv <m_EquipCfg.InforceValue)
            {
                if (equipBasicData.awakenStatus)
                    percentage = EquipCfgMgr.GetEquipEnforceIncPercentage(subType, equipBasicData.enforceLv + 1, equipBasicData.awakenStatus);
                else
                    percentage = EquipCfgMgr.GetEquipEnforceIncPercentage(subType, equipBasicData.awakenEnforceLV + 1, equipBasicData.awakenStatus);
            }
                
            UnityEngine.Color color = new Color();
            //show & hide 
            for (int i = 0; i < m_PropTrans.childCount; i++)
            {
                //show needed infos
                if (i<totalPropertyNum)
                {
	                //get attrs property
	                if (i< baseAttrs.Count)
	                {
	                    if (itrBase.MoveNext())
	                    {
                            attrId = itrBase.Current.Key;
                            attrName =Config.AttributeDefine.Get(itrBase.Current.Key).attrName;
	                        attrValue = itrBase.Current.Value;
                            color = m_PropertyInfoList[i].defaultColor;
                        }
	                }
	                else
	                {
                        if (i < (baseAttrs.Count + customAttrs.Count))
                        {
                            if (itrCus.MoveNext())
                            {
                                attrId = itrCus.Current.Key;
                                attrName = Config.AttributeDefine.Get(itrCus.Current.Key).attrName;
                                attrValue = itrCus.Current.Value;
                                color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName("G3"));
                            }
                        }
                        else
                        {
                            if (itrRd.MoveNext())
                            {
                                attrId = itrRd.Current.Key;
                                attrName = Config.AttributeDefine.Get(itrRd.Current.Key).attrName;
                                attrValue = itrRd.Current.Value;
                                color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName("G3"));
                            }
                        }

	                }
                    //set name
	                m_PropertyInfoList[i].SetName(attrId, attrName, color);
                    //set addVal
                    //if addVal is 0 , addValueText is automatically hided
                    addValue = Math.Ceiling(attrValue * percentage/100);
                    m_PropertyInfoList[i].SetAddValue(addValue);
                    if (addValue>0)
                    {
                        m_PropertyInfoList[i].SetArrowUp();
                    }
                    else
                    {
                        if (addValue==0)
                        {
                            m_PropertyInfoList[i].HideArrow();
                        }
                        else
                        {
                            m_PropertyInfoList[i].SetArrowDown();
                        }
                    }
                    //set val
                    double value = EquipCfgMgr.GetPropertValInTotal(data.id, subType, attrValue, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus);
                    m_PropertyInfoList[i].SetValue(value, color);
	                m_PropertyInfoList[i].ShowLine();
                    m_PropertyInfoList[i].Show();
                } 
                //hide others
                else
                {
                    m_PropertyInfoList[i].Hide();
                }
            }
            #endregion


            #region 刷新等级
            //refresh Lv icon
            int level = equipBasicData.enforceLv;

            for (int i = 0; i < level; i++)
            {
                m_LvIconTransList[i].gameObject.SetActive(true);
                m_LvIconTransList[i].SetCurrentState(1,false);
            }
            for (int i = level; i < m_EquipCfg.InforceValue; i++)
            {
                m_LvIconTransList[i].gameObject.SetActive(true);
                m_LvIconTransList[i].SetCurrentState(0, false);
            }
            for (int i = m_EquipCfg.InforceValue; i < m_LvIconTransList.Count; i++)
            {
                m_LvIconTransList[i].gameObject.SetActive(false);
            }
            #endregion

            //name
            m_EquipName.text = m_EquipCfg.name;
            color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName(Config.QualitySourceConfig.Get(m_EquipCfg.quality).colorname));
            m_EquipName.color = color;
            //level
            m_MaxLvNum.text ="/"+ m_EquipCfg.InforceValue.ToString();
            m_LvNum.text = equipBasicData.enforceLv.ToString();

        }


    }
}
#endif
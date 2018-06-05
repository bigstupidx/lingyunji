#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class RefMaterialItem: MaterialItem
    {
        int refType;
        Color defaultTextColor = new Color();
        public RefMaterialItem(Transform trans):base(trans)
        {
            if (m_Btn != null)
            {
                base.SetOnClickListener(() =>
                {
                    m_EventAgent.FireEvent<int>(EventID.Equip_SelectRefMaterial, refType);
                });
                ButtonPress btnPress = m_Btn.GetComponent<ButtonPress>();
                if (btnPress != null)
                {
                    btnPress.onPress.RemoveAllListeners();
                    btnPress.onPress.AddListener(OnItemPress);
                }
                   
            }
            defaultTextColor = m_Count.color;
        }

        public void SetData(int refType)
        {
            this.refType = refType;

            var propertyCfg = Config.RefinePropertyTable.Get(refType);
            if (propertyCfg == null)
            {
                XYJLogger.LogError("Invalid refType:"+refType);
                return;
            }
            itemId = propertyCfg.materialCostId;
            consumeNum = propertyCfg.materialCostCount;

            var itemCfg = Config.Item.Get(itemId);
            Helper.SetSprite(m_IconSp, itemCfg.icon);
            Helper.SetSprite(m_QualitySp, Config.QualitySourceConfig.Get(itemCfg.quality).icon);
            m_IconSp.gameObject.SetActive(true);
            m_QualitySp.gameObject.SetActive(true);

            ResetNums();

        }

        public new void SetText(string str)
        {
            m_Count.gameObject.SetActive(true);
            m_Count.text = str;
        }
        //public void SetOnClickListener(UnityAction action)
        //{
        //    m_Btn.onClick.RemoveAllListeners();
        //    m_Btn.onClick.AddListener(action);
        //}

        public new void ResetNums()
        {
            var itemCfg = Config.Item.Get(itemId);
            if (itemCfg!=null)
            {
                totalNum = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(itemCfg.id);
                m_Count.text = totalNum.ToString();
                if (totalNum == 0)
                {
                    Color color = new Color();
                    color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName("R1"));
                    m_Count.color = color;
                }
                else
                    m_Count.color = defaultTextColor;
            }
        }

        void OnItemPress()
        {
            if (Config.Item.Get(itemId) != null)
                UICommon.ShowItemTips(itemId);
        }
    }
}

#endif

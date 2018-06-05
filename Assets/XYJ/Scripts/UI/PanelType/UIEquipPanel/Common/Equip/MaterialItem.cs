#if !USE_HOT
using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WXB;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class MaterialItem
    {
        protected Image m_IconSp;
        protected Text m_Count;
        protected Image m_QualitySp;
        protected Button m_Btn;
        protected bool m_IsUsingDefaultCallBack;

        protected const string DefaultMaterialIcon = "";
        protected const string DefaultMaterialQuality = "ui_Common_Quality_White";
        public int itemId
        {
            get;
            protected set;
        }
        protected Transform m_Trans;

        protected hot.Event.HotObjectEventAgent m_EventAgent;
        public int totalNum
        {
            get;
            protected set;
        }
        public int consumeNum
        {
            get;
            protected set;
        }
        public MaterialItem(Transform trans)
        {
            m_Trans = trans;
            m_IconSp = trans.Find("Icon").GetComponent<Image>();
            m_Count = trans.Find("Text").GetComponent<Text>();
            m_QualitySp = trans.Find("Quality").GetComponent<Image>();
            m_Btn = trans.GetComponent<Button>();

            m_EventAgent = new hot.Event.HotObjectEventAgent(xys.App.my.localPlayer.eventSet);
            if (null != m_Btn )
                m_Btn.onClick.AddListener(OnItemClick);
            m_IsUsingDefaultCallBack = true;
        }
        ~MaterialItem()
        {
            if (m_EventAgent != null)
            {
                m_EventAgent.Release();
                m_EventAgent = null;
            }
        }

        public void OnHide()
        {

        }

        public void SetData(int itemId, int cost, bool defultCallBackActive)
        {
            this.itemId = itemId;
            consumeNum = cost;
            string iconName = "";
            ItemQuality quality;
            if (Config.Item.GetAll().Keys.Contains(itemId))
            {
                var itemCfg = Config.Item.Get(itemId);
                iconName = itemCfg.icon;
                quality = itemCfg.quality;
            }
            else
            {
                if (Config.EquipPrototype.GetAll().Keys.Contains(itemId))
                {
                    var equipCfg = Config.EquipPrototype.Get(itemId);
                    iconName = equipCfg.icon;
                    quality = equipCfg.quality;
                }
                else
                    return;
            }
            Helper.SetSprite(m_IconSp, iconName);
            Helper.SetSprite(m_QualitySp, Config.QualitySourceConfig.Get(quality).icon);
            m_IconSp.gameObject.SetActive(true);
            m_QualitySp.gameObject.SetActive(true);

            SetText(consumeNum);
            if (!defultCallBackActive && m_IsUsingDefaultCallBack)
                RemoveAllListener();
        }
        public void SetText(int cost)
        {
            consumeNum = cost;
            m_Count.gameObject.SetActive(true);
            ResetNums();
        }
        public void SetTextActive(bool active)
        {
            m_Count.gameObject.SetActive(active);
        }
        public void SetIconActive(bool active)
        {
            m_IconSp.gameObject.SetActive(active);
            m_QualitySp.gameObject.SetActive(active);
        }
        public void SetOnClickListener(UnityAction action)
        {
            if (m_Btn!=null)
            {
                m_IsUsingDefaultCallBack = false;
                m_Btn.onClick.RemoveAllListeners();
                m_Btn.onClick.AddListener(action);
            }
        }
        public void RemoveAllListener()
        {
            if (m_Btn != null)
                m_Btn.onClick.RemoveAllListeners();
        }
        public void SetTotalNum(int totalNum)
        {
            this.totalNum = totalNum;
        }
        public void ResetNums()
        {
            if (consumeNum>0)
            {
                totalNum = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(this.itemId);
                Color color = new Color();
                if (totalNum < consumeNum)
                    color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName("R1"));
                else
                    color = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName("G2"));
                m_Count.color = color;
                m_Count.text = string.Format("{0}/{1}", totalNum, consumeNum);
            }
        }
        public bool IsMatSufficient()
        {
            if (m_Trans.gameObject.activeSelf)
                return (totalNum!=0)&&(totalNum < consumeNum ? false : true);
            else
                return true;
        }
        void OnItemClick()
        {
            if(Config.Item.Get(itemId)!=null)
                UICommon.ShowItemTips(itemId);
        }
        public void Reset()
        {
            m_Count.gameObject.SetActive(false);
            m_IconSp.gameObject.SetActive(false);
            Helper.SetSprite(m_QualitySp, DefaultMaterialQuality);
        }
    }
}

#endif

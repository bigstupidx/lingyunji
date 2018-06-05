#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys;
using UnityEngine.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    class UITrainScrollItem
    {
        System.Action<UITrainScrollItem> m_Action;
        public Image m_QualityIcon;
        public Image m_Icon;
        public Text m_Count;
        public Button m_Btn;
        public Transform root;

        int m_ItemID;
        void OnEnable()
        {
            m_Btn.onClick.AddListener(this.ClickEvent);
        }

        void OnDisable()
        {
            m_Btn.onClick.RemoveListener(this.ClickEvent);
        }

        public void Set(int itemID, System.Action<UITrainScrollItem> action = null)
        {
            //m_Btn.onClick.RemoveAllListeners();

            Config.Item itemData = Config.Item.Get(itemID);
            if (itemData == null)
                return;
            this.Set(itemData, action);
        }

        public void Set(Config.Item itemData, System.Action<UITrainScrollItem> action = null)
        {
            if (itemData == null)
                return;

            m_ItemID = itemData.id;

            Helper.SetSprite(m_Icon, itemData.icon);
            Helper.SetSprite(m_QualityIcon, Config.QualitySourceConfig.Get(itemData.quality).icon);

            PackageMgr pm = hotApp.my.GetModule<HotPackageModule>().packageMgr;

            // int bindIDCount = itemData.bindId != 0 ? pm.GetItemCount(itemData.bindId) : 0;
            int itemCount = pm.GetItemCount(itemData.id);// + bindIDCount;
            if (itemCount == 0)
                m_Count.text = string.Format("<color=#{0}>", "ef3c49") + itemCount + "</color>";
            else
                m_Count.text = itemCount.ToString();

            m_Action = action;
        }
        public void RefleshData()
        {
            this.Set(m_ItemID, m_Action);
        }
        void ClickEvent()
        {
            if (m_Action != null)
                m_Action(this);
        }

        public int itemID { get { return m_ItemID; } }
    }
}
#endif
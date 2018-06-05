#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using xys.UI.State;
using xys.UI;
using xys.UI.Dialog;

namespace xys.hot.UI
{
    [System.Serializable]
    class UIPetsSkillPanelItem
    {
        System.Action<UIPetsSkillPanelItem> m_Action = null;
        bool m_Active;
        int m_ItemID;
       // [SerializeField]
        Transform m_Transform;
        public Transform transform { get { return m_Transform; } }

        //[SerializeField]
        Image m_Icon;
        //[SerializeField]
        Text m_SkillName;
        //[SerializeField]
        Text m_ItemCount;
        //[SerializeField]


        //         void OnEnable()
        //         {
        //             this.m_Transform.GetComponent<Button>().onClick.AddListener(this.OnClickEvent);
        //         }
        //         void OnDisable()
        //         {
        //             this.m_Transform.GetComponent<Button>().onClick.RemoveListener(this.OnClickEvent);
        //         }

        public void OnInit(GameObject obj)
        {
            this.m_Transform = obj.transform;
            this.m_Icon = obj.transform.Find("Icon").GetComponent<Image>();
            this.m_SkillName = obj.transform.Find("Name").GetComponent<Text>();
            this.m_ItemCount = obj.transform.Find("ShuLiangText").GetComponent<Text>();
            this.m_Transform.GetComponent<Button>().onClick.AddListener(this.OnClickEvent);
        }

        public void Set(int itemID,bool isLearn = false, System.Action<UIPetsSkillPanelItem> action = null)
        {
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;

            m_Active = true;
            m_ItemID = itemID;// data.GetInt(ItemTable.ITEMID);
            this.m_Transform.GetComponent<StateRoot>().CurrentState = isLearn ? 1 : 0;

            Helper.SetSprite(m_Icon, Config.Item.Get(m_ItemID).icon);
            m_ItemCount.text = "" + packageMgr.GetItemCount(m_ItemID);
            m_SkillName.text = Config.Item.Get(m_ItemID).name;

            this.m_Transform.Find("line").gameObject.SetActive(this.m_Transform.GetSiblingIndex() % 2 == 0);

            m_Action = action;
        }

        public void Set(Config.Item data, System.Action<UIPetsSkillPanelItem> action = null)
        {
            m_Active = false;
            m_ItemID = data.id;
            this.m_Transform.GetComponent<StateRoot>().CurrentState = 2;

            Helper.SetSprite(m_Icon, Config.Item.Get(m_ItemID).icon);
            m_ItemCount.text = string.Empty;
            m_SkillName.text = Config.Item.Get(m_ItemID).name;

            this.m_Transform.Find("line").gameObject.SetActive(this.m_Transform.GetSiblingIndex() % 2 == 0);

            m_Action = action;
        }

        void OnClickEvent()
        {
            if (m_Action != null)
                m_Action(this);
        }

        public int itemID { get { return m_ItemID; } }
        public bool active { get { return m_Active; } }
    }
}


#endif
#if !USE_HOT
using NetProto.Hot;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [System.Serializable]
    class UIPetsSkillScorllView
    {
        enum SkillQuality
        {
            Lower = 2,
            High = 3,
            Stunt = 4,
        }
        [SerializeField]
        public GameObject m_ItemPrefab;

        [SerializeField]
        protected Transform m_ItemGrid;
        [SerializeField]
        protected Transform m_DropItems;

        [SerializeField]
        StateToggle m_SkillToggle;

        [SerializeField]
        StateRoot m_IsShowSR;
        [SerializeField]
        UnityEngine.UI.ScrollRect m_ScrollRect;
        [SerializeField]
        Vector3 m_TipsPos = new Vector3(288, 62, 0);
        [SerializeField]
        GameObject m_Logo;

        PetsPanel m_Panel;
        public PetsPanel panel { set { m_Panel = value; } }

        UIPetsSkillPanelItem m_SelectedItem = null;
        Dictionary<int, UIPetsSkillPanelItem> m_ItemDic = new Dictionary<int, UIPetsSkillPanelItem>();
        List<UIPetsSkillPanelItem> m_DeadItemList = new List<UIPetsSkillPanelItem>();
        bool m_IsShowAll;
        int m_CurrentIndex;

        public void OnInit()
        {
            m_SkillToggle.OnSelectChange = this.SelecetdChangeEvent;
        }

        public void OnShow()
        {
            m_CurrentIndex = 2;
            this.Refresh();
            m_IsShowAll = true;
            m_SkillToggle.Select = 0;
        }

        public void OnHide()
        {
            //m_SkillToggle.OnSelectChange = null;
            this.HideTips();
        }

        void SelecetdChangeEvent(StateRoot sr, int index)
        {
            m_CurrentIndex = index + 2;
            this.Set((Config.ItemQuality)m_CurrentIndex, m_IsShowAll);
        }

        public void Refresh()
        {
            this.Set((Config.ItemQuality)m_CurrentIndex, m_IsShowAll);
        }

        void Set(Config.ItemQuality index, bool showAll)
        {
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return;
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;
            PetsAttribute attribute = petsMgr.m_PetsTable.attribute[m_Panel.selected];
            if (attribute == null)
                return;

            this.Clear();
            m_IsShowSR.CurrentState = showAll ? 0 : 1;
            //
            List<int> nSkillIDTempList =new List<int>();
            if(attribute.trick_skills != null)
                nSkillIDTempList.Add(attribute.trick_skills.id);
            if (attribute.talent_skills != null)
                nSkillIDTempList.Add(attribute.talent_skills.id);
            for (int i = 0; i < attribute.passive_skills.Count; i++)
                if (attribute.passive_skills[i] != null)
                    nSkillIDTempList.Add(attribute.passive_skills[i].id);
            //检查道具表所有宠物技能道具
            Dictionary<int, Config.Item> list = Config.Item.GetAll();
            foreach (Config.Item item in list.Values)
            {
                if (item.type != Config.ItemType.consumables)
                    continue;

                if (item.sonType != (int)Config.ItemChildType.petSkillBook
                    && item.sonType != (int)Config.ItemChildType.petSuperSkill)
                    continue;

                if (item.quality != index)
                    continue;

                if (packageMgr.GetItemCount(item.id) > 0)
                    this.NewItem(item.id, nSkillIDTempList.Contains(item.petSkill));
                else
                    this.NewItem(item);
            }

            m_ScrollRect.content.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            m_Logo.SetActive(!m_IsShowAll);
        }

        void NewItem(int itemId, bool learn)
        {
            GameObject obj = null;
            UIPetsSkillPanelItem item = null;
            if (m_DeadItemList.Count == 0)
            {
                obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null)
                    return;
                item = new UIPetsSkillPanelItem();
                item.OnInit(obj);
            }
            else
            {
                item = m_DeadItemList[0];
                obj = item.transform.gameObject;
            }
            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localScale = Vector3.one;
            
            item.Set(itemId, learn, this.OnSelectItem);
            m_ItemDic.Add(itemId, item);
            m_DeadItemList.Remove(item);
        }

        void NewItem(Config.Item data)
        {
            GameObject obj = null;
            UIPetsSkillPanelItem item = null;
            if (m_DeadItemList.Count == 0)
            {
                obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null)
                    return;
                item = new UIPetsSkillPanelItem();
                item.OnInit(obj);
            }
            else
            {
                item = m_DeadItemList[0];
                obj = item.transform.gameObject;
            }
            obj.SetActive(m_IsShowAll);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localScale = Vector3.one;
            
            item.Set(data, this.OnSelectItem);
            m_ItemDic.Add(data.id, item);
            m_DeadItemList.Remove(item);
        }

        void OnSelectItem(UIPetsSkillPanelItem item)
        {
            if (item == m_SelectedItem)
                return;

            if (m_SelectedItem != null)
                m_SelectedItem.transform.Find("SelectIcon").gameObject.SetActive(false);

            m_SelectedItem = item;

            if (m_SelectedItem != null)
            {
                this.ShowUnactiveSkillTips(m_SelectedItem.itemID);
                m_SelectedItem.transform.Find("SelectIcon").gameObject.SetActive(true);
            }
        }

        void ShowUnactiveSkillTips(int itemId)
        {
//             AnyArgs args = new AnyArgs();
//             args.Add("id", itemId);
//             args.Add("isClickThrough", true);
//             args.Add("isShowObtain", true);
//             args.Add("centerOffset", new Vector2(304.0f, 67.0f));
//             UISystem.Instance.ShowPanel("UIItemTipsPanel", args, true);

        }
        public void ShowAll()
        {
            m_IsShowAll = !m_IsShowAll;
            m_IsShowSR.CurrentState = m_IsShowAll ? 0 : 1;
            foreach (UIPetsSkillPanelItem item in m_ItemDic.Values)
                if (!item.active)
                    item.transform.gameObject.SetActive(m_IsShowAll);

            m_Logo.SetActive(!m_IsShowAll);

            this.HideTips();
        }

        public void ShowPage(int index)
        {
            m_SkillToggle.Select = index;
        }
        void Clear()
        {
            if (m_SelectedItem != null)
                m_SelectedItem.transform.Find("SelectIcon").gameObject.SetActive(false);
            m_SelectedItem = null;

            foreach(UIPetsSkillPanelItem item in m_ItemDic.Values)
                m_DeadItemList.Add(item);

            while (m_ItemGrid.transform.childCount != 0)
            {
                Transform item = m_ItemGrid.transform.GetChild(0);
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }
            m_ItemDic.Clear();
        }
        void HideTips()
        {
            App.my.uiSystem.HidePanel("UIItemTipsPanel");
        }

        //public bool showAll { set { this.ShowAll(value); } }
        public bool selectedActive { get { return m_SelectedItem == null ? false : m_SelectedItem.active; } }
        public int selectedItemID { get { return m_SelectedItem == null ? 0 : m_SelectedItem.itemID; } }
    }
}
#endif
#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using NetProto;
    using Config;
    using xys.UI;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;

    partial class UIDPSkillTips
    {
        public class ItemCompera : IComparer<Item>
        {
            public int Compare(Item x, Item y)
            {
                return x.id < y.id ? 1 : -1;
            }
        }
        //
        [SerializeField]
        Button m_SelecteItemBtn;
        [SerializeField]
        Transform m_ItemGrids;
        [SerializeField]
        Transform m_DropItems;
        [SerializeField]
        GameObject m_ItemObj;

        int m_SelectedItemId = 0;
        int m_EventHandleId = -1;
        void InitExpItem()
        {
            this.ClearItems();

            List<Item> list = new List<Item>();
            foreach (Item data in Item.GetAll().Values)
            {
                if (data.liveskillexp != 0 && data.bindId != 0)
                    list.Add(data);
            }
            list.Sort(new ItemCompera());
            for (int i = 0; i < list.Count; i++)
            {
                this.NewItems(list[i]);
            }
            if (list.Count > 0)
            {
                int index = 0;
                int lv = 1;
                if (m_Mgr.m_Tables.skills.ContainsKey((int)m_SkillType))
                    lv = m_Mgr.m_Tables.skills[(int)m_SkillType].lv;
                //根据等级和拥有显示
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if(list[i].liveskilllimit <= lv && hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(list[i].id) > 0)
                    {
                        index = i;
                        break;
                    }
                }
                this.SelectedEvent(list[index].id, false);
            }
        }

        void NewItems(Item data)
        {
            GameObject obj = null;
            if (m_DropItems.childCount == 0)
            {
                obj = GameObject.Instantiate(m_ItemObj);
                if (obj == null) return;
            }
            else
                obj = m_DropItems.GetChild(0).gameObject;

            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrids, false);
            obj.transform.localPosition = Vector3.zero;

            #region UI
            Transform root = obj.transform;
            Helper.SetSprite(root.Find("TipsBtn/Icon").GetComponent<Image>(), data.icon);
            Helper.SetSprite(root.Find("TipsBtn/Quality").GetComponent<Image>(), QualitySourceConfig.Get(data.quality).icon);
            root.Find("Name").GetComponent<Text>().text = string.Format("<color=#{0}>{1}</color>", QualitySourceConfig.Get(data.quality).color, data.name);

            int count = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(data.id);
            root.Find("Count").GetComponent<Text>().text = count == 0 ?
                 GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", "R1", count.ToString())) : count.ToString();

            root.Find("TipsBtn/Mark").gameObject.SetActive(hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(data.id) == 0);
            root.Find("Des").GetComponent<Text>().text = "熟练度+" + data.liveskillexp;

            root.Find("Level").GetComponent<Text>().text = data.liveskilllimit == 0 ? string.Empty : string.Format("{0}级以上可用", data.liveskilllimit);
            //事件
            root.Find("TipsBtn").GetComponent<Button>().onClick.RemoveAllListeners();
            root.Find("TipsBtn").GetComponent<Button>().onClick.AddListener(()=> { this.ShowItemTipsEvent(data.id); });
            root.Find("EmptyBtn").GetComponent<Button>().onClick.RemoveAllListeners();
           root.Find("EmptyBtn").GetComponent<Button>().onClick.AddListener(() => { this.SelectedEvent(data.id); });
            #endregion
        }

        void ClearItems()
        {
            m_SelectedItemId = 0;
            while(m_ItemGrids.childCount > 0)
            {
                Transform item = m_ItemGrids.GetChild(0);
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }
        }

        void SelectedEvent(int itemId,bool showTips = true)
        {
            if (showTips)
                this.HideSelecteTips();
            if (m_SelectedItemId == itemId)
                return;
            Item itemData = Item.Get(itemId);
            int lv = 1;
            if (m_Mgr.m_Tables.skills.ContainsKey((int)m_SkillType))
                lv = m_Mgr.m_Tables.skills[(int)m_SkillType].lv;
            if(lv < itemData.liveskilllimit)
            {
                SystemHintMgr.ShowHint(string.Format(TipsContent.GetByName("demonplot_skill_limit").des, itemData.liveskilllimit));
                return;
            }
            m_MarName.text = string.Format("<color=#{0}>{1}</color>", QualitySourceConfig.Get(itemData.quality).color, itemData.name);
            Helper.SetSprite(m_MarIcon, itemData.icon);
            Helper.SetSprite(m_MarQuality, QualitySourceConfig.Get(itemData.quality).icon);

            int count = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(itemId);
            if (count == 1)
                m_MarCount.text = string.Empty;
            else if (count == 0)
                m_MarCount.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", "R1", count.ToString()));
            else
                m_MarCount.text = count.ToString();

            m_MarMark.SetActive(count == 0);
            if (count == 1 && showTips == true)
                this.ShowItemTipsEvent(itemId);

            m_SelectedItemId = itemId;
        }

        void ShowItemTipsEvent(int itemId)
        {
            UICommon.ShowItemTips(itemId);
        }

        void ShowSelectTips()
        {
            m_ItemGrids.gameObject.SetActive(true);
            m_TipsClostBtn.gameObject.SetActive(true);
        }
        void HideSelecteTips()
        {
            m_ItemGrids.gameObject.SetActive(false);
            m_TipsClostBtn.gameObject.SetActive(false);
        }
        bool OnPress(GameObject obj,BaseEventData data)
        {
            if (obj == null || !obj.transform.IsChildOf(this.m_ItemGrids))
            {
                this.HideSelecteTips();
                return false;
            }
            return true;
        }
    }
}
#endif
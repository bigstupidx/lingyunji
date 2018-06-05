#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys;

namespace xys.hot.UI
{
    [System.Serializable]
    class UITrainScrollView
    {
        [SerializeField]
        public GameObject m_ItemPrefab;
        [SerializeField]
        public Transform m_ItemGrid;
        [SerializeField]
        public Transform m_DropItems;
        [SerializeField]
        public DPOnCenterChild m_CenterChild;

        UITrainScrollItem m_SelectedItem;
        Dictionary<int, UITrainScrollItem> m_ItemDic = new Dictionary<int, UITrainScrollItem>();
        System.Action<int> m_Acion = null;

        public void OnInit()
        {
        }

        public void OnDisable()
        {
            m_ItemGrid.parent.GetComponent<UnityEngine.UI.ScrollRect>().content.anchoredPosition = Vector2.zero;
        }

        public void Show(int itemId, System.Action<int> action = null)
        {
            this.Clear();
            m_Acion = action;
            Dictionary<int, Config.Item> list = Config.Item.GetAll();
            List<int> passItemList = new List<int>();
            List<Config.Item> petsItemList = new List<Config.Item>();
            foreach (Config.Item item in list.Values)//记录绑定ID
            {
                if (item.type != Config.ItemType.consumables)
                    continue;

                if (item.sonType != (int)Config.ItemChildType.petExpDrug
                    && item.sonType != (int)Config.ItemChildType.petTrainItem
                    && item.sonType != (int)Config.ItemChildType.petPersonalityResetItem)
                    continue;

                if (item.bindId != 0)
                    passItemList.Add(item.bindId);
                petsItemList.Add(item);
            }
            foreach (Config.Item item in petsItemList)
            {
                if (item.type != Config.ItemType.consumables)
                    continue;
                
                if (item.sonType != (int)Config.ItemChildType.petExpDrug
                    && item.sonType != (int)Config.ItemChildType.petTrainItem
                    && item.sonType != (int)Config.ItemChildType.petPersonalityResetItem)
                    continue;
                //过滤绑定ID
                if (passItemList.Contains(item.id))
                    continue;
                this.NewItem(item);
            }

            if (m_ItemDic.Count > 0)
            {
                List<int> keys = new List<int>(m_ItemDic.Keys);
                if (itemId == 0)
                {
                    this.OnSelectItem(m_ItemDic[keys[0]]);
                }
                else
                {
                    if (m_ItemDic.ContainsKey(itemId))//非绑定
                    {
                        this.OnSelectItem(m_ItemDic[itemId]);
                    }
                    else//绑定
                    {
                        foreach (int key in m_ItemDic.Keys)
                            if (Config.Item.Get(key).bindId == itemId)
                                this.OnSelectItem(m_ItemDic[key]);
                    }
                    m_CenterChild.SetIndex(m_SelectedItem.root.transform.GetSiblingIndex() + 1);
                }
            }
        }

        void NewItem(Config.Item data)
        {
            GameObject obj = null;
            if (m_DropItems.childCount == 0)
            {
                obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null)
                    return;
            }
            else
            {
                obj = m_DropItems.GetChild(0).gameObject;
            }
            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localScale = Vector3.one;

            ILMonoBehaviour il = obj.GetComponent<ILMonoBehaviour>();
            if (il == null)
                return;
            UITrainScrollItem item = (UITrainScrollItem)il.GetObject() ;// obj.GetComponent<UITrainScrollItem>();
            item.Set(data, this.OnSelectItem);
            m_ItemDic.Add(data.id, item);
        }

        void OnSelectItem(UITrainScrollItem item)
        {
            if (item == m_SelectedItem)
                return;

            if (m_SelectedItem != null)
                m_SelectedItem.root.transform.Find("panel/SelectIcon").gameObject.SetActive(false);

            m_SelectedItem = item;

            if (m_SelectedItem != null)
            {
                m_SelectedItem.root.transform.Find("panel/SelectIcon").gameObject.SetActive(true);
            }

            if (m_Acion != null)
                m_Acion(m_SelectedItem.itemID);
        }

        void Clear()
        {
            if (m_SelectedItem != null)
                m_SelectedItem.root.transform.Find("panel/SelectIcon").gameObject.SetActive(false);
            m_SelectedItem = null;

            while (m_ItemGrid.transform.childCount != 0)
            {
                Transform item = m_ItemGrid.transform.GetChild(0);
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }
            m_ItemDic.Clear();

            m_Acion = null;
        }

        public void RefleshData()
        {
            foreach (UITrainScrollItem item in m_ItemDic.Values)
                item.RefleshData();
        }

        public int SelectedItem { get { return m_SelectedItem.itemID; } }
    }
}

#endif
#if !USE_HOT
using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;
using xys.UI;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{
    [AutoILMono]
    class UIRoleAttriScrollView
    {
        [SerializeField]
        protected Transform m_ItemGrid;
        [SerializeField]
        protected Transform m_DropItem;
        [SerializeField]
        [PackTool.Pack]
        protected GameObject m_ItemPrefab;

        [SerializeField]
        protected HotTablePage m_Page;

        EventAgent m_EventAgent;

        protected Dictionary<int, List<AttributeDefine>> attriColumDic = new Dictionary<int, List<AttributeDefine>>();
        protected Dictionary<int, Text> attriTextDic = new Dictionary<int, Text>();
        BattleAttri battleAttri = new BattleAttri();

        public void SetEvent(EventAgent eventAgent = null)
        {
            m_EventAgent = eventAgent;
        }

        public IEnumerator Show(System.Action callback = null)
        {
            Hide();
            GetColumData();
            ShowColumData();

            yield return 0;

            if (callback != null)
            {
                callback();
            }
        }

        public void Hide()
        {
            HideColumGo();

            attriColumDic.Clear();
            attriTextDic.Clear();
        }

        void GetColumData()
        {
            List<AttributeDefine> data = new List<AttributeDefine>(AttributeDefine.GetAll().Values);
            foreach (AttributeDefine item in data)
            {
                if (item.attrColumn == 0 || item.attrOrder == 0)
                    continue;

                if (!attriColumDic.ContainsKey(item.attrColumn))
                {
                    attriColumDic.Add(item.attrColumn, new List<AttributeDefine>());
                }
                attriColumDic[item.attrColumn].Add(item);
            }
            SortAttri();
        }

        void SortAttri()
        {
            List<int> keys = new List<int>(attriColumDic.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                attriColumDic[keys[i]].Sort((x, y) => x.attrOrder.CompareTo(y.attrOrder));
            }
        }

        void ShowColumData()
        {
            if (App.my != null && App.my.localPlayer != null)
                battleAttri = App.my.localPlayer.uiShowBattleAttri;

            List<int> keys = new List<int>(attriColumDic.Keys);
            keys.Sort((x, y) => x.CompareTo(y));

            float titleHeight = m_ItemPrefab.transform.Find("title").GetComponent<RectTransform>().sizeDelta.y;
            float itemHeight = m_ItemPrefab.transform.Find("Grid").GetComponent<GridLayoutGroup>().cellSize.y;

            RoleAttri.RoleAttributePage page = null;
            if (m_Page != null) page = (RoleAttri.RoleAttributePage)m_Page.GetObject();

            for (int i = 0; i < keys.Count; i++)
            {
                GameObject go = NewNullItem(m_ItemGrid, m_ItemPrefab, m_DropItem);
                go.transform.Find("title/Name").GetComponent<Text>().text = SetColumTitle(keys[i]);

                for (int j = 0; j < attriColumDic[keys[i]].Count; j++)
                {
                    AttributeDefine data = attriColumDic[keys[i]][j];
                    GameObject goItem = NewNullItem(go.transform.Find("Grid"), go.transform.Find("Item").gameObject, go.transform.Find("DropItem"));
                    goItem.transform.Find("Name").GetComponent<Text>().text = data.attrName;
                    goItem.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (page != null)
                            page.OnAttriItemClick(data.id, goItem.transform);
                    });
                    attriTextDic.Add(data.id, goItem.transform.Find("value").GetComponent<Text>());
                    RefreshItemValue(data.id, battleAttri.Get(data.id));
                }

                float height = titleHeight + itemHeight * ((attriColumDic[keys[i]].Count + 1) / 2) + 4;
                go.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(go.transform.GetComponent<RectTransform>().sizeDelta.x, height);
            }
        }

        string SetColumTitle(int index)
        {
            string title = string.Empty;
            switch (index)
            {
                case 1:
                    title = "攻击属性";
                    break;
                case 2:
                    title = "防御属性";
                    break;
                case 3:
                    title = "五行属性";
                    break;
                case 4:
                    title = "高级属性";
                    break;
            }
            return title;
        }

        protected GameObject NewNullItem(Transform grid, GameObject item, Transform dropItem)
        {
            GameObject obj = null;
            if (dropItem.childCount == 0)
            {
                obj = GameObject.Instantiate(item);
            }
            else
            {
                obj = dropItem.GetChild(0).gameObject;
            }

            obj.SetActive(true);
            obj.transform.SetParent(grid, false);
            obj.transform.localScale = Vector3.one;
            return obj;
        }

        void HideColumGo()
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in m_ItemGrid)
            {
                children.Add(child);
            }

            for (int i = 0; i < children.Count; i++)
            {
                Transform item = children[i];
                HideDropItem(item.Find("Grid"), item.Find("DropItem"));
            }

            HideDropItem(m_ItemGrid, m_DropItem);
        }

        protected void HideDropItem(Transform grid, Transform dropItem)
        {
            while (grid.childCount != 0)
            {
                Transform item = grid.GetChild(0);
                item.SetParent(dropItem, false);
                item.gameObject.SetActive(false);
            }
        }

        public void RefreshColumData()
        {
            List<int> keys = new List<int>(attriColumDic.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                for (int j = 0; j < attriColumDic[keys[i]].Count; j++)
                {
                    AttributeDefine data = attriColumDic[keys[i]][j];
                    RefreshItemValue(data.id, battleAttri.Get(data.id));
                }
            }
        }

        void RefreshItemValue(int id, double value)
        {
            if (attriTextDic.ContainsKey(id))
                attriTextDic[id].text = AttributeDefine.GetValueStr(id, value);
        }
    }
}
#endif
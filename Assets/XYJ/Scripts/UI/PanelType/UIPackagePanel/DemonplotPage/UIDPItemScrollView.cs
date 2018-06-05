#if !USE_HOT
using System.Collections;
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using UnityEngine.UI;
    using NetProto;
    using NetProto.Hot;

    [System.Serializable]
    public class UIDPItemScrollView
    {
        public class ItemParam
        {
            #region 道具子对象
            public Transform transform;

            StateRoot m_StateRoot;

            Image m_Icon;
            Image m_Quality;
            Text m_Level;
            GameObject m_SelectedIcon;

            int m_MatchinID = 0;
            public int matchinID { get { return m_MatchinID; } }

            Action<ItemParam> m_SelectedCallback = null;
            public Action<ItemParam> selectedCallback { set { m_SelectedCallback = value; } }
            public void OnInit()
            {
                transform.GetComponent<Button>().onClick.AddListener(this.OnClick);

                m_StateRoot = transform.GetComponent<StateRoot>();
                m_Icon = transform.Find("Icon").GetComponent<Image>();
                m_Quality = transform.Find("Quality").GetComponent<Image>();
                m_SelectedIcon = transform.Find("HighLightItem").gameObject;
                m_Level = transform.Find("Level").GetComponent<Text>();
            }
            public void Set(DemonplotProperty property,Action<ItemParam> callback)
            {
                //根据配置设置UI
                Item itemData = Item.Get(property.produce.id);
                Helper.SetSprite(m_Icon, itemData.icon);
                Helper.SetSprite(m_Quality, QualitySourceConfig.Get(itemData.quality).icon);

                //根据技能设置UI
                DemonplotSkillType skillType = property.skilltype;
                DemonplotsTable skillTables = ((DemonplotsMgr)App.my.localPlayer.GetModule<DemonplotsModule>().demonplotMgr).m_Tables;
                if (skillTables == null)
                    return;
                DemonplotSkillData data = null;
                if (!skillTables.skills.ContainsKey((int)skillType))
                {
                    data = new DemonplotSkillData();
                    data.lv = 1;
                    data.exp = 0;
                }
                else
                {
                    data = skillTables.skills[(int)skillType];
                }
                int skillLv = data.lv;
                if (skillLv < property.needlv)
                {
                    m_Level.text = property.needlv + "级";
                    m_StateRoot.CurrentState = 2;
                }
                else
                    m_StateRoot.CurrentState = 0;

                this.m_SelectedCallback = callback;

                this.m_MatchinID = property.id;
            }

            void OnClick()
            {
                if (this.m_SelectedCallback != null)
                {
                    this.m_SelectedCallback(this);
                }
            }

            public void OnEnable(bool enable)
            {
                m_SelectedIcon.SetActive(enable);
            }
            #endregion
        }

        [SerializeField]
        GameObject m_ItemPrefab;
        [SerializeField]
        Transform m_DropItems;
        [SerializeField]
        Transform m_ItemGrid;

        ItemParam m_SelectedItem = null;
        public int selectedID { get { return m_SelectedItem == null ? -1 : m_SelectedItem.matchinID; } }


        Action m_SelectedCallback = null;
        public Action selectedCallback { set { m_SelectedCallback = value; } }

        Dictionary<int, ItemParam> m_Items = new Dictionary<int, ItemParam>();
        List<ItemParam> m_DeadItems = new List<ItemParam>();

        public void OnInit()
        {
            m_Items.Clear();
            m_DeadItems.Clear();
        }

        public void Create(List<DemonplotProperty> list)
        {
            this.Clear();
            foreach(DemonplotProperty item in list)
                this.CreateItem(item);

            if(m_Items.Count > 0)
            {
                List<int> keys = new List<int>(m_Items.Keys);
                this.OnSelectedItem(m_Items[keys[0]]);
            }
        }

        bool CreateItem(DemonplotProperty property)
        {
            GameObject obj = null;
            if (m_DropItems.childCount == 0)
            {
                obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null)
                    return false;
            }
            else
            {
                obj = m_DropItems.GetChild(0).gameObject;
            }
            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localPosition = Vector3.zero;

            ItemParam item;
            if (m_DeadItems.Count > 0)
                item = m_DeadItems[0];
            else
                item = new ItemParam();
            item.transform = obj.transform;
            item.OnInit();
            item.Set(property,this.OnSelectedItem);

            m_Items.Add(property.id, item);
            m_DeadItems.Remove(item);
            return true;
        }

        void Clear()
        {
            if (m_SelectedItem != null)
                m_SelectedItem.OnEnable(false);
            m_SelectedItem = null;

            while(m_ItemGrid.childCount > 0)
            {
                Transform item = m_ItemGrid.GetChild(0);
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }

            foreach(ItemParam item in m_Items.Values)
            {
                m_DeadItems.Add(item);
            }

            m_Items.Clear();
        }

        void OnSelectedItem(ItemParam item)
        {
            if(m_SelectedItem != item)
            {
                if (m_SelectedItem != null)
                    m_SelectedItem.OnEnable(false);

                m_SelectedItem = item;

                if (m_SelectedItem != null)
                    m_SelectedItem.OnEnable(true);

                if (this.m_SelectedCallback != null)
                    this.m_SelectedCallback();
            }
            else
            {

            }
        }
    }
}
#endif
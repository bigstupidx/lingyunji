#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using xys.UI;
    using xys.UI.State;
    using UIWidgets;
    using Config;
    using UnityEngine.UI;

    [System.Serializable]
    class UIDPScroll
    {
        [SerializeField]
        Accordion m_ItemAccGrid;
        [SerializeField]
        Accordion m_DemonAccGrid;

        [SerializeField]
        Transform m_DropItems;
        [SerializeField]
        GameObject m_ItemPrefab;
        [SerializeField]
        Transform m_ItemGrid;

        DemonplotSkillType m_SkillType;
        public DemonplotSkillType skilltype { get { return m_SkillType; } }

        Action m_SelectedCallback;
        public Action selectedCallback {set { m_SelectedCallback = value; } }

        Transform m_SelectedItem = null;
        Dictionary<DemonplotSkillType, Transform> m_Items = new Dictionary<DemonplotSkillType, Transform>();

        public void OnInit()
        {
            m_ItemAccGrid.OnlyOneOpen = false;
            m_DemonAccGrid.OnlyOneOpen = false;
             this.Refleash();
        }

        public void OnHide()
        {
            if (m_SelectedItem != null)
                m_SelectedItem.GetComponent<StateRoot>().CurrentState = 0;
            m_SelectedItem = null;

            for(int i = 0; i < m_ItemAccGrid.Items.Count;i++)
            {
                AccordionItem item = m_ItemAccGrid.Items[i];
                item.ContentObject.transform.SetParent(m_DropItems);
                item.ContentObject.SetActive(false);
            }
            m_ItemAccGrid.Items.Clear();

            for (int i = 0; i < m_DemonAccGrid.Items.Count; i++)
            {
                AccordionItem item = m_DemonAccGrid.Items[i];
                item.ContentObject.transform.SetParent(m_DropItems);
                item.ContentObject.SetActive(false);
            }
            m_DemonAccGrid.Items.Clear();

            m_Items.Clear();
        }

        public void Refleash()
        {
            this.Create(DemonplotProperty.m_ItemGroup, m_ItemAccGrid);
            this.Create(DemonplotProperty.m_DemonGroup, m_DemonAccGrid);
            //打开默认选择第一个 
            if(m_Items.Count > 0)
            {
                List<DemonplotSkillType> list = new List<DemonplotSkillType>(m_Items.Keys);
                this.OnSelectItem(list[0], m_Items[list[0]]);
            }
        }
       void Create(Dictionary<DemonplotSkillType, List<DemonplotProperty>> dic,Accordion root)
        {
            foreach(DemonplotSkillType type in dic.Keys)
            {
                this.CreateItems(type,root);
            }
        }
        bool CreateItems(DemonplotSkillType type, Accordion root)
        {
            GameObject obj = null;
            if(m_DropItems.childCount == 0)
            {
                obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null) return false;
            }
            else
            {
                obj = m_DropItems.GetChild(0).gameObject;
            }
            Button btn = obj.GetComponent<Button>();
            if (btn == null)
                return false;

            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localPosition = Vector3.zero;

            //加入acc组件
            AccordionItem accItem = new AccordionItem();
            accItem.ToggleObject = root.gameObject;
            accItem.ContentObject = obj;
            accItem.Open = true;
            root.Items.Add(accItem);

            obj.GetComponentInChildren<Text>().text = DemonplotSkill.Get(type).name;

            //设置层级
            obj.transform.SetSiblingIndex(root.transform.GetSiblingIndex() + root.Items.Count);

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { this.OnSelectItem(type, obj.transform); });
            m_Items.Add(type, obj.transform);
            return true;
        }

        void OnSelectItem(DemonplotSkillType type,Transform select)
        {
            if(m_SelectedItem != select)
            {
                if (m_SelectedItem != null)
                    m_SelectedItem.GetComponent<StateRoot>().CurrentState = 0;

                m_SelectedItem = select;

                if(m_SelectedCallback != null)
                    m_SelectedItem.GetComponent<StateRoot>().CurrentState = 1;

                m_SkillType = type;

                if (m_SelectedCallback != null)
                    m_SelectedCallback();
            }
            else
            {

            }
        }
    }
}
#endif

#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using NetProto;
    using NetProto.Hot;
    using Config;
    using xys.UI;
    using xys.UI.State;

    [System.Serializable]
    class UITrumpsTrainScroll 
    {
        protected class Param
        {
            Image m_Icon;
            Image m_Quality;
            Text m_Name;
            Text m_Level;
            Button m_Btn;

            StateRoot m_SelectedSR;

            int m_TrumpId = 0;

            System.Action<int> m_Action = null;

            public void OnInit(Transform root, System.Action<int> action = null)
            {
                m_Action = action;

                m_Icon = root.Find("Icon").GetComponent<Image>();
                m_Quality = root.Find("Quality").GetComponent<Image>();
                m_Name = root.Find("Name").GetComponent<Text>();
                m_Level = root.Find("Level").GetComponent<Text>();

                m_SelectedSR = root.GetComponent<StateRoot>();

                m_Btn = root.GetComponent<Button>();
                m_Btn.onClick.AddListener(this.OnClick);
            }

            public void Set(TrumpAttribute attribute, TrumpsMgr mgr)
            {
                m_TrumpId = attribute.id;

                TrumpProperty property = TrumpProperty.Get(attribute.id);
                Helper.SetSprite(m_Icon, property.icon);
                Helper.SetSprite(m_Quality, QualitySourceConfig.Get(property.quality).icon);
                m_Name.text = TrumpProperty.GetColorName(property.id);
                m_Level.text = mgr.GetTasteDes(attribute.id); //string.Format("第{0}重 第{1}层", attribute.tastelv, attribute.infuseds.Count + 1);
            }

            public void OnSelected(bool isShow)
            {
                m_SelectedSR.CurrentState = isShow ? 1 : 0;
            }
            
            void OnClick()
            {
                if (m_Action != null)
                    m_Action(m_TrumpId);
            }
        }
        [SerializeField]
        Transform m_ItemGrid;
        [SerializeField]
        Transform m_DropItems;
        [SerializeField]
        GameObject m_ItemPrefab;

        private List<float> m_ChildrenPos = new List<float>();

        System.Action m_SelectedCallback = null;
        public System.Action selectedCallback { set { m_SelectedCallback = value; } }

        int m_SelectedTrump;
        public int selectedTrump { get { return m_SelectedTrump; } }

        Dictionary<int, Param> m_Items;
        TrumpsMgr m_TrumpMgr;

        public void OnInit()
        {
            m_Items = new Dictionary<int, Param>();

            m_TrumpMgr = App.my.localPlayer.GetModule<TrumpsModule>().trumpMgr as TrumpsMgr;
        }

        public void Create(int selecteTrump = 0)
        {
            this.Clear();
            //佩戴
            foreach (int trumpid in m_TrumpMgr.table.equiptrumps.Values)
                if (trumpid != 0)
                    this.NewItems(m_TrumpMgr.table.attributes[trumpid]);
            //激活
            List<int> keys = new List<int>(m_TrumpMgr.table.attributes.Keys);
            keys.Sort();
            foreach (int trumpid in keys)
                if (!m_Items.ContainsKey(trumpid))
                    this.NewItems(m_TrumpMgr.table.attributes[trumpid]);
            //
            if (m_Items.Count > 0)
            {
                this.ResetGridPos();
                keys = new List<int>(m_Items.Keys);
                if (selecteTrump == 0)
                {
                    this.OnSelectedItem(keys[0]);
                    this.SelecteTrump(0);
                }
                else
                {
                    this.OnSelectedItem(selecteTrump);
                    this.SelecteTrump(keys.IndexOf(selectedTrump));
                }
            }
        }


        public void Refresh()
        {
            this.Create(m_SelectedTrump);
        }

        void ResetGridPos()
        {
            m_ChildrenPos.Clear();
            EasyLayout.EasyLayout grid = m_ItemGrid.GetComponent<EasyLayout.EasyLayout>();
            if (grid != null)
            {
                for (int i = 0; i < m_ItemGrid.childCount; i++)
                {
                    m_ChildrenPos.Add((i * (m_ItemGrid.GetChild(i).GetComponent<RectTransform>().sizeDelta.y + grid.Spacing.y)));
                }
            }
        }
        void SelecteTrump(int index)
        {
            float screenHight = m_ItemGrid.parent.GetComponent<RectTransform>().sizeDelta.y;
            float showHight = m_ChildrenPos[index];
            float offseHight = m_ItemGrid.GetChild(index).GetComponent<RectTransform>().sizeDelta.y;
            if (screenHight < showHight)
            {
                m_ItemGrid.localPosition = new Vector3(m_ItemGrid.localPosition.x, showHight - screenHight + offseHight, m_ItemGrid.localPosition.z);
            }
                
        }
        bool NewItems(TrumpAttribute attribute)
        {
            GameObject item = null;
            if(m_DropItems.childCount > 0)
            {
                item = m_DropItems.GetChild(0).gameObject;
            }
            else
            {
                item = GameObject.Instantiate(m_ItemPrefab);
                if (item == null)
                    return false;
            }
            item.SetActive(true);
            item.transform.SetParent(m_ItemGrid, false);
            item.transform.localScale = Vector3.one;

            Param param = new Param();
            param.OnInit(item.transform,this.OnSelectedItem);
            param.Set(attribute, m_TrumpMgr); 
            m_Items.Add(attribute.id, param);
            return true;
        }

        public void Clear()
        {
            if (m_SelectedTrump != 0)
                m_Items[m_SelectedTrump].OnSelected(false);
            m_SelectedTrump = 0;

            while(m_ItemGrid.childCount > 0)
            {
                Transform item = m_ItemGrid.transform.GetChild(0);
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }
            m_Items.Clear();
        }

        void OnSelectedItem(int trumpId)
        {
            if(m_SelectedTrump != trumpId)
            {
                if (m_SelectedTrump != 0)
                    m_Items[m_SelectedTrump].OnSelected(false);

                m_SelectedTrump = trumpId;

                if(m_SelectedTrump != 0)
                    m_Items[m_SelectedTrump].OnSelected(true);

                if (m_SelectedCallback != null)
                    m_SelectedCallback();
            }
        }
    }
}
#endif

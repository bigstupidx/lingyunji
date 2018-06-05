#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using battle;
    using System;

    [System.Serializable]
    public class UITrumpIdentifyScroll
    {
        protected class Param
        {
            Image m_Quality;
            Image m_Icon;
            StateRoot m_StateRoot;
            Button m_Btn;
            GameObject m_Selected;

            int m_TrumpID = 0;
            System.Action<int> m_Action = null; 

            public void OnInit(Transform transform)
            {
                m_Quality = transform.Find("Quality").GetComponent<Image>();
                m_Icon = transform.Find("Icon").GetComponent<Image>();
                m_StateRoot = transform.GetComponent<StateRoot>();
                m_Btn = transform.GetComponent<Button>();
                m_Selected = transform.Find("Selected").gameObject;

                m_Btn.onClick.AddListener(this.OnClickEvent);
            }

            public void Set(TrumpProperty property,int currectIndex,System.Action<int> action)
            {
                m_Action = action;
                Helper.SetSprite(m_Icon,property.icon);
                Helper.SetSprite(m_Quality, QualitySourceConfig.Get(property.quality).icon);
                //
                m_StateRoot.CurrentState = currectIndex;

                m_TrumpID = property.id;
            }

            public void Refresh(int currentIndex)
            {
                m_StateRoot.CurrentState = currentIndex;
            }

            void OnClickEvent()
            {
                if (m_Action != null)
                    m_Action(m_TrumpID);
            }

            public int trumpid { get { return m_TrumpID; } }
            public GameObject selectedObj { get { return m_Selected; } }
        }
        
        [SerializeField]
        Transform m_ItemGrid;
        [SerializeField]
        GameObject m_ItemPrefab;

        int m_SelectedTrumps = 0;
        public int selecteTrumps { get { return m_SelectedTrumps; } }

        public System.Action selectedCallback { private get; set; }

        Dictionary<int, Param> m_Items = new Dictionary<int, Param>();

        TrumpsMgr m_TrumpMgr;

        public void OnInit()
        {
            m_TrumpMgr = App.my.localPlayer.GetModule<TrumpsModule>().trumpMgr as TrumpsMgr;
            //初始化法宝列表
            foreach (TrumpProperty property in TrumpProperty.GetAll().Values)
                this.CreateItem(property);
        }

        public void ResetSelectedTrump()
        {
            this.Reflesh();
            if (m_SelectedTrumps != 0)
                m_Items[m_SelectedTrumps].selectedObj.SetActive(false);
            m_SelectedTrumps = 0;

            if (m_Items.Count > 0)
            {
                List<int> keys = new List<int>(m_Items.Keys);
                this.OnSelectItem(keys[0]);
            }
            m_ItemGrid.transform.localPosition = Vector3.zero;
        }

        void CreateItem(TrumpProperty property)
        {
            GameObject obj =  GameObject.Instantiate(m_ItemPrefab);
            if (obj == null)
                return;
            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localPosition = Vector3.zero;

            Param param = new Param();
            param.OnInit(obj.transform);
            int equipIndex = m_TrumpMgr.GetEquipPos(property.id);
            if (!m_TrumpMgr.CheckTrumps(property.id))
                param.Set(property, 5, this.OnSelectItem);
            else if (equipIndex == -1)
                param.Set(property, 0, this.OnSelectItem);
            else
                param.Set(property, equipIndex + 1, this.OnSelectItem);

            m_Items.Add(property.id, param);
        }
        
        void Reflesh()
        {
            foreach (Param param in m_Items.Values)
            {
                int equipIndex = m_TrumpMgr.GetEquipPos(param.trumpid);
                if (!m_TrumpMgr.CheckTrumps(param.trumpid))
                    param.Refresh(5);
                else if (equipIndex == -1)
                    param.Refresh(0);
                else
                    param.Refresh(equipIndex + 1);
            }
        }

        void OnSelectItem(int selectedIndex)
        {
            if(m_SelectedTrumps != selectedIndex)
            {
                if (m_Items.ContainsKey(m_SelectedTrumps))
                    m_Items[m_SelectedTrumps].selectedObj.gameObject.SetActive(false);

                m_SelectedTrumps = selectedIndex;

                if (m_Items.ContainsKey(m_SelectedTrumps))
                    m_Items[m_SelectedTrumps].selectedObj.gameObject.SetActive(true);

                if (selectedCallback != null)
                    selectedCallback();
            }
        }
    }
}
#endif
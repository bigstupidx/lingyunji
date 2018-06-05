#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using NetProto;
    using System;
    using NetProto.Hot;

    [System.Serializable]
    class UITrumpsEquipScroll
    {
        protected class Param
        {
            Image m_Icon;
            Image m_Quality;
            Text m_Name;
            Text m_Level;
            Text m_TasteLv;
            Button m_Btn;
            Button m_TipsBtn;
            //
            StateRoot m_StateRoot;
            StateRoot m_EquipSR;
            //
            int m_TrumpId;
            //
            System.Action<int> m_ItemTipsCallback = null;
            public System.Action<int> itemTipsCallback { set { m_ItemTipsCallback = value; } }
            //
            System.Action<int> m_TrumpTipsCallback = null;
            public System.Action<int> trumpTipsCallback { set { m_TrumpTipsCallback = value; } }
            TrumpsMgr m_Mgr;
            public void OnInit(Transform root)
            {
                m_Mgr = App.my.localPlayer.GetModule<TrumpsModule>().trumpMgr as TrumpsMgr;

                m_StateRoot = root.GetComponent<StateRoot>();
                m_EquipSR = root.Find("Tag").GetComponent<StateRoot>();
                m_Icon = root.Find("Icon").GetComponent<Image>();
                m_Quality = root.Find("Quality").GetComponent<Image>();
                m_Name = root.Find("Name").GetComponent<Text>();
                m_Level = root.Find("Level").GetComponent<Text>();
                m_Btn = root.Find("Btn").GetComponent<Button>();
                m_Btn.onClick.AddListener(this.OnClickEvent);
                m_TipsBtn = root.Find("TipsBtn").GetComponent<Button>();
                m_TasteLv = root.Find("TasteLv").GetComponent<Text>();
                m_TipsBtn.onClick.AddListener(this.OnTipsEvent);
            }
           public  void Set(TrumpProperty property)
            {
                m_StateRoot.CurrentState = 2;
                m_TrumpId = property.id;

                m_EquipSR.gameObject.SetActive(false);
                Helper.SetSprite(m_Icon, property.icon);
                Helper.SetSprite(m_Quality, QualitySourceConfig.Get(property.quality).icon);
                m_Name.text = TrumpProperty.GetColorName(property.id);
                m_Level.text = m_Mgr.GetInfusedDes(property.id);
                m_TasteLv.text = m_Mgr.GetTasteDes(property.id);
            }
            public void Set(TrumpAttribute attribute,int currentIndex = 1)
            {
                int index = m_Mgr.GetEquipPos(attribute.id);
                m_StateRoot.CurrentState = currentIndex;// index != -1 ? 0 : 1;

                m_EquipSR.gameObject.SetActive(index != -1);
                m_EquipSR.CurrentState = index != -1 ? index : 1;

                m_TrumpId = attribute.id;

                TrumpProperty property = TrumpProperty.Get(attribute.id);
                Helper.SetSprite(m_Icon, property.icon);
                Helper.SetSprite(m_Quality, QualitySourceConfig.Get(property.quality).icon);
                m_Name.text = TrumpProperty.GetColorName(property.id);
                m_TasteLv.text = m_Mgr.GetTasteDes(property.id);
                m_Level.text = m_Mgr.GetInfusedDes(property.id);
            }

            void OnClickEvent()
            {
                if (m_ItemTipsCallback != null)
                    m_ItemTipsCallback(m_TrumpId);
            }

            void OnTipsEvent()
            {
                if (m_TrumpTipsCallback != null)
                    m_TrumpTipsCallback(m_TrumpId);
            }
        }
        
        [SerializeField]
        Transform m_ItemGrid;
        [SerializeField]
        GameObject m_ItemPrefab;

        [SerializeField]
        float m_PosX = -134.0f;
        [SerializeField]
        float m_PosY = 279.0f;

        int m_SelectedTrump;
        List<Param> m_Items;

        System.Action<int> m_SelectedTrumpCallback = null;
        public System.Action<int> selectedTrump { set { m_SelectedTrumpCallback = value; } }

        TrumpsMgr m_TrumpMgr;
        public void OnInit()
        {
            this.m_Items = new List<Param>();
            foreach(TrumpProperty property in TrumpProperty.GetAll().Values)
            {
                GameObject obj = this.NewItems(property.id);
                Param param = new Param();
                param.OnInit(obj.transform);
                param.itemTipsCallback = this.OnItemTipsEvent;
                param.trumpTipsCallback = this.OnTrumpTipsEvent;
                m_Items.Add(param);
            }
            m_TrumpMgr = App.my.localPlayer.GetModule<TrumpsModule>().trumpMgr as TrumpsMgr;
        }

        public void Refreash(int equipIndex = -1)
        {
            int index = 0;
            int equipTrumpId = 0;
            List<int> trumps = new List<int>();
            if (m_TrumpMgr.table.equiptrumps.ContainsKey(equipIndex))
                equipTrumpId = m_TrumpMgr.table.equiptrumps[equipIndex];
            //佩戴
            foreach (int trumpid in m_TrumpMgr.table.equiptrumps.Values)
            {
                if (trumpid != 0)
                {
                    m_Items[index++].Set(m_TrumpMgr.table.attributes[trumpid], trumpid == equipTrumpId ? 0 : 1);
                    trumps.Add(trumpid);
                }
            }
            //激活
            List<int> keys = new List<int>(m_TrumpMgr.table.attributes.Keys);
            keys.Sort();
            foreach (int trumpid in keys)
            {
                if (!trumps.Contains(trumpid))
                {
                    m_Items[index++].Set(m_TrumpMgr.table.attributes[trumpid]);
                    trumps.Add(trumpid);
                }
            }
            //未激活
            keys = new List<int>(TrumpProperty.GetAll().Keys);
            foreach (int trumpid in keys)
                if (!trumps.Contains(trumpid))
                    m_Items[index++].Set(TrumpProperty.Get(trumpid));
        }

        GameObject NewItems(int trumpid)
        {
            GameObject item = GameObject.Instantiate(m_ItemPrefab);
            if (item == null)
                return null;
            item.SetActive(true);
            item.transform.SetParent(m_ItemGrid, false);
            item.transform.localScale = Vector3.one;
            return item;
        }

        void OnItemTipsEvent(int trumpId)
        {
            if (!m_TrumpMgr.CheckTrumps(trumpId))
                SystemHintMgr.ShowHint("道具获取tips");
            else
                if (m_SelectedTrumpCallback != null)
                    m_SelectedTrumpCallback(trumpId);
        }

        void OnTrumpTipsEvent(int trumpId)
        {
            //m_TrumpTips.Set(trumpId, new Vector3(m_PosX, m_PosY, 0.0f));
            UICommon.ShowTrumpTips(trumpId, new Vector2(m_PosX, m_PosY));
        }
    }
}
#endif

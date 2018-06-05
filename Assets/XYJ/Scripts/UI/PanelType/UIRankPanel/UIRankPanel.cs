#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    partial class UIRankPanel : HotPanelBase
    {
        [SerializeField]
        Accordion m_rankTypeSelector;
        [SerializeField]
        GameObject m_rankTypeItemPrefab;
        [SerializeField]
        GameObject m_playerRankBtn;
        [SerializeField]
        GameObject m_guildRankBtn;
        [SerializeField]
        GameObject m_activityRankBtn;
        [SerializeField]
        Text m_rankOrderTag;
        [SerializeField]
        Text m_rankOrderVal;
        [SerializeField]
        Text m_rankAbilityTag;
        [SerializeField]
        Text m_rankAbilityVal;
        [SerializeField]
        Transform m_filterGroup;
        [SerializeField]
        GameObject m_smallFilterItemPrefab;
        [SerializeField]
        GameObject m_bigFilterItemPrefab;
        [SerializeField]
        Transform m_titles;
        [SerializeField]
        GameObject m_rankItemPrefab;
        [SerializeField]
        ListViewRankItems m_rankItems;
        [SerializeField]
        StateRoot m_filterState;

        public UIRankPanel() : base(null) { }
        public UIRankPanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            m_rankTypeSelector.OnToggleItem.AddListener((AccordionItem toggleItem) => {
                hotApp.my.mainTimer.Register(0.001f, 1, ()=> {
                    OnToggleAccordionItem(toggleItem);
                });
            });

            this.InitRankOperas();
        }

        AccordionItem m_openAccItem;
        void OnToggleAccordionItem(AccordionItem toggleItem)
        {
            GameObject selectedRankTypeGo = null;
            if (toggleItem.Open)
            {
                if (toggleItem != m_openAccItem)
                {
                    if (null != m_openAccItem)
                    {
                        m_rankTypeSelector.Close(m_openAccItem);
                        m_openAccItem.ToggleObject.GetComponent<StateRoot>().SetCurrentState(0, false);
                        m_openAccItem = null;
                    }

                    if (toggleItem.ContentObject.transform.childCount > 0)
                        selectedRankTypeGo = toggleItem.ContentObject.transform.GetChild(0).gameObject;
                }
                m_openAccItem = toggleItem;
                Transform transform = m_rankTypeSelector.gameObject.transform;
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                m_rankTypeSelector.GetComponent<xys.UI.ContentSizeFitter>().SetDirty();
            }
            toggleItem.ToggleObject.GetComponent<StateRoot>().SetCurrentState(toggleItem.Open ? 1 : 0, false);
            if (null != selectedRankTypeGo)
                this.SelectRankType(selectedRankTypeGo);
        }
        protected override void OnShow(object p)
        {
            if (m_rankTypeSelector.Items.Count > 0)
            {
                var accItem = m_rankTypeSelector.Items.Find(x => x.ToggleObject == m_playerRankBtn);
                m_rankTypeSelector.Open(accItem);
            }
        }
        protected override void OnHide()
        {

        }

        RankOpera m_rankOpera;
        RankQueryRankResult m_rankDatas;
        GameObject m_selectedRankGo;
        GameObject m_selectedFilterGo;
        Dictionary<GameObject, RankOpera> m_rankOperas = new Dictionary<GameObject, RankOpera>();
        void SelectRankType(GameObject go)
        {
            if (null == go)
                return;
            RankOpera rankOpera;
            if (!m_rankOperas.TryGetValue(go, out rankOpera))
                return;

            if (go != m_selectedRankGo)
            {
                if (null != m_selectedRankGo)
                {
                    m_selectedRankGo.GetComponent<StateRoot>().SetCurrentState(0, false);
                    m_rankOpera.Exit();
                }
                go.GetComponent<StateRoot>().SetCurrentState(1, false);
                m_rankOpera = rankOpera;
                m_rankOpera.Enter();
            }
            m_selectedRankGo = go;
        }
    }
}
#endif
#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using NetProto;
    using Config;
    using xys.UI;
    using xys.UI.State;
    using NetProto.Hot;

    [Serializable]
    class UIDPMatchinInfos
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        StateRoot m_StateRoot;

        [SerializeField]
        Image m_Icon;
        [SerializeField]
        Image m_Quality;
        [SerializeField]
        Transform m_MarRoot;
        [SerializeField]
        Button m_CalculateBtn;
        [SerializeField]
        Button m_MatchinBtn;
        [SerializeField]
        Text m_BtnName;
        //
        [SerializeField]
        Button m_AddBtn;
        [SerializeField]
        Button m_SubBtn;
        [SerializeField]
        Text m_ValueText;
        //
        [SerializeField]
        Button m_ExchangeBtn;
        [SerializeField]
        Text m_NeedText;
        [SerializeField]
        Text m_CostText;

        [SerializeField]
        Button m_BgBtn;

        System.Action<int> m_ChangeCallback = null;
        public System.Action<int> changeCallback { set { m_ChangeCallback = value; } }

        public bool isActive { get { return m_Transform.gameObject.activeSelf; } }

        int m_ItemGridIndex = -1;
        int m_MatchinID = 0;
        int m_SelectedValue = 1;
        int m_SelectedMaxValue = 1;
        public void OnInit()
        {
            m_CalculateBtn.onClick.AddListener(this.ShowCalculatePanelEvent);
            m_MatchinBtn.onClick.AddListener(this.MatchinEvent);
            m_AddBtn.onClick.AddListener(this.AddValueEvent);
            m_SubBtn.onClick.AddListener(this.SubValueEvent);
            m_ExchangeBtn.onClick.AddListener(this.ExchangeEvent);
            m_BgBtn.onClick.AddListener(() => { this.m_Transform.gameObject.SetActive(false); });
        }

        public void OnHide()
        {
            this.m_Transform.gameObject.SetActive(false);
        }

        public void Set(int matchinId)
        {
            if(m_MatchinID != matchinId)
                m_SelectedValue = 1;
            m_MatchinID = matchinId;
            m_SelectedMaxValue = this.GetMaxValue(matchinId);
            m_ItemGridIndex = -1;

            if (!DemonplotProperty.GetAll().ContainsKey(matchinId) || m_SelectedMaxValue == 0)
            {
                this.m_Transform.gameObject.SetActive(false);
                return;
            }
            this.m_Transform.gameObject.SetActive(true);
            //
            DemonplotProperty property = DemonplotProperty.Get(matchinId);
            Item itemData = Item.Get(property.produce.id);
            if (property.matchinitems.list.Count > 0)
                m_StateRoot.CurrentState = 0;
            else
                m_StateRoot.CurrentState = 1;
            //
            Helper.SetSprite(m_Icon, itemData.icon);
            Helper.SetSprite(m_Quality, QualitySourceConfig.Get(itemData.quality).icon);
            //
            for (int i = 0; i < m_MarRoot.childCount; i++)
                m_MarRoot.GetChild(i).gameObject.SetActive(false);
            if(property.useitemid != 0)
            {
                Item marItemData = Item.Get(property.produce.id);
                Transform item = m_MarRoot.GetChild(0);
                item.gameObject.SetActive(true);
                Helper.SetSprite(item.Find("Icon").GetComponent<Image>(), marItemData.icon);
                Helper.SetSprite(item.Find("Quality").GetComponent<Image>(), QualitySourceConfig.Get(marItemData.quality).icon);
                item.Find("Text").GetComponent<Text>().text = string.Empty;

                m_ItemGridIndex = this.GetItemGridIndex(property.useitemid);
            }

            if (property.matchinitems.list.Count > 0)
            {
                int index = 0;
                for (int i = 0; i < property.matchinitems.list.Count; i++, index++)
                {
                    int id = property.matchinitems.list[i].id;
                    Transform item = m_MarRoot.GetChild(i);
                    if (!Item.GetAll().ContainsKey(id))
                        continue;
                    item.gameObject.SetActive(true);
                    Item marItemData = Item.Get(id);
                    Helper.SetSprite(item.Find("Icon").GetComponent<Image>(), marItemData.icon);
                    Helper.SetSprite(item.Find("Quality").GetComponent<Image>(), QualitySourceConfig.Get(marItemData.quality).icon);
                    item.Find("Text").GetComponent<Text>().text = property.matchinitems.list[i].count + "/" + hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(id);
                }
            }
            //
            m_NeedText.text = App.my.localPlayer.energyValue.ToString();

            if (property.needenergy > App.my.localPlayer.energyValue)
                m_CostText.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", "R1", (m_SelectedValue * property.needenergy).ToString()));
            else
                m_CostText.text = (m_SelectedValue * property.needenergy).ToString();
            //
            m_MatchinBtn.GetComponentInChildren<Text>().text = DemonplotSkill.Get(property.skilltype).matchinname;
            //
            m_ValueText.text = m_SelectedValue.ToString();
            this.ValueChangeEvent(m_SelectedValue);
        }

        #region 事件
        void ShowCalculatePanelEvent()
        {
            UICommon.ShowCalculator(new Vector3(22, 119, 0), m_SelectedMaxValue, 1, 1, this.ValueChangeEvent);
        }

        void ValueChangeEvent(int value)
        {
            m_ValueText.text = value.ToString();
            m_SelectedValue = value;
            m_SubBtn.GetComponent<StateRoot>().CurrentState = m_SelectedValue != 1 ? 1 : 0;
            m_AddBtn.GetComponent<StateRoot>().CurrentState = value < m_SelectedMaxValue ? 1 : 0;

            if (m_ChangeCallback != null)
                m_ChangeCallback(m_SelectedValue);

            DemonplotProperty property = DemonplotProperty.Get(m_MatchinID);
            value = m_SelectedValue * property.needenergy;
            if (property.needenergy > App.my.localPlayer.energyValue)
                m_CostText.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", "R1", value.ToString()));
            else
                m_CostText.text = value.ToString();
        }
        void MatchinEvent()
        {
            #region 
            //发送协议事件
            DemonplotRequest dr = new DemonplotRequest();
            dr.itemindex = m_ItemGridIndex;
            dr.matchinid = m_MatchinID;
            App.my.eventSet.FireEvent(EventID.Demonplot_Matchin, dr);
            #endregion
        }
        void AddValueEvent()
        {
            if (m_SelectedValue >= m_SelectedMaxValue)
                return;
            m_SelectedValue += 1;
            m_AddBtn.GetComponent<StateRoot>().CurrentState = m_SelectedValue < m_SelectedMaxValue ? 1 : 0;
            m_SubBtn.GetComponent<StateRoot>().CurrentState = m_SelectedValue != 1 ? 1 : 0;
            m_ValueText.text = m_SelectedValue.ToString();

            DemonplotProperty property = DemonplotProperty.Get(m_MatchinID);
            int value = m_SelectedValue * property.needenergy;
            if (property.needenergy > App.my.localPlayer.energyValue)
                m_CostText.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", "R1", value.ToString()));
            else
                m_CostText.text = value.ToString();

            if (m_ChangeCallback != null)
                m_ChangeCallback(m_SelectedValue);
        }
        void SubValueEvent()
        {
            if (m_SelectedValue <= 1)
                return;
            m_SelectedValue -= 1;
            m_AddBtn.GetComponent<StateRoot>().CurrentState = m_SelectedValue < m_SelectedMaxValue ? 1 : 0;
            m_SubBtn.GetComponent<StateRoot>().CurrentState = m_SelectedValue != 1 ? 1 : 0;
            m_ValueText.text = m_SelectedValue.ToString();

            DemonplotProperty property = DemonplotProperty.Get(m_MatchinID);
            int value = m_SelectedValue * property.needenergy;
            if (property.needenergy > App.my.localPlayer.energyValue)
                m_CostText.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", "R1", value.ToString()));
            else
                m_CostText.text = value.ToString();

            if (m_ChangeCallback != null)
                m_ChangeCallback(m_SelectedValue);
        }
        void ExchangeEvent()
        {
            //打开兑换商店
        }
        #endregion

        #region 私有
        int GetItemGridIndex(int itemId)
        {
            int index = -1;
            hotApp.my.GetModule<HotPackageModule>().packageMgr.package.ForEach((Grid g) =>
            {
                if (g.data != null)
                {
                    if (index == -1 && Item.Get(g.id).sonType == (int)ItemChildType.collectItem && g.id == itemId)
                    {
                        index = g.pos;
                    }
                }
            });
            return index;
        }
        int GetMaxValue(int matchinId)
        {
            int minValue = 0;
            if (!DemonplotProperty.GetAll().ContainsKey(matchinId))
                return minValue;
            if (App.my.localPlayer.energyValue == 0)
                return minValue;
            //
            DemonplotProperty property = DemonplotProperty.Get(matchinId);
            //活力
            minValue = minValue <= (int)(App.my.localPlayer.energyValue / property.needenergy) ? (int)(App.my.localPlayer.energyValue / property.needenergy) : minValue;
            //道具耐久
            if (property.useitemid != 0 && Item.GetAll().ContainsKey(property.useitemid))
            {
                int durable = 0;
                hotApp.my.GetModule<HotPackageModule>().packageMgr.package.ForEach((Grid g) =>
                {
                    if (g.data != null)
                        if (Item.Get(g.id).sonType == (int)ItemChildType.collectItem && g.id == property.useitemid)
                            durable += g.data.data.durable;
                });
                if (property.durable == 0)
                    return 0;
                minValue = minValue > durable / property.durable ? durable / property.durable : minValue;
            }
            //材料
            if (property.matchinitems.list.Count > 0)
            {
                int count = 0;
                for (int i = 0; i < property.matchinitems.list.Count; i++)
                {
                    count = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(property.matchinitems.list[i].id);
                    if (count == 0)
                        return 0;
                    minValue = minValue > count / property.matchinitems.list[i].count ? count / property.matchinitems.list[i].count : minValue;
                }
            }
            return minValue;
        }
        #endregion
    }
}
#endif
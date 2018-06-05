#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using NetProto;
    using NetProto.Hot;

    [System.Serializable]
    class UIDPItemInfos
    {
        [SerializeField]
        UIDPMatchinInfos m_MatchinInfos;
        [SerializeField]
        Image m_Icon;
        [SerializeField]
        Image m_Quality;
        [SerializeField]
        Text m_ItemName;
        [SerializeField]
        Text m_ItemDes;
        [SerializeField]
        Text m_SkillLevel;
        [SerializeField]
        Text m_ProductMap;

        [SerializeField]
        Transform m_MatricalRoot;

        [SerializeField]
        Text m_NeedEnergy;
        [SerializeField]
        Text m_HaveEnergy;

        [SerializeField]
        Text m_ToolText;
        [SerializeField]
        Text m_Durable;

        [SerializeField]
        Button m_MatchinBtn;
        [SerializeField]
        Button m_ExchangeBtn;

        int m_MatchinID;

        public void OnInit()
        {
            m_MatchinBtn.onClick.AddListener(this.MatchinEvent);
            m_ExchangeBtn.onClick.AddListener(this.ExchangeEvent);
            m_MatchinInfos.OnInit();
            m_MatchinInfos.changeCallback = this.ChangeValueEvent;
        }
        public void OnHide()
        {
            m_MatchinInfos.OnHide();
        }

        public void Set(DemonplotProperty data)
        {
            DemonplotsTable skillTables = ((DemonplotsMgr)App.my.localPlayer.GetModule<DemonplotsModule>().demonplotMgr).m_Tables;
            if (skillTables == null)
                return;

            m_MatchinID = data.id;
            Item itemData = Config.Item.Get(data.produce.id);
            Helper.SetSprite(m_Icon, itemData.icon);
            Helper.SetSprite(m_Quality, QualitySourceConfig.Get(itemData.quality).icon);

            m_ItemName.text = string.Format("<color=#{0}>{1}</color>", QualitySourceConfig.Get(itemData.quality).color, itemData.name);
            m_ItemDes.text = GlobalSymbol.ToUT(itemData.desc);
            m_SkillLevel.text = data.needlv.ToString();
            if (data.needenergy > App.my.localPlayer.energyValue)
                m_NeedEnergy.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", "R1", data.needenergy.ToString()));
            else
                m_NeedEnergy.text = data.needenergy.ToString();


            m_HaveEnergy.text = App.my.localPlayer.energyValue.ToString();

            for (int i = 0; i < m_MatricalRoot.childCount; i++)
                m_MatricalRoot.GetChild(i).gameObject.SetActive(false);

            if (data.useitemid != 0)
            {
                m_ProductMap.gameObject.SetActive(true);
                m_ProductMap.text = data.producemapname;
                this.SetChildUI(data.useitemid, data.durable, m_MatricalRoot.GetChild(0));
            }
            if(data.matchinitems.list.Count != 0)
            {
                m_ProductMap.gameObject.SetActive(false);
                m_ProductMap.text = string.Empty;
                for (int i = 0; i < data.matchinitems.list.Count; i++)
                {
                    this.SetChildUI(data.matchinitems.list[i].id, data.matchinitems.list[i].count, m_MatricalRoot.GetChild(i));
                }
            }
            //判断是否满足技能等级
            if (skillTables.skills.ContainsKey((int)data.skilltype) && data.needlv > skillTables.skills[(int)data.skilltype].lv)
                m_MatchinBtn.enabled = false;
            else
                m_MatchinBtn.enabled = true;
            m_MatchinBtn.GetComponentInChildren<Text>().text = DemonplotSkill.Get(data.skilltype).matchinname;
            //如果小面板打开，刷新小面板数据
            if (m_MatchinInfos.isActive)
                m_MatchinInfos.Set(m_MatchinID);
        }

        void SetChildUI(int itemId, int needCount, Transform root)
        {
            PackageMgr mgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (mgr == null)
                return;
            Item itemData = Item.Get(itemId);
            if (itemData == null)
                return;

            this.m_Durable.text = string.Empty;

            root.gameObject.SetActive(true);
            Text res = root.Find("Text").GetComponent<Text>();
            GameObject emptyIcon = root.Find("EmptyIcon").gameObject;

            Button clickIcon = root.Find("EmptyBtn").GetComponent<Button>();
            clickIcon.onClick.RemoveAllListeners();
            clickIcon.onClick.AddListener(() => { this.ClickMarEvent(itemId); });

            Helper.SetSprite(root.Find("Icon").GetComponent<Image>(), itemData.icon);
            Helper.SetSprite(root.Find("Quality").GetComponent<Image>(), QualitySourceConfig.Get(itemData.quality).icon);

            res.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n",
                mgr.GetItemCount(itemId) < needCount ? "R1" : "G2",
                mgr.GetItemCount(itemId) + "/" + needCount));

            if (itemData.sonType == (int)ItemChildType.collectItem)
            {
                res.gameObject.SetActive(false);
                m_ToolText.text = "所需工具";
                int durable = 0;
                mgr.package.ForEach((Grid g) =>
                {
                    if (g.data != null)
                        if (Item.Get(g.id).sonType == (int)ItemChildType.collectItem && g.id == itemId)
                            durable = g.data.data.durable;
                });
                emptyIcon.SetActive(durable == 0);
                m_Durable.gameObject.SetActive(durable != 0);
                m_Durable.text = durable != 0 ? durable + "/" + 100 : string.Empty;
            }
            else
            {
                res.gameObject.SetActive(true);
                m_ToolText.text = "所需材料";
                m_Durable.gameObject.SetActive(false);
                emptyIcon.gameObject.SetActive(false);
            }
        }
        void ClickMarEvent(int itemId)
        {
            PackageMgr mgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (mgr == null)
                return;
            if (mgr.GetItemCount(itemId) <= 0)
                UICommon.ShowItemTips(itemId);
        }
        void ExchangeEvent()
        {
            SystemHintMgr.ShowHint("此处应该打开兑换界面");
        }

        void ChangeValueEvent(int value)
        {
            DemonplotProperty data = DemonplotProperty.Get(m_MatchinID);
            if (data.needenergy > App.my.localPlayer.energyValue)
                m_NeedEnergy.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", "R1", (value * data.needenergy).ToString()));
            else
                m_NeedEnergy.text = (value * data.needenergy).ToString();
        }

        void MatchinEvent()
        {
            PackageMgr mgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (mgr == null)
                return;
            if (!DemonplotProperty.GetAll().ContainsKey(m_MatchinID))
                return;
            DemonplotProperty property = DemonplotProperty.Get(m_MatchinID);
            DemonplotSkill skillproperty = DemonplotSkill.Get(property.skilltype);
            //判断工具
            MatchinData matchinData = property.matchinitems;
            if (property.useitemid != 0)
            {
                int itemId = property.useitemid;
                int durable = 0;
                mgr.package.ForEach((Grid g) =>
                {
                    if (g.data != null)
                        if (Item.Get(g.id).sonType == (int)ItemChildType.collectItem && g.id == itemId)
                            durable = g.data.data.durable;
                });
                //耐久不足
                if (durable < property.durable)
                {
                    UICommon.ShowItemTips(itemId);
                    return;
                }
            }
            if(matchinData.list.Count > 0)//判断材料
            {
                for (int i = 0; i < matchinData.list.Count; i++)
                {
                    if (matchinData.list[i].count > mgr.GetItemCount(matchinData.list[i].id))
                    {
                        UICommon.ShowItemTips(matchinData.list[i].id);
                        return;
                    }
                }
            }
            //判断活力
            if (property.needenergy > App.my.localPlayer.energyValue)
            {
                this.ExchangeEvent();
                return;
            }
            //判断背包是否已满
            if (mgr.isFullOfPackage())
            {
                SystemHintMgr.ShowHint(string.Format(TipsContent.GetByName("demonplot_package_full").des, skillproperty.matchinname.Replace(" ", string.Empty)));
                return;
            }

            m_MatchinInfos.Set(m_MatchinID);
        }
    }
}
#endif

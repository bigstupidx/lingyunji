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
    partial class UIRankPanel
    {
        void EmptyAction() { }
        class RankOpera
        {
            public RankOpera()
            {
                Enter = EmptyAction;
                Exit = EmptyAction;
                OnClickFilter = EmptyAction;
                OnClickRankItem = EmptyAction;
                HandleQueryRankResult = EmptyAction;
                HandleQueryRankDetail = EmptyAction;
            }

            public Config.RankPanel cfg;
            public Action Enter;
            public Action Exit;
            public Action<GameObject> OnClickFilter;
            public Action<ListViewRankItemData> OnClickRankItem;
            public Action<RankQueryRankResult> HandleQueryRankResult;
            public Action<RankQueryRankDetailResult> HandleQueryRankDetail;

            #region EmptyAction
            void EmptyAction() { }
            void EmptyAction(GameObject param) { }
            void EmptyAction(ListViewRankItemData param) { }
            void EmptyAction(RankQueryRankResult param) { }
            void EmptyAction(RankQueryRankDetailResult param) { }
            #endregion
        }

        void InitRankOperas()
        {
            Dictionary<Config.ClientRankType, RankOpera> rankOperas = new Dictionary<Config.ClientRankType, RankOpera>();
            foreach (var cfg in Config.RankPanel.GetAll().Values)
            {
                RankOpera opera = null;
                GameObject belongGo = null;
                if (cfg.clientRanktype > Config.ClientRankType.PlayerTypeMin && cfg.clientRanktype < Config.ClientRankType.PlayerTypeMax)
                {
                    opera = GenPlayerRankDefaultOpera(cfg.clientRanktype);
                    belongGo = m_playerRankBtn;
                }
                if (cfg.clientRanktype > Config.ClientRankType.GuildTypeMin && cfg.clientRanktype < Config.ClientRankType.GuildTypeMax)
                {
                    opera = GenGuildRankDefaultOpera(cfg.clientRanktype);
                    belongGo = m_guildRankBtn;
                }
                if (cfg.clientRanktype > Config.ClientRankType.ActivityMin && cfg.clientRanktype < Config.ClientRankType.ActivityMax)
                {
                    opera = GenDefaultOpera(cfg.clientRanktype);
                    belongGo = m_activityRankBtn;
                }
                if (null == belongGo || null == opera)
                    continue;
                AccordionItem accItem = m_rankTypeSelector.Items.Find(x => x.ToggleObject == belongGo);
                if (null == accItem)
                    continue;
                GameObject go = GameObject.Instantiate(m_rankTypeItemPrefab);
                go.SetActive(true);
                go.transform.parent = accItem.ContentObject.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = cfg.rankName;
                go.GetComponent<Button>().onClick.AddListener(() => { this.SelectRankType(go); });
                m_rankOperas[go] = opera;
                rankOperas[cfg.clientRanktype] = opera;
            }

            // 下边需要特殊处理的排行榜操作
            {
                RankOpera rankOpera = rankOperas[Config.ClientRankType.PetAbility];
                rankOpera.OnClickRankItem = (ListViewRankItemData data) => { QueryRankExtraData(rankOpera, data); };
                rankOpera.OnClickFilter = (GameObject go) => { DefaultPlayerRankOnClickFilterOpera(rankOpera, go, PetsRankDataItemModifier); };
                rankOpera.HandleQueryRankDetail = (RankQueryRankDetailResult data) => { PetRankHandleQueryRankDetail(rankOpera, data); };
            }
            {
                RankOpera rankOpera = rankOperas[Config.ClientRankType.Weapon];
                rankOpera.OnClickFilter = (GameObject go) => { DefaultPlayerRankOnClickFilterOpera(rankOpera, go, this.WeaponRankDataItemModifier); };
                rankOpera.OnClickRankItem = (ListViewRankItemData data) => { QueryRankExtraData(rankOpera, data); };
                rankOpera.HandleQueryRankDetail = (RankQueryRankDetailResult data) => { WeaponRankHandleQueryRankDetail(rankOpera, data); };
            }
            {
                RankOpera rankOpera = rankOperas[Config.ClientRankType.Shitu];
                rankOpera.OnClickFilter = (GameObject go) => { DefaultPlayerRankOnClickFilterOpera(rankOpera, go, this.ShituRankDataItemModifier); };
            }
        }

        RankOpera GenDefaultOpera(Config.ClientRankType clientRankType)
        {
            Config.RankPanel panelCfg = Config.RankPanel.GetCfg(clientRankType);
            if (null == panelCfg)
                return null;

            RankOpera opera = new RankOpera();
            opera.cfg = panelCfg;
            opera.Enter = () => { DefaultEnterOpera(opera); };
            opera.Exit = () => { DefaultExitOpera(opera); };
            opera.OnClickFilter = (GameObject go) => { };
            opera.OnClickRankItem = (ListViewRankItemData data) => { };
            opera.HandleQueryRankResult = (RankQueryRankResult data) => { };
            opera.HandleQueryRankDetail = (RankQueryRankDetailResult data) => { };
            return opera;
        }
        RankOpera GenPlayerRankDefaultOpera(Config.ClientRankType clientRankType)
        {
            Config.RankPanel panelCfg = Config.RankPanel.GetCfg(clientRankType);
            if (null == panelCfg)
                return null;

            RankOpera opera = new RankOpera();
            opera.cfg = panelCfg;
            opera.Enter = () => { DefaultEnterOpera(opera); };
            opera.Exit = () => { DefaultExitOpera(opera); };
            opera.OnClickFilter = (GameObject go) => { DefaultPlayerRankOnClickFilterOpera(opera, go); };
            opera.OnClickRankItem = (ListViewRankItemData data) => { DefaultPlayerRankOnClickRankItemOpera(opera, data); };
            opera.HandleQueryRankResult = (RankQueryRankResult data) => { DefaultPlayerRankHandleQueryRankResult(opera, data); };
            opera.HandleQueryRankDetail = (RankQueryRankDetailResult data) => { };
            return opera;
        }
        RankOpera GenGuildRankDefaultOpera(Config.ClientRankType clientRankType)
        {
            Config.RankPanel panelCfg = Config.RankPanel.GetCfg(clientRankType);
            if (null == panelCfg)
                return null;

            RankOpera opera = new RankOpera();
            opera.cfg = panelCfg;
            opera.Enter = () => { DefaultEnterOpera(opera); };
            opera.Exit = () => { DefaultExitOpera(opera); };
            opera.OnClickFilter = (GameObject go) => { DefaultGuildRankOnClickFilterOpera(opera, go); };
            opera.OnClickRankItem = (ListViewRankItemData data) => { };
            opera.HandleQueryRankResult = (RankQueryRankResult data) => { DefaultGuildRankHandleQueryRankResult(opera, data); };
            opera.HandleQueryRankDetail = (RankQueryRankDetailResult data) => { };
            return opera;
        }
        void DefaultEnterOpera(RankOpera opera)
        {
            if (opera != m_rankOpera)
                return;

            bool isShowFilter = Config.ClientRankFilterType.Invalid != opera.cfg.filterType;
            isShowFilter &= opera.cfg.filterParams.Length > 0;
            m_filterState.SetCurrentState((isShowFilter ? 1 : 0), false);
            if (Config.ClientRankFilterType.Invalid != opera.cfg.filterType)
            {
                GameObject filterPrefab = opera.cfg.filterStrs.Length > 7 ? m_smallFilterItemPrefab : m_bigFilterItemPrefab;
                int spacing = opera.cfg.filterStrs.Length > 7 ? 0 : 2;
                m_filterGroup.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().spacing = spacing;
                for (int i = 0; i < opera.cfg.filterStrs.Length; ++i)
                {
                    GameObject go = GameObject.Instantiate(filterPrefab);
                    go.SetActive(true);
                    go.transform.Find("Text").GetComponent<Text>().text = opera.cfg.filterStrs[i];
                    go.GetComponent<Button>().onClick.AddListener(() => { opera.OnClickFilter(go); });
                    go.transform.parent = m_filterGroup;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                }
            }
            const int TitleCount = 5;
            for (int i = 0; i < TitleCount; ++i)
            {
                m_titles.transform.Find(i.ToString()).GetComponent<Text>().text = opera.cfg.titles[i];
            }
            m_rankOrderTag.text = opera.cfg.rankOrderStr;
            m_rankAbilityTag.text = opera.cfg.rankAbilityStr;
            m_rankOrderVal.text = "-";
            m_rankAbilityVal.text = "-";
            opera.OnClickFilter(null);
            if (opera.cfg.serverRankType > 0)
            {
                hotApp.my.GetModule<HotRankModule>().QueryRank((RankType)opera.cfg.serverRankType, opera.HandleQueryRankResult);
            }
        }
        void DefaultExitOpera(RankOpera opera)
        {
            if (opera != m_rankOpera)
                return;

            m_rankDatas = null;
            m_selectedFilterGo = null;
            while (m_filterGroup.childCount > 0)
            {
                GameObject go = m_filterGroup.GetChild(0).gameObject;
                GameObject.DestroyImmediate(go);
            }
            m_rankItems.DataSource = new ObservableList<ListViewRankItemData>();
        }
        int GetFilterParams(GameObject go)
        {
            int ret = int.MinValue;
            if (null != go && null != m_rankOpera && null != m_rankOpera.cfg)
            {
                for (int i = 0; i < m_filterGroup.childCount; ++i)
                {
                    if (ReferenceEquals(go, m_filterGroup.GetChild(i).gameObject))
                    {
                        if (i < m_rankOpera.cfg.filterParams.Length)
                            ret = m_rankOpera.cfg.filterParams[i];
                        break;
                    }
                }
            }
            return ret;
        }

        void SelectFilterObjectAction(GameObject go)
        {
            if (null == go && m_filterGroup.childCount > 0)
                go = m_filterGroup.GetChild(0).gameObject;
            if (go != m_selectedFilterGo)
            {
                if (null != m_selectedFilterGo)
                    m_selectedFilterGo.GetComponent<StateRoot>().SetCurrentState(0, false);
                if (null != go)
                    go.GetComponent<StateRoot>().SetCurrentState(1, false);
            }
            m_selectedFilterGo = go;
        }
        void DefaultPlayerRankOnClickFilterOpera(RankOpera opera, GameObject go,
            System.Action<ListViewRankItemData, RankDbPlayerData> rankItemDataModifier = null)
        {
            if (opera != m_rankOpera)
                return;

            this.SelectFilterObjectAction(go);

            int filterVal = this.GetFilterParams(go);
            Config.RankPanel cfg = opera.cfg;
            List<ListViewRankItemData> viewDatas = new List<ListViewRankItemData>();
            long uid = hotApp.my.localPlayer.charid;
            int idx = 0;
            int rankPos = 0;
            if (null != m_rankDatas && null != m_rankDatas.playerRankDatas && null != m_rankDatas.playerRankDatas.datas)
            {
                foreach (var item in m_rankDatas.playerRankDatas.datas)
                {
                    ++rankPos;
                    if (filterVal >= 0)
                    {
                        if (Config.ClientRankFilterType.Prof == cfg.filterType && item.prof != filterVal)
                            continue;
                        if (Config.ClientRankFilterType.EquipSlot == cfg.filterType && item.keyParam != filterVal)
                            continue;
                    }

                    ++idx;
                    ListViewRankItemData viewData = new ListViewRankItemData();
                    viewDatas.Add(viewData);
                    viewData.playerRankData = item;
                    viewData.id = item.uid;
                    viewData.keyParam = item.keyParam;
                    viewData.rankType = m_rankDatas.rankType;
                    viewData.rankOrder = rankPos;
                    viewData.isSelected = (1 == idx);
                    viewData.field0 = idx.ToString();
                    viewData.field1 = item.name;
                    Config.RoleJob rjCfg = Config.RoleJob.Get(item.prof);
                    viewData.field2 = (null != rjCfg ? rjCfg.name : "-");
                    viewData.field3 = string.IsNullOrEmpty(item.guildName) ? "空" : item.guildName;
                    viewData.field4 = item.cmpParam1.ToString();
                    viewData.onClickCb = opera.OnClickRankItem;
                    viewData.isMark = (uid == item.uid);
                    if (null != rankItemDataModifier)
                        rankItemDataModifier(viewData, item);
                }
            }
            m_rankItems.DataSource = new ObservableList<ListViewRankItemData>(viewDatas);
        }
        void DefaultPlayerRankOnClickRankItemOpera(RankOpera opera, ListViewRankItemData data)
        {
            if (opera != m_rankOpera || null == data || null == data.playerRankData)
                return;

            RankDbPlayerData rankData = data.playerRankData;
            if (hotApp.my.localPlayer.charid == rankData.uid)
                return;

            UIRoleOperationData obj = new UIRoleOperationData(rankData.uid, RoleOperShowType.Rank);
            obj.panelPos = Vector3.zero;
            obj.playerId = rankData.uid;
            obj.jobSex = rankData.sex;
            obj.job = rankData.prof;
            obj.level = rankData.level;
            obj.name = rankData.name;
            App.my.uiSystem.ShowPanel(PanelType.UIRoleOperationPanel, obj, true);
        }
        void DefaultPlayerRankHandleQueryRankResult(RankOpera opera, RankQueryRankResult data)
        {
            if (opera != m_rankOpera)
                return;

            m_rankDatas = data;
            int rankOrder = 0;
            long rankAbility = 0;
            {
                long uid = hotApp.my.localPlayer.charid;
                int rankPos = 0;
                if (null != m_rankDatas.playerRankDatas && null != m_rankDatas.playerRankDatas.datas)
                {
                    foreach (var item in m_rankDatas.playerRankDatas.datas)
                    {
                        ++rankPos;
                        if (uid == item.uid)
                        {
                            rankOrder = rankPos;
                            rankAbility = item.cmpParam1;
                            break;
                        }
                    }
                }
            }
            m_rankOrderVal.text = (0 == rankOrder) ? "-" : rankOrder.ToString();
            m_rankAbilityVal.text = (0 == rankOrder) ? "-" : rankAbility.ToString();
            opera.OnClickFilter(m_selectedFilterGo);
        }
        void QueryRankExtraData(RankOpera opera, ListViewRankItemData data)
        {
            if (null == opera || null == data)
                return;

            hotApp.my.GetModule<HotRankModule>().QueryRankDetail((RankType)data.rankType, data.id, data.keyParam,
                (RankQueryRankDetailResult detailData) => { opera.HandleQueryRankDetail(detailData); });
        }
        void DefaultPlayerRankQueryExtraData(RankOpera opera, ListViewRankItemData data)
        {
            if (opera != m_rankOpera || null == data || null == data.playerRankData)
                return;

            RankDbPlayerData rankData = data.playerRankData;
            if (hotApp.my.localPlayer.charid == rankData.uid)
                return;
            this.QueryRankExtraData(opera, data);
        }
        void PetRankHandleQueryRankDetail(RankOpera opera, RankQueryRankDetailResult data)
        {
            if (null == opera || null == data || null == data.playerRankData || null == data.playerRankData.extraBytes)
                return;

            PetsAttribute extraData = new PetsAttribute();
            extraData.MergeFrom(Network.BitStream.CreateReader(data.playerRankData.extraBytes.buffer));
            UICommon.ShowPetTips(extraData);
        }
        void WeaponRankHandleQueryRankDetail(RankOpera opera, RankQueryRankDetailResult data)
        {
            if (null == opera || null == data || null == data.playerRankData || null == data.playerRankData.extraBytes)
                return;

            ItemData extraData = new ItemData();
            extraData.MergeFrom(Network.BitStream.CreateReader(data.playerRankData.extraBytes.buffer));
            xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
            tipsData.type = xys.UI.InitItemTipsData.Type.CommonTips;
            tipsData.itemData = new NetProto.ItemGrid() { count = 1, data = extraData };
            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
        }

        void PetsRankDataItemModifier(ListViewRankItemData item, RankDbPlayerData data)
        {
            if (null == item || null == data)
                return;
            item.field2 = data.extralong.ToString();
            item.field3 = data.extraStr;
        }
        void WeaponRankDataItemModifier(ListViewRankItemData item, RankDbPlayerData data)
        {
            if (null == item || null == data)
                return;
            Config.EquipPrototype cfg = Config.EquipPrototype.Get((int)data.extralong);
            string equipName = string.Empty;
            if (null != cfg)
                equipName = cfg.name;
            item.field2 = equipName;
            item.field3 = data.extraStr;
        }
        void ShituRankDataItemModifier(ListViewRankItemData item, RankDbPlayerData data)
        {
            if (null == item || null == data)
                return;
            item.field3 = data.level.ToString();
        }
        void DefaultGuildRankOnClickFilterOpera(RankOpera opera, GameObject go,
            System.Action<ListViewRankItemData, RankDbGuildData> rankItemDataModifier = null)
        {
            if (opera != m_rankOpera)
                return;

            Config.RankPanel cfg = opera.cfg;
            List<ListViewRankItemData> viewDatas = new List<ListViewRankItemData>();
            long clanId = hotApp.my.localPlayer.clanResponeIdValue;
            int idx = 0;
            int rankPos = 0;
            if (null != m_rankDatas && null != m_rankDatas.guildRankDatas && null != m_rankDatas.guildRankDatas.datas)
            {
                foreach (var item in m_rankDatas.guildRankDatas.datas)
                {
                    ++idx;
                    ListViewRankItemData viewData = new ListViewRankItemData();
                    viewDatas.Add(viewData);
                    viewData.guildRankData = item;
                    viewData.id = item.id;
                    viewData.keyParam = 0;
                    viewData.rankType = m_rankDatas.rankType;
                    viewData.rankOrder = rankPos;
                    viewData.isSelected = (1 == idx);
                    viewData.field0 = idx.ToString();
                    viewData.field1 = item.name;
                    viewData.field2 = item.level.ToString();
                    viewData.field3 = item.ownerName;
                    viewData.field4 = item.cmpParam1.ToString();
                    viewData.onClickCb = opera.OnClickRankItem;
                    viewData.isMark = (clanId == item.id);
                    if (null != rankItemDataModifier)
                        rankItemDataModifier(viewData, item);
                }
            }
            m_rankItems.DataSource = new ObservableList<ListViewRankItemData>(viewDatas);
        }
        void DefaultGuildRankHandleQueryRankResult(RankOpera opera, RankQueryRankResult data)
        {
            if (opera != m_rankOpera)
                return;

            m_rankDatas = data;
            int rankOrder = 0;
            long rankAbility = 0;
            {
                long clanId = hotApp.my.localPlayer.clanResponeIdValue;
                int rankPos = 0;
                if (null != m_rankDatas.guildRankDatas && null != m_rankDatas.guildRankDatas.datas)
                {
                    foreach (var item in m_rankDatas.guildRankDatas.datas)
                    {
                        ++rankPos;
                        if (clanId == item.id)
                        {
                            rankOrder = rankPos;
                            rankAbility = item.cmpParam1;
                            break;
                        }
                    }
                }
            }
            m_rankOrderVal.text = (0 == rankOrder) ? "-" : rankOrder.ToString();
            m_rankAbilityVal.text = (0 == rankOrder) ? "-" : rankAbility.ToString();
            opera.OnClickFilter(null);
        }
    }
}
#endif

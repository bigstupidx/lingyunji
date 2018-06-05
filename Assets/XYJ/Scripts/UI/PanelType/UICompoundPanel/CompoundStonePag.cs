#if !USE_HOT
namespace xys.hot.UI
{
    using NetProto;
    using NetProto.Hot;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Config;

    [System.Serializable]
    class CompoundStonePage
    {
        [SerializeField]
        Button m_CompoundBtn;               // 合成按钮

        [SerializeField]
        GridLayoutGroup m_GridGroup;        // 格子列表容器

        [SerializeField]
        Button m_GridItem;                  // 格子

        [SerializeField]
        GameObject m_TargetItem;            // 目标合成物品

        [SerializeField]
        Text m_TargetNameText;                  // 目标道具名字

        [SerializeField]
        GameObject m_Meterial;              // 合成材料

        [SerializeField]
        Text m_RandomText;                  // 显示合成概率

        xys.UI.UIHotPanel m_Panel;

        xys.UI.Dialog.TwoBtn m_Screen;

        Button m_SelectedBtn;

        int m_SelectedId;

        Dictionary<int, int> m_GridDatas;

        PackageMgr packageMgr;

        C2APackageRequest request_;
        C2APackageRequest request
        {
            get
            {
                if (request_ == null)
                    request_ = hotApp.my.GetModule<HotPackageModule>().request;

                return request_;
            }
        }

        public void OnInit(xys.UI.UIHotPanel panel)
        {
            m_Panel = panel;
            m_SelectedBtn = null;
            m_SelectedId = 0;
            m_GridDatas = new Dictionary<int, int>();
            packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;

            RegistButton();
        }

        public void OnShow(int itemId)
        {
            m_SelectedId = itemId;

            RegistEvent();
            LoadGridItems(itemId);
            ArrangeGrid();
        }

        public void OnHide()
        {
            m_GridGroup.transform.DestroyChildren();
            m_SelectedId = 0;
            m_GridDatas.Clear();
        }

        public void RegistEvent()
        {
            m_Panel.Event.Subscribe(EventID.Package_UpdatePackage, UpdatePanel);
        }

        public void RegistButton()
        {
            // 合成按钮
            m_CompoundBtn.onClick.AddListener(() =>
            {
                if (!CanCompound(m_SelectedId))
                {
                    Config.TipsContent tipsConfig = Config.TipsContent.Get(3133);
                    if (tipsConfig == null)
                        return;
                    xys.UI.SystemHintMgr.ShowHint(tipsConfig.des);
                    return;
                }

                if (m_Screen != null)
                    return;

                Config.TipsContent config = Config.TipsContent.Get(3132);
                if (config == null)
                    return;

                if (isHadBindMaterial(m_SelectedId))
                {
                    m_Screen = xys.UI.Dialog.TwoBtn.Show(
                           "",
                           string.Format(GlobalSymbol.ToUT(config.des)),
                           "取消", () => false,
                           "确定", () =>
                           {
                               CompoundStart();

                               return false;
                           }, true, true, () => m_Screen = null);
                }
                else
                {
                    CompoundStart();
                }
            });

            // tips
            m_Meterial.GetComponent<Button>().onClick.AddListener(() =>
            {
                xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
                tipsData.type = xys.UI.InitItemTipsData.Type.Compound;
                ItemGrid grid = new ItemGrid();
                ItemData data = new ItemData();
                data.id = m_SelectedId;
                grid.data = data;
                tipsData.itemData = grid;

                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
            });

            // 目标道具
            m_TargetItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                Item config = Item.Get(m_SelectedId);
                if (config == null)
                    return;
                ItemCompound compoundCfg = ItemCompound.Get(config.comId);
                if (compoundCfg == null)
                    return;
                xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
                tipsData.type = xys.UI.InitItemTipsData.Type.Compound;
                ItemGrid grid = new ItemGrid();
                ItemData data = new ItemData();
                data.id = compoundCfg.targetId;
                grid.data = data;
                tipsData.itemData = grid;

                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
            });
        }

        // 加载格子列表
        void LoadGridItems(int selectId)
        {
            List<NetProto.ItemGrid> items = new List<NetProto.ItemGrid>();
            packageMgr.package.ForEach((Grid g) =>
            {
                if (g.data != null)
                {
                    ItemBase config = ItemBaseAll.Get(g.data.data.id);
                    Item itemConfig = Item.Get(g.data.data.id);
                    if (config != null &&
                        itemConfig != null &&
                        config.sonType == (int)ItemChildType.equipStrengthenItem &&
                        itemConfig.comId != 0)
                        items.Add(g.data);
                }
            });

            bool isSelected = false;
            foreach (var v in items)
            {
                if (IsBindItem(v.data.id))
                    continue;

                Button obj = GameObject.Instantiate(m_GridItem);
                CreateGridItem(obj, ItemBaseAll.Get(v.data.id));

                // 默认选中同类
                if (!isSelected)
                {
                    Item config = Item.Get(selectId);
                    if (v.data.id == selectId ||
                        (config != null && config.bindId != 0 && v.data.id == config.bindId) ||
                        (config != null && config.unbindId != 0 && v.data.id == config.unbindId))
                    {
                        SelectItem(obj, v.data.id);
                        isSelected = true;
                    }
                }
            }
        }

        // 创建格子
        void CreateGridItem(Button obj, ItemBase config)
        {
            obj.transform.SetParent(m_GridGroup.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.gameObject.SetActive(true);

            // icon
            Image iconImage = obj.transform.Find("Icon").GetComponent<Image>();
            xys.UI.Helper.SetSprite(iconImage, config.icon);

            // 品质
            Image qualityImage = obj.transform.Find("Quality").GetComponent<Image>();
            QualitySourceConfig qualityConfig = QualitySourceConfig.Get(config.quality);
            xys.UI.Helper.SetSprite(qualityImage, qualityConfig.icon);

            // 名字(只显示对应非绑定道具的名字)
            ItemBase unBindCfg = GetUnBindeItem(config.id);
            Text name = obj.transform.Find("Text").GetComponent<Text>();
            name.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", qualityConfig.colorname, unBindCfg.name));

            // 数量
            int hadCount = packageMgr.GetItemCount(config.id);
            Text text = obj.transform.Find("num").GetComponent<Text>();
            text.text = GlobalSymbol.ToUT(hadCount.ToString());

            // 缓存道具id到组件上
            Text id = obj.transform.Find("Id").GetComponent<Text>();
            id.text = unBindCfg.id.ToString();

            // 点击事件
            obj.onClick.AddListener(() =>
            {
                SelectItem(obj, config.id);
            });
        }

        // 选中格子
        void SelectItem(Button obj, int itemId)
        {
            if (m_SelectedBtn != null)
                m_SelectedBtn.transform.Find("choose").GetComponent<Image>().gameObject.SetActive(false);

            obj.transform.Find("choose").GetComponent<Image>().gameObject.SetActive(true);
            m_SelectedBtn = obj;
            m_SelectedId = itemId;

            SetPanelInfo(itemId);
        }

        // 设置面板信息
        void SetPanelInfo(int itemId)
        {
            ItemBase config = ItemBaseAll.Get(itemId);
            if (config == null)
                return;
            Item itemConfig = Item.Get(itemId);
            if (itemConfig == null)
                return;
            ItemCompound compoundConfig = ItemCompound.Get(itemConfig.comId);
            if (compoundConfig == null)
                return;

            SetMaterialItem(config);
            SetTargetItem(ItemBaseAll.Get(compoundConfig.targetId));

            string str = "100%";
            if (compoundConfig.random != 0)
                str = compoundConfig.random + "%";
            m_RandomText.text = str;
        }

        // 设置材料
        void SetMaterialItem(ItemBase config)
        {
            if (config == null)
                return;

            // icon
            Image iconImage = m_Meterial.transform.Find("Icon").GetComponent<Image>();
            xys.UI.Helper.SetSprite(iconImage, config.icon);

            // 品质
            Image qualityImage = m_Meterial.transform.Find("Quality").GetComponent<Image>();
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(config.quality);
            xys.UI.Helper.SetSprite(qualityImage, qualitConfig.icon);

            // 数量
            ItemCompound compoundConfig = ItemCompound.Get(Item.Get(config.id).comId);
            if (compoundConfig != null)
            {
                int hadCount = packageMgr.GetItemCount(config.id);
                Text text = m_Meterial.transform.Find("ShuZiText").GetComponent<Text>();
                if (hadCount < compoundConfig.count_1)
                    text.text = GlobalSymbol.ToUT(string.Format("#[R]{0}/{1}#n", hadCount, compoundConfig.count_1));
                else
                    text.text = GlobalSymbol.ToUT(string.Format("#[G2]{0}/{1}#n", hadCount, compoundConfig.count_1));
            }
        }

        // 设置合成物品
        void SetTargetItem(ItemBase config)
        {
            // icon
            Image iconImage = m_TargetItem.transform.Find("Icon").GetComponent<Image>();
            xys.UI.Helper.SetSprite(iconImage, config.icon);

            // 品质
            Image qualityImage = m_TargetItem.transform.Find("Quality").GetComponent<Image>();
            QualitySourceConfig qualityConfig = QualitySourceConfig.Get(config.quality);
            xys.UI.Helper.SetSprite(qualityImage, qualityConfig.icon);

            // 名字
            m_TargetNameText.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", qualityConfig.colorname, config.name));
        }

        // 更新面板
        void UpdatePanel()
        {
            // 如果有新的创建新的
            LoadGridItems(m_SelectedId);
            // 更新旧格子信息
            UpdateGridGroup();
            // 更新消耗信息
            UpdateMaterialNum();
            // 排序
            ArrangeGrid();
        }

        // 更新材料数量
        void UpdateMaterialNum()
        {
            ItemBase config = ItemBaseAll.Get(m_SelectedId);
            if (config == null)
                return;
            SetMaterialItem(config);
        }

        // 更新格子列表
        void UpdateGridGroup()
        {
            Button[] objs = m_GridGroup.transform.GetComponentsInChildren<Button>();
            foreach (var v in objs)
            {
                Text num = v.transform.Find("num").GetComponent<Text>();
                Text id = v.transform.Find("Id").GetComponent<Text>();
                int hadCount = packageMgr.GetItemCount(int.Parse(id.text));
                if (hadCount <= 0)
                {
                    GameObject.Destroy(v.gameObject);
                    m_GridDatas.Remove(int.Parse(id.text));
                    continue;
                }
                num.text = GlobalSymbol.ToUT(hadCount.ToString());
            }
        }

        void CompoundStart()
        {
            Item config = Item.Get(m_SelectedId);
            if (config == null)
                return;

            CompoundItemReq input = new CompoundItemReq();
            input.id = config.comId;
            input.count = 1;
            request.CompoundItem(input, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                int codeValue = 0;
                if (respone.code == ReturnCode.Package_NotEnoughSpace)
                    codeValue = 3110;
                else if (respone.code == ReturnCode.Material_Error)
                    codeValue = 3133;
                else if (respone.code == ReturnCode.Package_Compound_Fail)
                    codeValue = 3134;
                else if (respone.code == ReturnCode.ReturnCode_OK)
                    codeValue = 3135;
                else if (respone.code == ReturnCode.Package_NotEnoughSpaceToCompound)
                    codeValue = 3110;
                else
                    return;

                Config.TipsContent tipsConfig = Config.TipsContent.Get(codeValue);
                if (tipsConfig == null)
                    return;
                xys.UI.SystemHintMgr.ShowHint(tipsConfig.des);
            });
        }

        // 是否有对应关系的绑定道具
        bool IsBindItem(int itemId)
        {
            if (m_GridDatas.ContainsKey(itemId))
                return true;

            Item config = Item.Get(itemId);
            if (config == null)
                return true;

            if (config.bindId != 0 && m_GridDatas.ContainsKey(config.bindId))
                return true;
            if (config.unbindId != 0 && m_GridDatas.ContainsKey(config.unbindId))
                return true;

            m_GridDatas[itemId] = itemId;

            return false;
        }

        // 格子排序
        void ArrangeGrid()
        {
            List<Button> list = new List<Button>();
            Button[] objs = m_GridGroup.GetComponentsInChildren<Button>();
            foreach (var v in objs)
            {
                list.Add(v);
            }

            list.Sort(Sorting);

            for (int i = 0; i < list.Count; ++i)
            {
                list[i].transform.SetSiblingIndex(i);
            }
        }

        int Sorting(Button a, Button b)
        {
            int x = int.Parse(a.transform.Find("Id").GetComponent<Text>().text);
            int y = int.Parse(b.transform.Find("Id").GetComponent<Text>().text);

            return x.CompareTo(y);
        }

        // 合成材料中是否有绑定材料
        bool isHadBindMaterial(int itemId)
        {
            Item itemConfig = Item.Get(itemId);
            if (itemConfig == null)
                return false;

            ItemCompound config = ItemCompound.Get(itemConfig.comId);
            if (config == null)
                return false;

            if (config.materialId_1 != 0 && isBindItem(config.materialId_1, config.count_1))
                return true;
            if (config.materialId_2 != 0 && isBindItem(config.materialId_2, config.count_2))
                return true;
            if (config.materialId_3 != 0 && isBindItem(config.materialId_3, config.count_3))
                return true;
            if (config.materialId_4 != 0 && isBindItem(config.materialId_4, config.count_4))
                return true;

            return false;
        }

        // 是否是绑定道具
        bool isBindItem(int itemId, int count)
        {
            ItemBase config = ItemBaseAll.Get(itemId);
            if (config == null)
                return false;

            if (config.isBind)
            {
                count = count - packageMgr.GetItemRealCount(itemId);
                if (count <= 0)
                    return false;
                else if (packageMgr.GetItemRealCount(itemId) != 0)
                    return true;
            }

            Item itemConfig = Item.Get(itemId);
            if (itemConfig == null)
                return false;

            if (itemConfig.bindId != 0)
            {
                count = count - packageMgr.GetItemRealCount(itemConfig.bindId);
                if (count <= 0)
                    return false;
                else if (packageMgr.GetItemRealCount(itemConfig.bindId) != 0)
                    return true;
            }

            return false;
        }

        // 获取对应非绑定道具
        ItemBase GetUnBindeItem(int itemId)
        {
            ItemBase config = ItemBaseAll.Get(itemId);
            if (config == null)
                return config;

            if (!config.isBind)
                return config;

            Item itemConfig = Item.Get(itemId);
            if (itemConfig == null)
                return config;

            if (itemConfig.unbindId != 0)
            {
                if (ItemBaseAll.Get(itemConfig.unbindId) != null)
                    return ItemBaseAll.Get(itemConfig.unbindId);
            }
            return config;
        }

        // 是否可以合成
        bool CanCompound(int itemId)
        {
            Item itemConfig = Item.Get(itemId);
            if (itemConfig == null)
                return false;

            ItemCompound config = ItemCompound.Get(itemConfig.comId);
            if (config == null)
                return false;

            // 判断消耗是否足够
            if (config.materialId_1 != 0)
            {
                if (packageMgr.GetItemCount(config.materialId_1) < config.count_1)
                    return false;
            }
            if (config.materialId_2 != 0)
            {
                if (packageMgr.GetItemCount(config.materialId_2) < config.count_2)
                    return false;
            }
            if (config.materialId_3 != 0)
            {
                if (packageMgr.GetItemCount(config.materialId_3) < config.count_3)
                    return false;
            }
            if (config.materialId_4 != 0)
            {
                if (packageMgr.GetItemCount(config.materialId_4) < config.count_4)
                    return false;
            }

            return true;
        }
    }
}
#endif
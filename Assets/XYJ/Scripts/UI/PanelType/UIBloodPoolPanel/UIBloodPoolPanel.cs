#if !USE_HOT
namespace xys.hot.UI
{
    using NetProto;
    using NetProto.Hot;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Config;

    class UIBloodPoolPanel : HotPanelBase
    {
        [SerializeField]
        Button m_CloseBtn;

        [SerializeField]
        Text m_LeftHp;              // 生命值文本

        [SerializeField]
        Text m_RightHpRatio;        // 宠物血条百分比

        [SerializeField]
        Image m_LeftHpImage;        // 生命条

        [SerializeField]
        Image m_RightHpImage;       // 宠物生命条

        [SerializeField]
        Slider m_SliderBar;            // 宠物血条滑块

        [SerializeField]
        GridLayoutGroup m_LeftGridGroup;    // 生命道具容器

        [SerializeField]
        GridLayoutGroup m_RightGridGroup;   // 宠物道具容器

        [SerializeField]
        Button m_LeftGrid;          // 生命道具格子

        [SerializeField]
        Button m_RightGrid;         // 宠物道具格子

        [SerializeField]
        Button m_WearPetItem;       // 装备上的宠物道具

        PackageMgr packageMgr;

        int m_SelectAutoRestoreValue;

        float DefineRate = 0.3f;

        Material m_grayMat;

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

        public UIBloodPoolPanel() :base(null){ }
        public UIBloodPoolPanel(xys.UI.UIHotPanel panel) : base(panel)
        {
            m_SelectAutoRestoreValue = 0;
        }

        protected override void OnInit()
        {
            packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            m_grayMat = Resources.Load("UIGray") as Material;

            LoadLeftGridGroup();
            LoadRightGridGroup();

            RegistButton();
        }

        protected override void OnShow(object args)
        {
            SetLeftPanelInfo();
            SetRightPanelInfo(App.my.localPlayer.autoRestoreRateValue/100f);
            SetWearItemGridInfo(App.my.localPlayer.petBloodBottleValue);
            UpdatePanel();

            RegistEvent();
        }

        protected override void OnHide()
        {
        }

        void RegistEvent()
        {
            Event.Subscribe(AttType.AT_BloodPool, SetLeftPanelInfo);
            Event.Subscribe(EventID.Package_CountChange, UpdatePanel);
        }

        void RegistButton()
        {
            m_CloseBtn.onClick.AddListener(() =>
            {
                App.my.uiSystem.HidePanel(xys.UI.PanelType.UIBloodPoolPanel);
            });

            m_SliderBar.onValueChanged.AddListener((value) =>
            {
                SetRightPanelInfo(value);
            });

            m_SliderBar.GetComponent<xys.UI.ButtonPress>().onRelease.AddListener(() =>
            {
                Int32 input = new Int32();
                input.value = m_SelectAutoRestoreValue;
                request.ChangeAutoRestoreRate(input, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                        return;
                });
            });

            m_WearPetItem.onClick.AddListener(() =>
            {
                if (ItemBaseAll.Get(App.my.localPlayer.petBloodBottleValue) == null)
                    return;

                xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
                tipsData.type = xys.UI.InitItemTipsData.Type.Compound;
                ItemGrid grid = new ItemGrid();
                ItemData data = new ItemData();
                data.id = App.my.localPlayer.petBloodBottleValue;
                grid.data = data;
                tipsData.itemData = grid;

                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
            });
        }

        // 加载生命界面格子列表
        void LoadLeftGridGroup()
        {
            string[] tempStr = kvCommon.Get("BloodPoolDrug").value.Split('|');
            foreach (var v in tempStr)
            {
                ItemBase itemConfig = ItemBaseAll.Get(int.Parse(v));
                if (itemConfig != null)
                    CreateGrid(GameObject.Instantiate(m_LeftGrid), m_LeftGridGroup, itemConfig);
            }
        }

        // 加载宠物面板格子列表
        void LoadRightGridGroup()
        {
            string[] tempStr = kvCommon.Get("PetBloodDrug").value.Split('|');
            foreach (var v in tempStr)
            {
                ItemBase itemConfig = ItemBaseAll.Get(int.Parse(v));
                if (itemConfig != null)
                    CreateGrid(GameObject.Instantiate(m_RightGrid), m_RightGridGroup, itemConfig);
            }
        }

        // 创建格子
        void CreateGrid(Button obj, GridLayoutGroup gridGroup, ItemBase config)
        {
            Text name = obj.transform.Find("Name").GetComponent<Text>();
            Text Level = obj.transform.Find("Level").GetComponent<Text>();
            Image iconImage = obj.transform.Find("Equip/Icon").GetComponent<Image>();
            Image qualityImage = obj.transform.Find("Equip/Quality").GetComponent<Image>();
            Text num = obj.transform.Find("Equip/Text").GetComponent<Text>();
            Text id = obj.transform.Find("Equip/Id").GetComponent<Text>();
            Button iconTips = obj.transform.Find("Equip").GetComponent<Button>();
            Button useBtn = obj.transform.Find("ShiYongBtn").GetComponent<Button>();

            obj.transform.SetParent(gridGroup.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.gameObject.SetActive(true);

            // 名字
            name.text = config.name;
            // 增加值
            Level.text = "能量+" + Item.Get(config.id).addHp.ToString();
            // icon
            xys.UI.Helper.SetSprite(iconImage, config.icon);
            // 品质
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(config.quality);
            xys.UI.Helper.SetSprite(qualityImage, qualitConfig.icon);
            // 数量
            int hadCount = packageMgr.GetItemCount(config.id);
            if (hadCount == 0)
                num.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", hadCount));
            else
                num.text = hadCount.ToString();
            // id
            id.text = config.id.ToString();

            // tips
            iconTips.onClick.AddListener(() =>
            {
                xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
                tipsData.type = xys.UI.InitItemTipsData.Type.Compound;
                ItemGrid grid = new ItemGrid();
                ItemData data = new ItemData();
                data.id = config.id;
                grid.data = data;
                tipsData.itemData = grid;

                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
            });

            // 使用按钮
            useBtn.onClick.AddListener(() =>
            {
                if (config.sonType == (int)ItemChildType.petDrug)
                {
                    SetWearItemGridInfo(config.id);
                    SetRightPanelInfo(DefineRate);

                    if (App.my.localPlayer.petBloodBottleValue != config.id)
                    {
                        Int32 input = new Int32();
                        input.value = config.id;
                        request.WearPetBloodBottle(input, (error, respone) =>
                        {
                            if (error != wProtobuf.RPC.Error.Success)
                                return;
                        });
                    }

                    if (App.my.localPlayer.autoRestoreRateValue != DefineRate)
                    {
                        Int32 input = new Int32();
                        input.value = (int)(DefineRate * 100);
                        request.ChangeAutoRestoreRate(input, (error, respone) =>
                        {
                            if (error != wProtobuf.RPC.Error.Success)
                                return;
                        });
                    }
                }
                else
                {
                    int count = packageMgr.GetItemCount(config.id);
                    if (count > 0)
                        packageMgr.UseItemById(config.id, 1);
                }
            });
        }

        void UpdatePanel()
        {
            UpdateItemCount(m_LeftGridGroup);
            UpdateItemCount(m_RightGridGroup);
        }

        // 更新面板物品数量
        void UpdateItemCount(GridLayoutGroup gridGroup)
        {
            for(int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform obj = gridGroup.transform.GetChild(i);
                Text num = obj.transform.Find("Equip/Text").GetComponent<Text>();
                Text idText = obj.transform.Find("Equip/Id").GetComponent<Text>();
                Button useBtn = obj.transform.Find("ShiYongBtn").GetComponent<Button>();

                int id = int.Parse(idText.text);
                int hadCount = packageMgr.GetItemCount(id);
                if (hadCount == 0)
                    num.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", hadCount));
                else
                    num.text = hadCount.ToString();

                if (hadCount > 0)
                {
                    useBtn.gameObject.SetActive(true);
                    Graphic[] graphic = obj.transform.GetComponentsInChildren<Graphic>();
                    foreach (var v in graphic)
                        v.material = null;
                }
                else
                {
                    useBtn.gameObject.SetActive(false);
                    Graphic[] graphic = obj.transform.GetComponentsInChildren<Graphic>();
                    foreach (var v in graphic)
                        v.material = m_grayMat;
                }
            }
        }

        // 设置生命界面信息
        void SetLeftPanelInfo()
        {
            // 血量
            kvCommon kvConfig = kvCommon.Get("BloodPoolPerformanceLimit");
            if (kvConfig == null)
                return;

            m_LeftHp.text = App.my.localPlayer.bloodPoolValue + "/" + kvConfig.value;
            float ratio = App.my.localPlayer.bloodPoolValue / float.Parse(kvConfig.value);
            m_LeftHpImage.fillAmount = ratio;
        }

        // 设置宠物界面信息
        void SetRightPanelInfo(float ratio)
        {
            m_RightHpRatio.text = ((int)(ratio*100)).ToString() + "%";
            m_RightHpImage.fillAmount = ratio;
            m_SliderBar.value = ratio;
            m_SelectAutoRestoreValue = (int)(ratio*100);
        }

        // 设置装备道具格子信息
        void SetWearItemGridInfo(int itemId)
        {
            ItemBase config = ItemBaseAll.Get(itemId);
            if (config == null)
                return;

            Image icon = m_WearPetItem.transform.Find("Icon").GetComponent<Image>();
            Image quality = m_WearPetItem.transform.Find("Quality").GetComponent<Image>();
            Text num = m_WearPetItem.transform.Find("Text").GetComponent<Text>();

            // icon
            xys.UI.Helper.SetSprite(icon, config.icon);
            // 品质
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(config.quality);
            xys.UI.Helper.SetSprite(quality, qualitConfig.icon);
            // 数量
            int hadCount = packageMgr.GetItemCount(itemId);
            if (hadCount == 0)
                num.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", hadCount));
            else
                num.text = hadCount.ToString();
        }
    }
}
#endif
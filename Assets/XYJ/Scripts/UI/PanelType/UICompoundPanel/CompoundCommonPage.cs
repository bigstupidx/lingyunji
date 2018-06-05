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
    class CompoundCommonPage
    {
        [SerializeField]
        Button m_CompoundBtn;                   // 合成按钮

        [SerializeField]
        GridLayoutGroup m_GridGroup;            // 材料显示容器

        [SerializeField]
        GameObject m_TargetGrid;                // 目标道具

        [SerializeField]
        Button m_MeterialGrid;                  // 材料道具

        [SerializeField]
        Button m_PlusBtn;                       // 加号按钮

        [SerializeField]
        Button m_LessBtn;                       // 减号按钮

        [SerializeField]
        Slider m_SliderBar;                     // 滑块

        [SerializeField]
        Image m_SliderBlack;                    // 滑块条

        [SerializeField]
        Text m_CountText;                       // 数量

        [SerializeField]
        Text m_CountText2;                      // 数量

        Dictionary<int, Text> m_Materials;      // 材料数量 
        Dictionary<int, int> m_MaterialsCount;  // 材料数量           

        int m_SelectedId;

        int m_SelectedCount;

        int SliderBlackHeight;

        int SliderBlackWidth;

        xys.UI.Dialog.TwoBtn m_Screen;

        xys.UI.UIHotPanel m_Panel;

        PackageMgr packageMgr;

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

        public void OnInit(xys.UI.UIHotPanel panel)
        {
            m_Panel = panel;
            m_SelectedId = 0;
            m_SelectedCount = 0;
            SliderBlackWidth = 397;
            SliderBlackHeight = 25;

            m_Materials = new Dictionary<int, Text>();
            m_MaterialsCount = new Dictionary<int, int>();
            m_grayMat = Resources.Load("UIGray") as Material;

            packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;

            RegistButton();
        }

        public void OnShow(int itemId)
        {
            m_SelectedId = itemId;

            RegistEvent();
            LoadMaterialGrid(itemId);
            UpdateSliderProgress();
        }

        public void OnHide()
        {
            m_GridGroup.transform.DestroyChildren();
            m_Materials.Clear();
            m_MaterialsCount.Clear();
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

            // +
            m_PlusBtn.onClick.AddListener(() =>
            {
                m_SelectedCount += 1;
                if (m_SelectedCount > m_SliderBar.maxValue)
                    m_SelectedCount = (int)m_SliderBar.maxValue;

                CalcSliderProgress();
                SetCompoundBtnState();
            });

            // -
            m_LessBtn.onClick.AddListener(() =>
            {
                m_SelectedCount -= 1;
                if (m_SelectedCount < 0)
                    m_SelectedCount = 0;

                CalcSliderProgress();
                SetCompoundBtnState();
            });

            // 滑块
            m_SliderBar.onValueChanged.AddListener((value) =>
            {

                m_SliderBlack.rectTransform.sizeDelta = new Vector2(SliderBlackWidth * (value / m_SliderBar.maxValue), SliderBlackHeight);
                m_SelectedCount = (int)value;
                m_CountText.text = m_SelectedCount.ToString();
                m_CountText2.text = m_SelectedCount.ToString();

                SetCompoundBtnState();
            });

            // 目标道具
            m_TargetGrid.GetComponent<Button>().onClick.AddListener(() =>
            {
                ItemCompound config = ItemCompound.Get(m_SelectedId);
                if (config == null)
                    return;

                xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
                tipsData.type = xys.UI.InitItemTipsData.Type.Compound;
                ItemGrid grid = new ItemGrid();
                ItemData data = new ItemData();
                data.id = config.targetId;
                grid.data = data;
                tipsData.itemData = grid;

                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
            });
        }

        // 加载材料
        void LoadMaterialGrid(int itemId)
        {
            ItemCompound config = ItemCompound.Get(itemId);
            if (config == null)
                return;
            ItemBase itemConfig = ItemBaseAll.Get(config.targetId);
            if (itemConfig == null)
                return;

            // icon
            Image iconImage = m_TargetGrid.transform.Find("Icon").GetComponent<Image>();
            xys.UI.Helper.SetSprite(iconImage, itemConfig.icon);

            // 品质
            Image qualityImage = m_TargetGrid.transform.Find("Quality").GetComponent<Image>();
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(itemConfig.quality);
            xys.UI.Helper.SetSprite(qualityImage, qualitConfig.icon);

            if (config.materialId_1 != 0)
                CreateMaterialGrid(config.materialId_1, config.count_1);
            if (config.materialId_2 != 0)
                CreateMaterialGrid(config.materialId_2, config.count_1);
            if (config.materialId_3 != 0)
                CreateMaterialGrid(config.materialId_3, config.count_1);
            if (config.materialId_4 != 0)
                CreateMaterialGrid(config.materialId_4, config.count_1);

        }

        // 创建材料
        void CreateMaterialGrid(int itemId, int count)
        {
            ItemBase config = ItemBaseAll.Get(itemId);
            if (config == null)
                return;
            ItemCompound compoundConfig = ItemCompound.Get(itemId);

            Button obj = GameObject.Instantiate(m_MeterialGrid);
            obj.transform.SetParent(m_GridGroup.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.gameObject.SetActive(true);

            // icon
            Image iconImage = obj.transform.Find("Icon").GetComponent<Image>();
            xys.UI.Helper.SetSprite(iconImage, config.icon);

            // 品质
            Image qualityImage = obj.transform.Find("Quality").GetComponent<Image>();
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(config.quality);
            xys.UI.Helper.SetSprite(qualityImage, qualitConfig.icon);

            // 数量
            int hadCount = packageMgr.GetItemCount(itemId);
            Text text = obj.transform.Find("ShuZiText").GetComponent<Text>();
            if (hadCount < count)
                text.text = GlobalSymbol.ToUT(string.Format("#[R]{0}/{1}#n", hadCount, count));
            else
                text.text = GlobalSymbol.ToUT(string.Format("#[G2]{0}/{1}#n", hadCount, count));

            // 记录数量，用于道具数量变化界面刷新
            m_Materials[itemId] = text;
            m_MaterialsCount[itemId] = count;

            // 点击事件
            obj.onClick.AddListener(() =>
            {
                xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
                tipsData.type = xys.UI.InitItemTipsData.Type.Compound;
                ItemGrid grid = new ItemGrid();
                ItemData data = new ItemData();
                data.id = itemId;
                grid.data = data;
                tipsData.itemData = grid;

                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
            });
        }

        void UpdatePanel()
        {
            UpdateMaterailCount();
            UpdateSliderProgress();
        }

        // 更新材料数量
        void UpdateMaterailCount()
        {
            foreach (var v in m_Materials)
            {
                int hadCount = packageMgr.GetItemCount(v.Key);
                if (hadCount < m_MaterialsCount[v.Key])
                    v.Value.text = GlobalSymbol.ToUT(string.Format("#[R]{0}/{1}#n", hadCount, m_MaterialsCount[v.Key]));
                else
                    v.Value.text = GlobalSymbol.ToUT(string.Format("#[G2]{0}/{1}#n", hadCount, m_MaterialsCount[v.Key]));
            }
        }

        // 更新滑块
        void UpdateSliderProgress()
        {
            // 最大值
            ItemCompound config = ItemCompound.Get(m_SelectedId);
            if (config == null)
                return;

            int maxCount = int.MaxValue;

            if (config.materialId_1 != 0)
            {
                int num = packageMgr.GetItemCount(config.materialId_1);
                int count = num / config.count_1;
                if (maxCount > count)
                    maxCount = count;
            }
            if (config.materialId_2 != 0)
            {
                int num = packageMgr.GetItemCount(config.materialId_2);
                int count = num / config.count_2;
                if (maxCount > count)
                    maxCount = count;
            }
            if (config.materialId_3 != 0)
            {
                int num = packageMgr.GetItemCount(config.materialId_3);
                int count = num / config.count_3;
                if (maxCount > count)
                    maxCount = count;
            }
            if (config.materialId_4 != 0)
            {
                int num = packageMgr.GetItemCount(config.materialId_4);
                int count = num / config.count_4;
                if (maxCount > count)
                    maxCount = count;
            }

            if (maxCount == 0)
            {
                m_SelectedCount = 1;
                m_SliderBar.maxValue = 1;
            }
            else
            {
                m_SelectedCount = maxCount;
                m_SliderBar.maxValue = maxCount;
            }

            SetSliderBar(1);

            m_CountText.text = m_SelectedCount.ToString();
            m_CountText2.text = m_SelectedCount.ToString();
        }

        // 计算滑块进度
        void CalcSliderProgress()
        {
            SetSliderBar(m_SelectedCount / m_SliderBar.maxValue);

            m_CountText.text = m_SelectedCount.ToString();
            m_CountText2.text = m_SelectedCount.ToString();
        }

        // 设置滑块
        void SetSliderBar(float value)
        {
            m_SliderBar.normalizedValue = value;
            m_SliderBlack.rectTransform.sizeDelta = new Vector2(SliderBlackWidth * value, SliderBlackHeight);
        }

        // 设置合成按钮状态
        void SetCompoundBtnState()
        {
            if (m_SelectedCount == 0)
            {
                m_CompoundBtn.GetComponent<Image>().material = m_grayMat;
                m_CompoundBtn.enabled = false;
            }
            else
            {
                m_CompoundBtn.GetComponent<Image>().material = null;
                m_CompoundBtn.enabled = true;
            }
        }

        // 合成
        void CompoundStart()
        {
            CompoundItemReq input = new CompoundItemReq();

            input.id = m_SelectedId;
            input.count = m_SelectedCount;

            if (input.count <= 0)
                return;

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

        // 是否有绑定材料
        bool IsBindMaterial()
        {
            ItemCompound compoundConfig = ItemCompound.Get(m_SelectedId);
            if (compoundConfig == null)
                return false;

            if (compoundConfig.materialId_1 != 0)
            {

            }
            return true;
        }

        // 合成材料中是否有绑定材料
        bool isHadBindMaterial(int targetId)
        {
            ItemCompound config = ItemCompound.Get(targetId);
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
    }
}
#endif
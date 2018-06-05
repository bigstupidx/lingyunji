#if !USE_HOT
namespace xys.hot.UI
{
    using NetProto;
    using NetProto.Hot;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Config;

    class UIDecomposePanel : HotPanelBase
    {
        [SerializeField]
        Button m_ClosePanel;

        [SerializeField]
        GridLayoutGroup m_GridLayout;

        [SerializeField]
        Button m_DecomposeBtn;

        xys.UI.UIHotPanel m_Panel;

        xys.UI.Dialog.TwoBtn m_Screen;

        PackageMgr packageMgr;

        Material m_grayMat;

        Dictionary<int, int> m_RareEquips; // 稀有装备

        Dictionary<int, ItemGrid> m_SelectedList;      // 装备选中列表

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

        public UIDecomposePanel() :base(null){ }
        public UIDecomposePanel(xys.UI.UIHotPanel panel) : base(panel)
        {
            m_Panel = panel;
        }

        protected override void OnInit()
        {
            m_SelectedList = new Dictionary<int, ItemGrid>();
            m_RareEquips = new Dictionary<int, int>();
            packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            m_grayMat = Resources.Load("UIGray") as Material;

            LoadToggle();
            RegistButton();
        }

        protected override void OnShow(object args)
        {
            ItemFuncObject obj = args as ItemFuncObject;
            RegistEvent();
            LoadEquipList(obj.GridIndex);
            SetButtonState();
            ArrangeGrid();
        }

        protected override void OnHide()
        {
            Toggle[] objs = parent.transform.Find("Root/AssignPage/DiBu").GetComponentsInChildren<Toggle>();
            foreach(var v in objs)
            {
                v.isOn = false;
            }

            xys.UI.State.StateRoot[] stateRoots = parent.transform.Find("Root/AssignPage/DiBu").GetComponentsInChildren<xys.UI.State.StateRoot>();
            foreach (var v in stateRoots)
            {
                v.CurrentState = 0;
            }

            m_GridLayout.transform.DestroyChildren();
            m_RareEquips.Clear();
            m_SelectedList.Clear();
        }

        void RegistEvent()
        {
            Event.Subscribe(EventID.Package_ReomveChange, UpdateEquipList);
        }

        void RegistButton()
        {
            m_DecomposeBtn.onClick.AddListener(() =>
            {
                if (m_RareEquips.Count > 0)
                {
                    if (m_Screen != null)
                        return;

                    Config.TipsContent config = Config.TipsContent.Get(3123);
                    if (config == null)
                        return;

                    m_Screen = xys.UI.Dialog.TwoBtn.Show(
                        "",
                        string.Format(GlobalSymbol.ToUT(config.des)),
                        "取消", () => false,
                        "确定", () =>
                        {
                            Decompose();

                            return false;
                        }, true, true, () => m_Screen = null);
                }
                else
                {
                    Decompose();
                }
            });

            m_ClosePanel.onClick.AddListener(() =>
            {
                App.my.uiSystem.HidePanel(xys.UI.PanelType.UIDecomposePanel);
            });
        }

        void RegistToggle(Toggle obj, ItemQuality type)
        {
            obj.onValueChanged.AddListener((flag) =>
            {
                xys.UI.State.StateRoot stateRoot = obj.transform.GetComponent<xys.UI.State.StateRoot>();
                if (flag)
                    stateRoot.CurrentState = 1;
                else
                    stateRoot.CurrentState = 0;
                SelectedQuality(type, flag);
                SetButtonState();
            });
        }

        void LoadToggle()
        {
            Toggle obj = m_Panel.transform.Find("Root/AssignPage/DiBu/Bai").GetComponent<Toggle>();
            RegistToggle(obj, ItemQuality.white);

            obj = m_Panel.transform.Find("Root/AssignPage/DiBu/Lv").GetComponent<Toggle>();
            RegistToggle(obj, ItemQuality.green);

            obj = m_Panel.transform.Find("Root/AssignPage/DiBu/Lan").GetComponent<Toggle>();
            RegistToggle(obj, ItemQuality.blue);

            obj = m_Panel.transform.Find("Root/AssignPage/DiBu/Zi").GetComponent<Toggle>();
            RegistToggle(obj, ItemQuality.purple);
        }

        // 加载装备列表
        void LoadEquipList(int index)
        {
            EquipPrototype equipConfig;
            Dictionary<int, ItemGrid> items = new Dictionary<int, ItemGrid>();
            packageMgr.package.ForEach((Grid g) =>
            {
                items[g.pos] = g.data;
            });

            Dictionary<int, ItemGrid> tempEquip = new Dictionary<int, ItemGrid>();
            foreach (var v in items)
            {
                if (v.Value == null)
                    continue;
                equipConfig = EquipPrototype.Get(v.Value.data.id);
                if (equipConfig == null)
                    continue;
                if (equipConfig.decProduc == 0)
                    continue;
                if (equipConfig.quality > ItemQuality.purple)
                    continue;
                tempEquip[v.Key] = v.Value;
            }

            if (tempEquip.Count == 0)
                return;

            Button instance = GameObject.Instantiate(m_Panel.transform.Find("Root/AssignPage/Content/HeChengEquipItem").GetComponent<Button>());

            foreach (var v in tempEquip)
            {
                Button obj = GameObject.Instantiate(instance);
                // 创建格子
                CreateGrid(v.Key, v.Value, obj);
                // 设置格子信息
                SetEquipGridInfo(obj, EquipPrototype.Get(v.Value.data.id), v.Key);
                // 默认选中格子
                if (v.Key == index)
                {
                    Image btnObj = obj.transform.Find("GuoXuanBg").GetComponent<Image>();
                    SelectedEquip(index, v.Value, true);
                    btnObj.gameObject.SetActive(true);
                    SetButtonState();
                }
            }
        }

        // 更新装备列表
        void UpdateEquipList()
        {
            EquipPrototype equipConfig;
            Dictionary<int, ItemGrid> items = new Dictionary<int, ItemGrid>();
            packageMgr.package.ForEach((Grid g) =>
            {
                items[g.pos] = g.data;
            });

            Dictionary<int, ItemGrid> tempEquip = new Dictionary<int, ItemGrid>();
            foreach (var v in items)
            {
                if (v.Value == null)
                    continue;
                equipConfig = EquipPrototype.Get(v.Value.data.id);
                if (equipConfig == null)
                    continue;
                if (equipConfig.decProduc == 0)
                    continue;
                if (equipConfig.quality > ItemQuality.purple)
                    continue;
                tempEquip[v.Key] = v.Value;
            }

            m_GridLayout.transform.DestroyChildren();

            if (tempEquip.Count == 0)
                App.my.uiSystem.HidePanel(xys.UI.PanelType.UIDecomposePanel);

            Button instance = GameObject.Instantiate(m_Panel.transform.Find("Root/AssignPage/Content/HeChengEquipItem").GetComponent<Button>());

            int index = 0;
            foreach (var v in tempEquip)
            {
                Button obj = GameObject.Instantiate(instance);
                CreateGrid(v.Key, v.Value, obj);
                SetEquipGridInfo(obj, EquipPrototype.Get(v.Value.data.id), v.Key);

                index++;
            }

            SetButtonState();

            ArrangeGrid();
        }

        // 创建新格子
        void CreateGrid(int index, ItemGrid data, Button obj)
        {
            obj.transform.SetParent(m_GridLayout.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.gameObject.SetActive(true);

            // 单击事件
            obj.GetComponent<xys.UI.ButtonPress>().onClick.AddListener(() =>
            {
                Image btnObj = obj.transform.Find("GuoXuanBg").GetComponent<Image>();
                SelectedEquip(index, data, !btnObj.gameObject.activeSelf);
                btnObj.gameObject.SetActive(!btnObj.gameObject.activeSelf);
                SetButtonState();
            });

            // 长按事件
            obj.GetComponent<xys.UI.ButtonPress>().onPress.AddListener(() =>
            {
                xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
                tipsData.type = xys.UI.InitItemTipsData.Type.Compound;
                tipsData.itemData = data;
                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
            });
        }

        // 选中品质
        void SelectedQuality(ItemQuality type, bool isOn)
        {
            EquipPrototype equipConfig;
            Button[] coms = m_GridLayout.transform.GetComponentsInChildren<Button>();
            foreach (var v in coms)
            {
                int id = int.Parse(v.transform.Find("Id").GetComponent<Text>().text);
                ItemGrid data = packageMgr.GetItemInfo(id);
                equipConfig = EquipPrototype.Get(data.data.id);
                if (equipConfig.quality == type)
                {
                    SelectedEquip(id, data, isOn);
                    Image btnObj = v.transform.Find("GuoXuanBg").GetComponent<Image>();
                    btnObj.gameObject.SetActive(isOn);
                }
            }
        }

        // 选中装备
        void SelectedEquip(int index, ItemGrid data, bool isOn)
        {
            if (isOn)
            {
                if (!m_SelectedList.ContainsKey(index))
                {
                    m_SelectedList[index] = data;
                    if (EquipPrototype.Get(data.data.id).quality == ItemQuality.purple)
                        m_RareEquips[index] = index;
                }
            }
            else
            {
                m_SelectedList.Remove(index);
                m_RareEquips.Remove(index);
            }
        }

        // 设置按钮状态
        void SetButtonState()
        {
            if (m_SelectedList.Count == 0)
            {
                m_DecomposeBtn.GetComponent<Image>().material = m_grayMat;
                m_DecomposeBtn.enabled = false;
            }
            else
            {
                m_DecomposeBtn.GetComponent<Image>().material = null;
                m_DecomposeBtn.enabled = true;
            }
        }

        // 设置装备格子信息
        void SetEquipGridInfo(Button obj, EquipPrototype config, int index)
        {
            Image QualityLevel = obj.transform.Find("Quality").GetComponent<Image>();
            Image Icon = obj.transform.Find("Icon").GetComponent<Image>();
            Image SelectedImage = obj.transform.Find("GuoXuanBg").GetComponent<Image>();
            Text id = obj.transform.Find("Id").GetComponent<Text>();

            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(config.quality);
            xys.UI.Helper.SetSprite(QualityLevel, qualitConfig.icon);
            QualityLevel.gameObject.SetActive(true);

            xys.UI.Helper.SetSprite(Icon, config.icon);
            Icon.gameObject.SetActive(true);

            SelectedImage.gameObject.SetActive(false);

            id.text = index.ToString();
        }

        // 分解装备操作
        void Decompose()
        {
            DecomposeItemReq input = new DecomposeItemReq();

            if (m_SelectedList.Count == 0)
                return;

            foreach (var v in m_SelectedList)
            {
                input.index.Add(v.Key);
            }

            request.DecomposeItem(input, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                int codeValue = 0;
                if (respone.code == ReturnCode.Package_NotEnoughSpace)
                    codeValue = 3122;
                else if (respone.code == ReturnCode.ReturnCode_OK)
                    codeValue = 3124;
                else
                    return;

                Config.TipsContent config = Config.TipsContent.Get(codeValue);
                if (config == null)
                    return;
                xys.UI.SystemHintMgr.ShowHint(config.des);
            });

            m_SelectedList.Clear();
        }

        // 格子排序
        void ArrangeGrid()
        {
            List<Button> list = new List<Button>();
            Button[] objs = m_GridLayout.GetComponentsInChildren<Button>();
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
            ItemGrid data1 = packageMgr.GetItemInfo(x);
            ItemGrid data2 = packageMgr.GetItemInfo(y);

            return data1.data.id.CompareTo(data2.data.id);
        }
    }
}
#endif
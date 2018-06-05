#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Config;
    using xys.UI;
    using NetProto;

    class BloodTipsPanel : HotPanelBase
    {
        GridLayoutGroup m_GridGroup;

        Button m_Grid;

        PackageMgr packageMgr;

        BloodTipsPanel() : base(null) { }

        BloodTipsPanel(UIHotPanel parent) : base(parent) { }

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

        protected override void OnInit()
        {
            m_GridGroup = parent.transform.Find("Content/ViewPoint/GridGroup").GetComponent<GridLayoutGroup>();
            m_Grid = parent.transform.Find("Content/Grid").GetComponent<Button>();
            packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
        }

        protected override void OnShow(object args)
        {
            LoadGridList();
        }

        protected override void OnHide()
        {
            base.OnHide();
            m_GridGroup.transform.DestroyChildren();
        }

        // 加载格子列表
        void LoadGridList()
        {
//             List<NetProto.ItemGrid> items = new List<NetProto.ItemGrid>();
//             packageMgr.package.ForEach((Grid g) =>
//             {
//                 if (g.data != null)
//                 {
//                     ItemBase config = ItemBaseAll.Get(g.data.data.id);
//                     Item itemConfig = Item.Get(g.data.data.id);
//                     if (config != null &&
//                         itemConfig != null &&
//                         config.sonType == (int)ItemChildType.cure &&
// //                        itemConfig.isCanUseCondition())
//                      config.job.Has(xys.App.my.localPlayer.job) &&
//                      xys.App.my.localPlayer.levelValue >= itemConfig.useLevel)
//                         items.Add(g.data);
//                 }
//             });
// 
//             foreach (var v in items)
//             {
//                 CreateGrid(GameObject.Instantiate(m_Grid), v);
//             }

            kvCommon kvConfig = kvCommon.Get("BloodDrug");
            if (kvConfig == null)
                return;

            string[] strs = kvConfig.value.Split('|');
            foreach (var v in strs)
            {
                CreateGrid(GameObject.Instantiate(m_Grid), int.Parse(v));
            }
        }

        // 创建格子
        void CreateGrid(Button obj, int itemId)
        {
            ItemBase config = ItemBaseAll.Get(itemId);
            Item itemConfig = Item.Get(itemId);

            obj.transform.SetParent(m_GridGroup.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.gameObject.SetActive(true);

            Text name = obj.transform.Find("Name").GetComponent<Text>();
            Text Level = obj.transform.Find("Level").GetComponent<Text>();
            Image quality = obj.transform.Find("Equip/Quality").GetComponent<Image>();
            Image icon = obj.transform.Find("Equip/Icon").GetComponent<Image>();
            Text num = obj.transform.Find("Equip/Text").GetComponent<Text>();

            // 名字
            name.text = config.name;
            // 增加值
            Level.text = "能量+" + itemConfig.addHp.ToString();
            // 品质
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(config.quality);
            xys.UI.Helper.SetSprite(quality, qualitConfig.icon);
            // icon
            xys.UI.Helper.SetSprite(icon, config.icon);
            // 数量
            int hadCount = packageMgr.GetItemCount(itemId);
            if (hadCount == 0)
                num.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", hadCount));
            else
                num.text = hadCount.ToString();

            // 单击事件
            obj.onClick.AddListener(() =>
            {
                request.WearBloodBottle(new Int32() { value = itemId }, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                        return;
                    if (respone.value)
                    {
                        Event.FireEvent<int>(EventID.BloodPoolWearHp, itemId);

                        if (itemId == 0)
                            return;

                        Config.TipsContent tipsConfig = Config.TipsContent.Get(3101);
                        if (tipsConfig == null)
                            return;

                        QualitySourceConfig qualityConfig = QualitySourceConfig.Get(config.quality);
                        string strName = string.Format("#[{0}]{1}#n", qualityConfig.colorname, config.name);
                        xys.UI.SystemHintMgr.ShowHint(string.Format(tipsConfig.des, strName));
                    }
                });
            });
        }
    }
}
#endif
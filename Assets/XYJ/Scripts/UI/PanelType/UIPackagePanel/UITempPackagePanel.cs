#if !USE_HOT
namespace xys.hot.UI
{
    using NetProto;
    using NetProto.Hot;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    class UITempPackagePanel : HotPanelBase
    {
        [SerializeField]
        xys.UI.TempPackageView m_TempView;

        [SerializeField]
        Button m_Close;

        [SerializeField]
        Button m_GetAllBtn;

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

        public UITempPackagePanel() :base(null) { }
        public UITempPackagePanel(xys.UI.UIHotPanel panel) : base(panel)
        {

        }

        protected override void OnInit()
        {
            packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            RegistButton();
        }

        protected override void OnShow(object args)
        {
            RegistEvent();
            ShowPackage();
        }

        protected override void OnHide()
        {
            m_TempView.Deselect(m_TempView.SelectedIndex);
            m_TempView.Clear();      
        }

        void RegistEvent()
        {
            Event.Subscribe(EventID.Package_UpdatePackage, ShowPackage);
        }

        void RegistButton()
        {
            m_Close.onClick.AddListener(CloseTips);
            m_GetAllBtn.onClick.AddListener(GetAllItem);
        }

        void ShowPackage()
        {
            List<NetProto.ItemGrid> items = new List<NetProto.ItemGrid>();
            packageMgr.tempPackage.ForEach((Grid g) =>
            {
                items.Add(g.data);
            });

            // 重置选择效果
            m_TempView.Deselect(m_TempView.SelectedIndex);
            m_TempView.SetDataList(items);
        }

        void CloseTips()
        {
            App.my.uiSystem.HidePanel(xys.UI.PanelType.UITempPackagePanel);
        }

        void GetAllItem()
        {
            request.GetAllFromTemp((error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (!respone.value)
                {
                    Config.TipsContent config = Config.TipsContent.Get(3110);
                    if (config == null)
                        return;

                    xys.UI.SystemHintMgr.ShowHint(config.des);
                }
            });
        }
    }
}
#endif
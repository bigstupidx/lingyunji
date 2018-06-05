#if !USE_HOT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using UI;

namespace xys.hot.UI
{
    class UIConvenientPanel : HotPanelBase
    {
        [SerializeField]
        Button m_returnBtn;
        [SerializeField]
        Button m_enterBtn;

        public UIConvenientPanel() : base(null)
        {

        }

        public UIConvenientPanel(UIHotPanel parent) : base(parent)
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInit()
        {
            m_returnBtn.onClick.AddListener(OnClickReturn);
            m_enterBtn.onClick.AddListener(OnClickEnter);
        }

        protected override void OnShow(object args)
        {

        }

        protected override void OnHide()
        {

        }

        /// <summary>
        /// 点击返回
        /// </summary>
        void OnClickReturn()
        {
            parent.Hide(true);
        }

        /// <summary>
        /// 点击进入关卡,先硬编码，进入关卡203
        /// </summary>
        void OnClickEnter()
        {
            parent.Hide(true);
            //App.my.localPlayer.GetModule<LevelModule>().request.GMChange(new NetProto.Int32() { value = 203 }, null);
            Event.FireEvent<int>(EventID.Level_Change, 203);
        }
    }
}
#endif

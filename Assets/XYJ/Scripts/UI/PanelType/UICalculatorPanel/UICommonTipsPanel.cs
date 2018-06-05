#if !USE_HOT
using xys.UI;
using UnityEngine;
using System.Collections;
using Config;
using xys;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    /// <summary>
    /// 通用tip功能
    /// editor zjh
    /// </summary>
    class UICommonTipsPanel : HotPanelBase
    {
        public class Param
        {
            public string des;//样式2,3说明文

            public string leftBtnTxt = "取消";//左按钮文本
            public string rightBtnTxt = "确定";//右

            public System.Action<bool> btnEvent = null;

            public int itemId = 0;
            public int itemNum = 0;

            public string promptKey = string.Empty;//根据key值查找是否显示的面板的记录
        }

        [SerializeField]
        StateRoot m_RootSR;
        [SerializeField]
        Text m_Text1;
        [SerializeField]
        Text m_Text2;
        [SerializeField]
        Button m_LeftBtn;
        [SerializeField]
        Text m_LeftBtnTxt;
        [SerializeField]
        Button m_RightBtn;
        [SerializeField]
        Text m_RightBtnTxt;
        [SerializeField]
        StateRoot m_PromptSR;
        [SerializeField]
        Image m_ItemIcon;
        [SerializeField]
        Image m_ItemQuality;
        [SerializeField]
        Text m_ItemCount;
        [SerializeField]
        Button m_ItemBtn;
        [SerializeField]
        Button m_PromptBtn;

        System.Action<bool> m_BtnCallback= null;

        int m_ItemId;
        string m_PromptKey;

        public UICommonTipsPanel() :base(null) { }
        public UICommonTipsPanel(UIHotPanel parent) : base(parent)
        {

        }

        protected override void OnInit()
        {
            m_RightBtn.onClick.AddListener(OnRightEvent);
            m_LeftBtn.onClick.AddListener(OnLeftEvent);
            m_ItemBtn.onClick.AddListener(OnItemEvent);
            m_PromptBtn.onClick.AddListener(OnPromptEvent);
        }

        protected override void OnShow(object args)
        {
            Param param = args as Param;
            if (param == null)
            {
                this.HidePanel();
                return;
            }
            if (param.itemId  != 0)
            {
                m_RootSR.CurrentState = 2;
                Item itemData = Item.Get(param.itemId);
                Helper.SetSprite(m_ItemIcon, itemData.icon);
                Helper.SetSprite(m_ItemQuality, QualitySourceConfig.Get(itemData.quality).icon);
                int hasItemCount = 0;
                hasItemCount = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(param.itemId);

                if (param.itemNum <= hasItemCount)
                    m_ItemCount.text = hasItemCount + "/" + param.itemNum;
                else
                    m_ItemCount.text = string.Format("<color=#{0}>{1}/{2}</color>", "ef3c49", hasItemCount, param.itemNum);

                m_ItemId = param.itemId;
                m_Text2.text = param.des;
            }
            else if(param.promptKey != string.Empty)
            {
                m_RootSR.CurrentState = 1;
                m_PromptKey = param.promptKey;
                int promptState = PlayerPrefs.GetInt(param.promptKey,0);
                if(promptState == 1)
                {
                    this.HidePanel();
                    return;
                }
                m_PromptSR.CurrentState = promptState;
                m_Text2.text = param.des;
            }
            else
            {
                m_RootSR.CurrentState = 0;
                m_Text1.text = param.des;
            }

            m_LeftBtnTxt.text = param.leftBtnTxt;
            m_RightBtnTxt.text = param.rightBtnTxt;

            m_BtnCallback = param.btnEvent;
        }

        void OnRightEvent()
        {
            if (m_BtnCallback != null)
                m_BtnCallback(true);
            this.HidePanel();
        }

        void OnLeftEvent()
        {
            if (m_BtnCallback != null)
                m_BtnCallback(false);
            this.HidePanel();
        }

        void OnItemEvent()
        {
            ItemTipsPanel.Param param = new ItemTipsPanel.Param();
            param.itemId = this.m_ItemId;
            App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
        }

        void OnPromptEvent()
        {
            int promptState = PlayerPrefs.GetInt(m_PromptKey);
            m_PromptSR.CurrentState = promptState == 1 ? 0 : 1;
            PlayerPrefs.SetInt(m_PromptKey, promptState == 1 ? 0 : 1);
        }

        void HidePanel()
        {
            parent.Hide(true);
        }
    }
}

#endif
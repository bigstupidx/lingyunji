#if !USE_HOT
namespace xys.hot.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UIWidgets;
    using xys.UI;
    using xys.UI.State;

    class UIStoryAsidePanel : HotPanelBase
    {
        [SerializeField]
        GameObject m_viewObj;

        // 背景遮挡
        [SerializeField]
        GameObject m_bgObj;
        [SerializeField]
        Button m_bgBtn;

        // 基本信息
        [SerializeField]
        Text m_name;
        [SerializeField]
        Text m_content;
        [SerializeField]
        Image m_headIcon;


        // 变量
        TaskDialogPrototype m_prototype;
        Action<int> m_endFun;

        int m_autoCloseTimerId = -1;

        UIStoryAsidePanel() : base(null) { }
        UIStoryAsidePanel(UIHotPanel parent) : base(parent) { }

        #region 内部实现

        // 点击背景事件
        void OnBGClick()
        {
            m_bgBtn.onClick.RemoveAllListeners();
            
            CloseDialog();
        }

        // 关闭旁白
        void CloseDialog()
        {
            m_autoCloseTimerId = -1;
            parent.Hide(false);

            // 关闭或者切换到另外的对话
            if (m_endFun != null)
                m_endFun(m_prototype.dialogIdx + 1);
        }

        #endregion

        protected override void OnInit()
        {
            
        }

        protected override void OnShow(object p)
        {
            m_endFun = null;
            object[] objs = (object[])p;
            m_prototype = (TaskDialogPrototype)objs[0];
            m_endFun = (Action<int>)objs[1];

            // 设置界面基本信息
            m_viewObj.SetActive(true);
            m_name.text = m_prototype.roleName;
            m_content.text = m_prototype.content;
            if (!string.IsNullOrEmpty(m_prototype.roleIcon))
                Helper.SetSprite(m_headIcon, m_prototype.roleIcon);

            // 设置操作
            if (m_prototype.shieldOpt)// 屏蔽玩家对界面的操作，需要点击跳过
            {
                m_bgObj.SetActive(true);
                m_bgBtn.onClick.AddListener(OnBGClick);
            }
            else
            {
                m_bgObj.SetActive(false);
                m_bgBtn.onClick.RemoveAllListeners();
            }

            // 倒计时
            if (m_prototype.autoPlayTime>0)
            {
                m_autoCloseTimerId = hotApp.my.mainTimer.Register(m_prototype.autoPlayTime, 1, CloseDialog);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();

            if (m_autoCloseTimerId != -1)
            {
                hotApp.my.mainTimer.Cannel(m_autoCloseTimerId);
                m_autoCloseTimerId = -1;
            }

            // 重置面板对象
            m_viewObj.SetActive(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
#endif
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

    class UIDialoguePanel : HotPanelBase
    {
        // 面板基本对象
        [SerializeField]
        GameObject m_buttomObj;

        [SerializeField]
        GameObject m_bgObj;
        [SerializeField]
        Button m_bgBtn;

        [SerializeField]
        GameObject m_nextObj;

        [SerializeField]
        Text m_name;
        [SerializeField]
        Text m_content;

        // 任务信息
        [SerializeField]
        GameObject m_taskObj;
        [SerializeField]
        Text m_taskDesc;

        // 选项相关对象
        [SerializeField]
        GameObject m_optionsObj;
        [SerializeField]
        UIGroup m_optGroup;

        // 小分支
        [SerializeField]
        GameObject m_smallBranchObj;

        [SerializeField]
        Button m_smallOptBtn1;
        [SerializeField]
        Text m_smallOptTitle1;
        [SerializeField]
        GameObject m_smallOptLight1;

        [SerializeField]
        Button m_smallOptBtn2;
        [SerializeField]
        Text m_smallOptTitle2;
        [SerializeField]
        GameObject m_smallOptLight2;

        // 大分支
        [SerializeField]
        GameObject m_branchObj;

        [SerializeField]
        Button m_optBtn1;
        [SerializeField]
        Text m_optTitle1;
        [SerializeField]
        Text m_optDesc1;
        [SerializeField]
        GameObject m_optLight1;

        [SerializeField]
        Button m_optBtn2;
        [SerializeField]
        Text m_optTitle2;
        [SerializeField]
        Text m_optDesc2;
        [SerializeField]
        GameObject m_optLight2;

        
        // 变量
        TaskDialogPrototype m_prototype;
        Action<int> m_endFun;

        const float m_playEffectTime = 0.5f;

        int m_autoCloseTimerId = -1;
        int m_playEffectTimerId = -1;

        UIDialoguePanel() : base(null) { }
        UIDialoguePanel(UIHotPanel parent) : base(parent) { }

        #region 内部实现

        /// <summary>
        /// 点击背景事件
        /// </summary>
        void OnBGClick()
        {
            m_bgBtn.onClick.RemoveAllListeners();
            
            CloseDialog(m_prototype.dialogIdx + 1);
        }

        /// <summary>
        /// 点击选项事件
        /// </summary>
        /// <param name="data"></param>
        void OnClickOption(DialogOptionData data)
        {
            if (data.IsFireEvent)
            {
                // 发送事件
            }
            m_bgBtn.onClick.RemoveAllListeners();

            if (m_prototype.uiType == 2 || m_prototype.uiType == 3)
            {
                // 做特殊效果表现
                m_optBtn1.onClick.RemoveAllListeners();
                m_optBtn2.onClick.RemoveAllListeners();
                m_smallOptBtn1.onClick.RemoveAllListeners();
                m_smallOptBtn2.onClick.RemoveAllListeners();

                m_playEffectTimerId = hotApp.my.mainTimer.Register(m_playEffectTime, 1, () =>
                {
                    if (data.IsFinishDialog)
                        CloseDialog(0);
                    else
                        CloseDialog(data.nextTaskIdx);
                });
            }
            else
            {
                if (data.IsFinishDialog)
                    CloseDialog(0);
                else
                    CloseDialog(data.nextTaskIdx);
            }

        }

        /// <summary>
        /// 关闭对话
        /// </summary>
        /// <param name="nextIdx"></param>
        void CloseDialog(int nextIdx)
        {
            m_autoCloseTimerId = -1;
            parent.Hide(false);
            
            // 关闭结束对话
            if (m_endFun != null)
                m_endFun(nextIdx);
        }

        /// <summary>
        /// 设置选项
        /// </summary>
        /// <param name="data"></param>
        /// <param name="go"></param>
        void SetOption(DialogOptionData data, GameObject go)
        {
            Button btn = go.GetComponentInChildren<Button>();
            Text text = go.GetComponentInChildren<Text>();
            text.text = data.title;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { OnClickOption(data); });
        }

        

        /// <summary>
        /// 重置面板对象
        /// </summary>
        void ResetPanel()
        {
            m_buttomObj.SetActive(false);
            m_nextObj.SetActive(false);
            m_optionsObj.SetActive(false);
            m_branchObj.SetActive(false);
            m_smallBranchObj.SetActive(false);
        }

        #endregion

        protected override void OnInit()
        {
            m_bgObj.SetActive(true);
            ResetPanel();
        }

        protected override void OnShow(object p)
        {
            m_endFun = null;
            object[] objs = (object[])p;
            m_prototype = (TaskDialogPrototype)objs[0];
            m_endFun = (Action<int>)objs[1];

            // 基本信息
            m_buttomObj.SetActive(true);
            m_bgObj.SetActive(true);
            if (m_prototype.shieldOpt)
            {
                m_bgBtn.onClick.RemoveAllListeners();
                m_nextObj.SetActive(true);
            }
            else
            {
                m_bgBtn.onClick.AddListener(OnBGClick);
            }
            m_name.text = m_prototype.roleName;
            m_content.text = GlobalSymbol.ToUT(m_prototype.content);

            // 自动关闭
            if (m_prototype.autoPlayTime>0)
            {
                m_autoCloseTimerId = hotApp.my.mainTimer.Register(m_prototype.autoPlayTime, 1, () => {
                    CloseDialog(m_prototype.dialogIdx + 1);
                });
            }
            // 任务信息
            if (!string.IsNullOrEmpty(m_prototype.taskDesc))
            {
                m_taskObj.SetActive(true);
                m_taskDesc.text = m_prototype.taskDesc;
            }
            else
            {
                m_taskObj.SetActive(false);
            }

            // 界面类型
            if (m_prototype.m_options!=null && m_prototype.m_options.Count>0)
            {
                if (m_prototype.uiType == 1)
                {
                    // 功能选项
                    m_optionsObj.SetActive(true);

                    m_optGroup.SetCount(m_prototype.m_options.Count);
                    for (int i = 0; i < m_prototype.m_options.Count; ++i)
                    {
                        SetOption(m_prototype.m_options[i], m_optGroup.Get(i));
                    }
                }
                else if (m_prototype.uiType == 2)
                {
                    // 大分支选项
                    m_branchObj.SetActive(true);
                    m_optLight1.SetActive(false);
                    m_optLight2.SetActive(false);

                    DialogOptionData optData1 = m_prototype.m_options[0];
                    m_optTitle1.text = optData1.title;
                    m_optDesc1.text = optData1.desc;
                    m_optBtn1.onClick.RemoveAllListeners();
                    m_optBtn1.onClick.AddListener(() => { OnClickOption(optData1); });
                    m_optBtn1.onClick.AddListener(() => { m_optLight1.SetActive(true); });

                    DialogOptionData optData2 = m_prototype.m_options[1];
                    m_optTitle2.text = optData2.title;
                    m_optDesc2.text = optData2.desc;
                    m_optBtn2.onClick.RemoveAllListeners();
                    m_optBtn2.onClick.AddListener(() => { OnClickOption(optData2); });
                    m_optBtn2.onClick.AddListener(() => { m_optLight2.SetActive(true); });

                }
                else if (m_prototype.uiType == 3)
                {
                    // 小分支选项
                    m_smallBranchObj.SetActive(true);
                    m_smallOptLight1.SetActive(false);
                    m_smallOptLight2.SetActive(false);

                    DialogOptionData optData1 = m_prototype.m_options[0];
                    m_smallOptTitle1.text = optData1.title;
                    m_smallOptBtn1.onClick.RemoveAllListeners();
                    m_smallOptBtn1.onClick.AddListener(() => { OnClickOption(optData1); });
                    m_smallOptBtn1.onClick.AddListener(() => { m_smallOptLight1.SetActive(true); });

                    DialogOptionData optData2 = m_prototype.m_options[1];
                    m_smallOptTitle2.text = optData2.title;
                    m_smallOptBtn2.onClick.RemoveAllListeners();
                    m_smallOptBtn2.onClick.AddListener(() => { OnClickOption(optData2); });
                    m_smallOptBtn2.onClick.AddListener(() => { m_smallOptLight2.SetActive(true); });
                }
            }

            // 镜头类型
            if (m_prototype.camType == 1)
            {
                // 角色镜头
            }
            else if (m_prototype.camType == 2)
            {
                // 模型镜头
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

            ResetPanel();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
#endif
#if !USE_HOT
using xys.UI;
using UnityEngine;
using System.Collections;
using Config;
using xys;
using UnityEngine.UI;

namespace xys.hot.UI
{
    /// <summary>
    /// 小键盘功能
    /// editor zjh
    /// </summary>
    class UICalculatorPanel : HotPanelBase
    {
        public class Param
        {
            public System.Action<int> valueChange = null;
            public System.Action<int> valueSure = null;
            public System.Action onMaxValue = null;

            public Vector3 pos = Vector3.zero;

            public int defaultValue = 1;
            public int maxValue = 1;
            public int minValue = 1;
        }
        
        [SerializeField]
        Transform m_MunRoot;
        [SerializeField]
        Button m_MaxBtn;
        [SerializeField]
        Button m_SureBtn;
        [SerializeField]
        Button m_CancelBtn;

        private int m_Value;
        private int m_MaxValue;
        private int m_MinValue;
        private int m_DefaultValue;

        protected System.Action<int> m_OnValueChange;

        private bool m_IsSynthesis;

        public UICalculatorPanel() : base(null) { }

        public UICalculatorPanel(UIHotPanel parent) : base(parent)
        {

        }

        protected override void OnInit()
        { 
            for(int i = 0; i < m_MunRoot.childCount;i++)
            {
                int index = i;
                m_MunRoot.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.OnInput(index);
                });
            }

            m_MaxBtn.onClick.AddListener(this.OnMax);
            m_SureBtn.onClick.AddListener(this.OnSure);
            m_CancelBtn.onClick.AddListener(this.OnCanel);
        }

        protected override void OnHide()
        {
            if (m_Value == 0)
            {
                if (m_OnValueChange != null)
                    m_OnValueChange(1);
            }
            m_OnValueChange = null;
        }

        protected override void OnShow(object args)
        {
            Param tempEvent = args as Param;
            if (tempEvent == null)
            {
                Debuger.LogError("UICalculatorPanel :: Event is null");
                App.my.uiSystem.HidePanel(PanelType.UICalculatorPanel, true);
                return;
            }

            parent.transform.GetChild(0).localPosition = tempEvent.pos;
            m_MaxValue = tempEvent.maxValue;
            m_MinValue = tempEvent.minValue;
            m_DefaultValue = tempEvent.defaultValue;
            m_OnValueChange = tempEvent.valueChange;

            m_Value = m_DefaultValue == 1 ? 1 : m_DefaultValue;

            if (m_OnValueChange != null)
                m_OnValueChange(m_Value);

            m_IsSynthesis = true;
        }

        void OnInput(int index)
        {
            if (index < 0 || index > 9)
                return;

            string value = m_Value.ToString();
            if (m_IsSynthesis)
            {
                value = index.ToString();
            }
            else
            {
                value = value.Insert(value.Length, index.ToString());
            }

            try
            {
                int tempValue;
                int.TryParse(value, out tempValue);
                if (tempValue > m_MaxValue)
                {
                    SystemHintMgr.ShowHint("输入数量超出范围");
                    m_Value = m_MaxValue;
                    if (m_OnValueChange != null)
                        m_OnValueChange(m_MaxValue);
                }
                else
                    m_Value = tempValue;
            }
            catch (System.Exception e)
            {
                Debuger.LogError("UICalculatorPanel : Input error!");
            }

            if (m_OnValueChange != null)
                m_OnValueChange(m_Value);

            m_IsSynthesis = false;
        }

        void OnCanel()
        {
            string value = m_Value.ToString();
            if (value.Length == 1 && m_Value ==0)
            {
                m_Value = 0;
                m_IsSynthesis = false;
            }
            else if (value.Length == 1 && m_Value > 1)
            {
                m_Value = 0;
                if (m_OnValueChange != null)
                    m_OnValueChange(m_Value);
                return;
            }
            else
            {
                value = value.Substring(0, value.Length - 1);
                try
                {
                    int.TryParse(value, out m_Value);
                }
                catch (System.Exception e)
                {
                    Debuger.LogError("UICalculatorPanel : Input error!");
                }
            }
            if (m_OnValueChange != null)
                m_OnValueChange(m_Value);
        }

        void OnMax()
        {
            m_IsSynthesis = false;
            m_Value = m_MaxValue;
            if (m_OnValueChange != null)
                m_OnValueChange(m_Value);
        }

        void OnSure()
        {
            m_IsSynthesis = false;
            if (m_OnValueChange != null)
                m_OnValueChange(m_Value);
            App.my.uiSystem.HidePanel(PanelType.UICalculatorPanel, true);
        }
    }
}
#endif
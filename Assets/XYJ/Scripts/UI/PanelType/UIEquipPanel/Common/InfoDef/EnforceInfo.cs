#if !USE_HOT


namespace xys.hot.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI.State;

    class EnforceInfo: InfoBase
    {
        Text addVal;
        StateRoot line;
        StateRoot arrow;
        Transform m_Trans;
        enum LineState
        {
            STATE_HIDE,
            STATE_SHOW
        }
        enum UpArrowState
        {
            STATE_ASCEND,
            STATE_HIDE,
            STATE_FALL
        }
        public EnforceInfo() { }
        public EnforceInfo(Transform trans): base(trans.Find("Name").GetComponent<Text>(), trans.Find("Text").GetComponent<Text>())
        {
            m_Trans = trans;
            var tempTans = trans.Find("ZengJia");
            if (tempTans!=null)
            {
                addVal = tempTans.GetComponent<Text>();
            }
            tempTans = trans.Find("Line");
            if (tempTans != null)
            {
                line = tempTans.GetComponent<StateRoot>();
            }
            tempTans = trans.Find("UpRow");
            if (tempTans!=null)
            {
                arrow = tempTans.GetComponent<StateRoot>();
            }
            
        }

        public void SetAddValue(double value)
        {
            if (value == 0 )
            {
                addVal.gameObject.SetActive(false);
            }
            else
            {
                addVal.gameObject.SetActive(true);
                addVal.text = value.ToString();
            }
            
        }

        public void ShowLine()
        {
            line.SetCurrentState((int)LineState.STATE_SHOW, false);
        }
        public void HideLine()
        {
            line.SetCurrentState((int)LineState.STATE_HIDE, false);
        }
        public void SetArrowUp()
        {
            arrow.SetCurrentState((int)UpArrowState.STATE_ASCEND, false);
        }
        public void HideArrow()
        {
            arrow.SetCurrentState((int)UpArrowState.STATE_HIDE, false);
        }
        public void SetArrowDown()
        {
            arrow.SetCurrentState((int)UpArrowState.STATE_FALL, false);
        }

        public new void Hide()
        {
            m_Trans.gameObject.SetActive(false);
        }
        public new void Show()
        {
            m_Trans.gameObject.SetActive(true);
        }
    }
}
#endif
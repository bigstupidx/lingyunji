#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace xys.hot.UI
{
    [AutoILMono]
    public class FunctionItem 
    {
        [SerializeField]
        Button m_Button; // 对应的点击按钮
        [SerializeField]
        Transform redRoot; // 红点根结点
        [SerializeField]
        Transform m_Trans;
        [SerializeField]
        Text m_Text;

        public enum ItemState
        {
            Normal, // 正常状态
            Red, // 红点状态
        }

        public ItemState state
        {
            get { return redRoot.gameObject.activeSelf ? ItemState.Red : ItemState.Normal; }
            set { redRoot.gameObject.SetActive(value == ItemState.Normal ? false : true); }
        }

        public void SetActive(bool active)
        {
            m_Trans.gameObject.SetActive(active);
        }

        public void AddOnClickListener(UnityAction action)
        {
            if(m_Button!=null)
                m_Button.onClick.AddListener(action);
        }
        public void SetText(string str)
        {
            m_Text.text = str;
        }
    }
}
#endif

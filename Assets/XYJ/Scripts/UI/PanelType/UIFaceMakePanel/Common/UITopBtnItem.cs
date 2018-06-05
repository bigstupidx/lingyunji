#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class UITopBtnItem
    {
        [SerializeField]
        public StateRoot m_StateRoot; 
        [SerializeField]  
        public Text m_Text;
        [SerializeField]
        public Image m_Image;
        public int m_Index = 0;
        private System.Action<UITopBtnItem> m_ClickEvent = null;
        private System.Action<UITopBtnItem> m_StateChangeEvent = null;
        void Awake()
        {
            if(m_StateRoot!=null)
            {
                m_StateRoot.onClick.AddListener(OnClick);
                m_StateRoot.onStateChange.AddListener(OnSelectChange);
            }

        }
        void OnClick()
        {
            if (m_ClickEvent != null)
            {
                m_ClickEvent(this);
            }
        }
        void OnSelectChange()
        {
            if(m_StateChangeEvent!=null)
            {
                m_StateChangeEvent(this);
            }
            
        }
        public void Set(int index, System.Action<UITopBtnItem> clickEvent, System.Action<UITopBtnItem> stateChangeEvent, string name = null,Sprite sprite=null)
        {
            this.m_StateRoot.CurrentState = 0;
            this.m_Index = index;
            this.m_ClickEvent = clickEvent;
            this.m_StateChangeEvent = stateChangeEvent;
            if(name !=null)
            {
                m_Text.text = name;
            }
            if(sprite!=null)
            {
                m_Image.sprite = sprite;
            }

        }
    }
}
#endif
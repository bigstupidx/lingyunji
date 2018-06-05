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
    class UIShapeSliderItem
    {
        [SerializeField]
        Text m_SliderText;
        [SerializeField]
        public Slider m_Slider;
        [SerializeField]
        Text m_NumText;

        public int m_Index;


        private System.Action<UIShapeSliderItem,System.Single> m_OnValueChange = null;
        void Awake()
        {
            if(m_Slider!=null)
            {
                m_Slider.onValueChanged.RemoveAllListeners();
                m_Slider.onValueChanged.AddListener(OnValueChange);
            }
            
        }
        public void Set(int index,float value,string name,System.Action<UIShapeSliderItem, System.Single> onValueChange)
        {
            m_OnValueChange = null;
            m_Index = index;
            m_SliderText.text = name;
            m_Slider.value = value;  
            m_OnValueChange = onValueChange;
        }
        void OnValueChange(System.Single value)
        {
            if(m_OnValueChange!=null)
            {
                m_OnValueChange(this, value);
            }

            if(m_NumText!=null)
            {
                if(value>0)
                {
                    m_NumText.text = "+" + ((int)(value * 100)).ToString();
                }
                else
                {
                    m_NumText.text = "-" + ((int)(value * 100)).ToString();
                }
                
            }
        }

    }
}
#endif
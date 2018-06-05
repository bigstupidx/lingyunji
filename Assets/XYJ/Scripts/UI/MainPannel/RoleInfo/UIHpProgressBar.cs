#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Config;
using xys.battle;
using xys.UI;

namespace xys.hot.UI
{
    //血量条
    public class UIHpProgressBar
    {
        //进度条
        Image m_progressBar;
        Text m_valueText;
        int m_curValue;
        int m_maxValue;
        public UIHpProgressBar(Image progressBar, Text valueText)
        {
            m_progressBar = progressBar;
            m_valueText = valueText;
            if (progressBar != null)
                progressBar.type = Image.Type.Filled;
        }

        public void Init( int curValue,int maxValue)
        {
            m_curValue = curValue;
            m_maxValue = maxValue;
            RefreshUI();
        }

        public void SetCurValue( int curValue )
        {
            m_curValue = curValue;
            RefreshUI();
        }

        public void SetMaxValue( int maxValue )
        {
            m_maxValue = maxValue;
            RefreshUI();
        }

        void RefreshUI()
        {
            if (m_progressBar != null)
            {
                float mul;
                if (m_maxValue == 0)
                    mul = 0;
                else
                    mul = (float)m_curValue / m_maxValue;
                if (mul > 1)
                    mul = 1;
                m_progressBar.fillAmount = mul;
            }
            if (m_valueText != null)
                m_valueText.text = string.Format("{0}/{1}", m_curValue, m_maxValue);
        }
    }


    //真气
    class UIMpProgressBar
    {
        Transform[] m_items;
        public UIMpProgressBar(Transform root)
        {
            Transform itemTrans = root.Find("mpList");
            m_items = new Transform[itemTrans.childCount];
            for (int i = 0; i < m_items.Length; i++)
                m_items[i] = itemTrans.GetChild(i);
        }

        public void SetCurValue(int curValue)
        {
            for(int i=0;i<m_items.Length;i++)
            {
                m_items[i].gameObject.SetActive(i < curValue);
            }
        }
    }
}
#endif
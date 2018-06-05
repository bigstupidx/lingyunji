#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class RefInfo: InfoBase
    {
        Image m_PercentageSp;
        Image m_LightSp;
        StateRoot m_ColorState;
        Transform m_Transform;
        static Dictionary<int, int> qualityToCorlor = new Dictionary<int, int>() { {3,0}, {1,1}, {2,3}, {4,2} };  //Quality与color的StateRoot中state的Id与状态对应DIC
        Vector3 m_lightInitialPos;


        public RefInfo(Transform trans):base(trans.Find("Name").GetComponent<Text>(), trans.Find("Text").GetComponent<Text>())
        {
            m_Transform = trans;
            m_LightSp = trans.Find("Mask").Find("Light").GetComponent<Image>();
            m_PercentageSp = trans.Find("Tiao").GetComponent<Image>();
            m_ColorState = trans.Find("Tiao").GetComponent<StateRoot>();
            m_lightInitialPos = m_LightSp.transform.localPosition;
        }

        public new void Hide()
        {
            m_Transform.gameObject.SetActive(false);
        }
        public new void Show()
        {
            m_Transform.gameObject.SetActive(true);
        }

        public void SetValue(double value,double total,int quality)
        {
            m_LightSp.transform.localPosition = m_lightInitialPos;
            base.SetValue(value,base.defaultColor);
            m_PercentageSp.fillAmount = (float)(value / total);
            if (m_PercentageSp.gameObject.activeSelf && m_PercentageSp.fillAmount > 0)
            {
                m_LightSp.gameObject.SetActive(true);
                float offset = (m_PercentageSp.fillAmount - 0.5f) * m_PercentageSp.GetComponent<RectTransform>().rect.width + 2 * m_LightSp.GetComponent<RectTransform>().rect.width;
                m_LightSp.transform.localPosition += new Vector3(offset, 0, 0);
            }
            else
                m_LightSp.gameObject.SetActive(false);
            if (qualityToCorlor.ContainsKey(quality))
            {
                m_ColorState.SetCurrentState(qualityToCorlor[quality],false);
            }
            else
            {
                XYJLogger.LogError("Undefined Quality type:"+quality);
            }
        }
    }
}
#endif

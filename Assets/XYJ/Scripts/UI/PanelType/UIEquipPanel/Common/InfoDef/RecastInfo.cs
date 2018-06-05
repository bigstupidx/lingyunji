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
    class RecastInfo:EnforceInfo
    {
        Image m_PercentageSp;
        Image m_LightSp;
        Transform m_Trans;
        Vector3 m_lightInitialPos;
        public RecastInfo(Transform trans):base(trans)
        {
            m_Trans = trans;
            m_PercentageSp = trans.Find("Tiao").GetComponent<Image>();
            m_LightSp = trans.Find("Mask").Find("Light").GetComponent<Image>();
            m_lightInitialPos = m_LightSp.transform.localPosition;
        }

        public void SetValue(double value,double min, double max)
        {
            m_PercentageSp.fillAmount = (float)((value-min)*0.8 / (max-min)+0.2);
            RefreshLightPos();
            base.SetValue(value,base.defaultColor);
        }
        public void SetValue(double value, float percentage)
        {
            m_PercentageSp.fillAmount = percentage;
            RefreshLightPos();
            base.SetValue(value, base.defaultColor);
        }
        void RefreshLightPos()
        {
            if (m_PercentageSp.gameObject.activeSelf && m_PercentageSp.fillAmount > 0)
            {
                m_LightSp.transform.localPosition = m_lightInitialPos;
                float offset = (m_PercentageSp.fillAmount - 0.5f) * m_PercentageSp.GetComponent<RectTransform>().rect.width + 2 * m_LightSp.GetComponent<RectTransform>().rect.width;
                m_LightSp.transform.localPosition += new Vector3(offset, 0, 0);
            }
        }
        public new void Show()
        {
            m_Trans.gameObject.SetActive(true);
        }
        public new void Hide()
        {
            m_Trans.gameObject.SetActive(false);
        }
        public bool IsActive()
        {
            return m_Trans.gameObject.activeSelf;
        }

        public float GetPercentage()
        {
            return m_PercentageSp.fillAmount;
        }
        public Vector3 GetLocalPosition()
        {
            return m_Trans.position;
        }
    }
}
#endif

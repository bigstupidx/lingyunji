using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;

namespace xys.UI
{
    /// <summary>
    /// 类似技能cd的逻辑处理
    /// </summary>
    public class ImageFillAmount : MonoBehaviour
    {
        Text m_text;
        Image m_image;
        float m_canPlayTime;
        float m_cdLenght;

        public void Init(Text text, Image ima)
        {
            m_text = text;
            m_image = ima;
            SetEnable(false);
        }

        public void PlayCD(float canPlayTime, float cdLenght)
        {
            if (cdLenght <= 0 || canPlayTime <= BattleHelp.timePass)
            {
                SetEnable(false);
                return;
            }
            m_canPlayTime = canPlayTime;
            m_cdLenght = cdLenght;
            SetEnable(true);
        }

        public void Stop()
        {
            SetEnable(false);
        }

        void SetEnable(bool enable)
        {
            if (this.enabled != enable)
                this.enabled = enable;
            if (m_text != null)
                m_text.gameObject.SetActive(enable);
            if (m_image != null)
                m_image.gameObject.SetActive(enable);
        }

        void Update()
        {
            float cdLeft = m_canPlayTime-BattleHelp.timePass;
            if (cdLeft<=0)
            {
                SetEnable(false);
                return;
            }

            float mul = cdLeft / m_cdLenght;
            if (m_image != null)
                m_image.fillAmount = mul;
            if (m_text != null)
                m_text.text = cdLeft.ToString("f2");
        }
    }
}


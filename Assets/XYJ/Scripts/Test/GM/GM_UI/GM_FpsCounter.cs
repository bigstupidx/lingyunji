using UnityEngine;
using System.Collections;
using UI;

namespace xys.gm
{

    public class GM_FpsCounter : MonoBehaviour
    {
        int m_frameCnt = 0;
        float m_frameStartTime;
        Label m_label;

        public void InitFps(Label label)
        {
            m_frameStartTime = Time.realtimeSinceStartup;
            m_label = label;
            m_label.horizontalOverflow = HorizontalWrapMode.Overflow;
            m_label.verticalOverflow = VerticalWrapMode.Overflow;
            m_label.alignment = TextAnchor.UpperLeft;
        }

        void Update()
        {
            m_frameCnt++;
            if (Time.realtimeSinceStartup - m_frameStartTime > 1)
            {
                m_label.text = string.Format("{0}/{1}", m_frameCnt, Application.targetFrameRate);

                m_frameCnt = 0;
                m_frameStartTime = Time.realtimeSinceStartup;
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                Time.timeScale *= 2;
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                Time.timeScale /= 2;
            if (Input.GetKeyDown(KeyCode.F1))
                GM_BattlePage.ReloadConfig();
        }
    }


}
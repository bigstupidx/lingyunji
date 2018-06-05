#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class SkillKeyboardPoint
    {
        [SerializeField]
        Transform m_transform;
        [SerializeField]
        GameObject m_PitchOn;

        public void ShowPoint(int index)
        {
            if (index < 0)
            {
                m_PitchOn.SetActive(false);
                return;
            }

            Transform point = m_transform.Find(index.ToString());

            m_PitchOn.transform.localPosition = point.localPosition;
            m_PitchOn.GetComponent<RectTransform>().sizeDelta = point.Find("Bg").GetComponent<RectTransform>().sizeDelta;
            m_PitchOn.SetActive(true);
        }
    }
}
#endif
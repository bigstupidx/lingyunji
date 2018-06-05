using UnityEngine;

/// <summary>
/// Tween the object's local scale.
/// </summary>

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SizeScale : MonoBehaviour
    {
        private RectTransform m_RectTransform;
        public RectTransform rectTransform
        {
            get { return m_RectTransform; }
        }

        Vector2 startSize;

        int mWidth;
        int mHeight;

        Vector2 offsetMin;
        Vector2 offsetMax;

        void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            startSize = rectTransform.rect.size;

            offsetMin = m_RectTransform.offsetMin;
            offsetMax = m_RectTransform.offsetMax;

            mWidth = Screen.width;
            mHeight = Screen.height;
        }

        void LateUpdate()
        {
            int w = Screen.width;
            int h = Screen.height;

            if (w != mWidth || h != mHeight)
            {
                mWidth = w;
                mHeight = h;

                Vector2 offsetMin1 = m_RectTransform.offsetMin;
                Vector2 offsetMax1 = m_RectTransform.offsetMax;

                m_RectTransform.offsetMin = offsetMin;
                m_RectTransform.offsetMax = offsetMax;

                startSize = rectTransform.rect.size;

                m_RectTransform.offsetMin = offsetMin1;
                m_RectTransform.offsetMax = offsetMax1;
            }
        }

        public float value
        {
            get { return rectTransform.rect.size.x / startSize.x; }
            set
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startSize.x * value);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, startSize.y * value);
            }
        }
    }
}
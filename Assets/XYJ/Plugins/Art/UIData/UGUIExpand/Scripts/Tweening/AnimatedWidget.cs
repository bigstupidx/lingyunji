using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Makes it possible to animate the widget's width and height using Unity's animations.
/// </summary>

namespace UI
{
    [ExecuteInEditMode]
    public class AnimatedWidget : MonoBehaviour
    {
        public float width = 1f;
        public float height = 1f;

        MaskableGraphic mGraphic;

        void OnEnable()
        {
            mGraphic = GetComponent<MaskableGraphic>();
            LateUpdate();
        }

        void LateUpdate()
        {
            if (mGraphic != null)
            {
                mGraphic.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                mGraphic.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    }
}

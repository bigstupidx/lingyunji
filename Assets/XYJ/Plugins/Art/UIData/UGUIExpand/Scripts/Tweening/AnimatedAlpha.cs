using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Makes it possible to animate alpha of the widget or a panel.
    /// </summary>

    [ExecuteInEditMode]
    public class AnimatedAlpha : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float alpha = 1f;

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
                Color color = mGraphic.color;
                color.a = alpha;
                mGraphic.color = color;
            }
        }
    }
}
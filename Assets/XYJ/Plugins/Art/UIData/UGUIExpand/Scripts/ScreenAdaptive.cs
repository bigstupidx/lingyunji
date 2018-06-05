using UnityEngine;
using UnityEngine.EventSystems;

namespace xys.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class ScreenAdaptive : UIBehaviour
    {
        RectTransform rect;

        protected override void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        void Set()
        {
            rect.pivot = Vector2.one * 0.5f;

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1334);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 750);
        }

        private void LateUpdate()
        {
            Set();
        }
    }
}
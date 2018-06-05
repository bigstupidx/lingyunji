using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Makes it possible to animate a color of the widget.
/// </summary>

namespace UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MaskableGraphic))]
    public class AnimatedColor : MonoBehaviour
    {
        public Color color = Color.white;

        MaskableGraphic mGraphic;

        void OnEnable() { mGraphic = GetComponent<MaskableGraphic>(); LateUpdate(); }
        void LateUpdate() { mGraphic.color = color; }
    }
}

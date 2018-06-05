using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class AutoScaling : MonoBehaviour
{
    RectTransform parent;

    Graphic graphic;

    IEnumerator Start()
    {
        graphic = GetComponent<Graphic>();
        parent = graphic.rectTransform.parent as RectTransform;

        yield return 0;
        OnScreenResize();
    }

    // 屏幕分辨率发生了变化
    void OnScreenResize()
    {
        //Vector2 size = parent.rect.size;

        graphic.SetNativeSize();

        Vector2 graphicSize = graphic.rectTransform.rect.size;

        Vector2 parentSize = parent.rect.size;

        float aspect = 1.0f * graphicSize.x / graphicSize.y;

        if (graphicSize.x < parentSize.x)
        {
            graphicSize.x = (int)parentSize.x;
            graphicSize.y = (int)(graphicSize.x / aspect);
        }

        if (graphicSize.y < parentSize.y)
        {
            graphicSize.y = (int)parentSize.y;
            graphicSize.x = (int)(graphicSize.y * aspect);
        }

        graphic.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphicSize.x);
        graphic.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, graphicSize.y);
    }
}

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class uAnchor : MonoBehaviour
{
    [SerializeField]
    RectTransform rect;

    [SerializeField]
    Vector2 posOffset;

    [SerializeField]
    Vector2 sizeOffset;

    RectTransform cache;
    RectTransform self { get { if (cache == null) cache = GetComponent<RectTransform>(); return cache; } }

    public enum SizeType
    {
        Null,
        WithAndHeight,
        Width,
        Height,
    }

    public enum OffsetType
    {
        Null,
        X,
        Y,
        XandY,
    }

    [SerializeField]
    SizeType sizeType = SizeType.WithAndHeight;

    [SerializeField]
    OffsetType offsetType = OffsetType.XandY;

    void LateUpdate()
    {
        if (rect == null)
            return;

        switch (offsetType)
        {
        case OffsetType.X:
            {
                Vector3 pos = self.anchoredPosition;
                self.anchoredPosition = new Vector2(pos.x + posOffset.x, pos.y);
            }
            break;
        case OffsetType.Y:
            {
                Vector3 pos = self.anchoredPosition;
                self.anchoredPosition = new Vector2(pos.x, pos.y + posOffset.y);
            }
            break;
        case OffsetType.XandY:
            {
                self.anchoredPosition = self.anchoredPosition + posOffset;
            }
            break;
        }

        Rect area = rect.rect;

        switch (sizeType)
        {
        case SizeType.WithAndHeight:
            {
                self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, area.width + sizeOffset.x);
                self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, area.height + sizeOffset.y);
            }
            break;
        case SizeType.Width:
            {
                self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, area.width + sizeOffset.x);
            }
            break;
        case SizeType.Height:
            {
                self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, area.height + sizeOffset.y);
            }
            break;
        }
    }
}

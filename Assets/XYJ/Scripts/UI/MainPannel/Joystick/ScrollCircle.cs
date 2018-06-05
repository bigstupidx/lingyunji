using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 可以用来实现摇杆
/// </summary>
public class ScrollCircle : ScrollRect
{
    [HideInInspector]
    public float m_radius = 0;

    public Action<PointerEventData> onBeginDrag;
    public Action<PointerEventData> onDrag;
    public Action<PointerEventData> onEndDrag;

    protected override void Awake()
    {
        base.Awake();
        //计算半径
        m_radius = (transform as RectTransform).sizeDelta.x * 0.5f;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (onBeginDrag != null)
            onBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        var contentPostion = this.content.anchoredPosition;
        if (contentPostion.magnitude > m_radius)
        {
            contentPostion = contentPostion.normalized * m_radius;
            SetContentAnchoredPosition(contentPostion);
        }
        if (onDrag != null)
            onDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (onEndDrag != null)
            onEndDrag(eventData);
    }
}
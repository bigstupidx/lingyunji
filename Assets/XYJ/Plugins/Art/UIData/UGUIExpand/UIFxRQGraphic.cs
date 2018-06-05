using UnityEngine;
using System.Collections.Generic;

public class UIFxRQGraphic : MonoBehaviour
{
    [SerializeField]
    public Canvas canvas;

    List<Renderer> renderers = new List<Renderer>();

    [SerializeField]
    public int relativeSortingOrder = 0;

    int lastOrderInLayer = -1;
    
#if UNITY_EDITOR
    public List<Renderer> Renderers { get { return renderers; } }
#endif

    public void Start()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                enabled = false;
                return;
            }
        }

        gameObject.GetComponentsInChildren(true, renderers);
        int sortingOrder = canvas.sortingOrder + relativeSortingOrder;
        CheckQueue(sortingOrder);
    }

    void Update()
    {
        int sortingOrder = canvas.sortingOrder + relativeSortingOrder;
        if (lastOrderInLayer == sortingOrder)
            return;

        CheckQueue(sortingOrder);
    }

    void CheckQueue(int sortingOrder)
    {
        for (int i = 0; i < renderers.Count; ++i)
            renderers[i].sortingOrder = sortingOrder;
        lastOrderInLayer = sortingOrder;
    }
}

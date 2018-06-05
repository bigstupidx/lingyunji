using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UI
{
    /// <summary>
    /// Labels are graphics that display text.
    /// </summary>

    [AddComponentMenu("UI/Label", 10)]
    public class Label: Text
    {
        Vector2 last_size = new Vector2(-1000f, -1000f);
         
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            base.OnPopulateMesh(toFill);
            last_size = rectTransform.rect.size;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (gameObject.activeInHierarchy)
            {
                // prevent double dirtying...
                if (CanvasUpdateRegistry.IsRebuildingLayout())
                {
                    if (last_size == rectTransform.rect.size)
                        return;

                    SetVerticesDirty();
                }
                else
                {
                    if (last_size != rectTransform.rect.size)
                        SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }
    }
}

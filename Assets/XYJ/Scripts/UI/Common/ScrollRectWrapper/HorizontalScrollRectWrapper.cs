﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    [DisallowMultipleComponent]
    public class HorizontalScrollRectWrapper : ScrollRectWrapper
    {
        protected override float GetSize(RectTransform item)
        {
            float size = ContentSpacing;
            if(GridLayout != null)
            {
                size += GridLayout.cellSize.x;
            }
            else
            {
                size += LayoutUtility.GetPreferredWidth(item);
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return vector.x;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(-value, 0);
        }

        public override bool SetMarkerToCenter(int index, Action callBack)
        {
            // TODO 
            return false;
        }

        protected override void Awake()
        {
            base.Awake();
            DirectionSign = 1;

            GridLayoutGroup layout = Content.GetComponent<GridLayoutGroup>();
            if(layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount)
            {
                Debug.LogError("[HorizontalScrollRectWarpper] unsupported GridLayoutGroup constraint");
            }
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;
            if(viewBounds.max.x > contentBounds.max.x)
            {
                float size = NewItemAtEnd();
                if(size > 0)
                {
                    if(Threshold < size)
                    {
                        // 加阈值防止重复删除和创建
                        Threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if(viewBounds.max.x < contentBounds.max.x - Threshold)
            {
                float size = DeleteItemAtEnd();
                if(size > 0)
                {
                    changed = true;
                }
            }

            if(viewBounds.min.x < contentBounds.min.x)
            {
                float size = NewItemAtStart();
                if(size > 0)
                {
                    if(Threshold < size)
                    {
                        Threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if(viewBounds.min.x > contentBounds.min.x + Threshold)
            {
                float size = DeleteItemAtStart();
                if(size > 0)
                {
                    changed = true;
                }
            }
            return changed;
        }
    }
}

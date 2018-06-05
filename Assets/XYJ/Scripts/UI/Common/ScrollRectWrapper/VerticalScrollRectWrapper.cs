using System;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class VerticalScrollRectWrapper : ScrollRectWrapper
    {
        protected override float GetSize(RectTransform item)
        {
            float size = ContentSpacing;
            if(GridLayout != null)
            {
                size += GridLayout.cellSize.y;
            }
            else
            {
                size += LayoutUtility.GetPreferredHeight(item);
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return vector.y;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(0, value);
        }

        public virtual void SetBottom()
        {
            RefillCells(TotalCount - Content.childCount);
            var height = Content.rect.height;
            for(int i = ItemTypeStart ; i < ItemTypeEnd ; i++)
            {
                height += SizeDatas[i];
            }
            Content.localPosition = new Vector3(Content.localPosition.x, height, Content.localPosition.z);
            StopMovement();
            IsLock = false;
        }
        public override bool SetMarkerToCenter(int index, Action callBack = null)
        {
            if(TotalCount - MaxCount > index || TotalCount < index)
            {
                return false;
            }

            RectTransform startItem;
            try
            {
                startItem = Content.GetChild(0) as RectTransform;
            }
            catch (Exception e)
            {
                return false;
            }

            if(null == startItem)
            {
                Debug.LogError("conntent is not init!");
                return false;
            }

            var itemCenterPosInScroll = ViewRect.InverseTransformPoint(GetWidgetWorldPoint(startItem));
            var centerPosInScroll = ViewRect.InverseTransformPoint(GetWidgetWorldPoint(Viewport));
            var itemStartOffset = centerPosInScroll - itemCenterPosInScroll;

            var count = ItemTypeStart - index;
            var sign = Mathf.Sign(count);

            Vector2 offset = Vector2.zero;
            for(var i = 0 ; i < count * sign ;)
            {
                // marker到当前第一个element的向量
                offset.y -= sign * SizeDatas[ItemTypeStart - (int)( ++i * sign )];
            }
            // 差量修正
            offset.y += itemStartOffset.y;
            offset.y += sign * SizeDatas[index] / 2;
            offset.y -= sign > 0 ? startItem.rect.height / 2 : 0;

            // 最后一个element到marker的高度
            float h = 0f;
            for(int i = 0 ; i < TotalCount - 1 - index ; i++)
            {
                h += SizeDatas[TotalCount - 1 - i];
            }
            // 差量修正
            h -= SizeDatas[index] / 2;

            var vh = ViewRect.rect.height / 2;
            if(h < vh)
            {
                // 此情况y必然大于0
                offset.y -= vh - h;
            }

            ConstOffset = offset;
            IsMarkerMoving = true;

            if(null != callBack)
            {
                MarkerCallback = callBack;
            }
            // 定位锁定
            IsLock = true;
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            DirectionSign = -1;
            ItemTypeEnd = 0;
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;
            if(viewBounds.min.y < contentBounds.min.y + 1)
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
            else if(viewBounds.min.y > contentBounds.min.y + Threshold)
            {
                float size = DeleteItemAtEnd();
                if(size > 0)
                {
                    changed = true;
                }
            }

            if(viewBounds.max.y > contentBounds.max.y - 1)
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
            else if(viewBounds.max.y < contentBounds.max.y - Threshold)
            {
                float size = DeleteItemAtStart();
                if(size > 0)
                {
                    changed = true;
                }
            }

            if (!changed && !IsLock && AddNewData && !Dragging)
            {
                float size = NewItemAtEnd();
                if(size > 0)
                {
                    if(Threshold < size)
                    {
                        Threshold = size * 1.1f;
                    }
                    changed = true;
                }
                AddNewData = false;
            }
            return changed;
        }
    }
}

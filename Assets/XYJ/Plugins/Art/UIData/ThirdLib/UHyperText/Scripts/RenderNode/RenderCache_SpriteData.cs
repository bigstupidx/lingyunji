﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    // 缓存渲染元素
    public partial class RenderCache
    {
        class SpriteData : BaseData
        {
            public Sprite sprite;

            public void Reset(NodeBase n, Sprite s, Rect r, Line l)
            {
                node = n;
                sprite = s;
                rect = r;
                line = l;
            }

            protected override void OnRelease()
            {
                sprite = null;
            }

            public override void Render(VertexHelper vh, Rect area, Vector2 offset, float pixelsPerUnit)
            {
                Color currentColor = node.d_color;
                if (currentColor.a <= 0.01f)
                    return;

                var uv = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);

                Vector2 leftPos = GetStartLeftBottom(1f) + offset;
                Tools.LB2LT(ref leftPos, area.height);

                float width = rect.width;
                float height = rect.height;

                int count = vh.currentVertCount;
                vh.AddVert(new Vector3(leftPos.x, leftPos.y), currentColor, new Vector2(uv.x, uv.y));
                vh.AddVert(new Vector3(leftPos.x, leftPos.y + height), currentColor, new Vector2(uv.x, uv.w));
                vh.AddVert(new Vector3(leftPos.x + width, leftPos.y + height), currentColor, new Vector2(uv.z, uv.w));
                vh.AddVert(new Vector3(leftPos.x + width, leftPos.y), currentColor, new Vector2(uv.z, uv.y));

                vh.AddTriangle(count, count + 1, count + 2);
                vh.AddTriangle(count + 2, count + 3, count);
            }
        }
    }
}
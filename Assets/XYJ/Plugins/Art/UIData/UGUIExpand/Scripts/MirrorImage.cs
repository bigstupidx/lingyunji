using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    public class MirrorImage : Image
    { 
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : Sprites.DataUtility.GetPadding(overrideSprite);
            var size = overrideSprite == null ? Vector2.zero : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            Rect r = GetPixelAdjustedRect();
            // Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            v = new Vector4(
                    r.x + r.width * v.x,
                    r.y + r.height * v.y,
                    r.x + r.width * v.z,
                    r.y + r.height * v.w
                    );

            return v;
        }

        // 镜像
        void GenerateSimpleMirrorSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            var uv = (overrideSprite != null) ? Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            Vector4 left = new Vector4(v.x, v.y, v.x + (v.z - v.x) / 2f, v.w);

            var color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(left.x, left.y), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(left.x, left.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(left.z, left.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(left.z, left.y), color32, new Vector2(uv.z, uv.y));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);

            Vector4 right = new Vector4(v.x + (v.z - v.x) / 2f, v.y, v.z, v.w);

            int vertCount = vh.currentVertCount;
            vh.AddVert(new Vector3(right.x, right.y), color32, new Vector2(uv.z, uv.y));
            vh.AddVert(new Vector3(right.x, right.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(right.z, right.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(right.z, right.y), color32, new Vector2(uv.x, uv.y));

            vh.AddTriangle(vertCount + 0, vertCount + 1, vertCount + 2);
            vh.AddTriangle(vertCount + 2, vertCount + 3, vertCount + 0);
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    float borderScaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }

        static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
        {
            int startIndex = vertexHelper.currentVertCount;

            for (int i = 0; i < 4; ++i)
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        static readonly Vector2[] s_VertScratch = new Vector2[4];
        static readonly Vector2[] s_UVScratch = new Vector2[4];

        static void ToMesh(VertexHelper toFill, bool fillCenter, Color color, Rect rect, Vector4 outer, Vector4 inner, Vector4 padding, Vector4 border)
        {
            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            s_VertScratch[1].x = border.x;
            s_VertScratch[1].y = border.y;
            s_VertScratch[2].x = rect.width - border.z;
            s_VertScratch[2].y = rect.height - border.w;

            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    if (!fillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;

                    AddQuad(toFill,
                        new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                        new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                        color,
                        new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                        new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y));
                }
            }
        }

        private void GenerateSlicedMirrorSprite(VertexHelper toFill)
        {
            if (!hasBorder)
            {
                GenerateSimpleMirrorSprite(toFill, false);
                return;
            }

            Vector4 outer, inner, padding, border;

            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
                padding = Sprites.DataUtility.GetPadding(overrideSprite);
                border = overrideSprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }
            toFill.Clear();

            Rect rect = GetPixelAdjustedRect();
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;

            Rect left = new Rect(rect.x, rect.y, rect.width / 2f, rect.height);
            ToMesh(toFill, fillCenter, color, left, outer, inner, padding, border);

            Rect right = new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height);

            outer = new Vector4(outer.z, outer.y, outer.x, outer.w);
            inner = new Vector4(inner.z, inner.y, inner.x, inner.w);
            ToMesh(toFill, fillCenter, color, right, outer, inner, padding, border);
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (overrideSprite == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            switch (type)
            {
            case Type.Simple:
                GenerateSimpleMirrorSprite(toFill, preserveAspect);
                break;
            case Type.Sliced:
                GenerateSlicedMirrorSprite(toFill);
                break;
            default:
                toFill.Clear();
                break;
            }
        }

        public override void SetNativeSize()
        {
            if (overrideSprite != null)
            {
                float w = overrideSprite.rect.width / pixelsPerUnit * 2;
                float h = overrideSprite.rect.height / pixelsPerUnit * 2;
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
                SetAllDirty();
            }
        }
    }
}
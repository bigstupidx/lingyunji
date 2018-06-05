using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UI
{
    [AddComponentMenu("UI/Line Image", 12)]
    public class LineImage : MaskableGraphic
    {
        protected LineImage()
        {
            useLegacyMeshGeneration = false;
            raycastTarget = false;
        }

        [PackTool.Pack]
        public Sprite m_Sprite;
        [PackTool.Pack]
        public Sprite m_LSprite;
        [PackTool.Pack]
        public Sprite m_TSprite;
        [PackTool.Pack]
        public Sprite m_XSprite;

        UIDraw mLSpriteDraw;
        UIDraw mTSpriteDraw;
        UIDraw mXSpriteDraw;

        // 点
        [SerializeField]
        List<Point> mPoints;

        public List<Point> Points
        {
            get { return mPoints; }
            set
            {
                mPoints = value;
                SetVerticesDirty();
            }
        }

        [SerializeField]
        float m_Width = 16;

        public float width
        {
            get { return m_Width; }
            set
            {
                m_Width = value;
                SetVerticesDirty();
            }
        }

        void CheckPoint(ref Point first, ref Point second)
        {
            if (first.x == second.x)
            {
                // 垂直方向
                if (first.y < second.y)
                {
                    var t = first;
                    first = second;
                    second = t;
                }
            }
            else
            {
                // 水平方向
                if (first.x > second.x)
                {
                    var t = first;
                    first = second;
                    second = t;
                }
            }
        }

        // 检测拐角
        void CheckCorner(Dictionary<Point, List<Point>> Corners)
        {
            for (int i = 1; i < mPoints.Count; i += 2)
            {
                Point fp = mPoints[i - 1];
                Point sp = mPoints[i];

                List<Point> first, second;
                Corners.TryGetValue(fp, out first);
                Corners.TryGetValue(sp, out second);

                if (first == null)
                {
                    first = new List<Point>();
                    Corners.Add(fp, first);
                }

                if (second == null)
                {
                    second = new List<Point>();
                    Corners.Add(sp, second);
                }

                if (!first.Contains(sp))
                    first.Add(sp);

                if (!second.Contains(fp))
                    second.Add(fp);
            }
        }

        void AddQuad(VertexHelper vh, Point center, float width, ref Vector4 uv)
        {
            int count = vh.currentVertCount;
            vh.AddVert(new Vector3(center.x - width, center.y + width), color, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(center.x + width, center.y + width), color, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(center.x + width, center.y - width), color, new Vector2(uv.z, uv.y));
            vh.AddVert(new Vector3(center.x - width, center.y - width), color, new Vector2(uv.x, uv.y));

            vh.AddTriangle(count + 0, count + 1, count + 2);
            vh.AddTriangle(count + 2, count + 3, count + 0);
        }

        // 拐角朝向
        enum CornerDir
        {
            Null = 0,
            LeftX = 1, // 是否有X轴左朝向
            RightX = 2, // 是否有X轴右朝向

            UpY = 4, // 是否有Y轴左朝向
            DownY = 8, // 是否有Y轴右朝向
        }

        // 判断下XY朝向
        static CornerDir CheckXY(Point p, List<Point> ps)
        {
            CornerDir dir = CornerDir.Null;
            for (int i = 0; i < ps.Count; ++i)
            {
                Point c = ps[i];
                if (p.x == c.x)
                {
                    // X轴相同，判断下Y轴在上向下
                    dir |= (c.y > p.y ? CornerDir.UpY : CornerDir.DownY);
                }
                else
                {
                    dir |= (c.x > p.x ? CornerDir.RightX : CornerDir.LeftX);
                }
            }

            return dir;
        }

        public static void AddChild(GameObject go, GameObject child)
        {
            Transform ctf = child.transform;
            ctf.SetParent(go.transform);

            ctf.localScale = Vector3.one;
            ctf.localPosition = Vector3.zero;
            ctf.localEulerAngles = Vector3.zero;

            child.layer = go.layer;
        }

        public static T Create<T>(GameObject parent) where T : MonoBehaviour
        {
            T obj = null;
#if UNITY_EDITOR
            // If we're in the editor, create the game object with hide flags set right away
            GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("", HideFlags.NotEditable | HideFlags.DontSave | HideFlags.HideInHierarchy);
            obj = go.AddComponent<T>();
            AddChild(parent, go);
            go.name = typeof(T).Name;
#else
            obj = UITools.AddChild<T>(parent);
            //obj = parent.AddChild<T>();
            obj.name = "";
#endif
            obj.transform.SetParent(parent.transform);
            return obj;
        }

        protected override void UpdateMaterial()
        {
            if (!IsActive())
                return;

            Material material = materialForRendering;

            if (m_Sprite != null)
            {
                canvasRenderer.materialCount = 1;
                canvasRenderer.SetMaterial(material, 0);
                canvasRenderer.SetTexture(m_Sprite.texture);
            }

            if (mLSpriteDraw != null)
            {
                mLSpriteDraw.UpdateMaterial(material, m_LSprite.texture);
            }

            if (mTSpriteDraw != null)
            {
                mTSpriteDraw.UpdateMaterial(material, m_TSprite.texture);
            }

            if (mXSpriteDraw != null)
            {
                mXSpriteDraw.UpdateMaterial(material, m_XSprite.texture);
            }
        }

        void CheckTwoCorners(VertexHelper vh, Dictionary<Point, List<Point>> Corners)
        {
            vh.Clear();
            Vector4 uv;
            float h = width / 2f;
            foreach (var itor in Corners)
            {
                switch (itor.Value.Count)
                {
                case 2:
                    {
                        uv = (m_LSprite != null) ? DataUtility.GetOuterUV(m_LSprite) : Vector4.zero;
                        CornerDir dir = CheckXY(itor.Key, itor.Value);
                        if ((dir == (CornerDir.LeftX | CornerDir.RightX)) || (dir == (CornerDir.UpY | CornerDir.DownY)))
                            break;

                        if (mLSpriteDraw == null)
                        {
                            mLSpriteDraw = Create<UIDraw>(gameObject);
                        }

                        if ((dir & CornerDir.LeftX) == 0)
                        {
                            float t = uv.x;
                            uv.x = uv.z;
                            uv.z = t;
                        }

                        if ((dir & CornerDir.DownY) == 0)
                        {
                            float t = uv.y;
                            uv.y = uv.w;
                            uv.w = t;
                        }

                        Point p = itor.Key;
                        Vector4 v = new Vector4(p.x - h, p.y - h, p.x - h + width, p.y - h + width);

                        var color32 = color;
                        int count = vh.currentVertCount;
                        vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y));
                        vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
                        vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w));
                        vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y));

                        vh.AddTriangle(count + 0, count + 1, count + 2);
                        vh.AddTriangle(count + 2, count + 3, count + 0);
                    }
                    break;
                }
            }

            if (mLSpriteDraw != null)
            {
                vh.FillMesh(workerMesh);
                mLSpriteDraw.FillMesh(workerMesh);
                vh.Clear();
            }
        }

        void CheckThridCorners(VertexHelper vh, Dictionary<Point, List<Point>> Corners)
        {
            vh.Clear();
            Vector4 uv;
            float h = width / 2f;
            foreach (var itor in Corners)
            {
                switch (itor.Value.Count)
                {
                case 3:
                    {
                        uv = (m_TSprite != null) ? DataUtility.GetOuterUV(m_TSprite) : Vector4.zero;
                        CornerDir dir = CheckXY(itor.Key, itor.Value);
                        if (mTSpriteDraw == null)
                            mTSpriteDraw = Create<UIDraw>(gameObject);

                        Vector2 lb = new Vector2(uv.x, uv.y);
                        Vector2 lt = new Vector2(uv.x, uv.w);
                        Vector2 rt = new Vector2(uv.z, uv.w);
                        Vector2 rb = new Vector2(uv.z, uv.y);

                        if (dir == (CornerDir.LeftX | CornerDir.RightX | CornerDir.DownY))
                        {

                        }
                        else
                        {
                            if ((dir & CornerDir.LeftX) == 0)
                            {
                                // 没有朝向左边，要旋转90度
                                lb = new Vector2(uv.x, uv.w);
                                lt = new Vector2(uv.z, uv.w);
                                rt = new Vector2(uv.z, uv.x);
                                rb = new Vector2(uv.x, uv.y);
                            }
                            else if ((dir & CornerDir.RightX) == 0)
                            {
                                lb = new Vector2(uv.z, uv.y);
                                lt = new Vector2(uv.x, uv.y);
                                rt = new Vector2(uv.x, uv.w);
                                rb = new Vector2(uv.z, uv.w);
                            }
                            else if ((dir & CornerDir.DownY) == 0)
                            {
                                lb = new Vector2(uv.x, uv.w);
                                lt = new Vector2(uv.x, uv.y);
                                rt = new Vector2(uv.z, uv.y);
                                rb = new Vector2(uv.z, uv.w);
                            }
                        }

                        Point p = itor.Key;
                        Vector4 v = new Vector4(p.x - h, p.y - h, p.x - h + width, p.y - h + width);

                        var color32 = color;
                        int count = vh.currentVertCount;
                        vh.AddVert(new Vector3(v.x, v.y), color32, lb);
                        vh.AddVert(new Vector3(v.x, v.w), color32, lt);
                        vh.AddVert(new Vector3(v.z, v.w), color32, rt);
                        vh.AddVert(new Vector3(v.z, v.y), color32, rb);

                        vh.AddTriangle(count + 0, count + 1, count + 2);
                        vh.AddTriangle(count + 2, count + 3, count + 0);
                    }
                    break;
                }
            }

            if (mTSpriteDraw != null)
            {
                vh.FillMesh(workerMesh);
                mTSpriteDraw.FillMesh(workerMesh);
                vh.Clear();
            }
        }

        void CheckFourCorners(VertexHelper vh, Dictionary<Point, List<Point>> Corners)
        {
            vh.Clear();
            Vector4 uv;
            float h = width / 2f;
            foreach (var itor in Corners)
            {
                switch (itor.Value.Count)
                {
                case 4:
                    {
                        if (mXSpriteDraw == null)
                        {
                            mXSpriteDraw = Create<UIDraw>(gameObject);
                        }

                        uv = (m_XSprite != null) ? DataUtility.GetOuterUV(m_XSprite) : Vector4.zero;
                        Point p = itor.Key;
                        Vector4 v = new Vector4(p.x - h, p.y - h, p.x - h + width, p.y - h + width);

                        var color32 = color;
                        int count = vh.currentVertCount;
                        vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y));
                        vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
                        vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w));
                        vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y));

                        vh.AddTriangle(count + 0, count + 1, count + 2);
                        vh.AddTriangle(count + 2, count + 3, count + 0);
                    }
                    break;
                }
            }

            if (mXSpriteDraw != null)
            {
                vh.FillMesh(workerMesh);
                mXSpriteDraw.FillMesh(workerMesh);
                vh.Clear();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (mLSpriteDraw != null)
                mLSpriteDraw.canvasRenderer.Clear();
            if (mTSpriteDraw != null)
                mTSpriteDraw.canvasRenderer.Clear();
            if (mXSpriteDraw != null)
                mXSpriteDraw.canvasRenderer.Clear();
            
            vh.Clear();
            if (mPoints == null || mPoints.Count == 0)
                return;

            Point first;
            Point second;
            short h = (short)(width / 2f);
            var uv = (m_Sprite != null) ? DataUtility.GetOuterUV(m_Sprite) : Vector4.zero;

            // 把拐角补齐
            Dictionary<Point, List<Point>> Corners = new Dictionary<Point, List<Point>>();
            CheckCorner(Corners);

            // 开始画出拐角
            CheckTwoCorners(vh, Corners);
            CheckThridCorners(vh, Corners);
            CheckFourCorners(vh, Corners);

            vh.Clear();
            for (int i = 1; i < mPoints.Count; i += 2)
            {
                first = mPoints[i - 1];
                second = mPoints[i];

                CheckPoint(ref first, ref second);

                List<Point> first_corners = Corners[first];
                List<Point> second_corners = Corners[second];

                if (first.x == second.x)
                {
                    Point srcFirst = first;

                    // 垂直方向
                    // 拐角不是0
                    switch (first_corners.Count)
                    {
                    case 1: break;
                    case 2:
                        {
                            Point other = first_corners.Find((Point p) => { return p.GetHashCode() != second.GetHashCode(); });
                            if (first.x == other.x)
                            {
                                // 
                            }
                            else
                            {
                                first.y -= h;
                            }

                        }
                        break;
                    case 3:
                        {
                            first.y -= h;
                        }
                        break;
                    case 4:
                        {
                            first.y -= h;
                        }
                        break;
                    default:
                        break;
                    }

                    switch (second_corners.Count)
                    {
                    case 1:
                        break;
                    case 2:
                        {
                            Point other = second_corners.Find((Point p) => { return p.GetHashCode() != srcFirst.GetHashCode(); });
                            if (second.x == other.x)
                            {

                            }
                            else
                            {
                                second.y += h;
                            }
                        }
                        break;
                    case 3:
                        {
                            second.y += h;
                        }
                        break;
                    case 4:
                        {
                            second.y += h;
                        }
                        break;
                    default:
                        break;
                    }

                    int count = vh.currentVertCount;
                    vh.AddVert(new Vector3(first.x - h, first.y), color, new Vector2(uv.x, uv.w));
                    vh.AddVert(new Vector3(first.x + h, first.y), color, new Vector2(uv.z, uv.w));
                    vh.AddVert(new Vector3(second.x + h, second.y), color, new Vector2(uv.z, uv.y));
                    vh.AddVert(new Vector3(second.x - h, second.y), color, new Vector2(uv.x, uv.y));

                    vh.AddTriangle(count + 0, count + 1, count + 2);
                    vh.AddTriangle(count + 2, count + 3, count + 0);
                }
                else
                {
                    // 水平方向
                    switch (first_corners.Count)
                    {
                    case 1: break;
                    case 2:
                        {
                            Point other = first_corners.Find((Point p) => { return p.GetHashCode() != first.GetHashCode(); });
                            if (first.x == other.x)
                            {
                                // 
                                first.x += h;
                            }
                            else
                            {

                            }

                        }
                        break;
                    case 3:
                        {
                            first.x += h;
                        }
                        break;
                    case 4:
                        {
                            first.x += h;
                        }
                        break;
                    default:
                        break;
                    }

                    switch (second_corners.Count)
                    {
                    case 1: break;
                    case 2:
                        {
                            Point other = second_corners.Find((Point p) => { return p.GetHashCode() != first.GetHashCode(); });
                            if (second.x == other.x)
                            {
                                // 
                                second.x -= h;
                            }
                            else
                            {

                            }

                        }
                        break;
                    case 3:
                        {
                            second.x -= h;
                        }
                        break;
                    case 4:
                        {
                            second.x -= h;
                        }
                        break;
                    default:
                        break;
                    }

                    int count = vh.currentVertCount;
                    vh.AddVert(new Vector3(first.x, first.y + h), color, new Vector2(uv.x, uv.y));
                    vh.AddVert(new Vector3(second.x, first.y + h), color, new Vector2(uv.x, uv.w));
                    vh.AddVert(new Vector3(second.x, first.y - h), color, new Vector2(uv.z, uv.w));
                    vh.AddVert(new Vector3(first.x, first.y - h), color, new Vector2(uv.z, uv.y));

                    vh.AddTriangle(count + 0, count + 1, count + 2);
                    vh.AddTriangle(count + 2, count + 3, count + 0);
                }
            }
        }
    }
}
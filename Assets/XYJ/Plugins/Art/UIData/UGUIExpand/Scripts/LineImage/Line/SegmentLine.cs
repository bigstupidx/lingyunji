#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI;

namespace Geometry2D
{
    // 线段
    public struct SegmentLine
    {
        Vector2 start;
        Vector2 end;
        Beeline line;

        public Vector2 Start { get { return start; } }

        public Vector2 End { get { return end; } }

        public Beeline Line { get { return line; } }

        public SegmentLine(Point s, Point e)
        {
            start.x = s.x; start.y = s.y;
            end.x = e.x; end.y = e.y;

            line = new Beeline(start, end);
        }

        public SegmentLine(Vector3 s, Vector3 e)
        {
            start.x = s.x;
            start.y = s.z;

            end.x = e.x;
            end.y = e.z;

            line = new Beeline(start, end);
        }

        public SegmentLine(Vector2 s, Vector2 e)
        {
            start = s;
            end = e;

            line = new Beeline(start, end);
        }

        public bool isIntersect(SegmentLine sl)
        {
            Vector2 point;
            return Intersect(sl, out point);
        }

        // 判断两条线段是否有相交
        public bool Intersect(SegmentLine sl, out Vector2 point)
        {
            if (!line.Intersect(sl.line, out point))
                return false;
             
            // 还需要判断下，相交的点是否都在两个线段内
            if (isContains(point) && sl.isContains(point))
            {
                return true;
            }

            return false;
        }

        static public bool IsContains(SegmentLine sl, List<SegmentLine> sls, int ex, out SegmentLine rsl)
        {
            for (int i = 0; i < sls.Count; ++i)
            {
                if (i == ex)
                    continue;

                if (sls[i].Contains(sl.start) && sls[i].Contains(sl.end))
                {
                    rsl = sls[i];
                    return true;
                }
            }

            rsl = new SegmentLine();
            return false;
        }

        // 是否有交点
        public bool Contains(Vector2 point)
        {
            if (Math2D.Equals(point, start))
                return true;

            if (Math2D.Equals(point, end))
                return true;

            float y = 0f;
            if (line.GetY(point.x, out y))
            {
                if (!Math2D.Equals(point.y, y))
                    return false;
            }
            else
            {
                // y为任意数
                if (!Math2D.Equals(point.x, (-line.c / line.a)))
                {
                    return false;
                }
            }

            if (!isContains(point))
                return false;

            return true;
        }

        public static bool operator ==(SegmentLine lhs, SegmentLine rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SegmentLine lhs, SegmentLine rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object other)
        {
            SegmentLine rhs = (SegmentLine)other;
            return (Math2D.Equals(start, rhs.start) && Math2D.Equals(end, rhs.end)) || (Math2D.Equals(end, rhs.start) && Math2D.Equals(start, rhs.end));
        }

        public override string ToString()
        {
            return string.Format("start:{0} end:{1}", start, end);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        bool isContains(Vector2 point)
        {
            float xMax = Mathf.Max(start.x, end.x) + 0.01f;
            float xMin = Mathf.Min(start.x, end.x) - 0.01f;

            float yMax = Mathf.Max(start.y, end.y) + 0.01f;
            float yMin = Mathf.Min(start.y, end.y) - 0.01f;

            if (point.x >= xMin && point.x <= xMax &&
                point.y >= yMin && point.y <= yMax)
            {
                return true;
            }

            return false;
        }
    }
}
#endif
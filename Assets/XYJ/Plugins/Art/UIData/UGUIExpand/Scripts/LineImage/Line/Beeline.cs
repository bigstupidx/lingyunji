#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Geometry2D
{
    public struct Beeline
    {
        // ax + by + c = 0; 平面直线的方程
        public float a;
        public float b;
        public float c;

        public float k
        {
            get { return -a / b; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // 两点定义一线直接
        public void Set(Vector2 x, Vector2 y)
        {
            if (Math2D.Equals(x.x, y.x))
            {
                b = 0;
                a = 1;
                c = -x.x;
            }
            else if (Math2D.Equals(x.y, y.y))
            {
                a = 0;
                b = 1;
                c = -x.y;
            }
            else
            {
                a = y.y - x.y;
                b = x.x - y.x;
                c = y.x * x.y - x.x * y.y;
            }
        }

        public Beeline(Vector3 x, Vector3 y)
        {
            a = b = c = 0f;
            Set(new Vector2(x.x, x.z), new Vector2(y.x, y.z));
        }

        public Beeline(Vector2 x, Vector2 y)
        {
            a = b = c = 0f;
            Set(x, y);
        }

        // 已知y，求x
        public bool GetX(float y, out float x)
        {
            if (Math2D.Equals(a, 0f))
            {
                x = 0;
                return false;
            }

            x = -(b * y + c) / a;
            return true;
        }

        // 已知x，求y
        public bool GetY(float x, out float y)
        {
            if (Math2D.Equals(b, 0f))
            {
                y = 0f;
                return false;
            }

            y = -(a * x + c) / b;
            return true;
        }

        // 与此线段垂直，并且通过某点的直接方程
        public Beeline GetVerticalPoint(Vector2 point)
        {
            Beeline bl = new Beeline();
            if (Math2D.Equals(b, 0f))
            {
                // 没有斜率
                bl.a = 0;
                bl.b = 1;
                bl.c = -point.y;
            }
            else
            {
                bl.a = -b;
                bl.b = a;
                bl.c = -bl.a * point.x - bl.b * point.y;
            }

            return bl;
        }

        // 与此线段平行，并且通过某点的直接方程
        public Beeline GetParallelPoint(Vector2 point)
        {
            Beeline bl = new Beeline();
            bl.a = a;
            bl.b = b;
            bl.c = -bl.a * point.x - bl.b * point.y;
            return bl;
        }

        // 得到与点start相距distance的点
        public Vector2 GetDistancePoint(Vector2 start, float distance)
        {
            if (!Math2D.Equals(b, 0f))
            {
                distance = (distance) * (float)System.Math.Cos(System.Math.Atan(k));
                Vector2 temp = start;
                temp.x += distance;
                GetY(temp.x, out temp.y);
                return temp;
            }
            else
            {
                return new Vector2(start.x, start.y + distance);
            }
        }

        // 经过点（start相距distance的点）的并且与源直线垂直的直线
        public Beeline GetDistancePointBeeline(Vector2 start, float distance)
        {
            return GetVerticalPoint(GetDistancePoint(start, distance));
        }

        // 判断两条直线是否有相交
        public bool Intersect(Beeline bl, out Vector2 point)
        {
            point = Vector2.zero;
            if (a * bl.b == b * bl.a) // 平行线永不相交
                return false;

            if (Math2D.Equals(a, 0f))
            {
                point.y = -c / b;
                point.x = -(bl.b * point.y + bl.c) / bl.a;
            }
            else if (Math2D.Equals(b, 0))
            {
                point.x = -c / a;
                point.y = -(bl.a * point.x + bl.c) / bl.b;
            }
            else if (Math2D.Equals(bl.a, 0))
            {
                point.y = -bl.c / bl.b;
                point.x = -(b * point.y + c) / a;
            }
            else if (Math2D.Equals(bl.b, 0))
            {
                point.x = -bl.c / bl.a;
                point.y = -(a * point.x + c) / b;
            }
            else
            {
                point.y = (a * bl.c - bl.a * c) / (bl.a * b - a * bl.b);
                point.x = -(bl.b * point.y + bl.c) / bl.a;
            }

            //float x, y;
            //if (GetX(point.y, out x))
            //{
            //    if (!Math2D.Equals(point.x, x))
            //    {
            //        Debug.LogErrorFormat("{0}", Mathf.Abs(point.x - x));
            //    }
            //}

            //if (GetY(point.x, out y))
            //{
            //    if (!Math2D.Equals(point.y, y))
            //    {
            //        Debug.LogErrorFormat("{0}", Mathf.Abs(point.y - y));
            //    }
            //}

            //if (bl.GetX(point.y, out x))
            //{
            //    if (!Math2D.Equals(point.x, x))
            //    {
            //        Debug.LogErrorFormat("{0}", Mathf.Abs(point.x - x));
            //    }
            //}

            //if (bl.GetY(point.x, out y))
            //{
            //    if (!Math2D.Equals(point.y, y))
            //    {
            //        Debug.LogErrorFormat("{0}", Mathf.Abs(point.y - y));
            //    }
            //}

            return true;
        }

        public override bool Equals(object other)
        {
            Beeline rhs = (Beeline)other;
            return this == rhs;
        }

        public static bool operator ==(Beeline lhs, Beeline rhs)
        {
            if (!Math2D.Equals(lhs.a, 0))
            {
                if (Math2D.Equals(rhs.a, 0))
                    return false;

                if (!Math2D.Equals(lhs.b / lhs.a,  rhs.b / rhs.a))
                    return false;

                if (!Math2D.Equals(lhs.c / lhs.a, rhs.c / rhs.a))
                    return false;
             
                return true;
            }
            else if (!Math2D.Equals(lhs.b, 0))
            {
                if (Mathf.Equals(rhs.b, 0))
                    return false;

                if (!Mathf.Equals(lhs.a / lhs.b, rhs.a / rhs.b))
                    return false;

                if (!Mathf.Equals(lhs.c / lhs.b, rhs.c / rhs.b))
                    return false;

                return true;
            }

            return Math2D.Equals(lhs.a, rhs.a) && Math2D.Equals(lhs.b, rhs.b) && Math2D.Equals(lhs.c, rhs.c);
        }

        public static bool operator !=(Beeline lhs, Beeline rhs)
        {
            return !(lhs == rhs);
        }
    }
}
#endif
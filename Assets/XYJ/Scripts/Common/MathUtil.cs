using System.IO;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    public static class MathUtil
    {
        //知道两个点，求矩形
        public static Rect GetRectByTwoPoint(Vector2 p1, Vector2 p2)
        {
            return new Rect(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y), Mathf.Abs(p1.x - p2.x), Mathf.Abs(p1.y - p2.y));
        }

        //知道中点，求矩形
        public static Rect GetRectByCenter(float centerX, float centerY, float width, float height)
        {
            return new Rect(centerX - width / 2, centerY - height / 2, width, height);
        }

        public static Rect GetRectInWorldSpace(RectTransform rt)
        {
            Vector3 vMin = rt.TransformPoint(rt.rect.min);
            Vector3 vMax = rt.TransformPoint(rt.rect.max);
            return GetRectByTwoPoint(vMin, vMax);
        }
        public static Vector3 GetCenterInWorldSpace(RectTransform rt)
        {
            Vector3 vMin = rt.TransformPoint(rt.rect.min);
            Vector3 vMax = rt.TransformPoint(rt.rect.max);
            return GetRectByTwoPoint(vMin, vMax).center;
        }

        //包围盒是不是超出边框了
        public static bool IsOutsideOfBox(Rect box, Rect bound)
        {
            
            return bound.xMin < box.xMin || bound.xMax > box.xMax || bound.yMin < box.yMin || bound.yMax > box.yMax;
        }

        //已知边框和包围盒大小，自动选择位置保证包围盒在边框内,注意这里返回的是包围盒的中心点
        public static Vector2 GetPosInsideBox(Rect box, Rect bound)
        {
            if (!IsOutsideOfBox(box, bound))
                return bound.center;
            else
                return GetPosOfBoxEdge(box, bound);
        }

        static float[] diss = new float[4];

        //已知边框和包围盒大小，自动选择位置保证包围盒在边框的边缘
        public static Vector2 GetPosOfBoxEdge(Rect box, Rect bound)
        {
            //先缩小一圈，方便计算
            box = GetRectByCenter(box.center.x, box.center.y, box.width- bound.width, box.height- bound.height);

            Vector2 newCenter = bound.center;
            //在四个角上
            if (newCenter.x <= box.xMin && newCenter.y <= box.yMin)
                return new Vector2(box.xMin, box.yMin);
            else if (newCenter.x <= box.xMin && newCenter.y >= box.yMax)
                return new Vector2(box.xMin, box.yMax);
            else if (newCenter.x >= box.xMax && newCenter.y >= box.yMax)
                return new Vector2(box.xMax, box.yMax);
            else if(newCenter.x >= box.xMax && newCenter.y <= box.yMin)
                return new Vector2(box.xMax, box.yMin);

            //在四条边外面
            if (newCenter.x <= box.xMin )
                return new Vector2(box.xMin, newCenter.y);
            else if (newCenter.x >= box.xMax)
                return new Vector2(box.xMax, newCenter.y);
            else if (newCenter.y<=box.yMin)
                return new Vector2(newCenter.x, box.yMin);
            else if (newCenter.y>= box.yMax)
                return new Vector2(newCenter.x, box.yMax);

            //在里面，那么找4条边中最近的,注意这个算法取一次没有问题，update中不断取的话，可能会有抖动
            diss[0] = Mathf.Abs(newCenter.x - box.xMin);
            diss[1] = Mathf.Abs(newCenter.x - box.xMax);
            diss[2] = Mathf.Abs(newCenter.y - box.yMin);
            diss[3] = Mathf.Abs(newCenter.y - box.yMax);
            int closeest = 0;
            float tem = float.MaxValue;
            for (int i=0;i<diss.Length;++i)
            {
                if(diss[i]< tem)
                {
                    tem = diss[i];
                    closeest = i;
                }
            }
            switch (closeest)
            {
                case 0:newCenter.x = box.xMin;break;
                case 1: newCenter.x = box.xMax; break;
                case 2: newCenter.y = box.yMin; break;
                case 3: newCenter.y = box.yMax; break;
            }
            
            return newCenter;
        }


        //已知边框和包围盒大小，获取到下边缘的投影，用这个点就不会有GetPosOfBoxEdge的抖动问题，这里取ui坐标(即yMin为下边)
        public static Vector2 GetPosOfBoxYMin(Rect box, Rect bound)
        {
            //先缩小一圈，方便计算
            box = GetRectByCenter(box.center.x, box.center.y, box.width - bound.width, box.height - bound.height);

            return new Vector2(Mathf.Clamp(bound.center.x, box.xMin, box.xMax), box.yMin);
        }
    }
}
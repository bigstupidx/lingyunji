#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Geometry2D;
using UI;
using XTools;

public static class Segment
{
    public static Point V2P(this Vector2 v)
    {
        return new Point((short)v.x, (short)v.y);
    }

    static public List<Pair<Point, Point>> Split(List<Pair<Point, Point>> segments)
    {
        List<SegmentLine> segmentLines = new List<SegmentLine>(segments.Count);
        for (int i = 0; i < segments.Count; ++i)
            segmentLines.Add(new SegmentLine(segments[i].first, segments[i].second));

        List<SegmentLine> results = Split(segmentLines);
        List<Pair<Point, Point>> r = new List<Pair<Point, Point>>(results.Count);
        for (int i = 0; i < results.Count; ++i)
        {
            r.Add(new Pair<Point, Point>(results[i].Start.V2P(), results[i].End.V2P()));
        }

        return r;
    }

    static bool ListSet(List<Vector2> l, Vector2 v)
    {
        for (int i = 0; i < l.Count; ++i)
        {
            if (Math2D.Equals(l[i], v))
                return false;
        }

        l.Add(v);
        return true;
    }

    static bool IsInSegments(List<SegmentLine> sls, SegmentLine sl)
    {
        for (int i = 0; i < sls.Count; ++i)
        {
            if (sls[i].Contains(sl.Start) && sls[i].Contains(sl.End))
                return true;
        }

        return false;
    }

    static List<SegmentLine> Split(List<SegmentLine> sls)
    {
        List<Vector2> allPoints = new List<Vector2>();
        for (int i = 0; i < sls.Count; ++i)
        {
            ListSet(allPoints, sls[i].Start);
            ListSet(allPoints, sls[i].End);

            // 再求下相交的点
            for (int j = i + 1; j < sls.Count; ++j)
            {
                Vector2 point;
                if (sls[i].Intersect(sls[j], out point))
                {
                    ListSet(allPoints, point);
                }
            }
        }

        //Debug.LogFormat("point:{0}", allPoints.Count);
        List<SegmentLine> dsl = new List<SegmentLine>();
        for (int i = 0; i < sls.Count; ++i)
        {
            SegmentLine sl = sls[i];
            List<Vector2> segPoints = new List<Vector2>();
            for (int m = 0; m < allPoints.Count; ++m)
            {
                if (sl.Contains(allPoints[m]))
                    segPoints.Add(allPoints[m]);
            }

            segPoints.Sort((Vector2 x, Vector2 y) => 
            {
                if (x.x == y.x)
                    return x.y.CompareTo(y.y);

                return x.x.CompareTo(y.x);
            });

            for (int m = 1; m < segPoints.Count; ++m)
            {
                SegmentLine s = new SegmentLine(segPoints[m - 1], segPoints[m]);
                if (dsl.Contains(s))
                    continue;

                dsl.Add(s);
            }
        }
        
        return dsl;
    }
}
#endif
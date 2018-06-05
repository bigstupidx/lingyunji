#if UNITY_EDITOR
using XTools;
using UnityEngine;
using System.Collections.Generic;

namespace UI
{
    public class TestLine : MonoBehaviour
    {
        [SerializeField]
        LineImage lineImage;

        void Test()
        {
            List<Pair<Point, Point>> points = new List<Pair<Point, Point>>();
            for (int i = 0; i < 5; ++i)
            {
                Pair<Point, Point> pp = new Pair<Point, Point>(new Point((short)(i * 100), 0), new Point((short)(i * 100), 500));
                points.Add(pp);
            }

            for (int i = 0; i < 5; ++i)
            {
                Pair<Point, Point> pp = new Pair<Point, Point>(new Point(0, (short)(i * 100)), new Point(500, (short)(i * 100)));
                points.Add(pp);
            }

            lineImage.Points = new List<Point>();
            points = Segment.Split(points);

            int count = Random.Range(0, points.Count - 2);
            for (int i = 0; i < count; ++i)
            {
                points.RemoveAt(Random.Range(0, points.Count - 1));
            }

            for (int i = 0; i < points.Count; ++i)
            {
                lineImage.Points.Add(points[i].first);
                lineImage.Points.Add(points[i].second);
            }
            lineImage.SetAllDirty();
        }

        // Use this for initialization
        void Start()
        {
            Test();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Test();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                lineImage.Points.Clear();
                //lineImage.Points.Add(new Point(0, 0));
                //lineImage.Points.Add(new Point(0, 100));
                //lineImage.Points.Add(new Point(0, 0));
                //lineImage.Points.Add(new Point(100, 0));

                //lineImage.Points.Add(new Point(0, 0));
                //lineImage.Points.Add(new Point(0, -100));

                //lineImage.Points.Add(new Point(300, 300));
                //lineImage.Points.Add(new Point(300, 200));

                //lineImage.Points.Add(new Point(300, 300));
                //lineImage.Points.Add(new Point(300, 400));

                lineImage.Points.Add(new Point(300, 300));
                lineImage.Points.Add(new Point(400, 300));

                lineImage.Points.Add(new Point(300, 300));
                lineImage.Points.Add(new Point(200, 300));

                lineImage.Points.Add(new Point(300, 300));
                lineImage.Points.Add(new Point(300, 400));

                //lineImage.Points.Add(new Point(300, 300));
                //lineImage.Points.Add(new Point(300, 200));

                //lineImage.Points.Add(new Point(300, 300));
                //lineImage.Points.Add(new Point(300, 400));

                //lineImage.Points.Add(new Point(300, 300));
                //lineImage.Points.Add(new Point(400, 300));

                lineImage.SetAllDirty();
            }
        }
    }
}
#endif
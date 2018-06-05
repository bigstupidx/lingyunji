using UnityEngine;
using System.Collections;


namespace Eyesblack.FX {
    [AddComponentMenu("Eyesblack/FX/Helix Path")]
    public class HelixPath : CurvePathBase {
#if UNITY_EDITOR
        public float _topRadius = 0;
        public float _middleRadius = 1;
        public float _length = 5;
        public int _turns = 3;

        const int SEGMENTS = 12;

        public override float StraightLength { get { return _length; } }


        public override void CreatePathPoints() {
            base.CreatePathPoints();

            int middleIndex = _turns * SEGMENTS / 2;
            int index = 0;
            for (int i = 0; i < _turns; i++) {
                for (int j = 0; j < SEGMENTS; j++) {
                    float angle = 360 / SEGMENTS * j * Mathf.Deg2Rad;
                    float radius = Mathf.Lerp(_middleRadius, _topRadius, Mathf.Abs(index - middleIndex) / (float)middleIndex);
                    float z = radius * Mathf.Cos(angle);
                    float y = radius * Mathf.Sin(angle);
                    float x = _length / _turns / SEGMENTS * index;

                    PathPoint pathPoint = new PathPoint();
                    pathPoint._position = new Vector3(x, y, z);
                    pathPoint._percent = (float)index / (_turns * SEGMENTS);

                    index++;

                    _pathPoints.Add(pathPoint);
                }
            }


            {
                float z = _topRadius;
                float y = 0;
                float x = _length;

                PathPoint pathPoint = new PathPoint();
                pathPoint._position = new Vector3(x, y, z);
                pathPoint._percent = 1;

                _pathPoints.Add(pathPoint);
            }


            for (int i = 1; i < _pathPoints.Count; i++) {
                _pathLength += Vector3.Magnitude(_pathPoints[i]._position - _pathPoints[i - 1]._position);
            }

        }
#endif
    }
}
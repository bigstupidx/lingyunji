using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Eyesblack.FX {
    public abstract class CurvePathBase : MonoBehaviour {
        [System.Serializable]
        public struct PathPoint {
            public Vector3 _position;
            public float _percent;
        }

        [HideInInspector]
        public List<PathPoint> _pathPoints;
        [HideInInspector]
        public float _pathLength;

        // public float PathLength { get { return _pathLength * transform.lossyScale.x; } }

        public virtual float StraightLength {
            get {
                Debug.LogError("this base method should not be called");
                return 0;
            }
        }


        public Vector3 GetPosition(float percent) {
            percent = Mathf.Clamp(percent, 0, 1);

            Vector3 finalPos = Vector3.zero;
            if (percent == 0)
                finalPos = _pathPoints[0]._position;
            else if (percent == 1)
                finalPos = _pathPoints[_pathPoints.Count - 1]._position;

            else {
                for (int i = 1; i < _pathPoints.Count; i++) {
                    if (_pathPoints[i]._percent >= percent) {
                        PathPoint prePoint = _pathPoints[i - 1];
                        finalPos = Vector3.Lerp(prePoint._position, _pathPoints[i]._position, (percent - prePoint._percent) / (_pathPoints[i]._percent - prePoint._percent));
                        break;
                    }
                }
            }

            finalPos = transform.TransformPoint(finalPos);

            return finalPos;
        }


#if UNITY_EDITOR
    public virtual void CreatePathPoints() {
        if (_pathPoints == null)
            _pathPoints = new List<PathPoint>();
        _pathPoints.Clear();
        _pathLength = 0;
    }
#endif


        void OnDrawGizmos() {
            if (_pathPoints == null)
                return;

            Gizmos.matrix = transform.localToWorldMatrix;
            for (int i = 0; i < _pathPoints.Count - 1; i++) {
                Vector3 p1 = _pathPoints[i]._position;
                Vector3 p2 = _pathPoints[i + 1]._position;
                Gizmos.DrawLine(p1, p2);
            }
        }
    }
}

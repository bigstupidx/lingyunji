using UnityEngine;
using System.Collections;

namespace Eyesblack.FX {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]

    [AddComponentMenu("Eyesblack/FX/Deformed Follow Path Mesh")]
    public class DeformedFollowPathMesh : MonoBehaviour {
        public delegate void HitTargetDelegate(GameObject go);
        public HitTargetDelegate OnHitTarget { private get; set; }

        public CurvePathBase Path { private get; set; }

        public float MoveSpeed { private get; set; }

        public float _width = 0.5f;
        public float _length = 2;
        public int _segments = 5;
        public bool _faceCamera = true;

        float _currentPosition = 0;
        bool _end = false;

        Mesh _mesh;
        Vector3[] _vertices;

        void Awake() {
            CreateMesh();
        }

        void OnEnable() {
            _currentPosition = 0;
            _end = false;
        }

        void CreateMesh() {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            _mesh.MarkDynamic();

            _vertices = new Vector3[_segments * 2];
            Vector2[] uv = new Vector2[_segments * 2];
            int[] triangles = new int[(_segments - 1) * 2 * 3];

            for (int i = 0; i < _segments; i++) {
                //_vertices[i * 2 + 0] = new Vector3((_length / (_segments - 1)) * i, -_width / 2, 0);
                //_vertices[i * 2 + 1] = new Vector3((_length / (_segments - 1)) * i, _width / 2, 0);
                uv[i * 2 + 0] = new Vector2((float)i / (_segments - 1), 0);
                uv[i * 2 + 1] = new Vector2((float)i / (_segments - 1), 1);
            }

            for (int i = 0; i < _segments - 1; i++) {
                triangles[i * 6 + 0] = 0 + i * 2;
                triangles[i * 6 + 1] = 1 + i * 2;
                triangles[i * 6 + 2] = 2 + i * 2;
                triangles[i * 6 + 3] = 2 + i * 2;
                triangles[i * 6 + 4] = 1 + i * 2;
                triangles[i * 6 + 5] = 3 + i * 2;
            }

            _mesh.vertices = _vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
        }

        void Update() {
            if (Camera.main == null || Path == null || _end)
                return;

            Vector3 camUpDir = Vector3.up;
            if (_faceCamera)
                camUpDir = Camera.main.transform.up;
            float _lengthPerSegment = (float)_length / (_segments - 1);
            for (int i = 0; i < _segments; i++) {
                float percent = _currentPosition - (_lengthPerSegment / Path._pathLength) * i;
                Vector3 position = Path.GetPosition(percent);
                _vertices[i * 2 + 0] = position - _width / 2 * camUpDir;
                _vertices[i * 2 + 1] = position + _width / 2 * camUpDir;

                _vertices[i * 2 + 0] = transform.InverseTransformPoint(_vertices[i * 2 + 0]);
                _vertices[i * 2 + 1] = transform.InverseTransformPoint(_vertices[i * 2 + 1]);

                if (i == _segments - 1 && percent >= 1) {
                    _end = true;
                }
            }

            _mesh.vertices = _vertices;
            _mesh.RecalculateBounds();

            _currentPosition += MoveSpeed / Path._pathLength * Time.deltaTime;

            if (_end) {
                if (OnHitTarget != null) {
                    OnHitTarget(gameObject);
                }
            }
        }

    }
}
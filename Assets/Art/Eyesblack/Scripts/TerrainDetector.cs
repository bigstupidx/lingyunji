using UnityEngine;
using System.Collections;

namespace Eyesblack.FX {
    [AddComponentMenu("Eyesblack/FX/Terrain Detector")]
    public class TerrainDetector : MonoBehaviour {
		public LayerMask _layerMask = 1 | (1 << 30); // Default & Scene
        public float _width = 1;
        public float _height = 1;

        const float _accuracy = 1;
        Vector3 _lastPosition = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);

        float[,] _heightmap;


        public float GetHeight(Vector3 posWorld) {
            Vector3 curPosition = transform.position;

            int xIndex = Mathf.FloorToInt((posWorld.x - curPosition.x + _width / 2) / _accuracy);
            int yIndex = Mathf.FloorToInt((posWorld.z - curPosition.z + _height / 2) / _accuracy);
            if (xIndex < 0 || xIndex >= _heightmap.GetLength(0) - 1)
                return 0;
            if (yIndex < 0 || yIndex >= _heightmap.GetLength(1) - 1)
                return 0;

            float xFactor = (posWorld.x - (curPosition.x - _width / 2 + xIndex * _accuracy)) / _accuracy;
            float yFactor = (posWorld.z - (curPosition.z - _height / 2 + yIndex * _accuracy)) / _accuracy;


            float leftBottomHeight = _heightmap[xIndex, yIndex];
            float rightBottomHeight = _heightmap[xIndex + 1, yIndex];
            float leftTopHeight = _heightmap[xIndex, yIndex + 1];
            float rightTopHeight = _heightmap[xIndex + 1, yIndex + 1];

            float bottomHeight = Mathf.NegativeInfinity;
            if (leftBottomHeight == Mathf.NegativeInfinity && rightBottomHeight == Mathf.NegativeInfinity) { }
            else if (leftBottomHeight == Mathf.NegativeInfinity)
                bottomHeight = rightBottomHeight;
            else if (rightBottomHeight == Mathf.NegativeInfinity)
                bottomHeight = leftBottomHeight;
            else
                bottomHeight = Mathf.Lerp(_heightmap[xIndex, yIndex], _heightmap[xIndex + 1, yIndex], xFactor);


            float topHeight = Mathf.NegativeInfinity;
            if (leftTopHeight == Mathf.NegativeInfinity && rightTopHeight == Mathf.NegativeInfinity) { }
            else if (leftTopHeight == Mathf.NegativeInfinity)
                topHeight = rightTopHeight;
            else if (rightTopHeight == Mathf.NegativeInfinity)
                topHeight = leftTopHeight;
            else
                topHeight = Mathf.Lerp(_heightmap[xIndex, yIndex + 1], _heightmap[xIndex + 1, yIndex + 1], xFactor);


            float height = Mathf.NegativeInfinity;
            if (bottomHeight == Mathf.NegativeInfinity && topHeight == Mathf.NegativeInfinity) { }
            else if (bottomHeight == Mathf.NegativeInfinity)
                height = topHeight;
            else if (topHeight == Mathf.NegativeInfinity)
                height = bottomHeight;
            else
                height = Mathf.Lerp(bottomHeight, topHeight, yFactor);


            return height == Mathf.NegativeInfinity ? 0 : height - transform.position.y;
        }

        void Start() {
            foreach (CloseToTerrain ct in GetComponentsInChildren<CloseToTerrain>()) {
				if (ct.enabled) {
					ct.Detector = this;
					ct.ReplaceMesh();
				}
            }

            Vector3 curPosition = transform.position;
            UpdateGridInfos(curPosition);
            _lastPosition = curPosition;
        }

        void OnBecameVisible() {
            enabled = true;
        }

        void OnBecameInvisible() {
            enabled = false;
        }

        void Update() {
            Vector3 curPosition = transform.position;
            if (Mathf.Abs(curPosition.x - _lastPosition.x) > Mathf.Epsilon || Mathf.Abs(curPosition.z - _lastPosition.z) > Mathf.Epsilon) {
                UpdateGridInfos(curPosition);
            }

            _lastPosition = curPosition;
        }

        void UpdateGridInfos(Vector3 curPosition) {
            int xCount = _heightmap != null ? _heightmap.GetLength(0) : 0;
            int yCount = _heightmap != null ? _heightmap.GetLength(1) : 0;

            int xCountNew = Mathf.CeilToInt(_width / _accuracy);
            int yCountNew = Mathf.CeilToInt(_height / _accuracy);

            if (xCount != xCountNew || yCount != yCountNew) {
                _heightmap = new float[xCountNew, yCountNew];
            }

            int curXIndex = 0;
            int curYIndex = 0;
            for (float x = curPosition.x - _width / 2; curXIndex < xCountNew; x += _accuracy, curXIndex++) {
                curYIndex = 0;
                for (float z = curPosition.z - _height / 2; curYIndex < yCountNew; z += _accuracy, curYIndex++) {
                    Vector3 position = new Vector3(x, curPosition.y + 5, z);
                    RaycastHit hitInfo;
					if (Physics.Raycast (position, Vector3.down, out hitInfo, Mathf.Infinity, _layerMask))
						_heightmap [curXIndex, curYIndex] = hitInfo.point.y;
                    else
                        _heightmap[curXIndex, curYIndex] = Mathf.NegativeInfinity;
                }
            }
        }


        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;

            Vector3 curPosition = transform.position;
            Vector3 v1 = curPosition + new Vector3(-_width / 2, 0.1f, -_height / 2);
            Vector3 v2 = curPosition + new Vector3(-_width / 2, 0.1f, _height / 2);
            Vector3 v3 = curPosition + new Vector3(_width / 2, 0.1f, _height / 2);
            Vector3 v4 = curPosition + new Vector3(_width / 2, 0.1f, -_height / 2);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v4);
            Gizmos.DrawLine(v4, v1);
        }

    }
}


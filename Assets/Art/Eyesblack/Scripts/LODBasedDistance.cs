using UnityEngine;
using System.Collections;


namespace Eyesblack.Optimizations {
    [AddComponentMenu("Eyesblack/Optimizations/LOD Based On Distance")]
    public class LODBasedDistance : MonoBehaviour {
        static bool _enabled = true;
        public static bool Enabled { private get { return _enabled; } set { _enabled = value; } }

        public enum DistanceLevel {
            High = 0,
            Medium,
            Low,
            Lowest
        }

        public DistanceLevel _distanceLevel;

        
        Renderer _renderer;
        Vector3 _center;
        float _radius;

        void Awake() {
            _renderer = this.GetComponent<Renderer>();

            if (_renderer != null) {
                _center = _renderer.bounds.center;
                _radius = Vector3.Magnitude(_renderer.bounds.max - _renderer.bounds.min) / 2;
            }
        }

        void Start() {
            HandleLOD();
            InvokeRepeating("HandleLOD", Random.Range(1, 3), 1);
        }

        void HandleLOD() {
            if (Enabled) {
                if (_renderer != null && Camera.main != null) {
                    float maxViewDistance = GraphicsDetails.ObjectViewDistance[GraphicsDetails.ObjectDetailLevel, (int)_distanceLevel];
                    bool enabled = Vector3.Magnitude(Camera.main.transform.position - _center) - _radius < maxViewDistance;
                    _renderer.enabled = enabled;
                }
            } else {
                if (_renderer != null) {
                    if (!_renderer.enabled)
                        _renderer.enabled = true;
                }
            }
        }
    }
}

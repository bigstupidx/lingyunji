using UnityEngine;



namespace Eyesblack {
    [AddComponentMenu("Eyesblack/Misc/ShowFps")]

    [ExecuteInEditMode]
    [RequireComponent(typeof(GUILayer))]
    public class ShowFps : MonoBehaviour {
        public Vector3 _position = new Vector3(0, 0.6f, 0);
        public int _fontSize = 30;
        private GUIText _gui;

        private float _updateInterval = 1.0f;
        private float _lastInterval;
        private int _frames = 0;


        void Start() {
#if !UNITY_EDITOR
#if UNITY_IPHONE || UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif
#endif

            _lastInterval = Time.realtimeSinceStartup;
            _frames = 0;
        }

        void OnDisable() {
            if (_gui)
                DestroyImmediate(_gui.gameObject);
        }

        void Update() {
            ++_frames;
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > _lastInterval + _updateInterval) {
                if (!_gui) {
                    GameObject go = new GameObject("FPS Display");
                    go.hideFlags = HideFlags.HideAndDontSave;
                    go.transform.position = _position;
                    _gui = go.AddComponent<GUIText>();
                    _gui.pixelOffset = new Vector2(5, 55);
                    _gui.fontSize = _fontSize;
                    _gui.color = new Color(1.0f, 0.0f, 0.0f);
                }
                float fps = _frames / (timeNow - _lastInterval);
                float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
                _gui.text = ms.ToString("f1") + "ms " + fps.ToString("f2") + "FPS";
                _frames = 0;
                _lastInterval = timeNow;
            }

        }
    }
}




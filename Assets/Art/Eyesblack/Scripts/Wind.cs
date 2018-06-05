using UnityEngine;
using System.Collections;

namespace Eyesblack.Nature {
    [AddComponentMenu("Eyesblack/Nature/Wind")]

    [ExecuteInEditMode]
    public class Wind : MonoBehaviour {
        public float _frequency = 1.0f;
        public float _strength = 0.5f;

        private Transform _tr;

        void Awake() {
            _tr = this.transform;
        }


        void Update() {
            Vector3 fr = _tr.forward;
            float factor = Mathf.Sin(Time.time * _frequency);
            Shader.SetGlobalVector("_Wind", new Vector4(fr.x, fr.y, fr.z, _strength * factor));
        }
    }
}


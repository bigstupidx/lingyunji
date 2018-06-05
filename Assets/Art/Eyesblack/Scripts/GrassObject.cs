using UnityEngine;
using System.Collections;

namespace Eyesblack.Nature {
    [AddComponentMenu("")]
    public class GrassObject : MonoBehaviour {
        public Material _material;
        public int _x { get { return Mathf.FloorToInt(transform.position.x); } }
        public int _z { get { return Mathf.FloorToInt(transform.position.z); } }
    }
}

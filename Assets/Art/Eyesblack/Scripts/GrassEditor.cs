using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Eyesblack.Nature {
    [AddComponentMenu("Eyesblack/Nature/Grass Editor")]

    [ExecuteInEditMode]
    public class GrassEditor : MonoBehaviour {
        public int _layerMask = -1;

        public int _brushSize = 5;
        public int _density = 5;
        public bool _isKeepInDeep = false;
        public float _scaleInLightmap = 0.5f;

        public Mesh _grassMesh;
        public GameObject _burshProjectorPrefab;
        public Texture2D _buttonOnTex;
        public Texture2D _buttonOffTex;

        public List<GrassPrototype> _prototypes = new List<GrassPrototype>();


        void Start() {
            this.gameObject.tag = "EditorOnly";
        }

    }

}


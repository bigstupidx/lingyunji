using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Eyesblack.FX {
    [AddComponentMenu("")]
    public class GhostRenderer : MonoBehaviour {
        public float _intervalTime;
        public float _lifeTime;
        public float _minDistance;
       // public Color _rimColor;
		public Material _material;
		public string _colorKey;
        //public UnityEngine.Rendering.BlendMode _srcBlendMode;
        //public UnityEngine.Rendering.BlendMode _dstBlendMode;

		public bool GenerateEnabled { private get; set; }
		public bool GenerateFinished { get; set; }


		Renderer _renderer;
        SkinnedMeshRenderer _skinnedMeshRenderer;
        Mesh _sharedMesh;
       // Material[] _materals;

        float _lastCreateTime = 0;
		bool _firstCreate = false;
        Vector3 _lastPostion = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);

        public class GhostInfo {
            public Mesh _mesh;
            public MaterialPropertyBlock _matBlock;
            public Vector3 _pos;
            public Matrix4x4 _matrix;
            public float _life;
            public bool _used;
        }

        List<GhostInfo> _ghostInfos = new List<GhostInfo>();


        void Start() {
            _renderer = GetComponent<Renderer>();
			if (_renderer == null) {
				enabled = false;
                return;
            }



			_skinnedMeshRenderer = _renderer as SkinnedMeshRenderer;
            if (_skinnedMeshRenderer != null) {
                //_sharedMesh = skinRenderer.sharedMesh;
            } else {
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                if (meshFilter == null) {
                    enabled = false;
                    return;
                }

                _sharedMesh = meshFilter.sharedMesh;
            }

			/*
            _materals = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < _materals.Length; i++) {
                _materals[i] = new Material(Shader.Find("Hidden/Eyesblack/ColoredRim"));
                _materals[i].SetColor("_RimColor", _rimColor);
                _materals[i].SetInt("_SrcBlend", (int)_srcBlendMode);
                _materals[i].SetInt("_DstBlend", (int)_dstBlendMode);
            }
            */
        }

        /*
        void OnDisable() {
            _ghostInfos.Clear();
        }
        */

        GhostInfo NewGhostInfo() {
            for (int i = 0; i < _ghostInfos.Count; i++) {
                if (!_ghostInfos[i]._used) {
                    _ghostInfos[i]._used = true;
                    return _ghostInfos[i];
                }
            }

            GhostInfo info = new GhostInfo();
            info._mesh = new Mesh();
            info._matBlock = new MaterialPropertyBlock();
            info._used = true;
            _ghostInfos.Add(info);
            return info;
        }


        void LateUpdate() {
#if UNITY_EDITOR
			/*
            for (int i = 0; i < _materals.Length; i++) {
                _materals[i].SetInt("_SrcBlend", (int)_srcBlendMode);
                _materals[i].SetInt("_DstBlend", (int)_dstBlendMode);
            }
            */
#endif


            if (GenerateEnabled && Time.time - _lastCreateTime > _intervalTime) {
                if (Vector3.SqrMagnitude(transform.position - _lastPostion) > _minDistance * _minDistance) {
                    _lastCreateTime = Time.time;

                    GhostInfo info = NewGhostInfo();
                    if (_skinnedMeshRenderer != null) {
                        _skinnedMeshRenderer.BakeMesh(info._mesh);
                    } else {
                        info._mesh = _sharedMesh;
                    }
                    info._pos = transform.position;
                    info._matrix = transform.localToWorldMatrix;
                    info._life = _lifeTime;

                    _lastPostion = transform.position;

					if (!_firstCreate)
						_firstCreate = true;
                }
            }

            bool noneUsed = true;
            for (int i = _ghostInfos.Count - 1; i >= 0; i--) {
                GhostInfo info = _ghostInfos[i];
                info._life -= Time.deltaTime;
                if (info._life < 0) {
                    info._used = false;
                } else {
                    noneUsed = false;

					//Color c = _rimColor;
					//c.a *= info._life / _lifeTime;
					//info._matBlock.SetColor("_RimColor", c);
					if (!string.IsNullOrEmpty (_colorKey)) {
						Color c = _material.GetColor (_colorKey);
						c.a *= info._life / _lifeTime;
						info._matBlock.SetColor(_colorKey, c);
					}
                }
            }

            if (_ghostInfos.Count > 0 && noneUsed) {
                _lastPostion = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);
            }

			if (_firstCreate && noneUsed) {
				GenerateFinished = true;
			}


            for (int i = 0; i < _ghostInfos.Count; i++) {
                GhostInfo info = _ghostInfos[i];
                if (info._used) {
                    if (Vector3.SqrMagnitude(info._pos - transform.position) > 0.25f) {
                        int lenght = _renderer.sharedMaterials.Length;
                        int subMesh = Mathf.Min(info._mesh.subMeshCount, lenght); 
						for (int j = 0; j < subMesh; j++) {
                            Graphics.DrawMesh(info._mesh, info._matrix, _material, gameObject.layer, null, j, info._matBlock);
                        }
                    }
                }
            }
        }
    }

}

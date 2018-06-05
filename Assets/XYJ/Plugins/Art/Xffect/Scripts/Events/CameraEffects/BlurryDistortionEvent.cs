using UnityEngine;
using System.Collections;


namespace Xft {
    public class BlurryDistortionEvent : CameraEffectEvent {

        [Range(0, 2)]
        private int downsample = 1;
        //[Range(0, 10.0f)]
        //public float blurSize = 1.3f;
        [Range(1, 4)]
        private int blurIterations = 1;

        //public Texture2D _distortTextrue;
        //public float _distortTextrueScale = 3;
        //[Range(0, 0.05f)]
        //public float _distortIntensity = 0.01f;



        Camera _camera;
        bool _supported;
        string _shaderName;
        Shader _shader;
        Material _material;


        public GameObject _targetObject;
        GameObject _oldTargetObject;
        //[Range(0, 0.5f)]
        //public float _outlineWidth = 0.05f;

        private Renderer[] _renderers = null;

        private static RenderTexture _maskTexture;

        Material _outlineMaterial;
        Mesh[] _meshes = null;



        public BlurryDistortionEvent(XftEventComponent owner)
            : base(CameraEffectEvent.EType.BlurryDistortion, owner)
        {
            _shader = owner.BlurryDistortionShader;


            if (_maskTexture == null)
                _maskTexture = new RenderTexture(Screen.width / 4, Screen.height / 4, 0);
            if (_outlineMaterial == null)
                _outlineMaterial = new Material(Shader.Find("Hidden/Eyesblack/ColoredOutline"));
        }

        public override void Initialize() {
            base.Initialize();
            _targetObject = m_owner.CallerGo;
        }

        public override void PlayEvent() {
            _targetObject = m_owner.CallerGo;
        }

        public override bool CheckSupport() {
            bool ret = true;
            if (!SystemInfo.supportsImageEffects)
                ret = false;

            if (_material == null) {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }

            if (!_shader.isSupported)
                ret = false;

            return ret;
        }

#if UNITY_EDITOR
        public override void Update(float deltaTime) {
            m_elapsedTime += deltaTime;

            if (_targetObject != _oldTargetObject) {
                if (_targetObject != null) {
                    _renderers = _targetObject.GetComponentsInChildren<Renderer>();
                    if ((_renderers.Length > 0 && _meshes == null) || (_renderers.Length != _meshes.Length)) {
                        _meshes = new Mesh[_renderers.Length];
                        for (int i = 0; i < _renderers.Length; i++) {
                            _meshes[i] = new Mesh();
                        }
                    }
                } else {
                    _renderers = null;
                    _meshes = null;
                }


                _oldTargetObject = _targetObject;
            }
        }
#endif


        public override void OnPostRender() {
            if (!_maskTexture || !_targetObject)
                return;

            _maskTexture.DiscardContents();
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = _maskTexture;
            GL.Clear(false, true, Color.black);


            _outlineMaterial.SetFloat("_OutlineWith", m_owner.BDTargetOutlineWidth); 
            _outlineMaterial.SetColor("_Color", Color.white);
            _outlineMaterial.SetPass(0);

            for (int i = 0; i < _renderers.Length; i++) {
                Renderer renderer = _renderers[i];
                SkinnedMeshRenderer skinRenderer = renderer as SkinnedMeshRenderer;
                if (skinRenderer) {
                    skinRenderer.BakeMesh(_meshes[i]);
                } else {
                    _meshes[i] = renderer.GetComponent<MeshFilter>().sharedMesh;
                }
                Graphics.DrawMeshNow(_meshes[i], renderer.transform.localToWorldMatrix);
            }

            RenderTexture.active = currentRT;
        }

        public override void OnRenderImage(RenderTexture source, RenderTexture destination) {
            float widthMod = 1.0f / (1.0f * (1 << downsample));

            _material.SetVector("_Parameter", new Vector4(m_owner.BDBlurSize * widthMod, -m_owner.BDBlurSize * widthMod, 0.0f, 0.0f));
            source.filterMode = FilterMode.Bilinear;

            int rtW = source.width >> downsample;
            int rtH = source.height >> downsample;

            // downsample
            RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);

            rt.filterMode = FilterMode.Bilinear;
            //Graphics.Blit(source, rt, _material, 0);
            Graphics.Blit(source, rt);

            for (int i = 0; i < blurIterations; i++) {
                float iterationOffs = (i * 1.0f);
                _material.SetVector("_Parameter", new Vector4(m_owner.BDBlurSize * widthMod + iterationOffs, -m_owner.BDBlurSize * widthMod - iterationOffs, 0.0f, 0.0f));

                // vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, _material, 1);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;

                // horizontal blur
                rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, _material, 2);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }


            _material.SetTexture("_BlurTex", rt);
            _material.SetTexture("_MaskTex", _targetObject != null ? _maskTexture : null);
            _material.SetTexture("_DistortTex", m_owner.BDDistortTexture);
            _material.SetFloat("_DistortTexCoordScale", m_owner.BDDistortTextureScale);
            _material.SetFloat("_DistortIntensity", m_owner.BDDistortIntensity);
            _material.SetFloat("_SampleStrength", Mathf.Clamp01(m_owner.BDSampleStrength));

            Graphics.Blit(source, destination, _material, 0);


            RenderTexture.ReleaseTemporary(rt);
        }
    }
}
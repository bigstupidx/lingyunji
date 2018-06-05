//#define LOW_RES_SUNSHAFTS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 414


namespace Eyesblack.ImageEffects {
    [AddComponentMenu("Eyesblack/Image Effects/Post Effect")]

    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]

    public class PostEffect : MonoBehaviour {
        const int DIVIDER = 4;

        public enum ScreenBlendMode {
            Screen = 0,
            Add = 1,
        }
        [SerializeField]
        private bool _sunShaftsEnabled;
        public bool SunShaftsEnabled {
            get { return _sunShaftsEnabled; }
            set {
                if (_sunShaftsEnabled != value) {
                    _sunShaftsEnabled = value;
                    EnableSunShaftsKeywords();
                }
            }
        }
        public ScreenBlendMode _sunShaftsScreenBlendMode = ScreenBlendMode.Screen;
        public Transform _sunTransform;
        public Color _sunColor = Color.white;
        [Range(0, 10)]
        public float _sunShaftsBlurRadius = 2.5f;
        public float _sunShaftsIntensity = 1.15f;
        public float _sunShaftsMaxRadius = 0.75f;


        [SerializeField]
        private bool _DOFEnabled;
        public bool DOFEnabled {
            get { return _DOFEnabled; }
            set {
                if (!CameraDepthRender.IsDepthTextureSupported()) {
                    _DOFEnabled = false;
                } else {
                    if (_DOFEnabled != value) {
                        _DOFEnabled = value;
                        EnableDOFKeywords();
                    }
                }
            }
        }
        public bool _DOFForegroundBlur = false;
        [Range(0.1f, 1000)]
        public float _DOFFocalDistance = 50;
        [Range(0.01f, 1)]
        public float _DOFSmoothness = 0.01f;
        [Range(0.1f, 1)]
        public float _DOFBlurWidth = 0.5f;
        public Transform _DOFFocalObject;
        private bool _isDepthTextureSupported;



        [SerializeField]
        private bool _bloomEnabled;
        public bool BloomEnabled {
            get { return _bloomEnabled; }
            set {
                if (_bloomEnabled != value) {
                    _bloomEnabled = value;
                    EnableBloomKeywords();
                }
            }
        }
        [Range(0, 2.5f)]
        public float _bloomIntensity = 1;
        [Range(0, 1.5f)]
        public float _bloomThreshhold = 0.27f;
        public float _bloomBlurWidth = 1.0f;


        [SerializeField]
        private bool _radialBlurEnabled;
        public bool RadialBlurEnabled {
            get { return _radialBlurEnabled; }
            set {
                if (_radialBlurEnabled != value) {
                    _radialBlurEnabled = value;
                    EnableRadialBlurKeywords();
                }
            }
        }
        [Range(0, 5)]
        public float _radialBlurWidth = 0.5f;
        [Range(0, 20)]
        public float _radialBlurRange = 5;


        public enum ColorCorrectionMode {
            Simple,
            Amplify
        }
        [SerializeField]
        private bool _colorCorrectionEnabled;
        public bool ColorCorrectionEnabled {
            get { return _colorCorrectionEnabled; }
            set {
                if (_colorCorrectionEnabled != value) {
                    _colorCorrectionEnabled = value;
                    EnableColorCorrectionKeywords();
                }
            }
        }
        public ColorCorrectionMode _colorCorrectionMode;
        public AnimationCurve _CCRedChannel = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
        public AnimationCurve _CCGreenChannel = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
        public AnimationCurve _CCBlueChannel = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
        private Texture2D _CCRgbChannelTex;
        private bool _CCUpdateTexturesOnStartup = true;
        public Texture _CCLutTexture = null;


        [SerializeField]
        private bool _vignettingEnabled;
        public bool VignettingEnabled {
            get { return _vignettingEnabled; }
            set {
                if (_vignettingEnabled != value) {
                    _vignettingEnabled = value;
                    EnableVignettingKeywords();
                }
            }
        }
        public float _vignettingIntensity = 5;

        [SerializeField]
        private bool _saturationEnable;
        public bool SaturationEnable {
            get { return _saturationEnable; }
            set {
                if (_saturationEnable != value) {
                    _saturationEnable = value;
                    EnableSaturationKeywords();
                }
            }
        }
        [Range(0, 5)]
        public float _saturation = 1.0f;





        // static Texture2D _blackTexture = Texture2D.blackTexture;


        string _shaderName;
        Shader _shader;
        Material _material;

        bool _supported;
        Transform _tr;
        Camera _camera;
        //CameraDepthRender _depthRender;

        void Awake() {
            _tr = this.transform;
            _camera = this.GetComponent<Camera>();

            _isDepthTextureSupported = CameraDepthRender.IsDepthTextureSupported();
           // _depthRender = CameraDepthRender.CreateDepthRender(_camera);

            PostEffectProxy proxy = GetComponent<PostEffectProxy>();
            if (proxy == null) {
                proxy = gameObject.AddComponent<PostEffectProxy>();
            }
        }

        void Start() {
            _shaderName = "Hidden/Eyesblack/Image Effects/PostEffect";

            if (_colorCorrectionEnabled) {
                if (_colorCorrectionMode == ColorCorrectionMode.Simple)
                    _CCUpdateTexturesOnStartup = true;
                else if (_colorCorrectionMode == ColorCorrectionMode.Amplify)
                    CheckLutDimensions(_CCLutTexture);
            }
        }

        void OnEnable() {
        }

        void OnDisable() {
        }

        void OnDestroy() {
            if (_material != null) {
                DestroyImmediate(_material);
                _material = null;
            }

            if (_colorCorrectionEnabled) {
                if (_colorCorrectionMode == ColorCorrectionMode.Simple) {
                    if (_CCRgbChannelTex) {
                        DestroyImmediate(_CCRgbChannelTex);
                        _CCRgbChannelTex = null;
                    }
                    _CCUpdateTexturesOnStartup = true;
                }
            }
        }

        bool Supported() {
            if (_supported)
                return true;

            _supported = (SystemInfo.supportsImageEffects && /*SystemInfo.supportsRenderTextures && */_shader.isSupported);
            return _supported;
        }

        bool CreateMaterial() {
            if (_shader == null)
                _shader = Shader.Find(_shaderName);

            if (_shader == null)
                return false;

            if (!_shader.isSupported)
                return false;

            if (_material == null) {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.HideAndDontSave;

                EnableShaderKeywords();
            }

            return true;
        }

        bool CreateResources() {
            if (!CreateMaterial())
                return false;

            if (!Supported())
                return false;

            if (_colorCorrectionEnabled) {
                if (_colorCorrectionMode == ColorCorrectionMode.Simple) {
                    if (_CCRgbChannelTex == null) {
                        _CCRgbChannelTex = new Texture2D(256, 4, TextureFormat.RGBA32, false, true);

                        _CCRgbChannelTex.hideFlags = HideFlags.DontSave;
                        _CCRgbChannelTex.wrapMode = TextureWrapMode.Clamp;
                        _CCRgbChannelTex.filterMode = FilterMode.Point;
                        _CCRgbChannelTex.anisoLevel = 0;
                    }
                } else if (_colorCorrectionMode == ColorCorrectionMode.Amplify) {
                    //return _CCLutTexture != null;
                }
            }


            return true;
        }


        private void UpdateParameters() {
            if (_CCRgbChannelTex == null) {
                return;
            }

            for (float i = 0.0f; i <= 1.0f; i += 1.0f / 255.0f) {
                float rCh = Mathf.Clamp(_CCRedChannel.Evaluate(i), 0.0f, 1.0f);
                float gCh = Mathf.Clamp(_CCGreenChannel.Evaluate(i), 0.0f, 1.0f);
                float bCh = Mathf.Clamp(_CCBlueChannel.Evaluate(i), 0.0f, 1.0f);

                _CCRgbChannelTex.SetPixel(Mathf.FloorToInt(i * 255.0f), 0, new Color(rCh, rCh, rCh));
                _CCRgbChannelTex.SetPixel(Mathf.FloorToInt(i * 255.0f), 1, new Color(gCh, gCh, gCh));
                _CCRgbChannelTex.SetPixel(Mathf.FloorToInt(i * 255.0f), 2, new Color(bCh, bCh, bCh));
            }

            _CCRgbChannelTex.Apply();
        }

        public void UpdateTextures() {
            UpdateParameters();
        }

        private bool CheckLutDimensions(Texture lut) {
            bool valid = true;
            if (lut != null) {
                if ((lut.width / lut.height) != lut.height) {
                    Debug.LogWarning("[AmplifyColor] Lut " + lut.name + " has invalid dimensions.");
                    valid = false;
                } else {
                    if (lut.anisoLevel != 0)
                        lut.anisoLevel = 0;
                }
            }
            return valid;
        }

        public void EnableShaderKeywords() {
            EnableDOFKeywords();
            EnableSunShaftsKeywords();
            EnableBloomKeywords();
            EnableRadialBlurKeywords();
            EnableColorCorrectionKeywords();
            EnableVignettingKeywords();
            EnableSaturationKeywords();
        }

        void EnableDOFKeywords() {
            if (!_material) return;

            if (_DOFEnabled) {
                if (_DOFForegroundBlur) {
                    _material.EnableKeyword("DOF_BGFG");
                    _material.DisableKeyword("DOF_OFF");
                    _material.DisableKeyword("DOF_BG");
                } else {
                    _material.EnableKeyword("DOF_BG");
                    _material.DisableKeyword("DOF_OFF");
                    _material.DisableKeyword("DOF_BGFG");
                }
            } else {
                _material.EnableKeyword("DOF_OFF");
                _material.DisableKeyword("DOF_BG");
                _material.DisableKeyword("DOF_BGFG");
            }
        }

        void EnableSunShaftsKeywords() {
            if (!_material) return;

            if (_sunShaftsEnabled) {
                if (_sunShaftsScreenBlendMode == ScreenBlendMode.Screen) {
                    _material.EnableKeyword("SS_SCREEN");
                    _material.DisableKeyword("SS_OFF");
                    _material.DisableKeyword("SS_ADD");
                } else if (_sunShaftsScreenBlendMode == ScreenBlendMode.Add) {
                    _material.EnableKeyword("SS_ADD");
                    _material.DisableKeyword("SS_OFF");
                    _material.DisableKeyword("SS_SCREEN");
                }
            } else {
                _material.EnableKeyword("SS_OFF");
                _material.DisableKeyword("SS_SCREEN");
                _material.DisableKeyword("SS_ADD");
            }
        }

        void EnableBloomKeywords() {
            if (!_material) return;

            if (_bloomEnabled) {
                _material.EnableKeyword("BLOOM_ON");
                _material.DisableKeyword("BLOOM_OFF");
            } else {
                _material.EnableKeyword("BLOOM_OFF");
                _material.DisableKeyword("BLOOM_ON");
            }
        }

        void EnableRadialBlurKeywords() {
            if (!_material) return;

            if (_radialBlurEnabled) {
                _material.EnableKeyword("RB_ON");
                _material.DisableKeyword("RB_OFF");
            } else {
                _material.EnableKeyword("RB_OFF");
                _material.DisableKeyword("RB_ON");
            }
        }

        void EnableColorCorrectionKeywords() {
            if (!_material) return;

            if (_colorCorrectionEnabled) {
                if (_colorCorrectionMode == ColorCorrectionMode.Simple) {
                    _material.EnableKeyword("CC_SIMPLE");
                    _material.DisableKeyword("CC_OFF");
                    _material.DisableKeyword("CC_AMPLIFY");
                } else if (_colorCorrectionMode == ColorCorrectionMode.Amplify) {
                    _material.EnableKeyword("CC_AMPLIFY");
                    _material.DisableKeyword("CC_OFF");
                    _material.DisableKeyword("CC_SIMPLE");
                }
            } else {
                _material.EnableKeyword("CC_OFF");
                _material.DisableKeyword("CC_SIMPLE");
                _material.DisableKeyword("CC_AMPLIFY");
            }
        }

        void EnableVignettingKeywords() {
            if (!_material) return;

            if (_vignettingEnabled) {
                _material.EnableKeyword("VIGN_ON");
                _material.DisableKeyword("VIGN_OFF");
            } else {
                _material.EnableKeyword("VIGN_OFF");
                _material.DisableKeyword("VIGN_ON");
            }
        }

        void EnableSaturationKeywords() {
            if (!_material) return;

            if (_saturationEnable) {
                _material.EnableKeyword("SAT_ON");
                _material.DisableKeyword("SAT_OFF");
            } else {
                _material.EnableKeyword("SAT_OFF");
                _material.DisableKeyword("SAT_ON");
            }
        }






        List<RenderTexture> _tempRTList = new List<RenderTexture>();




        void DOF(RenderTexture source) {
            int rtW = source.width / 2;
            int rtH = source.height / 2;

            RenderTexture tempRtLowA = RenderTexture.GetTemporary(rtW, rtH, 0);
            RenderTexture tempRtLowB = RenderTexture.GetTemporary(rtW, rtH, 0);
            _tempRTList.Add(tempRtLowA);


            float oneOverW = 1.0f / (source.width * 1.0f);
            float oneOverH = 1.0f / (source.height * 1.0f);

            float focalDistance = _DOFFocalObject != null ? _camera.WorldToViewportPoint(_DOFFocalObject.position).z : _DOFFocalDistance;
            if (focalDistance < 0)
                focalDistance = 0;
            _material.SetVector("_DOFParameter", new Vector4(focalDistance, 100 * _DOFSmoothness, 0, 0));

            // downsample & blur
            tempRtLowA.DiscardContents();
            Graphics.Blit(source, tempRtLowA);

            oneOverW *= 4.0f * _DOFBlurWidth;
            oneOverH *= 4.0f * _DOFBlurWidth;
            _material.SetVector("_OffsetsA", new Vector4(1.5f * oneOverW, 0.0f, -1.5f * oneOverW, 0.0f));
            _material.SetVector("_OffsetsB", new Vector4(0.5f * oneOverW, 0.0f, -0.5f * oneOverW, 0.0f));
            tempRtLowB.DiscardContents();
            Graphics.Blit(tempRtLowA, tempRtLowB, _material, 2);

            _material.SetVector("_OffsetsA", new Vector4(0.0f, 1.5f * oneOverH, 0.0f, -1.5f * oneOverH));
            _material.SetVector("_OffsetsB", new Vector4(0.0f, 0.5f * oneOverH, 0.0f, -0.5f * oneOverH));
            tempRtLowA.DiscardContents();
            Graphics.Blit(tempRtLowB, tempRtLowA, _material, 2);


            _material.SetTexture("_DOFBlur", tempRtLowA);

            RenderTexture.ReleaseTemporary(tempRtLowB);
        }

        void SunShafts(RenderTexture source) {
            Texture2D blackTexture = Texture2D.blackTexture;

            if (!_sunTransform) {
                _material.SetTexture("_ColorBuffer", blackTexture);
                return;
            }

            Vector3 sunScreenPosition = _camera.WorldToViewportPoint(_sunTransform.position);
            if (sunScreenPosition.z < 0) {
                _material.SetTexture("_ColorBuffer", blackTexture);
                return;
            }


            int rtW = source.width / 2;
            int rtH = source.height / 2;
            RenderTexture tempRtLowA = RenderTexture.GetTemporary(rtW, rtH, 0);
            RenderTexture tempRtLowB = RenderTexture.GetTemporary(rtW, rtH, 0);
#if LOW_RES_SUNSHAFTS
            RenderTexture tmpRTBackground = RenderTexture.GetTemporary(rtW, rtH, 0);
#else
            RenderTexture tmpRTBackground = RenderTexture.GetTemporary(source.width, source.height, 0);
#endif
            _tempRTList.Add(tempRtLowA);


#if LOW_RES_SUNSHAFTS
            // downsample & blur
            tempRtLowB.DiscardContents();
            Graphics.Blit(source, tempRtLowB);
#endif

            RenderTexture.active = tmpRTBackground;
            if (RenderSettings.skybox != null)
                GL.ClearWithSkybox(false, _camera);
            else
                GL.Clear(false, true, _camera.backgroundColor, 0);
            _material.SetTexture("_Skybox", tmpRTBackground);
            _material.SetVector("_SunPosition", new Vector4(sunScreenPosition.x, sunScreenPosition.y, sunScreenPosition.z, _sunShaftsMaxRadius));

            tempRtLowA.DiscardContents();
#if LOW_RES_SUNSHAFTS
            Graphics.Blit(tempRtLowB, tempRtLowA, _material, 4);
#else
            Graphics.Blit(source, tempRtLowA, _material, 4);
#endif
            RenderTexture.ReleaseTemporary(tmpRTBackground);


            // radial blur:
            float ofs = _sunShaftsBlurRadius * (1.0f / 768.0f);
            _material.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            for (int it = 0; it < 2; it++) {
                tempRtLowB.DiscardContents();
                Graphics.Blit(tempRtLowA, tempRtLowB, _material, 3);
                ofs = _sunShaftsBlurRadius * (((it * 2.0f + 1.0f) * 6.0f)) / 768.0f;
                _material.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));

                tempRtLowA.DiscardContents();
                Graphics.Blit(tempRtLowB, tempRtLowA, _material, 3);

                ofs = _sunShaftsBlurRadius * (((it * 2.0f + 2.0f) * 6.0f)) / 768.0f;
                _material.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            }

            _material.SetVector("_SunColor", new Vector4(_sunColor.r, _sunColor.g, _sunColor.b, _sunColor.a) * _sunShaftsIntensity);
            _material.SetTexture("_ColorBuffer", tempRtLowA);

            RenderTexture.ReleaseTemporary(tempRtLowB);
        }

        void Bloom(RenderTexture source) {
            int rtW = source.width / DIVIDER;
            int rtH = source.height / DIVIDER;
            RenderTexture tempRtLowA = RenderTexture.GetTemporary(rtW, rtH, 0);
            RenderTexture tempRtLowB = RenderTexture.GetTemporary(rtW, rtH, 0);
            _tempRTList.Add(tempRtLowA);


            // downsample & blur
            tempRtLowB.DiscardContents();
            Graphics.Blit(source, tempRtLowB);

            _material.SetVector("_Parameter", new Vector4(0.0f, 0.0f, _bloomThreshhold, _bloomIntensity / (1.0f - _bloomThreshhold)));

            float oneOverW = 1.0f / (source.width * 1.0f);
            float oneOverH = 1.0f / (source.height * 1.0f);

            _material.SetVector("_OffsetsA", new Vector4(1.5f * oneOverW, 1.5f * oneOverH, -1.5f * oneOverW, 1.5f * oneOverH));
            _material.SetVector("_OffsetsB", new Vector4(-1.5f * oneOverW, -1.5f * oneOverH, 1.5f * oneOverW, -1.5f * oneOverH));

            tempRtLowA.DiscardContents();
            Graphics.Blit(tempRtLowB, tempRtLowA, _material, 1);

            oneOverW *= 4.0f * _bloomBlurWidth;
            oneOverH *= 4.0f * _bloomBlurWidth;

            _material.SetVector("_OffsetsA", new Vector4(1.5f * oneOverW, 0.0f, -1.5f * oneOverW, 0.0f));
            _material.SetVector("_OffsetsB", new Vector4(0.5f * oneOverW, 0.0f, -0.5f * oneOverW, 0.0f));
            tempRtLowB.DiscardContents();
            Graphics.Blit(tempRtLowA, tempRtLowB, _material, 2);

            _material.SetVector("_OffsetsA", new Vector4(0.0f, 1.5f * oneOverH, 0.0f, -1.5f * oneOverH));
            _material.SetVector("_OffsetsB", new Vector4(0.0f, 0.5f * oneOverH, 0.0f, -0.5f * oneOverH));

            tempRtLowA.DiscardContents();
            Graphics.Blit(tempRtLowB, tempRtLowA, _material, 2);

            _material.SetTexture("_Bloom", tempRtLowA);

            RenderTexture.ReleaseTemporary(tempRtLowB);

        }


        void ColorCorrection(RenderTexture source) {
            if (_colorCorrectionMode == ColorCorrectionMode.Simple) {
                if (_CCUpdateTexturesOnStartup) {
                    UpdateParameters();
                    _CCUpdateTexturesOnStartup = false;
                }
                _material.SetTexture("_CCRgbTex", _CCRgbChannelTex);
            } else if (_colorCorrectionMode == ColorCorrectionMode.Amplify) {
                _material.SetTexture("_CCRgbTex", _CCLutTexture);
            }
        }


        void Vignetting() {
            _material.SetFloat("_VignettingIntensity", _vignettingIntensity);
        }

        void Saturation() {
            _material.SetFloat("_Saturation", _saturation);
        }

        void RadialBlur() {
            _material.SetFloat("_RadialBlurWidth", _radialBlurWidth);
            _material.SetFloat("_RadialBlurRange", _radialBlurRange);
        }

        /*
        void Update() {
            _depthRender.IsNeedRender = _DOFEnabled;
        }
        */

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (!CreateResources()) {
                Graphics.Blit(source, destination);
                return;
            }

            if (_sunShaftsEnabled)
                SunShafts(source);

            if (_DOFEnabled && _isDepthTextureSupported)
                DOF(source);

            if (_bloomEnabled)
                Bloom(source);

            if (_radialBlurEnabled)
                RadialBlur();

            if (_colorCorrectionEnabled)
                ColorCorrection(source);

            if (_vignettingEnabled)
                Vignetting();

            if (_saturationEnable)
                Saturation();

            Graphics.Blit(source, destination, _material, 0);


            for (int i = 0; i < _tempRTList.Count; i++)
                RenderTexture.ReleaseTemporary(_tempRTList[i]);
            _tempRTList.Clear();

        }

    }
}
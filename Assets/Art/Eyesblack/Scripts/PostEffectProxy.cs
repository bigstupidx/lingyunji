using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Eyesblack.ImageEffects {
    [AddComponentMenu("")]
    public class PostEffectProxy : MonoBehaviour {
        PostEffect _postEffect;

        //场景本身设置的值
        bool _rawEnabled;
        bool _rawSunShaftsEnabled;
        bool _rawDOFEnabled;
        bool _rawBloomEnabled;
        bool _rawRadialBlurEnabled;
        bool _rawColorCorrectionEnabled;
        bool _rawVignettingEnabled;
        bool _rawSaturationEnable;

        //视频选项设置的值
        bool _optnSunShaftsEnabled;
        bool _optnDOFEnabled;
        bool _optnBloomEnabled;
        bool _optnRadialBlurEnabled;
        bool _optnColorCorrectionEnabled;
        bool _optnVignettingEnabled;
        bool _optnSaturationEnable;


        void Awake() {
            _postEffect = GetComponent<PostEffect>();
            if (_postEffect == null) {
                Destroy(this);
            } else {
                _rawEnabled = _postEffect.enabled;

                _rawSunShaftsEnabled = _postEffect.SunShaftsEnabled;
                _rawDOFEnabled = _postEffect.DOFEnabled;
                _rawBloomEnabled = _postEffect.BloomEnabled;
                _rawRadialBlurEnabled = _postEffect.RadialBlurEnabled;
                _rawColorCorrectionEnabled = _postEffect.ColorCorrectionEnabled;
                _rawVignettingEnabled = _postEffect.VignettingEnabled;
                _rawSaturationEnable = _postEffect.SaturationEnable;

                OnChangeQualityLevel();
            }
        }
       
        void CheckEffectEnabled(bool enabled) {
            if (enabled)
                _postEffect.enabled = true;
        }

        void Optimization() {
            if (!SunShaftsEnabled &&
                !DOFEnabled &&
                !BloomEnabled &&
                !RadialBlurEnabled &&
                !ColorCorrectionEnabled &&
                !VignettingEnabled &&
                !SaturationEnable)
                _postEffect.enabled = false;
        }


        public bool Enabled {
            get { return _postEffect.enabled; }
            set { _postEffect.enabled = value; }
        }

        public bool SunShaftsEnabled {
            get { return _postEffect.SunShaftsEnabled; }
            set {
                if (value) {
                    if (_optnSunShaftsEnabled) {
                        _postEffect.enabled = true;
                        _postEffect.SunShaftsEnabled = true;
                    }
                } else {
                    _postEffect.SunShaftsEnabled = false;
                    Optimization();
                }
            }
        }

        public bool OptnSunShaftsEnabled {
            get { return _optnSunShaftsEnabled; }
            set {
                _optnSunShaftsEnabled = value;
                if (value) {
                    if (_rawEnabled && _rawSunShaftsEnabled) {
                        _postEffect.enabled = true;                     
                        _postEffect.SunShaftsEnabled = true;
                    }
                } else {
                    _postEffect.SunShaftsEnabled = false;
                    Optimization();
                }              
            }
        }


        public bool DOFEnabled {
            get { return _postEffect.DOFEnabled; }
            set {
                if (value) {
                    if (_optnDOFEnabled) {
                        _postEffect.enabled = true;
                        _postEffect.DOFEnabled = true;
                    }
                } else {
                    _postEffect.DOFEnabled = false;
                    Optimization();
                }
            }
        }

        public bool OptnDOFEnabled {
            get { return _optnDOFEnabled; }
            set {
                _optnDOFEnabled = value;
                if (value) {
                    if (_rawEnabled && _rawDOFEnabled) {
                        _postEffect.enabled = true;                      
                        _postEffect.DOFEnabled = true;
                    }
                } else {
                    _postEffect.DOFEnabled = false;
                    Optimization();
                }
            }
        }

        public float DOFFocalDistance {
            get { return _postEffect._DOFFocalDistance; }
            set { _postEffect._DOFFocalDistance = value; }
        }

        public float DOFSmoothness {
            get { return _postEffect._DOFSmoothness; }
            set { _postEffect._DOFSmoothness = value; }
        }

        public float DOFBlurWidth {
            get { return _postEffect._DOFBlurWidth; }
            set { _postEffect._DOFBlurWidth = value; }
        }

        public bool BloomEnabled {
            get { return _postEffect.BloomEnabled; }
            set {
                if (value) {
                    if (_optnBloomEnabled) {
                        _postEffect.enabled = true;
                        _postEffect.BloomEnabled = true;
                    }
                } else {
                    _postEffect.BloomEnabled = false;
                    Optimization();
                }
            }
        }

        public bool OptnBloomEnabled {
            get { return _optnBloomEnabled; }
            set {
                _optnBloomEnabled = value;
                if (value) {
                    if (_rawEnabled && _rawBloomEnabled) {
                        _postEffect.enabled = true;                        
                        _postEffect.BloomEnabled = true;
                    }
                } else {
                    _postEffect.BloomEnabled = false;
                    Optimization();
                }
            }
        }

        public bool RadialBlurEnabled {
            get { return _postEffect.RadialBlurEnabled; }
            set {
                if (value) {
                    if (_optnRadialBlurEnabled) {
                        _postEffect.enabled = true;
                        _postEffect.RadialBlurEnabled = true;
                    }
                } else {
                    _postEffect.RadialBlurEnabled = false;
                    Optimization();
                }
            }
        }

        public bool OptnRadialBlurEnabled {
            get { return _optnRadialBlurEnabled; }
            set {
                _optnRadialBlurEnabled = value;
                if (value) {
                    if (_rawEnabled && _rawRadialBlurEnabled) {
                        _postEffect.enabled = true;
                        _postEffect.RadialBlurEnabled = true;
                    }
                } else {
                    _postEffect.RadialBlurEnabled = false;
                    Optimization();
                }
            }
        }

        public float RadialBlurWidth {
            get { return _postEffect._radialBlurWidth; }
            set { _postEffect._radialBlurWidth = value; }
        }

        public float RadialBlurRange {
            get { return _postEffect._radialBlurRange; }
            set { _postEffect._radialBlurRange = value; }
        }

        public bool ColorCorrectionEnabled {
            get { return _postEffect.ColorCorrectionEnabled; }
            set {
                if (value) {
                    if (_optnColorCorrectionEnabled) {
                        _postEffect.enabled = true;
                        _postEffect.ColorCorrectionEnabled = true;
                    }
                } else {
                    _postEffect.ColorCorrectionEnabled = false;
                    Optimization();
                }
            }
        }

        public bool OptnColorCorrectionEnabled {
            get { return _optnColorCorrectionEnabled; }
            set {
                _optnColorCorrectionEnabled = value;
                if (value) {
                    if (_rawEnabled && _rawColorCorrectionEnabled) {
                        _postEffect.enabled = true;                       
                        _postEffect.ColorCorrectionEnabled = true;
                    }
                } else {
                    _postEffect.ColorCorrectionEnabled = false;
                    Optimization();
                }
            }
        }

        public bool VignettingEnabled {
            get { return _postEffect.VignettingEnabled; }
            set {
                if (value) {
                    if (_optnVignettingEnabled) {
                        _postEffect.enabled = true;
                        _postEffect.VignettingEnabled = true;
                    }
                } else {
                    _postEffect.VignettingEnabled = false;
                    Optimization();
                }
            }
        }

        public bool OptnVignettingEnabled {
            get { return _optnVignettingEnabled; }
            set {
                _optnVignettingEnabled = value;
                if (value) {
                    if (_rawEnabled && _rawVignettingEnabled) {
                        _postEffect.enabled = true;                      
                        _postEffect.VignettingEnabled = true;
                    }
                } else {
                    _postEffect.VignettingEnabled = false;
                    Optimization();
                }
            }
        }

        public float VignettingIntensity {
            get { return _postEffect._vignettingIntensity; }
            set { _postEffect._vignettingIntensity = value; }
        }


        public bool SaturationEnable {
            get { return _postEffect.SaturationEnable; }
            set {
                if (value) {
                    if (_optnSaturationEnable) {
                        _postEffect.enabled = true;
                        _postEffect.SaturationEnable = true;
                    }
                } else {
                    _postEffect.SaturationEnable = false;
                    Optimization();
                }
            }
        }

        public bool OptnSaturationEnable {
            get { return _optnSaturationEnable; }
            set {
                _optnSaturationEnable = value;
                if (value) {
                    if (_rawEnabled && _rawSaturationEnable) {
                        _postEffect.enabled = true;                     
                        _postEffect.SaturationEnable = true;
                    }
                } else {
                    _postEffect.SaturationEnable = false;
                    Optimization();
                }
            }
        }

        public float Saturation {
            get { return _postEffect._saturation; }
            set { _postEffect._saturation = value; }
        }

        public void DisableAllEffects() {
            if (_postEffect.enabled) {
                SunShaftsEnabled = false;
                DOFEnabled = false;
                BloomEnabled = false;
                RadialBlurEnabled = false;
                ColorCorrectionEnabled = false;
                VignettingEnabled = false;
                SaturationEnable = false;
            }
        }

        public void RevertAllEffects() {
            Enabled = _rawEnabled;
            if (_rawEnabled) {
                SunShaftsEnabled = OptnSunShaftsEnabled && _rawSunShaftsEnabled;
                DOFEnabled = OptnDOFEnabled && _rawDOFEnabled;
                BloomEnabled = OptnBloomEnabled && _rawBloomEnabled;
                RadialBlurEnabled = OptnRadialBlurEnabled && _rawRadialBlurEnabled;
                ColorCorrectionEnabled = OptnColorCorrectionEnabled && _rawColorCorrectionEnabled;
                VignettingEnabled = OptnVignettingEnabled && _rawVignettingEnabled;
                SaturationEnable = OptnSaturationEnable && _rawSaturationEnable;
            }
        }



        public void OnChangeQualityLevel() {
            if (_postEffect == null)
                return;

            switch (PostEffectsQuality.QualityLevel) {
                case PostEffectsQuality.Quality.High:
                    OptnSunShaftsEnabled = true;
                    OptnDOFEnabled = true;
                    OptnBloomEnabled = true;
                    OptnRadialBlurEnabled = true;
                    OptnColorCorrectionEnabled = true;
                    OptnVignettingEnabled = true;
                    OptnSaturationEnable = true;
                    break;

                case PostEffectsQuality.Quality.Medium:
                    OptnSunShaftsEnabled = false;
                    OptnDOFEnabled = false;
                    OptnBloomEnabled = false;
                    OptnRadialBlurEnabled = false;
                    OptnColorCorrectionEnabled = true;
                    OptnVignettingEnabled = true;
                    OptnSaturationEnable = true;
                    break;

                case PostEffectsQuality.Quality.Low:
                    OptnSunShaftsEnabled = false;
                    OptnDOFEnabled = false;
                    OptnBloomEnabled = false;
                    OptnRadialBlurEnabled = false;
                    OptnColorCorrectionEnabled = false;
                    OptnVignettingEnabled = false;
                    OptnSaturationEnable = false;
                    break;
            }
        }


    }
}
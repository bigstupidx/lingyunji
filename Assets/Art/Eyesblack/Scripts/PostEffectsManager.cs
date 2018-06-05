using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Eyesblack.ImageEffects;

namespace Eyesblack {
    public class PostEffectsManager : MonoBehaviour {
        static PostEffectsManager _instance;
        public static PostEffectsManager Instance {
            get {
                if (_instance == null) {
                    GameObject go = new GameObject("__PostEffectsManager__");
                    _instance = go.AddComponent<PostEffectsManager>();
                }

                return _instance;
            }
        }


        PostEffectProxy _currentPostEffectProxy;

        float _radialBlurFadeInStartTime = -1;
        float _radialBlurFadeInEndTime = 0;
        float _radialBlurFadeOutStartTime = 0;
        float _radialBlurFadeOutEndTime = 0;
        const float MAX_RB_BLUR_SPREAD = 0.5f;
        const float MAX_RB_BLUR_RANGE = 5;

        float _saturation = 0;
        float _saturationChangeSpeed = 0;
        float _saturationChangeEndTime = -1;


        public PostEffectProxy CreatePostEffectProxy(Camera cam) {
            PostEffect postEffect = cam.GetComponent<PostEffect>();
            if (postEffect == null) {
                postEffect = cam.gameObject.AddComponent<PostEffect>();
            }

            return cam.GetComponent<PostEffectProxy>();
        }

        public PostEffectProxy GetPostEffectProxy(Camera cam) {
            return cam.GetComponent<PostEffectProxy>();
        }

        void SetCurrentProxy(Camera cam) {
            PostEffectProxy postEffectProxy = CreatePostEffectProxy(cam);
            _currentPostEffectProxy = postEffectProxy;
        }



        //fadeInTime 淡入时间长度
        //fadeOutTime 淡出时间长度
        //keepTime 保持之间长度
        //这些时间都可以为0
        public void StartRadialBlur(Camera cam, float fadeInTime, float fadeOutTime, float keepTime) {
            if (cam == null)
                return;

            SetCurrentProxy(cam);
            _currentPostEffectProxy.RadialBlurEnabled = true;
            _currentPostEffectProxy.RadialBlurWidth = 0;
            _currentPostEffectProxy.RadialBlurRange = 0;
            _radialBlurFadeInStartTime = Time.time;
            _radialBlurFadeInEndTime = _radialBlurFadeInStartTime + fadeInTime;

            _radialBlurFadeOutStartTime = _radialBlurFadeInEndTime + keepTime;
            _radialBlurFadeOutEndTime = _radialBlurFadeOutStartTime + fadeOutTime;
        }

        //fadeOutTime 淡出时间长度
        public void EndRadialBlur(float fadeOutTime) {
            _radialBlurFadeOutEndTime = Time.time + fadeOutTime;
        }


        public void SetDOFEnabled(bool enable, Camera cam, float focalDistance = 2, float Smoothness = 0.01f, float BlurWidth = 1) {
            if (enable) {
                PostEffectProxy postEffectProxy = CreatePostEffectProxy(cam);
                postEffectProxy.DOFEnabled = true;
                postEffectProxy.DOFFocalDistance = focalDistance;
                postEffectProxy.DOFSmoothness = Smoothness;
                postEffectProxy.DOFBlurWidth = BlurWidth;
            } else {
                PostEffectProxy postEffectProxy = GetPostEffectProxy(cam);
                if (postEffectProxy != null)
                    postEffectProxy.DOFEnabled = false;
            }
        }

        public void SetVignettingEnabled(bool enable, Camera cam) {
            if (enable) {
                PostEffectProxy postEffectProxy = CreatePostEffectProxy(cam);
                postEffectProxy.VignettingEnabled = true;
            } else {
                PostEffectProxy postEffectProxy = GetPostEffectProxy(cam);
                if (postEffectProxy != null)
                    postEffectProxy.VignettingEnabled = false;
            }
        }

        public void SetSaturation(Camera cam, float saturation, float time) {
            SetCurrentProxy(cam);
            _currentPostEffectProxy.SaturationEnable = true;
            _saturation = saturation;
            _saturationChangeSpeed = time > 0 ? (saturation - _currentPostEffectProxy.Saturation) / time : 0;
            _saturationChangeEndTime = Time.time + time;
        }

        public void DisableAllPostEffects(Camera cam) {
            PostEffectProxy postEffectProxy = GetPostEffectProxy(cam);
            if (postEffectProxy != null)
                postEffectProxy.DisableAllEffects();
        }

        public void RevertAllPostEffects(Camera cam) {
            PostEffectProxy postEffectProxy = GetPostEffectProxy(cam);
            if (postEffectProxy != null)
                postEffectProxy.RevertAllEffects();
        }



        public void Update() {
            HandleRadialBlur();
        }


        private void HandleRadialBlur() {
            if (_currentPostEffectProxy == null)
                return;

            if (_radialBlurFadeInStartTime >= 0) {
                float currentTime = Time.time;

                if (currentTime > _radialBlurFadeOutEndTime) {
                    _radialBlurFadeInStartTime = -1;
                    _currentPostEffectProxy.RadialBlurEnabled = false;
                    _currentPostEffectProxy = null;
                } else {
                    if (currentTime >= _radialBlurFadeInStartTime && currentTime <= _radialBlurFadeInEndTime) {
                        float f = Mathf.InverseLerp(_radialBlurFadeInStartTime, _radialBlurFadeInEndTime, currentTime);
                        _currentPostEffectProxy.RadialBlurWidth = Mathf.Lerp(0, MAX_RB_BLUR_SPREAD, f);
                        _currentPostEffectProxy.RadialBlurRange = Mathf.Lerp(0, MAX_RB_BLUR_RANGE, f);
                    } else if (currentTime >= _radialBlurFadeOutStartTime && currentTime <= _radialBlurFadeOutEndTime) {
                        float f = Mathf.InverseLerp(_radialBlurFadeOutStartTime, _radialBlurFadeOutEndTime, currentTime);
                        _currentPostEffectProxy.RadialBlurWidth = Mathf.Lerp(MAX_RB_BLUR_SPREAD, 0, f);
                        _currentPostEffectProxy.RadialBlurRange = Mathf.Lerp(MAX_RB_BLUR_RANGE, 0, f);
                    } else {
                        _currentPostEffectProxy.RadialBlurWidth = MAX_RB_BLUR_SPREAD;
                        _currentPostEffectProxy.RadialBlurRange = MAX_RB_BLUR_RANGE;
                    }
                }

            }


            if (_saturationChangeEndTime >= 0) {
                if (Time.time < _saturationChangeEndTime) {
                    _currentPostEffectProxy.Saturation += _saturationChangeSpeed * Time.deltaTime;
                } else {
                    _currentPostEffectProxy.Saturation = _saturation;
                    if (_saturation == 1) {
                        _currentPostEffectProxy.SaturationEnable = false;
                    }
                    _saturationChangeEndTime = -1;
                }

            }
        }
    }
}

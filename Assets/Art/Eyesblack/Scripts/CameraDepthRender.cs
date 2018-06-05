using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Eyesblack {
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class CameraDepthRender : MonoBehaviour {
        private bool _isNeedRender = false;
        public bool IsNeedRender {
            set {
                if((!_isNeedRender) && value ) {
                    _isNeedRender = true;
                }
            }

            get {
                return _isNeedRender;
            }
        }

        Camera _camera;
        Transform _tr;
        RenderTexture _depthTexture;
        Camera _depthCamera;
        Transform _depthCameraTr;
        static Shader _replacementShader;


        public static CameraDepthRender CreateDepthRender(Camera cam) {
            CameraDepthRender depthRender = cam.GetComponent<CameraDepthRender>();
            if (depthRender == null) {
                depthRender = cam.gameObject.AddComponent<CameraDepthRender>();
            }
            depthRender.enabled = true;
            return depthRender;
        }


        void Awake() {
            _camera = this.GetComponent<Camera>();
            _tr = this.transform;
        }


        void Start() {
            if (IsDepthTextureSupported())
                CreateDepthCamera();
        }


        public static bool IsDepthTextureSupported() {
            bool supported = false;
            //在Unity4中存在bug，安卓平台无法正常显示Depth格式
            supported = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth);
            return supported;
        }

        void CreateDepthCamera() {
            if (_depthCamera == null) {
                if (_replacementShader == null) {
                    _replacementShader = Shader.Find("Hidden/Eyesblack/Render Depth");
                    if (_replacementShader == null) {
                        Debug.LogError("[CameraDepthRender.DepthCamera] can not find Render Depth Shader!");
                        return;
                    }
                }

                GameObject go = new GameObject("__DepthCamera__", typeof(Camera));
                go.hideFlags = HideFlags.HideAndDontSave;

                _depthCamera = go.GetComponent<Camera>();
                _depthCameraTr = go.transform;
                _depthCamera.SetReplacementShader(_replacementShader, "RenderType");
                if (_depthTexture == null) {
                    _depthTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Depth);
                }
                _depthTexture.hideFlags = HideFlags.HideAndDontSave;
                _depthCamera.targetTexture = _depthTexture;
                _depthCamera.clearFlags = CameraClearFlags.SolidColor;
                _depthCamera.backgroundColor = Color.white;
                _depthCamera.enabled = false;

                Shader.SetGlobalTexture("_CameraDepthTexture", _depthTexture);
            }
        }

        IEnumerator OnPreRender() {
            if(_isNeedRender && _depthCamera!=null) {
                SyncDepthCamera();
                _depthCamera.Render();
            }

            yield return new WaitForEndOfFrame();
            _isNeedRender = false;
        }


        void SyncDepthCamera() {
            _depthCamera.depth = -100;// _camera.depth - 1;
            _depthCamera.cullingMask = _camera.cullingMask;
            _depthCamera.projectionMatrix = _camera.projectionMatrix;
            _depthCamera.fieldOfView = _camera.fieldOfView;
            _depthCamera.farClipPlane = _camera.farClipPlane;
            _depthCamera.nearClipPlane = _camera.nearClipPlane;

            _depthCameraTr.position = _tr.position;
            _depthCameraTr.rotation = _tr.rotation;
        }

    }

}

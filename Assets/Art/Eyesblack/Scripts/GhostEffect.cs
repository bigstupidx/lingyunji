using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Eyesblack.FX {
    [AddComponentMenu("Eyesblack/FX/Ghost Effect")]
    public class GhostEffect : EffecttBaseEvent {
		public float _durationTime = 1;
        public float _intervalTime = 0.02f;
        public float _lifeTime = 0.5f;
        public float _minDistance = 0.5f;
        [PackTool.Pack]
		public Material _material;
		public string _colorKey;
       // public Color _rimColor = new Color(1, 1, 1, 0.8f);
		public bool _autoDestroy = true;

        //public UnityEngine.Rendering.BlendMode _srcBlendMode = UnityEngine.Rendering.BlendMode.SrcAlpha;
        //public UnityEngine.Rendering.BlendMode _dstBlendMode = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;

        List<GhostRenderer> _ghostRenderers = new List<GhostRenderer>();

		float _startCreatTime = 0;


        //播放事件
        protected override void PlayEvent(AnimationEffectManage effectmanage) {
            _ghostRenderers.Clear();

            foreach (Renderer renderer in effectmanage.GetComponentsInChildren<Renderer>()) {
                GhostRenderer ghostRenderer = renderer.GetComponent<GhostRenderer>();
                if (ghostRenderer != null) {
                    if (!ghostRenderer.enabled)
                        ghostRenderer.enabled = true;
                } else
                    ghostRenderer = renderer.gameObject.AddComponent<GhostRenderer>();
                _ghostRenderers.Add(ghostRenderer);

                SyncGhostParamaters(ghostRenderer);

				ghostRenderer.GenerateEnabled = true;
				ghostRenderer.GenerateFinished = false;
            }

			_startCreatTime = Time.time;
        }


        void OnDisable() {
            for (int i = 0; i < _ghostRenderers.Count; i++) {
                GhostRenderer ghostRenderer = _ghostRenderers[i];
                if (ghostRenderer != null)
                    ghostRenderer.enabled = false;
            }
            _ghostRenderers.Clear();
        }


        void SyncGhostParamaters(GhostRenderer ghostRenderer) {
            ghostRenderer._intervalTime = _intervalTime;
            ghostRenderer._lifeTime = _lifeTime;
            ghostRenderer._minDistance = _minDistance;
			ghostRenderer._material = _material;
			ghostRenderer._colorKey = _colorKey;
            //ghostRenderer._rimColor = _rimColor;
            //ghostRenderer._srcBlendMode = _srcBlendMode;
            //ghostRenderer._dstBlendMode = _dstBlendMode;
        }



        void Update() {
            for (int i = 0; i < _ghostRenderers.Count; i++) {
                GhostRenderer ghostRenderer = _ghostRenderers[i];
                if (ghostRenderer != null) {
#if UNITY_EDITOR
                    SyncGhostParamaters(ghostRenderer);
#endif
					ghostRenderer.GenerateEnabled = Time.time < _startCreatTime + _durationTime;
                    if (ghostRenderer.GenerateFinished && _autoDestroy)
                    {
						Destroy (gameObject);
					}
                }
            }
        }

    }

}


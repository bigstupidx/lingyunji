using System;
using UnityEngine;
using System.Collections.Generic;

namespace UnityStandardAssets.ImageEffects
{

    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Camera/Camera Depth of Field (deprecated)")]
    public class CameraDepthOfFieldDeprecated : DepthOfFieldDeprecatedBase, CameraEffects.Effect
    {
        public Effect effect = new Effect();

        public override Camera GetCamera()
        {
            return effect.currentCamera;
        }

        void Awake()
        {
            effect.Init(gameObject, this);
        }

        void OnDisable()
        {
            effect.OnDisable();
        }

        protected override void OnEnable()
        {
            effect.LateUpdate();
            base.OnEnable();
        }

        void OnRemove()
        {
            effect.OnRemove();
        }

        void LateUpdate()
        {
            effect.LateUpdate();
        }
    }
}
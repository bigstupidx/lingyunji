using System;
using UnityEngine;
using System.Collections.Generic;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu ("Image Effects/Bloom and Glow/CameraBloom")]
    public class CameraBloom : BloomBase, CameraEffects.Effect
    {
        [SerializeField]
        Effect effect = new Effect();

        public override Camera GetCamera()
        {
            return effect.currentCamera;
        }

        public void RestoreCamera()
        {

        }

        void Awake()
        {
            effect.Init(gameObject, this);
        }

        void OnDisable()
        {
            effect.OnDisable();
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
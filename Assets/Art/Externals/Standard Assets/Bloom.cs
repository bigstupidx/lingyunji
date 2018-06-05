using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    [AddComponentMenu ("Image Effects/Bloom and Glow/Bloom")]
    public class Bloom : BloomBase
    {
        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            onRenderImage(source, destination);
        }
    }
}

using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Camera/Depth of Field (deprecated)")]
    public class DepthOfFieldDeprecated : DepthOfFieldDeprecatedBase
    {
        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            onRenderImage(source, destination);
        }
    }
}

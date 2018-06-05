using System;
using UnityEngine;
using System.Collections.Generic;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraEffects : MonoBehaviour
    {
        public interface Effect
        {
            void onRenderImage(RenderTexture source, RenderTexture destination);

            void RestoreCamera(); // 有些特效会修改相机的一些参数，在效果取消的时候，需要删掉

            bool enabled { get; }
        }

        public Camera Camera { get; private set; }

        void Awake()
        {
            Camera = GetComponent<Camera>();
        }

        public Effect current;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (current == null)
            {
                Graphics.Blit(source, destination);
				#if UNITY_EDITOR
				if(!Application.isPlaying)
					DestroyImmediate(this);
				else
					Destroy(this);
				#else
					Destroy(this);
				#endif
                return;
            }

            current.onRenderImage(source, destination);
        }
    }
}
using System;
using UnityEngine;

namespace xys.UI
{
    [ExecuteInEditMode]
    public class uBlur : MonoBehaviour
    {
        private Shader blurShader;
        public float blurSpread = 0.6f;
        public float iterations = 3f;
        private static Material m_Material;

        private void Awake()
        {
            this.blurShader = Shader.Find("UBlur/Blur");
            if (this.blurShader == null)
            {
                Debug.LogError("Cannot load blur shader!");
            }
        }

        private void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            float y = 1f;
            Vector2[] offsets = new Vector2[] { new Vector2(-y, -y), new Vector2(-y, y), new Vector2(y, y), new Vector2(y, -y) };
            Graphics.BlitMultiTap(source, dest, this.material, offsets);
        }

        public void FourTapCone(RenderTexture source, RenderTexture dest, float iteration, float step)
        {
            float y = 0.5f + (iteration * this.blurSpread);
            Vector2[] offsets = new Vector2[] { new Vector2(-y, -y), new Vector2(-y, y), new Vector2(y, y), new Vector2(y, -y) };
            Graphics.BlitMultiTap(source, dest, this.material, offsets);
        }

        protected void OnDisable()
        {
            if (m_Material != null)
            {
                UnityEngine.Object.DestroyImmediate(m_Material);
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (this.iterations > 0f)
            {
                int width = source.width / 4;
                int height = source.height / 4;
                RenderTexture dest = RenderTexture.GetTemporary(width, height, 0);
                this.DownSample4x(source, dest);
                float step = 0.1f;
                for (float i = 0f; i < this.iterations; i += step)
                {
                    RenderTexture texture2 = RenderTexture.GetTemporary(width, height, 0);
                    this.FourTapCone(dest, texture2, i, step);
                    RenderTexture.ReleaseTemporary(dest);
                    dest = texture2;
                }
                Graphics.Blit(dest, destination);
                RenderTexture.ReleaseTemporary(dest);
            }
        }

        protected void Start()
        {
            if (!SystemInfo.supportsImageEffects)
            {
                base.enabled = false;
            }
            else if ((this.blurShader == null) || !this.material.shader.isSupported)
            {
                base.enabled = false;
            }
        }

        protected Material material
        {
            get
            {
                if (m_Material == null)
                {
                    m_Material = new Material(this.blurShader);
                    m_Material.hideFlags = HideFlags.DontSaveInEditor;
                }
                return m_Material;
            }
        }
    }
}


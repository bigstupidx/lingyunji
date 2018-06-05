using UnityEngine;
using System.Collections;

public class BlurToTexture : MonoBehaviour
{
    public int iterations = 2;

    public float blurSpread = 0.2f;

    Material m_Material = null;

    // 保存的纹理
    [System.NonSerialized]
    public RenderTexture mRenderTexture;

    // 是否捕获结束
    public bool isDone { get; protected set; }

    Camera mCamera = null;

    public static BlurToTexture Get(Camera camera)
    {
        if (camera == null)
            return null;

        BlurToTexture btt = camera.GetComponent<BlurToTexture>();
        if (btt == null)
        {
            btt = camera.gameObject.AddComponent<BlurToTexture>();
            btt.isDone = false;
            btt.enabled =false;
            btt.mCamera = camera;
        }

        return btt;
    }

    // 捕获当前相机的纹理
    public bool BeginCapture()
    {
        if (!isSupported)
            return false;

        if (enabled)
        {
            if (isSetLayer)
            {
                mCamera.cullingMask = mLayerMask;
                isSetLayer = false;
            }
        }
        else
        {
            enabled = true;
        }

        isDone = false;
        return true;
    }

    bool isSetLayer = false;
    int mLayerMask = 0;

    public void EndCapture()
    {
        if (isSetLayer)
        {
            mCamera.cullingMask = mLayerMask;
            isSetLayer = false;
        }

        mLayerMask = 0;
        enabled = false;

        if (mRenderTexture != null)
        {
            RenderTexture.ReleaseTemporary(mRenderTexture);
            mRenderTexture = null;
        }
    }

    public bool IsDoing { get { return enabled; } }

    static Shader blurShader
    {
        get { return Shader.Find("Hidden/BlurEffectConeTap"); }
    }

    protected Material material
    {
        get
        {
            if (m_Material == null)
            {
                m_Material = new Material(blurShader);
                m_Material.hideFlags = HideFlags.DontSave;
            }
            return m_Material;
        }
    }

    // 是否支持
    bool isSupported
    {
        get
        {
            if (!SystemInfo.supportsImageEffects)
                return false;

            if (!blurShader || !material.shader.isSupported)
                return false;

            return true;
        }
    }

    // Performs one blur iteration.
    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = 0.5f + iteration * blurSpread;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    // Downsamples the texture to a quarter resolution.
    private void DownSample4x(RenderTexture source, RenderTexture dest)
    {
        float off = 1.0f;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    void OnDestroy()
    {
        if (m_Material != null)
        {
            DestroyImmediate(m_Material);
        }

        if (mRenderTexture != null)
        {
            RenderTexture.ReleaseTemporary(mRenderTexture);
            mRenderTexture = null;
        }
    }

    // Called by the camera to apply the image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (isDone == false)
        {
            Graphics.Blit(source, destination);
            int rtW = source.width / 4;
            int rtH = source.height / 4;
            if (mRenderTexture == null)
                mRenderTexture = RenderTexture.GetTemporary(rtW, rtH, 0);

            // Copy source to the 4x4 smaller texture.
            DownSample4x(source, mRenderTexture);

            // Blur the small texture
            if (iterations != 0)
            {
                RenderTexture tmpTex = RenderTexture.GetTemporary(rtW, rtH, 0);
                RenderTexture tt = null;
                for (int i = 0; i < iterations; i++)
                {
                    FourTapCone(mRenderTexture, tmpTex, i);
                    tt = tmpTex;

                    tmpTex = mRenderTexture;
                    mRenderTexture = tt;
                }

                RenderTexture.ReleaseTemporary(tmpTex);
            }

            mLayerMask = mCamera.cullingMask;
            mCamera.cullingMask = 0;
            isSetLayer = true;

            isDone = true;
        }
        else
        {
            Graphics.Blit(mRenderTexture, destination);
        }
    }
}

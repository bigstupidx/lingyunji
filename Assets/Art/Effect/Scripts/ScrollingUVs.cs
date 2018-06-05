using UnityEngine;
using System.Collections;

public class ScrollingUVs : MonoBehaviour 
{
    [SerializeField]
	int materialIndex = 0;

	public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
	public string textureName = "_MainTex";

    Renderer mRenderer;

    Material[] srcMaterials;

    public void ResetMaterial()
    {
        if (mRenderer == null)
            return;

        srcMaterials = mRenderer.sharedMaterials;
        if (srcMaterials == null || materialIndex >= srcMaterials.Length || materialIndex < 0 || ((srcMaterials[materialIndex]) == null))
        {
            this.enabled = false;
            srcMaterials = null;
            Debuger.LogError("ScrollingUVs 纹理超了!" + this.gameObject.name);
            return;
        }
    }

    void Start()
    {
        gameObject.isStatic = false;

        mRenderer = GetComponent<Renderer>();

        if (mRenderer == null)
        {
            this.enabled = false;
            Debuger.LogError("该对象没有render组件，不应该挂脚本ScrollingUVs  go=" + this.gameObject.name);
            return;
        }

        ResetMaterial();
    }

    Vector2 uvOffset = Vector2.zero;
	
	void LateUpdate() 
	{
        if (!isVisible || mRenderer == null || !mRenderer.enabled)
            return;

        uvOffset += uvAnimationRate * Time.unscaledDeltaTime;
        if (uvOffset.x > 1.0f || uvOffset.x < -1.0f)
            uvOffset.x = 0;

        if (uvOffset.y > 1.0f || uvOffset.y < -1.0f)
            uvOffset.y = 0;

        var materials = mRenderer.materials;
        if (materialIndex < materials.Length)
        {
            materials[materialIndex].SetTextureOffset(textureName, uvOffset);
        }
	}

    bool isVisible = true;

    void OnBecameVisible()
    {
        isVisible = true;
    }

    void OnBecameInvisible()
    {
        isVisible = false;
    }


	void OnDisable()
	{
		uvOffset = Vector2.zero; 
	}

    void OnEnable()
    {
        isVisible = true;
    }
}
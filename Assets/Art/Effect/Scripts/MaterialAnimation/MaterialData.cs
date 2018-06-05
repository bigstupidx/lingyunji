using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MaterialData : MonoBehaviour
{
#if !SCENE_DEBUG
    [PackTool.Pack]
#endif
    public Material material;
    public string colorName = "";
    public Color color;

    public string textureName = "";
    public Vector2 textureOffset;
    public Vector2 textureScale;

    public bool bPlayer = false;
	
	// Update is called once per frame
	void Update()
    {
        if (!bPlayer)
            return;

        if (material == null)
            return;

        if (material.HasProperty(colorName))
        {
            material.SetColor(colorName, color);
        }

        if (material.HasProperty(textureName))
        {
            material.SetTextureOffset(textureName, textureOffset);
            material.SetTextureScale(textureName, textureScale);
        }
	}
}

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

public class AtlasInfo : MonoBehaviour
{
    [SerializeField]
    Texture texture;

    void Update()
    {
        texture = null;
        Graphic g = GetComponent<Graphic>();
        if (g != null)
        {
            if (g is Image)
            {
                Image i = g as Image;
                if (i.sprite != null)
                {
                    texture = i.sprite.texture;
                }
            }
        }
    }
}
#endif
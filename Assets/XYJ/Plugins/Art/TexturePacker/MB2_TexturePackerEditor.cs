#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using DigitalOpus.MB.Core;

[UnityEngine.ExecuteInEditMode]
public class MB2_TexturePackerEditor : UnityEngine.MonoBehaviour
{
    static MB2_TexturePackerEditor instance_ = null;
    public static MB2_TexturePackerEditor instance
    {
        get
        {
            if (instance_ == null)
            {
                (new GameObject("MB2_TexturePackerEditor")).AddComponent<MB2_TexturePackerEditor>();
            }

            return instance_;
        }
    }

    public MB2_TexturePacker packer { get; set; }

    void Awake()
    {
        instance_ = this;
    }

    void OnDrawGizmos()
    {
        if (packer == null)
            return;

        packer.DrawGizmos();
    }
}
#endif
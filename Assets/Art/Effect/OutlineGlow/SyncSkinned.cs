using UnityEngine;
using System.Collections;

public class SyncSkinned : MonoBehaviour
{
    static Material _Empty;
    static Material Empty
    {
        get
        {
            if (_Empty == null)
            {
                _Empty = new Material(Shader.Find("Hidden/Empty"));
            }

            return _Empty;
        }
    }

    public static SyncSkinned Begin(SkinnedMeshRenderer smr, OutlineGlowRenderer parent)
    {
        SyncSkinned child = smr.GetComponentInChildren(typeof(SyncSkinned)) as SyncSkinned;
        if (child == null)
        {
            GameObject cgo = new GameObject("copy_smr");
            cgo.SetActive(false);
            child = cgo.AddComponent<SyncSkinned>();
            Transform childtran = cgo.transform;

            childtran.parent = smr.transform;
            childtran.localPosition = Vector3.zero;
            childtran.localRotation = Quaternion.identity;
            childtran.localScale = Vector3.one;

            child.parent = parent;
            cgo.SetActive(true);
        }

        child.smr = smr;
        return child;
    }

    OutlineGlowRenderer parent;

    public SkinnedMeshRenderer smr;

    void OnDestroy()
    {
        if (mesh != null)
        {
            Object.Destroy(mesh);
            mesh = null;
        }
    }

    void Start()
    {
        if (smr != null)
            Set(smr);
    }

    Mesh mesh;
    MeshRenderer meshRenderer = null;

    public void Set(SkinnedMeshRenderer lsmr)
    {
        smr = lsmr;
        if (meshRenderer == null)
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
        }

        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        if (mf == null)
            mf = gameObject.AddComponent<MeshFilter>();

        mf.sharedMesh = mesh;
        Material[] mats = smr.sharedMaterials;
        for (int i = 0; i < mats.Length; ++i)
            mats[i] = Empty;

        meshRenderer.sharedMaterials = mats;
    }

    void LateUpdate()
    {
        if (parent == null || !parent.enabled || !parent.gameObject.activeInHierarchy )
        {
            Object.Destroy(gameObject);
            return;
        }

        if (smr != null && mesh != null)
        {
            smr.BakeMesh(mesh);
            mesh.RecalculateBounds();

            transform.position = smr.transform.position;
            transform.rotation = smr.transform.rotation;
            Vector3 scale = smr.transform.lossyScale;
            transform.localScale = (new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z));
        }
    }
}

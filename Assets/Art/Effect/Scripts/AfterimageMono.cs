using UnityEngine;
using System.Collections;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

[ExecuteInEditMode]
public class AfterimageMono : MonoBehaviour
{
    Material mMaterial; // 材质
    string mColname; // 颜色名
    float mSpeed; // 速度
    Color mColor; // 当前颜色

    // 是否设置了新的透明值
    bool isSetNew = false;

    public float alpha
    {
        get { return mColor.a; }
        set { mColor.a = value; isSetNew = true; }
    }

    void OnDestroy()
    {
        if (mMaterial != null)
        {
            Object.Destroy(mMaterial);
            mMaterial = null;
        }
    }

    public void Begin(float speed, Material material, string colname)
    {
        mMaterial = material;
        mSpeed = speed;
        mColname = colname;

        if (string.IsNullOrEmpty(mColname))
            mColname = "MainColor";

        if (!mMaterial.HasProperty(colname))
        {
            Debuger.LogError("obj:" + gameObject.name + " material:" + material.name + " not has color:" + colname);
            DestroyObj(gameObject);
            return;
        }

        mColor = mMaterial.GetColor(mColname);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            XTools.GlobalCoroutine.StartCoroutine(BeginUpdate());
            return;
        }
#endif

        StartCoroutine(BeginUpdate());
    }

    IEnumerator BeginUpdate()
    {
        while (true)
        {
            if (!isSetNew)
                mColor.a -= GetDeltaTime() * mSpeed;
            else
                isSetNew = false;

            if (mColor.a <= 0f)
            {
                mColor.a = 0;
                mMaterial.SetColor(mColname, mColor);
                DestroyObj(gameObject);
                yield break;
            }

            mMaterial.SetColor(mColname, mColor);
            yield return 0;
        }
    }

    float GetDeltaTime()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return 0.033f;
#endif

        return Time.deltaTime;
    }

    static public void DestroyObj(GameObject go)
    {
        if (go != null)
        {
            if (Application.isPlaying)
            {
                go.transform.parent = null;
                UnityEngine.Object.Destroy(go);
            }
            else UnityEngine.Object.DestroyImmediate(go);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

public class SceneOptimize : MonoBehaviour
{
    enum OptType
    {
        ParticleSystem,
        MeshRenderer,
    }

    // 此列表当中的数据，判断与主相机之间的距离，超过一定范围，就把层级设置下，全部隐藏掉
    [System.Serializable]
    class Obj
    {
        public Obj(Transform tran, OptType type)
        {
            go = tran.gameObject;
            position = tran.position;
            position.y = 0f;
            layer = go.layer;
            this.type = type;

            Collider c = tran.GetComponent<Collider>();
            if (c != null)
            {
                Vector3 size = c.bounds.size;
                lenght = Mathf.Max(size.x, size.y, size.z, 2f);
            }
            else
            {
                lenght = 2f;
            }
        }

        public OptType type;
        public GameObject go;
        public Vector3 position;
        public int layer;
        public bool isset = false; // 当前是否有设置
        public float lenght = 0;

        public void Check(Vector3 cameraPosition, float magnitude)
        {
            if (((position - cameraPosition).magnitude) >= (magnitude + lenght))
            {
                // 不可见
                if (isset)
                    return; // 已经调协了，不管
                isset = true;
                xys.UI.Helper.SetLayer(go, Layer.no);
            }
            else
            {
                // 可见
                if (!isset)
                    return; // 当前也没有设置

                isset = false;
                xys.UI.Helper.SetLayer(go, layer);
            }
        }
    }

    [SerializeField]
    List<Obj> mObjs = new List<Obj>();

    public void Add(GameObject root)
    {
        Transform parent = root.transform;
        if (parent.GetComponent<MeshRenderer>() != null)
        {
            mObjs.Add(new Obj(parent, OptType.MeshRenderer));
        }
        else if (parent.GetComponent<ParticleSystem>() != null)
        {
            mObjs.Add(new Obj(parent, OptType.ParticleSystem));
        }

        int childCount = parent.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            var child = parent.GetChild(i);
            if (child.GetComponent<SceneOptimize>() != null)
                continue;

            Add(parent.GetChild(i).gameObject);
        }
    }

#if UNITY_EDITOR
    [SerializeField]
    int nototal = 0;
#endif

    // 清除所有
    public void Clear()
    {
        XYJLogger.LogWarning("Clear");
        mObjs.Clear();
    }

    private void Awake()
    {
        Add(gameObject);
    }

    Transform mainTrans;
    Camera mainCamera;
    Vector3 lastPosition;

    void CheckMain()
    {
        Camera camera = Camera.main;
        if (camera == mainCamera)
            return;

        mainCamera = camera;
        if (mainCamera != null)
        {
            mainCamera.cullingMask &= ~Layer.noMask;
            mainTrans = camera.transform;
            lastPosition = Vector3.zero * -100000;

            XYJLogger.LogWarning("new mainCamera:{0} no:{1}", mainCamera.cullingMask, Layer.no);
        }
    }

    [SerializeField]
    float magnitude = 500f;

    [SerializeField]
    float particleMagnitude = 30f;

    void LateUpdate()
    {
        CheckMain();
        if (mainCamera == null)
            return;

        Vector3 cameraPosition = mainTrans.position;
        cameraPosition.y = 0f;
        if ((lastPosition - cameraPosition).sqrMagnitude <= 4f)
            return;

#if UNITY_EDITOR
        nototal = 0;
#endif
        XYJLogger.LogWarning("LateUpdate mainCamera:{0}", mainCamera.cullingMask);
        lastPosition = cameraPosition;
        for (int i = 0; i < mObjs.Count; ++i)
        {
            switch (mObjs[i].type)
            {
            case OptType.ParticleSystem:
                mObjs[i].Check(lastPosition, particleMagnitude);
                break;
            case OptType.MeshRenderer:
                mObjs[i].Check(lastPosition, magnitude);
                break;
            }

#if UNITY_EDITOR
            if (mObjs[i].isset)
                ++nototal;
#endif
        }
    }
}
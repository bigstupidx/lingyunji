#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace xys.UI3D
{
    //

    [RequireComponent(typeof(Canvas))]
    public class TitleSystem : MonoBehaviour
    {
        static public TitleSystem instance { get; protected set; }
        Canvas canvas;

        Camera mainCamera; // 当前的主相机
        Transform mainCameraTrans;

        [SerializeField]
        float scale = 0.008f;

        Transform trans;

        const float DEFAULT_SCREEN_WIDTH = 1334;

        void Awake()
        {
            instance = this;
            trans = transform;
            canvas = GetComponent<Canvas>();

            enabled = false;

            //DontDestroyOnLoad(gameObject);
        }

#if !USE_RESOURCESEXPORT
        [System.Serializable]
#endif
        class Data
        {
            public TransformWrap target; // 3D目标
            public RectTransform title; // 称号
            public Vector3 offset; // 本地坐标偏移量
            public float distance; // 与相机的距离

            protected Vector3 last_position = Vector3.zero;
            protected Vector3 last_eulerAngles = Vector3.zero;
            protected float last_scale = -1f;

            public void Free()
            {
                target = null;
                title = null;
                offset = Vector3.zero;
                distance = 0f;

                last_position = Vector3.zero;
                last_eulerAngles = Vector3.zero;
                last_scale = -1f;

                XTools.Buff<Data>.Free(this);
            }

            public bool Update(Vector3 eulerAngles, Vector3 cameraPosition, float scaleV)
            {
                if (target == null || title == null || target.transform == null)
                    return false;

                if (!title.gameObject.activeInHierarchy)
                    return true;

                Matrix4x4 localToWorldMatrix = target.localToWorldMatrix;
                Vector3 localPosition = offset;

                Vector3 worldPosition = localToWorldMatrix.MultiplyPoint(localPosition);
                if (worldPosition != last_position)
                {
                    title.position = worldPosition;
                    last_position = worldPosition;
                }

                if (last_eulerAngles != eulerAngles)
                {
                    title.eulerAngles = eulerAngles;
                    last_eulerAngles = eulerAngles;
                }

                //动态更新大小
                distance = (cameraPosition - worldPosition).magnitude;
                float scale = distance * scaleV;
                if (last_scale != scale)
                {
                    title.localScale = Vector3.one * scale;
                    last_scale = scale;
                }

                return true;
            }
        }

        Dictionary<TransformWrap, Data> mMaps = new Dictionary<TransformWrap, Data>();

#if !USE_RESOURCESEXPORT
        [SerializeField]
#endif
        List<Data> mList = new List<Data>();

        public void Bind(TransformWrap target, RectTransform title, Vector3 offset)
        {

            if (mMaps.ContainsKey(target))
            {
                Debug.LogErrorFormat("id:{0} repate!", target.gameObject);
                return;
            }

            UI.Helper.SetLayer(title, Layer.ui3d);
            title.SetParent(trans);
            title.localScale = Vector3.one;
            title.localEulerAngles = Vector3.zero;

            Data d = XTools.Buff<Data>.Get();
            d.target = target;
            d.title = title;
            d.offset = offset;

            mList.Add(d);
            mMaps.Add(target, d);

            if (!enabled)
                enabled = true;
        }

        public bool Remove(TransformWrap target)
        {
            Data d = null;
            if (!mMaps.TryGetValue(target, out d))
                return false;
            mMaps.Remove(d.target);
            int index = mList.IndexOf(d);
            if (index != -1)
                mList[index] = null;

            d.Free();

            return true;

        }

        static void OnCameraActive(Camera camera)
        {
            if (camera != null)
                camera.cullingMask |= Layer.ui3dMask;
        }

        static void OnCameraUnactive(Camera camera)
        {
            if (camera != null)
                camera.cullingMask &= ~Layer.ui3dMask;
        }

        void LateUpdate()
        {
            Camera main = Camera.main;
            if (main != mainCamera)
            {
                OnCameraUnactive(mainCamera);
                mainCamera = main;
                mainCameraTrans = (mainCamera == null ? null : mainCamera.transform);
                OnCameraActive(mainCamera);
                canvas.worldCamera = mainCamera;
            }

            if (mainCamera == null)
                return;

            trans.localScale = Vector3.one * scale;

            Vector3 eulerAngles = mainCameraTrans.eulerAngles;
            Vector3 cameraPosition = mainCameraTrans.position;

            float scaleV = 250f * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) / /*mainCamera.pixelWidth*/DEFAULT_SCREEN_WIDTH;

            Data d = null;
            for (int i = mList.Count - 1; i >= 0; --i)
            {
                d = mList[i];
                if (d == null)
                {
                    mList.RemoveAt(i);
                    continue;
                }

                if (!d.Update(eulerAngles, cameraPosition, scaleV))
                {
                    mList.RemoveAt(i);
                    mMaps.Remove(d.target);

                    d.Free();
                }
            }

            mList.Sort((Data x, Data y) =>
            {
                return x.distance.CompareTo(y.distance);
            });

            int count = mList.Count;
            for (int i = 0; i < count; ++i)
                mList[i].title.SetSiblingIndex(count - i - 1);

            if (mMaps.Count == 0)
            {
                enabled = false;
            }
        }

        void OnEnable()
        {
            OnCameraActive(mainCamera);
            canvas.worldCamera = mainCamera;
            mainCameraTrans = (mainCamera == null ? null : mainCamera.transform);
        }

        void OnDisable()
        {
            OnCameraUnactive(mainCamera);
            mainCamera = null;
            mainCameraTrans = null;
            canvas.worldCamera = null;
        }
    }
}

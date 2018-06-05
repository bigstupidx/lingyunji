#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace xys.UI
{
    //
    [RequireComponent(typeof(Canvas))]
    public class Title2DSystem : MonoBehaviour
    {
        static public Title2DSystem instance { get; protected set; }

        Canvas canvas;

        Transform trans;

        protected void Awake()
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
            public int id;
            public ITarget target; // 3D目标
            public RectTransform title; // 称号

            public bool Update(Vector3 cameraPosition, Camera mainCamera, float scaleFactor)
            {
                if (target.isDestroy || title == null)
                    return false;

                if (!target.isVisable)
                {
                    title.anchoredPosition = new Vector2(-10000, -10000);
                    return true;
                }

                Vector3 position = target.position;
                float distance = (cameraPosition - position).sqrMagnitude;
                if (distance >= 100 * 100)
                {
                    // 不需要显示
                    title.anchoredPosition = new Vector2(-10000, -10000);
                }
                else
                {
                    Vector3 screenPoint = mainCamera.WorldToScreenPoint(position);
                    if (screenPoint.z >= 0.01f)
                    {
                        //Vector2 current = title.anchoredPosition;
                        Vector2 newpos = screenPoint * scaleFactor;
                        title.anchoredPosition = newpos;
                    }
                    else
                        title.anchoredPosition = new Vector2(-10000, -10000);
                }

                return true;
            }
        }

        Dictionary<int, Data> mMaps = new Dictionary<int, Data>();

#if !USE_RESOURCESEXPORT
        [SerializeField]
#endif
        List<Data> mList = new List<Data>();

        public interface ITarget
        {
            bool isVisable { get; }
            Vector3 position { get; }

            bool isDestroy { get; }

            int GetInstanceID();
        }

        class TransformTarget : ITarget
        {
            public TransformTarget(Transform t, Vector3 offset)
            {
                trans = t;
                this.offset = offset;
            }

            Transform trans;

            Vector3 offset;

            public bool isVisable
            {
                get
                {
                    return trans.gameObject.activeSelf;
                }
            }

            public Vector3 position
            {
                get
                {
                    return trans.localToWorldMatrix.MultiplyPoint(offset);
                }
            }

            public bool isDestroy { get { return trans == null ? true : false; } }

            public int GetInstanceID()
            {
                return trans.GetInstanceID();
            }
        }

        public void Bind(ITarget target, RectTransform title)
        {
            int id = target.GetInstanceID();
            if (mMaps.ContainsKey(id))
            {
                Debug.LogErrorFormat("id:{0} repate!", id);
                return;
            }

            Helper.SetLayer(title, Layer.ui);

            title.anchorMin = title.anchorMax = Vector2.zero;

            title.SetParent(trans);
            title.localScale = Vector3.one;
            title.localEulerAngles = Vector3.zero;

            Data d = new Data();
            d.id = id;
            d.target = target;
            d.title = title;

            mList.Add(d);
            mMaps.Add(id, d);

            if (!enabled)
                enabled = true;
        }

        public void Bind(Transform target, RectTransform title, Vector3 offset)
        {
            Bind(new TransformTarget(target, offset), title);
        }

        public bool Remove(Transform target)
        {
            int id = target.GetInstanceID();
            Data d = null;
            if (!mMaps.TryGetValue(id, out d))
                return false;

            int index = mList.IndexOf(d);
            if (index != -1)
                mList[index] = null;

            return true;
        }

        void LateUpdate()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                canvas.enabled = false;
                return;
            }

            canvas.enabled = true;

            Data d = null;
            Vector3 cameraPosition = mainCamera.transform.position;
            float scaleFactor = 1f / canvas.scaleFactor;
            for (int i = mList.Count - 1; i >= 0; --i)
            {
                d = mList[i];
                if (d == null)
                {
                    mList.RemoveAt(i);
                    continue;
                }

                if (!d.Update(cameraPosition, mainCamera, scaleFactor))
                {
                    mList.RemoveAt(i);
                    mMaps.Remove(d.id);
                }
            }

            if (mMaps.Count == 0)
            {
                enabled = false;
            }
        }
    }
}

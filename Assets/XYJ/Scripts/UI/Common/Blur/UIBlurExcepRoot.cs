#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace xys.UI
{
    public class UIBlurExcepRoot : MonoBehaviour
    {
        public static List<UIBlurExcepRoot> ActiveList = new List<UIBlurExcepRoot>();

        public static int Count { get { return ActiveList.Count; } }

        static bool isIniting = false;

        public static UIBlurExcepRoot Get(GameObject obj)
        {
            UIBlurExcepRoot root = obj.GetComponent<UIBlurExcepRoot>();
            if (root == null)
            {
                isIniting = true;
                root = obj.AddComponent<UIBlurExcepRoot>();
                root.enabled = false;
                isIniting = false;
            }

            return root;
        }

        public static T Get<T>(GameObject obj) where T : UIBlurExcepRoot
        {
            T root = obj.GetComponent<T>();
            if (root == null)
            {
                isIniting = true;
                root = obj.AddComponent<T>();
                root.enabled = false;
                isIniting = false;
            }

            return root;
        }

        protected Canvas canvas;

        protected RectTransform rectTransform;

        void ResetParent(RectTransform parent)
        {
            rectTransform.SetParent(parent);
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        // 激活时的父窗口
        protected virtual RectTransform activeParent
        {
            get
            {
                return xys.App.my.uiSystem.Blur.rectTransform;
            }
        }

        protected virtual RectTransform disableParent
        {
            get
            {
                return xys.App.my.uiSystem.PanelRoot;
            }
        }

        protected virtual void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        public int depth { get { return canvas.sortingOrder; } }

        protected virtual void OnEnable()
        {
            if (isIniting)
                return;

            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            ActiveList.Add(this);
        }

        bool mActiveEffect = false; // 当前是否有开启此效果

        // 是否激活效果
        protected virtual bool resetParent
        {
            get { return gameObject.activeSelf; }
        }

        // 开启效果
        public void Open()
        {
            if (mActiveEffect)
                return;

            mActiveEffect = true;
            ResetParent(activeParent);
        }

        public void Close()
        {
            if (!mActiveEffect)
                return;

            mActiveEffect = false;
            if (resetParent)
                ResetParent(disableParent);
        }

#if UNITY_EDITOR
        bool isQuit = false;
        void OnApplicationQuit()
        {
            isQuit = true;
        }
#endif

        protected virtual void OnDisable()
        {
            if (isIniting)
                return;

#if UNITY_EDITOR
            if (isQuit)
                return;
#endif
            if (!ActiveList.Remove(this))
                return;

            Close();
        }
    }
}

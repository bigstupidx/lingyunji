using UnityEngine;
using System;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    public class UIDraw : MonoBehaviour
    {
        protected virtual void OnTransformParentChanged()
        {
            if (!isActiveAndEnabled)
                return;

            UpdateRect();
        }

        protected virtual void OnDisable()
        {
            if (canvasRenderer == null)
                return;

            canvasRenderer.Clear();
        }

        protected void OnEnable()
        {
            UpdateRect();
        }

        protected void Start()
        {
            UpdateRect();
        }

        protected void Awake()
        {
            canvasRenderer = GetComponent<CanvasRenderer>();
            rectTransform = GetComponent<RectTransform>();
        }

        public RectTransform rectTransform { get; private set; }

        public CanvasRenderer canvasRenderer
        {
            get;
            private set;
        }

        protected void UpdateRect()
        {
            WXB.Tools.UpdateRect(rectTransform, Vector2.zero);
        }

        public void FillMesh(Mesh workerMesh)
        {
            canvasRenderer.SetMesh(workerMesh);
        }

        public virtual void UpdateMaterial(Material mat, Texture texture)
        {
            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(mat, 0);
            canvasRenderer.SetTexture(texture);
        }
    }
}
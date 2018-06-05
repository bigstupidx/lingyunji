using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Rect Soft Alpha Mask")]
    [ExecuteInEditMode]
    public class RectSoftAlphaMask : MonoBehaviour
    {
        [HideInInspector]
        public Vector4 SoftEdge = new Vector4(10, 10, 10, 10); // 软边

        static List<AlphaMaskMaterial> s_AlphaMaskMaterial = new List<AlphaMaskMaterial>();

        void OnDisable()
        {
            s_AlphaMaskMaterial.Clear();
            gameObject.GetComponentsInChildren(s_AlphaMaskMaterial);
            for (int i = 0; i < s_AlphaMaskMaterial.Count; ++i)
                s_AlphaMaskMaterial[i].SetMaterialDirty();
            s_AlphaMaskMaterial.Clear();
        }

        void OnEnable()
        {
            Set();
        }

        public Rect MaskRect
        {
            get
            {
                Rect rect = rectTransform.rect;
                rect.xMin -= SoftEdge.x;
                rect.xMax -= SoftEdge.z;
                rect.yMin -= SoftEdge.y;
                rect.yMax -= SoftEdge.w;
                return rect;
            }
        }

        RectTransform _rectTransform;

        public RectTransform rectTransform
        {
            get { return _rectTransform; }
            protected set { _rectTransform = value; }
        }

        [SerializeField]
        RectSoftAlphaMask parentMask;

        static List<Graphic> s_graphics = new List<Graphic>();

        public RectSoftAlphaMask ParentMask { get { return parentMask; } }

        void Set()
        {
            if (parentMask == null && rectTransform.parent != null)
                parentMask = rectTransform.parent.gameObject.GetComponentInParent<RectSoftAlphaMask>();

            s_graphics.Clear();
            GetComponentsInChildren(s_graphics);
            GameObject go = gameObject;
            for (int i = 0; i < s_graphics.Count; ++i)
            {
                if (go == s_graphics[i].gameObject)
                    continue;

                AlphaMaskMaterial amm = s_graphics[i].GetComponent<AlphaMaskMaterial>();
                if (amm == null)
                {
                    amm = s_graphics[i].gameObject.AddComponent<AlphaMaskMaterial>();
                    amm.rectParent = this;
                }

                amm.SetMaterialDirty();
            }

            s_graphics.Clear();
        }

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
#if UNITY_EDITOR
            if (GetComponent<Mask>() != null || GetComponent<RectMask2D>() != null)
            {
                Debuger.LogError("RectSoftAlphaMask 和Mask 组件不能同时存在 go=" + this.gameObject.name);
            }
#endif
        }

        static Vector3[] worldCorners;

        Dictionary<int, Material> BaseMaterialToSoftMaterial = new Dictionary<int, Material>();

        List<Material> currents = new List<Material>();

        public void Update()
        {
            if (!rectTransform.hasChanged)
                return;

            UpdateMaterial(currents);
        }

        void OnDestroy()
        {
            for (int i = 0; i < currents.Count; ++i)
            {
                UITools.Destroy(currents[i]);
            }

            currents.Clear();
            BaseMaterialToSoftMaterial.Clear();
        }

        Rect GetSelfMaskRect()
        {
            float xMin, xMax, yMin, yMax;

            worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            xMin = Mathf.Min(worldCorners[0].x, worldCorners[1].x);
            xMin = Mathf.Min(xMin, worldCorners[2].x);
            xMin = Mathf.Min(xMin, worldCorners[3].x);

            yMin = Mathf.Min(worldCorners[0].y, worldCorners[1].y);
            yMin = Mathf.Min(yMin, worldCorners[2].y);
            yMin = Mathf.Min(yMin, worldCorners[3].y);

            xMax = Mathf.Max(worldCorners[0].x, worldCorners[1].x);
            xMax = Mathf.Max(xMax, worldCorners[2].x);
            xMax = Mathf.Max(xMax, worldCorners[3].x);

            yMax = Mathf.Max(worldCorners[0].y, worldCorners[1].y);
            yMax = Mathf.Max(yMax, worldCorners[2].y);
            yMax = Mathf.Max(yMax, worldCorners[3].y);

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public Rect GetMaskRect()
        {
            if (parentMask == null)
                return GetSelfMaskRect();

            Rect rect = GetSelfMaskRect();
            Rect parentRect = parentMask.GetMaskRect();

            rect.xMin = Mathf.Max(rect.xMin, parentRect.xMin);
            rect.xMax = Mathf.Min(rect.xMax, parentRect.xMax);
            rect.yMin = Mathf.Max(rect.yMin, parentRect.yMin);
            rect.yMax = Mathf.Min(rect.yMax, parentRect.yMax);

            return rect;
        }

        public void UpdateMaterial(IEnumerable<Material> mats)
        {
            Rect maskRect = GetMaskRect();

            Vector2 size = maskRect.size;
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);

            Vector2 screenSize = rectTransform.rect.size;

            size.x /= screenSize.x;
            size.y /= screenSize.y;

            Vector4 softEdge = new Vector4(
                SoftEdge.x * size.x,
                SoftEdge.y * size.y,
                SoftEdge.z * size.x,
                SoftEdge.w * size.y);

            foreach (var m in mats)
            {
                if (m == null)
                    continue;

                m.SetVector("_Min", maskRect.min);
                m.SetVector("_Max", maskRect.max);
                m.SetVector("_SoftEdge", softEdge);
            }
        }

        public Material GetDefaultMaterial(Material baseMaterial)
        {
            Material mat;
            if (BaseMaterialToSoftMaterial.TryGetValue(baseMaterial.GetInstanceID(), out mat))
                return mat;

            Shader s = baseMaterial.shader;
            if (baseMaterial == null || baseMaterial.shader == null)
                return baseMaterial;

            string shaderName = s.name + " Soft Mask";
            s = Shader.Find(shaderName);
            if (s == null)
                return baseMaterial;

            var currentMaterial = new Material(baseMaterial);
            currentMaterial.shader = s;
            BaseMaterialToSoftMaterial.Add(baseMaterial.GetInstanceID(), currentMaterial);
            currents.Add(currentMaterial);

            return currentMaterial;
        }
	}
}
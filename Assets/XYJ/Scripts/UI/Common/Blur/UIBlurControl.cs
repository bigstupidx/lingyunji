#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

namespace xys.UI
{
    // 注意下，UI下的Blur效果，需要多个相机支持，开启这个效果，可以设置某个面板为例外，也即此面板仍是正常显示
    // 1 开启normalCamera的的Blur效果
    // 2 调整需要正常显示的面板层级为UIBlurExcep
    public class UIBlurControl : MonoBehaviour
    {
        [SerializeField]
        Camera normalCamera; // 正常的相机

        [SerializeField]
        Camera exceptionCamera; // 例外的

        uBlur normalBlur;

        Canvas canvas;
        UnityEngine.UI.CanvasScaler canvasScaler;

        [SerializeField]
        GameObject bg;

        public Camera uiCamera { get { return normalCamera; } }
        public Camera blurCamera { get { return exceptionCamera; } }

        [SerializeField]
        BaseRaycaster m_Raycaster;

        public bool isBlur
        {
            get { return exceptionCamera.enabled; }
            set
            {
                exceptionCamera.enabled = value;
                canvas.enabled = value;
                canvasScaler.enabled = value;
                normalBlur.enabled = value;
                m_Raycaster.enabled = value;
                bg.SetActive(value);
            }
        }

        public RectTransform rectTransform { get; protected set; }

        // 对话框的根结点
        public RectTransform dialogRoot;

        void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasScaler = GetComponent<UnityEngine.UI.CanvasScaler>();
            rectTransform = GetComponent<RectTransform>();
            normalBlur = normalCamera.GetComponent<uBlur>();

            //BaseRaycaster br = exceptionCamera.GetComponent<BaseRaycaster>();
            //if (br != null)
            //    Destroy(br);

            IntBit bit = new IntBit(exceptionCamera.cullingMask);
            //bit.Set(Layer.uiBlurExcep, true);
            bit.Set(Layer.ui, true);

            exceptionCamera.cullingMask = bit.value;

            isBlur = false;
            //DontDestroyOnLoad(gameObject);
        }
    }
}

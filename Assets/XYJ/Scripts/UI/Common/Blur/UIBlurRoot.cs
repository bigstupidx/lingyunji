using UnityEngine;

namespace xys.UI
{
    // 
    public class UIBlurRoot : UIBlurExcepRoot
    {
        static int total = 0;

        RectTransform startParent;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            startParent = rectTransform.parent as RectTransform;
        }

        protected override RectTransform activeParent
        {
            get
            {
                return xys.App.my.uiSystem.Blur.dialogRoot;
            }
        }

        protected override RectTransform disableParent
        {
            get
            {
                return startParent;
            }
        }

        protected void Update()
        {
            if (startParent == null)
            {
                UITools.Destroy(gameObject);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            ++total;
            if (total == 1)
            {
                RectTransform dialogRoot = activeParent;

                dialogRoot.localScale = Vector3.one;
                dialogRoot.localPosition = Vector3.zero;
                dialogRoot.anchorMin = Vector2.zero;
                dialogRoot.anchorMax = Vector2.one;

                dialogRoot.offsetMin = Vector2.zero;
                dialogRoot.offsetMax = Vector2.zero;

                dialogRoot.GetComponent<Canvas>().enabled = true;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            --total;
            if (total == 0)
            {
                activeParent.GetComponent<Canvas>().enabled = false;
            }
        }
    }
}
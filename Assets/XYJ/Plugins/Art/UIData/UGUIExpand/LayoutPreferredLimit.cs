using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Layout Preferred Limit", 141)]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class LayoutPreferredLimit : UIBehaviour, ILayoutElement
    {
        [SerializeField] private float m_PreferredWidthLimit = -1;
        [SerializeField] private float m_PreferredHeightLimit = -1;
        [SerializeField]
        private bool m_syncMin = true;
        List<Component> m_layouts = new List<Component>();
        

       
        public virtual void CalculateLayoutInputHorizontal() {}
        public virtual void CalculateLayoutInputVertical() {}
        public virtual float minWidth { get { return m_syncMin? preferredWidth:-1; }  }
        public virtual float minHeight { get { return m_syncMin ? preferredHeight : -1; }  }
        public virtual float preferredWidth {
            get {
                
                return m_PreferredWidthLimit<=0?-1:Mathf.Min(GetOtherLayoutProperty(e => e.preferredWidth), m_PreferredWidthLimit);//其他组件的preferred如果不大于限制值，那么不修改
            } 
            set {
                if (m_PreferredWidthLimit == value)
                    return;
                m_PreferredWidthLimit = value;
                SetDirty(); 
            }
        }
        public virtual float preferredHeight
        {
            get
            {
                return m_PreferredHeightLimit <= 0 ? -1 : Mathf.Min(GetOtherLayoutProperty(e => e.preferredHeight), m_PreferredHeightLimit);//其他组件的preferred如果不大于限制值，那么不修改
            }
            set
            {
                if (m_PreferredHeightLimit == value)
                    return;
                m_PreferredHeightLimit = value;
                SetDirty();
            }
        }
        public virtual float flexibleWidth { get { return -1; }  }
        public virtual float flexibleHeight { get { return -1; }  }
        public virtual int layoutPriority { get { return 2; } }


        protected LayoutPreferredLimit()
        {}

        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            SetDirty();
        }

        protected override void OnDisable()
        {
            SetDirty();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            SetDirty();
        }

        #endregion


        float GetOtherLayoutProperty(System.Func<ILayoutElement, float> property)
        {
            float min = 0;
            int maxPriority = System.Int32.MinValue;
            m_layouts.Clear();
            GetComponents(typeof(ILayoutElement), m_layouts);
            for (int i = 0; i < m_layouts.Count; i++)
            {
                if (m_layouts[i] == this)//自己排除在外
                    continue;
                var layoutComp = m_layouts[i] as ILayoutElement;
                if (layoutComp is Behaviour && !((Behaviour)layoutComp).isActiveAndEnabled)//禁用的排除在外
                    continue;

                int priority = layoutComp.layoutPriority;
                if (priority < maxPriority)//只获取优先级最高的
                    continue;

                float prop = property(layoutComp);
                // If this layout property is set to a negative value, it means it should be ignored.
                if (prop < 0)
                    continue;

                // If this layout component has higher priority than all previous ones,
                // overwrite with this one's value.
                if (priority > maxPriority)
                {
                    min = prop;
                    maxPriority = priority;
                }
                // If the layout component has the same priority as a previously used,
                // use the largest of the values with the same priority.
                else if (prop > min)
                {
                    min = prop;
                }
            }
            return min;
        }
        protected void SetDirty()
        {
            if (!IsActive())
                return;
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

    #endif
    }
}

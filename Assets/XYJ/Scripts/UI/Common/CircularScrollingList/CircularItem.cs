namespace xys
{
    using UnityEngine;
    using System.Collections;
    using UIWidgets;
    using UI;

    public class CircularItem : ListViewIconsItemComponent
    {
        private RectTransform m_moveItem;
        public Transform CircleMoveItem
        {
            get
            {
                if (null == m_moveItem)
                {
                    m_moveItem = new GameObject("CircleMoveItem").AddMissingComponent<RectTransform>();
                    m_moveItem.SetParent(transform);
                    m_moveItem.localPosition = Vector3.zero;
                    m_moveItem.localScale = Vector3.one;

                    //将所有对象设置到moveitem下
                    for (int i = transform.childCount - 1; i >= 0; --i)
                    {
                        Transform child = transform.GetChild(i);
                        if (child == m_moveItem)
                        {
                            continue;
                        }
                        child.SetParent(m_moveItem);
                        child.SetAsFirstSibling();
                    }
                }
                return m_moveItem;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        void Update()
        {

        }

        public override void SetData(ListViewIcons lvi, ListViewIconsItemDescription newItem)
        {
            base.SetData(lvi, newItem);
            if (lvi.SelectedIndicies.Contains(this.Index))
            {
                //选中

            }
            else
            {
                //非选中

            }
        }
    }

}
namespace xys
{
    using UnityEngine;
    using System.Collections;
    using UIWidgets;
    using UI.State;


    public class UIMapNpcItem : CircularItem
    {
        StateRoot m_bgState;

        protected override void Awake()
        {
            base.Awake();
            m_bgState = transform.Find("Btn").GetComponent<StateRoot>();
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void SetData(ListViewIcons lvi, ListViewIconsItemDescription newItem)
        {
            base.SetData(lvi, newItem);
            if (lvi.SelectedIndicies.Contains(this.Index))
            {
                //选中
                OnSelect(true);
            }
            else
            {
                //非选中
                OnSelect(false);
            }
        }

        public void OnSelect(bool select)
        {
            if (select)
            {
                //选中
                m_bgState.SetCurrentState(1, true);
            }
            else
            {
                //非选中
                m_bgState.SetCurrentState(0, true);
            }
        }
    }

}

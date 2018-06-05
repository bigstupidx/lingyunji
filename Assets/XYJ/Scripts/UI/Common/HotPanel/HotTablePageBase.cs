#if !USE_HOT

namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;

    abstract class HotTablePageBase
    {
        protected HotTablePageBase(xys.UI.HotTablePage parent)
        {
            this.parent = parent;
        }

        public Event.HotObjectEventAgent Event { get; private set; }

        public xys.UI.HotTablePage parent { get; private set; }
        public HotTablePanelBase panel { get; private set; }

        public int id { get; private set; }

        public void SetID(int id)
        {
            this.id = id;
        }

        void Init(xys.hot.UI.HotTablePanelBase panel)
        {
            this.panel = panel;
            panel.OnCreatePage(this);

            OnInit();
        }

        IEnumerator InitSync()
        {
            yield return OnInitSync();
        }

        void Hide()
        {
            if (Event != null)
            {
                Event.Release();
                Event = null;
            }

            OnHide();
        }

        void Show(object p)
        {
            if (Event != null)
            {
                Event.Release();
            }
            else
            {
                Event = new Event.HotObjectEventAgent(App.my.localPlayer.eventSet);
            }

            OnShow(p);
        }

        IEnumerator ShowSync(object p)
        {
            yield return OnShowSync(p);
        }

        protected abstract void OnShow(object p);
        protected abstract void OnInit();
        protected virtual void OnHide() { }
        protected virtual IEnumerator OnInitSync() { yield break; }
        protected virtual IEnumerator OnShowSync(object p) { yield break; }
    }
}

#endif
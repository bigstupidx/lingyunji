#if !USE_HOT

namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;

    abstract class HotPanelBase
    {
        public HotPanelBase(xys.UI.UIHotPanel parent)
        {
            this.parent = parent;
        }

        public xys.UI.UIHotPanel parent { get; private set; }

        protected void Init()
        {
            hotApp.my.OnPanelCreate(this);
            OnInit();
        }

        IEnumerator InitSync()
        {
            yield return OnInitSync();
        }

        public xys.hot.Event.HotObjectEventAgent Event { get; private set; }

        protected void Show(object p)
        {
            if (Event != null)
            {
                Event.Release();
                Event = null;
            }

            Event = new hot.Event.HotObjectEventAgent(xys.App.my.localPlayer.eventSet);
            OnShow(p);
        }

        IEnumerator ShowSync(object p)
        {
            yield return OnShowSync(p);
        }

        void BeginPlayAnimShow()
        {
            OnBeginPlayAnimShow();
        }

        void EndPlayAnimShow()
        {
            OnEndPlayAnimShow();
        }

        void BeginPlayAnimHide()
        {
            OnBeginPlayAnimHide();
        }

        void EndPlayAnimHide()
        {
            OnEndPlayAnimHide();
        }

        protected void Hide()
        {
            if (Event != null)
            {
                Event.Release();
                Event = null;
            }

            OnHide();
        }

        protected void Destroy()
        {
            if (Event != null)
            {
                Event.Release();
                Event = null;
            }

            hotApp.my.OnPanelDestroy(this);
            OnDestroy();
        }

        protected abstract void OnInit();
        protected virtual IEnumerator OnInitSync() { yield break; }
        protected abstract void OnShow(object p);
        protected virtual IEnumerator OnShowSync(object p) { yield break; }
        protected virtual void OnBeginPlayAnimShow() { }
        protected virtual void OnEndPlayAnimShow() { }
        protected virtual void OnEndPlayAnimHide() { }
        protected virtual void OnBeginPlayAnimHide() { }        
        protected virtual void OnHide() { }
        protected virtual void OnDestroy() { }
    }
}

#endif
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace xys.UI
{
    using System;
    using State;
    [System.Serializable]
    public class HotPageData
    {
        [SerializeField]
        HotTablePage mono_page; // 对应的table页

#if UNITY_EDITOR
        public bool isFoldouts { get; set; }
#endif
        public HotTablePage Get()
        {
            return mono_page;
        }

        public void Set(HotTablePage v)
        {
            mono_page = v;
        }
    }

    public class HotTablePanel : UIHotPanel
    {
        [SerializeField]
        [HideInInspector]
        protected List<HotPageData> PageList = new List<HotPageData>();

        protected HotPageData Current;// 当前显示的页

        protected StateToggleInfo stateToggleInfo;

        [SerializeField]
        [HideInInspector]
        string CurrentShow; // 显示的页面

        public string CurrentPage
        {
            get { return CurrentShow; }
            set { CurrentShow = value; }
        }

#if UNITY_EDITOR
        public bool isShowPageList 
        {
            [EditorField]
            get; 
            [EditorField]
            set; 
        }
#endif
        public List<HotPageData> GetPageList()
        {
            return PageList;
        }

        protected override IEnumerator OnInitSync()
        {
            stateToggleInfo = new StateToggleInfo();
            for (int i = 0; i < PageList.Count; )
            {
                HotPageData data = PageList[i];
                HotTablePage tp = data.Get();
                if (tp == null)
                {
                    PageList.RemoveAt(i);
                    Debuger.ErrorLog("{0} Type Page index:{1} Null!", GetType().Name, i);
                    continue;
                }

                tp.parent = this;
                IEnumerator itor = tp.Init(this);
                while (itor.MoveNext())
                    yield return 0;

                stateToggleInfo.list.Add(tp.ToggleBtn);
                ++i;
            }

            // 页面切换
            stateToggleInfo.OnSelectChange = OnSelectChange;
            stateToggleInfo.OnPreChange = OnPreChange;
            stateToggleInfo.Init();

            IEnumerator ator = base.OnInitSync();
            while (ator.MoveNext())
                yield return 0;
        }

        bool OnPreChange(StateRoot sr, int index)
        {
            return OnPreChange(PageList[index].Get());
        }

        protected bool OnPreChange(HotTablePage page)
        {
            return (bool)refType.InvokeMethodReturn("OnPreChange", page);
        }

        protected void OnSelectChange(StateRoot sr, int index)
        {
            handleSwitchPage(sr, index);
        }

        protected override IEnumerator OnShowSync(object args)
        {
            IEnumerator itor = null;
            itor = ShowPage(CurrentPage, args);

            while (itor.MoveNext())
                yield return 0;

            itor = base.OnShowSync(args);
            while (itor.MoveNext())
                yield return 0;
        }

        protected override void OnHide()
        {
            if (Current != null)
            {
                Current.Get().Hide();
                Current = null;
            }

            CurrentShow = "";
            base.OnHide();
        }

        protected HotPageData Get(string type)
        {
            return PageList.Find((HotPageData d) => { return string.Equals(d.Get().pageType, type); });
        }

        public HotTablePage GetShowPage()
        {
            return Current == null ? null : Current.Get();
        }

        protected IEnumerator ShowPage(string pageType, object args)
        {
            int pos = pageType.LastIndexOf('.');
            if (pos != -1)
                pageType = pageType.Substring(pos + 1);

            if (PageList.Count != 0)
            {
                HotPageData newshow = null;
                HotTablePage temp = null;
                for (int i = 0; i < PageList.Count; ++i)
                {
                    temp = PageList[i].Get();
                    if (Equals(temp.pageType, pageType))
                    {
                        newshow = PageList[i];
                        continue;
                    }

                    temp.ToggleBtn.SetCurrentState(0, false);
                    temp.Hide();
                }

                if (newshow == null)
                {
                    newshow = PageList[0];
                }

                temp = newshow.Get();
                CurrentPage = temp.pageType;
                Current = newshow;
                temp.ToggleBtn.SetCurrentState(1, false);
                OnShowPage(temp);
                IEnumerator itor = temp.Show(args);
                while (itor.MoveNext())
                    yield return 0;
            }
        }

        public void ShowType(int index, object args)
        {
            ShowType(GetPageList()[index].Get().pageType, args);
        }

        public void ShowType(string pageType, object args)
        {
            StartCoroutine(ShowPage(pageType, args));
        }

        void handleSwitchPage(StateRoot item, int id)
        {
            if (item.CurrentState == 0)
            {
                // 隐藏
                OnHidePage(PageList[id].Get());
            }
            else
            {
                StartCoroutine(ShowPage(PageList[id].Get().pageType, null));
            }
        }

        protected virtual void OnShowPage(HotTablePage page)
        {
            refType.InvokeMethod("OnShowPage", page);
        }

        protected virtual void OnHidePage(HotTablePage page)
        {
            refType.InvokeMethod("OnHidePage", page);
        }
    }
}
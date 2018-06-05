using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace xys.UI
{
    using System;
    using State;
    [System.Serializable]
    public class PageData
    {
        [SerializeField]
        MonoBehaviour mono_page; // 对应的table页

#if UNITY_EDITOR
        public bool isFoldouts { get; set; }
#endif

        public TablePage<T> Get<T>()
        {
            return mono_page as TablePage<T>;
        }

        public void Set<T>(TablePage<T> v)
        {
            mono_page = v;
        }
    }

    public abstract class TablePanel<T> : UIPanelBase
    {
        [SerializeField]
        [HideInInspector]
        protected List<PageData> PageList = new List<PageData>();

        protected PageData Current;// 当前显示的页

        protected StateToggle stateToggle;

        [SerializeField]
        [HideInInspector]
        string CurrentShow; // 显示的页面

        public T CurrentPage
        {
            get { return Str2Enum.To<T>(CurrentShow, default(T)); }
            set { CurrentShow = value.ToString(); }
        }

#if UNITY_EDITOR
        public bool isShowPageList { get; set; }

        public List<PageData> GetPageList()
        {
            return PageList;
        }
#endif

        protected override void OnInit()
        {
        }

        protected override IEnumerator OnInitSync()
        {
            stateToggle = cachedGameObject.AddMissingComponent<StateToggle>();
            for (int i = 0; i < PageList.Count; )
            {
                PageData data = PageList[i];
                TablePage<T> tp = data.Get<T>();
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

                stateToggle.list.Add(tp.ToggleBtn);
                ++i;
            }

            // 页面切换
            stateToggle.OnSelectChange = OnSelectChange;
            stateToggle.OnPreChange = OnPreChange;
            yield break;
        }

        bool OnPreChange(StateRoot sr, int index)
        {
            return OnPreChange(PageList[index].Get<T>());
        }

        protected virtual bool OnPreChange(TablePage<T> page)
        {
            return true;
        }

        protected virtual void OnSelectChange(StateRoot sr, int index)
        {
            handleSwitchPage(sr, index);
        }

        protected override void OnShow(object args)
        {

        }

        protected override IEnumerator OnShowSync(object args)
        {
            IEnumerator itor = null;
            itor = ShowPage(CurrentPage, args);

            while (itor.MoveNext())
                yield return 0;
        }

        protected override void OnHide()
        {
            if (Current != null)
            {
                Current.Get<T>().Hide();
                Current = null;
            }

            CurrentShow = "";
        }

        public abstract bool Equals(T x, T y);

        protected PageData Get(T type)
        {
            return PageList.Find((PageData d) => { return Equals(d.Get<T>().pageType, type); });
        }

        public T1 Get<T1>() where T1 : TablePage<T>
        {
            for (int i = 0; i < PageList.Count; ++i)
            {
                TablePage<T> v = PageList[i].Get<T>();
                if (v is T1)
                    return (T1)v;
            }

            return null;
        }

        public TablePage<T> GetShowPage()
        {
            return Current == null ? null : Current.Get<T>();
        }

        protected IEnumerator ShowPage(T pageType, object args)
        {
            if (PageList.Count != 0)
            {
                PageData newshow = null;
                TablePage<T> temp = null;
                for (int i = 0; i < PageList.Count; ++i)
                {
                    temp = PageList[i].Get<T>();
                    if (Equals(temp.pageType, pageType))
                    {
                        newshow = PageList[i];
                        continue;
                    }

                    temp.ToggleBtn.CurrentState = 0;
                    temp.Hide();
                }

                temp = newshow.Get<T>();
                CurrentPage = temp.pageType;
                Current = newshow;
                temp.ToggleBtn.SetCurrentState(1, false);
                OnShowPage(temp);
                IEnumerator itor = temp.Show(args);
                while (itor.MoveNext())
                    yield return 0;
            }
        }

        public void Show(T pageType, object args)
        {
            StartCoroutine(ShowPage(pageType, args));
        }

        protected virtual void OnShowPage(TablePage<T> page)
        {

        }

        protected virtual void OnHidePage(TablePage<T> page)
        {

        }

        void handleSwitchPage(StateRoot item, int id)
        {
            if (item.CurrentState == 0)
            {
                // 隐藏
                OnHidePage(PageList[id].Get<T>());
            }
            else
            {
                StartCoroutine(ShowPage(PageList[id].Get<T>().pageType, null));
            }
        }
    }
}
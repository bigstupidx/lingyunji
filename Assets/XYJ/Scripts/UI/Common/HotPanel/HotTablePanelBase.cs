#if !USE_HOT

namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;

    abstract class HotTablePanelBase : HotPanelBase
    {
        public HotTablePanelBase(xys.UI.HotTablePanel parent) : base(parent)
        {

        }

        public xys.UI.HotTablePanel tableParent { get { return (xys.UI.HotTablePanel)parent; } }

        // 页面列表
        List<HotTablePageBase> pages = new List<HotTablePageBase>();

        // 创建页面
        public void OnCreatePage(HotTablePageBase page)
        {
            page.SetID(pages.Count);
            pages.Add(page);
        }

        public HotTablePageBase GetPage(int id)
        {
            return pages[id];
        }

        public T GetPage<T>() where T : HotTablePageBase
        {
            string name = typeof(T).FullName;
            for (int i = 0; i < pages.Count; ++i)
            {
                if (pages[i].GetType().FullName == name)
                    return pages[i] as T;
            }

            return null;
        }

        protected virtual void OnShowPage(xys.UI.HotTablePage page)
        {

        }


        protected virtual bool OnPreChange(xys.UI.HotTablePage page)
        {
            return true;
        }
    }
}

#endif
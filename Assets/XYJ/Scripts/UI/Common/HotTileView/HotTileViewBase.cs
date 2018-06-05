#if !USE_HOT
namespace xys.hot.UI
{
    using UIWidgets;
    using System.Collections.Generic;

    [xys.UI.HotTileViewAttribute]
    abstract class HotTileViewBase<Parent, T1, T2> where T1 : HotTComponentBase where Parent : xys.UI.HotTileView
    {
        public HotTileViewBase(Parent parent)
        {
            this.parent = parent;
        }

        public Parent parent { get; private set; }

        public T1 GetItem(int index)
        {
            return (T1)((xys.UI.HotTComponent)parent.GetItem(index)).refType.Instance;
        }

        public T2 GetItemData(int index)
        {
            return (T2)parent.DataSource[index];
        }

        public void SetDataList(List<T2> datas)
        {
            List<object> objs = new List<object>(datas.Count);
            foreach (var obj in datas)
                objs.Add(obj);

            parent.DataSource.Set(objs);
        }

        protected abstract void SetData(T1 component, T2 item);

        // 选中某个项
        protected void SelectItem(int index)
        {
            OnSelectItem(index);
        }

        protected abstract void OnSelectItem(int index);

        // 某个项失去选中
        protected void DeselectItem(int index)
        {
            OnDeselectItem(index);
        }

        protected abstract void OnDeselectItem(int index);
    }
}
#endif
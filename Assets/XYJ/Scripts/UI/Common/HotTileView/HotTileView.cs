namespace xys.UI
{
    using UIWidgets;
    using UnityEngine;
    using System.Collections.Generic;

    public class HotTileViewAttribute : System.Attribute
    {

    }

    public class HotTileView : TileView<HotTComponent, object>, ILSerialized
    {
        [SerializeField]
        [HideInInspector]
        string typeName; // 对应的类型

        [SerializeField]
        [HideInInspector]
        List<Object> objs; // 对应的unit对象

        List<Object> ILSerialized.Objs { get { return objs; } }

        [SerializeField]
        [HideInInspector]
        string json; // 对应的json数据字段

        public RefType refType { get; private set; }

#if UNITY_EDITOR
        [System.NonSerialized]
        public bool isShowTypeInfo = false;
        object instance;
#endif

        protected override void Awake()
        {
            base.Awake();

            refType = new RefType(typeName, this);
            IL.MonoSerialize.MergeFrom(refType.Instance, new IL.MonoStream(new JSONObject(json), objs));
            json = null;
            objs = null;

            refType.InvokeMethod("Awake");
        }

        protected override void SetData(HotTComponent component, object item)
        {
            refType.InvokeMethod("SetData", component.refType.Instance, item);
        }

        protected override void SelectItem(int index)
        {
            refType.InvokeMethod("SelectItem", index);
        }

        protected override void DeselectItem(int index)
        {
            refType.InvokeMethod("DeselectItem", index);
        }

        public override void Clear()
        {
            base.Clear();
            refType.InvokeMethod("Clear");
        }
    }
}

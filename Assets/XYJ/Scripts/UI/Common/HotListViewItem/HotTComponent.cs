namespace xys.UI
{
    using UIWidgets;
    using UnityEngine;
    using System.Collections.Generic;

    public class HotTComponent : ListViewItem, ILSerialized
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
        object instance;
        [System.NonSerialized]
        public bool isShowTypeInfo = false;
#endif

        protected override void Awake()
        {
            base.Awake();

            refType = new RefType(typeName, this);
            IL.MonoSerialize.MergeFrom(refType.Instance, new IL.MonoStream(new JSONObject(json), objs));
            json = null;
            objs = null;
        }
    }
}
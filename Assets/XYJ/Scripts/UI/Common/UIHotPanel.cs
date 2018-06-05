using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.UI
{
#if UNITY_EDITOR
    // 挂了此脚本的，只会显示，此脚本里指定的类型，如果没有查找到，则会报错
    public class SinglePanelType : System.Attribute
    {
        public SinglePanelType(string type) { this.type = type; }
        public string type { get; private set; }
    }
#endif

    [RequireComponent(typeof(Canvas))]
    public class UIHotPanel : UIPanelBase, ILSerialized
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
        [EditorField]
        public bool isShowTypeInfo = true;
        string GetPanelType()
        {
            int last = typeName.LastIndexOf('.');
            if (last == -1)
                return typeName;

            return typeName.Substring(last + 1);
        }
#endif
        string PanelType;

        protected override void OnInit()
        {
            refType = new RefType(typeName, this);
            IL.MonoSerialize.MergeFrom(refType.Instance, new IL.MonoStream(new JSONObject(json), objs));
            json = null;
            objs = null;

            PanelType = refType.Type.Name;
            refType.InvokeMethod("Init");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            refType.InvokeMethod("Destroy");
        }

        protected override IEnumerator OnInitSync()
        {
            IEnumerator itor = (IEnumerator)refType.InvokeMethodReturn("InitSync");
            while (itor.MoveNext())
                yield return 0;

            yield break;
        }

        protected override void OnShow(object args)
        {
            refType.InvokeMethod("Show", args);
        }

        protected override IEnumerator OnShowSync(object args)
        {
            IEnumerator itor = (IEnumerator)refType.InvokeMethodReturn("ShowSync", args);
            while (itor.MoveNext())
                yield return 0;

            yield break;
        }

        protected override void OnBeginPlayAnimShow()
        {
            refType.InvokeMethod("BeginPlayAnimShow");
        }

        protected override void OnEndPlayAnimShow()
        {
            refType.InvokeMethod("EndPlayAnimShow");
        }

        protected override void OnBeginPlayAnimHide()
        {
            refType.InvokeMethod("BeginPlayAnimHide");
        }

        protected override void OnEndPlayAnimHide()
        {
            refType.InvokeMethod("EndPlayAnimHide");
        }

        protected override void OnHide()
        {
            refType.InvokeMethod("Hide");
        }
    }
}
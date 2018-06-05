namespace xys.UI
{
    using UIWidgets;
    using UnityEngine;
    using System.Collections.Generic;

    public class HotTComponent : ListViewItem, ILSerialized
    {
        [SerializeField]
        [HideInInspector]
        string typeName; // ��Ӧ������

        [SerializeField]
        [HideInInspector]
        List<Object> objs; // ��Ӧ��unit����

        List<Object> ILSerialized.Objs { get { return objs; } }

        [SerializeField]
        [HideInInspector]
        string json; // ��Ӧ��json�����ֶ�

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
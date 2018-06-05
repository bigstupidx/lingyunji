using Config;
using NetProto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.ItemObtainShow;

namespace xys.UI
{
    public class Obtain
    {
        public int id;              //物品ID
        public int count;           //物品数量

        public Obtain(int i, int v)
        {
            id = i;
            count = v;
        }
    }

    public class ObtainShow
    {
        public int id;
        public int value;
        public ItemObtainShowRule rule;
    }

    public class ObtainItemShowMgr : MonoBehaviour, ILSerialized
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
#endif
        public static void ShowObtain(List<Obtain> obtains)
        {
            App.my.uiSystem.obtainItemShowMgr.ShowRewards(obtains);
        }

        private void ShowRewards(List<Obtain> obtainList)
        {
            refType.InvokeMethod("ShowRewards", obtainList);
        }

        public void Init()
        {
            refType = new RefType(typeName, this);
            IL.MonoSerialize.MergeFrom(refType.Instance, new IL.MonoStream(new JSONObject(json), objs));
            json = null;
            objs = null;

            refType.InvokeMethod("Init");

            var v = typeof(Queue<WaveText>);
            v = typeof(Queue<int>);
            v = typeof(List<xys.UI.Obtain>);
        }
    }
}
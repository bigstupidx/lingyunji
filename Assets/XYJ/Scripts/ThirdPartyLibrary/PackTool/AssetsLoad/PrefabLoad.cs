#if USE_RESOURCESEXPORT
using UnityEngine;
using XTools;

namespace PackTool
{
    public partial class PrefabLoad : AssetLoad<GameObject>
    {
        static PrefabLoad()
        {
            CurrentLoading = 0;
        }

        public class PrefabData : Data
        {
            public PrefabData()
            {
                isEnd = false;
            }

            public void Reset(PrefabLoad l, ResourcesEnd<GameObject> e, object p, bool init)
            {
                base.Reset(l, e, p);
                isInit = init;
                isEnd = false;
            }

            public override void Release()
            {
                base.Release();
                isInit = false;
                isEnd = false;
            }

            public bool isInit = false;

            private bool isEnd = false;

            public float progress { get { return (load as PrefabLoad).progress; } }

            public override bool isDone { get { return isEnd; } }

            internal override void OnEnd(GameObject go)
            {
                if (!isCancel)
                {
                    asset = go;
                    isEnd = true;

                    if (isInit && go != null)
                        asset = GameObject.Instantiate(go) as GameObject;

                    if (End != null)
                    {
                        try
                        {
                            End(asset, Endp);
                        }
                        catch (System.Exception e)
                        {
                            Debuger.LogException(e);
                        }
                    }
                }

                End = null;
                Endp = null;

                SubRef();
            }
        }

        // 当前正在加载当中的资源
        public static int CurrentLoading { get; protected set; }

        public PrefabBeh prefabBeh { get { return asset == null ? null : asset.GetComponent<PrefabBeh>(); } }

        enum DestroyType
        {
            Manual, // 手动删除
            Auto, // 自动删除
            Destroy, // 已删掉
        }

        DestroyType destroyType = DestroyType.Manual;

        protected override Data Add(ResourcesEnd<GameObject> fun, object funp)
        {
            PrefabData data = CreateData() as PrefabData;
            data.Reset(this, fun, funp, false);
            mDataList.Add(data);

            return data;
        }

        protected override Data CreateData()
        {
            PrefabData pd = Buff<PrefabData>.Get();
            pd.AddRef();
            return pd;
        }

        protected override void FreeData(Data d)
        {
            d.Release();
            Buff<PrefabData>.Free(d as PrefabLoad.PrefabData);
        }

        static PrefabLoad Create(string url)
        {
            ++CurrentLoading;
            return CreateAsset<PrefabLoad>(url);
        }

        static public void ResetLoading()
        {
            CurrentLoading = 0;
        }

        public static string GetFullName(string group, string name)
        {
#if USE_RESOURCES
            return name;
#else
#if USE_RESOURCESEXPORT
            if (!name.EndsWith(".prefab", true, null))
                name = name + ".prefab";
#else
            name += ".prefab";
#endif
            string fullname = null;
            if (!string.IsNullOrEmpty(group))
                fullname = ResourcesGroup.GetFullPath(group, name);
            else
                fullname = name;

            return fullname;
#endif
        }

        // 资源加载
        static public void LoadGroup(string group, string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
            LoadFullPath(GetFullName(group, name), fun, funp, isinit, isAutoDestory);
        }

        static public PrefabData LoadAsyncGroup(string group, string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
            return LoadAsyncFullPath(GetFullName(group, name), fun, funp, isinit, isAutoDestory);
        }

        // 默认的资源组
        static public void Load(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
            LoadFullPath(GetFullName(ResourcesGroup.Prefab, name), fun, funp, isinit, isAutoDestory);
        }

        static public void LoadFullPath(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
            LoadFullPathImp(name, fun, funp, isinit, isAutoDestory);
        }

        static public PrefabData LoadAsync(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
            return LoadAsyncGroup(ResourcesGroup.Prefab, name, fun, funp, isinit, isAutoDestory);
        }

        static public PrefabData LoadAsyncFullPath(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
            PrefabData pd = LoadFullPathImp(name, fun, funp, isinit, isAutoDestory);
            pd.AddRef();
            return pd;
        }

        // 需要完整的路径
        static public PrefabData LoadFullPathImp(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
#if USE_RESOURCESEXPORT
            PrefabData data = PrefabLoad.LoadImp(name, fun, funp, Create) as PrefabData;
            if (isAutoDestory)
                ((PrefabLoad)data.load).destroyType = DestroyType.Auto;

            data.isInit = isinit;
            return data;
#else
            PrefabLoad load = new PrefabLoad();
            load.Reset(name);

#if USER_ALLRESOURCES
            if (AllResources.Instance != null)
                load.asset = AllResources.Instance.GetPrefab(name);
#else
#if USE_RESOURCES
            load.asset = AssetResoruce.Load<GameObject>(name);
#elif UNITY_EDITOR
            load.asset = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + name, typeof(GameObject));
#endif
#endif
            if (load.asset == null)
            {
                Debuger.LogError("name:" + name + " Resources Not Find!!");
            }

            PrefabLoad.PrefabData data = load.Add(fun, funp) as PrefabData;
            data.isInit = isinit;

//加载预制体故意加点延时,较好的模拟真实环境
#if COM_DEBUG
            AssetsLoad.s_delayTime = Random.Range(0.1f,0.15f);
#endif

            load.NextFrame();

#if COM_DEBUG
            AssetsLoad.s_delayTime = 0;
#endif
            return data;
#endif
        }

        // 如果资源当前已经加载完成，则会立即调用回调函数，而非在下一桢回调
        static public void LoadSync(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
            PrefabData data = LoadFullPathSyncImp(GetFullName(ResourcesGroup.Prefab, name), fun, funp, isinit, isAutoDestory);
            data.SubRef();
        }

        // 如果当前有此资源，会立即调用回调
        static PrefabData LoadFullPathSyncImp(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true, bool isAutoDestory = false)
        {
            PrefabData data = PrefabLoad.LoadImp(name, fun, funp, Create) as PrefabData;
            if (isAutoDestory)
                ((PrefabLoad)data.load).destroyType = DestroyType.Auto;

            data.isInit = isinit;
            data.AddRef();

            if (data.load.isDone)
            {
                data.load.Remove(data);

                data.OnEnd(data.load.asset);
                data.isCancel = true;
            }
            return data;
        }

        ComponentSave com_save = null;

        public float progress
        {
            get
            {
                if (isDone)
                    return 1.0f;

                if (com_save == null)
                    return 0.1f;

                return 0.1f + com_save.progress * 0.9f;
            }
        }

        protected override void LoadAsset(string name)
        {
            AssetBundleLoad.Load(name, OnLoadAssetEnd);
        }

#if UNITY_EDITOR
        public Object[] DependenceList { get; private set; }
#endif

        protected override void ReleaseImp()
        {
            base.ReleaseImp();

#if UNITY_EDITOR
            DependenceList = null;
#endif
            com_save = null;
        }

        // 回收自身
        protected override void FreeSelf()
        {
            destroyType = DestroyType.Manual;
            FreeAsset<PrefabLoad>(this);
        }
    }
}
#endif
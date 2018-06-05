#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    public partial class AssetLoad<T> : AssetLoadObject where T : UnityEngine.Object
    {
        static Dictionary<string, AssetLoadObject> _AlreadLoadList = new Dictionary<string, AssetLoadObject>();

        static public void AddKey(string key, AssetLoadObject obj)
        {
            try
            {
                _AlreadLoadList.Add(key, obj);
                if (!key.StartsWith("Default-"))
                    AllAsset.AlreadLoadList.Add(key, obj);
#if MEMORY_CHECK
                AssetSyncMgr.isDirty = true;
#endif
            }
            catch(System.Exception ex)
            {
                Debuger.ErrorLog("AddKey:{0}! type:{1}", key, typeof(T).Name);
                Logger.LogException(ex);
            }
        }

        protected void RemoveKey(string key)
        {
#if MEMORY_CHECK
            AssetSyncMgr.isDirty = true;
#endif
            _AlreadLoadList.Remove(key);
            AllAsset.AlreadLoadList.Remove(key);

            //Debuger.DebugLog("Unload:{0}", key);
        }

        public static AssetLoad<T> Get(string name)
        {
            AssetLoadObject assetload = null;
            if (_AlreadLoadList.TryGetValue(name, out assetload))
                return assetload as AssetLoad<T>;
            return null;
        }

        public override Object GetAssetObj()
        {
            return asset;
        }

        public static bool DestroyByName(string name)
        {
            AssetLoad<T> assetload = null;
            foreach (var itor in _AlreadLoadList)
            {
                assetload = itor.Value as AssetLoad<T>;
                if (assetload != null && assetload.asset != null && assetload.asset.name == name)
                {
                    return assetload.DestroySelf();
                }
            }

            return false;
        }

        // 彻底释放某个资源
        public static bool Destroy(string url)
        {
            AssetLoad<T> assetload = Get(url);
            if (assetload == null)
                return false;

            return assetload.DestroySelf();
        }

        public static bool Destroy(T o)
        {
            foreach (KeyValuePair<string, AssetLoadObject> itor in _AlreadLoadList)
            {
                AssetLoad<T> assetload = itor.Value as AssetLoad<T>;
                if (assetload.asset == o)
                    return assetload.DestroySelf();
            }

            return false;
        }

        public delegate AssetLoad<T> CreateAssetLoad(string name);

        public static void InitDebug()
        {
            XTools.TimerMgrObj.Instance.addFrameUpdate(CheckIsEnd, null);
        }

#if MEMORY_CHECK
        static HashSet<string> LastList = new HashSet<string>(); // 上一次的加载列表

        public static void GetAllLogInfo(System.Text.StringBuilder sb)
        {
            sb.AppendLine(string.Format("type:{0} total:{1}", typeof(T).Name, _AlreadLoadList.Count));
            foreach (KeyValuePair<string, AssetLoadObject> itor in _AlreadLoadList)
                sb.AppendLine(string.Format("   url:{0} Refcount:{1}", itor.Key, itor.Value.Refcount));

            if (LastList.Count != 0)
            {
                XTools.Utility.SetAddOrSub(LastList, _AlreadLoadList,
                    (string key, AssetLoadObject alo) =>
                    {
                        sb.AppendLine(string.Format("   type:{0} add:{1}", typeof(T).Name, key));
                    },
                    (string key) =>
                    {
                        sb.AppendLine(string.Format("   type:{0} sub:{1}", typeof(T).Name, key));
                    });
            }

            LastList.Clear();
            foreach (KeyValuePair<string, AssetLoadObject> itor in _AlreadLoadList)
                LastList.Add(itor.Key);
        }
#endif

        public static Dictionary<string, AssetLoadObject> GetAllList()
        {
            return _AlreadLoadList;
        }

        static bool CheckIsEnd(object p)
        {
            string text = "";
            AssetLoad<T> load = null;
            foreach (KeyValuePair<string, AssetLoadObject> itor in _AlreadLoadList)
            {
                load = itor.Value as AssetLoad<T>;
                if (!load.isDone)
                    text += itor.Key;
                else if (load.mDataList.Count != 0)
                    text += itor.Key;
            }

            return true;
        }

        static protected AssetLoad<T>.Data LoadImp(string name, ResourcesEnd<T> fun, object funp, CreateAssetLoad create)
        {
            AssetLoad<T>.Data data = null;
            AssetLoad<T> assetload = Get(name) as AssetLoad<T>;
            if (assetload != null)
            {
                data = assetload.Add(fun, funp);
                if (assetload.isDone)
                    assetload.NextFrame();
            }
            else
            {
                assetload = create(name);
                data = assetload.Add(fun, funp);
                AddKey(name, assetload);
                assetload.LoadAsset(name);
            }

            return data;
        }

        static Dictionary<int, AssetLoadObject> InstanceIDKeyList = new Dictionary<int, AssetLoadObject>();

        static public void Clear()
        {
            InstanceIDKeyList.Clear();
            foreach (KeyValuePair<string, AssetLoadObject> itor in GetAllList())
            {
                AssetLoad<T> assetload = itor.Value as AssetLoad<T>;
                if (assetload.isDone && (assetload.mDataList.Count == 0) && assetload.asset != null)
                {
                    assetload.isDone = false;
                    InstanceIDKeyList.Add(assetload.asset.GetInstanceID(), assetload);
                    assetload.asset = null;
                }
            }
        }

        public static Dictionary<int, UnityEngine.Object> Recover()
        {
            Object[] ts = Resources.FindObjectsOfTypeAll(typeof(T));
            Dictionary<int, UnityEngine.Object> allInsList = new Dictionary<int, UnityEngine.Object>();
            foreach (UnityEngine.Object t in ts)
                allInsList.Add(t.GetInstanceID(), t);

            AssetLoad<T> alo = null;
            Object asset = null;
            foreach (KeyValuePair<int, AssetLoadObject> itor in InstanceIDKeyList)
            {
                alo = itor.Value as AssetLoad<T>;
                if (allInsList.TryGetValue(itor.Key, out asset))
                {
                    alo.asset = asset as T;
                    alo.isDone = true;
                    alo.NextFrame();
                }
                else
                {
                    if (alo.mDataList.Count != 0)
                    {
                        // 资源重加载
                        alo.LoadAsset(alo.url);
                        Debuger.DebugLog("ReLoad:{0}", alo.url);
                    }
                    else
                    {
                        Debuger.DebugLog("Recover Release:{0}", alo.url);
                        alo.DestroySelf();
                    }
                }
            }

            return allInsList;
        }
    }
}

#endif
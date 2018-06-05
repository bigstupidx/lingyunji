#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace PackTool
{
    public class PrefabBeh : MonoBehaviour
    {
#if UNITY_EDITOR || MEMORY_CHECK
        public static int total = 0;
#endif
        public string url; // 预置体对象

        static public bool isAutoAdd = false; // 是否主动添加

        static public int wait_total = 0; // 等待计数的个数

        // 自动删除的资源列表
        public static Dictionary<string, PrefabLoad> AutoDestroyList = new Dictionary<string, PrefabLoad>();

        public static void Add(PrefabLoad pl)
        {
            if (AutoDestroyList.ContainsKey(pl.url))
            {
                Debuger.ErrorLog("PrefabBeh AutoDestroyList:{0}", pl.url);
                return;
            }
            AutoDestroyList.Add(pl.url, pl);
        }

        void AddRef()
        {
            isAddRef = true;

#if UNITY_EDITOR || MEMORY_CHECK
            ++total;
            Data d = Get();
            d.AddToLift(gameObject);
            d.total_lift_num = Mathf.Max(d.total_lift_num, d.lift_num);
            ++d.create_num;
#endif
            PrefabLoad p = PrefabLoad.Get(url) as PrefabLoad;
            if (p == null)
            {
                Debuger.ErrorLog("PrefabBeh {0} find error!", url);
                return;
            }

            p.AddRef();
            p.OnAutoDestroy();
        }

        bool isAddRef = false;

        PrefabBeh()
        {
            if (isAutoAdd)
                return;

            ++wait_total;
            XTools.TimerMgrObj.Instance.addFrameLateUpdate(CheckURL, this);

#if MEMORY_CHECK
            AssetSyncMgr.isDirty = true;
#endif
        }

        static bool CheckURL(object p)
        {
            --wait_total;
            PrefabBeh pb = p as PrefabBeh;
            if (pb == null)
                return false;

            pb.AddRef();
            return false;
        }

        void OnDestroy()
        {
#if MEMORY_CHECK
            AssetSyncMgr.isDirty = true;
#endif
            if (!isAddRef)
                return;

#if UNITY_EDITOR || MEMORY_CHECK
            --total;
            AllList[url].RemoveToLift(gameObject);
#endif
            PrefabLoad p = PrefabLoad.Get(url) as PrefabLoad;
            if (p == null)
            {
                if (!AutoDestroyList.TryGetValue(url, out p))
                {
                    Debuger.ErrorLog("PrefabBeh {0} find error!", url);
                    return;
                }
                else
                {
                    p.SufRef();
                    if (p.Refcount == 0)
                    {
                        p.DestroySelf();
                        AutoDestroyList.Remove(url);
                        Debuger.DebugLog("Auto Destroy:{0}!", url);
                    }
                }
            }
            else
            {
                p.SufRef();
            }
        }

#if UNITY_EDITOR || MEMORY_CHECK
        public class Data
        {
            public string url;
            public int create_num = 0; // 总共的创建次数
            public int load_num = 0; // 当前加载的次数
            public int lift_num = 0; // 在前存活的最高数量
            public int total_lift_num = 0; // 最高存活个数
            public int active_total = 0; // 激活的历史最高个数
            public int active_current = 0; // 当前激活的个数

#if MEMORY_CHECK
            public override string ToString()
            {
                return url;
            }

            public void Write(Network.BitStream bs)
            {
                if (string.IsNullOrEmpty(url))
                    bs.Write("");
                else
                    bs.Write(url);
                bs.Write(create_num);
                bs.Write(load_num);
                bs.Write(total_lift_num);
                bs.Write(active_total);
                bs.Write(active_current);
                bs.Write(lift_num);
            }

            public void Reader(Network.BitStream bs)
            {
                url = bs.ReadString();
                create_num = bs.ReadInt32();
                load_num = bs.ReadInt32();
                total_lift_num = bs.ReadInt32();
                active_total = bs.ReadInt32();
                active_current = bs.ReadInt32();
                lift_num = bs.ReadInt32();
            }
#endif

            List<GameObject> liftList = new List<GameObject>();
            public List<GameObject> LiftList
            {
                get { return liftList; }
            }

            public void AddToLift(GameObject go)
            {
                ++lift_num;
                liftList.Add(go);
            }

            public void RemoveToLift(GameObject go)
            {
                --lift_num;
                liftList.Remove(go);
            }

            public string Text
            {
                get
                {
                    return string.Format("加载总次数:{2} 创建总个数:{0} 当前存活个数:{1} 历史最高存活个数:{3} 激活个数:{4} 最高激活个数:{5}", create_num, lift_num, load_num, total_lift_num, active_current, active_total);
                }
            }

            public void AddActive()
            {
                ++active_current;
                if (active_current > active_total)
                    active_total = active_current;
            }

            public void SubActive()
            {
                --active_current;
            }
        }

        static XTools.Map<string, Data> AllList = new XTools.Map<string, Data>();

        public static XTools.Map<string, Data> GetAll() { return AllList; }

        public static Data Get(string url)
        {
            Data d = null;
            if (AllList.TryGetValue(url, out d))
                return d;

            d = new Data();
            d.url = url;
            AllList.Add(url, d);

            return d;
        }

        public Data Get()
        {
            return Get(url);
        }

        void OnEnable()
        {
            Get().AddActive();
        }

        void OnDisable()
        {
            Get().SubActive();
        }

        void OnApplicationQuit()
        {
            AllList.Clear();
        }

        public static void GetAllLogInfo(System.Text.StringBuilder sb)
        {
            sb.AppendFormat("Prefab Load:{0}", AllList.Count);
            sb.AppendLine();
            int total = 0;
            foreach (KeyValuePair<string, Data> itor in AllList)
            {
                total += itor.Value.lift_num;
                sb.AppendFormat("url:{0} has:{1} {2}", itor.Key, PrefabLoad.Get(itor.Key) == null ? false : true, itor.Value.Text);
                sb.AppendLine();
            }

            sb.AppendLine("prefab total:" + total);
        }
#endif
    }
}
#endif
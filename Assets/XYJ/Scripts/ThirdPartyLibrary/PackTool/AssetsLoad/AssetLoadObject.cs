#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    public abstract class AssetLoadObject
#if MEMORY_CHECK
        : MemoryObject
#endif
    {
        public string url { get; protected set; }

        // 是否完成
        public bool isDone { get; protected set; }

        // 当前请求此份资源的数量
        public abstract int request_count { get; }

        public bool DestroySelf()
        {
            if (!DestroyImp())
                return false;

            SubDepRef();

            FreeSelf();

            return true;
        }

        public void Release()
        {
            url = null;

            isAddSubRef = false;
            live_count = 0;
            dependences.Clear();

            ReleaseImp();
        }

        public abstract void Reset(string u);

        protected abstract void ReleaseImp();

        protected abstract bool DestroyImp();

        // 回收自身
        protected abstract void FreeSelf();

        public abstract Object GetAssetObj();

        int live_count = 0; // 当前存活的预置体的个数

        public int Refcount { get { return live_count; } }

        // 计数减少
        public virtual void SufRef()
        {
            --live_count;
            //Debuger.Log(string.Format("SufRef url:{0} live_count:{1}", url, live_count));
        }

        public virtual void AddRef()
        {
            ++live_count;
            //Debuger.Log(string.Format("AddRef url:{0} live_count:{1}", url, live_count));
        }

        // 当前的依赖列表
        private List<string> dependences = new List<string>();

        public bool isAddSubRef = false; // 是否已增加了依赖计数

        public List<string> GetDependences()
        {
            return dependences;
        }

        public void AddDep(string key)
        {
            if (key.LastIndexOf('.') == -1)
                return;

            if (dependences.Contains(key))
                return;

            dependences.Add(key);
        }

        public bool AddDepRef()
        {
            if (isAddSubRef)
                return false;

            isAddSubRef = true;
            AssetLoadObject obj = null;

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendFormat("AddDepRef:{0} num:{1}", url, dependences.Count);
            //sb.AppendLine();

            for (int i = 0; i < dependences.Count; ++i)
            {
                if (AllAsset.AlreadLoadList.TryGetValue(dependences[i], out obj))
                {
                    obj.AddRef();
                    //sb.AppendLine(dependences[i]);
                }
                else
                {
                    Debuger.ErrorLog("url:{0} dep:{1} not find!", url, dependences[i]);
                }
            }

            //Debuger.Log(sb.ToString());

            return true;
        }

        public bool SubDepRef()
        {
            if (!isAddSubRef)
                return false;

            isAddSubRef = false;
            AssetLoadObject obj = null;
            foreach (string key in dependences)
            {
                if (AllAsset.AlreadLoadList.TryGetValue(key, out obj))
                    obj.SufRef();
            }
            return true;
        }

        protected static T CreateAsset<T>(string name) where T : PackTool.AssetLoadObject, new()
        {
            T matload = XTools.Buff<T>.Get();
            matload.Reset(name);
            return matload;
        }

        protected static void FreeAsset<T>(T obj) where T : PackTool.AssetLoadObject, new()
        {
            obj.Release();
            XTools.Buff<T>.Free(obj);
        }
    }
}
#endif
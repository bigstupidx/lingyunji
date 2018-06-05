#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;

namespace PackTool
{
    public partial class AssetLoad<T> : AssetLoadObject where T : UnityEngine.Object
    {
#if ASSET_DEBUG
        public static TimeTrack AssetLoadInit = TimeTrackMgr.Instance.Get("ComponentSave.AssetLoad");
        object asset_key;
#endif

        protected AssetLoad()
        {

        }

        public override void Reset(string u)
        {
            url = u;
            isDone = false;
            asset = null;

#if ASSET_DEBUG
            asset_key = AssetLoadInit.Begin(u);
#endif
        }

        public T asset { get; protected set; }

        public class Data
#if MEMORY_CHECK
        : MemoryObject
#endif
        {
            public Data()
            {

            }

            public void Reset(AssetLoad<T> l, ResourcesEnd<T> e, object p)
            {
                load = l;
                End = e;
                Endp = p;
            }

            public virtual void Release()
            {
                asset = null;
                End = null;
                Endp = null;
                isCancel = false;
            }

            public string url { get { return load.url; } }

            public AssetLoad<T> load { get; protected set; }

            public T asset { get; protected set; }

            public ResourcesEnd<T> End;// 回调
            public object Endp;
            public bool isCancel = false; // 是否取消

            public virtual bool isDone { get { return load.isDone; } }

            public int ref_count = 0;

            public void AddRef()
            {
                ++ref_count;
            }

            public void SubRef()
            {
                --ref_count;
                if (ref_count == 0)
                {
                    load.FreeData(this);
                }
                else if(ref_count < 0)
                {
                    Debuger.ErrorLog("ref_count < 0! {0}", ref_count);
                }
            }

            internal virtual void OnEnd(T data)
            {
                if (!isCancel)
                {
                    asset = data;
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

        protected List<Data> mDataList = new List<Data>();

        // 当前请求此份资源的数量
        public override int request_count { get { return mDataList.Count; } }

        protected virtual Data Add(ResourcesEnd<T> fun, object funp)
        {
            Data data = CreateData();
            data.Reset(this, fun, funp);
            mDataList.Add(data);

            return data;
        }

        public virtual bool Remove(Data d)
        {
            for (int i = mDataList.Count - 1; i >= 0; --i)
            {
                if (mDataList[i] == d)
                {
                    mDataList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        protected virtual Data CreateData()
        {
            Data d = Buff<AssetLoad<T>.Data>.Get();
            d.AddRef();
            return d;
        }

        protected virtual void FreeData(Data d)
        {
            d.Release();
            Buff<AssetLoad<T>.Data>.Free(d);
        }

        protected void OnEnd()
        {
#if ASSET_DEBUG
            if (asset_key != null)
            {
                AssetLoadInit.End(asset_key);
                asset_key = null;
            }
#endif
            isDone = true;
            int num = mDataList.Count;
            for (int i = 0; i < num; ++i)
            {
                mDataList[i].OnEnd(asset);
                mDataList[i] = null;
            }

            mDataList.RemoveRange(0, num);
        }

        protected bool NextFrame(object p)
        {
            frameupdate = null;
            OnEnd();
            return false;
        }

        protected TimerFrame.Frame frameupdate = null;

        protected virtual void LoadAsset(string name)
        {
            Debuger.LogError("AssetLoad<T>.LoadAsset()" + name);
        }

        protected void NextFrame()
        {
            if (frameupdate == null)
            {
                if (mDataList.Count == 0)
                    return;

                frameupdate = XTools.TimerMgrObj.Instance.AddLateUpdate(NextFrame, null);
            }
        }

        protected virtual void DestroyAssetAsset()
        {
            Object.DestroyImmediate(asset, true);
        }

        protected override bool DestroyImp()
        {
            if (mDataList.Count != 0)
                return false;

            if (asset != null)
            {
                if (url.StartsWith(":"))
                {
                    Resources.UnloadAsset(asset);
                }
                else
                {
                    DestroyAssetAsset();
                }
            }

            RemoveKey(url);
            return true;
        }

        protected override void ReleaseImp()
        {
            mDataList.Clear();
            frameupdate = null;
            asset = null;
            isDone = false;

#if ASSET_DEBUG
            asset_key = null;
#endif
        }

        protected override void FreeSelf()
        {
            Debuger.ErrorLog(typeof(T).Name);
            throw new System.NotImplementedException();
        }
    }
}
#endif
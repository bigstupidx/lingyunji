using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 由于预制体是异步加载的，不小心使用会导致特效创建多个，或者特效没有销毁等bug
/// 例如打开面板会加载一个角色模型，模型没加载完就关闭了面板,这时候很可能会导致模型没有正常销毁了.
/// 使用该类能方便的解决这种问题
/// 多次调用Load,Destroy也能保证只有一个实例
/// </summary>
/// 
namespace xys
{
    public class PrefabsLoadReference:ILoadReference
#if MEMORY_CHECK
        : MemoryObject
#endif
    {
        enum LoadState
        {
            Null,
            Loading,
            FinishLoad,
        }

        //创建对象
        public GameObject m_go { get; private set; }

        LoadState m_loadState;
        //是否可见
        bool m_visable = true;

        bool m_usepool = false;

        string m_name;
        public PrefabsLoadReference(bool useObjectPool = false)
        {
            m_usepool = useObjectPool;
        }
        //是否已经加载完成
        public bool IsLoad()
        {
            return m_go != null;
        }

        //正在加载
        public bool IsLoading()
        {
            return m_loadState == LoadState.Loading;
        }

        public bool IsDestroy()
        {
            return m_loadState == LoadState.Null;
        }

        public void SetDestroy()
        {
            if (m_go != null)
            {
                if (m_usepool)
                    XYJObjectPool.Destroy(m_go);
                else
                    GameObject.Destroy(m_go);
            }

            m_go = null;
            m_loadState = LoadState.Null;
        }

        //多次调用也会保证只加载一个
        public void Load(string name,Action<GameObject, object> fun, object funp,Vector3 pos,Quaternion rot)
        {
            //没有加载或者加载的对象被外部销毁了
            if (m_loadState == LoadState.Null || (m_loadState == LoadState.FinishLoad && m_go == null))
            {
                m_loadState = LoadState.Loading;
                m_name = name;
                if (m_usepool)
                    XYJObjectPool.LoadPrefab(name, OnFinishLoadPool, new object[] { fun, funp },pos,rot, false);
                else
                    RALoad.LoadPrefab(name, OnFinishLoad, new object[] { fun, funp, pos, rot }, false);
            }
        }

        public void Load(string name,Action<GameObject, object> fun=null, object funp=null )
        {
            Load(name, fun, funp, Vector3.zero, Quaternion.identity);
        }

        //设置可见,没有会先创建.fun每次show都会调用
        public void Show(string name, Action<GameObject, object> fun = null, object funp = null)
        {
            m_visable = true;
            if (IsLoad())
            {
                m_go.SetActive(true);
                if (fun != null)
                    fun(m_go, funp);
            }
            else
                Load(name, fun, funp,Vector3.zero,Quaternion.identity);
        }

        //设置不可见
        public void Hide()
        {
            m_visable = false;
            if (IsLoad())
                m_go.SetActive(false);
        }

        void OnFinishLoadPool(GameObject go, object para)
        {
            //外部调用了销毁
            if (m_loadState == LoadState.Null)
            {
                XYJObjectPool.Destroy(go);
                return;
            }

            //加载的不是同一个
            if (m_name != go.name)
            {
                Debuger.LogError(string.Format("加载的资源不是同一个 go1={0} go2{1}", m_name, go.name));
                GameObject.Destroy(go);
                return;
            }
 

            if (m_go != null)
            {
                Debuger.LogError("产生多个实例 "+go.name);
                XYJObjectPool.Destroy(go);
                return;
            }
            m_go = go;
#if !SCENE_DEBUG
            //有些特效是根据时间删除，而不是手动删除的
            ObjectPoolDestroy poolDestroy = go.GetComponent<ObjectPoolDestroy>();
            if (poolDestroy != null)
                poolDestroy.m_prefabsLoadReference = this;
#endif

            m_loadState = LoadState.FinishLoad;
            m_go.SetActive(m_visable);
            object[] pList = para as object[];
            Action<GameObject, object> fun = pList[0] as Action<GameObject, object>;
            if (fun != null)
                fun(m_go, pList[1]);
        }

        void OnFinishLoad(GameObject prefabs, object para)
        {
            //外部调用了销毁
            if (m_loadState == LoadState.Null)
            {
                return;
            }

            //加载的不是同一个
            if (m_name != prefabs.name)
            {
                Debuger.LogError(string.Format("加载的资源不是同一个 go1={0} go2{1}", m_name, prefabs.name));
                return;
            }


            if (m_go != null)
            {
                Debuger.LogError("产生多个实例");
                return;
            }

            object[] pList = para as object[];
            m_loadState = LoadState.FinishLoad;
            m_go = GameObject.Instantiate(prefabs,(Vector3)pList[2],(Quaternion)pList[3]);
            m_go.SetActive(m_visable);

            Action<GameObject, object> fun = pList[0] as Action<GameObject, object>;
            if (fun != null)
                fun(m_go, pList[1]);
        }

    }

}

using UnityEngine;
using System.Collections;
using System;


namespace xys
{

    public interface ILoadReference
    {
        bool IsDestroy();
        void SetDestroy();
    }
    public class MaterialLoadReference : ILoadReference
    {
        enum LoadState
        {
            Null,
            Loading,
            FinishLoad,
        }

        LoadState m_loadState;
        public void SetDestroy()
        {
            m_loadState = LoadState.Null;
        }

        //多次调用也会保证只加载一个
        public void Load(string name, Action<Material, object> fun, object funp)
        {
            m_loadState = LoadState.Loading;
            RALoad.LoadMaterail(name, OnFinishLoad, new object[] { fun, funp});
        }

        public bool IsDestroy()
        {
            return m_loadState == null;
        }


        void OnFinishLoad(Material mat, object para)
        {
            //外部调用了销毁
            if (m_loadState == LoadState.Null)
            {
                return;
            }

            object[] pList = para as object[];
            m_loadState = LoadState.FinishLoad;
            Action<Material, object> fun = pList[0] as Action<Material, object>;
            if (fun != null)
                fun(mat, pList[1]);
        }

    }

}

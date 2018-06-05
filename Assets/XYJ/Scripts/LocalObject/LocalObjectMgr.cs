namespace xys
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 本地纯客户端角色
    /// （后面把故事角色类型也定义到这里）
    /// </summary>
    public enum LocalObjectType
    {
        Normal,          // 自己定义
        MoneyTreeObject, // 摇钱树对象
    }

    /// <summary>
    /// 本地角色管理类
    /// </summary>
    public class LocalObjectMgr
    {

        Dictionary<int, ILocalObject> m_objMap = new Dictionary<int, ILocalObject>();
        List<ILocalObject> m_objList = new List<ILocalObject>();

        public LocalObjectMgr()
        {

        }

        public Dictionary<int, ILocalObject> GetObjMap()
        {
            return m_objMap;
        }

        public List<ILocalObject> GetObjList()
        {
            return m_objList;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ILocalObject GetObj(int uid)
        {
            ILocalObject obj = null;
            if (m_objMap.TryGetValue(uid, out obj))
                return obj;

            return null;
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="type"></param>
        /// <param name="roleId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ILocalObject CreateObj(LocalObjectType type, int roleId, object data)
        {
            ILocalObject obj = null;
            switch (type)
            {
                case LocalObjectType.Normal:
                    obj = new LocalNormalActor();
                    break;
                case LocalObjectType.MoneyTreeObject:
                    obj = new MoneyTreeActor();
                    break;
            }

            if (obj != null)
            {
                obj.InitData(GenObjectID(), data);
                AddObj(obj);
                return obj;
            }

            return null;
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="uid"></param>
        public void RemoveObj(int uid)
        {
            DeleteObj(uid);
        }

        /// <summary>
        /// 销毁管理类
        /// </summary>
        public void Destory()
        {
            ClearAllObjects();
            ResetID();
        }

        /// <summary>
        /// 更新角色管理
        /// </summary>
        public void Update()
        {
            UpdateAllObjects();
        }

        #region 内部实现

        static int _ObjectID = 0;
        static int GenObjectID()
        {
            return _ObjectID++;
        }
        static void ResetID()
        {
            _ObjectID = 0;
        }

        void AddObj(ILocalObject obj)
        {
            if (m_objMap.ContainsKey(obj.uid))
            {
                Debuger.LogError(string.Format("重复本地添加对象, uid={0}", obj.uid));
                return;
            }

            m_objMap.Add(obj.uid, obj);
            m_objList.Add(obj);
        }

        void DeleteObj(int uid)
        {
            ILocalObject obj = null;
            if (m_objMap.TryGetValue(uid, out obj))
            {
                obj.Destroy();

                m_objMap.Remove(uid);
                m_objList.Remove(obj);
            }
        }

        void UpdateAllObjects()
        {
            for (int i = 0; i < m_objList.Count; ++i)
            {
                m_objList[i].Update();
            }
        }


        void ClearAllObjects()
        {
            for (int i = 0; i < m_objList.Count; ++i)
            {
                m_objList[i].Destroy();
            }

            m_objList.Clear();
            m_objMap.Clear();
        }

        #endregion


    }
}
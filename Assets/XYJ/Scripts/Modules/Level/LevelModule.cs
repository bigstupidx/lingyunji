using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    public class LevelModule : HotModule
    {
        public LevelModule() : base("xys.hot.HotLevelModule")
        {
            if(levelMgr != null) { }
        }

        private RefType m_levelMgrType;
        private object m_levelMgr;

        public object levelMgr
        {
            get
            {
                if (null == m_levelMgr)
                {
                    m_levelMgr = refType.GetField("m_levelMgr");
                    m_levelMgrType = new RefType(m_levelMgr);
                }
                return m_levelMgr;
            }
        }

        //关卡配置id
        public int levelId
        {
            get
            {
                return m_levelMgrType.GetProperty<int>("levelId");
            }
        }

        //zoneId
        public long zoneId
        {
            get
            {
                return m_levelMgrType.GetProperty<long>("zoneId");
            }
        }

        //获取事件列表
        public List<LevelDesignConfig.LevelEventObjData> GetEventList(bool overAll)
        {
            List<LevelDesignConfig.LevelEventObjData> ret = null;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("GetEventList", overAll);
                ret = null != retVal ? (List<LevelDesignConfig.LevelEventObjData>)retVal : null;
            }
            return ret;
        }

        //获取点集数据
        public LevelDesignConfig.LevelPointData GetPointData(string id)
        {
            LevelDesignConfig.LevelPointData ret = null;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("GetPointData", id);
                ret = null != retVal ? (LevelDesignConfig.LevelPointData)retVal : null;
            }
            return ret;
        }

        /// <summary>
        /// 获取路径数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PathPointVo> GetPathData(string id)
        {
            List<PathPointVo> ret = null;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("GetPathById", id);
                ret = null != retVal ? (List<PathPointVo>)retVal : null;
            }
            return ret;
        }

        //获取条件vo
        public Dictionary<string, Config.EventMonitorVo> GetConditionVo()
        {
            Dictionary<string, Config.EventMonitorVo> ret = null;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("GetConditionVo");
                ret = null != retVal ? (Dictionary<string, Config.EventMonitorVo>)retVal : null;
            }
            return ret;
        }

        //是否贴地
        public bool SpawnNeedToGround(int spawnId)
        {
            bool ret = false;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("SpawnNeedToGround", spawnId);
                ret = null != retVal ? (bool)retVal : false;
            }
            return ret;
        }

        //获取刷新点大小   
        public Vector3 GetSpawnScale(int spawnId)
        {
            Vector3 ret = Vector3.one;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("GetSpawnScale", spawnId);
                ret = null != retVal ? (Vector3)retVal : Vector3.one;
            }
            return ret;
        }

        //获取刷新点数据
        public LevelDesignConfig.LevelSpawnData GetSpawnData(int spawnId)
        {
            LevelDesignConfig.LevelSpawnData ret = null;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("GetSpawnData", spawnId);
                ret = null != retVal ? (LevelDesignConfig.LevelSpawnData)retVal : null;
            }
            return ret;
        }

        //获取关卡剩余时间
        public int GetLevelLastTime()
        {
            int ret = 0;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("GetLevelLastTime");
                ret = null != retVal ? (int)retVal : 0;
            }
            return ret;
        }

        //关卡是否能主动退出
        public bool CanInitiativeExit(bool needCheckLeader)
        {
            bool ret = false;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("CanInitiativeExit", needCheckLeader);
                ret = null != retVal ? (bool)retVal : false;
            }
            return ret;
        }

        //判断关卡次数
        public bool CheckLevelCount(int id)
        {
            bool ret = false;
            if (null != levelMgr)
            {
                object retVal = m_levelMgrType.InvokeMethodReturn("CheckLevelCount", id);
                ret = null != retVal ? (bool)retVal : false;
            }
            return ret;
        }

        //执行一个事件
        public void DoEvent(string eventId)
        {
            if (null != levelMgr)
            {
                m_levelMgrType.InvokeMethod("DoEvent", eventId);
            }
        }
    }
}


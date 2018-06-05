using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.json;

namespace xys.GameStory
{
    [System.Serializable]
    public class StoryConfig : IJsonFile
    {
        public static bool TryGet(string name, out StoryConfig config)
        {
            return JsonConfigMgr.StoryConfigs.TryGet(name, out config);
        }

        public string GetKey()
        {
            return storyID;
        }
        public void SetKey(string key)
        {
            storyID = key;
        }

        // 基本信息
        public string storyID = string.Empty;
        public string description = string.Empty;

        // 设置信息
        public bool stopGameLogic = false;// 中断游戏逻辑
        public bool isCanSkip = false;//是否能跳过该剧情
        public int shieldActorsType = 0;//屏蔽角色类型

        // 对象列表
        public List<StoryObjectElement> objectList = new List<StoryObjectElement>();

        // 事件列表
        public List<StoryEventElement> eventList = new List<StoryEventElement>();

        // 点集列表
        public List<Points> pointsList = new List<Points>();

        // 路点列表
        public List<WayPoints> wayPointsList = new List<WayPoints>();

        // 镜头路径列表
        public List<CamPoints> camPointsList = new List<CamPoints>();

        /// <summary>
        /// 获取刷新点对象配置
        /// </summary>
        /// <param name="refreshId"></param>
        /// <returns></returns>
        public StoryObjectElement GetObject(string refreshId)
        {
            for (int i=0; i<objectList.Count; ++i)
            {
                if (refreshId == objectList[i].storyObjectID)
                {
                    return objectList[i];
                }
            }

            Debuger.LogError(string.Format("剧情={0}，没有找到刷新点id={1}", storyID, refreshId));
            return null;
        }

        public Points GetPoints(string id)
        {
            for (int i = 0; i < pointsList.Count; ++i)
            {
                if (id == pointsList[i].id)
                    return pointsList[i];
            }
            return null;
        }

        public WayPoints GetWayPoints(string id)
        {
            for (int i = 0; i < wayPointsList.Count; ++i)
            {
                if (id == wayPointsList[i].id)
                    return wayPointsList[i];
            }
            return null;
        }

        public CamPoints GetCamPoints(string id)
        {
            for (int i = 0; i < camPointsList.Count; ++i)
            {
                if (id == camPointsList[i].id)
                    return camPointsList[i];
            }
            return null;
        }
        

        #region Editor Points Methods

        public List<string> GetIDList<T>() where T : Points
        {
            List<string> idList = new List<string>();
            Points p;
            if (typeof(T).Equals(typeof(Points)))
            {
                for (int i = 0; i < pointsList.Count; ++i)
                {
                    p = pointsList[i];
                    if (!idList.Contains(p.id))
                        idList.Add(p.id);
                }
            }
            else if (typeof(T).Equals(typeof(WayPoints)))
            {
                for (int i = 0; i < wayPointsList.Count; ++i)
                {
                    p = wayPointsList[i];
                    if (!idList.Contains(p.id))
                        idList.Add(p.id);
                }
            }
            else if (typeof(T).Equals(typeof(CamPoints)))
            {
                for (int i = 0; i < camPointsList.Count; ++i)
                {
                    p = camPointsList[i];
                    if (!idList.Contains(p.id))
                        idList.Add(p.id);
                }
            }

            return idList;
        }

        public void ClearPoints<T>() where T : Points
        {
            if (typeof(T).Equals(typeof(Points)))
            {
                pointsList.Clear();
            }
            else if (typeof(T).Equals(typeof(WayPoints)))
            {
                wayPointsList.Clear();
            }
            else if (typeof(T).Equals(typeof(CamPoints)))
            {
                camPointsList.Clear();
            }
        }

        public void AddPoints<T>() where T : Points
        {
            if (typeof (T).Equals(typeof(Points)))
            {
                pointsList.Add(new Points());
            }
            else if(typeof(T).Equals(typeof(WayPoints)))
            {
                wayPointsList.Add(new WayPoints());
            }
            else if (typeof(T).Equals(typeof(CamPoints)))
            {
                camPointsList.Add(new CamPoints());
            }
        }

        public void RemovePointsAt<T>(int index) where T : Points
        {
            if (typeof(T).Equals(typeof(Points)))
            {
                pointsList.RemoveAt(index);
            }
            else if (typeof(T).Equals(typeof(WayPoints)))
            {
                wayPointsList.RemoveAt(index);
            }
            else if (typeof(T).Equals(typeof(CamPoints)))
            {
                camPointsList.RemoveAt(index);
            }
        }

        #endregion

        #region Editor StoryEvent Methods

        int m_MaxEventID = 0;// event id
        public int GenerateEventID()
        {
            return ++m_MaxEventID;
        }

        /// <summary>
        /// 用来初始化事件数据对象
        /// </summary>
        public void InitEvents()
        {
            for (int i=0; i<eventList.Count; ++i)
            {
                eventList[i].GetEventData();
            }
        }

        public void ClearEvents ()
        {
            eventList.Clear();
        }

        public void AddEvent()
        {
            StoryEventElement element = new StoryEventElement();
            element.id = GenerateEventID();
            eventList.Add(element);
        }

        public void MoveEvent(int index, StoryEventElement source)
        {
            StoryEventElement element = new StoryEventElement(source);
            element.id = GenerateEventID();
            eventList.Insert(index, element);
            eventList.Remove(source);
        }

        public void CloneEvent(StoryEventElement source)
        {
            StoryEventElement element = new StoryEventElement(source);
            element.id = GenerateEventID();
            eventList.Add(element);
        }

        public void RemoveEventAt(int index)
        {
            eventList.RemoveAt(index);
        }

        #endregion

        #region Object Methods

        public void ClearObjects()
        {
            objectList.Clear();
        }

        public void AddObject()
        {
            StoryObjectElement element = new StoryObjectElement();
            objectList.Add(element);
        }

        public void RemoveObjectAt(int index)
        {
            objectList.RemoveAt(index);
        }

        #endregion

    }
}


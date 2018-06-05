using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{

    // 路点循环模式
    public enum WayRepeatMode
    {
        Once = 0,
        Loop,
        PingPong,
        ClampForever,
        Random,
    }

    public enum WayPointEventType
    {
        动作,
        冒泡,
        特效,
        转向,
        隐藏,
        出现,
    }

    [System.Serializable]
    public class WayPoints : Points
    {

        public WayRepeatMode repeatMode = WayRepeatMode.Once;

        public bool useDefaultSpeed = true;

        public List<WayPointData> wayDatas = new List<WayPointData>();

        #region 镜头数据操作方法

        public void AddWayData()
        {
            wayDatas.Add(new WayPointData());
        }

        public void InsertWayData(int index, WayPointData data)
        {
            wayDatas.Insert(index, data);
        }

        public void InsertWayData(int index)
        {
            wayDatas.Insert(index, new WayPointData());
        }

        public void RemoveWayDataAt(int index)
        {
            wayDatas.RemoveAt(index);
        }

        public void MoveWayData(int sourceIndex, int targetIndex)
        {
            if (sourceIndex != targetIndex &&
                sourceIndex >= 0 && sourceIndex <= positions.Count &&
                targetIndex >= 0 && targetIndex <= positions.Count)
            {
                InsertWayData(targetIndex, wayDatas[sourceIndex]);
                RemoveWayDataAt(sourceIndex);
            }
        }

        #endregion

        public override void ClearAll()
        {
            base.ClearAll();

            wayDatas.Clear();
        }
    }

    [System.Serializable]
    public class WayPointData
    {
        public float waitTime = 0.0f;
        public float moveSpeed = 0.0f;

        public List<WayPointEvent> pointEvents;

        public WayPointData()
        {
            waitTime = 0.0f;
            pointEvents = new List<WayPointEvent>();
        }

        public void Add(WayPointEventType type)
        {
            pointEvents.Add(new WayPointEvent(type));
        }

        public void RemoveAt(int index)
        {
            pointEvents.RemoveAt(index);
        }

        public void Clear()
        {
            pointEvents.Clear();
        }
    }

    [System.Serializable]
    public class WayPointEvent
    {

        public WayPointEventType eventType;
        public string eventParam;

        public WayPointEvent(WayPointEventType type)
        {
            eventType = type;
            eventParam = string.Empty;
        }

    }

}
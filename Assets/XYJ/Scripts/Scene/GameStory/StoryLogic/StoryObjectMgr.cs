using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class StoryObjectMgr
    {
        StoryPlayer m_storyPlayer;


        List<StoryObjectBase> m_objList = new List<StoryObjectBase>();
        Dictionary<int, StoryObjectBase> m_objMap = new Dictionary<int, StoryObjectBase>();

        public void Init(StoryPlayer player)
        {
            m_storyPlayer = player;
        }

        public void OnLoadSceneStart()
        {
            ClearObjects();
        }

        public void OnLoadSceneEnd()
        {
            ResetID();
        }

        public void OnStoryStart()
        {
            ClearObjects();
        }

        public void OnStoryEnd()
        {
            ClearObjects();
        }

        public void OnStoryUpdate()
        {
            UpdataObj();
        }

        public List<StoryObjectBase> GetRefreshObjects(string refreshId)
        {
            List<StoryObjectBase> objs = new List<StoryObjectBase>();
            for (int i = 0; i < m_objList.Count; ++i)
            {
                if (m_objList[i].refreshID == refreshId)
                {
                    objs.Add(m_objList[i]);
                }
            }
            return objs;
        }

        #region 对象管理

        static int _ObjectID = 0;
        static int GenObjectID()
        {
            return _ObjectID++;
        }
        static void ResetID()
        {
            _ObjectID = 0;
        }

        public void CreateRefresh(string refreshId)
        {
            StoryObjectElement element = m_storyPlayer.ConfigData.GetObject(refreshId);
            if (element == null)
                return;

            if (element.points.Count == 0)
            {
                Debuger.LogError("没有位置点信息");
                return;
            }

            for (int i=0; i<element.points.Count; ++i)
            {
                CreateObj(element, element.points.positions[i]);
            }
        }

        public void DeleteRefresh(string refreshId)
        {
            if (string.IsNullOrEmpty(refreshId))
                return;
            for (int i = 0; i < m_objList.Count; ++i)
            {
                if (m_objList[i].refreshID == refreshId)
                    DeleteObj(m_objList[i].objectStoryID);
            }
        }

        public void CreateObj(StoryObjectElement element, Vector3 pos)
        {
            StoryObjectBase newObj = new StoryActor();

            newObj.InitData(element, GenObjectID(), element.storyObjectID, pos);
            AddObj(newObj);
        }

        public void AddObj(StoryObjectBase obj)
        {
            if (m_objMap.ContainsKey(obj.objectStoryID))
            {
                Debuger.LogError(string.Format("场景重复添加对象, objectId={0}, refreshId={1}", obj.name, obj.refreshID));
                return;
            }
            obj.OnStart();

            m_objMap.Add(obj.objectStoryID, obj);
            m_objList.Add(obj);
        }

        public void DeleteObj(int id)
        {
            StoryObjectBase obj;
            if (m_objMap.TryGetValue(id, out obj))
            {
                obj.OnDestroy();

                m_objMap.Remove(id);
                m_objList.Remove(obj);
            }
        }

        void UpdataObj()
        {
            for (int i = 0; i < m_objList.Count; ++i)
            {
                m_objList[i].Update();
            }
        }

        void ClearObjects()
        {
            for (int i = 0; i < m_objList.Count; ++i)
            {
                m_objList[i].OnDestroy();
            }

            m_objList.Clear();
            m_objMap.Clear();
        }

        #endregion

    }
}

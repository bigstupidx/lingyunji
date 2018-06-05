using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.GameStory
{
    /// <summary>
    /// 剧情角色对象类
    /// </summary>
    public abstract class StoryObjectBase
    {
        // 角色基本信息
        public int objectStoryID;//由剧情分配的UID
        public string refreshID;

        public string name;
        public string model;
        public Vector3 bornPosition { get; protected set; }

        public StoryObjectElement cfgInfo { get; protected set; }

        public ObjectComponentHandler ComHandler;

        //public T GetObjectComponent<T> () where T : IObjectComponent
        //{
        //    return default(T);
        //}


        public void InitData(StoryObjectElement element, int uid, string refreshId, Vector3 bornPos)
        {
            cfgInfo = element;

            name = cfgInfo.objectDefine.name;
            model = cfgInfo.objectDefine.model;

            objectStoryID = uid;
            refreshID = refreshId;
            bornPosition = bornPos;

            ComHandler = new ObjectComponentHandler();
            ComHandler.Init(this);
        }

        public virtual void OnStart()
        {
            if (ComHandler != null)
                ComHandler.Start();
        }

        public virtual void OnDestroy()
        {
            if (ComHandler != null)
                ComHandler.Destroy();
        }

        // Update is called once per frame
        public virtual void Update()
        {

            if (ComHandler != null)
                ComHandler.Update();

        }
    }
}

#if !USE_HOT
namespace xys.hot.Event
{
    using CommonBase;
    public class HotEventSet
    {
        public void fireEvent(NetProto.AttType e2)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.fireEvent(e2);
        }

        public void fireEvent(EventID name)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.fireEvent(name);
        }

        public void FireEvent<T>(EventID name, T args)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.FireEvent(name, (object)args);
        }

        public void FireEvent(CDType type, CDEventData data)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.FireEvent(type, data);
        }

        public void FireEvent(CDType type, short id, CDEventData data)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.FireEvent(type, id, data);
        }

        public IConnection Subscribe(CDType type, short id, System.Action<CDEventData> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(type, id, action);
        }

        public IConnection Subscribe(CDType type, System.Action<CDEventData> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(type, action);
        }

        public IConnection Subscribe(EventID name, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(name, action);
        }

        public IConnection Subscribe(EventID name, int group, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(name, group, action);
        }

        public IConnection Subscribe<T1>(EventID name, int group, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            return App.my.eventSet.Subscribe(name, group, se.OnEnd);
        }

        public IConnection Subscribe<T1>(EventID name, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            return App.my.eventSet.Subscribe(name, se.OnEnd);
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(e2, action);
        }

        public IConnection Subscribe(NetProto.AttType e2, int group, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(e2, group, action);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, int group, System.Action<T1> action) where T1 : ObjectBase
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            return App.my.eventSet.Subscribe(e2, group, se.OnEnd);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, int group, System.Action<T1, AttributeChange> action) where T1 : ObjectBase
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(e2, group, action);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, System.Action<T1> action) where T1 : ObjectBase
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(e2, action);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, System.Action<T1, AttributeChange> action) where T1 : ObjectBase
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEventAttType<T1> se = new SubscribeEventAttType<T1>(action);
            return App.my.eventSet.Subscribe(e2, se.OnEnd);
        }

        public void fireEvent(string eventName)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.fireEvent(eventName);
        }

        public void fireEvent(string e1, string e2)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.fireEvent(e1, e2);
        }

        public void FireEvent<T>(string name, T args)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.FireEvent(name, (object)args);
        }

        public void FireEvent<T>(string e1, string e2, T args)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.FireEvent(e1, e2, (object)args);
        }

        public IConnection Subscribe(string name, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(name, action);
        }

        public IConnection Subscribe(string name, int group, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(name, group, action);
        }

        public IConnection Subscribe<T1>(string name, int group, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            return App.my.eventSet.Subscribe(name, group, se.OnEnd);
        }

        public IConnection Subscribe<T1>(string name, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            return App.my.eventSet.Subscribe(name, se.OnEnd);
        }

        public IConnection Subscribe(string e1, string e2, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(e1, e2, action);
        }

        public IConnection Subscribe(string e1, string e2, int group, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            return App.my.eventSet.Subscribe(e1, e2, group, action);
        }

        public IConnection Subscribe<T1>(string e1, string e2, int group, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            return App.my.eventSet.Subscribe(e1, e2, group, se.OnEnd);
        }

        public IConnection Subscribe<T1>(string e1, string e2, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            return App.my.eventSet.Subscribe(e1, e2, se.OnEnd);
        }
    }

    public class HotObjectEventSet
    {
        public HotObjectEventSet(ObjectEventSet eventSet)
        {
            this.eventSet = eventSet;
        }

        ObjectEventSet eventSet;

        public void fireEvent(ObjEventID e1)
        {
            eventSet.fireEvent(e1);
        }

        public void FireEvent<T>(ObjEventID e1, T para)
        {
            eventSet.FireEvent(e1, (object)para);
        }

        public void FireEvent<T>(EventID e1, T para)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.FireEvent(e1, (object)para);
        }

        public void fireEvent(EventID e1)
        {
#if UNITY_EDITOR
            if (App.my == null) return;
#endif
            App.my.eventSet.fireEvent(e1);
        }

        public void FireEvent(NetProto.AttType e2, AttributeChange para)
        {
            eventSet.FireEvent(e2, para);
        }

        public IConnection Subscribe(ObjEventID e1, System.Action action)
        {
            return eventSet.Subscribe(e1, action);
        }

        public IConnection Subscribe<T1>(ObjEventID e1, System.Action<T1> action)
        {
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            return eventSet.Subscribe(e1, se.OnEnd);
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action action)
        {
            return eventSet.Subscribe(e2, action);
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action<AttributeChange> action)
        {
            return eventSet.Subscribe(e2, action);
        }

        public IConnection Subscribe(EventID eid, System.Action action)
        {
            return App.my.eventSet.Subscribe(eid, action);
        }

        public IConnection Subscribe<T1>(EventID eid, System.Action<T1> action)
        {
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);

            return App.my.eventSet.Subscribe(eid, se.OnEnd);
        }
    }
}

#endif
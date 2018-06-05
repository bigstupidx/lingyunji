#if !USE_HOT
namespace xys.hot.Event
{
    using CommonBase;
    using System.Collections.Generic;

    class SubscribeEvent<T>
    {
        public SubscribeEvent(System.Action<T> action)
        {
            this.action = action;
        }

        System.Action<T> action;

        public void OnEnd(object p)
        {
            if (action != null)
            {
                try
                {
                    action((T)p);
                }
                catch (System.Exception ex)
                {
                    if (p == null)
                    {
                        Debuger.ErrorLog("p == null");
                    }
                    else
                    {
                        Debuger.ErrorLog("srcType:{0} dstType:{1}", typeof(T).Name, p.GetType().Name);
                    }

                    Debuger.LogException(ex);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("action:{0}", CommonBase.Help.GetDelegateInfo(action));
        }
    }

    class SubscribeEvent<T1, T2>
    {
        public SubscribeEvent(System.Action<T1, T2> action)
        {
            this.action = action;
        }

        System.Action<T1, T2> action;

        public void OnEnd(object p1, object p2)
        {
            if (action != null)
                action((T1)p1, (T2)p2);
        }

        public override string ToString()
        {
            return string.Format("action:{0}", CommonBase.Help.GetDelegateInfo(action));
        }
    }

    class SubscribeEventAttType<T> where T : ObjectBase
    {
        public SubscribeEventAttType(System.Action<T, AttributeChange> action)
        {
            this.action = action;
        }

        System.Action<T, AttributeChange> action;

        public void OnEnd(ObjectBase p1, AttributeChange p2)
        {
            if (action != null)
                action((T)p1, p2);
        }

        public override string ToString()
        {
            return string.Format("action:{0}", CommonBase.Help.GetDelegateInfo(action));
        }
    }

    public class HotEventAgent
    {
        List<IConnection> Connects = new List<IConnection>();

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
            var v = App.my.eventSet.Subscribe(type, id, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(CDType type, System.Action<CDEventData> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(type, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(EventID name, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(name, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(EventID name, int group, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(name, group, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(EventID name, int group, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            var v = App.my.eventSet.Subscribe(name, group, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(EventID name, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            var v = App.my.eventSet.Subscribe(name, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(e2, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(NetProto.AttType e2, int group, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(e2, group, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, int group, System.Action<T1> action) where T1 : ObjectBase
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            var v = App.my.eventSet.Subscribe(e2, group, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, int group, System.Action<T1, AttributeChange> action) where T1 : ObjectBase
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(e2, group, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, System.Action<T1> action) where T1 : ObjectBase
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(e2, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, System.Action<T1, AttributeChange> action) where T1 : ObjectBase
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEventAttType<T1> se = new SubscribeEventAttType<T1>(action);
            var v = App.my.eventSet.Subscribe(e2, se.OnEnd);
            Connects.Add(v);
            return v;
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
            var v = App.my.eventSet.Subscribe(name, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(string name, int group, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(name, group, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(string name, int group, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            var v = App.my.eventSet.Subscribe(name, group, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(string name, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            var v = App.my.eventSet.Subscribe(name, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(string e1, string e2, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(e1, e2, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(string e1, string e2, int group, System.Action action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            var v = App.my.eventSet.Subscribe(e1, e2, group, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(string e1, string e2, int group, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            var v = App.my.eventSet.Subscribe(e1, e2, group, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(string e1, string e2, System.Action<T1> action)
        {
#if UNITY_EDITOR
            if (App.my == null) return null;
#endif
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            var v = App.my.eventSet.Subscribe(e1, e2, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public void Release()
        {
            for (int i = 0; i < Connects.Count; ++i)
            {
                Connects[i].disconnect();
            }

            Connects.Clear();
        }
    }

    public class HotObjectEventAgent
    {
        public HotObjectEventAgent(ObjectEventSet eventSet)
        {
            this.eventSet = eventSet;
        }

        List<IConnection> Connects = new List<IConnection>();
        ObjectEventSet eventSet;

        public int Count { get { return Connects.Count; } }

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
            var v = eventSet.Subscribe(e1, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(ObjEventID e1, System.Action<T1> action)
        {
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);
            var v = eventSet.Subscribe(e1, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action action)
        {
            var v = eventSet.Subscribe(e2, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action<AttributeChange> action)
        {
            var v = eventSet.Subscribe(e2, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe(EventID eid, System.Action action)
        {
            var v = App.my.eventSet.Subscribe(eid, action);
            Connects.Add(v);
            return v;
        }

        public IConnection Subscribe<T1>(EventID eid, System.Action<T1> action)
        {
            SubscribeEvent<T1> se = new SubscribeEvent<T1>(action);

            var v = App.my.eventSet.Subscribe(eid, se.OnEnd);
            Connects.Add(v);
            return v;
        }

        public void Release()
        {
            for (int i = 0; i < Connects.Count; ++i)
            {
                Connects[i].disconnect();
            }

            Connects.Clear();
        }
    }
}

#endif
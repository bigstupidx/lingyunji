namespace xys
{
    using CommonBase;

    struct EventKey
    {
        enum Type
        {
            EventID, // 事件ID
            EventIDATT,// 属性事件
            EventIDState, // App状态事件
            EventIDStateBegin, // 进入App状态
            EventIDStateEnd, // 离开App状态
            EventIDCD, // CD计时事件
            StrEvent, // 字符串事件
        }

        public readonly byte type;
        public readonly int id;
        public readonly string eventName;

        public EventKey(EventID eid)
        {
            type = (byte)Type.EventID;
            id = (ushort)eid;
            eventName = null;
        }

        public EventKey(NetProto.AttType type)
        {
            this.type = (byte)Type.EventIDATT;
            id = (ushort)type;
            eventName = null;
        }

        public EventKey(AppStateType stateType)
        {
            this.type = (byte)Type.EventIDState;
            id = (ushort)stateType;
            eventName = null;
        }

        public EventKey(StateEvent se, AppStateType stateType)
        {
            type = (byte)(se == StateEvent.Begin ? Type.EventIDStateBegin : Type.EventIDStateEnd);
            id = (int)stateType;
            eventName = null;
        }

        public EventKey(CDType type, short id)
        {
            this.type = (byte)Type.EventIDCD;
            this.id = CDUtils.Combine(type, id);

            eventName = null;
        }


        public EventKey(string en)
        {
            type = (byte)Type.StrEvent;
            eventName = en;

            id = 0;
        }

        public EventKey(string e1, string e2) : this(string.Format("{0}@{1}", e1, e2))
        {

        }
    }

    public class ObjectEventSet
    {
        public ObjectEventSet(ObjectBase obj)
        {
            this.obj = obj;
        }

        readonly ObjectBase obj;
        readonly EventSet<int> eventSet = new EventSet<int>();

        public void RemoveAllEvents()
        {
            eventSet.removeAllEvents();
        }

        public void fireEvent(ObjEventID e1)
        {
            eventSet.fireEvent((int)e1 << 16);
        }

        public void FireEvent(ObjEventID e1, object para)
        {
            eventSet.fireEvent((int)e1 << 16, para);
        }

        public void FireEvent<T>(ObjEventID e1, T para)
        {
            eventSet.fireEvent((int)e1 << 16, para);
        }

        public void FireEvent(NetProto.AttType e2, AttributeChange para)
        {
            eventSet.fireEvent((int)e2, para);
            App.my.eventSet.FireEvent(e2, obj, para);
        }

        public IConnection Subscribe(ObjEventID e1, System.Action action)
        {
            return eventSet.Subscribe((int)e1 << 16, action);
        }

        public IConnection Subscribe(ObjEventID e1, System.Action<object> action)
        {
            return eventSet.Subscribe((int)e1 << 16, action);
        }

        public IConnection Subscribe<T1>(ObjEventID e1, System.Action<T1> action)
        {
            return eventSet.Subscribe((int)e1 << 16, action);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, System.Action<T1> action)
        {
            return eventSet.Subscribe((int)e2, action);
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action action)
        {
            return eventSet.Subscribe((int)e2, action);
        }
    }

    public class EventSet
    {
        // 指定对象
        EventSet<EventKey> eventSet = new EventSet<EventKey>();

        public bool isMuted
        {
            get { return eventSet.isMuted; }
            set { eventSet.isMuted = value; }
        }

        // 属性变化
        public void fireEvent(NetProto.AttType att)
        {
            eventSet.fireEvent(new EventKey(att));
        }

        public void fireEvent(EventID name)
        {
            eventSet.fireEvent(new EventKey(name));
        }

        public void FireEvent<T>(EventID name, T args)
        {
            eventSet.fireEvent(new EventKey(name), args);
        }

        public void FireEvent(EventID name, object args)
        {
            eventSet.fireEvent(new EventKey(name), args);
        }

        public void FireEvent<T>(AppStateType state, T args) where T : IAppState
        {
            eventSet.fireEvent(new EventKey(state), args);
        }

        public void FireEvent(StateEvent e1, AppStateType state)
        {
            eventSet.fireEvent(new EventKey(e1, state));
        }

        public void FireEvent<T>(StateEvent e1, AppStateType state, T args) where T : IAppState
        {
            eventSet.fireEvent(new EventKey(e1, state), args);
        }

        public void FireEvent(NetProto.AttType e2, ObjectBase obj, AttributeChange att)
        {
            eventSet.fireEvent(new EventKey(e2), obj, att);
        }

        public void FireEvent(CDType type, CDEventData data)
        {
            eventSet.fireEvent(new EventKey(type, 0), data);
        }

        public void FireEvent(CDType type, short id, CDEventData data)
        {
            eventSet.fireEvent(new EventKey(type, id), data);
        }

        public IConnection Subscribe(CDType type, short id, System.Action<CDEventData> action)
        {
            return eventSet.Subscribe(new EventKey(type, id), action);
        }

        public IConnection Subscribe(CDType type, System.Action<CDEventData> action)
        {
            return eventSet.Subscribe(new EventKey(type, 0), action);
        }

        public IConnection Subscribe(EventID name, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(name), action);
        }

        public IConnection Subscribe(EventID name, int group, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(name), group, action);
        }

        public IConnection Subscribe(EventID name, int group, System.Action<object> action)
        {
            return eventSet.Subscribe(new EventKey(name), group, action);
        }

        public IConnection Subscribe<T1>(EventID name, int group, System.Action<T1> action)
        {
            return eventSet.Subscribe(new EventKey(name), group, action);
        }

        public IConnection Subscribe(EventID name, System.Action<object> action)
        {
            return eventSet.Subscribe(new EventKey(name), -1, action);
        }

        public IConnection Subscribe<T1>(EventID name, System.Action<T1> action)
        {
            return eventSet.Subscribe(new EventKey(name), -1, action);
        }

        public IConnection Subscribe<T1>(StateEvent name, AppStateType state, System.Action<T1> action) where T1 : IAppState
        {
            return eventSet.Subscribe(new EventKey(name, state), -1, action);
        }

        public IConnection Subscribe<T1>(StateEvent name, AppStateType state, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(name, state), -1, action);
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(e2), -1, action);
        }

        public IConnection Subscribe(NetProto.AttType e2, int group, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(e2), group, action);
        }

        public IConnection Subscribe(NetProto.AttType e2, int group, System.Action<object> action)
        {
            return eventSet.Subscribe(new EventKey(e2), group, action);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, int group, System.Action<T1> action) where T1 : ObjectBase
        {
            return eventSet.Subscribe(new EventKey(e2), group, action);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, System.Action<T1> action) where T1 : ObjectBase
        {
            return eventSet.Subscribe(new EventKey(e2), action);
        }

        public IConnection Subscribe(NetProto.AttType e2, int group, System.Action<ObjectBase, AttributeChange> action)
        {
            return eventSet.Subscribe(new EventKey(e2), group, action);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, int group, System.Action<T1, AttributeChange> action) where T1 : ObjectBase
        {
            return eventSet.Subscribe(new EventKey(e2), group, action);
        }

        public IConnection Subscribe(NetProto.AttType e2, System.Action<ObjectBase, AttributeChange> action)
        {
            return eventSet.Subscribe(new EventKey(e2), action);
        }

        public IConnection Subscribe<T1>(NetProto.AttType e2, System.Action<T1, AttributeChange> action) where T1 : ObjectBase
        {
            return eventSet.Subscribe(new EventKey(e2), action);
        }

        public void fireEvent(string eventName)
        {
            eventSet.fireEvent(new EventKey(eventName));
        }

        public void fireEvent(string e1, string e2)
        {
            eventSet.fireEvent(new EventKey(e1, e2));
        }

        public void FireEvent(string name, object args)
        {
            eventSet.fireEvent(new EventKey(name), args);
        }

        public void FireEvent<T>(string name, T args)
        {
            eventSet.fireEvent(new EventKey(name), args);
        }

        public void FireEvent<T>(string e1, string e2, T args)
        {
            eventSet.fireEvent(new EventKey(e1, e2), args);
        }

        public void FireEvent(string e1, string e2, object args)
        {
            eventSet.fireEvent(new EventKey(e1, e2), args);
        }

        public IConnection Subscribe(string name, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(name), action);
        }

        public IConnection Subscribe(string name, int group, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(name), group, action);
        }

        public IConnection Subscribe(string name, int group, System.Action<object> action)
        {
            return eventSet.Subscribe(new EventKey(name), group, action);
        }

        public IConnection Subscribe<T1>(string name, int group, System.Action<T1> action)
        {
            return eventSet.Subscribe(new EventKey(name), group, action);
        }

        public IConnection Subscribe(string name, System.Action<object> action)
        {
            return eventSet.Subscribe(new EventKey(name), action);
        }

        public IConnection Subscribe<T1>(string name, System.Action<T1> action)
        {
            return eventSet.Subscribe(new EventKey(name), action);
        }

        public IConnection Subscribe(string e1, string e2, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(e1, e2), action);
        }

        public IConnection Subscribe(string e1, string e2, int group, System.Action action)
        {
            return eventSet.Subscribe(new EventKey(e1, e2), group, action);
        }

        public IConnection Subscribe<T1>(string e1, string e2, int group, System.Action<T1> action)
        {
            return eventSet.Subscribe(new EventKey(e1, e2), group, action);
        }

        public IConnection Subscribe(string e1, string e2, int group, System.Action<object> action)
        {
            return eventSet.Subscribe(new EventKey(e1, e2), group, action);
        }

        public IConnection Subscribe(string e1, string e2, System.Action<object> action)
        {
            return eventSet.Subscribe(new EventKey(e1, e2), action);
        }

        public IConnection Subscribe<T1>(string e1, string e2, System.Action<T1> action)
        {
            return eventSet.Subscribe(new EventKey(e1, e2), action);
        }

        public void removeAllEvents() { eventSet.removeAllEvents(); }
    }
}
#if !USE_HOT
using NetProto;
using NetProto.Hot;
using System.Collections;

namespace xys.hot
{
    public abstract class AppStateBase
    {
       public AppStateBase(HotAppStateBase parent)
        {
            this.parent = parent;
        }

        public HotAppStateBase parent { get; private set; }

        public Event.HotEventAgent Event { get; private set; }

        // 进入此状态
        void Enter(object p)
        {
            Event = new hot.Event.HotEventAgent();
            OnEnter(p);
        }

        void Level()
        {
            OnLevel();

            if (Event != null)
                Event.Release();
        }

        protected virtual void OnEnter(object p) { }
        protected virtual void OnLevel() { }
    }
}
#endif
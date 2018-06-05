#if !USE_HOT
namespace xys.hot
{
    using Network;

    abstract class HotModuleBase
    {
        public HotModuleBase(xys.ModuleBase parent)
        {
            this.parent = parent;
        }

        public xys.ModuleBase parent { get; protected set; }

        public Event.HotObjectEventSet Event { get { return hotApp.my.localEvent; } }

        void Awake()
        {
            hotApp.my.OnModuleCreate(this);
            OnAwake();
        }

        protected abstract void OnAwake();

        void Deserialize(wProtobuf.IReadStream output)
        {
            OnDeserialize(output);
        }

        protected abstract void OnDeserialize(wProtobuf.IReadStream output);
    }
}
#endif
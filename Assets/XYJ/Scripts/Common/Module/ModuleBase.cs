using NetProto;

namespace xys
{
    public abstract class ModuleBase : IModule
    {
        public string moduleType { get; private set; } // 模块类型
        public int id { get; private set; } // 模块的ID

        void IModule.SetModuleType(string type, int id)
        {
            moduleType = type;
            this.id = id;
        }

        public IModuleOwner owner { get; private set; }
        public LocalPlayer user { get; private set; }

        public EventAgent Event { get; private set; }

        public void Awake(IModuleOwner owner)
        {
            this.owner = owner;
            user = (LocalPlayer)owner;
            Event = new EventAgent();

            OnAwake();
        }
         
        protected virtual void OnAwake() { }

        // 序列化数据
        public abstract void Deserialize(wProtobuf.IReadStream output);

        public void Release()
        {
            Event.Release();
            OnRelease();
        }

        protected virtual void OnRelease()
        {

        }
    }
}

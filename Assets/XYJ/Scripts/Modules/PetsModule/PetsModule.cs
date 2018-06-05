namespace xys
{
//     using NetProto;
//     using NetProto.Hot;
    using System.Reflection;

    public partial class PetsModule : HotModule
    {
        public PetsModule() : base("xys.hot.HotPetsModule") { }

        public object petMgr { get { return refType.GetField("petsMgr"); } }

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        //当前宠物名字
        public int GetCurrentPetId()
        {
            object mgr = petMgr;
            if (petMgr == null)
                return -1;
            return (int)refType.InvokeMethodReturn("GetCurrentPetId");
        }
        //当前宠物名字
        public string GetCurrentPetNickName()
        {
            object mgr = petMgr;
            if (petMgr == null)
                return string.Empty;
            return (string)refType.InvokeMethodReturn("GetCurrentPetNickName");
        }
        //当前宠物个数
        public int GetPetCount()
        {
            object mgr = petMgr;
            if (petMgr == null)
                return 0;
            return (int)refType.InvokeMethodReturn("GetPetCount");
        }
    }
}


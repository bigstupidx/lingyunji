namespace xys
{
    class CDModule : ModuleBase
    {
        public CDModule()
        {
            App.my.handler.Reg<NetProto.CDData>(NetProto.Protoid.A2C_StartCD, OnStartCD);
        }

        public CDMgr cdMgr { get; private set; }

        protected override void OnAwake()
        {
            cdMgr = user.cdMgr;
        }

        void OnStartCD(Network.IPacket packet, NetProto.CDData data)
        {
            cdMgr.OnStartCD(data);
        }

        public override void Deserialize(wProtobuf.IReadStream input)
        {
            cdMgr.Deserialize(input);
        }
    }
}

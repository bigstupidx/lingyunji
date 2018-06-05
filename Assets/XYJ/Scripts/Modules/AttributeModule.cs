namespace xys
{
    using Network;
    using NetProto;
    using CommonBase;
    
    public class AttributeChange
    {
        public AttType id; // 变化的属性ID
        public IAttribute<AttType> oldValue; // 原先值
        public IAttribute<AttType> currentValue; // 当前值
    }

    class AttributeModule : ModuleBase
    {
        public AttributeModule()
        {
            App.my.handler.Reg<wProtobuf.RealBytes>(Protoid.A2C_SyncAttChange, OnAttValue);
        }

        public AttributeSet<AttType> attributes { get; private set; }

        protected override void OnAwake()
        {
            attributes = user.attributes;
        }

        public override void Deserialize(wProtobuf.IReadStream input)
        {
            AttConfig.InitByStream(input.ReadBytes().buffer);
            AttConfig.InitAttributeByType(attributes, AttConfig.selfList, input);
        }

        void OnAttValue(IPacket packet, wProtobuf.RealBytes bytes)
        {
            wProtobuf.MessageStream input = new wProtobuf.MessageStream(bytes.bytes);
            input.WritePos = bytes.bytes.Length;

            while (input.ReadSize != 0)
            {
                AttType id = (AttType)input.ReadInt32();
                var att = attributes.Get(id);

                AttributeChange change = new AttributeChange();
                change.id = id;
                change.oldValue = att.Clone();

                att.MergeFrom(input);

                change.currentValue = att.Clone();

                user.eventSet.FireEvent(ObjEventID.ChangeAttri, change);
                user.eventSet.FireEvent(att.attid, change);

                App.my.eventSet.FireEvent(EventID.LocalAttributeChange, change);

                Debuger.Log(string.Format("属性修改 id={0} v={1}", id, att.realValue));
            }
        }
    }
}

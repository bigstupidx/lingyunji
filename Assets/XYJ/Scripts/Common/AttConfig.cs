using System.Collections.Generic;

namespace xys
{
    public class AttConfig
    {
        // 属性ID
        public NetProto.AttType id { get; set; }

        // 数值类型,
        public CommonBase.ValueType type { get; set; }

        public override string ToString()
        {
            return string.Format("id:{0} type:{1}", id, type);
        }

        static Dictionary<NetProto.AttType, AttConfig> Datas = new Dictionary<NetProto.AttType, AttConfig>();

        static List<AttConfig> SelfList = new List<AttConfig>();
        static List<AttConfig> RemoteList = new List<AttConfig>();
        static List<AttConfig> NpcList = new List<AttConfig>();

        public static List<AttConfig> selfList { get { return SelfList; } }
        public static List<AttConfig> remoteList { get { return RemoteList; } }
        public static List<AttConfig> npcList { get { return NpcList; } }

        public static void InitByStream(byte[] bytes)
        {
            SelfList.Clear();
            RemoteList.Clear();
            NpcList.Clear();
            Datas.Clear();

            Network.BitStream ms = new Network.BitStream(bytes);
            ms.WritePos = bytes.Length;
            int count = ms.ReadInt32();

            List<AttConfig> configs = new List<AttConfig>();
            for (int i = 0; i < count; ++i)
            {
                AttConfig config = new AttConfig();
                config.id = (NetProto.AttType)ms.ReadInt32();
                config.type = (CommonBase.ValueType)ms.ReadInt32();

                Datas.Add(config.id, config);
                configs.Add(config);
            }

            CommonBase.IBit bit = CommonBase.BitHelp.Create(configs.Count);
            bit.MergeFrom(ms); for (int i = 0; i < configs.Count; ++i) if (bit[i]) SelfList.Add(configs[i]);
            bit.MergeFrom(ms); for (int i = 0; i < configs.Count; ++i) if (bit[i]) RemoteList.Add(configs[i]);
            bit.MergeFrom(ms); for (int i = 0; i < configs.Count; ++i) if (bit[i]) NpcList.Add(configs[i]);
        }

        public static void InitAttributeByType(CommonBase.AttributeSet<NetProto.AttType> attset, List<AttConfig> list, wProtobuf.IReadStream input)
        {
            foreach (AttConfig config in list)
            {
                var att = attset.factory.Create(config.id, config.type);
                attset.Set(config.id, att);
                att.MergeFrom(input);
            }
        }
    }
}

using NetProto;
using System.Reflection;

namespace xys
{
    // 可热更的模块
    public class HotModule : ModuleBase
    {
        public HotModule(string fullname)
        {
            refType = new RefType(fullname, this);
        }

        public RefType refType { get; private set; }

        protected override void OnAwake()
        {
            refType.InvokeMethod("Awake");
        }

        public override void Deserialize(wProtobuf.IReadStream output)
        {
            refType.InvokeMethod("Deserialize", output);
        }
    }
}

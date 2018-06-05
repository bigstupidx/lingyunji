namespace xys
{
    using Network;
    using System.Collections.Generic;

    public class hotAppAgent
    {
        public hotAppAgent()
        {
            Debuger.DebugLog("hotAppAgent");
            refType = new RefType("xys.hot.hotApp");

            // 可能会用到的类型
            var v = typeof(List<int>);
            v = typeof(List<string>);
            v = typeof(List<long>);
            v = typeof(List<short>);
            v = typeof(List<char>);
            v = typeof(List<ushort>);

            v = typeof(HashSet<long>);
            v = typeof(HashSet<int>);
        }

        RefType refType;

        public void InitModule(ModuleFactoryMgr mgr)
        {
            refType.InvokeMethod("InitModule", mgr);
        }

        public void CsvLoad(CsvCommon.CsvLoadKey csv)
        {
            refType.InvokeMethod("CsvLoad", csv);
        }
    }
}

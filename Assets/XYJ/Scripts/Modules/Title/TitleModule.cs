namespace xys
{
    using NetProto;
    using System.Reflection;

    public class TitleModule : HotModule
    {
        public TitleModule() : base("xys.hot.HotTitleModule")
        {

        }

        public object m_TitleListData { get { return refType.GetField("m_TitleListData"); } }
    }
}
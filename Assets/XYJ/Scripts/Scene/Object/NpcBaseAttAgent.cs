namespace xys
{
    using NetProto;
    partial class NpcBase
    {
        public int qiLiValue // 气力
        {
            get { return attributes.Get(AttType.AT_QiLi).intValue; }
        }
    }
}

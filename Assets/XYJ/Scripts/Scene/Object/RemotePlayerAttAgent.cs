namespace xys
{
    using NetProto;
    partial class RemotePlayer
    {
        public ushort carrerValue // 职业
        {
            get { return attributes.Get(AttType.AT_Carrer).ushortValue; }
        }
        public int sexValue // 性别
        {
            get { return attributes.Get(AttType.AT_Sex).intValue; }
        }
        public int teamIDValue // 队伍ID
        {
            get { return attributes.Get(AttType.AT_TeamID).intValue; }
        }
        public long teamLeaderUidValue // 队长uid
        {
            get { return attributes.Get(AttType.AT_TeamLeaderUid).longValue; }
        }
        public int teamIsLeaderValue // 是否队长
        {
            get { return attributes.Get(AttType.AT_TeamIsLeader).intValue; }
        }
    }
}

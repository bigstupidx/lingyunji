namespace xys
{
    using NetProto;
    partial class LocalPlayer
    {
        public int qiLiValue // 气力
        {
            get { return attributes.Get(AttType.AT_QiLi).intValue; }
        }
        public uint expValue // 经验
        {
            get { return attributes.Get(AttType.AT_Exp).uintValue; }
        }
        public ushort carrerValue // 职业
        {
            get { return attributes.Get(AttType.AT_Carrer).ushortValue; }
        }
        public int sexValue // 性别
        {
            get { return attributes.Get(AttType.AT_Sex).intValue; }
        }
        public long silverShellValue // 银贝
        {
            get { return attributes.Get(AttType.AT_SilverShell).longValue; }
        }
        public long goldShellValue // 金贝
        {
            get { return attributes.Get(AttType.AT_GoldShell).longValue; }
        }
        public long fairyJadeValue // 仙玉
        {
            get { return attributes.Get(AttType.AT_FairyJade).longValue; }
        }
        public long jasperJadeValue // 碧玉
        {
            get { return attributes.Get(AttType.AT_JasperJade).longValue; }
        }
        public int energyValue // 活力
        {
            get { return attributes.Get(AttType.AT_Energy).intValue; }
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
        public long organizationValue // 门派贡献
        {
            get { return attributes.Get(AttType.AT_Organization).longValue; }
        }
        public long chivalrousValue // 侠义值
        {
            get { return attributes.Get(AttType.AT_Chivalrous).longValue; }
        }
        public long familyValue // 氏族贡献
        {
            get { return attributes.Get(AttType.AT_Family).longValue; }
        }
        public long xiuWeiValue // 修为
        {
            get { return attributes.Get(AttType.AT_XiuWei).longValue; }
        }
        public long gongXunValue // 功勋
        {
            get { return attributes.Get(AttType.AT_GongXun).longValue; }
        }
        public long honorValue // 荣誉
        {
            get { return attributes.Get(AttType.AT_Honor).longValue; }
        }
        public long tianPingValue // 天平战积分
        {
            get { return attributes.Get(AttType.AT_TianPing).longValue; }
        }
        public long xinHuoValue // 薪火值
        {
            get { return attributes.Get(AttType.AT_XinHuo).longValue; }
        }
        public long clanIdValue // 氏族ID
        {
            get { return attributes.Get(AttType.AT_ClanId).longValue; }
        }
        public long clanContributionAllValue // 氏族总贡献
        {
            get { return attributes.Get(AttType.AT_ClanContributionAll).longValue; }
        }
        public long clanContributionWeekValue // 本氏族贡献
        {
            get { return attributes.Get(AttType.AT_ClanContributionWeek).longValue; }
        }
        public int clanPostValue // 氏族职位
        {
            get { return attributes.Get(AttType.AT_ClanPost).intValue; }
        }
        public long clanResponeIdValue // 氏族响应ID
        {
            get { return attributes.Get(AttType.AT_ClanResponeId).longValue; }
        }
        public uint bloodPoolValue // 血池
        {
            get { return attributes.Get(AttType.AT_BloodPool).uintValue; }
        }
        public int bloodBottleValue // 血瓶
        {
            get { return attributes.Get(AttType.AT_BloodBottle).intValue; }
        }
        public int autoRestoreRateValue // 宠物自动回复血量百分比
        {
            get { return attributes.Get(AttType.AT_AutoRestoreRate).intValue; }
        }
        public int petBloodBottleValue // 宠物血瓶
        {
            get { return attributes.Get(AttType.AT_PetBloodBottle).intValue; }
        }
        public long combatPowerValue // 战斗力
        {
            get { return attributes.Get(AttType.AT_CombatPower).longValue; }
        }
        public long joinClanTimeValue // 加入氏族时间
        {
            get { return attributes.Get(AttType.AT_JoinClanTime).longValue; }
        }
        public int clanLeagueTimesValue // 氏族联赛次数
        {
            get { return attributes.Get(AttType.AT_ClanLeagueTimes).intValue; }
        }
        public int clanRunBusinessWeekValue // 氏族本周跑商
        {
            get { return attributes.Get(AttType.AT_ClanRunBusinessWeek).intValue; }
        }
        public int clanRunBusinessAllValue // 氏族累计跑商
        {
            get { return attributes.Get(AttType.AT_ClanRunBusinessAll).intValue; }
        }
    }
}

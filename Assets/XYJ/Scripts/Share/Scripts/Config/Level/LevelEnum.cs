namespace Config
{
    //关卡类型
    public enum LevelType
    {
        MainCity = 1,         //主城
        NormalDuplicate = 2,    //普通副本
        StroyDuplicate = 4,     //剧情定制副本
        Prison = 7,             //监狱
        Activity = 8,            //活动
        Guild = 9,            //氏族
    }

    //组队进入方式
    public enum TeamEnterType
    {
        Agree = 1,              //全员同意才能进入
        Call = 2,               //召唤到同一场景才能进入
    }

    //组队条件
    public enum TeamCondition
    {
        All = 1,            //都可以，单人就单人进入，多人需要判断组队进入方式，满足则全部进入
        NeedTeam,           //必须组队
        NeedNoTeam,         //必须单人
        Single,             //单人，无论是否在组队，都只有一个人进入
    }

    //死亡惩罚
    public enum DeadPunish
    {
        Null,               //无
        Durability,         //装备耐久
        Life,               //生命值
    }

    //普通关卡胜利结算类型
    public enum AccountType
    {
        Default,            //默认结算
        NoNeed,             //不需要胜利界面，直接退出
    }

    //关卡成败
    public enum LevelSOF
    {
        Null,
        Sucess,
        Fail,
    }

    //关卡摄像机类型
    public enum LevelCameraType
    {
        Null,
        Default,            //默认类型摄像机
        FixPosition,        //固定位置
    }

    //ui缩放
    public enum UIStatus
    {
        Default = 0,        //默认不变
        Close = 1,          //收起
        Open = 2,           //打开
    }

    //关卡等级设置类型  如果角色本身设置了等级或者玩法有等级生成规则，则该参数不生效
    public enum LevelGradeSetting
    {
        Null,
        Fixed,              //固定属性 等级读取【关卡等级】列
        TeamAverage,        //队伍平均等级 程序计算出关卡等级（默认）
        TeamMax,            //队伍最高等级 程序计算出关卡等级
        TeamMin,            //队伍最低等级 程序计算出关卡等级
    }

    //挑战次数扣除方式
    public enum DeductType
    {
        Win,            //胜利扣除次数
        Enter,          //进入就扣除次数
    }

    //附近复活限制
    public enum NearReliveType
    {
        Null,           //无限制
        OutBattle,      //队友脱战才能复活
    }
}

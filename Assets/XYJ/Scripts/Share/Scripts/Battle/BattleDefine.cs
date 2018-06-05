using System;
using System.Collections.Generic;
using UnityEngine;


//玩家阵营和怪物阵营是敌对关系，中立阵营和其他阵营都是中立关系,
//对象类型和战斗阵营没有关系，任何角色类型都可以为玩家阵营
public enum BattleCamp
{
    SameAsCfg = 0, //通信用，如果为0表示使用配置表的
    PlayerCamp = 1,
    MonsterCamp = 2,
    NeutralCamp = 3,
}


//护体状态
public enum HutiState : ushort
{
    Huti,       //护体状态
    Poti,       //破体状态
}

//角色状态
public enum ObjectState
{
    Idle,
    Battle,
    Born,
    EnterBattle,
}


//战斗关系
public enum BattleRelation
{
    Enemy,  //敌对
    Friend, //友军
    Neutral,//中立
}

//作用单位
public enum EffectTarget
{
    Self,   //自己
    Target, //目标
}

public enum PosType
{
    SelfPos,            //释放者坐标
    ActionTargetPos,    //action时目标坐标
    SkillTargetPos,     //施放技能时目标坐标
    WorldPos
};

namespace xys.battle
{
    /// <summary>
    /// 状态会影响到动作
    /// </summary>
    public enum StateType
    {
        Empty = 0,
        Ice = 1, //冻结
        Stone = 2, //石化
        Float = 3, //浮空
        KnockDown = 4, //倒地
        Weak = 5, //虚弱
        Dizzy = 6, //眩晕
        Suppress = 7, //禁锢
        BeHit = 8, //受击
        BeatBack = 9, //击退
        HitOut = 10, //击飞
        BaoShuai = 11,   //抱摔
        Tow = 12, //牵引


        Idle,
        Move,       //移动和快跑其实可以做成一个状态，为了方便技能施放条件才分开
        Skill,
        Jump,   //轻功
        Dead,
        Sing,   //吟唱
        PlayAni,
        Destroy,
    }

    //buff类型
    public enum BuffType : int
    {
        Empty = 0,
        Bati,   //霸体
        AddAttribute,//添加属性
    };

    //控制状态
    public enum CtrlState
    {
        CanMove,
        CanSkill,
        CanAttack,
    }

    //技能孔位
    public enum Slot
    {
        Attack = 0,
        Skill1,
        Skill2,
        Skill3,
        Skill4,
        Skill5,
        Skill6,
        Skill7,
        Skill8,
        MaxCnt
    };

    //主面板战斗操作
    public enum UIBattleFunType
    {
        SwitchTarget,   //切换目标
    }


    public partial class AniConst
    {
        //动画帧率
        public const int AnimationFrameRate = 30;
        //倒地去掉前后的帧时间，剩下的才是躺地时间
        public const float KnockDownReduceTime = (5 + 11) / 30.0f;
        //击退速度恢复到idle
        public const float BeatBackToIdleTime = 0.5f;
        //浮空转倒地后停留时间
        public const float FloatToGroundStayTime = 2.0f;

        //击飞转倒地后停留时间
        public const float FitOutToGroundStayTime = 2.0f;

        //抱摔转倒地后停留时间
        public const float BaoShuaiToGroundStayTime = 2.0f;
    }
}


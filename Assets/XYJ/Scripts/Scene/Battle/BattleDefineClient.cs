using UnityEngine;
using System.Collections;


namespace xys.battle
{
    //动作相关
    public partial class AniConst
    {
        public const string BeHit = "weak_hit";//头受击
        public const string BeakBack1 = "stong_hit_1";//击退
        public const string BeakBack2 = "stong_hit_2";//击退
        public const string UpToSky = "sky_1";//浮空上升
        public const string Sky = "sky_2";    //浮空
        public const string FallFromSky = "sky_3";//"下落"
        public const string KnockDownFromSky = "sky_4";//"浮空倒地"
        public const string SkyHit = "sky_hit";    //浮空受击
        public const string KnockDown = "knock_down";//倒地
        public const string KnockDownHit = "knock_down_hit";//倒地受击
        public const string Land = "land";//躺地
        public const string StandUp = "stand_up";//倒地起身
        public const string Weak1 = "intoState_2";//进入虚弱
        public const string Weak2 = "weakness";//虚弱
        public const string Dizzy1 = "intoState_1";//进入眩晕
        public const string Dizzy2 = "dizzy";//眩晕
        public const string Dead = "dead";//死亡

        public const string WeakToIdle = "weakness_to_idle_3"; //虚弱到待机
        public const string DizzyToIdel = "dizzy_to_idle_3"; //眩晕到待机
        public const string HitOutMove = "hit_out_1";             //被击飞并移动
        public const string HitOutStay = "hit_out_2";               //被击飞落地不移动

        //宠物不一样部分
        public const string PetEnter = "summon";
        public const string PetExit = "dismiss";

        //主角默认闲时待机动作
        public const string PlayerRelaxIdle = "idle_2";

        //伙伴出场
        public const string PartnerBorn = "intro_1";
        //招架动作
        public const string run_defend = "run_defend";
        public const string stand_defend = "stand_defend";

        public const string jump_1 = "jump_1";
        public const string jump_2 = "jump_2";
        public const string jump_3 = "jump_3";
        public const string jump_4 = "jump_4";
        public const string jump_5 = "jump_5";
        public const string jump_6 = "jump_6";
        public const string jump_down_loop_1 = "jump_down_loop_1";
        public const string jump_down_loop_2 = "jump_down_loop_2";
        public const string jump_land_1 = "jump_land_1";
        public const string jump_5_loop = "jump_5_loop";
        public const string jump_5_land = "jump_5_land";
        public const string jump_land_2 = "jump_land_2";
        public const string jump_land_3 = "jump_land_3";
    }


    //姿态类型
    public class PostureType
    {
        public const int RoleNoInit = -3; //角色姿态没有初始化

        public const int RoleInit = 0; //角色姿态初始化

        public const int SkillSuitAll = -2; //技能属于该姿态时,能被任何姿态使用
        public const int SkillNoChange = -1; //技能切换姿态为该姿态时，则无需切换

        public const int JianKe_JianWu = 1001; //剑客剑舞姿态
        public const int JianKe_YuQi = 1002; //剑客御气姿态

        public const int MoJia_XianShen = 2001; //墨家显身
        public const int MoJia_YinShen = 2002; //墨家隐身

        public const int Pet = 5001; //宠物姿态
        public const int Partner = 5; //伙伴姿态
        public const int Take = 100; //搬运姿态
    }
}


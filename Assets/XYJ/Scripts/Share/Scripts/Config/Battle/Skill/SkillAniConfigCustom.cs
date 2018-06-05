// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class SkillAniConfig
    {
        //动作播放类型
        public enum AniPlayType
        {
            Normal = 0,     //正常播放
            Loop = 1,       //循环
            MathchTime = 2, //动作匹配时间
        }

        public enum AniType
        {
            Normal = 0,           //普通
            Casting = 1,          //施法。其动作播放时间由技能表所填写的施放时间决定
            Prepare = 2,          //攻击准备动作。某些技能在重复使用时，可跳过该动作
            CastingContinue = 3,  //持续施法动作，其动作播放时间由技能表所填写的持续施法时间决定；可以其他指令；同时会打断本次施法；
            AttackPre = 4,        //起手动作。攻击的抬手动作（前摇）；从该动作开始，角色将会不在响应移动等操作
            AttackStay = 5,       //硬直动作。攻击的结束停顿的动作，在该动作下不响应移动操作，但是可响应技能等操作
            AttackAfter = 6,      //收招动作。收招时的动作，可以响应任意指令
            CastingContinueCanMove = 7, //移动持续施法动作，其动作播放时间同持续施法，不影响其他指令，可以移动，多用于旋风斩类技能
        }

        public bool crossFade;
        public List<SkillEventConfig> eventCfgList { get; private set; }
        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }


        static void OnLoadAllCsv()
        {
            foreach (var p in DataList)
            {
                if (!string.IsNullOrEmpty(p.eventid))
                    p.eventCfgList = SkillEventConfig.GetGroupBykey(p.eventid);
                if (p.eventCfgList == null)
                    p.eventCfgList = new List<SkillEventConfig>();
                if (p.aniSpeedMul == 0)
                    p.aniSpeedMul = 1.0f;
            }
        }
    }

}

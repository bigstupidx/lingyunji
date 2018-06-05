// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class SkillConfig 
    {
        //技能类型
        public enum Type : int
        {
            NormalSkill = 0,       //主动技能
            QingGongSkill = 1,     //轻功

            //PetStuntSkill = 3,     //宠物绝技技能
            //NormalAttack = 2,       //普通攻击
            //UnactiveSkill = 3,      //被动技能
            //PowerSkill = 4,         //灵力技能
            //GemSkill = 5,           //法宝技能
        };

        //技能切换属性
        public enum SkillSwitchAttribute
        {
            Default,        //常规技能
            SwitchSkill,    //切换技能
            NextSkill,      //后续技能
        }

        //技能切换类型
        public enum SwitchType
        {
            Null,
            SwitchByTime = 1,       //代表切换技能后,该技能存在时间结束了才会恢复原技能
            SwitchByOperation = 2,  //代表切换技能后,一旦使用了新技能或者移动了,就会恢复原技能;若不做任何操作,则切换时间结束了才会恢复原技能
            SwitchByCD = 3,         //当前技能cd中则会切换到另一个技能,cd结束后又变回来
        }

        public enum CastingBattleState
        {
            All,        //都可以施放
            Battle,     //战斗状态
            NoBattle,   //非战斗状态
        }


        public List<SkillAniConfig> aniCfgList { get; private set; }

        public SkillConditionConfig conditionCfg { get; private set; }

        public SearchAction targetSearchAction { get; private set; }

        //技能属性
        public SkillSwitchAttribute switchAttribute { get; private set; }
        //技能施放的action
        public List<IAction> excuteActions { get; private set; }

        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }
        

        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                if (!string.IsNullOrEmpty(p.aniid) && p.type != Type.QingGongSkill)
                    p.aniCfgList = SkillAniConfig.GetGroupBykey(p.aniid);
                if (p.aniCfgList == null)
                    p.aniCfgList = new List<SkillAniConfig>();
                if (p.castingConditions!=0)
                    p.conditionCfg = SkillConditionConfig.Get(p.castingConditions);
                //策划配孔位从1开始
                if (p.slotid > 0)
                    p.slotid -= 1;

                p.targetSearchAction = ActionManager.GetAction(p.targetSearch) as SearchAction;

                p.excuteActions = ActionManager.ParseActionList(p.excuteActionIds);

                SetSkillSwitchAttribute(p);
            }
        }

        //姿态是否合法
        public bool IsCastingPostureLegal( int objectPosture )
        {
            if (castingPosture.Length == 0)
                return true;
            for(int i=0;i<castingPosture.Length;i++)
            {
                if(castingPosture[i] == objectPosture)
                    return true;
            }
            return false;
        }

        static void SetSkillSwitchAttribute(SkillConfig info)
        {
            //设置技能属性
            if (info.switchAttribute==SkillSwitchAttribute.Default)
            {
                if (info.conditionCfg != null && !info.conditionCfg.isDefaultState)
                    info.switchAttribute = SkillSwitchAttribute.SwitchSkill;
            }

            //a切换出a，则a不算被切换出来的技能
            if (info.id == info.changeSkill)
                return;

            int initId = info.id;
            List<int> idList = new List<int>();

            while(info!=null)
            {
                //重复包含则直接返回
                if (idList.Contains(info.id))
                    return;
                idList.Add(info.id);

                SkillConfig next = null;
                if (info.changeSkill != 0 && DataList.TryGetValue(info.changeSkill, out next))
                {
                    next.switchAttribute = SkillSwitchAttribute.NextSkill;
                }
                info = next;
            }
        }
    }
}

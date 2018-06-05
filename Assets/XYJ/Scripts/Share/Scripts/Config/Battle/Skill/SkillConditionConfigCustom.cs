using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using xys;

namespace Config
{
    public partial class SkillConditionConfig
    {
        //瞬发条件
        public enum Condition
        {
            Null,
            Block,  //格挡成功
            Baoji,  //暴击
        };

        List<StateType> m_stateList;
        bool m_stateFastRun;
        //是否需要检查状态
        public bool isNeedCheckState { get { return m_stateList.Count > 0 || m_stateFastRun; } }

        public Condition condtion { get ;private set;}
        //是否包含常规状态
        public bool isDefaultState { get; private set; }
       

        //是否状态合适,如果都没有条件返回false
        public bool IsState( IObject obj )
        {
            if (obj == null)
                return false;

            if (m_stateFastRun == obj.battle.m_stateMgr.isFastRun && m_stateFastRun)
                return true;

            if (m_stateList.Count>0 )
            {
                if (m_stateList.Contains(obj.battle.m_stateMgr.m_curStType))
                    return true;
            }
            return false;
        }
        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.m_stateList = new List<StateType>();
                foreach( var name in p.state)
                {                  
                    if( name == "常规")
                    {
                        p.isDefaultState = true;
                    }
                    else if (name == "疾跑")
                    {
                        p.m_stateFastRun = true;
                    } 
                    else
                    {
                        StateType stType = AttackActionConfig.GetTypeByName("", name);
                        if (stType != StateType.Empty)
                            p.m_stateList.Add(stType);
                    }

                }

                if (p.block)
                    p.condtion = Condition.Block;
                else
                    p.condtion = Condition.Null;
            }
        }
    }
}
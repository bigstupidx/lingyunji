using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    //切换姿态
    class ChangePostureAction : SimpleAction
    {
        int toPosture;
        float timeLenght;


        public override RunType GetRunType()
        {
            return RunType.ServerOnly;
        }


        public override bool OnExcute(ActionInfo info)
        {
            if(cfg.targetType == EffectTarget.Self)
                info.source.battle.m_attrLogic.SetPosture(toPosture, timeLenght);
            else
            {
                if(info.target!=null && info.target.isAlive)
                    info.target.battle.m_attrLogic.SetPosture(toPosture, timeLenght);
            }
            return true;
        }

        public override bool ParsePara(SimpleActionConfig cfg,string[] para)
        {
            if(!int.TryParse(para[0],out toPosture))
                return false;
            if (para.Length == 2)
            {
                if(!float.TryParse(para[1],out timeLenght))
                    return false;
            }
            else
                timeLenght = -1;
            return true;
        }
    }

    //真气变化
    class ChangeZhenqiAction : SimpleAction
    {
        int addValue;


        public override RunType GetRunType()
        {
            return RunType.ServerOnly;
        }


        public override bool OnExcute(ActionInfo info)
        {
            if (cfg.targetType == EffectTarget.Self)
                info.source.battle.m_attrLogic.Zhenqi_Add(addValue);
            else
            {
                if (info.target != null && info.target.isAlive)
                    info.target.battle.m_attrLogic.Zhenqi_Add(addValue);
            }
            return true;
        }

        public override bool ParsePara(SimpleActionConfig cfg,string[] para)
        {
            if(!int.TryParse(para[0],out addValue))
            {
                return false;
            }
            return true;
        }
    }
}

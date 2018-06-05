using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.battle
{
    public abstract class IAction
    {
        public enum RunType
        {
            ServerOnly,
            ClientOnly,
            ServerToClient,
            Both,
        };

        public class ActionRecord
        {
            //服务器调用时: sourceActionList用来收集需要广播的action
            public Battle_SourceActionList sourceActionList { get; private set; }
            //记录action，一帧只能执行一次的action
            HashSet<int> actionIdRecord;

            public ActionRecord(Battle_SourceActionList sourceActionList)
            {
                this.sourceActionList = sourceActionList;
            }

            public bool IsCanAction(int actionid)
            {
                if (actionIdRecord == null)
                {
                    actionIdRecord = new HashSet<int>();
                    actionIdRecord.Add(actionid);
                    return true;
                }
                else
                {
                    if (actionIdRecord.Contains(actionid))
                        return false;
                    else
                    {
                        actionIdRecord.Add(actionid);
                        return true;
                    }
                }
            }
        }

        public class ActionInfo
        {
            public SkillLogic skill { get; private set; }
            public IObject source { get; private set; }
            public IObject target { get; private set; }
            public Vector3 firePos { get; private set; }
            public float fireAngle { get; private set; }

            //客户端调用时，sourceActionList为null，targetAction为服务器设置的值
            public ActionRecord actionRecord { get; private set; }
            public Battle_TargetAction targetAction { get; private set; }

            public ActionInfo(SkillLogic skill, IObject source, IObject target, ActionRecord actionRecord, Battle_TargetAction targetAction, Vector3 firePos, float fireAngle)
            {
                this.skill = skill;
                this.source = source;
                this.target = target;
                this.actionRecord = actionRecord;
                this.targetAction = targetAction;
                this.firePos = firePos;
                this.fireAngle = fireAngle;
            }
            public void SetTarget(IObject target)
            {
                this.target = target;
            }

            public void SetTargetAction( Battle_TargetAction targetAction)
            {
                this.targetAction = targetAction;
            }

            public void SetFirePos( Vector3 pos,float angle )
            {
                this.firePos = pos;
                this.fireAngle = angle;
            }
        }

        public abstract bool OnExcute(ActionInfo info);
        public abstract RunType GetRunType();
        //同步位置
        public abstract bool IsNeedSysPos();
        public abstract int GetIdHash();
    }


    public abstract class IAction<T>:IAction where T: ActionConfigBase
    {
        public T cfg { get; private set; }

        public void SetCfg(T cfg)
        {
            this.cfg = cfg;
        }

        public override int GetIdHash()
        {
            return cfg.idhash;
        }

        public override bool IsNeedSysPos() { return false; }
    }

    //需要持续时间action可以使用该接口，对应动作结束则事件也会结束
    public interface IActionUpdate
    {
        //返回true表示行为结束，需要结束动画
        bool Update();
        void Stop();
    }
}

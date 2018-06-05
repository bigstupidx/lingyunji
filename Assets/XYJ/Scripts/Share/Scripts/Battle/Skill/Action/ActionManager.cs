using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    static class ActionManager
    {
        static Dictionary<int, IAction> m_actions = new Dictionary<int, IAction>();

        static public void Clear()
        {
            m_actions.Clear();
        }

        static public void AddAction(string id, int idhash, IAction action)
        {
            if (!m_actions.ContainsKey(idhash))
                m_actions.Add(idhash, action);
            else
                XYJLogger.LogError("action id重复 id=" + id);
        }

        static public IAction GetAction(int idhash)
        {
            IAction p;
            if (m_actions.TryGetValue(idhash, out p))
                return p;
            else
                return null;
        }
        static public IAction GetAction(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            IAction action;
            if (m_actions.TryGetValue(BattleHelp.HashCode(id), out action))
                return action;
            else
            {
                XYJLogger.LogError("找不到action id=" + id);
                return null;
            }
        }

        //解释action组
        static public List<IAction> ParseActionList(string[] texts)
        {
            List<IAction> list = new List<IAction>();
            if (texts == null)
                return list;
            foreach (var p in texts)
            {
                IAction action = GetAction(p);
                if (action != null)
                    list.Add(action);
            }
            return list;
        }



#if !COM_SERVER
        //处理action
        static public void HandleActionListAndSendMsg(SkillLogic skill, IObject source, IObject target, List<IAction> actionList, Vector3 firePos, float fireAngle,bool excuteByServer = true)
        {
            HandleActionList(skill, source, target, null, actionList, firePos, fireAngle);
        }


        //处理action,只要有一个成功就算成功
        static public bool HandleActionList(SkillLogic skill, IObject source, IObject target, xys.battle.IAction.ActionRecord actionRecord, List<IAction> actionList, Vector3 firePos, float fireAngle,bool excuteByServer=true )
        {
            if (actionList == null || actionList.Count == 0)
                return false;
            IAction.ActionInfo info = new IAction.ActionInfo(skill, source, target, actionRecord, null, firePos, fireAngle);
            bool ret = false;

            foreach (var p in actionList)
            {
                    if (!BattleHelp.IsRunBattleLogic()
                        &&
                        ( p.GetRunType()== IAction.RunType.ServerOnly || p.GetRunType()== IAction.RunType.ServerToClient))
                        continue;

                    //角色死亡,执行过程中死亡了直接返回true
                    if ((target!=null && !target.isAlive) || !source.isAlive)
                        return ret;
                    ret|=p.OnExcute(info);
            }
            return ret;
        }
#else
        //处理action,并广播
        //实际上只有技能动作执行的事件excuteByServer=false,其他所有地方执行的都是true
        static public void HandleActionListAndSendMsg(SkillLogic skill, IObject source, IObject target, List<IAction> actionList, Vector3 firePos, float fireAngle, bool excuteByServer = true)
        {
            Battle_SourceActionList msg = new Battle_SourceActionList();
            msg.sourceid = source.charSceneId;
            IAction.ActionRecord actionRecord = new IAction.ActionRecord(msg);
            HandleActionList(skill,source, target, actionRecord, actionList,firePos,fireAngle, excuteByServer);
            source.zone.Send(Protoid.A2C_Battle_SkillAction, msg);
        }

        //处理action，并收集需要广播的action
        //只要有一个成功就算成功
        static public bool HandleActionList(SkillLogic skill, IObject source,IObject target, IAction.ActionRecord actionRecord, List<IAction> actionList, Vector3 firePos, float fireAngle,bool excuteByServer=true )
        {
            if (actionList == null || actionList.Count == 0)
                return false;
            bool ret = false;
            IAction.ActionInfo info = new IAction.ActionInfo(skill, source, target, actionRecord, null,firePos,fireAngle);
            foreach (var p in actionList)
            {
                if (p.GetRunType() == IAction.RunType.ClientOnly )
                    continue;

                //角色死亡
                if ((target!=null && !target.isAlive) || !source.isAlive)
                    return true;

                Battle_TargetAction targetAction = null;
                //添加msg
                if (info.actionRecord.sourceActionList != null
                    && (
                        p.GetRunType()== IAction.RunType.ServerToClient
                        //如果事件列表是由服务器执行的，列表中客户端需要执行的事件也需要通知客户端
                        || (excuteByServer && p.GetRunType()!= IAction.RunType.ServerOnly)
                        )
                    )
                {
                    targetAction = new Battle_TargetAction() {
                        actionid = p.GetIdHash(), targetid = info.target == null ? 0 : info.target.charSceneId };
                    firePos.ToPoint3(targetAction.firePos);
                    targetAction.fireAngle = (int)fireAngle;
                    info.actionRecord.sourceActionList.targets.Add(targetAction);
                }
                info.SetTargetAction(targetAction);
                ret|=p.OnExcute(info);

                //同步坐标
                if (info.targetAction != null && p.IsNeedSysPos() && info.target != null)
                {
                    info.target.position.ToPoint3(targetAction.topos);
                }
            }
            return ret;
        }
#endif
    }
}

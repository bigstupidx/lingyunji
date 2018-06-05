using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace xys.battle
{
    public class BattleProtocol
    {
        //是否单机模式
        public static bool testCtrlByLocal = false;

        public BattleProtocol()
        {
            App.my.handler.Reg<Battle_JumpToPos>(Protoid.A2C_Battle_JumpToPos, Jump_OnMoveToPos);
            App.my.handler.Reg<Battle_JumpToPos>(Protoid.A2C_Battle_JumpLand, Jump_OnLand);

            App.my.handler.Reg<Battle_PlaySkill>(Protoid.A2C_Battle_PlaySkill, Skill_OnPlay);
            App.my.handler.Reg<Battle_PlaySkillFail>(Protoid.A2C_Battle_PlaySkillFail, Skill_OnPlayFail);
            App.my.handler.Reg<None>(Protoid.A2C_Battle_StopSkill, Skill_OnStop);
            App.my.handler.Reg<Battle_SourceActionList>(Protoid.A2C_Battle_SkillAction, Skill_OnAction);


            App.my.handler.Reg<Battle_MoveToPos>(Protoid.A2C_Battle_MoveToPos, OnMoveToPos);
            App.my.handler.Reg<Battle_MoveToPos>(Protoid.A2C_Battle_FastMoveToPos, OnFastMoveToPos);
            App.my.handler.Reg<Battle_MoveToPos>(Protoid.A2C_Battle_MoveStop, OnMoveStop);
            App.my.handler.Reg<Battle_SetPos>(Protoid.A2C_Battle_SetPos, OnSetPos);


            App.my.handler.Reg<Battle_Dead>(Protoid.A2C_Battle_Dead, OnDead);
            App.my.handler.Reg<Battle_Attribute>(Protoid.A2C_Battle_Attribute, OnBattleAttribute);
            App.my.handler.Reg<Battle_AddBuff>(Protoid.A2C_Battle_AddBuff, OnAddBuff);
            App.my.handler.Reg<Battle_RemoveBuff>(Protoid.A2C_Battle_RemoveBuff, OnRemoveBuff);
        }


        void OnDead(Network.IPacket pack, Battle_Dead info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.sourceid);
            if (obj == null)
                return;
            obj.SetDead();
        }

        void OnBattleAttribute(Network.IPacket pack, Battle_Attribute info)
        {
            BattleAttri attri = App.my.localPlayer.uiShowBattleAttri;
            foreach (var p in info.values)
            {
                attri.Set(p.Key, p.Value);
            }

            BattleAttriCaculate.GetUIShowAttribute(attri, App.my.localPlayer.uiShowBattleAttri, App.my.localPlayer.job, App.my.localPlayer.levelValue);

            App.my.eventSet.fireEvent(EventID.LocalBattleAttChange);
        }

        void OnAddBuff(Network.IPacket pack, Battle_AddBuff info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.targetid);
            if (obj == null)
                return;
            obj.battle.m_buffMgr.AddBuff(App.my.sceneMgr.GetObj(info.sourceid), info.buffid);
            Debug.Log("addbuff " + info.buffid);
        }

        void OnRemoveBuff(Network.IPacket pack, Battle_RemoveBuff info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.sourceid);
            if (obj == null)
                return;
            obj.battle.m_buffMgr.RemoveBuff(info.buffid);
            Debug.Log("removebuff " + info.buffid);

        }


        #region 移动
        void OnMoveToPos(Network.IPacket pack, Battle_MoveToPos info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.charSceneId) as IObject;
            if (obj == null || obj.battle.m_isAiByLocal)
                return;

            //能移动的技能就不要中断动作
            if (!obj.battle.m_skillMgr.IsSkillCanMove())
                obj.battle.m_stateMgr.ChangeState(StateType.Move);
            obj.battle.m_moveMgr.Remote_SetMoveToPos(info.pos.ToVector3());
        }

        void OnFastMoveToPos(Network.IPacket pack, Battle_MoveToPos info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.charSceneId) as IObject;
            if (obj == null || obj.battle.m_isAiByLocal)
                return;

            obj.battle.m_stateMgr.ChangeState(StateType.Move);
            obj.battle.m_stateMgr.SetFastRun(true);
            obj.battle.m_moveMgr.Remote_SetMoveToPos(info.pos.ToVector3());
        }


        void OnMoveStop(Network.IPacket pack, Battle_MoveToPos info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.charSceneId) as IObject;
            if (obj == null || obj.battle.m_isAiByLocal)
                return;
            obj.battle.m_moveMgr.Remote_SetMoveStop(info.pos.ToVector3());
        }

        void OnSetPos(Network.IPacket pack, Battle_SetPos info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.charSceneId) as IObject;
            if (obj == null)
                return;
            obj.SetPosition(info.pos.ToVector3());
            obj.SetRotate(info.angle);
        }

    
        #endregion

        #region 技能
        void Skill_OnPlayFail(Network.IPacket pack, Battle_PlaySkillFail info)
        {
            //接触技能请求
            ((SkillManagerBase)App.my.localPlayer.battle.m_skillMgr).m_requestSkill = false;
            Debug.Log(" 请求技能失败 " + (SkillManager.PlayResult)info.code);
        }

        void Skill_OnStop(Network.IPacket pack, None info)
        {
            App.my.localPlayer.battle.m_stateMgr.ChangeState(StateType.Idle);
        }

        void Skill_OnPlay(Network.IPacket pack, Battle_PlaySkill info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.sourceid);
            if (obj == null)
                return;

            //接触技能请求
            ((SkillManagerBase)obj.battle.m_skillMgr).m_requestSkill = false;

            Vector3 toPos = info.pos.ToVector3();
            //本地施放轻功不需要等待服务器返回
            if (SkillConfig.Get(info.skillid).type == SkillConfig.Type.QingGongSkill)
            {
                if (info.sourceid == App.my.localPlayer.charSceneId)
                    return;

                //如果位置差别太大就强拉
                if (BattleHelp.GetDistance(toPos, obj.position) > kvBattle.clientSysPosMaxOff * 2)
                    obj.SetPosition(info.pos.ToVector3());
            }
            else
            {
                //不要往下走，不然会掉下地面
                toPos.y = obj.position.y;
                //如果位置差别太大就强拉
                if (BattleHelp.GetDistance(toPos, obj.position) > kvBattle.clientSysPosMaxOff)
                    obj.SetPosition(toPos);
            }

            //设置朝向
            obj.SetRotate(info.rotate);

            obj.battle.m_skillMgr.PlaySkillImpl(info.skillid, App.my.sceneMgr.GetObj(info.targetid), info.searchTarget);
        }

        void Skill_OnAction(Network.IPacket pack, Battle_SourceActionList sourceActionList)
        {
            IObject obj = App.my.sceneMgr.GetObj(sourceActionList.sourceid);
            if (obj == null)
                return;

            IAction.ActionRecord actionRecord = new IAction.ActionRecord(sourceActionList);
            //事件施放坐标和角度客户端应该不需要关注
            IAction.ActionInfo info = new IAction.ActionInfo(obj.battle.m_skillMgr.GetSkillLogic(), obj, null, actionRecord, null, Vector3.zero, 0);

            foreach (var p in sourceActionList.targets)
            {
                //处理一个action
                IAction action = ActionManager.GetAction(p.actionid);
                if (action == null)
                    continue;
                info.SetTarget(App.my.sceneMgr.GetObj(p.targetid));
                info.SetTargetAction(p);
                info.SetFirePos(p.firePos.ToVector3(), p.fireAngle);
                action.OnExcute(info);
            }
        }
  

        #endregion

        #region 轻功
        void Jump_OnMoveToPos(Network.IPacket pack, Battle_JumpToPos info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.charSceneId) as IObject;
            if (obj == null || obj.battle.m_isAiByLocal)
                return;

            obj.battle.m_stateMgr.ChangeState(StateType.Jump);
            obj.SetRotate(info.angle);
            ((JumpManagerRemote)obj.battle.m_jumpMgr).MoveToPos(info.pos.ToVector3(), info.changeSpeed);
        }


        void Jump_OnLand(Network.IPacket pack, Battle_JumpToPos info)
        {
            IObject obj = App.my.sceneMgr.GetObj(info.charSceneId) as IObject;
            if (obj == null || obj.battle.m_isAiByLocal)
                return;

            obj.battle.m_stateMgr.ChangeState(StateType.Jump);
            obj.SetRotate(info.angle);
            obj.battle.m_jumpMgr.PlayJumpLand();
            ((JumpManagerRemote)obj.battle.m_jumpMgr).MoveToPos(info.pos.ToVector3(), info.changeSpeed);
        }

        public bool Jump_Request(SkillConfig cfg, IObject source)
        {
            return source.battle.m_skillMgr.RequestPlaySkill(cfg.id, null);
        }



        public void Jump_SendSpeed(IObject obj, float speed)
        {
            Request_JumpSendPos(obj, speed);
        }


        #endregion

        #region 请求命令
        public void Request_JumpSendPos(IObject obj, float speed = 0)
        {
            if (!obj.battle.m_isAiByLocal)
            {
                Debug.LogError("非本地控制的角色不能发送请求 ");
                return;
            }
            Battle_JumpToPos info = new Battle_JumpToPos();
            obj.position.ToPoint3(info.pos);
            info.charSceneId = obj.charSceneId;
            info.angle = (int)obj.rotateAngle;
            info.changeSpeed = speed;
            App.my.socket.SendGame<Battle_JumpToPos>(Protoid.A2C_Battle_JumpToPos, info);
        }

        public void Request_JumpSendLand(IObject obj)
        {
            if (!obj.battle.m_isAiByLocal)
            {
                Debug.LogError("非本地控制的角色不能发送请求 ");
                return;
            }
            Battle_JumpToPos info = new Battle_JumpToPos() { angle = (int)obj.rotateAngle, charSceneId = obj.charSceneId };
            obj.position.ToPoint3(info.pos);
            App.my.socket.SendGame<Battle_JumpToPos>(Protoid.A2C_Battle_JumpLand, info);
        }
        public bool Request_PlaySkill(IObject source, IObject target, SkillConfig cfg)
        {
            if (!source.battle.m_isAiByLocal)
            {
                Debug.LogError("非本地控制的角色不能发送请求 ");
                return false;
            }

            SkillManagerBase skillMgr = source.battle.m_skillMgr as SkillManagerBase;
            //重复请求施放技能
            if (skillMgr.m_requestSkill && (BattleHelp.timePass - skillMgr.m_requestSkillTime < 5))
            {
                Debug.Log("技能没有返回就请求施放 skillid=" + cfg.id);
                return false;
            }

            skillMgr.m_requestSkill = true;
            skillMgr.m_requestSkillTime = BattleHelp.timePass;

            Battle_PlaySkill info = new Battle_PlaySkill() { rotate = (int)source.rotateAngle, skillid = cfg.id, sourceid = source.charSceneId };
            if (target != null)
            {
                info.targetid = target.charSceneId;
                target.position.ToPoint3(info.targetPos);
            }
            source.position.ToPoint3(info.pos);
            info.rotate = (int)source.rotateAngle;
            App.my.socket.SendGame<Battle_PlaySkill>(Protoid.A2C_Battle_PlaySkill, info);

            //轻功直接施放，不需要等服务器
            if (cfg.type == SkillConfig.Type.QingGongSkill)
                source.battle.m_skillMgr.PlaySkillImpl(cfg, target, source.battle.m_skillMgr.GetSkillSearchTargets(cfg, target));
            return true;
        }

        public void Request_MoveSendSys(IObject obj, StateType stType, bool isStop)
        {
            if (!obj.battle.m_isAiByLocal)
            {
                Debug.LogError("非本地控制的角色不能发送请求 ");
                return;
            }

            Battle_MoveToPos info = new Battle_MoveToPos();
            obj.position.ToPoint3(info.pos);
            info.charSceneId = obj.charSceneId;
            if (isStop)
                App.my.socket.SendGame<Battle_MoveToPos>(Protoid.A2C_Battle_MoveStop, info);
            else
            {
                if (obj.battle.m_stateMgr.isFastRun)
                    App.my.socket.SendGame<Battle_MoveToPos>(Protoid.A2C_Battle_FastMoveToPos, info);
                else
                    App.my.socket.SendGame<Battle_MoveToPos>(Protoid.A2C_Battle_MoveToPos, info);
            }
        }

        public void Request_StopSkill( IObject obj )
        {
            if (!obj.battle.m_isAiByLocal)
            {
                Debug.LogError("非本地控制的角色不能发送请求 ");
                return;
            }
            if (!obj.battle.m_skillMgr.m_requestStopSkill)
            {
                App.my.socket.SendGame(Protoid.A2C_Battle_StopSkill, new None());
                obj.battle.m_skillMgr.m_requestStopSkill = true;
            }
        }

        public void Request_FastRun( IObject obj,bool run )
        {
            if (!obj.battle.m_isAiByLocal)
            {
                Debug.LogError("非本地控制的角色不能发送请求 ");
                return;
            }
            Battle_FastRun info = new Battle_FastRun();
            info.fustRun = run ? 1 : 0;
            if (obj.battle.m_isAiByLocal)
                App.my.socket.SendGame<Battle_FastRun>(Protoid.C2A_Battle_AttFastRun, info);
        }

        #endregion
    }
}

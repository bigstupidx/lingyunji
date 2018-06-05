using UnityEngine;
using System.Collections;
using Battle;
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using xys;
using xys.battle;

public partial class GM_Cmd
{
    //添加所有gm命令,命令不区分大小写
    void RegisterAllCmd()
    {
        #region 战斗
        AddLocalCmd(OnParseTest, "test", "测试", "");
        AddLocalCmd(OnParseNetDelay, "delay", "网络延时", "最小 最大");
        AddLocalCmd(OnChangeBattleState, "battleState", "战斗状态", "");
        AddLocalCmd(OnPlayObjectEffect, "effect", "角色特效", "effectid time");
        AddLocalCmd(OnClientAI, "clientai", "客户端ai", "ai charid");
        AddLocalCmd(OnPath, "path", "寻路", "x y z");
        #endregion


        AddLocalCmd(UseLocalItem, "useloalitem", "测试使用道具接口", "id, count");
    }

    //添加本地命
    void AddLocalCmd(Action<CmdParse> handleFun, string cmd, string cmdExplain, string paraExplain,bool addToHelp=false)
    {
        AddCmd(handleFun, cmd, cmdExplain, paraExplain, false, addToHelp);
    }

    #region gm命令处理函数
    void OnParseTest(CmdParse para )
    {
        IObject target= App.my.localPlayer.battle.GetTarget();
        if (target == null)
            return;
        string name = para.GetStr();
        int frame=para.GetInt();
        float normalizedTime = frame/30.0f/target.battle.m_aniMgr.GetAniLenght(name);
        target.battle.m_aniMgr.PlayAni(name, 1.0f,0, normalizedTime);
    }

    void OnChangeBattleState(CmdParse para)
    {
        App.my.localPlayer.battle.m_attrLogic.SetBattleState(!App.my.localPlayer.battle.m_attrMgr.battleState);
    }

    void OnPlayObjectEffect(CmdParse para)
    {
        App.my.localPlayer.battle.m_effectMgr.AddEffect(para.GetStr(),para.GetFloat());
    }
    void OnClientAI(CmdParse para)
    {
        int ai = para.GetInt();
        int charid = para.GetInt();
        //服务器取消ai
        ParseCmd("ai 0 "+charid);

        IObject obj = App.my.sceneMgr.GetObj(charid);
        if(obj!=null)
        {
            obj.battle.m_ai.ChangeAI((SimpleAIType)ai);
        }
    }

    void OnPath(CmdParse para)
    {
        App.my.localPlayer.battle.State_PathToPos(new Vector3(para.GetFloat(), para.GetFloat(), para.GetFloat()));
    }

    void OnParseNetDelay(CmdParse para)
    {
#if COM_DEBUG
        App.my.debugNetDispatcher.OpenDelay(para.GetFloat(), para.GetFloat());
#endif
    }
    
    void UseLocalItem(CmdParse para)
    {
        App.my.localPlayer.GetModule<PackageModule>().UseItemById(para.GetInt(), para.GetInt());
    }
    #endregion
}

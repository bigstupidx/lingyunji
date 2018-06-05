using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.gm
{
    public class GM_BattlePage :GM_IPage
    {
        public void OnOpen()
        {

        }
        public void OnClose()
        {

        }
        public string GetTitle()
        {
            return "战斗";
        }

        
        public void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);

            //string temStr = GM_GUIHelper.TextField("测试技能", SkillManager.s_testSkillId.ToString());
            //int.TryParse(temStr, out SkillManager.s_testSkillId);

            //if (GUILayout.Button("被击"))
            //    App.my.localPlayer.battle.m_stateMgr.ChangeHitState(battle.StateType.BeHit, null, 1.0f);
            if (GUILayout.Button("击退"))
            {
                Vector3 toPos = App.my.localPlayer.position - App.my.localPlayer.root.transform.forward * 2;
                App.my.localPlayer.battle.m_stateMgr.ChangeHitState(battle.StateType.BeatBack, toPos, 0.2f + AniConst.BeatBackToIdleTime);
            }
            //if (GUILayout.Button("虚弱"))
            //    App.my.localPlayer.battle.m_stateMgr.ChangeHitState(battle.StateType.Weak, null, 2.0f);

            //if (GUILayout.Button("击飞"))
            //    App.my.localPlayer.battle.m_stateMgr.ChangeHitState(battle.StateType.Float, null, 3.0f);

            //if (GUILayout.Button("倒地"))
            //    App.my.localPlayer.battle.m_stateMgr.ChangeHitState(battle.StateType.KnockDown, null, 3.0f);

            //if (GUILayout.Button("死亡"))
            //    App.my.localPlayer.SetDead();

            if (GUILayout.Button("吟唱"))
                App.my.localPlayer.battle.State_Sing(1);

            if (GUILayout.Button("动画"))
                App.my.localPlayer.battle.State_PlayAni("dead");


            //BattleProtocol.testCtrlByLocal = GUILayout.Toggle(BattleProtocol.testCtrlByLocal,"单机测试技能");

            if (GUILayout.Button("重载配置F1"))
            {
                ReloadConfig();
            }

            if (GUILayout.Button("登录界面"))
            {
                App.my.BackToLogin();
            }

            if (GUILayout.Button("删除所有怪"))
                GM_Cmd.instance.ParseCmd("clearobject");

            GM_UI.s_showObjectInfo = GUILayout.Toggle(GM_UI.s_showObjectInfo,"显示角色属性");

            if (App.my.localPlayer != null && App.my.localPlayer.isAlive)
            if (App.my!=null && App.my.localPlayer != null && App.my.localPlayer.isAlive)
            {
                GUILayout.Label(string.Format("{0} {1}", App.my.localPlayer.battle.m_buffMgr.GetFlag(BuffManager.Flag.NoMove), App.my.localPlayer.battle.m_buffMgr.GetFlag(BuffManager.Flag.NoSkill)));

                GUILayout.Label(string.Format("battle={0} posture={1}", App.my.localPlayer.battle.m_attrMgr.battleState, App.my.localPlayer.postureValue));
            }

            GUILayout.EndArea();

        }

        public static void ReloadConfig()
        {
            //服务器重载配置
            GM_Cmd.instance.ParseCmd("reloadconfig");
            CsvLoadAdapter.All();

            foreach(var p in App.my.sceneMgr.GetObjs())
                ((ObjectBase)p.Value).GM_Reload();

            behaviac.Workspace.Instance.MyReLoad();

        }

    }

}

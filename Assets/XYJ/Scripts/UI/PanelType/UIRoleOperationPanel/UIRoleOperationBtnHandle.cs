using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    //玩家信息面板展示类型
    public enum RoleOperShowType
    {
        Defined,            //自定义
        Custom,             //一般
        FriendEnemyPanel,   //好友仇敌面板
        RecentlyListPanel,  //最近联系人面板
        Rank,               // 排行榜
    }

    //玩家信息面板按钮类型
    public enum RoleOperBtnType
    {
        Chat,                   //聊天
        CheckInfo,              //查看信息

        FriendAdd,              //好友添加
        FriendDelete,           //好友删除

        TeamApplyJoin,          //组队申请加入
        TeamInviteJoin,         //组队邀请加入
        TeamTransferLeader,     //组队转让队长
        TeamForceLeave,         //组队请离
        TeamLeave,              //组队离开

        ClanInviteJoin,         //氏族邀请加入
        ClanApplyJoin,          //氏族申请加入
        ClanLeaderTransfer,     //氏族族长禅让
        ClanLeaderImpeach,      //氏族弹劾族长
        ClanKick,               //氏族逐出

        BlacklistDelete,        //黑名单删除
        BlacklistAdd,           //黑名单加入

        Report,                 //举报
        EnemyDelete,            //仇敌删除

        RecentlyListRemove,     //最近列表删除
    }

    //玩家信息面板数据
    public class UIRoleOperationData : object
    {
        public long playerId;
        public int jobSex = 0;
        public int job = 1;
        public int level = 0;
        public string name = "";
        public string clamName = "无";
        public string team = "无";

        public Vector3 panelPos = Vector3.zero;
        public RoleOperShowType showType = RoleOperShowType.Custom;
        public List<UIRoleOperationBtnHandle> handleList;

        public UIRoleOperationData(long charId, RoleOperShowType roleOperShowType)
        {
            playerId = charId;
            showType = roleOperShowType;
        }
    }

    //玩家信息面板按钮定义
    public class UIRoleOperationBtnHandle
    {
        RoleOperBtnType m_BtnType;
        GameObject m_GameObject;
        Func<bool> m_Fun;
        Func<bool> m_FunRefreshShow;
        long m_PlayerId;

        public GameObject gameObject { get { return m_GameObject; } }

        // 玩家信息面板按钮定义（按钮类型，按钮执行事件（返回false为关闭信息面板），按钮显示条件（返回true为显示））
        public UIRoleOperationBtnHandle(RoleOperBtnType btnType, Func<bool> fun, Func<bool> funRefreshShow)
        {
            m_BtnType = btnType;
            m_Fun = fun;
            m_FunRefreshShow = funRefreshShow;
        }

        public void SetBtn(Transform tf, long playerId)
        {
            Transform btnTf = tf.Find(m_BtnType.ToString());
            m_GameObject = btnTf.gameObject;
            Button btn = btnTf.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ClickHandle);

            m_PlayerId = playerId;
        }

        protected virtual void ClickHandle()
        {
            if (m_Fun != null)
            {
                if (!m_Fun())
                {
                    App.my.uiSystem.HidePanel(PanelType.UIRoleOperationPanel);
                }
            }
        }

        public virtual void RefreshShowCondition()
        {
            if (m_FunRefreshShow != null)
                m_GameObject.SetActive(m_FunRefreshShow());
        }
    }
}
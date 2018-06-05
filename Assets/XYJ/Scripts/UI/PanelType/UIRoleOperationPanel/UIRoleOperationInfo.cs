#if !USE_HOT
using Config;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIRoleOperationInfo
    {
        [SerializeField]
        Image m_Icon;       //头像
        [SerializeField]
        Image m_IconBg;     //头像底图
        [SerializeField]
        Image m_Job;        //职业
        [SerializeField]
        Image m_JobColor;   //职业彩色
        [SerializeField]
        Text m_PlayerId;    //玩家ID
        [SerializeField]
        Text m_Level;       //等级
        [SerializeField]
        Text m_Name;        //名字
        [SerializeField]
        Text m_ClanName;    //氏族名称
        [SerializeField]
        Text m_Team;        //队伍

        public void SetRoleInfo(int jobSex, int job, string name, int level, string clamName, string teamMembers)
        {
            RoleJob roleJob = RoleJob.Get(job);

            //1为男性角色
            Helper.SetSprite(m_Icon, jobSex == 1 ? roleJob.maleIcon : roleJob.femalIcon);
            Helper.SetSprite(m_IconBg, jobSex == 1 ? roleJob.maleHeadBack : roleJob.femalHeadBank);
            Helper.SetSprite(m_Job, roleJob.icon);
            Helper.SetSprite(m_JobColor, roleJob.colorIcon);

            m_Name.text = name;
            m_Level.text = level.ToString();

            SetRoleClanInfo(clamName);
            SetRoleTeamInfo(teamMembers);
        }

        public void SetRoleClanInfo(string clamName)
        {
            m_ClanName.text = string.IsNullOrEmpty(clamName) ? "无" : clamName;
        }

        public void SetRoleTeamInfo(string teamMembers)
        {
            m_Team.text = string.IsNullOrEmpty(teamMembers) ? "0/0" : teamMembers;
        }
    }
}
#endif
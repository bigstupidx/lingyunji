using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.UI
{
    public class ModelUIRoleTitle : MonoBehaviour
    {

        #region UI Objects Reference

        // 名字
        public Text m_nickName;
        public Outline m_nickOutline;

        // 称号
        public Text m_titleName;
        public Outline m_titleOutline;

        // 角色类型 -- 0:不显示类型，1:帮派，2:精英怪
        public StateRoot m_typeState;
        public Text m_bangpaiName;

        // 队伍标识
        public StateRoot m_teamSign;// 0:无，1:队长，2:队员

        // 任务图标状态
        public StateRoot m_taskSign;// 0:无任务，1:可接任务，2:已完成任务（可交），3:有在执行任务

        // PK标识
        public GameObject m_pkSign;

        // 血条
        public GameObject m_bloodObj;
        public Image m_bloodImage;

        #endregion

        public class Cxt
        {
            public string nickName;
            public Color nickColor;
            public Color nickOutline;

            public string titleName;
            public Color titleColor;
            public Color titleOutline;

            public int roleType = 0;// 0:无类型，1:帮派，2:精英怪
            public string bangpaiName;
            public string bangPaiColor;

            public int taskSign = 0;

            public int teamSign = 0;

            public int pkSign = 0;

            public int blood = 0;// 0:无血条
        }

        public void SetData(Cxt cxt)
        {
            if (cxt == null)
                return;

            SetName(cxt.nickName, cxt.nickColor, cxt.nickOutline);
            SetTitle(cxt.titleName, cxt.titleColor, cxt.titleOutline);

            if (cxt.roleType == 0 || cxt.roleType == 1)
                SetBangpai(cxt.bangpaiName);
            else
                SetMonster();

            SetTaskSign(cxt.taskSign);

            SetTeamSign(cxt.teamSign);

            SetPKSign(cxt.pkSign == 0 ? false : true);

            SetBlood(0);

        }

        public void ResetObject()
        {
            m_nickName.text = "";
            m_titleName.text = "";
            m_typeState.SetCurrentState(0, false);

            m_taskSign.SetCurrentState(0, false);
            m_teamSign.SetCurrentState(0, false);
            m_pkSign.SetActive(false);
            m_bloodObj.SetActive(false);
        }

        public void SetName(string name, Color nameColor, Color nameOutline)
        {
            m_nickName.text = name;

            if (string.IsNullOrEmpty(name))
                return;
            if (nameColor.a == 0)
                return;
            m_nickName.color = nameColor;
            m_nickOutline.effectColor = nameOutline;
        }

        public void SetTitle(string titleName, Color titleColor, Color titleOutline)
        {
            m_titleName.text = string.IsNullOrEmpty(titleName) ? titleName : string.Format("<{0}>", titleName);

            if (string.IsNullOrEmpty(titleName))
                return;
            if (titleColor.a == 0)
                return;
            m_titleName.color = titleColor;
            m_titleOutline.effectColor = titleOutline;
        }

        public void SetBlood(int type)
        {
            if (type == 0)
                m_bloodObj.SetActive(false);
            else
                m_bloodObj.SetActive(true);
        }

        // 任务图标状态
        // 0:无任务，1:可接任务，2:已完成任务（可交），3:有在执行任务
        public void SetTaskSign(int sign)
        {
            m_taskSign.SetCurrentState(sign, false);
        }

        public void SetBangpai(string name)
        {
            if (string.IsNullOrEmpty(name))
                m_typeState.SetCurrentState(0, true);
            else
            {
                m_typeState.SetCurrentState(1, true);
                m_bangpaiName.text = name;
            }
        }

        public void SetMonster()
        {
            m_typeState.SetCurrentState(2, true);
        }

        public void SetTeamSign(int sign)
        {
            m_teamSign.SetCurrentState(sign, false);
        }

        public void SetPKSign(bool active)
        {
            m_pkSign.SetActive(active);
        }

        void OnBloodChange()
        {

        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

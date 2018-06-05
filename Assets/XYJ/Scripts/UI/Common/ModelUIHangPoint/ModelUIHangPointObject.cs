using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace xys.UI
{

    /// <summary>
    /// 1.基本信息：名字，称号，标识（帮派,组队,任务等）
    /// 2.冒泡
    /// </summary>
    public class ModelUIHangPointObject : MonoBehaviour
    {
        /// <summary>
        /// 挂点数据
        /// </summary>
        public class Cxt
        {
            // 挂点基本数据
            public string nickName;
            public Config.NameColorConfig.ColorType nameColorType = Config.NameColorConfig.ColorType.PlayerGreen;

            public string titleName;
            public Config.NameColorConfig.ColorType titleColorType = Config.NameColorConfig.ColorType.PlayerTitleColor;
            public string titleColorStr;
            public string titleOutlineStr;

            // 角色标识类型
            public int signType = 0;// 0:无类型，1:帮派，2:精英怪
            public string bangpaiName;
            public string bangPaiColor;

            // 任务标识状态
            public int taskSign = 0;//0:无任务，1:可接任务，2:已完成任务（可交），3:有在执行任务

            // 组队标识状态
            public int teamSign = 0;//0:无，1:队长，2:队员

            // pk状态
            public int pkSign = 0;// 0:无pk，1有

            public int bloodType = 0;//0:无血条，1红色，2绿色

            // ================================

            // 挂点冒泡数据
            public string bubblingText;// 当前冒泡
            public float bublingTime;// 冒泡事件

            // 随机冒泡
            public string[] randomBubblingTexts;
            public float randomBubblingTime;
            public float randomIntervalTime;
        }

        // 挂点对象
        public ModelUIRoleTitle m_title;
        public ModelUIBubbling m_bubbling;

        // 内部共享数据
        int m_taskSign = 0;

        public void ResetConfig()
        {
            m_taskSign = 0;
            m_title.ResetObject();
            m_bubbling.ResetObject();
            m_title.gameObject.SetActive(true);
        }

        public void SetData(Cxt data)
        {

            SetName(data.nickName, data.nameColorType);
            if (string.IsNullOrEmpty(data.titleColorStr))
            { SetTitle(data.titleName, data.titleColorType); }
            else
            { SetTitle(data.titleName, data.titleColorStr, data.titleOutlineStr); }

            SetBlood(data.bloodType);
            SetTaskSign(data.taskSign);

            SetTeamSign(data.teamSign);
            SetPKSign(data.pkSign);

            // 根据标识设置
            if (data.signType == 1)
                SetBangpaiSign(data.bangpaiName);
            else if (data.signType == 2)
                SetMonsterSign();

            // 冒泡设置
            bool hasBubbling = true;
            if (string.IsNullOrEmpty(data.bubblingText))
                hasBubbling = false;
            SetRandomBubbling(data.randomBubblingTexts, data.randomBubblingTime, data.randomIntervalTime, !hasBubbling);

            if (hasBubbling)
            {
                ShowBubbling(data.bubblingText, data.bublingTime);
            }
        }

        public void SetName(string name, Config.NameColorConfig.ColorType type = Config.NameColorConfig.ColorType.PlayerGreen)
        {
            Color[] colors = Config.NameColorConfig.GetColors(type);
            m_title.SetName(name, colors[0], colors[1]);
            if (type == global::Config.NameColorConfig.ColorType.NPC)
                m_bubbling.SetNickName(name, 1);
            else
                m_bubbling.SetNickName(name, 0);
        }

        public void SetTitle(string title, Config.NameColorConfig.ColorType type = Config.NameColorConfig.ColorType.PlayerTitleColor)
        {
            Color[] colors = Config.NameColorConfig.GetColors(type);
            m_title.SetTitle(title, colors[0], colors[1]);
        }

        public void SetTitle(string title, string titleColor, string titleOutline)
        {
            Color main = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName(titleColor));
            Color outline = Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName(titleOutline));
            m_title.SetTitle(title, main, outline);
        }

        public void SetTaskSign(int sign)
        {
            m_taskSign = sign;
            m_title.SetTaskSign(sign);
            m_bubbling.SetTaskStatusIcon(sign);
        }

        public void SetBlood(int type)
        {
            m_title.SetBlood(type);
        }

        public void SetBangpaiSign(string name)
        {
            m_title.SetBangpai(name);
        }

        public void SetMonsterSign()
        {
            m_title.SetMonster();
        }

        public void SetTeamSign(int sign)
        {
            m_title.SetTeamSign(sign);
        }

        public void SetPKSign(int sign)
        {
            if (sign == 1)
                m_title.SetPKSign(true);
            else
                m_title.SetPKSign(false);
        }

        /// <summary>
        /// 随机冒泡
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="bubblingTime"></param>
        public void SetRandomBubbling(string[] contents, float bubblingTime, float intervalTime, bool immediate = true)
        {
            m_bubbling.SetRandomBubbling(contents, bubblingTime, intervalTime, immediate);
        }

        public void ShowBubbling(string content, float bubblingTime)
        {
            m_bubbling.ShowBubbling(content, bubblingTime);
        }

        public void ShowBubbling(string content, float bubblingTime, int taskSign)
        {
            m_bubbling.ShowBubbling(content, bubblingTime, taskSign);
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
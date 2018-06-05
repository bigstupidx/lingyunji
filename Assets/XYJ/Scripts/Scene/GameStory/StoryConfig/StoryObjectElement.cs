using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    [System.Serializable]
    public class StoryObjectElement
    {
        bool m_toggle = true;// 编辑器用
        public bool toggle
        {
            get { return m_toggle; }
            set { m_toggle = value; }
        }

        // 描述
        public string describe;

        // 刷新点对象
        public string storyObjectID;

        // 角色对象定义
        public StoryObjectDefine objectDefine;

        // 刷角色位置
        public Points points = new Points();

    }

    /// <summary>
    /// 一些角色设定数据，表现配置
    /// </summary>
    [System.Serializable]
    public class StoryObjectDefine
    {
        public string model;
        public string name;
        public int type;// 对象类型

        // 其他，如称号等
        //public string title = string.Empty;
        //public int posture = 0;//姿态
        //public float moveSpeed = 10f;移动速度
    }

}

namespace xys
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using Config;

    /// <summary>
    /// 任务对话原型
    /// </summary>
    public class TaskDialogPrototype
    {
        public int dialogId;// 对白id
        public int dialogIdx;// 对白编号，从1开始

        public int roleId;// 说对白的角色
        public string roleIcon;
        public string roleName;
        public string roleAni;

        public int taskId = 0;// 关联的任务id
        public string taskDesc;// 任务描述

        public string content;// 对白内容

        public int baseType;//对白类型，1全屏对白，2旁白，3过场动画对白（暂时无）
        public int uiType;// 针对全屏对白的分类：1普通选项，2左右大分支选项，3两个小分支选项
        public int camType;// 0不用镜头，1角色镜头，2模型镜头

        public float autoPlayTime;// 自动播放时间
        public bool shieldOpt;// 屏蔽操作

        public bool isGroupEnd = false;// 标记对白结束

        // 选项列表
        public List<DialogOptionData> m_options = null;

        /// <summary>
        /// 根据配置初始化
        /// </summary>
        /// <param name="define"></param>
        public void InitByDefine(TaskDialogDefine define)
        {
            dialogId = define.groupId;
            dialogIdx = define.numIdx;

            roleId = define.roleId;
            roleIcon = define.roleIcon;
            roleName = define.roleName;
            roleAni = define.aniName;

            // 处理角色头像
            if (string.IsNullOrEmpty(roleIcon))
            {
                if (roleId == -1)
                    roleIcon = App.my.localPlayer.cfgInfo.headIcon;
                else
                {
                    RoleDefine rd = RoleDefine.Get(roleId);
                    if (rd != null)
                        roleIcon = rd.headIcon;
                }
            }

            // 处理角色名字问题
            if (string.IsNullOrEmpty(roleName))
            {
                if (roleId == -1)
                    roleName = App.my.localPlayer.name;
                else
                {
                    RoleDefine rd = RoleDefine.Get(roleId);
                    if (rd != null)
                        roleName = rd.name;
                }
            }
            else if (roleName == "$name")
            {
                roleName = App.my.localPlayer.name;
            }

            content = GlobalSymbol.ToUT(ParseRoleTitle(define.content, App.my.localPlayer.sexValue));

            baseType = define.baseType;
            uiType = define.uiType;
            camType = define.camType;

            autoPlayTime = define.autoPlayTime;
            shieldOpt = define.shieldOpt;

            isGroupEnd = define.isGroupEnd;

            if (baseType == 1)
            {
                m_options = new List<DialogOptionData>();
                // 生成选项
                if (!string.IsNullOrEmpty(define.optName1))
                {
                    DialogOptionData opt = new DialogOptionData();
                    opt.title = define.optName1;
                    opt.desc = define.optDesc1;
                    opt.optIndex = 1;
                    opt.nextTaskIdx = define.nextNumIdx1;
                    opt.IsFinishDialog = define.optIsClose1;
                    opt.IsFireEvent = (define.optResult1 == 1);

                    m_options.Add(opt);
                }

                if (!string.IsNullOrEmpty(define.optName2))
                {
                    DialogOptionData opt = new DialogOptionData();
                    opt.title = define.optName2;
                    opt.desc = define.optDesc2;
                    opt.optIndex = 2;
                    opt.nextTaskIdx = define.nextNumIdx2;
                    opt.IsFinishDialog = define.optIsClose2;
                    opt.IsFireEvent = (define.optResult2 == 1);

                    m_options.Add(opt);
                }

                // 处理配了选项但，没配UI类型
                if (uiType == 0 && m_options.Count > 0)
                {
                    uiType = 1;
                }

                if (uiType == 1)
                {
                    if (!string.IsNullOrEmpty(define.optName3))
                    {
                        DialogOptionData opt = new DialogOptionData();
                        opt.title = define.optName3;
                        opt.optIndex = 3;
                        opt.nextTaskIdx = define.nextNumIdx3;
                        opt.IsFinishDialog = define.optIsClose3;
                        opt.IsFireEvent = (define.optResult3 == 1);

                        m_options.Add(opt);
                    }

                    if (!string.IsNullOrEmpty(define.optName4))
                    {
                        DialogOptionData opt = new DialogOptionData();
                        opt.title = define.optName4;
                        opt.optIndex = 4;
                        opt.nextTaskIdx = define.nextNumIdx4;
                        opt.IsFinishDialog = define.optIsClose4;
                        opt.IsFireEvent = (define.optResult4 == 1);

                        m_options.Add(opt);
                    }
                }
            }
        }

        /// <summary>
        /// 播放对话
        /// </summary>
        /// <param name="end"></param>
        public void Play(Action<int> end)
        {
            if (baseType == 1)
            {
                // 全屏对白
                App.my.uiSystem.ShowPanel("UIDialoguePanel", new object[] { this, end }, false);

            }
            else if (baseType == 2)
            {
                // 旁白
                App.my.uiSystem.ShowPanel("UIStoryAsidePanel", new object[] { this, end }, false);
            }
            else
            {
                Debuger.LogError(string.Format("对白={0}，编号={1}，没有定义对白类型：{2}", dialogId, dialogIdx, baseType));
            }
        }

        /// <summary>
        /// 对话结束
        /// </summary>
        public void Finished()
        {

        }

        /// <summary>
        /// 强制关闭对话
        /// </summary>
        public void Stoy()
        {
            if (baseType == 1)
            {
                App.my.uiSystem.HidePanel("UIDialoguePanel", false);
            }
            else if (baseType == 2)
            {
                App.my.uiSystem.HidePanel("UIStoryAsidePanel", false);
            }
        }

        #region 静态方法


        public static string ParseRoleTitle(string text, int sex)
        {
            if (text.Contains("$name"))
            {
                text = text.Replace("$name", App.my.localPlayer.name);
            }
            if (!text.Contains("[sex"))
                return text;
            Regex reg = new Regex(@"(?<=\[sex)[^\]]+(?=\])");
            //#[sex|哥哥|姐姐]#n   格式
            MatchCollection mc = reg.Matches(text);
            //替换原文本
            foreach (Match m in mc)
            {
                string origin = "[sex" + m.Value + "]";

                //判断主角性别
                string[] names = m.Value.Split(':');
                string name = (1 == sex) ? names[0] : names[1];
                text = text.Replace(origin, name);
            }

            return text;
        }

        #endregion

    }

    /// <summary>
    /// 选项数据
    /// 选项功能：关闭对话界面，后续对白，通知消息，功能操作
    /// </summary>
    public class DialogOptionData
    {
        public string title;
        public string desc;
        public int optIndex = 0;// 选项编号，编号从1开始，0为无效编号
        public int funId = 0;// 功能id

        public int nextTaskIdx = 0;// 后续任务编号
        public bool IsFinishDialog = false;// 结束整个对话
        public bool IsFireEvent = false;// 广播事件
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace xys.UI
{
    /// <summary>
    /// 提供给游戏内角色用，根据角色来处理
    /// </summary>
    public class ActorHangPointHandler : UIHangPointHandleBase
    {
        private ObjectBase m_actorObj;

        public void Init(Transform titleRoot, ObjectBase obj)
        {
            m_rootObject = new TransformWrap(titleRoot);
            m_actorObj = obj;

            LoadObject();
            SetName();
            SetTitle();

            App.my.eventSet.Subscribe(EventID.Title_Change, SetTitle);
            m_actorObj.eventSet.Subscribe(NetProto.AttType.AT_TeamIsLeader, OnTeamInfoChange);        
        }

        private void OnTeamInfoChange()
        {
            long teamid = m_actorObj.attributes.Get(NetProto.AttType.AT_TeamID).longValue;
            int teamSign = 0;
            if (teamid > 0)
                teamSign = (0 != m_actorObj.attributes.Get(NetProto.AttType.AT_TeamIsLeader).byteValue) ? 1 : 2;
            this.SetTeam(teamSign);
        }

        private void SetTeam(int teamSign)
        {
            if (null != m_object)
                m_object.SetTeamSign(teamSign);
            else
                Data.teamSign = teamSign;
        }

        public void SetName()
        {
            if (m_actorObj.cfgInfo.IsHideName())
                return;
            string name = string.Empty;
            Config.NameColorConfig.ColorType clrType = Config.NameColorConfig.ColorType.PlayerGreen;
            switch(m_actorObj.type)
            {
                case NetProto.ObjectType.Player:
                    {
                        name = m_actorObj.name;
                        clrType = Config.NameColorConfig.ColorType.PlayerGreen;
                    }
                    break;
                case NetProto.ObjectType.Npc:
                    {
                        name = m_actorObj.cfgInfo.name;
                        clrType = Config.NameColorConfig.ColorType.NPC;
                    }
                    break;
                case NetProto.ObjectType.Pet:
                    {
                        name = m_actorObj.cfgInfo.name;
                        clrType = Config.NameColorConfig.ColorType.Pet;
                    }
                    break;
                default:
                    clrType = Config.NameColorConfig.ColorType.PlayerGreen;
                    break;
            }
            
            if (m_object!=null)
            {
                m_object.SetName(name, clrType);
            }
            else
            {
                Data.nickName = name;
                Data.nameColorType = clrType;
            }
        }

        public void SetTitle()
        {
            string title = string.Empty;
            Config.NameColorConfig.ColorType clrType = Config.NameColorConfig.ColorType.PlayerTitleColor;
            switch (m_actorObj.type)
            {
                case NetProto.ObjectType.Player:
                    {
                        // 获取玩家称号
                        if (m_actorObj is LocalPlayer)
                        {
                            LocalPlayer lp = (LocalPlayer)m_actorObj;
                            // 这里获取信息bu
                            NetProto.TitleList td = lp.GetModule<TitleModule>().m_TitleListData as NetProto.TitleList;
                            if (td.currTitle == 0)// 如果为0表示无称号
                                return;
                            Config.RoleTitle rt = Config.RoleTitle.Get(td.currTitle);
                            if (rt != null)
                            {
                                title = rt.name;
                                if (m_object != null)
                                {
                                    m_object.SetTitle(title, rt.topEquilty, rt.topStoke);
                                }
                                else
                                {
                                    Data.titleName = title;
                                    Data.titleColorStr = rt.topEquilty;
                                    Data.titleOutlineStr = rt.topStoke;
                                }
                                return;
                            }
                        }
                        
                        // 玩家根据称号类型设置颜色类型
                    }
                    break;
                case NetProto.ObjectType.Npc:
                    {
                        clrType = Config.NameColorConfig.ColorType.NpcCall;
                    }
                    break;
                default:
                    clrType = Config.NameColorConfig.ColorType.PlayerTitleColor;
                    break;
            }
            if (m_object != null)
            {
                m_object.SetTitle(title, clrType);
            }
            else
            {
                Data.titleName = title;
                Data.titleColorType = clrType;

                Data.titleColorStr = string.Empty;
                Data.titleOutlineStr = string.Empty;
            }
        }

        // 称号颜色转换
        Config.NameColorConfig.ColorType Color2Color(Config.ItemQuality type)
        {
            switch (type)
            {
                case Config.ItemQuality.white:
                    return Config.NameColorConfig.ColorType.PlayerWhiteCall;
                case Config.ItemQuality.green:
                    return Config.NameColorConfig.ColorType.PlayerGreenCall;
                case Config.ItemQuality.blue:
                    return Config.NameColorConfig.ColorType.PlayerBlueCall;
                case Config.ItemQuality.purple:
                    return Config.NameColorConfig.ColorType.PlayerPurpleCall;
                case Config.ItemQuality.Orange:
                    return Config.NameColorConfig.ColorType.PlayerOrangeCall;
                case Config.ItemQuality.red:
                    return Config.NameColorConfig.ColorType.PlayerRedCall;
                default:
                    return Config.NameColorConfig.ColorType.PlayerWhiteCall;
            }
        }
    }

    /// <summary>
    /// 模型简单调用，现在剧情用
    /// </summary>
    public class ModelUIHangPointHandler : UIHangPointHandleBase
    {

        public void Init(Transform titleRoot)
        {
            m_rootObject = new TransformWrap(titleRoot);

            LoadObject();
        }

        public void SetName(string name, Config.NameColorConfig.ColorType type = Config.NameColorConfig.ColorType.PlayerGreen)
        {
            if (m_object != null)
            {
                m_object.SetName(name, type);
            }
            else
            {
                Data.nickName = name;
                Data.nameColorType = type;
            }
        }

        public void SetTitle(string name, Config.NameColorConfig.ColorType type = Config.NameColorConfig.ColorType.PlayerGreen)
        {
            if (m_object != null)
            {
                m_object.SetTitle(name, type);
            }
            else
            {
                Data.titleName = name;
                Data.titleColorType = type;
            }
        }

        public void SetTaskSign(int sign)
        {
            if (m_object != null)
            {
                m_object.SetTaskSign(sign);
            }
            else
            {
                Data.taskSign = sign;
            }
        }

    }

    /// <summary>
    /// 挂点处理基类
    /// </summary>
    public class UIHangPointHandleBase
    {
        protected ModelUIHangPointObject m_object;// 挂点
        public ModelUIHangPointObject UIObject
        {
            get { return m_object; }
        }

        protected TransformWrap m_rootObject;// 挂点对象

        protected ModelUIHangPointObject.Cxt m_data;
        protected ModelUIHangPointObject.Cxt Data
        {
            get
            {
                if (m_data == null)
                    m_data = new ModelUIHangPointObject.Cxt();
                return m_data;
            }
            set { m_data = value; }
        }

        /// <summary>
        /// 销毁时候的对象
        /// </summary>
        public void Destory()
        {
            if (m_object != null)
            {
                GameObject.Destroy(m_object.gameObject);
                m_object = null;
            }
        }

        /// <summary>
        /// 设置随机冒泡
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="bubblingTime"></param>
        public void SetRandomBubbling(string[] contents, float bubblingTime, float intervalTime, bool immediate = true)
        {
            if (m_object != null)
                m_object.m_bubbling.SetRandomBubbling(contents, bubblingTime, intervalTime, immediate);
            else
            {
                Data.randomBubblingTexts = contents;
                Data.randomBubblingTime = bubblingTime;
                Data.randomIntervalTime = intervalTime;
            }
        }

        /// <summary>
        /// 直接冒泡
        /// </summary>
        /// <param name="content"></param>
        /// <param name="bubblingTime"></param>
        public void ShowBubbling(string content, float bubblingTime)
        {
            if (m_object != null)
                m_object.m_bubbling.ShowBubbling(content, bubblingTime);
            else
            {
                Data.bubblingText = content;
                Data.bublingTime = bubblingTime;
            }
        }

        public void ShowBubbling(string content, float bubblingTime, int taskSign)
        {
            if (m_object != null)
                m_object.m_bubbling.ShowBubbling(content, bubblingTime, taskSign);
            else
            {
                Data.bubblingText = content;
                Data.bublingTime = bubblingTime;
                Data.taskSign = taskSign;
            }
        }

        /// <summary>
        /// 加载挂点对象
        /// </summary>
        protected void LoadObject()
        {
            XYJObjectPool.LoadPrefab("HangPoint", LoadObjectEnd);
        }
        void LoadObjectEnd(GameObject go, object param)
        {
            m_object = go.GetComponent<ModelUIHangPointObject>();
            if (m_object == null)
            {
                Debuger.LogError("HangPoint没有ModelUIHangPointObject组件");
                return;
            }

            if (xys.App.my != null)
                xys.App.my.uiSystem.titleSystem.Bind(m_rootObject, go.GetComponent<RectTransform>(), Vector3.zero);

            m_object.ResetConfig();

            m_object.SetData(Data);
        }
    }

}

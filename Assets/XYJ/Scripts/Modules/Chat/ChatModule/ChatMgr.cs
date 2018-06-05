#if !USE_HOT
using System;
using Config;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProto;
using NetProto.Hot;
using Network;
using UnityEngine;
using UnityEngine.Assertions;
using wProtobuf;
using xys.hot.Team;
using xys.UI;

namespace xys.hot
{
    /// <summary>
    /// 1. TeamWorld和Team分别为未组队形态和组队形态，未组队时可接受TeamWorld频道的消息，组队时不可接受
    /// </summary>
    public class ChatMgr
    {
        #region Field
        // 聊天中的物品信息
        private readonly Dictionary<long, ItemData> itemDict = new Dictionary<long, ItemData>();
        // 聊天中的宠物信息
        private readonly Dictionary<long, PetsAttribute> petsDict = new Dictionary<long, PetsAttribute>();
        // 聊天保存的最大消息数量
        private readonly Dictionary<ChannelType, int> msgNum = new Dictionary<ChannelType, int>();
        // 当前保存的最大索引数量
        private readonly Dictionary<ChannelType, int> msgMaxIndex = new Dictionary<ChannelType, int>();

        // 聊天CD
        private readonly long[] m_times = new long[(int)ChannelType.Channel_Count];
        // 聊天记录
        private readonly Dictionary<ChannelType, Dictionary<int, ChatMsgRspone>> chatMsg = new Dictionary<ChannelType, Dictionary<int, ChatMsgRspone>>();
        // 系统记录
        private readonly Dictionary<int, string> systemMsg = new Dictionary<int, string>();
        // 主界面聊天消息
        private readonly Dictionary<int, ChatMsgRspone> mainMsg = new Dictionary<int, ChatMsgRspone>();
        // 聊天开始索引
        private readonly Dictionary<ChannelType, int> msgStartIndex = new Dictionary<ChannelType, int>();

        // 当前频道
        private ChannelType currentChannel = ChannelType.Channel_System;
        public ChannelType CurrentChannel
        {
            get { return currentChannel; }
            set
            {
                if(ChannelType.Channel_Private == value || ChannelType.Channel_Count == value || ChannelType.Channel_None == value)
                {
                    Log.Error("set channel error ! -> {0}", value);
                    return;
                }
                lastChannelType = currentChannel;
                currentChannel = value;
            }
        }
        // 上一个频道
        private ChannelType lastChannelType = ChannelType.Channel_System;
        public ChannelType LastChannelType
        {
            get { return lastChannelType; }
        }

        // 配置缓存
        private readonly Dictionary<int, ChatChannel> configs;

        // 物品缓存索引
        public int itemIndex = 0;
        // 宠物缓存索引
        public int petsIndex = 0;
        // 输入框的名字缓存
        private readonly Dictionary<string, long> nameCache = new Dictionary<string, long>();
        // 输入框的物品缓存
        private readonly Dictionary<string, KeyValuePair<string, ItemData>> itemCache = new Dictionary<string, KeyValuePair<string, ItemData>>();
        // 输入框的宠物缓存
        private readonly Dictionary<string, KeyValuePair<string, PetsAttribute>> petsCache = new Dictionary<string, KeyValuePair<string, PetsAttribute>>();

        // 接收信息时物品的临时ID数据列表
        private List<long> itemIdList;
        // 接收信息时宠物的临时ID数据列表
        private List<long> petsIdList;

        // 历史输入数据
        private readonly List<ChatDefins.History> historyList;
        // 历史输入列表最大数量
        public int historyNum { get; private set; }
        // 跑马灯消息队列
        public Queue<ChatMsgRspone> MarqueeQueue { get; private set; }
        #endregion

        #region Constructor
        public ChatMgr()
        {
            configs = ChatChannel.GetAll();

            for(var i = 1 ; i < (int)ChannelType.Channel_Count ; i++)
            {
                // 读取配置表中的消息最大消息数量
                msgNum.Add((ChannelType)i, configs[i].max);
                // 初始化
                msgMaxIndex.Add((ChannelType)i, 0);
                chatMsg.Add((ChannelType)i, new Dictionary<int, ChatMsgRspone>());
                msgStartIndex.Add((ChannelType)i, 0);
                // 主界面默认全部显示
                SetChannelCondition((ChannelType)i, true);
            }
            // 主界面默认全部显示
            SetChannelCondition(ChannelType.Channel_None, true);
            // 主界面作为none
            msgNum.Add(ChannelType.Channel_None, 999);
            msgMaxIndex.Add(ChannelType.Channel_None, 0);
            // 系统默认显示
            SetChannelCondition(ChannelType.Channel_System, true);
            // 不超过20条
            historyNum = Mathf.Min(kvClient.Get("ChatHistoryNum").value.ToInt32(), ChatDefins.ChatHistoryNum);
            // 聊天输入历史
            historyList = new List<ChatDefins.History>();

            MarqueeQueue = new Queue<ChatMsgRspone>();
            // 登陆信息

        }
        #endregion

        #region ADD
        public void AddMsg(ChannelType channel, ChatMsgRspone msg)
        {
            Assert.IsFalse(channel == ChannelType.Channel_System, "使用AddSystemMsg来添加系统消息");

            // 在队伍中时不接受世界队伍的信息
            if(msg.channel == ChannelType.Channel_GlobalTeam && TeamUtil.teamMgr.InTeam())
            {
                return;
            }

            // 缓存数量检查
            var count = msgMaxIndex[channel] - msgNum[channel];
            if(msgMaxIndex[channel] > msgNum[channel])
            {
                if(chatMsg[channel].ContainsKey(count))
                {
                    chatMsg[channel].Remove(count);
                }
            }

            // 装备ID缓存
            if(msg.itemIds.Count > 0)
            {
                itemIdList = msg.itemIds;
            }
            // 宠物
            if(msg.petsDatas.Count > 0)
            {
                petsIdList = msg.petsIds;
            }
            // 将临时列表ID数据转译
            msg.msg = ChatUtil.ReplaceLinkValue(msg.msg);

            AddCommonMsg(channel, msg);
            AddMainMsg(msg);
            // 当前场景说话冒泡
            if(channel == ChannelType.Channel_Zone || channel == ChannelType.Channel_Team
                || channel == ChannelType.Channel_Family || channel == ChannelType.Channel_Battle)
            {
                if(null == msg.fromUser || 0 == msg.fromUser.charid)
                {
                    return;
                }
                IObject roleObj = App.my.sceneMgr.GetObj(msg.charSceneId);
                if(null == roleObj)
                {
                    Debuger.Log("cannot find this role is scene !"
                        + "\nid:" + msg.fromUser.charid
                        + "\nname:" + msg.fromUser.name
                        + "\nscene:" + msg.charSceneId);
                    return;
                }
                roleObj.battle.actor.m_hangPoint.ShowBubbling(msg.msg, kvClient.GetFloat("chat_bubbing_time", 3f));
            }
        }

        // 根据ID新增一条本地系统消息
        public void AddSystemMsg(ChannelType channel, int id, params object[] args)
        {
            var str = string.Format(ChatInfo.Get(id).messageDes, args);
            var index = msgMaxIndex[channel];
            systemMsg.Add(index, str);
            ++index;
            msgMaxIndex[channel] = index;
            // 添加到主界面
            var newMainMsg = new ChatMsgRspone
            {
                msg = str,
                channel = channel,
            };
            hotApp.my.eventSet.FireEvent(EventID.ChatPanel_OnReceiveSystemMsg, newMainMsg);
            AddMainMsg(newMainMsg);
            // TODO 系统公告用id区分
            if(id > 1000000)
            {
                MarqueeQueue.Enqueue(newMainMsg);
                var marquee = UISystem.Get("UIMarqueePanel");
                if(marquee == null || !marquee.isVisible)
                {
                    App.my.uiSystem.ShowPanel("UIMarqueePanel");
                }
            }
        }
        // 新增一条系统信息
        public void AddSystemMsg(ChannelType channel, string msg)
        {
            var index = msgMaxIndex[channel];
            systemMsg.Add(index, msg);
            ++index;
            msgMaxIndex[channel] = index;
            // 添加到主界面
            var newMainMsg = new ChatMsgRspone
            {
                msg = msg,
                channel = channel,
            };
            hotApp.my.eventSet.FireEvent(EventID.ChatPanel_OnReceiveSystemMsg, newMainMsg);
            AddMainMsg(newMainMsg);
        }
        // 新增一条非好友聊天消息
        public void AddCommonMsg(ChannelType channel, ChatMsgRspone msg)
        {
            int index;
            // 英雄帖在世界频道显示
            if(channel == ChannelType.Channel_Hero)
            {
                index = msgMaxIndex[ChannelType.Channel_Global];
                chatMsg[ChannelType.Channel_Global].Add(index, msg);
                ++index;
                msgMaxIndex[ChannelType.Channel_Global] = index;
                // 添加给跑马灯使用
                MarqueeQueue.Enqueue(msg);
                var marquee = UISystem.Get("UIMarqueePanel");
                if(marquee == null || !marquee.isVisible)
                {
                    App.my.uiSystem.ShowPanel("UIMarqueePanel");
                }
            }
            // 世界队伍在队伍频道展示
            else if(channel == ChannelType.Channel_GlobalTeam)
            {
                index = msgMaxIndex[ChannelType.Channel_Team];
                chatMsg[ChannelType.Channel_Team].Add(index, msg);
                ++index;
                msgMaxIndex[ChannelType.Channel_Team] = index;
            }
            else
            {
                index = msgMaxIndex[msg.channel];
                chatMsg[channel].Add(index, msg);
                ++index;
                msgMaxIndex[msg.channel] = index;
            }

            hotApp.my.eventSet.fireEvent(EventID.ChatPanel_OnReceiveMsg);
        }
        // 新增一条主界面消息
        private void AddMainMsg(ChatMsgRspone msg)
        {
            if(GetChannelCondition(msg.channel))
            {
                // 增加主界面带发言人名字，修改聊天颜色
                var prefixName = new ChatMsgRspone();
                switch(msg.channel)
                {
                    case ChannelType.Channel_None:
                    case ChannelType.Channel_Private:
                    case ChannelType.Channel_Count:
                        break;
                    case ChannelType.Channel_Zone:
                        prefixName.msg = string.Format("#[{0}][{1}]#[{2}]{3}"
                            , kvClient.Get("mainchat_namecolor_zone").value
                            , msg.fromUser.name
                            , kvClient.Get("mainchat_color_zone").value
                            , msg.msg);
                        prefixName.channel = ChannelType.Channel_Zone;
                        break;
                    case ChannelType.Channel_Hero:
                    case ChannelType.Channel_Global:
                        prefixName.msg = string.Format("#[{0}][{1}]#[{2}]{3}"
                            , kvClient.Get("mainchat_namecolor_world").value
                            , msg.fromUser.name
                            , kvClient.Get("mainchat_color_world").value
                            , msg.msg);
                        prefixName.channel = ChannelType.Channel_Global;
                        break;
                    case ChannelType.Channel_Family:
                        prefixName.msg = string.Format("#[{0}][{1}]#[{2}]{3}"
                            , kvClient.Get("mainchat_namecolor_family").value
                            , msg.fromUser.name
                            , kvClient.Get("mainchat_color_family").value
                            , msg.msg);
                        prefixName.channel = ChannelType.Channel_Family;
                        break;
                    case ChannelType.Channel_GlobalTeam:
                    case ChannelType.Channel_Team:
                        prefixName.msg = string.Format("#[{0}][{1}]#[{2}]{3}"
                            , kvClient.Get("mainchat_namecolor_team").value
                            , msg.fromUser.name
                            , kvClient.Get("mainchat_color_team").value
                            , msg.msg);
                        prefixName.channel = ChannelType.Channel_GlobalTeam;
                        break;
                    case ChannelType.Channel_Battle:
                        prefixName.msg = string.Format("#[{0}][{1}]#[{2}]{3}"
                            , kvClient.Get("mainchat_namecolor_battle").value
                            , msg.fromUser.name
                            , kvClient.Get("mainchat_color_battle").value
                            , msg.msg);
                        prefixName.channel = ChannelType.Channel_Battle;
                        break;
                    case ChannelType.Channel_System:
                        prefixName.msg = string.Format("#[{0}]{1}"
                            , kvClient.Get("mainchat_color_system").value
                            , msg.msg);
                        prefixName.channel = ChannelType.Channel_System;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // None频道作为主界面频道使用
                var index = msgMaxIndex[ChannelType.Channel_None];
                mainMsg.Add(index, prefixName);
                ++index;
                msgMaxIndex[ChannelType.Channel_None] = index;
                // 刷新UI
                hotApp.my.eventSet.FireEvent(EventID.ChatMainPanel_RefreshMainChatMsg, msg.channel);
                // 是否有人@我
                var ss = ChatUtil.GetLinkValueByType(msg.msg, "0");
                foreach(var s in ss)
                {
                    if(long.Parse(s) == App.my.localPlayer.charid)
                    {
                        hotApp.my.eventSet.FireEvent(EventID.ChatMainPanel_Marker, new object[] { index - 1, msg.channel });
                    }
                }
            }
        }
        #endregion

        #region GET
        // 获取主界面消息
        public ChatMsgRspone GetMainMsg(int index)
        {
            return mainMsg[index];
        }
        // 根据索引获取频道消息
        public ChatMsgRspone GetCommonMsgWithIndex(ChannelType channel, int index)
        {
            var dict = chatMsg[channel];
            if(dict == null)
            {
                return null;
            }
            ChatMsgRspone result;
            dict.TryGetValue(index, out result);
            return result;
        }
        // 根据索引获取系统消息
        public string GetSystemMsgWithIndex(int index)
        {
            return systemMsg[index];
        }
        // 获取开始时的消息索引
        public int GetStartIndex(ChannelType channel)
        {
            return msgStartIndex[channel];
        }

        public void GetFriendMsg(long id)
        {

        }

        // 消息的最大索引
        public int GetMaxIndex(ChannelType channel)
        {
            return msgMaxIndex[channel];
        }
        // 可缓存的最大数量
        public int GetMaxNumber(ChannelType channel)
        {
            int result;
            return msgNum.TryGetValue(channel, out result) ? result : 0;
        }
        // 当前消息数量
        public int GetMsgNumber(ChannelType channel)
        {
            return chatMsg[channel].Count;
        }
        #endregion

        #region Set
        // 保存消息浏览的开始索引
        public void SetMsgStartIndex(ChannelType channel, int startIndex)
        {
            msgStartIndex[channel] = startIndex;
        }
        #endregion

        #region TIME
        public bool CheckChannelTime(ChannelType channel, long time)
        {
            return true;
            // TODO 配置和频道无法对应，需要修改
            return time - m_times[(int)channel] > configs[(int)channel].time * 10000000;
        }

        #endregion

        #region 频道展示设置
        public void SetChannelCondition(ChannelType channel, bool show)
        {
            LocalSave.SetBool(string.Format(ChatDefins.ChannelShowCondition, channel), show);
        }

        public bool GetChannelCondition(ChannelType channel)
        {
            return LocalSave.GetBool(string.Format(ChatDefins.ChannelShowCondition, channel));
        }

        public void SetVoiceCondition(ChannelType channel, bool show)
        {
            LocalSave.SetBool(string.Format(ChatDefins.ChannelVoiceCondition, channel), show);
        }

        public bool GetChannelVoiceCondition(ChannelType channel)
        {
            return LocalSave.GetBool(string.Format(ChatDefins.ChannelVoiceCondition, channel));
        }
        #endregion

        #region 输入缓存
        // 不能复位，否则历史输入无法读取相关信息...
        public void ClearCacheOnSendMsg()
        {
            nameCache.Clear();
            itemCache.Clear();
            itemIndex = 0;
            petsIndex = 0;
        }

        public bool AddNameCacheData(string name, long charId)
        {
            long id;
            if(!nameCache.TryGetValue(name, out id))
            {
                nameCache.Add(name, charId);
                return true;
            }
            return false;
        }

        public void AddItemCacheData(string index, KeyValuePair<string, ItemData> data)
        {
            itemCache.Add(index, data);
        }

        public void AddPetCacheData(string index, KeyValuePair<string, PetsAttribute> data)
        {
            petsCache.Add(index, data);
        }

        public long GetNameDataFromCache(string name)
        {
            long result = -1;
            nameCache.TryGetValue(name, out result);
            return result;
        }

        public KeyValuePair<string, ItemData> GetItemDataFromCache(string index)
        {
            KeyValuePair<string, ItemData> result;
            itemCache.TryGetValue(index, out result);
            return result;
        }

        public KeyValuePair<string, PetsAttribute> GetPetDataFromCache(string index)
        {
            KeyValuePair<string, PetsAttribute> result;
            petsCache.TryGetValue(index, out result);
            return result;
        }
        #endregion

        #region 输入历史
        public void AddHistory(ChatDefins.History msg)
        {
            if(historyNum == historyList.Count)
            {
                historyList.RemoveAt(0);
            }
            historyList.Add(msg);
            // 刷新历史列表
            hotApp.my.eventSet.fireEvent(EventID.ChatPanel_RefreshHistory);
        }

        public ChatDefins.History GetHistory(int index)
        {
            ChatDefins.History result = new ChatDefins.History();
            if(index >= 0 && index < historyList.Count)
                result = historyList[index];
            return result;
        }

        public int GetHistoryListCount()
        {
            return historyList.Count;
        }
        #endregion

        #region 物品相关
        // 添加一个包含服务器ID的物品信息
        public void AddItemData(long itemId, ItemData data)
        {
            ItemData item;
            if(!itemDict.TryGetValue(itemId, out item))
            {
                itemDict.Add(itemId, data);
            }
        }
        // 根据物品临时ID列表转译对应装备ID
        public long GetItemId(int index)
        {
            if(index >= itemIdList.Count || index < 0)
            {
                return 0;
            }
            return itemIdList[index];
        }
        // 根据物品服务器ID获取数据
        public ItemData GetItemData(long id)
        {
            ItemData result;
            return itemDict.TryGetValue(id, out result) ? result : null;
        }
        #endregion

        #region 宠物相关

        public void AddPetsData(long id, PetsAttribute data)
        {
            petsDict.Add(id, data);
        }

        public PetsAttribute GetPetsData(long id)
        {
            PetsAttribute data;
            return petsDict.TryGetValue(id, out data) ? data : null;
        }

        public long GetPetId(int index)
        {
            if(index >= petsIdList.Count || index < 0)
            {
                return 0;
            }
            return petsIdList[index];
        }
        #endregion

        #region 本地保存
        // TODO : 好友本地保存
        //private void AddFriendMsg(ChatMsgRspone msg)
        //{
        //    List<ChatScrollItemData> list;
        //    if(!m_friendsMsg.TryGetValue(msg.fromUser.charid, out list))
        //    {
        //        list = new List<ChatScrollItemData>();
        //    }
        //    list.Add(new ChatScrollItemData(msg));
        //    m_friendsMsg[msg.fromUser.charid] = list;
        //}
        #endregion
    }


}

#endif
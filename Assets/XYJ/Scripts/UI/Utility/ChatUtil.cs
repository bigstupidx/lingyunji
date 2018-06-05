#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Config;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using WXB;
using xys.hot.Team;
using xys.UI;
using ItemGrid = NetProto.ItemGrid;


namespace xys.hot
{
    public class ChatUtil
    {
        public static ChatMgr ChatMgr
        {
            get { return hotApp.my.GetModule<HotChatModule>().ChatMgr; }
        }

        #region @名字转超链接
        private const string MarkerNameStr = @"(?<Name>@\S+)";
        private static readonly Regex MarkerNameRegex = new Regex(MarkerNameStr);
        public static string ReplaceMarkerName(string input)
        {
            return MarkerNameRegex.Replace(input, OnMatchMakerName);
        }
        private static string OnMatchMakerName(Match match)
        {
            if(match.Groups["Name"].Success)
            {
                return string.Format("<hy t={0} fc={1} l={2} >"
                    , match.Groups["Name"].Value
                    , kvClient.Get("marker_color").value
                    , string.Format("0|{0}", hotApp.my.GetModule<HotChatModule>().ChatMgr.GetNameDataFromCache(match.Groups["Name"].Value)));
            }
            return match.Value;
        }
        #endregion

        #region 队伍邀请超链接

        private const string TeamRecruitStr = @"(?<Team>\<%T\>)";
        private static readonly Regex TeamRegex = new Regex(TeamRecruitStr);
        public static void SendTeamRecruit(ChannelType channel, string messageId, int teamId, params object[] args)
        {
            var msg = string.Format(ChatInfo.GetByName(messageId).messageDes, args);
            msg = TeamRegex.Replace(msg, _ =>
            {
                return OnMatchTeamData(_, teamId);
            });

            var req = new ChatMsgRequest
            {
                channel = channel,
                msg = msg,
            };

            hotApp.my.eventSet.FireEvent(EventID.ChatModule_OnSendMsg, req);
            // 组队邀请队伍成员也能看到
            if(channel == ChannelType.Channel_GlobalTeam)
            {
                var req2 = new ChatMsgRequest
                {
                    channel = ChannelType.Channel_Team,
                    msg = msg,
                };
                hotApp.my.eventSet.FireEvent(EventID.ChatModule_OnSendMsg, req2);
            }
        }

        private static string OnMatchTeamData(Match match, int teamId)
        {
            if(match.Groups["Team"].Success)
            {
                return string.Format("<hy t={0} fc={1} l={2} >"
                    , "[" + kvClient.Get("team_invitation_text").value + "]"
                    , kvClient.Get("team_invitation_color").value
                    , "3|" + teamId);
            }
            return match.Value;
        }
        #endregion

        #region 转译超链接中的参数
        // 超链接中的参数
        private const string LinkValue = @"(?<Value>(?<=\sl=)[\d\w\S]*)";
        private static readonly Regex LinkValueRegex = new Regex(LinkValue);
        // 获取超链接中的参数
        public static List<string> GetLinkValueByType(string input, string type)
        {
            var values = LinkValueRegex.Matches(input, 0);
            var result = new List<string>();
            for(int i = 0 ; i < values.Count ; i++)
            {
                var ss = values[i].Value.Split('|');
                // 类型对应索引
                if(type == ss[0])
                {
                    result.Add(ss[1]);
                }
            }
            return result;
        }
        // 接收消息时对超链接中的数据进行转译，根据临时数据索引获取相应的数据
        public static string ReplaceLinkValue(string input)
        {
            int itemIndex = 0, petIndex = 0;
            return LinkValueRegex.Replace(input, _ =>
            {
                return OnMatchItemIndexData(_, ref itemIndex, ref petIndex);
            });
        }

        // 接收消息时需要将超链接中的装备索引转译成指定的装备ID
        private static string OnMatchItemIndexData(Match match, ref int itemIndex, ref int petIndex)
        {
            if(match.Groups["Value"].Success)
            {
                string[] s = match.Groups["Value"].Value.Split('|');
                string result = string.Empty;
                switch(s[0])
                {
                    case "1":// 道具
                        result = string.Format("1|{0}", hotApp.my.GetModule<HotChatModule>().ChatMgr.GetItemId(itemIndex));
                        ++itemIndex;
                        break;
                    case "4":// 宠物
                        result = string.Format("4|{0}", hotApp.my.GetModule<HotChatModule>().ChatMgr.GetPetId(petIndex));
                        ++petIndex;
                        break;
                    default:
                        result = match.Value;
                        break;
                }
                return result;
            }
            return match.Value;
        }

        #endregion

        #region symboltext click
        public static void OnNodeClick(NodeBase node)
        {
            if(node is HyperlinkNode)
            {
                var n = (HyperlinkNode)node;
                var ss = n.d_link.Split('|');
                var type = ss[0];
                switch(type)
                {
                    case "0":// 名字
                        ShowRoleTips(long.Parse(ss[1]));
                        break;
                    case "1":// 物品
                        hotApp.my.eventSet.FireEvent(EventID.ChatModule_OnSearchItems, long.Parse(ss[1]));
                        break;
                    case "2":// 任务
                        SystemHintMgr.ShowHint("点击任务！");
                        break;
                    case "3":// 组队邀请
                        TeamUtil.teamMgr.ReqJoinTeam(int.Parse(ss[1]));
                        break;
                    case "4":// 宠物
                        hotApp.my.eventSet.FireEvent(EventID.ChatModule_OnSearchPets, long.Parse(ss[1]));
                        break;
                    default:
                        SystemHintMgr.ShowHint("未定义的超链接类型 ->" + type);
                        break;
                }
            }
        }

        public static void OnNodeClickMainPanel(NodeBase node)
        {
            if(node is HyperlinkNode)
            {
                OnNodeClick(node);
            }
            else
            {
                var p = UISystem.Get("UIChatPanel");
                if(null == p || !p.gameObject.activeSelf)
                {
                    App.my.uiSystem.ShowPanel("UIChatPanel", new object() { }, false); 
                }
            }
        }

        public static void OnEmptyClick()
        {
            var p = UISystem.Get("UIChatPanel");
            if(null == p || !p.gameObject.activeSelf)
            {
                App.my.uiSystem.ShowPanel("UIChatPanel", new object() { }, false);
            }
        }
        public static int GetEmotionNum()
        {
            int result = 0;
            // 未知数量，因接口原因遍历100次，之后可能会更改
            for(int i = 1 ; i < 100 ; i++)
            {
                var cartoon = CartoonLoad.Get(i.ToString());
                if(cartoon == null)
                {
                    result = i - 1;
                    break;
                }
            }
            return result;
        }

        public static void ShowItemTips(ItemData itemData)
        {
            var tipsData = new InitItemTipsData
            {
                type = InitItemTipsData.Type.CommonTips,
                itemData = new ItemGrid
                {
                    data = itemData
                }
            };

            App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, tipsData);
        }

        public static void ShowPetsTips(PetsAttribute petsData)
        {
            App.my.uiSystem.ShowPanel("UIPetsTipsPanel", petsData, false);
        }

        public static void ShowRoleTips(long charId)
        {
            UIRoleOperationData recently = new UIRoleOperationData(charId, RoleOperShowType.Custom)
            {
                panelPos = new Vector3(0, 105, 0)
            };
            App.my.uiSystem.ShowPanel(PanelType.UIRoleOperationPanel, recently);
        }
        // 根据配置读取
        public static ChatChannel GetChatChannelConfig(ChannelType channel)
        {
            ChatChannel cfg = null;
            switch(channel)
            {
                case ChannelType.Channel_None:
                case ChannelType.Channel_System:
                    cfg = ChatChannel.Get(1);
                    break;
                case ChannelType.Channel_Private:
                    cfg = ChatChannel.Get(2);
                    break;
                case ChannelType.Channel_Count:
                    break;
                case ChannelType.Channel_Zone:
                    cfg = ChatChannel.Get(3);
                    break;
                case ChannelType.Channel_Global:
                    cfg = ChatChannel.Get(4);
                    break;
                case ChannelType.Channel_Family:
                    cfg = ChatChannel.Get(5);
                    break;
                case ChannelType.Channel_Team:
                    cfg = ChatChannel.Get(6);
                    break;
                case ChannelType.Channel_Battle:
                    cfg = ChatChannel.Get(7);
                    break;
                case ChannelType.Channel_Hero:
                    cfg = ChatChannel.Get(8);
                    break;
                case ChannelType.Channel_GlobalTeam:
                    cfg = ChatChannel.Get(9);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("channel", channel, null);
            }
            return cfg;
        }
        // 频道消耗检查
        public static bool CheckChannelConsume(ChannelType channel)
        {
            var user = hotApp.my.localPlayer;
            var cfg = GetChatChannelConfig(channel);
            if(null == cfg)
            {
                Debuger.LogError(string.Format("config ->{0} is not exit!", channel.ToString().Color()));
                return false;
            }

            if(cfg.silver > 0)
            {
                if(user.silverShellValue < cfg.silver)
                {
                    SystemHintMgr.ShowTipsHint("chat_yinbei");
                    return false;
                }
            }

            if(cfg.Gold > 0)
            {
                if(user.goldShellValue < cfg.Gold)
                {
                    SystemHintMgr.ShowTipsHint("chat_jinbei");
                    return false;
                }
            }

            if(cfg.Fairy > 0)
            {
                if(user.fairyJadeValue < cfg.Fairy)
                {
                    SystemHintMgr.ShowTipsHint("chat_xianyu");
                    return false;
                }
            }

            if(cfg.Jasper > 0)
            {
                if(user.jasperJadeValue < cfg.Jasper)
                {
                    SystemHintMgr.ShowTipsHint("chat_biyu");
                    return false;
                }
            }

            if(cfg.level > 0)
            {
                if(user.levelValue < cfg.level)
                {
                    SystemHintMgr.ShowTipsHint("chat_level", cfg.level);
                    return false;
                }
            }

            if(cfg.GetConsumeItemId() > 0 && cfg.GetConsumeItemNum() > 0)
            {
                var packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
                if(packageMgr.GetItemCount(cfg.GetConsumeItemId()) < cfg.GetConsumeItemNum())
                {
                    SystemHintMgr.ShowTipsHint("chat_item", "[" + ItemBaseAll.Get(cfg.GetConsumeItemId()).name + "]");
                    return false;
                }
            }
            return true;
        }
        // 超链接转译
        public static KeyValuePair<string, bool> ReplaceCacheDataToHyperlink(int index, string str, List<ChatDefins.CellData> cellCache)
        {
            if(cellCache[index].type == ChatDefins.CellData.Type.Equip)
            {
                ItemQuality q;

                // 已装备的道具
                if(cellCache[index].isEquiped)
                {
                    var equipData = ( (EquipMgr)hotApp.my.localPlayer.GetModule<EquipModule>().equipMgr ).GetEquipData(cellCache[index].subType);
                    if(equipData == null || equipData.id != cellCache[index].itemData.id)
                    {
                        goto BREAK;
                    }
                    var config = EquipPrototype.Get(equipData.id);
                    q = config.quality;
                }
                else
                {
                    var itemData = hotApp.my.GetModule<HotPackageModule>().packageMgr.package.GetItemInfo(cellCache[index].pos);
                    if(itemData == null || itemData.count != cellCache[index].count || itemData.data.id != cellCache[index].itemData.id)
                    {
                        goto BREAK;
                    }
                    var config = EquipPrototype.Get(itemData.data.id);
                    q = config.quality;
                }

                str = string.Format("<hy t={0} fc={1} l={2} >"
                    , str
                    , Helper.GetItemColorByQuality(q)
                    , string.Format("1|{0}", index));
            }
            else if(cellCache[index].type == ChatDefins.CellData.Type.Item)
            {
                var itemData = hotApp.my.GetModule<HotPackageModule>().packageMgr.package.GetItemInfo(cellCache[index].pos);
                if(itemData == null || itemData.count != cellCache[index].count || itemData.data.id != cellCache[index].itemData.id)
                {
                    goto BREAK;
                }

                var config = Item.Get(itemData.data.id);

                str = string.Format("<hy t={0} fc={1} l={2} >"
                    , str
                    , Helper.GetItemColorByQuality(config.quality)
                    , string.Format("1|{0}", index));
            }
            else
            {
                var petData = hotApp.my.GetModule<HotPetsModule>().petsMgr.m_PetsTable.attribute[cellCache[index].index];
                if(petData == null || petData.id != cellCache[index].petsData.id)
                {
                    goto BREAK;
                }

                str = string.Format("<hy t={0} fc={1} l={2} >"
                    , str
                    , "#[G2]"
                    , string.Format("4|{0}", index));
            }
            return new KeyValuePair<string, bool>(str, true);
            BREAK:
            SystemHintMgr.ShowTipsHint("chat_send_changed");
            return new KeyValuePair<string, bool>("", false);
        }
    }
    #endregion


}
#endif
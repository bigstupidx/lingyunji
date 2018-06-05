#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using WXB;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class ChatMessageSimple
    {
        [SerializeField]
        private LayoutElement parentElement;
        [SerializeField]
        private SymbolText message;
        [SerializeField]
        private StateRoot iconStateRoot;//0:系统，1:当前，2:世界，3:氏族，4:队伍
        [SerializeField]
        private RectTransform bgImg;
        [SerializeField]
        private SymbolTextEvent textEvent;
        [SerializeField]
        private RectTransform textRectTransform;


        public void OnCellAdding(int index)
        {
            var msg = ChatUtil.ChatMgr.GetMainMsg(index);
            message.text = msg.msg;
            SetChannelIcon(msg.channel);

            // 修改text宽高
            textRectTransform.sizeDelta = new Vector2
                                        ( Mathf.Clamp(message.preferredWidth, ChatDefins.ChatSimpleWidthMin, ChatDefins.ChatSimpleWidthMax )
                                        , message.preferredHeight);
            // 修改cell高度
            parentElement.preferredHeight = bgImg.rect.height + ChatDefins.ChatSimpleSpacing;
            // 修改rect高度
            hotApp.my.eventSet.FireEvent(EventID.ChatMainPanel_RefreshMainChatHeight, parentElement.preferredHeight);
            // 刷新按钮位置
            hotApp.my.eventSet.FireEvent(EventID.ChatMainPanel_RefreshMainChatBtnHeight, parentElement.preferredHeight);
            // 新消息提示
            hotApp.my.eventSet.fireEvent(EventID.ChatMainPanel_NewMessageTip);

            textEvent.OnClickWithoutDragging.AddListenerIfNoExist(ChatUtil.OnNodeClickMainPanel);
            textEvent.OnClickEmpty.AddListenerIfNoExist(ChatUtil.OnEmptyClick);
        }

        private void SetChannelIcon(ChannelType channel)
        {
            switch(channel)
            {
                case ChannelType.Channel_None:
                case ChannelType.Channel_Private:
                case ChannelType.Channel_Count:
                    break;
                case ChannelType.Channel_System:
                    iconStateRoot.SetCurrentState("系统", false);
                    break;
                case ChannelType.Channel_Zone:
                    iconStateRoot.SetCurrentState("当前", false);
                    break;
                case ChannelType.Channel_Global:
                    iconStateRoot.SetCurrentState("世界", false);
                    break;
                case ChannelType.Channel_Family:
                    iconStateRoot.SetCurrentState("氏族", false);
                    break;
                case ChannelType.Channel_Team:
                    iconStateRoot.SetCurrentState("队伍", false);
                    break;
                case ChannelType.Channel_Battle:
                    iconStateRoot.SetCurrentState("战场", false);
                    break;
                case ChannelType.Channel_Hero:
                    iconStateRoot.SetCurrentState("世界", false);
                    break;
                case ChannelType.Channel_GlobalTeam:
                    iconStateRoot.SetCurrentState("队伍", false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("channel", channel, null);
            }
        }

    }
}

#endif
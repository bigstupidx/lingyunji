#if !USE_HOT

using System;
using System.Collections.Generic;
using System.Text;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using WXB;
using xys.UI;
using xys.UI.State;
#pragma warning disable 649


namespace xys.hot.UI
{
    [AutoILMono]
    class ChatMessageCommon
    {
        // 通用
        [SerializeField]
        private GameObject other;                               // 他人信息体物体
        [SerializeField]
        private GameObject mine;                                // 自己消息体物体
        [SerializeField]
        private GameObject system;                              // 系统消息体物体
        [SerializeField]
        private LayoutElement elementLayout;                    // 整个SrollView的子物体
        // 己方
        [SerializeField]
        private SymbolText playerNameMine;                      // 玩家名
        [SerializeField]
        private Text playerLevelMine;                           // 玩家等级
        [SerializeField]
        private StateRoot playerImgMine;                        // 玩家头像
        [SerializeField]
        private ButtonEx playerHeadBtnMine;                     // 玩家头像按钮
        [SerializeField]
        private StateRoot channelIconMine;                      // 频道按钮
        [SerializeField]
        private StateRoot textBgMine;                           // 消息背景
        [SerializeField]
        private SymbolTextEvent textEventMine;                  // node点击事件
        [SerializeField]
        private SymbolText textMine;                            // 信息体文本
        [SerializeField]
        private RectTransform textRtMine;                       // 文本高度适配
        [SerializeField]
        private LayoutElement textLayoutMine;                   // 文本element
        [SerializeField]
        private LayoutElement messageLayoutMine;                // 消息element
        [SerializeField]
        private LayoutElement chatInfoLayoutMine;               // 整体信息element
        [SerializeField]
        private RectTransform bgRectTransformMine;              // 消息背景图片
        // 他人
        [SerializeField]
        private SymbolText playerNameOther;                     // 玩家名
        [SerializeField]
        private Text playerLevelOther;                          // 玩家等级
        [SerializeField]
        private StateRoot playerImgOther;                       // 玩家头像
        [SerializeField]
        private ButtonEx playerHeadBtnOther;                    // 玩家头像按钮
        [SerializeField]
        private StateRoot channelIconOther;                     // 频道按钮
        [SerializeField]
        private StateRoot textBgOther;                          // 消息背景
        [SerializeField]
        private SymbolTextEvent textEventOther;                 // node点击事件
        [SerializeField]
        private SymbolText textOther;                           // 信息体文本
        [SerializeField]
        private RectTransform textRtOther;                      // 文本高度适配
        [SerializeField]
        private LayoutElement textLayoutOther;                  // 文本element
        [SerializeField]
        private LayoutElement messageLayoutOther;               // 消息element
        [SerializeField]
        private LayoutElement chatInfoLayoutOther;              // 整体信息element
        [SerializeField]
        private RectTransform bgRectTransformOther;             // 消息背景图片
        // 系统
        [SerializeField]
        private SymbolText textSystem;                          // 系统消息体
        [SerializeField]
        private RectTransform systemRt;                         // 系统消息文本高度适配
        [SerializeField]
        private StateRoot channelIconSystem;                    // 系统消息频道图标
        [SerializeField]
        private SymbolTextEvent systemTextEvent;                // 系统消息文本点击事件
        [SerializeField]
        private LayoutElement systemElement;                    // 系统消息体element
        // temp
        private long playerCharId = -1;                         // 此消息中发言玩家的ID
        private SymbolText playerNameStr;                       // 此消息玩家的姓名
        private StateRoot playerIcon;                           // 此消息玩家的头像
        private Text playerLevel;                               // 此消息玩家的等级
        private ButtonEx playerHeadBtn;                         // 玩家头像按钮
        private SymbolTextEvent textEvent;                      // node点击事件
        private int m_index;                                    // 此消息的索引
        private StateRoot channelIcon;                          // 此消息的频道按钮
        private StateRoot textBg;                               // 消息背景
        private SymbolText text;                                // 文本
        private RectTransform textRt;                           // 文本高度适配
        private LayoutElement textLayout;                       // 文本layout
        private LayoutElement messageLayout;                    // 消息layout
        private LayoutElement chatInfoLayout;                   // 包含名字频道的整体消息layout
        private RectTransform bgRt;                             // 消息背景图片
        public int GetIndex()
        {
            return m_index;
        }

        private void OnCellAdding(int index)
        {
            m_index = index;
            var msg = ChatUtil.ChatMgr.GetCommonMsgWithIndex(ChatUtil.ChatMgr.CurrentChannel, index);
            if(null == msg)
            {
                elementLayout.gameObject.SetActive(false);
                return;
            }
            var isSystemMsg = null == msg.fromUser || msg.fromUser.charid == 0;
            var isHeroChannel = ChannelType.Channel_Hero == msg.channel;

            if(isSystemMsg)
            {
                // 只显示频道中的系统消息
                mine.SetActive(false);
                other.SetActive(false);
                system.SetActive(true);

                text = textSystem;
                playerNameStr = null;
                playerIcon = null;
                playerLevel = null;
                playerHeadBtn = null;
                textEvent = null;
                channelIcon = channelIconSystem;
                textBg = null;
                textRt = systemRt;
                bgRt = null;
            }
            else
            {
                var isLocalPlayer = msg.fromUser.charid == App.my.localPlayer.charid;
                if(isLocalPlayer)
                {
                    mine.SetActive(true);
                    other.SetActive(false);
                    system.SetActive(false);

                    text = textMine;
                    playerNameStr = playerNameMine;
                    playerIcon = playerImgMine;
                    playerLevel = playerLevelMine;
                    playerHeadBtn = playerHeadBtnMine;
                    textEvent = textEventMine;
                    channelIcon = channelIconMine;
                    textBg = textBgMine;
                    textRt = textRtMine;
                    textLayout = textLayoutMine;
                    messageLayout = messageLayoutMine;
                    chatInfoLayout = chatInfoLayoutMine;
                    textEvent = textEventMine;
                    bgRt = bgRectTransformMine;
                }
                else
                {
                    other.SetActive(true);
                    mine.SetActive(false);
                    system.SetActive(false);

                    text = textOther;
                    playerNameStr = playerNameOther;
                    playerIcon = playerImgOther;
                    playerLevel = playerLevelOther;
                    playerHeadBtn = playerHeadBtnOther;
                    textEvent = textEventOther;
                    channelIcon = channelIconOther;
                    textBg = textBgOther;
                    textRt = textRtOther;
                    textLayout = textLayoutOther;
                    messageLayout = messageLayoutOther;
                    chatInfoLayout = chatInfoLayoutOther;
                    textEvent = textEventOther;
                    bgRt = bgRectTransformOther;
                }
            }

            // 频道图标适配
            SetChannelIcon(channelIcon, msg.channel);
            // 文本背景
            if(null != textBg)
            {
                textBg.SetCurrentState(isHeroChannel ? "英雄帖" : "普通", false);
            }
            // 玩家名
            if(null != playerNameStr)
            {
                playerNameStr.text = msg.fromUser.name;
            }
            // 玩家头像
            if(null != playerIcon)
            {
                SetPlayerIcon(playerIcon, msg.fromUser.sex, msg.fromUser.career);
            }
            // 玩家等级
            if(null != playerLevel)
            {
                playerLevel.text = msg.fromUser.level.ToString();
            }
            // 高度适配
            text.text = msg.msg;
            textRt.sizeDelta = new Vector2(Mathf.Min(text.preferredWidth,ChatDefins.ChatCommonTextWidthMax), text.preferredHeight);
            if(isSystemMsg)
            {
                elementLayout.preferredHeight = systemElement.preferredHeight = text.preferredHeight;
            }
            else
            {
                playerCharId = msg.fromUser.charid;
                textLayout.preferredHeight = text.preferredHeight;
                // 消息体高度适配
                var height = text.preferredHeight - text.minLineHeight;
                // TODO 语音导致高度变化
                var width = Mathf.Clamp(text.preferredWidth + ChatDefins.ChatCommonBgWidthFix, ChatDefins.ChatCommonBgWidthMin, ChatDefins.ChatCommonBgWidthMax);
                messageLayout.preferredHeight = Mathf.Max(ChatDefins.TextLayoutMinHeight + height, textLayout.preferredHeight);
                elementLayout.preferredHeight = chatInfoLayout.preferredHeight
                                              = Mathf.Max(ChatDefins.ChatInfoMinHeight + height, messageLayout.preferredHeight);
                bgRt.sizeDelta = new Vector2(width, Mathf.Max(ChatDefins.TextLayoutMinHeight + height, textLayout.preferredHeight));
            }
            // 文本按钮事件
            if(null != textEvent)
            {
                textEvent.OnClickWithoutDragging.AddListenerIfNoExist(ChatUtil.OnNodeClick);
            }
            // 头像点击事件
            if(null != playerHeadBtn)
            {
                playerHeadBtn.OnLongPress.AddListenerIfNoExist(OnPlayerHeadLongpress);
                playerHeadBtn.OnClick.AddListenerIfNoExist(OnPlayerHeadBtnClick);
            }

            #region old
            //// 各频道系统消息
            //if(null == msg.fromUser || msg.fromUser.charid == 0)
            //{
            //    common.SetActive(false);
            //    system.SetActive(true);
            //    SetChannelIcon(msg.channel);
            //    textSystem.text = msg.msg;
            //    element.preferredHeight = systemElement.preferredHeight = textSystem.preferredHeight;
            //    systemTextRectTransform.sizeDelta = new Vector2(systemTextRectTransform.sizeDelta.x, textSystem.preferredHeight);
            //    systemTextEvent.OnClick.AddListener(ChatUtil.OnNodeClick);
            //}
            //else
            //{
            //    // 缓存玩家相关信息在此element中
            //    playerNameStr = msg.fromUser.name;
            //    playerCharId = msg.fromUser.charid;

            //    system.SetActive(false);
            //    common.SetActive(true);

            //    // 消息体高度适配
            //    textCommon.text = msg.msg;
            //    textElement.preferredHeight = textCommon.preferredHeight;
            //    textRectTransform.sizeDelta = new Vector2(textRectTransform.sizeDelta.x, textCommon.preferredHeight);
            //    var height = textCommon.preferredHeight - textCommon.minLineHeight;
            //    // TODO 语音导致高度变化
            //    chatMsgElement.preferredHeight = Mathf.Max(ChatDefins.TextLayoutMinHeight + height, textElement.preferredHeight);
            //    element.preferredHeight = chatInfoElement.preferredHeight = Mathf.Max(ChatDefins.ChatInfoMinHeight + height,
            //        chatMsgElement.preferredHeight);

            //    // TODO 头像根据职业、性别进行变化
            //    if(msg.fromUser.sex == 1)
            //    {
            //        playerImg.SetSprite("TianJian_Nv");
            //    }
            //    else
            //    {
            //        playerImg.SetSprite("TianJian_Nan");
            //    }
            //    // 频道图标
            //    SetChannelIcon(msg.channel);
            //    // 是否本人发言、是否英雄发言
            //    var isLocalPlayer = msg.fromUser.charid == App.my.localPlayer.charid;
            //    var isHeroChannel = ChannelType.Channel_Hero == msg.channel;

            //    if(isLocalPlayer)
            //    {
            //        bg.SetCurrentState(isHeroChannel ? 3 : 1, false);
            //        // 己方位置调整
            //        message.SetAsFirstSibling();
            //        //textPosStateRoot.SetCurrentState("自己",false);
            //        textCommon.rectTransform.localPosition = new Vector2(-186, 20);
            //    }
            //    else
            //    {
            //        //textPosStateRoot.SetCurrentState("他人",false);
            //        textCommon.rectTransform.localPosition = new Vector2(-177, 20);
            //        bg.SetCurrentState(isHeroChannel ? 2 : 0, false);
            //    }
            //    // 上方频道、人名信息
            //    channelIconPos.SetCurrentState(isLocalPlayer ? 1 : 0, false);
            //    playerName.text = msg.fromUser.name;
            //    playerName.alignment = isLocalPlayer ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
            //} 
            #endregion

        }

        private void OnPlayerHeadBtnClick()
        {
            ChatUtil.ShowRoleTips(playerCharId);
        }

        private void OnPlayerHeadLongpress()
        {
            KeyValuePair<string, long> kvp = new KeyValuePair<string, long>(playerNameStr.text, playerCharId);
            hotApp.my.eventSet.FireEvent(EventID.ChatInput_OnReceiveUserData, kvp);
        }

        private void SetChannelIcon(StateRoot sr, ChannelType channel)
        {
            switch(channel)
            {
                case ChannelType.Channel_None:
                case ChannelType.Channel_Private:
                case ChannelType.Channel_Count:
                    break;
                case ChannelType.Channel_Zone:
                    sr.SetCurrentState("当前", false);
                    break;
                case ChannelType.Channel_Hero:
                case ChannelType.Channel_Global:
                    sr.SetCurrentState("世界", false);
                    break;
                case ChannelType.Channel_Family:
                    sr.SetCurrentState("氏族", false);
                    break;
                case ChannelType.Channel_GlobalTeam:
                case ChannelType.Channel_Team:
                    sr.SetCurrentState("队伍", false);
                    break;
                case ChannelType.Channel_Battle:
                    sr.SetCurrentState("战场", false);
                    break;
                case ChannelType.Channel_System:
                    sr.SetCurrentState("系统", false);
                    break;
                default:
                    Debuger.LogError("未知参数");
                    break;
            }
        }
        // TODO 头像完善
        private void SetPlayerIcon(StateRoot sr, int sex, int career)
        {
            if(sex == 1)
            {
                sr.SetCurrentState("天剑女", false);
            }
            else
            {
                sr.SetCurrentState("天剑男", false);
            }
        }
    }
}
#endif

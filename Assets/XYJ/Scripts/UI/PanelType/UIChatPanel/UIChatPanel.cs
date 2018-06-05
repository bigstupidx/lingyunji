#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FMOD;
using NetProto;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Event;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    internal class UIChatPanel : HotPanelBase
    {
        #region Field
        [SerializeField]
        private StateToggle toggle; // 默认系统，0：系统，1：世界，2：当前，3：氏族，4：队伍
        [SerializeField]
        private StateRoot lockRoot;

        [SerializeField]
        private Button heroBtn;
        [SerializeField]
        private Button settingBtn;
        [SerializeField]
        private Button closeBtn;

        [SerializeField]
        private UIChatPanelCommon common;
        [SerializeField]
        private UIChatPanelSystem system;

        [SerializeField]
        private RectTransform offset;
        [SerializeField]
        private Button unreadBtn;
        [SerializeField]
        private Text unreadText;

        [SerializeField]
        private StateRoot chatRectStateRoot;

        private HotObjectEventAgent eventAgent;
        private ChatMgr mgr;
        private int newMsgCount;
        // 定位
        private bool isMarkerShow;
        private int markerIndex;
        #endregion

        #region Impl
        public UIChatPanel() : base(null) { }

        public UIChatPanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            mgr = hotApp.my.GetModule<HotChatModule>().ChatMgr;
            // 默认选择世界频道
            mgr.CurrentChannel = ChannelType.Channel_Global;
            common.OnInit();
            system.OnInit();
        }

        protected override void OnShow(object p)
        {
            eventAgent = new HotObjectEventAgent(App.my.localPlayer.eventSet);
            eventAgent.Subscribe(EventID.ChatPanel_OnReceiveMsg, AddCommonMsg);
            eventAgent.Subscribe<ChatMsgRspone>(EventID.ChatPanel_OnReceiveSystemMsg, AddSystemMsg);
            eventAgent.Subscribe<string>(EventID.ChatPanel_OnChatPanelHangUpState, OnChatPanelRectChange);
            eventAgent.Subscribe<string>(EventID.ChatPanel_OnChatPanelCommonState, OnChatPanelRectChange);

            heroBtn.onClick.AddListenerIfNoExist(OnHeroBtnClick);
            settingBtn.onClick.AddListenerIfNoExist(OnSettingBtnClick);
            closeBtn.onClick.AddListenerIfNoExist(OnCloseBtnClick);
            unreadBtn.onClick.AddListenerIfNoExist(OnUnreadBtnClick);

            toggle.OnSelectChange = OnSelectChanege;
            lockRoot.onStateChange.AddListenerIfNoExist(OnLockStateChange);

            var objects = p as object[];
            if(objects != null)
            {
                // 不为空时有定位
                var objs = objects;
                OnShowChannel((int)objs[0], (ChannelType)objs[1]);
            }
            else
            {
                if(ChannelType.Channel_System != mgr.CurrentChannel)
                {
                    OnShowChannel(mgr.GetStartIndex(mgr.CurrentChannel), mgr.CurrentChannel);
                }
                else
                {
                    system.OnShow();
                }
            }
        }

        protected override void OnHide()
        {
            eventAgent.Release();
            eventAgent = null;

            heroBtn.onClick.RemoveListener(OnHeroBtnClick);
            settingBtn.onClick.RemoveListener(OnSettingBtnClick);
            closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            unreadBtn.onClick.RemoveListener(OnUnreadBtnClick);

            if(ChannelType.Channel_System != mgr.CurrentChannel)
            {
                common.OnHide();
            }
            else
            {
                system.OnHide();
            }
        }

        protected override void OnDestroy() { }

        #endregion

        private void AddCommonMsg()
        {
            common.AddData();
            if(common.IsLock())
            {
                newMsgCount++;
                unreadBtn.gameObject.SetActive(true);
                unreadText.text = string.Format(kvClient.Get("chat_unread").value, newMsgCount);
            }
        }
        private void AddSystemMsg(ChatMsgRspone msg)
        {
            if(msg.channel == ChannelType.Channel_System)
            {
                system.AddData();
            }
            else
            {
                common.AddData();
            }
        }

        private void OnCloseBtnClick()
        {
            App.my.uiSystem.HidePanel("UIChatPanel");
        }

        private void OnSettingBtnClick()
        {
            App.my.uiSystem.ShowPanel("UIChatSettingPanel", new object() { }, false);
        }

        private void OnHeroBtnClick()
        {
            App.my.uiSystem.ShowPanel("UIChatHeroPostPanel", new object() { }, false);
        }

        private void OnUnreadBtnClick()
        {
            common.SetBot();
            unreadBtn.gameObject.SetActive(false);
            newMsgCount = 0;
        }

        private void OnSelectChanege(StateRoot sr, int index)
        {
            switch(index)
            {
                case 0:
                    mgr.CurrentChannel = ChannelType.Channel_System;
                    common.OnHide();
                    system.OnShow();
                    break;
                case 1:
                    mgr.CurrentChannel = ChannelType.Channel_Global;
                    goto default;
                case 2:
                    mgr.CurrentChannel = ChannelType.Channel_Zone;
                    goto default;
                case 3:
                    mgr.CurrentChannel = ChannelType.Channel_Family;
                    goto default;
                case 4:
                    mgr.CurrentChannel = ChannelType.Channel_Team;
                    goto default;
                default:
                    if(ChannelType.Channel_System != mgr.LastChannelType)
                    {
                        // 是否是定位展示
                        if(isMarkerShow && markerIndex > -1)
                        {
                            common.Refresh(markerIndex);
                            isMarkerShow = false;
                            markerIndex = -1;
                        }
                        else
                        {
                            common.Refresh();
                        }
                    }
                    else
                    {
                        system.OnHide();
                        if(isMarkerShow && markerIndex > -1)
                        {
                            common.OnShow(markerIndex);
                            isMarkerShow = false;
                            markerIndex = -1;
                        }
                        else
                        {
                            common.OnShow();
                        }
                    }
                    break;
            }
        }
        // 定位时展示,使用count来简化代码
        // 已选中的无法触发onSelectChange
        private void OnShowChannel(int index, ChannelType channel)
        {
            isMarkerShow = true;
            markerIndex = index;
            switch(channel)
            {
                case ChannelType.Channel_None:
                case ChannelType.Channel_System:
                case ChannelType.Channel_Private:
                case ChannelType.Channel_Hero:
                    // 无定位break
                    break;
                case ChannelType.Channel_Battle:
                    break;
                case ChannelType.Channel_Global:
                    if(toggle.Select != 1)
                    {
                        toggle.Select = 1;
                        break;
                    }
                    else
                    {
                        goto case ChannelType.Channel_Count;
                    }
                case ChannelType.Channel_Zone:
                    if(toggle.Select != 2)
                    {
                        toggle.Select = 2;
                        break;
                    }
                    else
                    {
                        goto case ChannelType.Channel_Count;
                    }
                case ChannelType.Channel_Family:
                    if(toggle.Select != 3)
                    {
                        toggle.Select = 3;
                        break;
                    }
                    else
                    {
                        goto case ChannelType.Channel_Count;
                    }
                case ChannelType.Channel_GlobalTeam:
                case ChannelType.Channel_Team:
                    if(toggle.Select != 4)
                    {
                        toggle.Select = 4;
                        break;
                    }
                    else
                    {
                        goto case ChannelType.Channel_Count;
                    }
                case ChannelType.Channel_Count:
                    if(ChannelType.Channel_System != mgr.LastChannelType)
                    {
                        common.Refresh(markerIndex);
                        isMarkerShow = false;
                        markerIndex = -1;
                    }
                    else
                    {
                        system.OnHide();
                        common.OnShow(markerIndex);
                        isMarkerShow = false;
                        markerIndex = -1;
                    }
                    break;
            }
        }

        private void OnChatPanelRectChange(string state)
        {
            chatRectStateRoot.SetCurrentState(state, false);
        }

        private void OnLockStateChange()
        {
            // 未锁定
            if(0 == lockRoot.CurrentState)
            {
                common.SetLock(false);
                system.SetLock(false);
            }
            else
            {
                common.SetLock(true);
                system.SetLock(true);
            }
        }


    }
}

#endif
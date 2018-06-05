#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIChatSettingPanel : HotPanelBase
    {
        #region Filed
        [SerializeField]
        private StateRoot all;
        [SerializeField]
        private StateRoot zone;
        [SerializeField]
        private StateRoot global;
        [SerializeField]
        private StateRoot team;
        [SerializeField]
        private StateRoot battle;
        [SerializeField]
        private StateRoot family;

        [SerializeField]
        private StateRoot channelBtn;
        [SerializeField]
        private StateRoot voiceBtn;
        [SerializeField]
        private StateToggle toggle;
        [SerializeField]
        private Button closeBtn;

        private ChatMgr mgr;
        #endregion        

        #region Impl
        public UIChatSettingPanel() : base(null) { }
        public UIChatSettingPanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            mgr = hotApp.my.GetModule<HotChatModule>().ChatMgr;
        }

        protected override void OnShow(object p)
        {
            toggle.OnSelectChange += OnSelectChange;
            all.onStateChange.AddListenerIfNoExist(OnStateChange);
            if(toggle.Select == 0)
            {
                GetChannelCondition();
            }
            else
            {
                GetVoiceCondition();
            }
            closeBtn.onClick.AddListenerIfNoExist(OnCloseBtnClick);
            zone.onClick.AddListenerIfNoExist(OnZoneBtnClick);
            global.onClick.AddListenerIfNoExist(OnGlobalBtnClick);
            team.onClick.AddListenerIfNoExist(OnTeamBtnClick);
            battle.onClick.AddListenerIfNoExist(OnBattleBtnClick);
            family.onClick.AddListenerIfNoExist(OnFamilyClick);
        }

        protected override void OnHide()
        {
            if(toggle.Select == 0)
            {
                SaveChannelCondition();
            }
            else
            {
                SaveVoiceCondition();
            }
            toggle.OnSelectChange -= OnSelectChange;
            all.onStateChange.RemoveListener(OnStateChange);
            closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            zone.onClick.RemoveListener(OnZoneBtnClick);
            global.onClick.RemoveListener(OnGlobalBtnClick);
            team.onClick.RemoveListener(OnTeamBtnClick);
            battle.onClick.RemoveListener(OnBattleBtnClick);
            family.onClick.RemoveListener(OnFamilyClick);
        }

        #endregion

        #region Event
        private void OnCloseBtnClick()
        {
            parent.gameObject.SetActive(false);
        }
        private void OnStateChange()
        {
            if(all.CurrentState == 1)
            {
                zone.SetCurrentState(1, false);
                family.SetCurrentState(1, false);
                global.SetCurrentState(1, false);
                team.SetCurrentState(1, false);
                battle.SetCurrentState(1, false);
            }
        }
        private void OnSelectChange(StateRoot root, int index)
        {
            if(root == channelBtn)
            {
                SaveVoiceCondition();
                GetChannelCondition();
            }
            else
            {
                SaveChannelCondition();
                GetVoiceCondition();
            }
        }

        private void OnZoneBtnClick()
        {
            if (zone.CurrentState == 0)
            {
                all.CurrentState = 0;
            }
        }

        private void OnGlobalBtnClick()
        {
            if (global.CurrentState == 0)
            {
                all.CurrentState = 0;
            }
        }

        private void OnTeamBtnClick()
        {
            if (team.CurrentState == 0)
            {
                all.CurrentState = 0;
            }
        }

        private void OnBattleBtnClick()
        {
            if (battle.CurrentState == 0)
            {
                all.CurrentState = 0;
            }
        }

        private void OnFamilyClick()
        {
            if (family.CurrentState == 0)
            {
                all.CurrentState = 0;
            }
        }
        #endregion

        #region Local
        private void SaveChannelCondition()
        {
            mgr.SetChannelCondition(ChannelType.Channel_Zone, zone.CurrentState == 1);
            mgr.SetChannelCondition(ChannelType.Channel_Global, global.CurrentState == 1);
            mgr.SetChannelCondition(ChannelType.Channel_Battle, battle.CurrentState == 1);
            mgr.SetChannelCondition(ChannelType.Channel_Family, family.CurrentState == 1);
            mgr.SetChannelCondition(ChannelType.Channel_Team, team.CurrentState == 1);
            mgr.SetChannelCondition(ChannelType.Channel_None, all.CurrentState == 1);
        }

        private void SaveVoiceCondition()
        {
            mgr.SetVoiceCondition(ChannelType.Channel_Zone, zone.CurrentState == 1);
            mgr.SetVoiceCondition(ChannelType.Channel_Global, global.CurrentState == 1);
            mgr.SetVoiceCondition(ChannelType.Channel_Battle, battle.CurrentState == 1);
            mgr.SetVoiceCondition(ChannelType.Channel_Family, family.CurrentState == 1);
            mgr.SetVoiceCondition(ChannelType.Channel_Team, team.CurrentState == 1);
            mgr.SetVoiceCondition(ChannelType.Channel_None, all.CurrentState == 1);
        }

        private void GetChannelCondition()
        {
            all.CurrentState = mgr.GetChannelCondition(ChannelType.Channel_None) ? 1 : 0;
            zone.CurrentState = mgr.GetChannelCondition(ChannelType.Channel_Zone) ? 1 : 0;
            global.CurrentState = mgr.GetChannelCondition(ChannelType.Channel_Global) ? 1 : 0;
            battle.CurrentState = mgr.GetChannelCondition(ChannelType.Channel_Battle) ? 1 : 0;
            family.CurrentState = mgr.GetChannelCondition(ChannelType.Channel_Family) ? 1 : 0;
            team.CurrentState = mgr.GetChannelCondition(ChannelType.Channel_Team) ? 1 : 0;
        }

        private void GetVoiceCondition()
        {
            all.CurrentState = mgr.GetChannelVoiceCondition(ChannelType.Channel_None) ? 1 : 0;
            zone.CurrentState = mgr.GetChannelVoiceCondition(ChannelType.Channel_Zone) ? 1 : 0;
            global.CurrentState = mgr.GetChannelVoiceCondition(ChannelType.Channel_Global) ? 1 : 0;
            battle.CurrentState = mgr.GetChannelVoiceCondition(ChannelType.Channel_Battle) ? 1 : 0;
            family.CurrentState = mgr.GetChannelVoiceCondition(ChannelType.Channel_Family) ? 1 : 0;
            team.CurrentState = mgr.GetChannelVoiceCondition(ChannelType.Channel_Team) ? 1 : 0;
        }
        #endregion
    }
}

#endif
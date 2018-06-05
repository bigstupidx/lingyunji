#if !USE_HOT
using System;
using System.Text;
using Config;
using NetProto;
using UnityEngine;
using UnityEngine.UI;
using WXB;
using xys.hot.Event;
using xys.hot.Team;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [Serializable]
    internal class UIMainChatIcons
    {
        [SerializeField]
        private RectTransform rect;
        [SerializeField]
        private Button settingBtn;
        [SerializeField]
        private Button viewBtn;
        [SerializeField]
        private Button hangUpBtn;
        [SerializeField]
        private Button bannedBtn;
        [SerializeField]
        private Button interactionBtn;
        [SerializeField]
        private ButtonEx chatBtn;
        [SerializeField]
        private StateRoot chatTextStateRoot;//0:世界，1:队伍，2:氏族
        [SerializeField]
        private StateRoot chatBtnStateRoot;
        [SerializeField]
        private SymbolText chatBtnText;
        [SerializeField]
        private StateRoot expandState; // 0:已扩展,1:待扩展
        [SerializeField]
        [Tooltip("初始化时高度")]
        private float rectInitPosition = -73f;
        [SerializeField]
        [Tooltip("点击扩展改变的高度")]
        private float expandChangePosition = 95f;
        [SerializeField]
        [Tooltip("扩展后的最大高度")]
        private float expandedMaxPosition = 160f;
        [SerializeField]
        [Tooltip("未扩展的最大高度")]
        private float noExpandMaxPosition = 75f;

        private HotObjectEventAgent eventAgent;
        private int timerId;
        private bool isCancel;
        public void OnInit()
        {
            // 无消息时位置
            rect.localPosition = new Vector2(rect.localPosition.x, rectInitPosition);
        }

        public void OnShow()
        {
            settingBtn.onClick.AddListenerIfNoExist(OnSettingBtnClick);
            viewBtn.onClick.AddListenerIfNoExist(OnViewBtnClick);
            hangUpBtn.onClick.AddListenerIfNoExist(OnHangUpBtnClick);
            bannedBtn.onClick.AddListenerIfNoExist(OnBannedBtnClick);
            interactionBtn.onClick.AddListenerIfNoExist(OnInteractionBtnClick);
            expandState.onClick.AddListenerIfNoExist(OnExpandBtnClick);

            chatBtn.OnClick.AddListenerIfNoExist(OnChatBtnClick);
            chatBtn.OnLongPress.AddListenerIfNoExist(OnChatBtnPress);
            chatBtn.longPressInterval = ChatDefins.ChatVoiceLongPressInterval;
            chatBtn.OnPointEnter.AddListenerIfNoExist(OnPointEnter);
            chatBtn.OnPointExit.AddListenerIfNoExist(OnPointExit);
            chatBtn.OnPointDown.AddListenerIfNoExist(OnPointDown);
            chatBtn.OnPointUpExitBounds.AddListenerIfNoExist(OnPointUpExitBounds);
            chatBtn.OnPointUpInBounds.AddListenerIfNoExist(OnPointUpInBounds);

            eventAgent = new HotObjectEventAgent(App.my.localPlayer.eventSet);
            eventAgent.Subscribe<float>(EventID.ChatMainPanel_RefreshMainChatBtnHeight, ChangeLocalPosition);
        }

        public void OnHide()
        {
            settingBtn.onClick.RemoveListener(OnSettingBtnClick);
            viewBtn.onClick.RemoveListener(OnViewBtnClick);
            hangUpBtn.onClick.RemoveListener(OnHangUpBtnClick);
            bannedBtn.onClick.RemoveListener(OnBannedBtnClick);
            interactionBtn.onClick.RemoveListener(OnInteractionBtnClick);
            expandState.onClick.RemoveListener(OnExpandBtnClick);

            chatBtn.OnClick.RemoveListener(OnChatBtnClick);
            chatBtn.OnLongPress.RemoveListener(OnChatBtnPress);
            chatBtn.OnPointEnter.RemoveListener(OnPointEnter);
            chatBtn.OnPointExit.RemoveListener(OnPointExit);
            chatBtn.OnPointDown.RemoveListener(OnPointDown);
            chatBtn.OnPointUpInBounds.RemoveListener(OnPointUpInBounds);
            chatBtn.OnPointUpExitBounds.RemoveListener(OnPointUpExitBounds);

            eventAgent.Release();
            eventAgent = null;
        }

        public void ChangeLocalPosition(float height)
        {
            if(expandState.CurrentState == 1)
            {
                rect.localPosition = rect.localPosition.y + height > noExpandMaxPosition
                    ? new Vector3(rect.localPosition.x, noExpandMaxPosition)
                    : new Vector3(rect.localPosition.x, rect.localPosition.y + height);
            }
            else
            {
                rect.localPosition = rect.localPosition.y + height > expandedMaxPosition
                    ? new Vector3(rect.localPosition.x, expandedMaxPosition)
                    : new Vector3(rect.localPosition.x, rect.localPosition.y + height);
            }
        }

        #region Event
        #region Voice
        private void OnChatBtnClick()
        {
            //chatTextStateRoot.SetNextStateWithLoop();
            // 世界
            if(0 == chatTextStateRoot.CurrentState)
            {
                if(!TeamUtil.teamMgr.InTeam())
                {
                    if(hotApp.my.localPlayer.clanIdValue > 0)
                    {
                        chatTextStateRoot.SetCurrentState("氏族", false);
                        chatBtnText.text = string.Format("#[{0}]{1}", kvClient.Get("mainchat_color_family").value, chatBtnText.text); 
                    }
                }
                else
                {
                    chatTextStateRoot.SetCurrentState("队伍", false);
                    chatBtnText.text = string.Format("#[{0}]{1}", kvClient.Get("mainchat_color_team").value, chatBtnText.text);
                }
            }
            // 队伍
            else if(1 == chatTextStateRoot.CurrentState)
            {
                chatTextStateRoot.SetCurrentState("氏族", false);
                chatBtnText.text = string.Format("#[{0}]{1}", kvClient.Get("mainchat_color_family").value, chatBtnText.text);
            }
            // 氏族
            else if(2 == chatTextStateRoot.CurrentState)
            {
                chatTextStateRoot.SetCurrentState("世界", false);
                chatBtnText.text = string.Format("#[{0}]{1}", kvClient.Get("mainchat_color_world").value, chatBtnText.text);
            }
            return;
            #region Test
            string s = DateTime.UtcNow.ToString("HH-mm-ss");
            var strArr = s.Split('-');

            var sb = new StringBuilder();
            var count = UnityEngine.Random.Range(1, 5);
            sb.AppendFormat("{0}时{1}分{2}秒", strArr[0], strArr[1], strArr[2]);
            for(int i = 0 ; i < count ; i++)
            {
                sb.Append(" ->test! ");
            }
            sb.Append(string.Format("<hy t=自己测试 l={0} fs=25 fc=#R fhc=Y ul=1>", string.Format("{0}|{1}", 0, App.my.localPlayer.charid)));
            var q = new ChatMsgRequest
            {
                msg = sb.ToString(),
                channel = ChannelType.Channel_Zone
            };
            hotApp.my.eventSet.FireEvent(EventID.ChatModule_OnSendMsg, q);
            Debug.Log("send message q ->" + q.msg.Color());
            #endregion
        }

        private void OnChatBtnPress()
        {
            // 超时注册
            timerId = hotApp.my.mainTimer.Register(ChatDefins.MaxVoiceDuratiuon, 1, () =>
            {
                SystemHintMgr.ShowTipsHint("chat_voice_length_toolong");
                VoiceMisc.Stop();
            });

            App.my.uiSystem.ShowPanel("UIChatVoiceTipsPanel");
            VoiceMisc.Start(VoiceCallBack);
        }

        private void OnPointEnter()
        {
            hotApp.my.eventSet.fireEvent(EventID.ChatVoiceTips_Misc);
        }

        private void OnPointExit()
        {
            hotApp.my.eventSet.fireEvent(EventID.ChatVoiceTips_Cancle);
        }

        private void OnPointUpInBounds()
        {
            chatBtnStateRoot.SetCurrentState("常态", false);
            VoiceMisc.Stop();
        }

        private void OnPointUpExitBounds()
        {
            chatBtnStateRoot.SetCurrentState("常态", false);
            VoiceMisc.Stop();
            isCancel = true;
        }

        private void OnPointDown()
        {
            chatBtnStateRoot.SetCurrentState("缩小", false);
        }

        private void VoiceCallBack(VoiceMisc.ResultData data)
        {
            App.my.uiSystem.HidePanel("UIChatVoiceTipsPanel");
            hotApp.my.mainTimer.Cannel(timerId);

            if(!string.IsNullOrEmpty(data.error))
            {
                Debuger.Log(string.Format("voiceo error -> {0}", data.error));
                return;
            }

            if(data.lenght < ChatDefins.MinVoiceDuration)
            {
                SystemHintMgr.ShowTipsHint("chat_voice_length_tooshort");
                return;
            }

            if(!isCancel)
            {
                var msg = TextRegexParser.FilterSensitiveWord(data.text);
                var channel = ChannelType.Channel_Global;
                if(chatTextStateRoot.CurrentStateName == "队伍")
                {
                    if(!TeamUtil.teamMgr.InTeam())
                    {
                        // 频道不对无法发送
                        SystemHintMgr.ShowTipsHint(7003);
                        isCancel = false;
                        return;
                    }
                    channel = ChannelType.Channel_Team;
                }
                if(chatTextStateRoot.CurrentStateName == "氏族")
                {
                    channel = ChannelType.Channel_Family;

                }
                var q = new ChatMsgRequest
                {
                    channel = channel,
                    msg = msg
                };
                hotApp.my.eventSet.FireEvent(EventID.ChatModule_OnSendMsg, q);
                SystemHintMgr.ShowTipsHint("chat_send_success");
            }
            else
            {
                SystemHintMgr.ShowTipsHint("chat_cancel");
            }
            isCancel = false;
        }
        #endregion

        private static void OnSettingBtnClick()
        {
            var p = UISystem.Get("UIChatSettingPanel");
            if(null != p && p.isVisible)
            {
                App.my.uiSystem.HidePanel("UIChatSettingPanel");
            }
            else
            {
                App.my.uiSystem.ShowPanel("UIChatSettingPanel", new object() { }, false);
            }
        }
        private void OnExpandBtnClick()
        {
            // 点击时先改变stateroot的状态，然后再触发onclick
            if(0 == expandState.CurrentState)
            {
                rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y + expandChangePosition);
                LocalSave.SetBool(ChatDefins.Expand, true);
                eventAgent.FireEvent(EventID.ChatMainPanel_ExpandBtnClick, expandChangePosition);
            }
            else
            {
                rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y - expandChangePosition);
                LocalSave.SetBool(ChatDefins.Expand, false);
                eventAgent.FireEvent(EventID.ChatMainPanel_ExpandBtnClick, -expandChangePosition);
            }
        }

        private static void OnViewBtnClick()
        {
            Debuger.Log("click view button");
        }

        private static void OnHangUpBtnClick()
        {
            Debuger.Log("click hangup button");
        }

        private static void OnBannedBtnClick()
        {
            Debuger.Log("click banned button");
        }

        private static void OnInteractionBtnClick()
        {
            Debuger.Log("click interaction button");
        }

        #endregion
    }
}

#endif
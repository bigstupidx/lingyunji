#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using xys.hot.Event;
using xys.UI;
using xys.UI.State;
using xys.UI.Utility;

namespace xys.hot.UI
{
    [Serializable]
    class UIChatInput
    {
        #region Field
        [SerializeField]
        private Button sendBtn;
        [SerializeField]
        private ButtonEx voiceBtn;
        [SerializeField]
        private StateRoot voiceStateRoot;
        [SerializeField]
        private Button emotionBtn;
        [SerializeField]
        private InputFieldEx input;
        [SerializeField]
        private GameObject inputGo;

        private ChatMgr mgr;
        private HotObjectEventAgent eventAgent;
        private bool isCancel;
        private int timerId;
        private int limitNum;
        private List<ChatDefins.CellData> cellCache;
        private List<ChatDefins.DataNode> nodeCache;
        #endregion

        #region Impl
        public void OnShow()
        {
            limitNum = kvClient.GetInt("chat_input_num", 50);
            input.text = "";
            mgr = hotApp.my.GetModule<HotChatModule>().ChatMgr;

            eventAgent = new HotObjectEventAgent(App.my.localPlayer.eventSet);
            eventAgent.Subscribe<KeyValuePair<string, long>>(EventID.ChatInput_OnReceiveUserData, OnReceiveUserData);
            eventAgent.Subscribe<ChatDefins.CellData>(EventID.ChatInput_OnReceiveItemData, OnReceiveItemData);
            eventAgent.Subscribe<ChatDefins.CellData>(EventID.ChatInput_OnReceivePetsData, OnReceivePetsData);
            eventAgent.Subscribe<string>(EventID.ChatInput_OnReceiveFaceData, OnReceiveFaceData);
            eventAgent.Subscribe<string>(EventID.ChatInput_OnReceiveInputSimple, OnReceiveSimpleInput);
            eventAgent.Subscribe<ChatDefins.History>(EventID.ChatInput_OnReceiveInputHistory, OnReceiveHistory);

            inputGo.SetActive(true);
            sendBtn.onClick.AddListenerIfNoExist(OnSendBtnClick);
            input.onValueChanged.AddListenerIfNoExist(OnValueChange);
            input.onValidateInput = OnValidateInput;
            emotionBtn.onClick.AddListenerIfNoExist(OnEmojiClick);
            voiceBtn.OnLongPress.AddListenerIfNoExist(OnVoiceBtnPress);
            voiceBtn.OnPointEnter.AddListenerIfNoExist(OnFingerEnter);
            voiceBtn.OnPointExit.AddListenerIfNoExist(OnFingerExit);
            voiceBtn.OnPointUpExitBounds.AddListenerIfNoExist(OnFingerUpOutside);
            voiceBtn.OnPointUpInBounds.AddListenerIfNoExist(OnFingerUp);
            voiceBtn.OnPointDown.AddListenerIfNoExist(OnPointDown);
            nodeCache = new List<ChatDefins.DataNode>();
            cellCache = new List<ChatDefins.CellData>();
        }

        public void OnHide()
        {
            if(null != nodeCache)
            {
                nodeCache.Clear();
            }
            if(cellCache != null)
            {
                cellCache.Clear();
            }
            nodeCache = null;
            cellCache = null;
            sendBtn.onClick.RemoveListener(OnSendBtnClick);
            input.onValueChanged.RemoveListener(OnValueChange);
            emotionBtn.onClick.RemoveListener(OnEmojiClick);
            voiceBtn.OnLongPress.RemoveListener(OnVoiceBtnPress);
            voiceBtn.OnPointEnter.RemoveListener(OnFingerEnter);
            voiceBtn.OnPointExit.RemoveListener(OnFingerExit);
            voiceBtn.OnPointUpInBounds.RemoveListener(OnFingerUp);
            voiceBtn.OnPointUpExitBounds.RemoveListener(OnFingerUpOutside);
            voiceBtn.OnPointDown.RemoveListener(OnPointDown);

            if(null != eventAgent)
            {
                eventAgent.Release();
                eventAgent = null;
            }
            inputGo.SetActive(false);
        }

        #endregion

        #region Event

        #region Voice
        private void OnFingerUp()
        {
            voiceStateRoot.SetCurrentState("常态", false);
            VoiceMisc.Stop();
        }
        private void OnFingerUpOutside()
        {
            voiceStateRoot.SetCurrentState("常态", false);
            VoiceMisc.Stop();
            isCancel = true;
        }
        private void OnFingerExit()
        {
            hotApp.my.eventSet.fireEvent(EventID.ChatVoiceTips_Cancle);
        }

        private void OnFingerEnter()
        {
            hotApp.my.eventSet.fireEvent(EventID.ChatVoiceTips_Misc);
        }
        private void OnPointDown()
        {
            voiceStateRoot.SetCurrentState("缩小", false);
        }
        private void OnVoiceBtnPress()
        {
            // 超时注册
            timerId = hotApp.my.mainTimer.Register(ChatDefins.MaxVoiceDuratiuon, 1, () =>
            {
                SystemHintMgr.ShowTipsHint("chat_voice_length_toolong");
                VoiceMisc.Stop();
            });

            App.my.uiSystem.ShowPanel("UIChatVoiceTipsPanel");

            VoiceMisc.Start(VoiceCallback);
        }

        private void VoiceCallback(VoiceMisc.ResultData data)
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
                var q = new ChatMsgRequest
                {
                    channel = mgr.CurrentChannel,
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

        #region InputField
        private char OnValidateInput(string text, int charindex, char addedchar)
        {
            // 确保不是删除操作
            isDelete = false;
            return addedchar;
        }

        private bool isDelete;
        private void OnValueChange(string str)
        {
            if(input.LastInputText.Length == 0)
            {
                isDelete = true;
                return;
            }

            // 新增字符数量
            var charCount = str.Length - input.LastInputText.Length +
                            Mathf.Abs(input.LastCaretPostion - input.LastSelectionAnchorPosition);
            // 单纯删除操作
            if(isDelete)
            {
                // 当前caretPos为删除的charPos
                for(int i = 0 ; i < nodeCache.Count ; i++)
                {
                    nodeCache[i].Delete(input.LastSelectionAnchorPosition, input.LastCaretPostion, limitNum);
                }
            }
            else
            {
                // 修改操作
                for(int i = 0 ; i < nodeCache.Count ; i++)
                {
                    nodeCache[i].AddChar(input.LastSelectionAnchorPosition, input.LastCaretPostion, charCount, limitNum);
                }
            }
            // 去除无效数据
            RemoveInvalidData();

            // 删除标志位
            isDelete = true;
        }
        // 去除被编辑过的数据
        private void RemoveInvalidData()
        {
            var nList = new List<ChatDefins.DataNode>();
            var cList = new List<ChatDefins.CellData>();

            for(int i = 0 ; i < nodeCache.Count ; i++)
            {
                if(nodeCache[i].isValid)
                {
                    nList.Add(nodeCache[i]);
                    cList.Add(cellCache[i]);
                }
            }

            nodeCache = nList;
            cellCache = cList;
        }

        private void OnReceiveItemData(ChatDefins.CellData data)
        {
            // 英雄帖打开时不操作面板输入框
            var heroPanel = UISystem.Get("UIChatHeroPostPanel");
            if(heroPanel != null && heroPanel.isVisible)
                return;

            // 有相同物品则不处理
            for(int i = 0 ; i < cellCache.Count ; i++)
            {
                if(cellCache[i].pos == data.pos)
                {
                    Assert.IsTrue(nodeCache[i].isValid, "无效数据监测异常");
                    return;
                }
            }

            string msg;
            if(data.isEquiped)
            {
                msg = string.Format("[{0}]", EquipPrototype.Get(data.itemData.id).name);
            }
            else
            {
                msg = data.type == ChatDefins.CellData.Type.Equip
                    ? string.Format("[{0}]", EquipPrototype.Get(data.itemData.id).name)
                    : string.Format("[{0}X{1}]", Item.Get(data.itemData.id).name, data.count);
            }

            msg = CheckCacheName(msg);

            var dataNode = new ChatDefins.DataNode
            {
                startPos = input.LastCaretPostion,
                isValid = true,
                text = msg,
            };
            nodeCache.Add(dataNode);
            cellCache.Add(data);

            CheckInputTextLength(msg);
        }
        // 检查是否存在相同的道具名
        private string CheckCacheName(string str)
        {
            var length = str.Length;
            var temp = 1;
            for(int i = 0 ; i < nodeCache.Count ; i++)
            {
                if(nodeCache[i].text == str)
                {
                    str = str.Substring(0, length) + temp;
                    temp++;
                    i = 0;
                }
            }
            return str;
        }

        private void OnReceivePetsData(ChatDefins.CellData data)
        {
            // 英雄帖打开时不操作面板输入框
            var heroPanel = UISystem.Get("UIChatHeroPostPanel");
            if(heroPanel != null && heroPanel.isVisible)
                return;

            // 有相同物品则不处理
            for(int i = 0 ; i < cellCache.Count ; i++)
            {
                if(cellCache[i].index == data.index)
                {
                    Assert.IsTrue(nodeCache[i].isValid, "无效数据监测异常");
                    return;
                }
            }

            // 宠物变异
            var msg = string.Format("[{0}{1}]"
                                   , data.isVariation ? "变异的" : ""
                                   , PetAttribute.Get(data.petsData.id).name);

            msg = CheckCacheName(msg);
            var dataNode = new ChatDefins.DataNode
            {
                startPos = input.LastCaretPostion,
                isValid = true,
                text = msg,
            };
            nodeCache.Add(dataNode);
            cellCache.Add(data);

            CheckInputTextLength(msg);
        }

        private void OnReceiveUserData(KeyValuePair<string, long> userdata)
        {
            // 同名不处理
            if(!mgr.AddNameCacheData(userdata.Key, userdata.Value))
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append('@');
            sb.Append(userdata.Key);
            sb.Append(' ');
            CheckInputTextLength(sb.ToString());
        }

        private void OnReceiveFaceData(string str)
        {
            var s = "#" + str;
            CheckInputTextLength(s);
        }

        private void OnReceiveHistory(ChatDefins.History history)
        {
            input.selectionAnchorPosition = input.selectionFocusPosition = input.caretPosition = input.text.Length;
            cellCache = history.datas;
            nodeCache = history.nodes;
            CheckInputTextLength(history.message);
        }
        private void OnReceiveSimpleInput(string str)
        {
            CheckInputTextLength(str);
        }

        private void CheckInputTextLength(string str)
        {
            // 超限确认
            if(input.text.Length + str.Length > limitNum)
            {
                SystemHintMgr.ShowTipsHint("chat_char_length");
            }
            var s = input.text.Insert(input.LastCaretPostion, str);
            input.text = s.Substring(0, Mathf.Min(s.Length, limitNum));
            input.LastCaretPostion += str.Length;
        }
        #endregion

        #region Emoji
        private void OnEmojiClick()
        {
            App.my.uiSystem.ShowPanel("UIEmotionPanel", EmotionOpenType.Chat, false);
        }
        #endregion

        #region SendBtn
        private void OnSendBtnClick()
        {
            // 非空字符
            var text = input.text;
            text = text.Replace(" ", "");
            if(string.IsNullOrEmpty(text))
            {
                // 空文本无法发送
                SystemHintMgr.ShowTipsHint(7004);
                return;
            }

            // 保存输入历史
            var historyText = input.text;
            var historyMessage = new ChatDefins.History
            {
                message = historyText,
                datas = cellCache,
                nodes = nodeCache,
            };
            mgr.AddHistory(historyMessage);

            // 频道消耗检查
            if(!ChatUtil.CheckChannelConsume(mgr.CurrentChannel))
            {
                return;
            }

            // 先对道具进行转译
            StringBuilder sb = new StringBuilder();
            var history = input.text;
            if(nodeCache.Count == 0)
            {
                sb.Append(input.text);
            }
            else
            {
                for(int i = 0 ; i < nodeCache.Count ; i++)
                {
                    if(i == 0)
                    {
                        // 头部拼接
                        sb.Append(history.Substring(0, nodeCache[i].startPos));
                        // 节点信息拼接
                        var s = history.Substring(nodeCache[i].startPos, nodeCache[i].text.Length);
                        var kvp = ChatUtil.ReplaceCacheDataToHyperlink(i, s, cellCache);
                        if(!kvp.Value)
                        {
                            SystemHintMgr.ShowTipsHint("chat_send_changed");
                            input.text = "";
                            input.LastCaretPostion = input.LastSelectionAnchorPosition =
                                input.LastSelectionFocusPosition = 0;
                            return;
                        }
                        sb.Append(kvp.Key);
                    }
                    else
                    {
                        // 其余普通文本拼接
                        var sPos = nodeCache[i - 1].startPos + nodeCache[i - 1].text.Length;
                        sb.Append(history.Substring(sPos, nodeCache[i].startPos - sPos));
                        // 节点信息拼接
                        var s = history.Substring(nodeCache[i].startPos, nodeCache[i].text.Length);
                        var kvp = ChatUtil.ReplaceCacheDataToHyperlink(i, s, cellCache);
                        if(!kvp.Value)
                        {
                            SystemHintMgr.ShowTipsHint("chat_send_changed");
                            input.text = "";
                            input.LastCaretPostion = input.LastSelectionAnchorPosition =
                                input.LastSelectionFocusPosition = 0;
                            return;
                        }
                        sb.Append(kvp.Key);
                    }
                    // 尾部拼接
                    if(i == nodeCache.Count - 1)
                    {
                        sb.Append(history.Substring(nodeCache[i].startPos + nodeCache[i].text.Length));
                    }
                }
            }
            List<PetsAttribute> pets = new List<PetsAttribute>();
            List<ItemData> items = new List<ItemData>();

            for(int i = 0 ; i < cellCache.Count ; i++)
            {
                if(cellCache[i].itemData != null)
                {
                    items.Add(cellCache[i].itemData);
                }
                else
                {
                    pets.Add(cellCache[i].petsData);
                }
            }

            // 过滤敏感词
            var message = TextRegexParser.FilterSensitiveWord(sb.ToString());

            // 转译@功能
            if(mgr.CurrentChannel == ChannelType.Channel_Family || mgr.CurrentChannel == ChannelType.Channel_Team)
            {
                message = ChatUtil.ReplaceMarkerName(message);
            }

            // 发送消息
            ChatMsgRequest msg = new ChatMsgRequest
            {
                channel = mgr.CurrentChannel,
                msg = message,
                itemDatas = items,
                petsDatas = pets,
            };
            hotApp.my.eventSet.FireEvent(EventID.ChatModule_OnSendMsg, msg);
            // 复位
            input.LastInputText = "";
            input.text = "";
            input.LastCaretPostion = input.LastSelectionAnchorPosition = input.LastSelectionFocusPosition = 0;
            nodeCache.Clear();
            cellCache.Clear();

            App.my.uiSystem.HidePanel("UIEmotionPanel", false);
        }
        #endregion
        #endregion
    }
}
#endif

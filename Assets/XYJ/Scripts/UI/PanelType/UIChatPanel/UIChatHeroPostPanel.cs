#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Text;
using Config;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using WXB;
using xys.hot.Event;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIChatHeroPostPanel : HotPanelBase
    {
        [SerializeField]
        private InputFieldEx input;
        [SerializeField]
        private Button sendBtn;
        [SerializeField]
        private Button closeBtn;
        [SerializeField]
        private Button emojiBtn;
        [SerializeField]
        private Text num;
        [SerializeField]
        private Image itemImg;
        [SerializeField]
        private SymbolText itemNum;
        [SerializeField]
        private Image itemBgImg;

        private HotObjectEventAgent eventAgent;
        private ChatMgr mgr;
        private int limitNum;                                   // 可输入的字符数限制
        private List<ChatDefins.CellData> cellCache;
        private List<ChatDefins.DataNode> nodeCache;
        private bool isDelete;
        public UIChatHeroPostPanel() : base(null) { }
        public UIChatHeroPostPanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            // 默认可输入50个字符
            limitNum = kvClient.GetInt("chat_hero_num", 50);
        }

        protected override void OnShow(object p)
        {
            // 英雄帖配置
            var cfg = ChatUtil.GetChatChannelConfig(ChannelType.Channel_Hero);
            var item = Item.Get(cfg.GetConsumeItemId());
            itemImg.SetSprite(item.icon);
            itemBgImg.SetSprite("ui_Common_Quality_" + item.quality.ToString().ToUpperFirst());
            var package = hotApp.my.GetModule<HotPackageModule>().packageMgr.package;
            // 当前数量/消耗数量
            var currentCount = package.GetItemCount(item.id);
            var consumeCount = cfg.GetConsumeItemNum();
            if(currentCount >= consumeCount)
            {
                itemNum.text = "#[G2]" + currentCount + "/" + consumeCount;
            }
            else
            {
                itemNum.text = "#[R1]" + currentCount + "/" + consumeCount;
            }
            // 消耗提示
            SystemHintMgr.ShowTipsHint("chat_hero_consume", item.name);

            input.text = "";
            sendBtn.onClick.AddListenerIfNoExist(OnSendBtnClick);
            closeBtn.onClick.AddListenerIfNoExist(OnCloseBtnClick);
            emojiBtn.onClick.AddListenerIfNoExist(OnEmojiBtnClick);
            input.onValueChanged.AddListenerIfNoExist(OnInputValueChange);
            input.onValidateInput = OnValidateInput;

            mgr = hotApp.my.GetModule<HotChatModule>().ChatMgr;
            eventAgent = new HotObjectEventAgent(App.my.localPlayer.eventSet);
            eventAgent.Subscribe<ChatDefins.CellData>(EventID.ChatInput_OnReceivePetsData, OnReceivePetsData);
            eventAgent.Subscribe<ChatDefins.CellData>(EventID.ChatInput_OnReceiveItemData, OnReceiveItemData);
            eventAgent.Subscribe<string>(EventID.ChatInput_OnReceiveFaceData, OnReceiveFaceData);
            eventAgent.Subscribe<ChatDefins.History>(EventID.ChatInput_OnReceiveInputSimple, OnReceiveHistory);
            eventAgent.Subscribe<string>(EventID.ChatInput_OnReceiveInputSimple, OnReceiveSimpleInput);
            nodeCache = new List<ChatDefins.DataNode>();
            cellCache = new List<ChatDefins.CellData>();
        }

        protected override void OnHide()
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
            closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            emojiBtn.onClick.RemoveListener(OnEmojiBtnClick);
            input.onValueChanged.RemoveListener(OnInputValueChange);

            eventAgent.Release();
            eventAgent = null;
        }

        private void OnEmojiBtnClick()
        {
            App.my.uiSystem.ShowPanel("UIEmotionPanel", EmotionOpenType.Hero, false);
        }

        private void OnCloseBtnClick()
        {
            App.my.uiSystem.HidePanel("UIChatHeroPostPanel");
        }

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
            if(!ChatUtil.CheckChannelConsume(ChannelType.Channel_Hero))
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
                        Debug.Log(string.Format("{0}\n startPos:{1}-{2}\n{3}", sb, nodeCache[i].startPos, nodeCache[i].text.Length, history));
                        var s = history.Substring(nodeCache[i].startPos, nodeCache[i].text.Length);
                        var kvp = ChatUtil.ReplaceCacheDataToHyperlink(i, s, cellCache);
                        if(!kvp.Value)
                        {
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
                        Debug.Log(string.Format("{0}\n <color=red>startPos</color>:{1}-{2}\n{3}", sb, nodeCache[i].startPos, nodeCache[i].text.Length, history));
                        var s = history.Substring(nodeCache[i].startPos, nodeCache[i].text.Length);
                        var kvp = ChatUtil.ReplaceCacheDataToHyperlink(i, s, cellCache);
                        if(!kvp.Value)
                        {
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

            // 发送消息
            ChatMsgRequest msg = new ChatMsgRequest
            {
                channel = ChannelType.Channel_Hero,
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
            OnCloseBtnClick();
        }
        private void OnReceiveItemData(ChatDefins.CellData data)
        {
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

        private void OnReceivePetsData(ChatDefins.CellData data)
        {
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

        private char OnValidateInput(string text, int charindex, char addedchar)
        {
            // 确保不是删除操作
            isDelete = false;
            return addedchar;
        }
        private void OnInputValueChange(string str)
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
        // 超限检查
        private void CheckInputTextLength(string str)
        {
            if(input.text.Length + str.Length > limitNum)
            {
                SystemHintMgr.ShowTipsHint("chat_char_length");
            }
            var s = input.text.Insert(input.LastCaretPostion, str);
            input.text = s.Substring(0, Mathf.Min(s.Length, limitNum));
            input.LastCaretPostion += str.Length;
        }
    }
}
#endif
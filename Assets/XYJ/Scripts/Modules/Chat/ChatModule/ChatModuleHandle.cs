#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using NetProto;
using NetProto.Hot;
using Network;
using wProtobuf.RPC;
using xys.UI;
using Int64 = NetProto.Int64;

namespace xys.hot
{
    partial class HotChatModule
    {
        private ChatModuleRequest chatRequest;
        private C2WItemDataRequest itemRequest;
        void Init()
        {
            // 注册协议
            chatRequest = new ChatModuleRequest(App.my.game.local);
            itemRequest = new C2WItemDataRequest(App.my.world.local);

            // 事件监听
            // 发送消息
            hotApp.my.eventSet.Subscribe<ChatMsgRequest>(EventID.ChatModule_OnSendMsg, OnSendMsg);
            // 物品搜索
            hotApp.my.eventSet.Subscribe<long>(EventID.ChatModule_OnSearchItems, OnSearchItems);
            // 宠物搜索
            hotApp.my.eventSet.Subscribe<long>(EventID.ChatModule_OnSearchPets, OnSearchPets);
            // 系统消息
            hotApp.my.localEvent.Subscribe<SystemChatMsg>(EventID.ChatModule_OnSendSystemMsg, OnSendSystemMsg);
            // 接收消息
            hotApp.my.handler.Reg<ChatMsgRspone>(Protoid.W2C_ChatChatMsg, OnReceiveMsg);
        }

        private void OnSendSystemMsg(SystemChatMsg data)
        {
            ChatMgr.AddSystemMsg(data.channel, 1);
        }
        private void OnSearchPets(long petsId)
        {
            var petsData = ChatMgr.GetPetsData(petsId);
            if(null != petsData)
            {
                ChatUtil.ShowPetsTips(petsData);
            }
            else
            {
                App.my.mainCoroutine.StartCoroutine(OnSearchPetsYield(petsId));
            }
        }

        private IEnumerator OnSearchPetsYield(long petsId)
        {
            Int64 input = new Int64() { value = petsId };
            var yyd = itemRequest.SearchPetsDataYield(input);
            yield return yyd;
            ChatMgr.AddPetsData(petsId, yyd.result);
            ChatUtil.ShowPetsTips(yyd.result);
        }

        private void OnSearchItems(long itemId)
        {
            var itemData = ChatMgr.GetItemData(itemId);
            if(null == itemData)
            {
                App.my.mainCoroutine.StartCoroutine(OnSearchItemsYield(itemId));
            }
            else
            {
                ChatUtil.ShowItemTips(itemData);
            }
        }

        private IEnumerator OnSearchItemsYield(long itemId)
        {
            Int64 input = new Int64 { value = itemId };
            var yyd = itemRequest.SearchItemDataYield(input);
            yield return yyd;
            ChatMgr.AddItemData(itemId, yyd.result);
            ChatUtil.ShowItemTips(yyd.result);
        }
        private void OnReceiveMsg(ChatMsgRspone msg)
        {
            Debuger.Log("receive message! ".Color() + msg.msg);
            ChatMgr.AddMsg(msg.channel, msg);
        }

        private void OnSendMsg(ChatMsgRequest msg)
        {
            // CD检查
            if(!ChatMgr.CheckChannelTime(msg.channel, App.my.mainTimer.GetTime.GetCurrentTime()))
            {
                return;
            }

            chatRequest.SendMsg(msg, (error, ret) =>
            {
                if(Error.Success != error)
                {
                    Debuger.LogError("send message rpc is no success!->{" + error + "}");
                    return;
                }

                if(ReturnCode.ReturnCode_OK != ret.code)
                {
                    Debuger.Log("send message fail -> {" + ret.code + "}");
                }

                var cfg = ChatUtil.GetChatChannelConfig(msg.channel);
                switch(ret.code)
                {
                    case ReturnCode.ReturnCode_OK:
                        break;
                    case ReturnCode.Chat_SendMsgFail:
                        SystemHintMgr.ShowHint(TipsContent.Get(7000).des);
                        break;
                    case ReturnCode.Chat_SendMsgFail_Colddown:
                        SystemHintMgr.ShowHint(TipsContent.Get(7001).des, cfg.time - (int)(ret.cd / 10000000));
                        break;
                    case ReturnCode.Chat_SendMsgFail_Itemless:
                        SystemHintMgr.ShowHint(TipsContent.Get(7002).des, "[" + ItemBaseAll.Get(cfg.GetConsumeItemId()).name + "]");
                        break;
                    case ReturnCode.Chat_SendMsgFail_Channel:
                        SystemHintMgr.ShowHint(TipsContent.Get(7003).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_TextEmpty:
                        SystemHintMgr.ShowHint(TipsContent.Get(7004).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_TextOverStarck:
                        SystemHintMgr.ShowHint(TipsContent.Get(7005).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_User:
                        SystemHintMgr.ShowHint(TipsContent.Get(7006).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_NotFriendship:
                        SystemHintMgr.ShowHint(TipsContent.Get(7007).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_SaveDB:
                        SystemHintMgr.ShowHint(TipsContent.Get(7008).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_Silver:
                        SystemHintMgr.ShowHint(TipsContent.Get(7010).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_Gold:
                        SystemHintMgr.ShowHint(TipsContent.Get(7011).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_Fairy:
                        SystemHintMgr.ShowHint(TipsContent.Get(7012).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendMsgFail_Jasper:
                        SystemHintMgr.ShowHint(TipsContent.Get(7013).des, args: "test");
                        break;
                    case ReturnCode.Chat_SendVoiceFail_Drity:
                        break;
                    case ReturnCode.Chat_SendVoiceFail_DurationLess:
                        break;
                    case ReturnCode.Chat_SendVoiceFail_DutationMore:
                        break;
                    default:
                        Debuger.LogError("chatmodule receive unknow return code! -> {" + ret.code + "}");
                        break;
                }

            });
        }

    }
}

#endif
#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;
using xys.UI;
using xys.hot.UI.Friend;
using xys.UI.State;
using UnityEngine.UI;
using NetProto;
using NetProto.Hot;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{
    [AutoILMono]
    class FriendMessage
    {

        public enum PageType
        {
            None = 0 ,  //未选择
            System = 1 , //系统  
            Friend = 2 , //好友
        };

        private PageType m_PageType;

        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        StateRoot m_PageState;

        FriendItemInfo m_data = null;

        public FriendMessage() { }

        public void OnStart()
        {
            m_PageType = PageType.None;
        }

        public void SetData(FriendItemInfo data,int idx) //0 没有选中 1系统 2好友
        {
                 
            m_data = null;
            m_data = data;

            if (idx == 0)
            {
                m_PageType = PageType.None;
            }
            else if (idx == 1)
            {
                m_PageType = PageType.System;
            }
            else if (idx == 2)
            {
                m_PageType = PageType.Friend;
            }

            RefreshUI();

        }

        public void SetEvent(EventAgent eventAgent)
        {
            eventAgent.Subscribe<FriendItemInfo>(EventID.Friend_ResetMessageFrame, this.ResetMsgFrame);
        }

        public void SetMessgeType(int index)
        {
            if (index >= (int)PageType.Friend && index <0)
            {
                return;
            }

            m_PageState.SetCurrentState(index, true);
        }

        public void RefreshUI()
        {
            if (m_data != null)
            {
                if (m_data.charid <= 0)
                {
                    m_PageState.SetCurrentState(1, true);
                    m_Transform.Find("FriendChatTips").Find("HaoYouText").gameObject.SetActive(false);
                }

                if (m_data.name != "" && m_PageType != PageType.System)
                {
                    m_Transform.Find("FriendChatTips").Find("Text").GetComponent<Text>().text = string.Format("正在与{0}聊", m_data.name);
                    m_Transform.Find("FriendChatTips").Find("HaoYouText").gameObject.SetActive(true);
                    m_Transform.Find("FriendChatTips").Find("HaoYouText").GetComponent<Text>().text = string.Format("好友度：{0}", m_data.friendLiness);
                }
                else if (m_PageType == PageType.System)
                {
                    m_Transform.Find("FriendChatTips").Find("Text").GetComponent<Text>().text = "系统消息";
                }
            }
        }

        public void ResetMsgFrame(FriendItemInfo info)
        {
            if (m_PageState.CurrentState == 2)
            {
                if (m_data.charid == info.charid)
                {
                    m_PageType = PageType.None;
                    m_PageState.SetCurrentState(0, true);
                }
            }
        }

    }

}
#endif
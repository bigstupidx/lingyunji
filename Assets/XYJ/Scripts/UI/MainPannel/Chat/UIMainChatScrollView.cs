#if !USE_HOT
using System;
using System.Collections.Generic;
using Config;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Event;
using xys.UI;

namespace xys.hot.UI
{
    [Serializable]
    class UIMainChatScrollView
    {
        [SerializeField]
        public RectTransform rect;
        [SerializeField]
        private VerticalScrollRectWrapper scrollRectWrapper;
        [SerializeField]
        [Tooltip("初始化时的高度")]
        private float initHeight = 37f;
        [SerializeField]
        [Tooltip("未扩展时的最大高度")]
        private float noExpandHeight = 185f;
        [SerializeField]
        [Tooltip("已扩展后的最大高度")]
        private float expandedHeight = 267f;
        [SerializeField]
        private Button botBtn;
        [SerializeField]
        private Text botText;

        [SerializeField]
        private Button markerBtn;
        [SerializeField]
        private Text markerText;

        private HotObjectEventAgent eventAgent;
        private ChatMgr mgr;
        private int newMsgNum = 0;
        private object[] markerKvp;
        #region Impl
        public void OnInit()
        {
            // 初始状态未扩展
            LocalSave.SetBool(ChatDefins.Expand, false);
            // 初始状态默认为空
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, initHeight);

            mgr = hotApp.my.GetModule<HotChatModule>().ChatMgr;
            scrollRectWrapper.enabled = true;
            scrollRectWrapper.SetMaxCount(mgr.GetMaxNumber(ChannelType.Channel_None));
            scrollRectWrapper.OnClickedWithoutDragEvent.AddListener(OnClickMainChatWithoutDrag);
            markerText.text = kvClient.Get("chat_marker").value;
        }

        public void OnShow()
        {
            eventAgent = new HotObjectEventAgent(App.my.localPlayer.eventSet);

            eventAgent.Subscribe<float>(EventID.ChatMainPanel_RefreshMainChatHeight, ChangeHeight);
            eventAgent.Subscribe<float>(EventID.ChatMainPanel_ExpandBtnClick, OnExpandBtnClick);
            eventAgent.Subscribe(EventID.ChatMainPanel_NewMessageTip, OnReceiveNewMessage);
            eventAgent.Subscribe<object[]>(EventID.ChatMainPanel_Marker, OnMarkerReceive);
            botBtn.onClick.AddListenerIfNoExist(OnBtnClick);
            markerBtn.onClick.AddListenerIfNoExist(OnMarkerBtnClick);
        }

        public void OnHide()
        {
            botBtn.onClick.RemoveListener(OnBtnClick);
            markerBtn.onClick.RemoveListener(OnMarkerBtnClick);
            eventAgent.Release();
            eventAgent = null;
        }

        #endregion
        public void AddData()
        {
            scrollRectWrapper.AddData();
        }

        #region Event
        private void ChangeHeight(float height)
        {

            if(LocalSave.GetBool(ChatDefins.Expand))
            {
                if(rect.rect.height + height > expandedHeight)
                {
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, expandedHeight);
                }
                else
                {
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + height);
                }
            }
            else
            {
                if(rect.rect.height + height > noExpandHeight)
                {
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, noExpandHeight);
                }
                else
                {
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + height);
                }
            }
        }

        private void OnExpandBtnClick(float changeHeight)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + changeHeight);
            scrollRectWrapper.SetBottom();
        }

        private void OnBtnClick()
        {
            var index = mgr.GetMsgNumber(ChannelType.Channel_None) - 1;
            if(index >= 0)
            {
                scrollRectWrapper.SetBottom();
                newMsgNum = 0;
                botBtn.gameObject.SetActive(false);
            }
        }

        private void OnReceiveNewMessage()
        {
            newMsgNum++;
            if(scrollRectWrapper.IsLock)
            {
                botBtn.gameObject.SetActive(true);
            }

            botText.text = string.Format(kvClient.Get("chat_unread").value, newMsgNum);
        }

        private void OnMarkerBtnClick()
        {
            App.my.uiSystem.ShowPanel("UIChatPanel", markerKvp, false);
            markerBtn.gameObject.SetActive(false);
        }

        private void OnMarkerReceive(object[] objs)
        {
            markerKvp = objs;
            markerBtn.gameObject.SetActive(true);
        }
        private void OnClickMainChatWithoutDrag()
        {
            var p = UISystem.Get("UIChatPanel");
            if(p == null || !p.gameObject.activeSelf)
            {
                App.my.uiSystem.ShowPanel("UIChatPanel", new object() { }, false);
            }
        }
        #endregion

    }
}

#endif
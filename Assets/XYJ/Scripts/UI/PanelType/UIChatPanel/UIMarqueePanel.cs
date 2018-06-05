#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using NetProto;
using UI;
using UnityEngine;
using WXB;
using xys.UI;

namespace xys.hot.UI
{
    class UIMarqueePanel : HotPanelBase
    {
        #region Field
        // 英雄帖
        [SerializeField]
        private GameObject nodeHero;
        [SerializeField]
        private RectTransform rectHero;
        [SerializeField]
        private RectTransform textRtHero;
        [SerializeField]
        private SymbolText textHero;
        [SerializeField]
        private UGUITweenPosition tweenHero;
        // 公告
        [SerializeField]
        private GameObject nodeSystem;
        [SerializeField]
        private RectTransform rectSystem;
        [SerializeField]
        private RectTransform textRtSystem;
        [SerializeField]
        private SymbolText textSystem;
        [SerializeField]
        private UGUITweenPosition tweenSystem;
        // temp
        private RectTransform rect;
        private RectTransform textRt;
        private SymbolText text;
        private UGUITweenPosition tween;

        private ChatMgr mgr;
        private float movePos;
        #endregion

        #region Impl
        public UIMarqueePanel() : base(null) { }
        public UIMarqueePanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            mgr = ChatUtil.ChatMgr;
        }

        protected override void OnShow(object p)
        {
            InitMarquee();
        }

        #region Local
        private void OnTweenFinished()
        {
            textRt.localPosition = Vector3.zero;
            tween.ResetToBeginning();
            InitMarquee();
        }

        private void InitMarquee()
        {
            if(mgr.MarqueeQueue.Count <= 0)
            {
                App.my.uiSystem.HidePanel("UIMarqueePanel", false);
                return;
            }
            var msg = mgr.MarqueeQueue.Dequeue();

            if(msg.channel == ChannelType.Channel_Hero)
            {
                rect = rectHero;
                textRt = textRtHero;
                text = textHero;
                tween = tweenHero;
                text.text = string.Format("[<color=#[{2}]>{0}</color>]{1}", msg.fromUser.name, msg.msg, kvClient.Get("chat_hero_name_color").value);
            }
            else if(msg.channel == ChannelType.Channel_System)
            {
                rect = rectSystem;
                textRt = textRtSystem;
                text = textSystem;
                tween = tweenSystem;
                text.text = msg.msg;
            }
            else
            {
                Debuger.LogError("错误的消息频道！".Color());
                App.my.uiSystem.HidePanel("UIMarqueePanel", false);
                return;
            }

            textRt.sizeDelta = new Vector2(text.preferredWidth, textRt.sizeDelta.y);
            movePos = rect.rect.width + textRt.rect.width;
            tween.from = Vector3.zero;
            tween.to = new Vector3(-1 * movePos, 0f);
            tween.duration = movePos / kvClient.GetFloat("chat_hero_moveSpeed", 10f);
            tween.onFinished.AddListenerIfNoExist(OnTweenFinished);
            tween.PlayForward();
        }
        #endregion

        #endregion
    }
}

#endif
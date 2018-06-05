#if !USE_HOT
using System;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [Serializable]
    class UIChatPanelCommon
    {
        [SerializeField]
        private GameObject tip;
        [SerializeField]
        private UIChatInput input;
        [SerializeField]
        private VerticalScrollRectWrapper scrollRect;
        [SerializeField]
        private GameObject empty;
        [SerializeField]
        private GameObject teamEmpty;
        [SerializeField]
        private GameObject parent;
        private ChatMgr mgr;
        public void OnInit()
        {
            mgr = hotApp.my.GetModule<HotChatModule>().ChatMgr;
            scrollRect.OnInit();
        }

        public void OnShow(object arg = null)
        {
            parent.SetActive(true);
            var current = mgr.CurrentChannel;
            scrollRect.Init(mgr.GetStartIndex(current)
                , mgr.GetMaxNumber(current)
                , mgr.GetMaxIndex(current));

            teamEmpty.SetActive(false);
            input.OnHide();
            empty.SetActive(false);
            tip.gameObject.SetActive(true);


            if(current != ChannelType.Channel_Family)
            {
                if(current == ChannelType.Channel_Team || current == ChannelType.Channel_GlobalTeam)
                {
                    if(!TeamUtil.teamMgr.InTeam())
                    {
                        teamEmpty.SetActive(true);
                    }
                    else
                    {
                        input.OnShow();
                    }
                }
                else
                {
                    input.OnShow();
                }
            }
            else
            {
                // TODO 氏族暂时屏蔽
                tip.gameObject.SetActive(false);
                empty.SetActive(true);
            }


            if(null != arg)
            {
                scrollRect.SetMarkerToCenter((int)arg);
            }
        }

        public void OnHide()
        {
            parent.SetActive(false);
            scrollRect.OnCellAdding = null;
            var current = mgr.CurrentChannel;
            mgr.SetMsgStartIndex(current, scrollRect.GetStartIndex());

            tip.gameObject.SetActive(false);
            input.OnHide();
            empty.SetActive(false);
            scrollRect.ClearCells();
        }

        public void Refresh(object arg = null)
        {
            parent.SetActive(true);
            // 保存上个频道的索引
            mgr.SetMsgStartIndex(mgr.LastChannelType, scrollRect.GetStartIndex());
            scrollRect.ClearCells();

            var current = mgr.CurrentChannel;
            scrollRect.Init(mgr.GetStartIndex(current)
                , mgr.GetMaxNumber(current)
                , mgr.GetMaxIndex(current));

            teamEmpty.SetActive(false);
            input.OnHide();
            empty.SetActive(false);
            tip.gameObject.SetActive(true);

            if(current != ChannelType.Channel_Family)
            {
                if(current == ChannelType.Channel_Team || current == ChannelType.Channel_GlobalTeam)
                {
                    if(!TeamUtil.teamMgr.InTeam())
                    {
                        teamEmpty.SetActive(true);
                    }
                    else
                    {
                        input.OnShow();
                    }
                }
                else
                {
                    input.OnShow();
                }
            }
            else
            {
                // TODO 氏族暂时屏蔽
                tip.gameObject.SetActive(false);
                empty.SetActive(true);
            }

            if(null != arg)
            {
                scrollRect.SetMarkerToCenter((int)arg);
            }
        }

        public void AddData()
        {
            scrollRect.AddData();
        }

        public bool IsLock()
        {
            return scrollRect.IsLock;
        }
        // 获取当前消息的最大索引数据
        public int GetMaxItemIndexInView()
        {
            if(scrollRect.Content.childCount > 0)
            {
                var child = scrollRect.Content.GetChild(scrollRect.Content.childCount - 1);
                var obj = (ChatMessageCommon)child.GetComponent<ILMonoBehaviour>().GetObject();
                return obj.GetIndex();
            }
            return 0;
        }

        public void SetBot()
        {
            scrollRect.SetBottom();
        }

        public void SetLock(bool isLock)
        {
            scrollRect.IsLock = isLock;
        }

        public VerticalScrollRectWrapper GetVeriticalWrapper()
        {
            return scrollRect;
        }
    }
}
#endif

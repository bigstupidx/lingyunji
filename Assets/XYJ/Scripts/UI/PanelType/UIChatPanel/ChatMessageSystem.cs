#if !USE_HOT
using System;
using UnityEngine;
using UnityEngine.UI;
using WXB;

namespace xys.hot.UI
{
    [AutoILMono]
    class ChatMessageSystem
    {
        [SerializeField]
        private SymbolText text;

        [SerializeField] private RectTransform textRectTransform;
        [SerializeField] private SymbolTextEvent textEvent;
        [SerializeField] private LayoutElement element;
        private void OnCellAdding(int index)
        {
            text.text = ChatUtil.ChatMgr.GetSystemMsgWithIndex(index);
            element.preferredHeight = text.preferredHeight;
            textRectTransform.sizeDelta = new Vector2(textRectTransform.sizeDelta.x,text.preferredHeight);
            textEvent.OnClickWithoutDragging.AddListenerIfNoExist(ChatUtil.OnNodeClick);
        }
    }
} 
#endif

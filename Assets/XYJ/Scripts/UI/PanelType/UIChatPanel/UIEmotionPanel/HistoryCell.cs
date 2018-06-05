#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    class HistoryCell
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private ButtonEx btnEx;

        private int index;
        private ChatDefins.History history;
        public void OnCellAdding(int index)
        {
            var mgr = hotApp.my.GetModule<HotChatModule>().ChatMgr;
            this.index = index;
            history = mgr.GetHistory(index);
            text.text = history.message;
        }

        private void OnEnable()
        {
            btnEx.OnClickWithoutDrag.AddListenerIfNoExist(OnBtnClick);
        }

        private void OnDisable()
        {
            btnEx.OnClickWithoutDrag.RemoveAllListeners();
        }

        private void OnBtnClick()
        {
            hotApp.my.eventSet.FireEvent(EventID.ChatInput_OnReceiveInputHistory, history);
        }
    }
}
#endif

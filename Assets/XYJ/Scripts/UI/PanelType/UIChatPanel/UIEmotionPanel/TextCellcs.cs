#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    class TextCellcs
    {
        [SerializeField] private Text text;
        [SerializeField] private ButtonEx btnEx;

        private void OnEnable()
        {
            btnEx.OnClickWithoutDrag.AddListenerIfNoExist(OnBtnClick);
        }

        private void OnDisable()
        {
            btnEx.OnLongPress.RemoveAllListeners();
            btnEx.OnClickWithoutDrag.RemoveAllListeners();
        }

        private void OnBtnClick()
        {
            hotApp.my.eventSet.FireEvent(EventID.ChatInput_OnReceiveInputSimple, text.text);
        }

        public void OnCellAdding(int index)
        {
            text.text = ChatInputSimple.Get(index).input;
        }
    }
} 
#endif

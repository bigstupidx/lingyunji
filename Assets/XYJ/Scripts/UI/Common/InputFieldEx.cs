using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace xys.UI
{
    /// <summary>
    /// 加入光标支持
    /// </summary>
    [AddComponentMenu("UI/Input Field Ex", 32)]
    public class InputFieldEx : InputField
    {
        private bool flag = true;
        [HideInInspector]
        public int LastCaretPostion { get; set; }
        [HideInInspector]
        public int LastSelectionFocusPosition { get; set; }
        [HideInInspector]
        public int LastSelectionAnchorPosition { get; set; }
        [HideInInspector]
        public string LastInputText { get; set; }

        public override void OnDeselect(BaseEventData eventData)
        {
            LastCaretPostion = caretPosition;
            LastSelectionFocusPosition = selectionFocusPosition;
            LastSelectionAnchorPosition = selectionAnchorPosition;
            flag = false;
            base.OnDeselect(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            selectionAnchorPosition = selectionFocusPosition = caretPosition = LastCaretPostion;
            base.OnSelect(eventData);
            flag = true;
        }

        private void Update()
        {
            if(flag)
            {
                LastCaretPostion = caretPosition;
                LastSelectionFocusPosition = selectionFocusPosition;
                LastSelectionAnchorPosition = selectionAnchorPosition;
            }
            LastInputText = text;
        }
    }
}

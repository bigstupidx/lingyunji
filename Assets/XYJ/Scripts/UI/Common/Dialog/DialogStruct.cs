using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace xys.UI
{
    namespace Dialog
    {
        public class Data
        {
            public string title = ""; // 标题
            public string message= ""; // 消息

            public bool isModle = false;

            public System.Action hideFun; // 面板隐藏时的回调

            public bool isBlur = false;
        }

        public class OneBtnData : Data
        {
            public string btnText; // 按钮文本
            public System.Func<bool> btnClick; // 按钮事件回调
        }

        public class TwoBtnData : Data
        {
            // 左右按钮
            public string leftBtnText;
            public string rightBtnText;

            public System.Func<bool> leftBtnClick;
            public System.Func<bool> rightBtnClick;

            public float autoHideTime = 0.0f;
        }
    }
}
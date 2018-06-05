using WXB;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    namespace Dialog
    {
        public class OneBtn : DialogBase
        {
            [SerializeField]
            Text mBtnText;

            Func<bool> mFunc;

            protected override void OnShow(Data d)
            {
                OneBtnData data = (OneBtnData)d;
                mBtnText.text = GlobalSymbol.ToUT(data.btnText);
                mFunc = data.btnClick;
            }

            // 注意，OneBtn对象一旦被隐藏，会被对象池自动回收，外部尽量不要保存些对象，
            // 如果保存，要注意下，一旦对象被隐藏，会被回收
            // fun 返回值为fasle,表示隐藏面板，此面板会被回收
            public static OneBtn Show(string title, string message, string btntext, Func<bool> fun, bool isPlayAnim, bool ismode = false, Action hfun = null)
            {
                OneBtnData s_data = new OneBtnData();
                s_data.title = title;
                s_data.message = message;
                s_data.isModle = ismode;
                s_data.hideFun = hfun;

                s_data.btnText = btntext;
                s_data.btnClick = fun;

                return xys.App.my.uiSystem.dialogMgr.Show<OneBtn>(s_data, isPlayAnim);
            }

            // 点击了按钮
            public void OnBtnClick()
            {
                OnClickBtn(mFunc);
            }

            protected override void OnRelease()
            {
                mFunc = null;
                mBtnText.text = "";
            }
        }
    }
}
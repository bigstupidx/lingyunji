using WXB;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    namespace Dialog
    {
        public class TwoBtn : DialogBase
        {
            [SerializeField]
            Text mLeftBtnText;

            [SerializeField]
            Text mRightBtnText;

            Func<bool> mLeftFunc;
            Func<bool> mRightFunc;

            // 左边按钮带自动取消定时
            int mAutoHideTimer = 0;
            float mAutoHideTimeCounter = 0.0f;
            string mLeftBtnStr = string.Empty;

            protected override void OnShow(Data d)
            {
                TwoBtnData data = (TwoBtnData)d;

                mLeftBtnText.text = GlobalSymbol.ToUT(data.leftBtnText);
                mRightBtnText.text = GlobalSymbol.ToUT(data.rightBtnText);

                mLeftFunc = data.leftBtnClick;
                mRightFunc = data.rightBtnClick;

                mAutoHideTimer = 0;
                mAutoHideTimeCounter = 0.0f;
                if (data.autoHideTime > 0.0f)
                {
                    mAutoHideTimeCounter = data.autoHideTime + 1.0f;
                    mLeftBtnStr = data.leftBtnText;
                    mAutoHideTimer = App.my.mainTimer.Register(1, int.MaxValue, AutoPlayHideTime); // Utils.TimerManage.AddTimer(AutoPlayHideTime, 0, 1, 0);
                }
            }
            // 自动隐藏事件
            void AutoPlayHideTime ()
            {
                mAutoHideTimeCounter -= 1.0f;
                if (mAutoHideTimeCounter<=0.0f)
                {
                    mAutoHideTimeCounter = 0.0f;
                    App.my.mainTimer.Cannel(mAutoHideTimer);
                    Hide(true);
                    return;
                }

                int sec = Mathf.FloorToInt(mAutoHideTimeCounter);
                if (mLeftBtnText != null)
                {
                    mLeftBtnText.text = string.Format("{0}({1})", mLeftBtnStr, sec);
                }	
            }

            // 注意，OneBtn对象一旦被隐藏，会被对象池自动回收，外部尽量不要保存些对象，
            // 如果保存，要注意下，一旦对象被隐藏，会被回收
            // leftfun rightbtn 返回值为fasle,表示隐藏面板，此面板会被回收
            // autoHideTime 左边按钮带自动取消定时的时间
            public static TwoBtn Show(
                string title,
                string message,
                string leftbtn, 
                Func<bool> leftfun, 
                string rightbtn, 
                Func<bool> rightfun, 
                bool isPlayAnim, 
                bool ismodal = false,
                Action hfun = null,
                float autoHideTime = 0.0f)
            {
                TwoBtnData s_data = new TwoBtnData();
                s_data.title = title;
                s_data.message = message;
                s_data.leftBtnClick = leftfun;
                s_data.rightBtnClick = rightfun;

                s_data.leftBtnText = leftbtn;
                s_data.rightBtnText = rightbtn;

                s_data.isModle = ismodal;
                s_data.hideFun = hfun;

                s_data.autoHideTime = autoHideTime;

                return xys.App.my.uiSystem.dialogMgr.Show<TwoBtn>(s_data, isPlayAnim);
            }

            public void OnBtnClickLeft()
            {
                OnClickBtn(mLeftFunc);
            }

            public void OnBtnClickRight()
            {
                OnClickBtn(mRightFunc);
            }

            protected override void OnRelease()
            {
                mLeftFunc = null;
                mRightFunc = null;

                mLeftBtnText.text = null;
                mRightBtnText.text = null;

                mAutoHideTimeCounter = 0.0f;
                App.my.mainTimer.Cannel(mAutoHideTimer);
            }
            //20170119 设置按钮左边的文字//
            public void SetLeftBtnText(string pTxt)
			{
				if (mLeftBtnText != null) {
					mLeftBtnText.text = "" + pTxt;
				}
			}
			//20170119 设置按钮右边的文字//
			public void SetRightBtnText(string pTxt)
			{
				if (mRightBtnText != null) {
					mRightBtnText.text = "" + pTxt;
                    mLeftBtnStr = pTxt;
				}	
			}
        }
    }
}
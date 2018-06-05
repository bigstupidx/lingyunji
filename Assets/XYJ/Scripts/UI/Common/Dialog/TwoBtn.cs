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

            // ��߰�ť���Զ�ȡ����ʱ
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
            // �Զ������¼�
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

            // ע�⣬OneBtn����һ�������أ��ᱻ������Զ����գ��ⲿ������Ҫ����Щ����
            // ������棬Ҫע���£�һ���������أ��ᱻ����
            // leftfun rightbtn ����ֵΪfasle,��ʾ������壬�����ᱻ����
            // autoHideTime ��߰�ť���Զ�ȡ����ʱ��ʱ��
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
            //20170119 ���ð�ť��ߵ�����//
            public void SetLeftBtnText(string pTxt)
			{
				if (mLeftBtnText != null) {
					mLeftBtnText.text = "" + pTxt;
				}
			}
			//20170119 ���ð�ť�ұߵ�����//
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
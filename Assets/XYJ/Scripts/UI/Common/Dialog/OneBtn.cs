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

            // ע�⣬OneBtn����һ�������أ��ᱻ������Զ����գ��ⲿ������Ҫ����Щ����
            // ������棬Ҫע���£�һ���������أ��ᱻ����
            // fun ����ֵΪfasle,��ʾ������壬�����ᱻ����
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

            // ����˰�ť
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
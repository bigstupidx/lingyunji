using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace xys.UI
{
    namespace Dialog
    {
        public class Data
        {
            public string title = ""; // ����
            public string message= ""; // ��Ϣ

            public bool isModle = false;

            public System.Action hideFun; // �������ʱ�Ļص�

            public bool isBlur = false;
        }

        public class OneBtnData : Data
        {
            public string btnText; // ��ť�ı�
            public System.Func<bool> btnClick; // ��ť�¼��ص�
        }

        public class TwoBtnData : Data
        {
            // ���Ұ�ť
            public string leftBtnText;
            public string rightBtnText;

            public System.Func<bool> leftBtnClick;
            public System.Func<bool> rightBtnClick;

            public float autoHideTime = 0.0f;
        }
    }
}
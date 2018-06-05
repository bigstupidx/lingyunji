using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PackTool;
using UI;
using UnityEngine.UI;
using xys.UI;

namespace xys.gm
{

    public partial class GM_UI : MonoBehaviour
    {
        static public GM_UI instance = null;

        Label m_fpsLabel;

        bool m_showGm = false;         // 是否显示该工具
        string m_gmCmd = string.Empty;   // GM命令

        public float w =100 ;
        public float h =50;

        public float w1 = 100;
        public float h1=50;

        public static void Create()
        {
            RALoad.LoadPrefab("GMTool", (GameObject go, object p) => { Helper.SetParent(go,App.my.uiSystem.PanelRoot); }, null);
        }

        void ClickHide()
        {
            m_showGm = !m_showGm;

            ////没有显示gm的时候不要调用OnGUI
            //if (m_showGm)
            //    this.enabled = true;
            //else
            //    this.enabled = false;
            this.enabled = true;

            //设置阻挡
            transform.Find("bg").gameObject.SetActive(m_showGm);
        }


        void Awake()
        {
            instance = this;

            m_fpsLabel = transform.Find("fps/label").GetComponent<Label>();
            GameObject gmGO = transform.Find("gm").gameObject;
            gmGO.GetComponent<Button>().onClick.AddListener(ClickHide);

            InitPage();         
            this.gameObject.AddComponentIfNoExist<GM_FpsCounter>().InitFps(m_fpsLabel);
            enabled = false;
        }





        void OnGUI()
        {
            float dw = Screen.width / 1280.0f;
            if (m_showGm)
            {

                BeginHanleFoneSize();

                PageGUI(dw);

                GM_Log.LogUI(m_logRect);

                EndHanleFoneSize();
            }

            OnGUIList();
        }

        static int oldLabelFontSize;
        static int oldButtonFontSize;
        static int oldToggleFontSize;
        //设置字体大小
        static void BeginHanleFoneSize()
        {
            //修改字体大小
            float height = 640f;
            //float width = Screen.width * height / Screen.height;
            float s = Screen.height / height;

            ///*int*/
            //oldLabelFontSize = GUI.skin.label.fontSize;
            ///*int*/
            //oldButtonFontSize = GUI.skin.button.fontSize;
            ///*int*/
            //oldToggleFontSize = GUI.skin.toggle.fontSize;
            GUI.skin.label.fontSize = (int)(20 * s);
            GUI.skin.button.fontSize = (int)(20 * s);
            GUI.skin.toggle.fontSize = (int)(20 * s);
            GUI.skin.textField.fontSize = (int)(20 * s);
            GUI.skin.textArea.fontSize = (int)(20 * s);
        }

        static void EndHanleFoneSize()
        {
            ////还原字体
            //GUI.skin.label.fontSize = oldLabelFontSize;
            //GUI.skin.button.fontSize = oldButtonFontSize;
            //GUI.skin.toggle.fontSize = oldToggleFontSize;
        }
    }

}

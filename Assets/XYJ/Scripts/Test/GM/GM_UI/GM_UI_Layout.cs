/*----------------------------------------------------------------
// 创建者：
// 创建日期:
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;

namespace xys.gm
{

    public partial class GM_UI
    {
        public Rect m_titleRect = new Rect(100, 200, 500, 500);
        public Rect m_pageRect = new Rect(200, 200, 500, 600);
        public Rect m_logRect = new Rect(600, 200, 500, 800);


        public int m_titleWidth = 100;


        Vector2 m_titleScrollPosition;

        List<GM_IPage> m_uiPages = new List<GM_IPage>();

        int m_curPage = -1;

        //初始化每个分页
        void InitPage()
        {
            m_uiPages.Add(new GM_BattlePage());
            m_uiPages.Add(new GM_InputCmdPage());
            m_uiPages.Add(new GM_ItemCmdPage());
            m_uiPages.Add(new GM_PetsCmdPage());
            m_uiPages.Add(new GM_EquipCmdPage());
            m_uiPages.Add(new GM_WelfareCmdPage());
            m_uiPages.Add(new GM_LevelCmdPage());
            m_uiPages.Add(new GM_ExStoreCmdPage());
            m_uiPages.Add(new GM_SKillCmdPage());
            m_uiPages.Add(new GM_AttributePage());
            m_uiPages.Add(new GM_TaskCmdPage());
            m_uiPages.Add(new GM_TrumpsCmdPage());
            m_uiPages.Add(new GM_AppearancePage());
            //默认选中第一个分页
            ClickPage(0);
        }


        void ClickPage(int id)
        {
            if (m_curPage >= 0)
                m_uiPages[m_curPage].OnClose();

            m_uiPages[id].OnOpen();
            m_curPage = id;
        }


        void PageGUI(float w)
        {
            //titile
            GUILayout.BeginArea(m_titleRect);
            m_titleScrollPosition = GUILayout.BeginScrollView(m_titleScrollPosition, GUILayout.Width(m_titleWidth));
            for (int i = 0; i < m_uiPages.Count; i++)
            {
                if (GUILayout.Button(m_uiPages[i].GetTitle()))
                {
                    ClickPage(i);
                    break;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            //page
            if (m_curPage < m_uiPages.Count && m_curPage >= 0)
                m_uiPages[m_curPage].OnGUI(m_pageRect);
        }


    }

}
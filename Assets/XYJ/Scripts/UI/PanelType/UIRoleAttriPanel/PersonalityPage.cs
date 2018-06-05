#if !USE_HOT
using Config;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using System;

namespace xys.hot.UI
{
    namespace RoleAttri
    {
        class PersonalityPage : HotTablePageBase
        {
            PersonalityPage() : base(null) { }
            PersonalityPage(HotTablePage page) : base(page)
            {

            }

            [SerializeField]
            protected Text m_Title;
            [SerializeField]
            protected Transform m_ItemGrid;
            [SerializeField]
            [PackTool.Pack]
            protected GameObject m_ItemPrefab;

            [SerializeField]
            UIRoleAttriPerCittaSkillTips m_CittaSkillTips;

            protected override void OnInit()
            {

            }

            protected override void OnShow(object args)
            {
                ShowTitle();
                SetPersonality();
            }

            protected override void OnHide()
            {
                m_CittaSkillTips.OnHide();
            }

            void ShowTitle()
            {
                string title = string.Empty;
                for (int i = 0; i < 3; i++)
                {
                    if (i != 0) title += "、";
                    title += PersonalityDefine.Get(i * 2 + 1).name;
                }
                m_Title.text = "当前个性" + title;
            }

            void SetPersonality()
            {
                m_ItemPrefab.SetActive(false);

                List<Transform> gridChildren = new List<Transform>();
                foreach (Transform child in m_ItemGrid)
                {
                    gridChildren.Add(child);
                }

                int gridIndex = 0;
                for (; gridIndex < 3; gridIndex++)
                {
                    GameObject go = null;
                    if (gridIndex < gridChildren.Count)
                    {
                        go = gridChildren[gridIndex].gameObject;
                    }
                    else
                    {
                        go = GameObject.Instantiate(m_ItemPrefab);
                        go.transform.SetParent(m_ItemGrid, false);
                        go.transform.localScale = Vector3.one;

                    }
                    go.name = (gridIndex * 2 + 1).ToString();
                    go.SetActive(true);
                }
                for (; gridIndex < gridChildren.Count; gridIndex++)
                {
                    gridChildren[gridIndex].gameObject.SetActive(false);
                }
            }
        }
    }
}
#endif
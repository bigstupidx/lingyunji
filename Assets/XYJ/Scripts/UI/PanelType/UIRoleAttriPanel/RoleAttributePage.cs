#if !USE_HOT
using Config;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using UnityEngine.EventSystems;
using xys.battle;

namespace xys.hot.UI
{
    namespace RoleAttri
    {
        class RoleAttributePage : HotTablePageBase
        {
            RoleAttributePage() : base(null) { }
            public RoleAttributePage(HotTablePage page) : base(page) { }
            [SerializeField]
            StateToggle m_Toggle;

            [SerializeField]
            StateToggle m_ToggleMainAttri;

            [SerializeField]
            Transform m_root;
            [SerializeField]
            Transform m_BasePageRoot;
            [SerializeField]
            Transform m_HigherPageRoot;
            [SerializeField]
            Transform m_AttriTips;

            [SerializeField]
            Button m_VitalityBtn;
            [SerializeField]
            Button m_AddBtn;

            [SerializeField]
            ILMonoBehaviour m_ILScrollView;
            UIRoleAttriScrollView m_ScrollView;

            protected Dictionary<int, Text> attriTextDic = new Dictionary<int, Text>();
            protected int m_ClickHandlerId = 0;
            BattleAttri battleAttri = new BattleAttri();

            protected override void OnInit()
            {
                m_Toggle.OnSelectChange = OnChange;
                m_Toggle.OnPreChange = PreChange;

                m_VitalityBtn.onClick.AddListener(() => { OnVitalityBtnClick(); });
                m_AddBtn.onClick.AddListener(() => { OnAddBtnClick(); });

                if (m_ILScrollView != null) m_ScrollView = (UIRoleAttriScrollView)m_ILScrollView.GetObject();

                m_AttriTips.gameObject.SetActive(false);
            }

            protected override void OnShow(object args)
            {
                Event.Subscribe(EventID.LocalBattleAttChange, ShowBasePage);
                Event.Subscribe(EventID.LocalBattleAttChange, RefreshScrollViewUI);
                ShowBasePage();
            }

            protected override void OnHide()
            {
                m_AttriTips.gameObject.SetActive(false);
                attriTextDic.Clear();
            }

            bool PreChange(StateRoot sr, int index)
            {
                return true;
            }

            void OnChange(StateRoot sr, int index)
            {
                if (index == 0)
                {
                    ShowBasePage();
                }
                else
                {
                    ShowHigherPage();
                }
            }

            private void OnVitalityBtnClick()
            {
                //打开积分界面 活力页签部分
                //xys.App.my.uiSystem.ShowPanel("", new object() { }, true);
            }

            private void OnAddBtnClick()
            {
                //打开血量储备界面
                //xys.App.my.uiSystem.ShowPanel("", new object() { }, true);
            }

            void ShowBasePage()
            {
                if (App.my != null && App.my.localPlayer != null)
                    battleAttri = App.my.localPlayer.uiShowBattleAttri;

                m_BasePageRoot.Find("BaseInfo/Clan").GetComponent<Text>().text = "无";
                m_BasePageRoot.Find("BaseInfo/Num").GetComponent<Text>().text = App.my.localPlayer.charid.ToString();
                m_BasePageRoot.Find("BaseInfo/Grade").GetComponent<Text>().text = "0";
                m_BasePageRoot.Find("BaseInfo/Vitality").GetComponent<Text>().text = "0";

                m_BasePageRoot.Find("BaseInfo/LifeBg/Life").GetComponent<Image>().fillAmount = (int)battleAttri.Get(AttributeDefine.iHp) * 100 / App.my.localPlayer.maxHpValue * 0.01f;
                m_BasePageRoot.Find("BaseInfo/LifeBg/Text").GetComponent<Text>().text = string.Format("{0}/{1}", (int)battleAttri.Get(AttributeDefine.iHp), App.my.localPlayer.maxHpValue);

                int level = App.my.localPlayer.levelValue;
                int maxExp = RoleExp.Get(level <= 0 ? 1 : level).player_exp;
                m_BasePageRoot.Find("BaseInfo/ExpBg/Exp").GetComponent<Image>().fillAmount = App.my.localPlayer.expValue * 100 / maxExp * 0.01f;
                m_BasePageRoot.Find("BaseInfo/ExpBg/Text").GetComponent<Text>().text = string.Format("{0}/{1}", (int)App.my.localPlayer.expValue, maxExp);

                List<Transform> upChildren = GetChildren(m_BasePageRoot.Find("BaseAttributeUp"));
                int upIndex = 0;
                SetAttribute(upChildren[upIndex], AttributeDefine.iStrength, ref upIndex);
                SetAttribute(upChildren[upIndex], AttributeDefine.iIntelligence, ref upIndex);
                SetAttribute(upChildren[upIndex], AttributeDefine.iBone, ref upIndex);
                SetAttribute(upChildren[upIndex], AttributeDefine.iPhysique, ref upIndex);
                SetAttribute(upChildren[upIndex], AttributeDefine.iAgility, ref upIndex);
                SetAttribute(upChildren[upIndex], AttributeDefine.iBodyway, ref upIndex);

                List<Transform> downChildren = GetChildren(m_BasePageRoot.Find("BaseAttributeDown"));
                int downIndex = 0;
                SetAttribute(downChildren[downIndex], AttributeDefine.iPhysicalAttack, ref downIndex);
                SetAttribute(downChildren[downIndex], AttributeDefine.iMagicAttack, ref downIndex);
                SetAttribute(downChildren[downIndex], AttributeDefine.iPhysicalDefense, ref downIndex);
                SetAttribute(downChildren[downIndex], AttributeDefine.iMagicDefense, ref downIndex);
                SetAttribute(downChildren[downIndex], AttributeDefine.iCritLevel, ref downIndex);
                SetAttribute(downChildren[downIndex], AttributeDefine.iHitLevel, ref downIndex);
                SetAttribute(downChildren[downIndex], AttributeDefine.iParryLevel, ref downIndex);
                SetAttribute(downChildren[downIndex], AttributeDefine.iAvoidLevel, ref downIndex);

                SetMainAttri();
                SetZhenQi((int)battleAttri.Get(AttributeDefine.iMp));
            }

            void SetMainAttri()
            {
                m_ToggleMainAttri.startSelect = RoleChangeAttr.GetMainAttriId(App.my.localPlayer.carrerValue) - 1;
            }

            //设置真气值
            void SetZhenQi(int value)
            {
                Transform zhenQiGrid = m_BasePageRoot.Find("BaseInfo/ZhanQiContent");
                List<Transform> gridChildren = GetChildren(zhenQiGrid);
                for (int i = 0; i < gridChildren.Count; i++)
                {
                    gridChildren[i].gameObject.SetActive(value > i ? true : false);
                }
            }

            void ShowHigherPage()
            {
                if (m_ILScrollView.gameObject.activeSelf)
                    parent.StartCoroutine(this.m_ScrollView.Show());
            }

            void SetAttribute(Transform nRoot, int attriId, ref int index, string name = "")
            {
                nRoot.Find("Name").GetComponent<Text>().text = name != "" ? name : AttributeDefine.GetTitleStr(attriId);
                nRoot.GetComponent<Button>().onClick.AddListener(() => { OnAttriItemClick(attriId, nRoot); });

                if (!attriTextDic.ContainsKey(attriId))
                    attriTextDic.Add(attriId, nRoot.Find("value").GetComponent<Text>());

                RefreshItemValue(attriId, battleAttri.Get(attriId));

                index++;
            }

            void RefreshItemValue(int id, double value)
            {
                if (attriTextDic.ContainsKey(id))
                    attriTextDic[id].text = AttributeDefine.GetValueStr(id, value);
            }

            public void OnAttriItemClick(int id, Transform tf)
            {
                Transform text = m_AttriTips.Find("panel/Text");
                text.GetComponent<Text>().text = AttributeDefine.GetAttriTips(id, App.my.localPlayer.carrerValue, battleAttri);

                RectTransform attriTipsRectTf = m_AttriTips.GetComponent<RectTransform>();
                attriTipsRectTf.sizeDelta = new Vector2(attriTipsRectTf.sizeDelta.x, text.GetComponent<RectTransform>().sizeDelta.y + 60);
                m_AttriTips.transform.position = tf.position;
                attriTipsRectTf.anchoredPosition -= new Vector2(attriTipsRectTf.sizeDelta.x + 10, -attriTipsRectTf.sizeDelta.y / 2);

                m_AttriTips.gameObject.SetActive(true);
                PlayAnimation(true);
            }

            void PlayAnimation(bool isOpen)
            {
                if (isOpen)
                    AnimationHelp.PlayAnimation(m_AttriTips.GetComponent<Animator>(), "open", "ui_TanKuang_Tips", OpenEvent);
                else
                    AnimationHelp.PlayAnimation(m_AttriTips.GetComponent<Animator>(), "close", "ui_TanKuang_Tips_Close", CloseEvent);
            }

            void OpenEvent(object obj)
            {
                if (m_ClickHandlerId != 0)
                    EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
                m_ClickHandlerId = EventHandler.pointerClickHandler.Add(OnGlobalClick);
            }

            void CloseEvent(object obj)
            {
                EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
                m_ClickHandlerId = 0;
                m_AttriTips.gameObject.SetActive(false);
            }

            protected bool OnGlobalClick(GameObject go, BaseEventData bed)
            {
                if (go == null || !go.transform.IsChildOf(m_AttriTips))
                {
                    PlayAnimation(false);
                    return false;
                }
                return true;
            }

            List<Transform> GetChildren(Transform tr)
            {
                List<Transform> children = new List<Transform>();
                foreach (Transform child in tr)
                {
                    children.Add(child);
                }
                return children;
            }

            void RefreshScrollViewUI()
            {
                if (m_ILScrollView.gameObject.activeSelf)
                    m_ScrollView.RefreshColumData();
            }
        }
    }
}
#endif
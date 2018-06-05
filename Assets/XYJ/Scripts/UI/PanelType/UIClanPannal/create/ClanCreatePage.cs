#if !USE_HOT
using Config;
using NetProto;
using NetProto.Hot;
using System;
using System.Collections.Generic;
using System.Text;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class ClanCreatePage : HotTablePageBase
    {

        [SerializeField]
        Transform m_CreateBtnTransform;
        [SerializeField]
        InputField m_ClanNameInput;
        [SerializeField]
        InputField m_ClanDec;
        [SerializeField]
        Button m_helpBtn;
        [SerializeField]
        Button m_CreateBtn;
        [SerializeField]
        Transform m_tipstransform;

        [SerializeField]
        InputField m_clanFlagInput;
        [SerializeField]
        Button m_selectColorBtn;
        [SerializeField]
        Text m_flagTxt;

        private int m_flagColorId;
        ClanCreatePage() : base(null) { }
        public ClanCreatePage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            m_helpBtn.onClick.AddListenerIfNoExist(() => { this.OnClickShowHelp(); });

            m_CreateBtn.onClick.AddListenerIfNoExist(() => { this.OnClickCreate(); });

            m_selectColorBtn.onClick.AddListenerIfNoExist(() => { this.OnClickSelectColor(); });

            m_clanFlagInput.onEndEdit.AddListener((string str)=> { this.OnEndEdit(str); });

            m_flagColorId = 0;
        }

        protected override void OnShow(object args)
        {
            m_flagColorId = 0;
        }


        public void OnClickShowHelp(object args = null)
        {
            //static bool isActive = true;

            m_tipstransform.gameObject.SetActive(true);

            xys.UI.EventHandler.pointerClickHandler.Add(OnClickHideHelp);
        }

        public bool OnClickHideHelp(GameObject obj, BaseEventData bed)
        {
            if (m_tipstransform.gameObject.activeSelf)
            {
                if (obj == null || obj == m_tipstransform.Find("Bg2").gameObject || !obj.transform.IsChildOf(m_tipstransform))
                {
                    m_tipstransform.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;
        }

        public void OnEndEdit(string str)
        {
            if (str == "")
            {
                SystemHintMgr.ShowHint("氏族旗帜不能为空");
                return;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if ((int)m_clanFlagInput.text[i] > 127)
                {

                }
                else
                {
                    SystemHintMgr.ShowHint("氏族旗帜只能是单字中文。");
                    return;
                }
            }

            m_flagColorId = m_flagColorId == 0 ? 1: m_flagColorId;
            SetTextColor(m_flagColorId);
        }
        public void OnClickCreate(object args = null)
        {
            if (m_ClanNameInput.text == "")
            {
                SystemHintMgr.ShowHint("氏族名称不能为空");
                return;
            }

            if (m_clanFlagInput.text == "")
            {
                SystemHintMgr.ShowHint("氏族旗帜不能为空");
                return;
            }
            else
            {
                for (int i = 0; i < m_clanFlagInput.text.Length; i++)
                {
                    if ((int)m_clanFlagInput.text[i] > 127)
                    {
                       
                    }
                    else
                    {
                        SystemHintMgr.ShowHint("氏族旗帜只能是单字中文。");
                        return;
                    }
                }
            }
            
            if (m_flagColorId <= 0)
            {
                SystemHintMgr.ShowHint("请选择一种氏族旗帜颜色。");
                return;
            }
            
            /*
            if (App.my.localPlayer.silverShellValue < 100000)
            {
                SystemHintMgr.ShowHint("创建氏族需要100000银贝");
            }
            */
            CreateData data = new CreateData();
            data.name = m_ClanNameInput.text;
            data.declaration = m_ClanDec.text != "" ? m_ClanDec.text : m_ClanDec.placeholder.transform.GetComponent<Text>().text;
            data.flag = m_clanFlagInput.text;
            data.colorid = m_flagColorId;
            App.my.eventSet.FireEvent<CreateData>(EventID.Clan_Create, data);
        }
        protected override void OnHide()
        {
            m_flagColorId = 0;
        }

        public void OnClickSelectColor(object args = null)
        {
            if (m_clanFlagInput.text == "")
            {
                SystemHintMgr.ShowHint("请先输入氏族旗帜单字名称。");
                return;
            }
            Dictionary<int, GameClanFlagColor> allConfig = GameClanFlagColor.GetAll();
            m_flagColorId++;

            if (m_flagColorId > allConfig.Count)
            {
                m_flagColorId = 1;
            }

            SetTextColor(m_flagColorId);

        }

        public void SetTextColor(int colorId)
        {
            Dictionary<int, GameClanFlagColor> allConfig = GameClanFlagColor.GetAll();
            if (colorId >= allConfig.Count)
            {
                return;
            }
            if (allConfig[colorId].mainColor.Length == 6)
            {
                float r = 0;
                float g = 0;
                float b = 0;
                StringToRgb(allConfig[colorId].mainColor, out r, out g, out b);
                m_clanFlagInput.textComponent.color = new Color(r / 255f, g / 255f, b / 255f);
            }

        }


        //必须是16进制颜色str  比如 FFFFFF
        public void StringToRgb(string Str, out float r, out float g, out float b)
        {
            string str1 = Str.Substring(0, 2);
            r = Convert.ToInt32(str1, 16);

            string str2 = Str.Substring(2, 2);
            g = Convert.ToInt32(str2, 16);

            string str3 = Str.Substring(4, 2);
            b = Convert.ToInt32(str3, 16);
        }
    }
}
#endif
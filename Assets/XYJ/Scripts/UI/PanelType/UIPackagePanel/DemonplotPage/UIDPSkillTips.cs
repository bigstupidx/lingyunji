#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using NetProto;
    using Config;
    using xys.UI;
    using System.Collections.Generic;
    using NetProto.Hot;

    [Serializable]
    partial class UIDPSkillTips
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Image m_SkillIcon;
        [SerializeField]
        Text m_SkillName;
        [SerializeField]
        Text m_SkillLv;
        [SerializeField]
        Text m_SkillMax;
        [SerializeField]
        Text m_ExpText;
        [SerializeField]
        Image m_ExpIma;
        [SerializeField]
        float m_ExpLength;
        [SerializeField]
        Transform m_CostRoot;
        [SerializeField]
        Button m_AddBtn;
        [SerializeField]
        Button m_AddMoreBtn;
        [SerializeField]
        Button m_UseBtn;
        [SerializeField]
        Button m_CloseBtn;
        [SerializeField]
        Button m_TipsClostBtn;
        //
        [SerializeField]
        Text m_MarName;
        [SerializeField]
        Image m_MarIcon;
        [SerializeField]
        Text m_MarCount;
        [SerializeField]
        GameObject m_MarMark;
        [SerializeField]
        Image m_MarQuality;

        int m_SkillLimit;//根据等级显示当前是用哪本手册

        DemonplotSkillType m_SkillType;
        DemonplotsMgr m_Mgr;
        public bool isActive { get { return m_Transform.gameObject.activeSelf; } }

        public void OnInit()
        {
            m_Mgr = (App.my.localPlayer.GetModule<DemonplotsModule>().demonplotMgr as DemonplotsMgr);
            m_AddBtn.onClick.AddListener(()=> { this.CostUseEvent(DemonplotAddExpType.oneTimes); });
            m_AddMoreBtn.onClick.AddListener(() => { this.CostUseEvent(DemonplotAddExpType.tenTimes); });
            m_UseBtn.onClick.AddListener(this.ItemUseEvent);
            m_SelecteItemBtn.onClick.AddListener(this.ShowTipsEvent);
            m_CloseBtn.onClick.AddListener(() => { this.m_Transform.gameObject.SetActive(false); });
            m_TipsClostBtn.onClick.AddListener(this.HideSelecteTips);
            m_SkillLimit = int.Parse(kvClient.Get("DemonplotSkillLimit").value);
        }

        public void OnHide()
        {
            this.m_Transform.gameObject.SetActive(false);
        }

        public void Set(DemonplotSkillType type, DemonplotSkillData skillData)
        {
            if (!DemonplotSkill.GetAll().ContainsKey(type))
            {
                this.m_Transform.gameObject.SetActive(false);
                return;
            }
            if(!DemonplotSkillExp.GetAll().ContainsKey(skillData.lv))
            {
                this.m_Transform.gameObject.SetActive(false);
                return;
            }
            this.m_Transform.gameObject.SetActive(true);

            DemonplotSkill skillProperty = DemonplotSkill.Get(type);
            DemonplotSkillExp expData = DemonplotSkillExp.Get(skillData.lv);
            Helper.SetSprite(m_SkillIcon, skillProperty.icon);
            m_SkillName.text = skillProperty.name;
            m_SkillLv.text = skillData.lv + "级";
            m_SkillMax.text = "等级上限：" + DemonplotSkillExp.GetAll().Count;
            m_ExpText.text = skillData.exp + "/" + expData.exp;

            m_ExpIma.rectTransform.sizeDelta = new Vector2((float)(skillData.exp) / (float)(expData.exp) * m_ExpLength, m_ExpIma.rectTransform.sizeDelta.y);
            //材料设置 
            string res = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", expData.familyvalue > App.my.localPlayer.familyValue ? "R1" : "G2", expData.familyvalue.ToString()));
            m_CostRoot.GetChild(0).Find("Cost").GetComponent<Text>().text = res;
            res = GlobalSymbol.ToBef(App.my.localPlayer.familyValue);
            m_CostRoot.GetChild(1).Find("Cost").GetComponent<Text>().text = res;
            res = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", expData.xiuwei > App.my.localPlayer.xiuWeiValue ? "R1" : "G2", expData.xiuwei.ToString()));
            m_CostRoot.GetChild(2).Find("Cost").GetComponent<Text>().text = res;
            res = GlobalSymbol.ToBef(App.my.localPlayer.xiuWeiValue);
            m_CostRoot.GetChild(3).Find("Cost").GetComponent<Text>().text = res;

            m_SkillType = type;

            this.InitExpItem();
        }

        #region 事件
        void CostUseEvent(DemonplotAddExpType addType)
        {
            if (m_SelectedItemId == 0)
                return;
            DemonplotCurrencyRequest dcr = new DemonplotCurrencyRequest();
            dcr.type = addType;
            dcr.skilltype = (int)m_SkillType;
            App.my.eventSet.FireEvent(EventID.Demonplot_AddExpCurrency, dcr);
        }

        void ItemUseEvent()
        {
            if (m_SelectedItemId == 0)
                return;
            if (hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(m_SelectedItemId) != 0)
            {
                //检查等级限制
                int lv = 1;
                if (m_Mgr.m_Tables.skills.ContainsKey((int)m_SkillType))
                    lv = m_Mgr.m_Tables.skills[(int)m_SkillType].lv;
                if(DemonplotSkillExp.CheckLv(lv, App.my.localPlayer.levelValue))
                {
                    SystemHintMgr.ShowHint(TipsContent.GetByName("demonplot_skill_limit_error").des);
                    return;
                }
                //
                DemonplotSkillRequest dsr = new DemonplotSkillRequest();
                dsr.itemid = m_SelectedItemId;
                dsr.skilltype = (int)m_SkillType;
                App.my.eventSet.FireEvent(EventID.Demonplot_AddExp, dsr);
            }
            else
                this.ShowItemTipsEvent(m_SelectedItemId);
        }

        void ShowTipsEvent()
        {
            this.ShowSelectTips();
        }
        #endregion
    }
}
#endif
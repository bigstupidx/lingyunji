#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using UnityEngine.UI;
    using NetProto;
    using NetProto.Hot;

    [System.Serializable]
    class UIDPInfos
    {
        [SerializeField]
        UIDPItemScrollView m_ItemScrollView;
        [SerializeField]
        UIDPItemInfos m_ItemInfos;
        [SerializeField]
        UIDPSkillTips m_SkillTips;

        [SerializeField]
        Image m_Icon;
        [SerializeField]
        Text m_SkillName;
        [SerializeField]
        Text m_SkillLv;
        [SerializeField]
        Text m_SKillLvMax;
        [SerializeField]
        Text m_SkillDes;
        [SerializeField]
        Text m_SkillTipsDes;
        [SerializeField]
        Text m_ExpText;
        [SerializeField]
        Image m_ExpIma;
        [SerializeField]
        float m_ExpLength;

        [SerializeField]
        Button m_AddBtn;

        DemonplotsModule m_Module;
        DemonplotSkillType m_SkillType;
        public void OnInit()
        {
            m_Module = App.my.localPlayer.GetModule<DemonplotsModule>();
            m_SkillTips.OnInit();
            m_ItemInfos.OnInit();
            m_ItemScrollView.OnInit();
            m_ItemScrollView.selectedCallback = this.ResetItemInfos;

            m_AddBtn.onClick.AddListener(this.ShowSkillTipsEvent);
        }

        public void OnHide()
        {
            m_SkillTips.OnHide();
            m_ItemInfos.OnHide();
        }

        public void ResetPage(DemonplotSkillType type)
        {
            this.ResetUI(type);

            if (DemonplotProperty.m_ItemGroup.ContainsKey(type))
            {
                m_ItemScrollView.Create(DemonplotProperty.m_ItemGroup[type]);
            }
            else if(DemonplotProperty.m_DemonGroup.ContainsKey(type))
            {
                m_ItemScrollView.Create(DemonplotProperty.m_DemonGroup[type]);
            }
            //如果小面板打开，刷新小面板数据
            if (m_SkillTips.isActive)
                this.ShowSkillTipsEvent();
        }

        void ResetItemInfos()
        {
            this.m_ItemInfos.Set(DemonplotProperty.Get(m_ItemScrollView.selectedID));
        }

        void ResetUI(DemonplotSkillType type)
        {
            DemonplotsTable skillTables = ((DemonplotsMgr)m_Module.demonplotMgr).m_Tables;
            DemonplotSkill skillproperty = DemonplotSkill.Get(type);
            DemonplotSkillData skillData = null;
            if (skillTables.skills.ContainsKey((int)type))
                skillData = skillTables.skills[(int)type];
            else
                skillData = (m_Module.demonplotMgr as DemonplotsMgr).NewSkillData((int)type);
            DemonplotSkillExp expData = DemonplotSkillExp.Get(skillData.lv);

            Helper.SetSprite(m_Icon, skillproperty.icon);
            this.m_SkillName.text = skillproperty.name + "：";
            this.m_SkillLv.text = skillData.lv + "级";
            this.m_SKillLvMax.text = "等级上限：" + DemonplotSkillExp.GetAll().Count.ToString();
            this.m_SkillTipsDes.text = "可" + skillproperty.matchinname.Replace(" ",string.Empty) + "物品";
            this.m_SkillDes.text = skillproperty.des;
            this.m_ExpText.text = skillData.exp + "/" + expData.exp;
            this.m_ExpIma.rectTransform.sizeDelta = new Vector2((float)(skillData.exp) / (float)(expData.exp) * m_ExpLength, this.m_ExpIma.rectTransform.sizeDelta.y);

            m_SkillType = type;
    }

        void ShowSkillTipsEvent()
        {
            DemonplotsTable skillTables = ((DemonplotsMgr)m_Module.demonplotMgr).m_Tables;
            DemonplotSkillData skillData = null;
            if (skillTables.skills.ContainsKey((int)m_SkillType))
                skillData = skillTables.skills[(int)m_SkillType];
            else
                skillData = (m_Module.demonplotMgr as DemonplotsMgr).NewSkillData((int)m_SkillType);
            m_SkillTips.Set(m_SkillType, skillData);
        }
    }
}
#endif
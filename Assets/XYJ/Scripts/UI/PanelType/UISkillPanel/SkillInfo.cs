#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using Config;
using xys.UI.Utility;
using xys.UI.State;
using xys.battle;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace xys.hot.UI
{
    [AutoILMono]
    class SkillInfo
    {
        class ZhenqiDecs
        {
            public string des = "";
            public int num = 0;
        }
        public enum InfoType
        {
            IT_First = 0,           //首要描述
            IT_Block,               //block事件描述
            IT_BlockSuc,            //block事件描述
            IT_Move,                //move事件描述
            IT_Omnislash,           //omnislash事件描述
            IT_AttackState,         //击中状态描述
            IT_Damage,              //伤害描述
            IT_Buff,                //buff描述
            IT_AniSpeed,            //技能快慢参数描述
            IT_Crit,                //附加暴击率描述
            IT_IgnoreDef,           //无视防御描述
            IT_Condition,           //条件判断效果描述
            IT_Vampire,             //吸血描述

            IT_All,                 //全部
        }
        [SerializeField]
        GameObject m_prompt;
        [SerializeField]
        GameObject m_content;
        [SerializeField]
        GameObject m_bottom;
        [SerializeField]
        GameObject m_scrollView;

        [SerializeField]
        Button m_compreBtn;         // 领悟按钮（底部）
        [SerializeField]
        StateRoot m_compreState;    // 领悟按钮状态（右上角）
        [SerializeField]
        Text m_name;
        [SerializeField]
        Text m_level;
        [SerializeField]
        Text m_grade;
        [SerializeField]
        Text m_zhenQi;
        [SerializeField]
        Text m_zhenQiN;
        [SerializeField]
        Text m_castingTime;
        [SerializeField]
        Text m_castingTimeN;
        [SerializeField]
        Text m_gunshot;
        [SerializeField]
        Text m_gunshotN;
        [SerializeField]
        Text m_range;
        [SerializeField]
        Text m_rangeN;
        [SerializeField]
        Text m_cd;
        [SerializeField]
        Text m_cdN;
        [SerializeField]
        Text m_useCondition;
        [SerializeField]
        Text m_bDes;

        int m_curSkillPoint = 0;
        int m_talentBookId = 0;
        SkillConfig m_actSkill = null;          // 激活技能
        SkillConfig m_chooseSkill = null;       // 选中技能
        SkillConfig m_conSkill = null;          // 对比技能

        private void Awake()
        {
            m_compreState.onStateChange.AddListener(CompreChange);
        }

        private void CompreChange()
        {
            if (m_chooseSkill == null) return;
            switch (m_compreState.CurrentState)
            {
                // 领悟查看状态 灰
                case 0:
                    ShowSkill(m_chooseSkill);
                    ShowWithoutConPart(m_chooseSkill);
                    if (m_actSkill == m_chooseSkill)
                    {
                        // 显示当前选择（激活）技能信息
                        ShowPureText(m_chooseSkill.id);
                    }
                    else
                    {
                        // 当前选择技能信息与激活信息对比
                        ContrastSkill(m_actSkill, m_chooseSkill);
                        ConPureText(m_actSkill.id, m_chooseSkill.id);
                    }
                    FitUI();
                    break;

                // 领悟查看状态 亮
                case 1:
                    // 当前选择天赋终极和激活信息对比
                    SkillTalentConfig stc = SkillTalentConfig.GetByKeyAndId(m_curSkillPoint, m_chooseSkill.id);
                    if (stc != null && stc.advanceSkillId != 0)
                    {
                        SkillConfig advanceSkill = SkillConfig.Get(stc.advanceSkillId);
                        if (advanceSkill == null) return;
                        ShowSkill(m_actSkill);
                        ShowWithoutConPart(advanceSkill);
                        ContrastSkill(m_actSkill, advanceSkill);
                        ConPureText(m_actSkill.id, advanceSkill.id);
                        FitUI();
                    }
                    break;
            }
        }

        public void SetEmpty()
        {
            if (m_prompt != null) m_prompt.SetActive(true);
            m_content.SetActive(false);
        }

        public void RefreshInfo(int skillPointId, int skillId, int activeSkillId = 0)
        {
            SkillConfig skill = SkillConfig.Get(skillId);
            if (skill == null) return;
            m_chooseSkill = skill;
            m_curSkillPoint = skillPointId;
            // 领悟查看按钮 领悟按钮
            m_compreState.CurrentState = 0;
            m_compreState.gameObject.SetActive(false);
            m_compreBtn.gameObject.SetActive(false);

            SkillTalentConfig stc = SkillTalentConfig.GetByKeyAndId(skillPointId, skillId);
            if (stc != null)
            {
                m_talentBookId = stc.bookId;
                if (stc.advanceSkillId != 0)
                {
                    m_compreState.gameObject.SetActive(true);
                    if (m_chooseSkill.openLevel <= App.my.localPlayer.levelValue)
                    {
                        m_compreBtn.gameObject.SetActive(true);
                        m_compreBtn.onClick.RemoveAllListeners();
                        m_compreBtn.onClick.AddListener(() => { Comprehend(); });
                    }
                }
            }

            if (activeSkillId != 0)
            {
                SkillConfig actSkill = SkillConfig.Get(activeSkillId);
                if (actSkill == null) return;
                m_actSkill = actSkill;
            }
            else m_actSkill = m_chooseSkill;
            CompreChange();
        }

        // 不做对比部分
        void ShowWithoutConPart(SkillConfig skill)
        {
            m_name.text = skill.name;
            m_bDes.text = skill.bDes;
            SkillTalentConfig stc = SkillTalentConfig.GetByKeyAndId(m_curSkillPoint, skill.id);
            if (stc != null)
            {
                // 天赋等级
                m_level.text = stc.gradeDes;
                m_level.gameObject.SetActive(string.IsNullOrEmpty(stc.gradeDes) ? false : true);
            }
        }

        public void ShowSkill(SkillConfig skill)
        {
            if (m_prompt != null) m_prompt.SetActive(false);
            m_content.SetActive(true);

            ZhenqiDecs zhenqiDecs = OnZhenqi(skill);
            m_zhenQi.text = zhenqiDecs.num > 0 ? zhenqiDecs.des + zhenqiDecs.num : "";

            m_castingTime.text = OnCastingTime(skill.castingTime);
            m_gunshot.text = OnGunshot(skill);
            m_range.text = OnSearchRange(skill.searchDes);
            m_cd.text = OnCD(skill.cd);

            OnUseCondition(GetCastingPostureDes(skill), GetListDes(GetFireConditionDesList(skill)));
            SetContrast(false);
        }

        public void SetContrast(bool state)
        {
            m_zhenQiN.gameObject.SetActive(state);
            m_castingTimeN.gameObject.SetActive(state);
            m_gunshotN.gameObject.SetActive(state);
            m_rangeN.gameObject.SetActive(state);
            m_cdN.gameObject.SetActive(state);
        }

        #region 带箭头对比部分 + 使用条件
        public void ContrastSkill(SkillConfig skill, SkillConfig skillCon)
        {
            ContrastPartZhenqi(skill, skillCon);
            ShowZhenqi();

            ContrastPartShow(m_castingTime, m_castingTimeN, OnCastingTime(skill.castingTime), OnCastingTime(skillCon.castingTime));
            ContrastPartShow(m_gunshot, m_gunshotN, OnGunshot(skill), OnGunshot(skillCon));
            ContrastPartShow(m_range, m_rangeN, OnSearchRange(skill.searchDes), OnSearchRange(skillCon.searchDes));
            ContrastPartShow(m_cd, m_cdN, OnCD(skill.cd), OnCD(skillCon.cd));

            OnUseCondition(skill, skillCon);
        }

        // 真气
        void ContrastPartZhenqi(SkillConfig skill, SkillConfig skillCon)
        {
            ZhenqiDecs zhenQi = OnZhenqi(skill);
            ZhenqiDecs zhenQiCon = OnZhenqi(skillCon);

            if (zhenQi.des == zhenQiCon.des)
            {
                // 真气消耗、恢复 前后两者均为空
                if (string.IsNullOrEmpty(zhenQi.des))
                {
                    m_zhenQi.text = "";
                    m_zhenQiN.text = "";
                    return;
                }
                //两者完全一样
                if (zhenQi.num == zhenQiCon.num)
                {
                    m_zhenQiN.text = "";
                    m_zhenQi.text = zhenQi.num > 0 ? zhenQi.des + zhenQi.num : "";
                    return;
                }
            }

            // 前者为空，后者不为空 前者和后者同文字值为0
            if (string.IsNullOrEmpty(zhenQi.des))
            {
                m_zhenQi.text = zhenQiCon.des + 0;
                m_zhenQiN.text = zhenQiCon.des + zhenQiCon.num;
                return;
            }
            // 后者为空，前者不为空 后者和前者同文字值为0
            if (string.IsNullOrEmpty(zhenQiCon.des))
            {
                m_zhenQi.text = zhenQi.des + zhenQi.num;
                m_zhenQiN.text = zhenQi.des + 0;
                return;
            }

            // 两者不一样
            m_zhenQi.text = zhenQi.des + zhenQi.num;
            m_zhenQiN.text = zhenQiCon.des + zhenQiCon.num;
        }

        void ShowZhenqi()
        {
            if (string.IsNullOrEmpty(m_zhenQiN.text))
            {
                m_zhenQiN.gameObject.SetActive(false);
                return;
            }

            string color = "#[Y2]{0}";
            m_zhenQi.text = GlobalSymbol.ToUT(string.Format(color, m_zhenQi.text));
            m_zhenQiN.text = GlobalSymbol.ToUT(string.Format(color, m_zhenQiN.text));
            m_zhenQiN.gameObject.SetActive(true);
        }

        ZhenqiDecs OnZhenqi(SkillConfig skill)
        {
            ZhenqiDecs desc = new ZhenqiDecs();
            if (skill.costZhenqi > 0)
            {
                desc.des = TipContentUtil.GenText("skill_zhenqiCost");
                desc.num = skill.costZhenqi;
            }
            else if (skill.zhenqiRec > 0)
            {
                desc.des = TipContentUtil.GenText("skill_zhenqiRecover");
                desc.num = skill.zhenqiRec;
            }
            return desc;
        }

        void ContrastPartShow(Text show, Text con, string showText, string conText)
        {
            if (showText == conText)
            {
                show.text = showText;
                return;
            }

            string color = "#[Y2]{0}";
            show.text = GlobalSymbol.ToUT(string.Format(color, showText));
            con.text = GlobalSymbol.ToUT(string.Format(color, conText));
            con.gameObject.SetActive(true);
        }

        // 施法
        string OnCastingTime(float value)
        {
            if (value > 0)
                return string.Format(TipContentUtil.GenText("skill_time"), value);
            return "即时";
        }

        // 射程
        string OnGunshot(SkillConfig skill)
        {
            return skill != null && skill.isNeedTarget ? string.Format(TipContentUtil.GenText("skill_range"), skill.range) : "原地";
        }

        // 范围
        string OnSearchRange(string value)
        {
            if (value != "")
            {
                SearchActionConfig sac = SearchActionConfig.Get(value);
                if (sac != null)
                {
                    if (sac.searchType == SearchActionConfig.SearchSharp.Rect)
                        return string.Format(TipContentUtil.GenText("skill_search_" + (int)sac.searchType), sac.searchPara[0], sac.searchPara[1]);
                    else
                        return string.Format(TipContentUtil.GenText("skill_search_" + (int)sac.searchType), sac.searchPara[0]);
                }
            }
            return "目标";
        }

        // 冷却
        string OnCD(float value)
        {
            if (value > 0)
                return string.Format(TipContentUtil.GenText("skill_time"), value);
            return "无";
        }

        #region 使用条件
        // 使用条件
        void OnUseCondition(string castingPostureDes, string fireCondition)
        {
            if (!string.IsNullOrEmpty(fireCondition))
                castingPostureDes += ("\n" + fireCondition);
            m_useCondition.text = castingPostureDes;
        }

        // 使用条件（施放姿态）
        string GetCastingPostureDes(SkillConfig skill)
        {
            if (skill.castingPosture.Length == 0)
                return "所有姿态可用";

            string first = "";
            for (int i = 0; i < skill.castingPosture.Length; i++)
            {
                PostureConfig pc = PostureConfig.Get(skill.castingPosture[i]);
                if (pc == null) continue;

                first += pc.name;
                if (i + 1 != skill.castingPosture.Length)
                    first += "、";
            }
            return first;
        }

        // 使用条件（施放条件）
        List<string> GetFireConditionDesList(SkillConfig skill)
        {
            if (skill.castingConditions == 0) return null;
            SkillConditionConfig scc = SkillConditionConfig.Get(skill.castingConditions);
            if (scc == null) return null;
            return new List<string>(scc.des);
        }

        // 使用条件（对比）
        void OnUseCondition(SkillConfig skill, SkillConfig skillCon)
        {
            string castingPostureDes = ConStyle(GetCastingPostureDes(skill), GetCastingPostureDes(skillCon));
            string fireCondition = OnConWithoutChange(GetFireConditionDesList(skill), GetFireConditionDesList(skillCon));
            OnUseCondition(castingPostureDes, fireCondition);
        }
        #endregion

        #endregion

        #region 纯文字信息部分
        // 单个技能纯文字信息显示
        void ShowPureText(int showSkillId)
        {
            ResetTextGo();
            SkillEffectDesConfig seac = SkillEffectDesConfig.Get(showSkillId);
            if (seac == null) return;

            m_bottom.transform.Find("Text_" + InfoType.IT_First).GetComponent<Text>().text = OnFirstDes(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_Block).GetComponent<Text>().text = OnBlockDes(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_BlockSuc).GetComponent<Text>().text = OnBlockSucDes(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_Move).GetComponent<Text>().text = OnMove(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_Omnislash).GetComponent<Text>().text = OnOmnislash(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_AttackState).GetComponent<Text>().text = OnAttackState(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_Damage).GetComponent<Text>().text = OnDamage(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_Buff).GetComponent<Text>().text = GetListDes(OnBuffDes(seac));
            m_bottom.transform.Find("Text_" + InfoType.IT_AniSpeed).GetComponent<Text>().text = OnAniSpeedDes(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_Crit).GetComponent<Text>().text = OnCritRateDes(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_IgnoreDef).GetComponent<Text>().text = OnIgnoreDefDes(seac);
            m_bottom.transform.Find("Text_" + InfoType.IT_Condition).GetComponent<Text>().text = GetListDes(OnConditionDes(seac));
            m_bottom.transform.Find("Text_" + InfoType.IT_Vampire).GetComponent<Text>().text = OnVampireDes(seac);
        }

        // 对比技能纯文字信息显示
        void ConPureText(int showSkillId, int conSkillId)
        {
            ResetTextGo();
            SkillEffectDesConfig seac = SkillEffectDesConfig.Get(showSkillId); if (seac == null) return;
            SkillEffectDesConfig seacCon = SkillEffectDesConfig.Get(conSkillId); if (seacCon == null) return;

            m_bottom.transform.Find("Text_" + InfoType.IT_First).GetComponent<Text>().text = OnFirstDes(seac);      //此项不作对比
            m_bottom.transform.Find("Text_" + InfoType.IT_Block).GetComponent<Text>().text = ConStyle(OnBlockDes(seac), OnBlockDes(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_BlockSuc).GetComponent<Text>().text = ConStyle(OnBlockSucDes(seac), OnBlockSucDes(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_Move).GetComponent<Text>().text = ConStyle(OnMove(seac), OnMove(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_Omnislash).GetComponent<Text>().text = ConStyle(OnOmnislash(seac), OnOmnislash(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_AttackState).GetComponent<Text>().text = ConStyle(OnAttackState(seac), OnAttackState(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_Damage).GetComponent<Text>().text = ConStyle(OnDamage(seac), OnDamage(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_Buff).GetComponent<Text>().text = OnConWithoutChange(OnBuffDes(seac), OnBuffDes(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_AniSpeed).GetComponent<Text>().text = ConStyle(OnAniSpeedDes(seac), OnAniSpeedDes(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_Crit).GetComponent<Text>().text = ConStyle(OnCritRateDes(seac), OnCritRateDes(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_IgnoreDef).GetComponent<Text>().text = ConStyle(OnIgnoreDefDes(seac), OnIgnoreDefDes(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_Condition).GetComponent<Text>().text = OnConWithoutChange(OnConditionDes(seac), OnConditionDes(seacCon));
            m_bottom.transform.Find("Text_" + InfoType.IT_Vampire).GetComponent<Text>().text = ConStyle(OnVampireDes(seac), OnVampireDes(seacCon));
        }

        void FitUI()
        {
            HideNullTextGo();
            m_bottom.transform.localPosition = Vector3.zero;
        }

        void HideNullTextGo()
        {
            for (int i = 0; i < (int)InfoType.IT_All; i++)
            {
                Text text = m_bottom.transform.Find("Text_" + (InfoType)i).GetComponent<Text>();
                text.gameObject.SetActive(string.IsNullOrEmpty(text.text) ? false : true);
            }
        }

        void ResetTextGo()
        {
            for (int i = 0; i < (int)InfoType.IT_All; i++)
            {
                Text text = m_bottom.transform.Find("Text_" + (InfoType)i).GetComponent<Text>();
                text.text = "";
                text.gameObject.SetActive(false);
            }
        }

        string ConStyle(string showText, string conText)
        {
            //新增
            if (string.IsNullOrEmpty(showText) && !string.IsNullOrEmpty(conText))
                return AddStyle(conText);

            //删除
            if (!string.IsNullOrEmpty(showText) && string.IsNullOrEmpty(conText))
                return DeleteStyle(showText);

            //更改
            if (!string.IsNullOrEmpty(showText) && showText != conText)
                return DeleteStyle(showText) + "\n" + ChangeStyle(conText);

            return showText;
        }

        string AddStyle(string text)
        {
            text = string.Format("#[G2]{0}（新增）#g", text);
            return text;
        }

        string DeleteStyle(string text)
        {
            text = string.Format("#[grey]#e{0}#g", text);
            return text;
        }

        string ChangeStyle(string text)
        {
            text = string.Format("#[Y2]{0}（更改）#g", text);
            return text;
        }

        // 首要描述
        string OnFirstDes(SkillEffectDesConfig sedc)
        {
            return string.Format(TipContentUtil.GenText("skill_first_des"), sedc.firstDes);
        }

        // block
        string OnBlockDes(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.blockDes)) return "";
            BlockActionConfig bac = BlockActionConfig.Get(sedc.blockDes);
            if (bac != null)
                return bac.des;
            return "";
        }

        // block成功
        string OnBlockSucDes(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.blockSucDes)) return "";
            BlockActionConfig bac = BlockActionConfig.Get(sedc.blockSucDes);
            if (bac != null)
                return bac.SucDes;
            return "";
        }

        // move
        string OnMove(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.moveDes)) return "";
            MoveActionConfig mac = MoveActionConfig.Get(sedc.moveDes);
            if (mac != null)
                return mac.des;
            return "";
        }

        // omnislash
        string OnOmnislash(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.omnislashDes)) return "";
            OmnislashActionConfig oac = OmnislashActionConfig.Get(sedc.omnislashDes);
            if (oac != null)
                return oac.des;
            return "";
        }

        // 击中状态
        string OnAttackState(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.stateDes)) return "";
            AttackActionConfig aac = AttackActionConfig.Get(sedc.stateDes);
            if (aac != null)
            {
                if (aac.stateName == "浮空")
                    return TipContentUtil.GenText("skill_state_float_des");
                else if (aac.stateName == "击中")
                    return string.Format(TipContentUtil.GenText("skill_state_attack_des"), aac.statePara[1]);
                else
                    return string.Format(TipContentUtil.GenText("skill_state_des"), aac.stateName, aac.statePara[0]);
            }
            return "";
        }

        // 伤害
        string OnDamage(SkillEffectDesConfig sedc)
        {
            if (sedc.damageDes.Length <= 1 && string.IsNullOrEmpty(sedc.damageDes[0])) return "";

            bool isGetPanelAttack = false;                  // 是否已获取面板攻击力
            double magicDamageValue = 0;                    // 面板攻击力
            string damageType = "";                         // 伤害类型
            float damageFixAll = 0;                         // 伤害系数
            float extraDamageFixAll = 0;                    // 附加伤害系数
            float extraDamageAll = 0;                       // 附加伤害
            for (int i = 0; i < sedc.damageDes.Length; i++)
            {
                if (string.IsNullOrEmpty(sedc.damageDes[i])) continue;
                AttackActionConfig aac = AttackActionConfig.Get(sedc.damageDes[i]);
                if (aac != null)
                {
                    if (!isGetPanelAttack)
                    {
                        BattleAttri battleAttri = new BattleAttri();
                        if (App.my != null && App.my.localPlayer != null)
                            battleAttri = App.my.localPlayer.uiShowBattleAttri;

                        magicDamageValue = aac.isMagicDamage ? battleAttri.Get(AttributeDefine.iMagicAttack) : battleAttri.Get(AttributeDefine.iPhysicalAttack);
                        damageType = aac.isMagicDamage ? "法术" : "物理";
                        isGetPanelAttack = true;
                    }
                    damageFixAll += aac.damageFix;
                    extraDamageFixAll += aac.extraDamageFix;
                    extraDamageAll += aac.extraDamage;
                }
            }

            // 附加伤害系数* 技能等级标准伤害
            double addDamage = extraDamageFixAll * SkillDamageConfig.Get(1).damage;
            // 伤害值 = 伤害系数 * 面板攻击力 + 附加伤害系数 * 技能等级标准伤害 + 附加伤害 
            double damageValue = damageFixAll * magicDamageValue + addDamage + extraDamageAll;
            // (括号) 伤害系数（百分比）+ 附加伤害系数 * 技能等级标准伤害
            string damageFixStr = damageFixAll * 100 + "%" + ((int)addDamage > 0 ? "+" + (int)addDamage : "");
            string value = string.Format("{0}({1})", (int)damageValue, damageFixStr);
            return string.Format(TipContentUtil.GenText("skill_damage_des"), value, damageType);
        }

        // buff
        List<string> OnBuffDes(SkillEffectDesConfig sedc)
        {
            List<string> list = new List<string>();
            foreach (string des in sedc.buffDes)
            {
                if (string.IsNullOrEmpty(des)) continue;
                AddBuffActionConfig abac = AddBuffActionConfig.Get(des);
                if (abac == null) continue;

                foreach (int item in abac.buffid)
                {
                    BuffConfig bc = BuffConfig.Get(item);
                    if (bc != null) list.Add(bc.des);
                }
            }
            return list;
        }

        // 快慢
        string OnAniSpeedDes(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.aniSpeedDes)) return "";
            SkillAniConfig sac = SkillAniConfig.Get(sedc.aniSpeedDes);
            if (sac != null)
                return string.Format(TipContentUtil.GenText("skill_ani_speed_des"), sac.aniSpeedMul);
            return "";
        }

        // 附加暴击率(百分比显示)
        string OnCritRateDes(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.baojiRateDes)) return "";
            AttackActionConfig aac = AttackActionConfig.Get(sedc.baojiRateDes);
            if (aac != null && aac.baojiRateExtra > 0)
                return string.Format(TipContentUtil.GenText("skill_baoji_rate_des"), aac.baojiRateExtra * 100 + "%");
            return "";
        }

        // 无视防御
        string OnIgnoreDefDes(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.ignoreDefDes)) return "";
            AttackActionConfig aac = AttackActionConfig.Get(sedc.ignoreDefDes);
            if (aac != null && aac.ignoreDefenseSkill)
                return TipContentUtil.GenText("skill_ignore_def_des");
            return "";
        }

        // 条件判断
        List<string> OnConditionDes(SkillEffectDesConfig sedc)
        {
            List<string> list = new List<string>();
            foreach (string des in sedc.conditionDes)
            {
                if (string.IsNullOrEmpty(des)) continue;
                ConditionActionConfig cac = ConditionActionConfig.Get(des);
                if (cac == null) continue;

                foreach (string item in cac.desc)
                    list.Add(item);
            }
            return list;
        }

        // 吸血描述(百分比显示)
        string OnVampireDes(SkillEffectDesConfig sedc)
        {
            if (string.IsNullOrEmpty(sedc.xixueDes)) return "";
            AttackActionConfig aac = AttackActionConfig.Get(sedc.xixueDes);
            if (aac != null && aac.vampireFix > 0)
                return string.Format(TipContentUtil.GenText("skill_xixue_des"), aac.vampireFix * 100 + "%");
            return "";
        }

        #endregion

        // 获取链表描述
        string GetListDes(List<string> list)
        {
            string str = "";
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    str += list[i];
                    if (i + 1 != list.Count)
                        str += "\n";
                }
            }
            return str;
        }

        // 只有新增或划去，没有更改
        string OnConWithoutChange(List<string> show, List<string> showCon)
        {
            //值 -1 = 删除；0 = 正常；1 = 新增；
            Dictionary<string, int> showDic = new Dictionary<string, int>();
            if (show != null)
            {
                foreach (string item in show)
                {
                    if (!showDic.ContainsKey(item))
                        showDic.Add(item, -1);
                }
            }

            if (showCon != null)
            {
                foreach (string item in showCon)
                {
                    if (showDic.ContainsKey(item))
                        showDic[item] = 0;
                    else
                        showDic.Add(item, 1);
                }
            }

            int count = 0;
            string str = "";
            foreach (string item in showDic.Keys)
            {
                string curText = item;
                switch (showDic[item])
                {
                    case -1:
                        curText = DeleteStyle(item);
                        break;
                    case 1:
                        curText = AddStyle(item);
                        break;
                }
                str += curText;
                count++;
                if (count != showDic.Count)
                    str += "\n";
            }
            return str;
        }

        #region 技能领悟
        void Comprehend()
        {
            UICommon.ShowTipsWithItem(xys.UI.Utility.TipContentUtil.GenText("skill_comprehend_tip"), m_talentBookId, 1, OnTipBtnsClick);
        }

        void OnTipBtnsClick(bool isConfirm)
        {
            if (!isConfirm) return;

            int hasItemCount = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(m_talentBookId);

            if (hasItemCount < 1)
            {
                //快捷购买
                xys.UI.SystemHintMgr.ShowHint("秘籍不足");
            }
            else
            {
                PackageModule pm = App.my.localPlayer.GetModule<PackageModule>();
                if (pm != null) pm.UseItemById(m_talentBookId, 1);
            }
        }
        #endregion
    }
}
#endif
#if !USE_HOT
namespace xys.hot
{
    using NetProto;

    partial class HotSkillModule
    {
        C2ASkillModuleRequest m_Request;

        void Init()
        {
            m_Request = new C2ASkillModuleRequest(App.my.game.local);

            hotApp.my.handler.Reg<SkillComprehend>(Protoid.A2C_Skill_Comprehend, OnComprehendScheme);
            App.my.eventSet.Subscribe<SkillSave>(EventID.Skill_SaveSkill, OnSaveSkill);
            App.my.eventSet.Subscribe<SkillSchemeData>(EventID.Skill_SaveSkillScheme, OnSaveSkillScheme);
            App.my.eventSet.Subscribe<int>(EventID.Skill_UseSkillScheme, OnUseSkillScheme);
            App.my.eventSet.Subscribe<SkillSchemeName>(EventID.Skill_SetSkillSchemeName, OnSetSkillSchemeName);
        }
        private void OnSaveSkill(SkillSave request)
        {
            m_Request.SaveSkill(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Skill_SchemeError)
                {
                    xys.UI.SystemHintMgr.ShowHint("该技能方案不存在");
                    return;
                }

                if (respone.code == ReturnCode.Skill_SkillError)
                {
                    xys.UI.SystemHintMgr.ShowHint("保存技能失败");
                    return;
                }

                int curId = skillMgr.m_SkillSchemeDBData.curSchemeId;
                if (skillMgr.m_SkillSchemeDBData.allScheme.ContainsKey(curId))
                {
                    if (curId != (int)SkillSchemeType.SST_Default)
                    {
                        skillMgr.m_SkillSchemeDBData.allScheme[(int)SkillSchemeType.SST_Default] = skillMgr.m_SkillSchemeDBData.allScheme[curId];
                        skillMgr.m_SkillSchemeDBData.curSchemeId = (int)SkillSchemeType.SST_Default;
                    }

                    if (skillMgr.m_SkillSchemeDBData.allScheme[(int)SkillSchemeType.SST_Default].skills.ContainsKey(request.skillPointId))
                        skillMgr.m_SkillSchemeDBData.allScheme[(int)SkillSchemeType.SST_Default].skills[request.skillPointId] = request.skillId;
                    else
                        skillMgr.m_SkillSchemeDBData.allScheme[(int)SkillSchemeType.SST_Default].skills.Add(request.skillPointId, request.skillId);
                }
                App.my.eventSet.FireEvent(EventID.Skill_RefreshSkill, request);
            });
        }

        private void OnSaveSkillScheme(SkillSchemeData request)
        {
            m_Request.SaveSkillScheme(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;

                 if (respone.code == ReturnCode.Skill_SchemeNumError)
                 {
                     xys.UI.SystemHintMgr.ShowHint("保存失败");
                     return;
                 }

                 if (!skillMgr.m_SkillSchemeDBData.allScheme.ContainsKey(respone.schemeId))
                     skillMgr.m_SkillSchemeDBData.allScheme.Add(respone.schemeId, new SkillSchemeData());
                 skillMgr.m_SkillSchemeDBData.allScheme[request.schemeId].skillSchemeName = "";
                 xys.UI.Utility.TipContentUtil.Show("skill_save_succed");
                 App.my.eventSet.FireEvent(EventID.Skill_RefreshSkillSchemeName, request.schemeId);
             });
        }

        private void OnUseSkillScheme(int index)
        {
            NetProto.Int32 request = new NetProto.Int32();
            request.value = index;

            m_Request.UseSkillScheme(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;

                 if (respone.code == ReturnCode.Skill_SchemeError)
                 {
                     xys.UI.SystemHintMgr.ShowHint("技能方案不存在");
                     return;
                 }

                 SkillSchemeData ssd = respone.skillScheme;
                 if (skillMgr.m_SkillSchemeDBData.allScheme.ContainsKey(ssd.schemeId))
                 {
                     skillMgr.m_SkillSchemeDBData.curSchemeId = ssd.schemeId;
                     skillMgr.m_SkillSchemeDBData.allScheme[ssd.schemeId] = ssd;
                 }
                 else if (ssd.schemeId == (int)SkillSchemeType.SST_PVE || ssd.schemeId == (int)SkillSchemeType.SST_PVP)
                 {
                     skillMgr.m_SkillSchemeDBData.curSchemeId = ssd.schemeId;
                     skillMgr.m_SkillSchemeDBData.allScheme.Add(ssd.schemeId, ssd);
                 }

                 string name = ssd.skillSchemeName;
                 if (string.IsNullOrEmpty(name))
                     name = xys.UI.Utility.TipContentUtil.GenText("skill_scheme_name_" + ssd.schemeId);
                 xys.UI.Utility.TipContentUtil.Show("skill_use_succed", name);

                 App.my.eventSet.fireEvent(EventID.Skill_RefreshSkillScheme);
             });
        }

        private void OnSetSkillSchemeName(SkillSchemeName request)
        {
            m_Request.SetSkillSchemeName(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;

                 if (respone.code == ReturnCode.Skill_SchemeError)
                 {
                     xys.UI.SystemHintMgr.ShowHint("技能方案不存在");
                     return;
                 }

                 if (skillMgr.m_SkillSchemeDBData.allScheme.ContainsKey(request.schemeId))
                 {
                     skillMgr.m_SkillSchemeDBData.allScheme[request.schemeId].skillSchemeName = request.skillSchemeName;
                 }

                 App.my.eventSet.FireEvent(EventID.Skill_RefreshSkillSchemeName, request.schemeId);
             });
        }

        private void OnComprehendScheme(SkillComprehend respone)
        {
            if (respone.code == ReturnCode.Skill_ComprehendError)
            {
                xys.UI.SystemHintMgr.ShowHint("该技能书籍已领悟");
                return;
            }

            skillMgr.m_SkillSchemeDBData.comprehendItems.Add(respone.itemId);
            skillMgr.m_SkillSchemeDBData.allSkills = respone.skills;           //所有技能对应天赋升级

            App.my.eventSet.fireEvent(EventID.Skill_ComprehendSucceed);
        }
    }
}
#endif
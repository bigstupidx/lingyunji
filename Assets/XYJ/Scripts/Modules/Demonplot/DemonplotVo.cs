#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using Config;
    using xys.UI;
    using NetProto.Hot;

    partial class HotDemonplotsModule
    {
        DemonplotsModuleRequest m_DemonplotModuleReq;
        void Init()
        {
            //
//             this.m_DemonplotModuleReq = new DemonplotsModuleRequest(App.my.game.local);
//             App.my.eventSet.Subscribe<DemonplotRequest>(EventID.Demonplot_Matchin, this.OnMatchinItem);
//             App.my.eventSet.Subscribe<DemonplotSkillRequest>(EventID.Demonplot_AddExp, this.OnAddExp);
//             App.my.eventSet.Subscribe<DemonplotCurrencyRequest>(EventID.Demonplot_AddExpCurrency, this.OnAddExpCurrency);


            this.m_DemonplotModuleReq = new DemonplotsModuleRequest(hotApp.my.gameRPC);
            Event.Subscribe<DemonplotRequest>(EventID.Demonplot_Matchin, this.OnMatchinItem);
            Event.Subscribe<DemonplotSkillRequest>(EventID.Demonplot_AddExp, this.OnAddExp);
            Event.Subscribe<DemonplotCurrencyRequest>(EventID.Demonplot_AddExpCurrency, this.OnAddExpCurrency);
        }

        void OnMatchinItem(DemonplotRequest request)
        {
            this.m_DemonplotModuleReq.Collect(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<DemonplotRespone>(respone))
                    return;
                if(respone.productId != 0)
                {

                }
                if(respone.byproductId != 0)
                {

                }
                if (!this.demonplotsMgr.m_Tables.skills.ContainsKey(respone.skilltype))
                {
                    DemonplotSkillData skillData = this.demonplotsMgr.NewSkillData(respone.skilltype);
                    this.demonplotsMgr.m_Tables.skills.Add(respone.skilltype, skillData);
                }
                this.demonplotsMgr.m_Tables.skills[respone.skilltype].exp = respone.exp;
                this.demonplotsMgr.m_Tables.skills[respone.skilltype].lv = respone.lv;

                App.my.eventSet.fireEvent(EventID.Demonplot_RefleashUI);
                App.my.eventSet.fireEvent(EventID.Package_UpdatePackage);
            });
        }
        
        void OnAddExp(DemonplotSkillRequest request)
        {
            this.m_DemonplotModuleReq.AddExp(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
                 if (NetReturnCode.isError<DemonplotSkillRespone>(respone))
                     return;
                 if (respone.code == ReturnCode.Demonplot_SkillProperty_Error)
                 {
                     return;
                 }
                 if (respone.code == ReturnCode.Demonplot_Item_Error)
                 {
                     return;
                 }
                 if(respone.code == ReturnCode.Demonplot_LvMax)
                 {
                     SystemHintMgr.ShowHint(TipsContent.GetByName("demonplot_skill_limit_error").des);
                     return;
                 }
                 if(!this.demonplotsMgr.m_Tables.skills.ContainsKey(respone.skilltype))
                 {
                     DemonplotSkillData skillData = this.demonplotsMgr.NewSkillData(respone.skilltype);
                     this.demonplotsMgr.m_Tables.skills.Add(respone.skilltype, skillData);
                 }
                 this.demonplotsMgr.m_Tables.skills[respone.skilltype].lv = respone.lv;
                 this.demonplotsMgr.m_Tables.skills[respone.skilltype].exp = respone.exp;
                 App.my.eventSet.fireEvent(EventID.Demonplot_RefleashUI);
             });
        }

        void OnAddExpCurrency(DemonplotCurrencyRequest request)
        {
            this.m_DemonplotModuleReq.AddExpCostCurrency(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<DemonplotSkillRespone>(respone))
                    return;
                if (respone.code == ReturnCode.Demonplot_SkillProperty_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Demonplot_Item_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Demonplot_LvMax)
                {
                    SystemHintMgr.ShowHint(TipsContent.GetByName("demonplot_skill_limit_error").des);
                    return;
                }
                if (!this.demonplotsMgr.m_Tables.skills.ContainsKey(respone.skilltype))
                {
                    DemonplotSkillData skillData = this.demonplotsMgr.NewSkillData(respone.skilltype);
                    this.demonplotsMgr.m_Tables.skills.Add(respone.skilltype, skillData);
                }
                this.demonplotsMgr.m_Tables.skills[respone.skilltype].lv = respone.lv;
                this.demonplotsMgr.m_Tables.skills[respone.skilltype].exp = respone.exp;
                App.my.eventSet.fireEvent(EventID.Demonplot_RefleashUI);
            });
        }
    }
}
    #endif
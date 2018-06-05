#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using wProtobuf;
using xys.UI;

namespace xys.hot
{
    partial class HotPetsModule
    {
        PetsModuleRequest m_PetsModuleRequest;
        void Init()
        {
            #region 旧代码
            ////注册协议
            //            this.m_PetsModuleRequest = new PetsModuleRequest(App.my.game.local);
            //            //
            //            Event.Subscribe<int>(EventID.Pets_Create, this.CreatePet);
            //            Event.Subscribe<int>(EventID.Pets_Delect, this.DelectPet);
            //            Event.Subscribe<int>(EventID.Pets_Wash, this.WashPet);
            //            Event.Subscribe<RefineryPetRequest>(EventID.Pets_Refinery, this.RefineryPet);
            //            Event.Subscribe<LearnSkillPetRequest>(EventID.Pets_LearnSkill, this.LearnSkill);
            //            Event.Subscribe<LockSkillPetRequest>(EventID.Pets_LockSkill, this.LockSkill);
            //            Event.Subscribe<PetItemRequest>(EventID.Pets_AddExp, this.AddExp);
            //            Event.Subscribe<SetPetPotentialPointRequest>(EventID.Pets_SetPotential, this.SetPotenial);
            //            Event.Subscribe<PetItemRequest>(EventID.Pets_ResetPotential, this.Resetpotenial);

            //            Event.Subscribe<SetPetPlayRequest>(EventID.Pets_SetPetPlay, this.SetPetPlay);
            //            Event.Subscribe<SetPetPotentialSliderRequest>(EventID.Pets_Slider, this.SetSlider);
            //            Event.Subscribe<PetsNickNameRequest>(EventID.Pets_SetName, this.SetNickName);
            //            Event.Subscribe<PetsAIRequest>(EventID.Pets_SetAi, this.SetAi);
            //            Event.Subscribe<PetQualificationRequest>(EventID.Pets_SetQualification, this.SetQualification);

            //            Event.Subscribe<PetItemRequest>(EventID.Pets_SetPavvy, this.SetPavvy);
            //            Event.Subscribe<PetItemRequest>(EventID.Pets_SetGrowth, this.SetGrowth);
            //            Event.Subscribe<PetItemRequest>(EventID.Pets_SetPersonality, this.SetPersonality);
            //            Event.Subscribe<PetItemRequest>(EventID.Pets_OpenHoles, this.OpenHoles);

            //            Event.Subscribe<int>(EventID.Pets_2Items, this.ToItems);

            //            App.my.handler.Reg<PetsAttribute>(Protoid.A2C_PetCreate, OnItem2Pets);
            #endregion
            //注册协议
            this.m_PetsModuleRequest = new PetsModuleRequest(App.my.gameRPC.local);
            //
            Event.Subscribe<int>(EventID.Pets_Create, this.CreatePet);
            Event.Subscribe<int>(EventID.Pets_Delect, this.DelectPet);
            Event.Subscribe<int>(EventID.Pets_Wash, this.WashPet);
            Event.Subscribe<RefineryPetRequest>(EventID.Pets_Refinery, this.RefineryPet);
            Event.Subscribe<LearnSkillPetRequest>(EventID.Pets_LearnSkill, this.LearnSkill);
            Event.Subscribe<LockSkillPetRequest>(EventID.Pets_LockSkill, this.LockSkill);
            Event.Subscribe<PetItemRequest>(EventID.Pets_AddExp, this.AddExp);
            Event.Subscribe<SetPetPotentialPointRequest>(EventID.Pets_SetPotential, this.SetPotenial);
            Event.Subscribe<PetItemRequest>(EventID.Pets_ResetPotential, this.Resetpotenial);

            Event.Subscribe<SetPetPlayRequest>(EventID.Pets_SetPetPlay, this.SetPetPlay);
            Event.Subscribe<SetPetPotentialSliderRequest>(EventID.Pets_Slider, this.SetSlider);
            Event.Subscribe<PetsNickNameRequest>(EventID.Pets_SetName, this.SetNickName);
            Event.Subscribe<PetsAIRequest>(EventID.Pets_SetAi, this.SetAi);
            Event.Subscribe<PetQualificationRequest>(EventID.Pets_SetQualification, this.SetQualification);

            Event.Subscribe<PetItemRequest>(EventID.Pets_SetPavvy, this.SetPavvy);
            Event.Subscribe<PetItemRequest>(EventID.Pets_SetGrowth, this.SetGrowth);
            Event.Subscribe<PetItemRequest>(EventID.Pets_SetPersonality, this.SetPersonality);
            Event.Subscribe<PetItemRequest>(EventID.Pets_OpenHoles, this.OpenHoles);

            Event.Subscribe<int>(EventID.Pets_2Items, this.ToItems);

            hotApp.my.handler.Reg<PetsAttribute>(Protoid.A2C_PetCreate, OnItem2Pets);
        }

        private void OnItem2Pets(PetsAttribute attribute)
        {
            if (attribute == null)
                return;
            this.petsMgr.m_PetsTable.attribute.Add(attribute);
            App.my.uiSystem.HidePanel(PanelType.UIPackagePanel, false);
            App.my.uiSystem.ShowPanel(PanelType.UIPetsPanel,attribute.id);
        }

        void OnItem2Pets(Network.IPacket pack,PetsAttribute attribute)
        {
            if (attribute == null)
                return;
            this.petsMgr.m_PetsTable.attribute.Add(attribute);
        }

        void CreatePet(int petid)
        {
            NetProto.Int32 request = new NetProto.Int32();
            request.value = petid;
            this.m_PetsModuleRequest.Create(request, (error,respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<PetAttributeRespone>(respone))
                    return;
                if(respone.code == ReturnCode.Pets_PlayerLv_Error)
                {
                    //不满足开启等级
                    return;
                }
                if (respone.code == ReturnCode.Pets_FullHoles_Error)
                {
                    //宠物槽满
                    return;
                }
                this.petsMgr.m_PetsTable.attribute.Add(respone.attribute);
                //通知UI刷新
                Event.fireEvent(EventID.Pets_CreateRefresh);
            });
        }
        void DelectPet(int index)
        {
            if (!this.CheckIndex(index))
                return;
            NetProto.Int32 request = new NetProto.Int32();
            request.value = index;
            this.m_PetsModuleRequest.Delete(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
                 if (NetReturnCode.isError<PetsRespone>(respone))
                     return;
                 if(respone.code == ReturnCode.Pets_Playing_Error)
                 {
                     //出战宠物不能删除 
                     return;
                 }
                 //移除时判断出战宠物INDEX
                 if (index < this.petsMgr.m_PetsTable.PlayPetID)
                     this.petsMgr.m_PetsTable.PlayPetID -= 1;
                 this.petsMgr.m_PetsTable.attribute.Remove(this.petsMgr.m_PetsTable.attribute[index]);
                 //通知UI刷新
                 Event.fireEvent(EventID.Pets_CreateRefresh);
             });
        }
        void WashPet(int index)
        {
            if (!this.CheckIndex(index))
                return;
            NetProto.Int32 request = new NetProto.Int32();
            request.value = index;
            this.m_PetsModuleRequest.Wash(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (respone.code == ReturnCode.Pets_Index_Error)
                {
                    //宠物索引数据有误
                    return;
                }
                if (respone.code == ReturnCode.Pets_WashTime_Error)
                {
                    //洗练次数
                    return;
                }
                if (respone.code == ReturnCode.Material_Error)
                {
                    //洗练材料
                    return;
                }
                if (respone.code == ReturnCode.Slider_Error)
                {
                    //银币不足
                    return;
                }

                if (NetReturnCode.isError<WashPetRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                Debuger.Log(respone.attribute.id);
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }
        void RefineryPet(RefineryPetRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.Refinery(request, (error,respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
                
                 if (respone.code == ReturnCode.Pets_Index_Error)
                 {
                     //宠物索引数据有误
                     return;
                 }
                 if(respone.code == ReturnCode.Pets_Refinery_Error)
                 {
                     return;
                 }

                 if (NetReturnCode.isError<RefineryPetRespone>(respone))
                     return;
                 this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                 //通知UI刷新
                 Event.fireEvent(EventID.Pets_RefreshUI);
             });
        }
        void LearnSkill(LearnSkillPetRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.LearnSkill(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
                 if(respone.code == ReturnCode.Material_Error)
                 {
                     //技能书为0
                     return;
                 }
                 if (respone.code == ReturnCode.Pets_Index_Error)
                 {
                     //
                     return;
                 }
                 if (respone.code == ReturnCode.Pets_LearnSkill_Error)
                 {
                     //道具属性有误
                     return;
                 }

                 if (NetReturnCode.isError<PetAttributeRespone>(respone))
                     return;

                 #region 宠物技能判断
                 //
                 PetsAttribute attribute = this.petsMgr.m_PetsTable.attribute[index];
                 int skillid, resskillid;
                 skillid = attribute.trick_skills.id;
                 resskillid = respone.attribute.trick_skills.id;
                 if (skillid != resskillid)
                 {
                     if (skillid == 0)
                         SystemHintMgr.ShowHint(string.Format("你的灵兽技艺仅仅，成功领悟了{0}", Config.SkillConfig.Get(resskillid).name));
                     else
                         SystemHintMgr.ShowHint(string.Format("你的灵兽成功领悟了{0}，同时遗忘了{1}", Config.SkillConfig.Get(resskillid).name,Config.SkillConfig.Get(skillid).name));
                 }
                 else
                 {
                     skillid = resskillid = 0;
                     for (int i = 0; i < respone.attribute.passive_skills.Count;i++)
                     {
                         resskillid = respone.attribute.passive_skills[i].id;
                         if (i >= attribute.passive_skills.Count)
                             break;//技能位不符合，新增加数据为新技能
                         if (resskillid != attribute.passive_skills[i].id)
                         {
                             skillid = attribute.passive_skills[i].id;
                             break;
                         }
                     }
                     string skillName = string.Empty;
                     if (Config.SkillConfig.GetAll().ContainsKey(skillid))
                         skillName = Config.SkillConfig.Get(skillid).name;
                     else if (Config.PassiveSkills.GetAll().ContainsKey(skillid))
                         skillName = Config.PassiveSkills.Get(skillid).name;

                     string resskillName = string.Empty;
                     if (Config.SkillConfig.GetAll().ContainsKey(resskillid))
                         resskillName = Config.SkillConfig.Get(resskillid).name;
                     else if (!Config.PassiveSkills.GetAll().ContainsKey(resskillid))
                         resskillName = Config.PassiveSkills.Get(resskillid).name;

                     if (skillid == 0)
                         SystemHintMgr.ShowHint(string.Format("你的灵兽技艺仅仅，成功领悟了{0}", resskillName));
                     else
                         SystemHintMgr.ShowHint(string.Format("你的灵兽成功领悟了{0}，同时遗忘了{1}", resskillName, skillName));
                 }
#endregion

                 this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                 //通知UI刷新
                 Event.fireEvent(EventID.Pets_RefreshUI);
             });
        }
        void LockSkill(LockSkillPetRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.LockSkill(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (respone.code == ReturnCode.Pets_LockSkill_Error)
                {
                    //锁定技能失败
                    return;
                }
                if (NetReturnCode.isError<PetAttributeRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }
        void AddExp(PetItemRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.AddExp(request, (error, respone) =>
             {
                 if (respone.code == ReturnCode.Pets_LvMax_Error)
                 {
                     return;
                 }
                 else if (respone.code == ReturnCode.Material_Error)
                 {
                     return;
                 }
                 if (NetReturnCode.isError<PetAttributeRespone>(respone))
                     return;
                 this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                 //通知UI刷新
                 Event.fireEvent(EventID.Pets_RefreshUI);
             });
        }
        void SetPotenial(SetPetPotentialPointRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.SetPotential(request, (error, respone) =>
             {
                 if (respone.code == ReturnCode.Pets_Potential_Error)
                 {
                     //加点有误
                     return;
                 }
                 if (NetReturnCode.isError<PetAttributeRespone>(respone))
                     return;
                 this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                 //通知UI刷新
                 Event.fireEvent(EventID.Pets_RefreshUI);
             });
        }
        void Resetpotenial(PetItemRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.ResetPotential(request, (error, respone) =>
            {
                if (respone.code == ReturnCode.Material_Error)
                {
                    //材料不足
                    return;
                }
                if (respone.code == ReturnCode.Pets_Potential_Error)
                {
                    //重置出错
                    return;
                }
                if (NetReturnCode.isError<PetAttributeRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }
        void SetPetPlay(SetPetPlayRequest request)
        {
            int index = request.index;
            this.m_PetsModuleRequest.SetPlayPet(request, (error, respone) =>
             {
                 if (NetReturnCode.isError<PetsRespone>(respone))
                     return;
                 if (index == -1)
                 {
                     this.petsMgr.m_PetsTable.PlayPetID = -1;
                 }
                 else
                 {
                     if (respone.code == ReturnCode.Pets_Index_Error)
                         return;
                     this.petsMgr.m_PetsTable.PlayPetID = index == this.petsMgr.m_PetsTable.PlayPetID ? -1 : index;
                 }
                 //通知UI刷新
                 Event.fireEvent(EventID.Pets_RefreshUI);
             });
        }
        void SetSlider(SetPetPotentialSliderRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.SetPotentialSlider(request, (erroe, respone) =>
            {
                if (respone.code == ReturnCode.Pets_Potential_Error)
                {
                    //加点有误
                    return;
                }
                if (NetReturnCode.isError<PetAttributeRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
                Event.fireEvent(EventID.Pets_SliderUI);
            });
        }
        void SetNickName(PetsNickNameRequest request)
        {
            if(!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.SetNickname(request, (error, respone) =>
             {
                 if (respone.code == ReturnCode.Pets_Name_Error)
                 {
                     //名字错误
                     return;
                 }
                 if (NetReturnCode.isError<PetsNickNameRespone>(respone))
                     return;
                 this.petsMgr.m_PetsTable.attribute[respone.index].nick_name = respone.newName;
                 //通知UI刷新
                 Event.fireEvent(EventID.Pets_RefreshUI);
             });
        }
        void SetAi(PetsAIRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.SetAI(request, (error, respone) =>
            {
                if (respone.code == ReturnCode.Pets_AI_Error)
                {
                    //AI类型出错
                    return;
                }
                if (NetReturnCode.isError<PetAIRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.attribute[respone.index].ai_type = respone.aitype;
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }
        void SetQualification(PetQualificationRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.SetQualification(request, (error, respone) =>
            {
                if (respone.code == ReturnCode.Material_Error)
                {
                    //材料不足
                    SystemHintMgr.ShowHint("材料不足");
                    return;
                }
                if (respone.code == ReturnCode.Pets_ResetQualification_Error)
                {
                    //
                    return;
                }

                if (NetReturnCode.isError<PetAttributeRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.attribute[index] = respone.attribute;
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }
        void SetPavvy(PetItemRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.SetPavvy(request, (error, respone) =>
            {
                if (respone.code == ReturnCode.Material_Error)
                {
                    //
                    return;
                }
                if (respone.code == ReturnCode.Pets_ResetQualification_Error)
                {
                    //
                    return;
                }
                if (NetReturnCode.isError<PetItemRespone>(respone))
                    return;
                PetsAttribute petAttribute = this.petsMgr.m_PetsTable.attribute[index];
                petAttribute.property_savvy = respone.value;

                if (petAttribute.use_item_list.ContainsKey(request.itemId))
                {
                    petAttribute.use_item_list[request.itemId].usetimes += 1;
                }
                else
                {
                    PetUseItemData useItem = new PetUseItemData();
                    useItem.itemid = request.itemId;
                    useItem.usetimes = 1;
                    petAttribute.use_item_list.Add(request.itemId, useItem);
                }
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }

        void SetGrowth(PetItemRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.SetGrowth(request, (error, respone) =>
            {
                if (respone.code == ReturnCode.Material_Error)
                {
                    //
                    return;
                }
                if (respone.code == ReturnCode.Pets_Growth_Error)
                {
                    //
                    return;
                }
                if (NetReturnCode.isError<PetItemRespone>(respone))
                    return;
                PetsAttribute petAttribute = this.petsMgr.m_PetsTable.attribute[index];
                petAttribute.property_grow = respone.value;

                if (petAttribute.use_item_list.ContainsKey(request.itemId))
                {
                    petAttribute.use_item_list[request.itemId].usetimes += 1;
                }
                else
                {
                    PetUseItemData useItem = new PetUseItemData();
                    useItem.itemid = request.itemId;
                    useItem.usetimes = 1;
                    petAttribute.use_item_list.Add(request.itemId, useItem);
                }

                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }
        void SetPersonality(PetItemRequest request)
        {
            if (!this.CheckIndex(request.index))
                return;
            int index = request.index;
            this.m_PetsModuleRequest.SetPersonality(request, (error, respone) =>
            {
                if (respone.code == ReturnCode.Material_Error)
                {
                    //
                    return;
                }
                if (respone.code == ReturnCode.Pets_Property_Error)
                {
                    //
                    return;
                }
                if (NetReturnCode.isError<PetItemRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.attribute[index].personality = respone.value;
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }
        void OpenHoles(PetItemRequest request)
        {
            this.m_PetsModuleRequest.SetOpenHoles(request, (error, respone) =>
            {
                if (respone.code == ReturnCode.Pets_FullHoles_Error)
                {
                    //
                    return;
                }
                if (respone.code == ReturnCode.Material_Error)
                {
                    //
                    return;
                }

                if (NetReturnCode.isError<PetItemRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.PetsHoles = respone.value;
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }

        void ToItems(int index)
        {
            NetProto.Int32 request = new NetProto.Int32();
            request.value = index;
            this.m_PetsModuleRequest.PetsToItem(request, (error, respone) =>
            {
                if (NetReturnCode.isError<PetsRespone>(respone))
                    return;
                this.petsMgr.m_PetsTable.attribute.Remove(this.petsMgr.m_PetsTable.attribute[index]);
                //通知UI刷新
                Event.fireEvent(EventID.Pets_RefreshUI);
            });
        }

        bool CheckIndex(int index)
        {
            return this.petsMgr.m_PetsTable.attribute.Count > index;
        }
    }
}
#endif
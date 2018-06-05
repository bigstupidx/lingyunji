#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using wProtobuf;
using Config;
using xys.hot.UI;
using xys.UI;

namespace xys.hot
{ 
    partial class HotEquipSoulModule
    {
        C2AEquipSoulRequest m_C2AEquipSoulRequest;
        void Init()
        {
            //注册协议
            this.m_C2AEquipSoulRequest = new C2AEquipSoulRequest(App.my.socket.game.local);
            Event.Subscribe<SoulEnforceRequest>(EventID.EquipSoul_Enforce, this.EquipSoulEnforce);
            Event.Subscribe<SoulLoadRequest>(EventID.EquipSoul_Load, this.LoadEquipSoul);
            Event.Subscribe<SoulUnLoadRequest>(EventID.EquipSoul_UnLoad, this.UnLoadEquipSoul); ;
            Event.Subscribe<OpenSoulGridRequest>(EventID.EquipSoul_OpenGrid, this.OpenGrid);
        }

        private void OpenGrid(OpenSoulGridRequest msg)
        {
            this.m_C2AEquipSoulRequest.OpenSoulGrid(msg, (error,response) =>
            {
                if (!CheckCommonError(response.code))
                    return;
                if (response.code == ReturnCode.Slider_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                    return;
                }
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    equipSoulMgr.OpenSoulGrid(msg.subType, msg.index);
                    Event.fireEvent(EventID.EquipSoul_RefreshSoulList);
                    Event.fireEvent(EventID.EquipSoul_UpdateData);
                    Event.fireEvent(EventID.EquipSoul_UnLoadFinish);
                }
            });
        }

        private void EquipSoulEnforce(SoulEnforceRequest msg)
        {
            this.m_C2AEquipSoulRequest.SoulEnforce(msg, (error, response) =>
            {
                if (!CheckCommonError(response.code))
                    return;

                if (response.code == ReturnCode.EquipSoul_SoulType_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6204).des);
                    return;
                }
                if (response.code == ReturnCode.EquipSoul_ID_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6201).des);
                    return;
                }
                if (response.code == ReturnCode.EquipSoul_SoulPower_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6206).des);
                    return;
                }
                if (response.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                if (response.code == ReturnCode.ReturnCode_OK && !msg.isPackageItem)
                {
                    equipSoulMgr.ChangeSoul(msg.subType, msg.index, msg.targetSoulID);
                    Event.fireEvent(EventID.EquipSoul_RefreshSoulList);
                    Event.fireEvent(EventID.EquipSoul_UpdateData);
                    Event.fireEvent(EventID.EquipSoul_UnLoadFinish);
                }
            });
        }
        void LoadEquipSoul(SoulLoadRequest msg)
        {
            this.m_C2AEquipSoulRequest.LoadSoul(msg, (error, response) =>
            {
                if (!CheckCommonError(response.code))
                    return;
                if (response.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                if (response.code == ReturnCode.Package_NoItem_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6206).des);
                    return;
                }
                if (response.code == ReturnCode.EquipSoul_ID_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6201).des);
                    return;
                }
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    equipSoulMgr.ChangeSoul(msg.subType, msg.index, msg.id);
                    Event.fireEvent(EventID.EquipSoul_RefreshSoulList);
                    Event.fireEvent(EventID.EquipSoul_UpdateData);
                    Event.fireEvent(EventID.EquipSoul_UnLoadFinish);
                }
            });
        }

        void UnLoadEquipSoul(SoulUnLoadRequest msg)
        {
            this.m_C2AEquipSoulRequest.UnLoadSoul(msg, (error, response) =>
            {
                if (!CheckCommonError(response.code))
                    return;
                if (response.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    equipSoulMgr.ChangeSoul(msg.subType, msg.index, 0);
                    Event.fireEvent(EventID.EquipSoul_RefreshSoulList);
                    Event.fireEvent(EventID.EquipSoul_UpdateData);
                    Event.fireEvent(EventID.EquipSoul_UnLoadFinish);
                }
            });
        }

        bool CheckCommonError(ReturnCode code)
        {
            string str = "";
            bool ret = true;
            if (code == ReturnCode.EquipSoul_SubType_Error)
            {
                str = TipsContent.Get(6200).des;
                ret = false;
            }
            if (code == ReturnCode.EquipSoul_Index_Error)
            {
                str = TipsContent.Get(6201).des;
                ret = false;
            }
            if (code == ReturnCode.EquipSoul_Active_Error)
            {
                str = TipsContent.Get(6202).des;
                ret = false;
            }
            if (code == ReturnCode.EquipSoul_InActive_Error)
            {
                str = TipsContent.Get(6203).des;
                ret = false;
            }
            if (!ret)
                SystemHintMgr.ShowHint(str);
            return ret;
        }
    }
}
#endif

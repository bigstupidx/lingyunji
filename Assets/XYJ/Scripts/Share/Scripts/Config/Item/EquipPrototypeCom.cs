// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using NetProto;
using System;
using GameServer;

namespace Config
{
    public partial class EquipPrototype
    {
        public EquipPartsType childType
        {
            get
            {
                return (EquipPartsType)sonType;
            }
        }

        public override bool IsCanUse { get { return false; } } // �Ƿ��ʹ��\
        public override bool IsCanSell { get { return isCanSell; } }
        public override bool IsCanAllUse { get { return false; } }
        public override bool IsCanLost { get { return true; } } // �Ƿ�ɶ���
#if COM_SERVER
        public override bool isCanUseCondition(GameServer.GameUser user) { return false; }

        // ��ʼʹ�õ���
        public override bool Use(GameServer.GameUser user, int count) { return false; }

        public override void OnCreate(ItemData data)
        {
            data.equipdata =EquipPropertyMgr.GenerateEquipDataByID(Config.EquipPrototype.Get(data.id));
        }

        public override bool Sell(GameUser user, int count)
        {
            if (priceSilver != 0)
                user.AddSilverShell(priceSilver * count);
            return true;
        }
#else
        // �Ƿ�����ʹ������
        public override bool isCanUseCondition() { return false; }
        public override bool Use() { return false; }
#endif
    }
}



#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using NetProto.Hot;
    using System.Collections.Generic;

    class PetsMgr
    {
        public PetsTable m_PetsTable { get; private set; }

        public readonly int MaxSkillLock = 3;

        public PetsMgr()
        {
            m_PetsTable = new PetsTable();
        }

        public int GetPetCount()
        {
            return m_PetsTable.attribute.Count;
        }

        public int GetHoles()
        {
            return m_PetsTable.PetsHoles;
        }

        public string GetCurrentPetNickName()
        {
            if (this.m_PetsTable.PlayPetID == -1)
                return string.Empty;
            return m_PetsTable.attribute[this.m_PetsTable.PlayPetID].nick_name;
        }

        public bool GetPetsHoleState(int index,int max)
        {
            //当前开的宠物槽位个数
            int holesCount = m_PetsTable.PetsHoles;
            if (max < index + 1)
                return false;
            if (holesCount >= index + 1)
                return true;
            return false;
        }

        public int GetFightPetID()
        {
            return m_PetsTable.PlayPetID;
        }
        public PetsAttribute GetFightPetData()
        {
            if (this.m_PetsTable.PlayPetID == -1)
                return null;
            return m_PetsTable.attribute[m_PetsTable.PlayPetID];
        }

        public bool CheckIndex(int index)
        {
            return m_PetsTable.attribute.Count > index;
        }

        public bool CheckPetsUseItem(int petIndex,int itemId, int itemusemax)
        {
            PetsAttribute attribute = this.m_PetsTable.attribute[petIndex];
            if (attribute.use_item_list.ContainsKey(itemId) && attribute.use_item_list[itemId].usetimes >= itemusemax)
                return false;
            return true;
        }

        public bool CanLvUp(int petIndex)
        {
            PetsAttribute attribute = this.m_PetsTable.attribute[petIndex];
            int playerLv = App.my.localPlayer.levelValue;
            int petLv = attribute.lv;
            if (petLv >= playerLv + 5)
                return false;
            return true;
        }

        public PetUseItemData GetPetsUseItemData(int petIndex,int itemId)
        {
            PetsAttribute attribute = this.m_PetsTable.attribute[petIndex];
            foreach (PetUseItemData item in attribute.use_item_list.Values)
            {
                if (item.itemid == itemId)
                    return item;
            }
            PetUseItemData data = new PetUseItemData();
            data.itemid = itemId;
            data.usetimes = 0;
            return data;
        }

        public Dictionary<int,PetUseItemData> GetPetsAllUseItemData(int petIndex)
        {
            if (this.CheckIndex(petIndex))
                return this.m_PetsTable.attribute[petIndex].use_item_list;
            return null;
        }

        public string GetPersonality(int type)
        {
            string personality = string.Empty;
            switch (type)
            {
                case 1:
                    personality = "勇敢";
                    break;
                case 2:
                    personality = "聪明";
                    break;
                case 3:
                    personality = "冷静";
                    break;
                case 4:
                    personality = "冲动";
                    break;
                case 5:
                    personality = "顺从";
                    break;
                case 6:
                    personality = "狂妄";
                    break;
            }
            return personality;
        }
        public string GetQualityBg(int quality)
        {
            string bg = "ui_Common_Quality_White";
            switch (quality)
            {
                case 1:
                    bg = "ui_Common_Quality_White";
                    break;
                case 2:
                    bg = "ui_Common_Quality_Green";
                    break;
                case 3:
                    bg = "ui_Common_Quality_Blue";
                    break;
                case 4:
                    bg = "ui_Common_Quality_Purple";
                    break;
                case 5:
                    bg = "ui_Common_Quality_Orange";
                    break;
                case 6:
                    bg = "ui_Common_Quality_Red";
                    break;
            }
            return bg;
        }
    }
}
#endif
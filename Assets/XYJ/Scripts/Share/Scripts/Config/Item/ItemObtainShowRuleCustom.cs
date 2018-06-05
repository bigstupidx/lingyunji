using System.Collections.Generic;

namespace Config
{
    public partial class ItemObtainShowRule
    {
        public static ItemObtainShowRule GetRuleByItemId(int grounpId, int itemId)
        {
            List<ItemObtainShowRule> ItemIdList = GetGroupBykey(grounpId);
            for (int i = 0; i < ItemIdList.Count; i++)
            {
                if (ItemIdList[i].itemId == itemId)
                    return ItemIdList[i];
            }
            return null;
        }

        public static ItemObtainShowRule GetRuleByItemType(int grounpId, int type)
        {
            List<ItemObtainShowRule> ItemIdList = GetGroupBykey(grounpId);
            for (int i = 0; i < ItemIdList.Count; i++)
            {
                if (ItemIdList[i].itemType == type)
                    return ItemIdList[i];
            }
            return null;
        }
    }
}
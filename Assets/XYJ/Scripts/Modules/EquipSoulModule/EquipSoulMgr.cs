#if !USE_HOT
namespace xys.hot
{
    using Config;
    using NetProto;
    using NetProto.Hot;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    class EquipSoulMgr
    {
        public EquipSoulsGrids soulGrids = new EquipSoulsGrids();

        public Dictionary<int, SoulGrids> GetAllData()
        {
            return soulGrids.soulDic;
        }
        public SoulGrids GetSoulGrids(int subType)
        {
            if (soulGrids.soulDic.ContainsKey(subType))
                return soulGrids.soulDic[subType];
            return null;
        }

        public void ChangeSoul(int subType, int index, int id)
        {
            soulGrids.soulDic[subType].soulData[index].soulID = id;
        }

        public void OpenSoulGrid(int subType, int index)
        {
            soulGrids.soulDic[subType].soulData[index].isActive = true;
        }
    }
}
#endif

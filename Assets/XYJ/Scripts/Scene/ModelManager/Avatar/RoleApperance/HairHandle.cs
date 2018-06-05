using System.Collections.Generic;
using Config;
using NetProto;
using System;
namespace xys
{

    public class HairHandle
    {
        public HairConfig m_hairConfig;
        public RoleHairData m_roleHairData;
        public HairHandle(int job, int sex, AppearanceData data)
        {
            m_hairConfig = new HairConfig(job, sex, data);
            m_roleHairData = new RoleHairData(data.hairStyleId,data.hairDressId);     
        }
    }
    public class HairConfig
    {
        private List<HairItem> m_hairList = new List<HairItem>();

        public HairConfig(int job, int sex, AppearanceData data)
        {
            Dictionary<int, FashionDefine> dataDic = FashionDefine.GetAll();
            foreach (var temp in dataDic.Values)
            {
                if (temp.part == (int)FashionItemPart.HairItem)
                {
                    if (((temp.job == -1) || (temp.job == job))&& (temp.sex == sex))
                    {
                        HairItem tempHair = new HairItem(temp.id, data);
                        m_hairList.Add(tempHair);
                    }
                }
            }
        }
        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <returns></returns>
        public List<HairItem> GetHairList()
        {
            return m_hairList;
        }
        /// <summary>
        /// 根据ID获取单个配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HairItem Get(int id)
        {
            HairItem temp = null;
            int count = m_hairList.Count;
            for(int i=0;i< count; i++)
            {
                temp = m_hairList[i];
                if(temp.m_id==id)
                {
                    return temp;
                }
            }
            return temp;
        }
    }
    public class HairItem
    {
        public int m_id;

        public int m_validTime;//有效期
        public AprItemState m_state;

        public HairItem(int id, AppearanceData data)
        {
            m_id = id;
            m_validTime = 0;
            m_state = AprItemState.Lock;
            RefreshData(data);
        }
        public void RefreshData(AppearanceData data)
        { 
            foreach (var temp in data.hairItems)
            {
                if (temp.hairId == m_id)
                {
                    ServerGetTime time = new ServerGetTime();
                    long nowTick = time.GetCurrentTime();
                    if (temp.hairCD == DateTime.MaxValue.Ticks)
                    {
                        m_validTime = -1;
                        m_state = AprItemState.Unlock;
                    }
                    else if (temp.hairCD > nowTick)
                    {
                        m_validTime = (int)((temp.hairCD - nowTick) / TimeSpan.TicksPerDay) + 1;
                        m_state = AprItemState.Unlock;
                    }
                    else
                    {
                        m_validTime = 0;
                        m_state = AprItemState.OutTime;
                    }
                    break;
                }
            }
        }

        public string GetName()
        {
            FashionDefine item = FashionDefine.Get(m_id);
            return item.name;
        }
        public int GetFashionType()
        {
            FashionDefine item = FashionDefine.Get(m_id);
            return item.fashionType;
        }
        public string GetIconName()
        {
            FashionDefine item = FashionDefine.Get(m_id);
            return item.icon;
        }
        public string GetModName()
        {
            FashionDefine item = FashionDefine.Get(m_id);
            return item.mod;
        }
        public string GetDes()
        {
            FashionDefine item = FashionDefine.Get(m_id);
            return item.des;
        }
        public void GetUnlockInfo(out int unlockItem, out int unlockItemNum)
        {
            FashionDefine item = FashionDefine.Get(m_id);
            ClothItem.StrToTwoInt(item.item, out unlockItem, out unlockItemNum);
        }
    }
    public class RoleHairData
    {
        public int m_hairStyleId;
        public int m_hairDressId;
        public RoleHairData(int hairStyleId,int hairDressId)
        {
            m_hairStyleId = hairStyleId;
            m_hairDressId = hairDressId;
        }
    }
}
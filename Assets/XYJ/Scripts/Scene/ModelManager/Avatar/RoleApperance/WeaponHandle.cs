using System.Collections.Generic;
using Config;
using NetProto;
using UnityEngine;
using System;
namespace xys
{
    public class WeaponHandle
    {
        public WeaponConfig m_weaponConfig;
        public RoleWeaponData m_roleWeaponData;

        public WeaponHandle(int job, AppearanceData data)
        {
            m_weaponConfig = new WeaponConfig(job,data);

            m_roleWeaponData =new RoleWeaponData(data.weaponStyleId,  data.weaponEffectIdx);
        }

        public void ResetEffect()
        {
            int curWeapon = m_roleWeaponData.m_weaponId;
            int curEffect = m_roleWeaponData.m_curEffect;
            foreach(var item in m_weaponConfig.GetWeaponList())
            {
                if(item.m_id== curWeapon)
                {
                    item.m_curEffect = curEffect;
                }
                else
                {
                    item.m_curEffect = 0;
                }
            }
        }
    }

    public class WeaponConfig
    {
        private List<WeaponItem> m_weaponItemList = new List<WeaponItem>();

        public WeaponConfig(int job, AppearanceData data)
        {
            Dictionary<int, List<WeaponDefine>> dataDic = WeaponDefine.GetAllGroupBykey();
            foreach(var temp in dataDic)
            {
                if(temp.Value[0].job==job)
                {
                    WeaponItem tempItem = new WeaponItem(temp.Key,data);
                    m_weaponItemList.Add(tempItem);
                }
            }
        }
        public List<WeaponItem> GetWeaponList()
        {
            return m_weaponItemList;
        }
        public WeaponItem Get(int id)
        {
            WeaponItem temp = null;
            int count = m_weaponItemList.Count;
            for(int i=0;i<count;i++)
            {
                temp = m_weaponItemList[i];

                if (temp.m_id == id)
                    return temp;
            }
            return temp;
        }
    }

    public class WeaponItem
    {
        public int m_id;
        public int m_keyCount;

        public int m_validTime;
        public AprItemState m_state;
        public int m_curEffect;
        public int m_maxEffect;
        public WeaponItem(int id, AppearanceData data)
        {
            m_id = id;
            List<WeaponDefine> weaponList = WeaponDefine.GetGroupBykey(m_id);
            m_keyCount = weaponList.Count;

            m_validTime=0;
            m_state = AprItemState.Lock;
            if(data.weaponStyleId==m_id)
            {
                m_curEffect = data.weaponEffectIdx;
            }
            else
            {
                m_curEffect = 0;
            }
           
            m_maxEffect = 0;
            RefreshData(data);
        }

        public void RefreshData(AppearanceData data)
        {

            foreach (var temp in data.weapoinItems)
            {
                if (temp.weaponStyleId == m_id)
                {
                    ServerGetTime time = new ServerGetTime();
                    long nowTick = time.GetCurrentTime();
                    if (temp.weaponCD == DateTime.MaxValue.Ticks)
                    {
                        m_validTime = -1;
                        m_state = AprItemState.Unlock;
                    }
                    else if (temp.weaponCD > nowTick)
                    {
                        m_validTime = (int)((temp.weaponCD - nowTick) / TimeSpan.TicksPerDay) + 1;
                        m_state = AprItemState.Unlock;
                    }
                    else
                    {
                        m_validTime = 0;
                        m_state = AprItemState.OutTime;
                    }
                    m_maxEffect = temp.weaponMaxEffect;
                    break;
                }
            }
        }

        public string GetName(int level)
        {
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach (var temp in dataList)
            {
                if (temp.effectLevel == level)
                {
                    return temp.name;
                }
            }
            
            return null;
        }
        public string GetIconName(int level)
        {
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach (var temp in dataList)
            {
                if (temp.effectLevel == level)
                {
                    return temp.icon;
                }
            }

            return null;
        }
        public string GetModNameL(int level)
        {
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach (var temp in dataList)
            {
                if (temp.effectLevel == level)
                {

                    string[] mod = temp.mod.Split(';');
                    return mod[0];
                }
            }
            return null;
        }
        public string GetModNameR(int level)
        {
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach (var temp in dataList)
            {
                if (temp.effectLevel == level)
                {
                    string[] mod = temp.mod.Split(';');
                    if(mod.Length>1)
                    {
                        return mod[1];
                    }
                    else
                    {
                        return mod[0];
                    }                  
                }
            }
            return null;
        }
        public string GetEffectName(int level)
        {
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach(var temp in dataList)
            {
                if(temp.effectLevel==level)
                {
                    return temp.effect;
                }
            }
            return null;
        }
        public Color GetEffectColor(int level)
        {
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach (var temp in dataList)
            {
                if (temp.effectLevel == level)
                {
                    return ClothItem.StrToColor(temp.hsv);
                }
            }
            return Color.clear;
        }
        public string GetDes(int level)
        {
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach (var temp in dataList)
            {
                if (temp.effectLevel == level)
                {
                    return temp.des;
                }
            }
            return null;

        }
        public int GetUnlockLevel(int level)
        {
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach (var temp in dataList)
            {
                if (temp.effectLevel == level)
                {
                    return temp.unlockLevel;
                }
            }
            return -1;
        }
        public void GetUnlockInfo(int level,out int unlockItem,out int unlockItemNum)
        {
            string item = null;
            List<WeaponDefine> dataList = WeaponDefine.GetGroupBykey(m_id);
            foreach (var temp in dataList)
            {
                if (temp.effectLevel == level)
                {
                    item = temp.item;
                }
            }
            ClothItem.StrToTwoInt(item, out unlockItem, out unlockItemNum);
        }
    }

    public class RoleWeaponData
    {
        public int m_weaponId;
        public int m_curEffect;

        public RoleWeaponData(int weaponId,int curEffect)
        {
            m_weaponId = weaponId;
            m_curEffect = curEffect;
        }
    }
}
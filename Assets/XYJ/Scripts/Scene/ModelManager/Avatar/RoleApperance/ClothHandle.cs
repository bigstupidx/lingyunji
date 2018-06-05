using System.Collections.Generic;
using Config;
using NetProto;
using UnityEngine;
using System;
namespace xys
{
    public class ClothHandle
    {
        //服装配置
        public ClothConfig m_clothConfig;  
        //角色数据 
        public RoleClothData m_roleClothData;
        public ClothHandle(int job, int sex, AppearanceData data)
        {
            m_clothConfig = new ClothConfig(job, sex,data);
            m_roleClothData = new RoleClothData(data.clothStyleId,data.clothColorIdx);
        
        }
        public void ResetColor()
        {
            int curId = m_roleClothData.m_clothId;
            int curColor = m_roleClothData.m_curColor;
            foreach (var item in m_clothConfig.GetClothList())
            {
                if(item.m_id== curId)
                {
                    item.m_curColor = curColor;
                }
                else
                {
                    item.m_curColor = 0;
                }
            }
        }
    }
    public class RoleClothData
    {
        public int m_clothId;
        public int m_curColor;
        public RoleClothData(int clothId,int colorId)
        {
            m_clothId = clothId;
            m_curColor = colorId;
        }
    }
    public class ClothConfig
    {
        private List<ClothItem> m_clothList = new List<ClothItem>();
        
        public ClothConfig(int job,int sex, AppearanceData data)
        {
            Dictionary<int, FashionDefine> dataDic = FashionDefine.GetAll();
            foreach(var temp in dataDic.Values)
            {
                if(temp.part==(int)FashionItemPart.ClothItem)
                {
                    if((temp.job==-1)||(temp.job==job)&&(temp.sex==sex))
                    {
                        ClothItem tempCloth= new ClothItem(temp.id, data);
                        m_clothList.Add(tempCloth);
                    }
                }
            }
        }  
        public List<ClothItem> GetClothList()
        {
            return m_clothList;
        }  
        public ClothItem Get(int id)
        {
            ClothItem temp = null;
            int count = m_clothList.Count;
            for(int i=0;i<count;i++)
            {
                temp = m_clothList[i];
                if (temp.m_id==id)
                {
                    return temp;
                }
            }
            return temp;
        }
    }

    //单个服装的数据
    public class ClothItem
    {
        //通过Get获取配置表项
        public int m_id;
        //读服务器数据
        public int m_validTime;//有效期
        public AprItemState m_state;
        public int m_curColor;//当前选中的颜色，关闭界面后恢复默认

        List<Color> m_colorList = new List<Color>();
        public ClothItem(int id, AppearanceData data)
        {
            m_id = id;
            FashionDefine item = FashionDefine.Get(m_id);
            if (!string.IsNullOrEmpty(item.hsv))
            {
                Color temp = StrToColor(item.hsv);
                m_colorList.Add(temp);
            }
            if (data.clothStyleId == m_id)
            {
                m_curColor = data.clothColorIdx;
            }
            else
            {
                m_curColor = 0;
            }

            m_validTime = 0;
            m_state = AprItemState.Lock ;
            RefreshData(data);
        }
        /// <summary>
        /// 用于根据模块的数据刷新物品数据
        /// 有效期，染色方案，当前状态（未解锁，解锁，过期）
        /// </summary>
        public void RefreshData(AppearanceData data)
        {
            foreach (var temp in data.clothItems)
            {
                if (temp.clothStyleId == m_id)
                {
                    ServerGetTime time = new ServerGetTime();
                    long nowTick = time.GetCurrentTime();
                    if (temp.clothCD == DateTime.MaxValue.Ticks)
                    {
                        m_validTime = -1;
                        m_state = AprItemState.Unlock;
                    }
                    else if (temp.clothCD > nowTick)
                    {
                        m_validTime = (int)((temp.clothCD - nowTick) / TimeSpan.TicksPerDay)+1; 
                        m_state = AprItemState.Unlock;
                    }
                    else
                    {
                        m_validTime = 0;
                        m_state = AprItemState.OutTime;
                    }

                    if(m_colorList.Count>1)
                    {
                        Color tempColor = m_colorList[0];
                        m_colorList.Clear();
                        m_colorList.Add(tempColor);
                    }
                    for (int i = 0; i < temp.hsv.Count; i++)
                    {
                        Color tempColor = HSVToColor(temp.hsv[i]);
                        m_colorList.Add(tempColor);
                    }

                    break;
                }
            } 
        }


        
        /// <summary>
        /// 调用会刷新物品状态；
        /// </summary>
        /// <returns></returns>
        public int GetValidTime()
        {
            return m_validTime;
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
        public List<Color> GetColorList()
        {
            return m_colorList;
        }
        public void GetUnlockInfo(out int itemId,out int itemNum)
        {
            FashionDefine item = FashionDefine.Get(m_id);
            StrToTwoInt(item.item, out itemId, out itemNum);
        }
        public static Color HSVToColor(ApColor hsv)
        {
            return Color.HSVToRGB(hsv.h / 360f, hsv.s / 3f, hsv.v / 3f);
        }
        public static Color StrToColor(string _str)
        {
            int h = 0;
            float s = 1f;
            float v = 1f;
            if (!string.IsNullOrEmpty(_str))
            {
                string[] hsv = _str.Split('|');
                if (hsv != null && hsv.Length == 3)
                {
                    int.TryParse(hsv[0], out h);
                    float.TryParse(hsv[1], out s);
                    float.TryParse(hsv[2], out v);
                }
            }
            return Color.HSVToRGB(h / 360f, s / 3f, v / 3f);
        }
        public static void StrToTwoInt(string str,out int num1,out int num2)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string[] itemInfo = str.Split('|');
                if (itemInfo != null && itemInfo.Length == 2)
                {
                    int.TryParse(itemInfo[0], out num1);
                    int.TryParse(itemInfo[1], out num2);
                    return;
                }
            }
            num1 = -1;
            num2 = -1;
        }
    }
}

using System.Collections.Generic;
using Config;
using NetProto;
using UnityEngine;
using System;
namespace xys
{
    public class RideHandle
    {
        public RideConfig m_rideConfig;
        public RoleRideData m_roleRideData;
        ModelPartManage m_modelManager;


        public RideHandle(AppearanceData data)
        {
            m_roleRideData = new RoleRideData(data.rideStyleId,data.rideColorIdx,data.loadedRideList);
            m_rideConfig = new RideConfig(data);
            
        }

        public void ResetColor()
        {
            int curRide = m_roleRideData.m_curRide;
            int curColor = m_roleRideData.m_curColor;
            foreach(var item in m_rideConfig.GetRideItemList())
            {
                if(item.m_id== curRide)
                {
                    item.m_curColor = curColor;
                }
                else
                {
                    item.m_curColor = 0;
                }
            }
        }


        public ModelPartManage GetModelManager()
        {
            if (m_modelManager != null)
                return m_modelManager;
            return null;
        }

        public void SetModelManager(ModelPartManage modelManager)
        {
            if(modelManager==null)
            {
                Debug.Log("RideModelManager is Null");
                return;
            } 
            m_modelManager = modelManager;
        }

        public void ChangeRideMaterial(string matName)
        {
            if(m_modelManager==null)
            {
                Debug.Log("RideModelManager is Null");
                return;
            }
            if(!string.IsNullOrEmpty(matName))
            {
                MaterialLoadReference loadref = new MaterialLoadReference();
                loadref.Load(matName, OnFinishLoadMaterial, null);
            }        
        }

        public void OnFinishLoadMaterial(Material mat,object obj)
        {
            m_modelManager.ChangeMaterial(mat);
        }

    }
    public class RideConfig
    {
        private List<RideItem> m_rideItemList = new List<RideItem>();
        public RideConfig(AppearanceData data)
        {
            Dictionary<int, List<RideDefine>> dataDic = RideDefine.GetAllGroupBykey();
            foreach(var temp in dataDic.Keys)
            {
                RideItem tempItem = new RideItem(temp,data);
                m_rideItemList.Add(tempItem);
            }     
        }
        public List<RideItem> GetRideItemList()
        {
            return m_rideItemList;
        }
        public RideItem Get(int id)
        {
            RideItem temp = null;
            int count = m_rideItemList.Count;
            for(int i=0;i< count;i++)
            {
                temp = m_rideItemList[i];
                if (temp.m_id == id)
                    return temp;
            }
            return temp;
        }
    }
    public class RideItem
    {
        public int m_id;
        public int m_keyCount;

        public int m_validTime;
        public AprItemState m_state;
        public int m_curColor;
        public List<int> m_unlockedColorList;

        public RideItem(int id, AppearanceData data)
        {
            m_id = id;
            m_keyCount = RideDefine.GetGroupBykey(m_id).Count;

            m_validTime = 0;
            m_state = AprItemState.Lock;
            if(m_id== data.rideStyleId)
            {
                m_curColor = data.rideColorIdx;
            }
            else
            {
                m_curColor = 0;
            }
            
            m_unlockedColorList = null;
            RefreshData(data);
        }
        public void RefreshData(AppearanceData data)
        {
            foreach (var temp in data.rideItems)
            {
                if (temp.rideStyleId == m_id)
                {
                    ServerGetTime time = new ServerGetTime();
                    long nowTick = time.GetCurrentTime();
                    if (temp.rideCD == DateTime.MaxValue.Ticks)
                    {
                        m_validTime = -1;
                        m_state = AprItemState.Unlock;
                    }
                    else if (temp.rideCD > nowTick)
                    {
                        m_validTime = (int)((temp.rideCD - nowTick) / TimeSpan.TicksPerDay) + 1;
                        m_state = AprItemState.Unlock;
                    }
                    else
                    {
                        m_validTime = 0;
                        m_state = AprItemState.OutTime;
                    }
                    m_unlockedColorList = temp.unlockedColor;
                    break;
                }
            }
        }

        public string GetName()
        {
            RideDefine item = RideDefine.Get(m_id);
            return item.name;
        }
        public string GetMod()
        {
            RideDefine item = RideDefine.Get(m_id);
            return item.mod;
        }
        public string GetIconName()
        {
            RideDefine item = RideDefine.Get(m_id);
            return item.icon;
        }
        public string GetDes()
        {
            RideDefine item = RideDefine.Get(m_id);
            return item.des;
        }
        public string GetTex(int index)
        {
            if (index >= m_keyCount) return null;
            List<RideDefine> dataList = RideDefine.GetGroupBykey(m_id);
            return dataList[index].tex;
        }
        public int GetSpeed()
        {
            RideDefine item = RideDefine.Get(m_id);
            return item.speed;
        }
        public void GetUnlockInfo(int index,out int unlockItem,out int unlockItemNum)
        {
            List<RideDefine> dataList = RideDefine.GetGroupBykey(m_id);

            string item = null;
            if (index < m_keyCount)
            {
                item = dataList[index].item;
            }

            ClothItem.StrToTwoInt(item, out unlockItem, out unlockItemNum);
        }

        public Color GetColor(int index)
        {
            List<RideDefine> dataList = RideDefine.GetGroupBykey(m_id);

            if(index<m_keyCount)
            {
                return ClothItem.StrToColor(dataList[index].hsv);
            }
            return Color.clear;
        }
        public int GetCameraView()
        {
            RideDefine item = RideDefine.Get(m_id);
            int fov = 0;
            if(int.TryParse(item.fov,out fov))
            {
                return fov;
            }
            return 60;
        }
        public Vector3 GetCamarePos()
        {
            RideDefine item = RideDefine.Get(m_id);
            Vector3 temp = new Vector3();
            if (!string.IsNullOrEmpty(item.camera))
            {
                string[] pos = item.camera.Split(',');
                if (pos != null && pos.Length == 3)
                {
                    float.TryParse(pos[0], out temp.x);
                    float.TryParse(pos[1], out temp.y);
                    float.TryParse(pos[2], out temp.z);
                }
                return temp;
            }
            return temp;
        }

        public string GetMatNameByIndex(int index)
        {
            List<RideDefine> dataList = RideDefine.GetGroupBykey(m_id);
            if(index>=0&&index< dataList.Count)
            {
                return dataList[index].tex;
            }
            return null;
        }
    }
    /// <summary>
    /// 角色坐骑数据
    /// 当前骑乘，当前骑乘的坐骑颜色，当前装备坐骑表
    /// </summary>
    public class RoleRideData
    {
        public int m_curRide;
        public int m_curColor;
        private List<NetProto.LoadedRide> m_loadedRideList = new List<NetProto.LoadedRide>();

        public RoleRideData(int curRide,int curColor,List<LoadedRide> loadedRideList)
        {
            m_curRide = curRide;
            m_curColor = curColor;
            m_loadedRideList.AddRange(loadedRideList);
        }

        //供随机坐骑使用
        public List<NetProto.LoadedRide> GetLoadedRideList()
        {
            return m_loadedRideList;
        }
        public bool RemoveLoadRideById(int id)
        {
            LoadedRide needDelete = null;
            foreach (var temp in m_loadedRideList)
            {
                if(temp.rideStyleId==id)
                {
                    needDelete = temp;
                    break;
                }
            }
            if(needDelete!=null)
            {
                m_loadedRideList.Remove(needDelete);
                return true;
            }
            return false;
        }
        public void AddLoadRide(int rideId,int colorIdx)
        {
            LoadedRide needAdd = new LoadedRide();
            needAdd.rideStyleId = rideId;
            needAdd.curColor = colorIdx;
            m_loadedRideList.Add(needAdd);
        }
        public bool IsInLoadRideList(int id)
        {
            int count = m_loadedRideList.Count;
            for (int i = 0; i < count; i++)
            {
                LoadedRide temp = m_loadedRideList[i];
                if (temp.rideStyleId == id)
                {
                    return true;
                }
            }
            return false;
        }
        public LoadedRide GetLoadRideById(int id)
        {
            int count = m_loadedRideList.Count;
            for (int i = 0; i < count; i++)
            {
                LoadedRide temp = m_loadedRideList[i];
                if (temp.rideStyleId == id)
                {
                    return temp;
                }
            }
            return null;
        }
    }

}

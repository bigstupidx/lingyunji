#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using NetProto.Hot;

namespace xys.hot.UI
{
    using Config;
    using NetProto;
    using System;
    using System.Linq;
    using xys.UI.State;

    [AutoILMono]
    class UISoulListView
    {
        [SerializeField]
        Transform m_SoulGrids;

        public int currentSubType { get; private set; }
        Action m_SelectedCallBack;
        Dictionary<int, UIEquipSoulItem> m_InstanceDic = new Dictionary<int, UIEquipSoulItem>();
        public void OnShow()
        {
            m_InstanceDic.Clear();
            currentSubType = 0;
            RefreashUI();
        }
        public void OnHide()
        {

        }
        public void RefreashUI()
        {
            EquipSoulMgr soulMgr = App.my.localPlayer.GetModule<EquipSoulModule>().equipSoulMgr as EquipSoulMgr;
            int totalNum = (int)EquipPartsType.totalNum;
            if (totalNum!= m_SoulGrids.childCount)
            {
                Debug.LogError(string.Format("SoulGrid Num:{0} is not equal to EquipType Num:{1}", m_SoulGrids.childCount, totalNum));
                return;
            }
            for (int i = 0; i < totalNum; i++)
            {
                ILMonoBehaviour ilEquipSoulItem = m_SoulGrids.GetChild(i).GetComponent<ILMonoBehaviour>();
                if (ilEquipSoulItem != null)
                {
                    UIEquipSoulItem equipSoulItem = (UIEquipSoulItem)ilEquipSoulItem.GetObject();
                    equipSoulItem.SetData(i, soulMgr.GetSoulGrids(i), OnItemClick);
                    m_InstanceDic.Add(i, equipSoulItem);
                }
                else
                    Debug.Log("Get EquipSoulItem fail index:"+ i);
            }
            if (totalNum > 0)
                OnItemClick(1);
        }

        public void OnItemClick(int subType)
        {
            if(m_InstanceDic.ContainsKey(currentSubType))
                m_InstanceDic[currentSubType].SetFocus(false);
            if (m_InstanceDic.ContainsKey(currentSubType))
                m_InstanceDic[subType].SetFocus(true);
            currentSubType = subType;
            if (m_SelectedCallBack != null)
                m_SelectedCallBack();
        }

        public void SetSelectedCallback(Action selectedCallBack)
        {
            m_SelectedCallBack = selectedCallBack;
        }

        public NetProto.SoulGrids GetSoulGrids(int subType)
        {
            if (m_InstanceDic.ContainsKey(subType))
                return m_InstanceDic[subType].GetData();
            else
                return null;
        }
    }
}
#endif

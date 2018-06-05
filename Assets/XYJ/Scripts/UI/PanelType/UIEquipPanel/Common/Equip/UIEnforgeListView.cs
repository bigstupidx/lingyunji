#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;
using NetProto;

namespace xys.hot.UI
{
    using System;
    using System.Linq;
    using xys.UI;

    [AutoILMono]
    class UIEnforgeListView
    {
        [SerializeField]
        GameObject m_equipIcon;
        [SerializeField]
        Transform m_Grid;

        Config.RoleJob.Job m_CurrentJobType = 0;
        int m_CurrentLv = 0;
        public int currentIndex { get; private set; }
        List<UIEnforgeDynamicDisplay> m_InstanceList = new List<UIEnforgeDynamicDisplay>();
        List<int> equipIDList = new List<int>();
        Action<int> m_SelectedCallBack = null;
        System.Action m_NoneEquipCallBack = null;
        void OnAwake()
        {

        }
        void OnShow()
        {
            RreashUI();
        }
        void OnHide()
        {

        }
        void RreashUI()
        {
            //refreshData
            equipIDList.Clear();
            currentIndex = -1;
            var cfgDic = Config.EquipForgePrototype.GetAll();
            var itr = cfgDic.GetEnumerator();
            while (itr.MoveNext())
            {
                var curEquipCfg =  Config.EquipPrototype.Get(itr.Current.equipId);
                if (curEquipCfg.job.Has(m_CurrentJobType)&& curEquipCfg.leve<= m_CurrentLv)
                {
                    equipIDList.Add(itr.Current.equipId);
                }
            }
            //refreshUI
            m_Grid.DestroyChildren();
            m_InstanceList.Clear();
            for (int i = 0;i < equipIDList.Count;i++)
            {
                GameObject go = GameObject.Instantiate(m_equipIcon);
                go.SetActive(true);
                go.transform.SetParent(m_Grid, false);
                go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                var instance = (UIEnforgeDynamicDisplay)go.GetComponent<ILMonoBehaviour>().GetObject();
                if (instance!=null)
                {
                    instance.Set(Config.EquipPrototype.Get(equipIDList[i]), OnItemSelected,i);
                    m_InstanceList.Add(instance);
                }
            }
            if(m_InstanceList.Count!=0)
            {
                OnItemSelected(0);
                m_InstanceList[0].SetFocus(true);
            }
            else
                OnNoneEquip();
        }
        protected void OnNoneEquip()
        {
            if (m_NoneEquipCallBack != null)
                m_NoneEquipCallBack();
        }
        public void SetJobType(RoleJob.Job jobType)
        {
            m_CurrentJobType = jobType;
            RreashUI();
        }
        public void SetLv(int level)
        {
            m_CurrentLv = level;
            RreashUI();
        }
        public void SetSelectedCallBack(Action<int> selectAction, Action noneEquipAction)
        {
            m_SelectedCallBack = selectAction;
            m_NoneEquipCallBack = noneEquipAction;
        }
        public void OnItemSelected(int index)
        {
            if (currentIndex >= 0 && m_InstanceList.Count > currentIndex)
                m_InstanceList[currentIndex].SetFocus(false);
            if (index >= 0 && m_InstanceList.Count > index)
                m_InstanceList[index].SetFocus(true);
            if (m_SelectedCallBack != null&&index!=currentIndex)
                m_SelectedCallBack(index);
            currentIndex = index;
        }
        public int GetEquipID(int index)
        {
            int ret = 0;
            if (index >= 0 && m_InstanceList.Count > index)
                ret = m_InstanceList[index].equipID;
            return ret;
        }
    }
}
#endif
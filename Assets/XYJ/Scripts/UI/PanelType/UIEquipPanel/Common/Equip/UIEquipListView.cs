#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using NetProto.Hot;

namespace xys.hot.UI
{
    using NetProto;
    using System;
    using System.Linq;
    using xys.UI.State;

    [AutoILMono]
    class UIEquipListView
    {
        [SerializeField]
        protected Transform m_EquipGrid;
        [SerializeField]
        protected Transform m_DropEquip;
        [SerializeField]
        public GameObject m_EquipPrefab;
        [SerializeField]
        StateRoot m_EquipListBtn;     //已穿戴装备列表按钮
        [SerializeField]
        StateRoot m_ItemListBtn;     //背包装备列表按钮

        protected List<UIEquipDynamicDisplay> m_InstanceDic = new List<UIEquipDynamicDisplay>(); //<subtype,instance>
        System.Action m_SelectCallBack = null;
        System.Action m_NoneEquipCallBack = null;
        EquipMgr m_EquipMgr;
        public int currentEquipIndex { get; private set; }
        public ItemData currentItemData { get; private set; }  //目前仅用于切换页面时的currentIndex对应的装备是否发生变化的判断
        public ShowType currentShowType { get; set; } //装备种类：背包/身上装备
        public UIEquipDynamicDisplay.ShowType currentItemType { get; set; }   //物品信息类型：强化等级、装备等级
        public PageType currentPageType { get; set; }
        List<int> m_IndexDifMap = new List<int>();
        hot.Event.HotObjectEventAgent m_EventAgent ;
        public enum ShowType
        {
            EquipView = 1,
            PackageView = 2,
        }
        public enum PageType
        {
            TYPE_ENFORCE,
            TYPE_RECAST,
            TYPE_REFINE
        }
        

        void Awake()
        {
            m_EquipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr as EquipMgr;
            //获取配置表信息,默认优先显示已穿戴的装备的数据

            if (m_EquipListBtn != null)
                m_EquipListBtn.onClick.AddListener(this.OnEquipListClick);
            if (m_ItemListBtn != null)
                m_ItemListBtn.onClick.AddListener(this.OnItemListClick);
            currentEquipIndex = 0;
            m_EquipGrid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }


        void OnEquipListClick()
        {
            currentShowType = ShowType.EquipView;
            m_EquipListBtn.SetCurrentState(0, false);
            m_ItemListBtn.SetCurrentState(1, false);
            m_IndexDifMap.Clear();
            RefreshData();
            RefreshUI();
        }

        void OnItemListClick()
        {
            currentShowType = ShowType.PackageView;
            m_EquipListBtn.SetCurrentState(1, false);
            m_ItemListBtn.SetCurrentState(0, false);
            m_IndexDifMap.Clear();
            RefreshData();
            RefreshUI();
        }

        public void OnShow()
        {
            OnEquipListClick();
            m_EventAgent = new hot.Event.HotObjectEventAgent(xys.App.my.localPlayer.eventSet);
            RefreshData();
        }
        public void OnHide()
        {
            if (m_EventAgent!=null)
            {
                m_EventAgent = new hot.Event.HotObjectEventAgent(xys.App.my.localPlayer.eventSet);
            }
        }

        protected void OnSelectEquip(int index)
        {
            m_InstanceDic[currentEquipIndex].SetFocus(false);
            m_InstanceDic[index].SetFocus(true);
            currentEquipIndex = index;
            currentItemData = m_InstanceDic[index].itemData;
            if (m_SelectCallBack != null && index >= 0)
            {
                m_SelectCallBack();
            }
        }

        protected void OnNoneEquip()
        {
            if (m_NoneEquipCallBack != null)
                m_NoneEquipCallBack();
        }
        public void SetCallBack(Action selectAction,Action noneEquipAction)
        {
            m_SelectCallBack = selectAction;
            m_NoneEquipCallBack = noneEquipAction;
        }
        public void RefreshData()
        {
            DestroyChildren();
            
            if (currentShowType == ShowType.EquipView)
            {
                var itr = m_EquipMgr.equipTable.equipDic.GetEnumerator();
                int i = 0;
                while (itr.MoveNext())
                    AddEquip(itr.Current.Value, i++);
            }
            else if (currentShowType == ShowType.PackageView)
            {
                PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
                int i = 0;
                packageMgr.package.ForEach((xys.Grid grid) =>
                {
                    if (isEquip(grid))
                        AddEquip(grid.data.data, i++, grid.pos);
                });
            }
            if (m_InstanceDic.Count<= currentEquipIndex )
                currentEquipIndex = 0;
            RefreshUI();
        }
        public void RefreshUI()
        {
            int activeCount = 0;
            currentItemType = (currentPageType == PageType.TYPE_ENFORCE) ? UIEquipDynamicDisplay.ShowType.TYPE_ENFORCE : UIEquipDynamicDisplay.ShowType.TYPE_LV;
            for (int i = 0; i < m_InstanceDic.Count; i++)
            {
                m_InstanceDic[i].SetFocus(false);
                if (!isEquipOperatable(m_InstanceDic[i].itemData.id))
                {
                    m_InstanceDic[i].SetActive(false);
                    m_IndexDifMap.Add(i);
                    if (currentEquipIndex == i)
                    {
                        if (currentEquipIndex == 0)
                            currentEquipIndex++;
                        else
                        {
                            if (!m_IndexDifMap.Contains(0))
                                currentEquipIndex = 0;
                            else
                                currentEquipIndex ++;
                        }
                    }
                }
                else
                {
                    m_InstanceDic[i].SetActive(true);
                    m_InstanceDic[i].SetItemType(currentItemType);
                    activeCount++;
                }
            }
            if (activeCount != 0)
                OnSelectEquip(currentEquipIndex);
            else
                OnNoneEquip();
        }

        public ItemData GetItemData(int index)
        {
            //for (int i = 0; i < m_InstanceDic.Count; i++)
            //{
            //    if (m_InstanceDic[i].isActive())
            //    {
            //        if (index-- > 0)
            //            continue;
            //        else
            //            return m_InstanceDic[i].itemData;
            //    }
            //}
            return m_InstanceDic[index].itemData;
        }

        /// <summary>
        /// 获得index对应的装备标签的信息
        /// </summary>
        /// <param name="index">在instanceList中的index</param>
        /// <param name="tag">查询标记</param>
        /// <returns>如果是在身上的装备类型，返回false，tag为目标装备的subType；如果是背包中得到装备，返回true，tag为背包中的pos</returns>
        public void GetInstanceInfo(int index,out int tag,out bool isPackageItem)
        {
            isPackageItem = currentShowType == ShowType.EquipView ? false : true;
            tag = currentShowType == ShowType.EquipView ? m_InstanceDic[index].subType : m_InstanceDic[index].gridIndex;
        }

        bool isEquip(xys.Grid grid)
        {
            return (!grid.isEmpty)&&Config.EquipPrototype.GetAll().ContainsKey(grid.data.data.id);
        }
        
        public void AddEquip(ItemData item,int index,int gridIndex =-1)
        {


            GameObject go = null;
            if (m_DropEquip.childCount == 0)
            {
                go = GameObject.Instantiate(m_EquipPrefab);
                if (go == null)
                    return;
            }
            else
            {
                go = m_DropEquip.GetChild(0).gameObject;
            }

            go.SetActive(true);
            go.transform.SetParent(m_EquipGrid, false);
            go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            UIEquipDynamicDisplay instance = (UIEquipDynamicDisplay)go.GetComponent<ILMonoBehaviour>().GetObject();
            if (instance == null)
                return;
            instance.Set(item, OnSelectEquip,currentItemType, index, gridIndex);   //绑定装备Icon

            m_InstanceDic.Add(instance);
        }

        void DestroyChildren()
        {
            m_InstanceDic.Clear();
            m_EquipGrid.DestroyChildren();
        }

        bool isEquipOperatable(int id)
        {
            if (currentPageType == PageType.TYPE_RECAST)
            {
                if (!Config.EquipCfgMgr.isEquipRecastable(id))
                    return false;
            }
            if (currentPageType == PageType.TYPE_REFINE)
            {
                if (!Config.EquipCfgMgr.isEquipRefinable(id))
                    return false;
            }
            return true;
        }
    }
}
#endif
#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;

namespace xys.hot.UI
{
    using NetProto;
    using NetProto.Hot;
    using xys.UI;

    [AutoILMono]
    class UIEquipDynamicDisplay
    {
        public enum ShowType
        {
            TYPE_ENFORCE,
            TYPE_LV,
        }
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Image m_Icon;
        [SerializeField]
        Image m_Focus;
        [SerializeField]
        Image m_Quality;
        [SerializeField]
        Text m_Level;
        [SerializeField]
        Text m_Name;
        public Transform transform { get { return m_Transform; } }

        public int subType { get; protected set; }
        public int indexInListView { get; protected set; }
        public int gridIndex { get;protected set; }
        public ItemData itemData { get; protected set; }
        protected System.Action<int> m_ClickEvent = null;

        void Awake()
        {
            UnityEngine.UI.Button selfButton = this.m_Transform.GetComponent<UnityEngine.UI.Button>();
            if (selfButton != null) selfButton.onClick.AddListener(this.OnClick);
        }

        public void Set(ItemData item,System.Action<int> action,ShowType type, int index, int gridIndex)
        {
            EquipPrototype data = EquipPrototype.Get(item.id);
            if (data == null)
                return;
            m_Name.text = data.name;
            m_ClickEvent = action;
            subType = data.sonType;
            indexInListView = index;
            itemData = item;

            SetItemType(type);
            Helper.SetSprite(m_Icon, data.icon);
            Helper.SetSprite(m_Quality,Config.QualitySourceConfig.Get(data.quality).icon);
            m_Focus.gameObject.SetActive(false);
            this.gridIndex = gridIndex;
        }

        void OnClick()
        {
            if (m_ClickEvent !=null)
            {
                m_ClickEvent(indexInListView);
            }
            
        }
        public void SetFocus(bool active)
        {
            m_Focus.gameObject.SetActive(active);
        }
        public void SetActive(bool active)
        {
            m_Transform.gameObject.SetActive(active);
        }
        public bool isActive()
        {
            return m_Transform.gameObject.activeSelf;
        }

        public void SetItemType(ShowType type)
        {
            EquipPrototype data = EquipPrototype.Get(itemData.id);
            if (data == null)
                return;
            if (ShowType.TYPE_ENFORCE == type)
            {
                if (itemData.equipdata.equipBasicData.awakenStatus)
                    m_Level.text = "觉醒强化" + itemData.equipdata.equipBasicData.awakenEnforceLV + "级";
                else
                    m_Level.text = "强化等级" + itemData.equipdata.equipBasicData.enforceLv + "级";
            }
            else
            {
                if (ShowType.TYPE_LV == type)
                    m_Level.text = data.leve + "级";
            }
        }
    }
}
#endif
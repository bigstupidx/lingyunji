#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;

namespace xys.hot.UI
{
    using System;
    using NetProto;
    using xys.UI;

    [AutoILMono]
    class UIEnforgeDynamicDisplay
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
        Text m_EquipType;
        [SerializeField]
        Text m_Name;
        public Transform transform { get { return m_Transform; } }

        public int equipID { get; private set; }
        int index = 0;
        protected System.Action<int> m_ClickEvent = null;

        void Awake()
        {
            UnityEngine.UI.Button selfButton = this.m_Transform.GetComponent<UnityEngine.UI.Button>();
            if (selfButton != null) selfButton.onClick.AddListener(this.OnClick);
        }

        public void Set(Config.EquipPrototype cfg, System.Action<int> action, int index)
        {
            equipID = cfg.id;
            this.index = index;
            m_Name.text = cfg.name;
            m_ClickEvent = action;
            m_EquipType.text = GetEquipType(cfg.sonType);
            Helper.SetSprite(m_Quality,QualitySourceConfig.Get(cfg.quality).icon);
            Helper.SetSprite(m_Icon, cfg.icon);
        }

        private string GetEquipType(int sonType)
        {
            switch (sonType)
            {
                case 1:
                    return "武器";
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    return "防具";
                case 7:
                case 8:
                case 9:
                    return "首饰";
                default:
                    return "";
            }
        }

        void OnClick()
        {
            if (m_ClickEvent != null)
                m_ClickEvent(index);
        }
        public void SetFocus(bool active)
        {
            m_Focus.gameObject.SetActive(active);
        }
    }
}
#endif
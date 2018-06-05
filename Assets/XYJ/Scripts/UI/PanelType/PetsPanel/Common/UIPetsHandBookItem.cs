#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
namespace xys.hot.UI
{
    using xys.UI;

    [AutoILMono]
    class UIPetsHandBookItem
    {
        [SerializeField]
        Transform m_Transform;

        public Transform transform{get {return m_Transform;} }

        protected PetAttribute m_Property = null;
        protected System.Action<UIPetsHandBookItem> m_ClickEvent = null;
        
        void Awake()
        {
            UnityEngine.UI.Button selfButton = this.m_Transform.GetComponent<UnityEngine.UI.Button>();
            if (selfButton != null) selfButton.onClick.AddListener(this.OnClick);
        }
        public void Set(PetAttribute item, System.Action<UIPetsHandBookItem> click_event = null)
        {
            Image image = this.m_Transform.Find("Icon").GetComponent<Image>();
            Helper.SetSprite(image, item.icon);
            m_Property = item;
            m_ClickEvent = click_event;
        }

        void OnClick()
        {
            if (m_ClickEvent != null)
                m_ClickEvent(this);
        }

        public PetAttribute property { get { return m_Property; } }
    }
}


#endif
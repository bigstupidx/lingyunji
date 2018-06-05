#if !USE_HOT
using UnityEngine;
using Config;
using UnityEngine.UI;
using xys.UI;
using xys.hot.Event;

namespace xys.hot.UI
{
    [AutoILMono]
    class WelfareRwdItem
    {
        [SerializeField]
        protected Image m_ItemSp;
        [SerializeField]
        protected Text m_Text;
        [SerializeField]
        protected Button m_ItemBtn;
        [SerializeField]
        protected Image m_QualitySp;
        public int itemID
        {
            get;
            private set;
        }
        public int mIndex
        {
            get;
            private set;
        }
        public int count
        {
            get;
            private set;
        }
        void Awake()
        {
            m_ItemBtn.onClick.AddListener(OnItemClick);
        }
        public HotObjectEventAgent m_Event;
        public void OnEnable()
        {
            if (m_Event != null)
            {
                m_Event.Release();
                m_Event = null;
            }
            m_Event = new HotObjectEventAgent(App.my.localPlayer.eventSet);
        }
        public void OnDisable()
        {
            if (m_Event != null)
            {
                m_Event.Release();
                m_Event = null;
            }
        }
        public void SetData(int index, ItemCount item)
        {
            mIndex = index;
            itemID = item.id;
            count = item.count;
            // 设置Item SP。数量等
            Helper.SetSprite(m_ItemSp, Config.Item.Get(item.id).icon);
            if (item.count>1)
            {
                m_Text.text = item.count.ToString();
            }
            else
            {
                m_Text.text = "";
            }
            Helper.SetSprite(m_QualitySp, QualitySourceConfig.Get(Config.Item.Get(item.id).quality).icon);

        }

        void OnItemClick()
        {
            ShowItemTip();
        }

        public void ShowItemTip()
        {
            UICommon.ShowItemTips(itemID);
        }
    }
}

#endif
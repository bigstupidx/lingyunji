#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class UITextureItem
    {
        [SerializeField]
        public StateRoot m_StateRoot;
        [SerializeField]
        public Image m_Image;
        [SerializeField]
        public RectTransform m_rectTran;

        public int m_Index;
        private System.Action<UITextureItem> m_ClickEvent = null;
        private System.Action<UITextureItem> m_StateChangeEvent = null;
        public void Set(int index,Color color, System.Action<UITextureItem> clickEvent, System.Action<UITextureItem> stateChangeEvent)
        {
            InitItem();
            m_Index = index;
            if(m_Image!=null)
            {
                m_Image.color = color;   
            }  
            m_ClickEvent = clickEvent;
            m_StateChangeEvent = stateChangeEvent;
        }
        public void Set(int index,int state, System.Action<UITextureItem> clickEvent, System.Action<UITextureItem> stateChangeEvent, string name = null)
        {
            InitItem();
            m_Index = index;
            m_ClickEvent = clickEvent;
            m_StateChangeEvent = stateChangeEvent;           
            if (m_Image!=null&&name!=null)
            {
                Helper.SetSprite(m_Image, name);             
            }
            
            m_StateRoot.SetCurrentState(state, false);
         
        }
        
        void Awake()
        {
            if(m_StateRoot!=null)
            {
                m_StateRoot.onClick.AddListener(OnClick);
                m_StateRoot.onStateChange.AddListener(OnStateChange);
            }

        }
        void InitItem()
        {
            m_StateRoot.SetCurrentState(0, false);
            m_ClickEvent = null;
            m_StateChangeEvent = null;
           
        }
        void OnClick()
        {
            if(m_ClickEvent!=null)
            {
                m_ClickEvent(this);
            }
        }
        void OnStateChange()
        {
            if(m_StateChangeEvent!=null)
            {
                m_StateChangeEvent(this);
            }
        }

        /// <summary>
        /// 添加空的TextureItem,用于对个相同图标StateRoot对象
        /// </summary>
        /// <param name="_count"></param>
        /// <param name="_itemRoot">目标根目录</param>
        /// <param name="_itemSource">源根目录，不可与目标相同</param>
        public static void AddNullItem(int _count, Transform _itemRoot, Transform _itemSource)
        {
            if (_itemRoot == _itemSource)
            {
                Debug.Log("源根目录，不可与目标根目录相同");
                return;
            };
            if (_itemRoot == null) return;
            if (_itemSource == null) return;
            GameObject itemPref = null;
            if (_itemSource.childCount > 0)
            {
                itemPref = _itemSource.GetChild(0).gameObject;
            }
            if (itemPref == null) return;

            for (int i = 0; i < _count; i++)
            {
                int curCount = _itemSource.childCount;
                GameObject tempObj = null;
                if (curCount > 0)
                {
                    tempObj = _itemSource.GetChild(0).gameObject;
                }
                else
                {
                    tempObj = GameObject.Instantiate(itemPref);
                }
                if (tempObj == null) return;
                tempObj.SetActive(true);
                tempObj.transform.SetParent(_itemRoot, false);
                tempObj.transform.localScale = Vector3.one;
            }
        }
        /// <summary>
        /// 回收Texture到源目录下，并隐藏。不会清除Item的数据；
        /// </summary>
        /// <param name="_itemRoot">目标目录</param>
        /// <param name="_itemSource">源目录</param>
        public static void RecycleItem(Transform _itemRoot, Transform _itemSource,int count)
        {
            if (_itemRoot == null) return;
            if (_itemSource == null) return;
            int i = 0;
            while (_itemRoot.childCount != 0&&i<count)
            {
                _itemRoot.GetChild(0).SetParent(_itemSource, false);
                i++;
            }
            _itemSource.gameObject.SetActive(false);
        }
        public static void RecycleItem(Transform _itemRoot, Transform _itemSource)
        {
            if (_itemRoot == null) return;
            if (_itemSource == null) return;   
            while (_itemRoot.childCount != 0 )
            {
                _itemRoot.GetChild(0).SetParent(_itemSource, false);
            }
            _itemSource.gameObject.SetActive(false);
        }
    }
}
#endif
#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{
    [AutoILMono]
    class UIPetsHandBookScrollView
    {
        [SerializeField]
        protected Transform m_ItemGrid;
        [SerializeField]
        protected Transform m_DropItem;
        [SerializeField]
        public GameObject m_ItemPrefab;

        protected UIPetsHandBookItem m_SelectItem = null;
        protected Dictionary<int, UIPetsHandBookItem> m_PetHandBookItems = new Dictionary<int, UIPetsHandBookItem>();

        protected System.Action m_SelectCallBack = null;

        public void SetEvent(Event.HotObjectEventAgent eventAgent)
        {
            eventAgent.Subscribe<int>(EventID.Pets_HB_Sort, this.OnSort);
        }

        void OnEnable()
        {
            m_ItemGrid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        public IEnumerator Show(System.Action callback = null)
        {
            this.Hide();
            List<PetAttribute> data = PetAttribute.GetAll();
            foreach(PetAttribute item in data)
            {
                if (item.variation == 1)
                    continue;
                this.NewItem(item);
            }

            if(m_PetHandBookItems.Count != 0)
            {
                List<int> keys = new List<int>(m_PetHandBookItems.Keys);
                this.OnSelectItem(m_PetHandBookItems[keys[0]]);
            }

            if(callback != null)
            {
                callback();
            }
            yield return 0;
        }

        public void Hide()
        {
            m_SelectItem = null;
            for(int i = 0; i < m_ItemGrid.childCount;i++)
            {
                GameObject.DestroyObject(m_ItemGrid.GetChild(i).gameObject);
            }

            for(int i = 0 ; i < m_DropItem.childCount;i++)
            {
                GameObject.DestroyObject(m_DropItem.GetChild(i).gameObject);
            }

            m_PetHandBookItems.Clear();
        }

        public bool NewItem(PetAttribute data)
        {
            if (data == null)
                return false;

            GameObject obj = null;
            if(m_DropItem.childCount == 0)
            {
                obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null)
                    return false;
            }
            else
            {
                obj = m_DropItem.GetChild(0).gameObject;
            }

            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);

            ILMonoBehaviour ilm = obj.GetComponent<ILMonoBehaviour>();
            UIPetsHandBookItem item = (UIPetsHandBookItem)ilm.GetObject();
            if (item == null)
                return false;
            item.Set(data, this.OnSelectItem);
            m_PetHandBookItems[data.identity] = item;
            return true;// obj.transform;
        }

        protected void OnSelectItem(UIPetsHandBookItem item)
        {
            if(m_SelectItem != item)
            {
                //do
                if (m_SelectItem != null)
                    m_SelectItem.transform.Find("selected_icon").gameObject.SetActive(false);
                //
                m_SelectItem = item;
                //DO
                if (m_SelectItem != null)
                    m_SelectItem.transform.Find("selected_icon").gameObject.SetActive(true);

                if (m_SelectCallBack != null && item != null)
                    m_SelectCallBack();
            }
        }

        void OnSort(int index)
        {
            OnSelectItem(null);
            List<PetAttribute> data = PetAttribute.GetAll();
            //返回全部列表
            if (index == 0)
            {
                for (int i = 0; i < m_ItemGrid.childCount; i++)
                {
                    m_ItemGrid.GetChild(i).gameObject.SetActive(true);
                }
                if (m_PetHandBookItems.Count != 0)
                {
                    this.OnSelectItem(m_PetHandBookItems[data[0].identity]);
                }
            }
            else//根据品质排序
            {
                for (int i = 0; i < m_ItemGrid.childCount; i++)
                {
                    UIPetsHandBookItem item = (UIPetsHandBookItem)m_ItemGrid.GetChild(i).GetComponent<ILMonoBehaviour>().GetObject();
                    if (item.property.type == index)
                    {
                        m_ItemGrid.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        m_ItemGrid.GetChild(i).gameObject.SetActive(false);
                    }
                }
                for (int i = 0; i < data.Count;i++ )
                {
                    if (data[i].type == index)
                    {
                        this.OnSelectItem(m_PetHandBookItems[data[i].identity]);
                        break;
                    }
                }
            }
        }

        public int selected { get { return m_SelectItem != null ? m_SelectItem.property.identity : 0; } }

        public System.Action selectedCallback { set { m_SelectCallBack = value; } }
    }

}
#endif
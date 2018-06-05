#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using xys.UI.State;
using NetProto.Hot;
using NetProto;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{
    [AutoILMono]
    class UIPetsScrollView
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Transform m_ItemGrid;
        [SerializeField]
        Transform m_DropItems;
        [SerializeField]
        GameObject m_ItemPrefab;

        protected GameObject m_DragedItem;

        protected UIPetsItem m_SelectedItem = null;
        protected Dictionary<int, UIPetsItem> m_PetItems = new Dictionary<int, UIPetsItem>();
        protected List<UIPetsItem> m_PetNullItems = new List<UIPetsItem>();

        protected System.Action m_SelectedCallback = null;
        protected System.Action m_ChangePetCallback = null;

        //更新每个宠物控件
        public void SetPetPlay(int index)
        {
            if (m_PetItems.Count != 0)
            {
                List<int> keys = new List<int>(m_PetItems.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    m_PetItems[keys[i]].EnablePlay(m_PetItems[keys[i]].attributeIndex == index);
                    if (keys[i] == index) this.OnSelectItem(m_PetItems[keys[i]]);
                }
            }
        }

        public void Show(int selected = -1,System.Action callback = null)
        {
            this.Hide();
            //创建空控件
            for (int i = 0; i < Config.PetsHoleDefine.GetAll().Count; i++)
            {
                this.NewNullItem();
            }
            //创建每个宠物控件
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return;

            foreach (PetsAttribute e in petsMgr.m_PetsTable.attribute)
            {
                this.NewItem(e);
            }
            if (m_PetItems.Count > 0)
            {
                int fightPetuuid = petsMgr.GetFightPetID();
                if (selected >= 0 && m_PetItems.ContainsKey(selected))
                {
                    this.OnSelectItem(m_PetItems[selected]);
                }
                else if(fightPetuuid == -1)
                {
                    List<int> keys = new List<int>(m_PetItems.Keys);
                    this.OnSelectItem(m_PetItems[keys[0]]);
                }
                else
                {
                    this.OnSelectItem(m_PetItems[fightPetuuid]);
                }
            }

            for (int i = petsMgr.GetHoles() + 1; i < m_ItemGrid.childCount;i++ )
            {
                m_ItemGrid.GetChild(i).gameObject.SetActive(false);
            }

            if (callback != null)
                callback();
        }

        public void Hide()
        {
            if (m_SelectedItem != null)
                m_SelectedItem.transform.GetComponent<StateRoot>().CurrentState = 0;
            m_SelectedItem = null;

            while (m_ItemGrid.transform.childCount != 0)
            {
                Transform item = m_ItemGrid.transform.GetChild(0);
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }

            m_PetNullItems.Clear();
            m_PetItems.Clear();
        }

        protected bool NewItem(PetsAttribute element)
        {
            if (element == null || Config.PetAttribute.Get(element.id) == null)
                return false;
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return false;
            if (m_PetNullItems.Count == 0 || m_PetItems.Count >= petsMgr.GetHoles())
                return false;
            //
            UIPetsItem item = m_PetNullItems[0];
            item.transform.gameObject.SetActive(true);
            item.transform.gameObject.transform.SetParent(m_ItemGrid, false);
            item.transform.gameObject.transform.localScale = Vector3.one;

            int attributeIndex = petsMgr.m_PetsTable.attribute.IndexOf(element);
            item.Set(attributeIndex, element, false, this.OnSelectItem);
            m_PetItems[attributeIndex] = item;
            m_PetNullItems.Remove(item);
            return true ;
        }

        protected bool NewNullItem()
        {
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return false;

            GameObject obj = null;
            if (m_DropItems.childCount == 0)
            {
                obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null) return false;
            }
            else
            {
                obj = m_DropItems.GetChild(0).gameObject;
            }

            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localScale = Vector3.one;

            ILMonoBehaviour ilobj = obj.GetComponent<ILMonoBehaviour>();
            if (ilobj == null)
                return false;
            UIPetsItem item = (UIPetsItem)ilobj.GetObject();
            bool isLock = !petsMgr.GetPetsHoleState(obj.transform.GetSiblingIndex(), Config.PetsHoleDefine.GetAll().Count);
            item.Set(-1,null, isLock, this.OnSelectItem);
            m_PetNullItems.Add(item);
            return true;
        }

        protected void OnSelectItem(UIPetsItem item)
        {
            if(item == null)
            {
                if (m_SelectedItem != null)
                    m_SelectedItem.transform.Find("selected_icon").gameObject.SetActive(false);
                return;
            }
            if (item.element == null)
                return;
            if (m_SelectedItem != item)
            {
                if (m_SelectedItem != null)
                    m_SelectedItem.transform.Find("selected_icon").gameObject.SetActive(false);

                m_SelectedItem = item;
                 if (m_SelectedItem != null )
                     m_SelectedItem.transform.Find("selected_icon").gameObject.SetActive(true);

                if (m_ChangePetCallback != null)
                    m_ChangePetCallback();

                if (m_SelectedCallback != null)
                    m_SelectedCallback();
            }
        }

        public IEnumerator OnRefresh()
        {
            if(!this.m_Transform.gameObject.activeSelf)
                yield break;
            this.Show(m_SelectedItem.attributeIndex);
        }

        public int selected { get { return m_SelectedItem != null ? m_SelectedItem.attributeIndex : -1; } }

        public System.Action selectedCallback { set { m_SelectedCallback = value; } }
        public System.Action changePetCallback { set { m_ChangePetCallback = value; } }

        public GameObject itemPrefab { set { m_ItemPrefab = value; } get { return m_ItemPrefab; } }
    }
}

#endif
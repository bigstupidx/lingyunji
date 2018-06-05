#if !USE_HOT
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using NetProto;
using xys.UI;
using UI;
using NetProto.Hot;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIPetsHandBookSort
    {
        protected enum State
        {
            Show,
            Hide,
            TweenAlpha,
        }
        protected State m_State = State.Hide;
        int m_ClickHandlerId = -1;

        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        UGUITweenPosition m_Tweener;
        [SerializeField]
        Button m_SortBtn;
        [SerializeField]
        Transform m_Root;
        [SerializeField]
        Transform m_SortImage;

        //UIPetsHandBookScrollView m_ScrollView;
        // PetsPanel m_Panel;
        private void OnEnable()
        {
            m_SortImage.eulerAngles = new Vector3(0, 0, 0);
            m_Tweener.onFinished.AddListener(this.OnTweenAlphaFinish);
            m_SortBtn.onClick.AddListener(this.OnSortEvent);

            for (int i = 0; i < m_Root.childCount; i++)
            {
                int index = i;
                m_Root.GetChild(i).GetComponent<Button>().onClick.AddListener(() => { this.OnSortTypeEvent(index); });
            }
        }

        //         public void SetParent(xys.hot.UI.PetsPanel parent)
        //         {
        //             m_Panel = parent;
        //         }

        private void OnDisable()
        {
            m_SortBtn.onClick.RemoveListener(this.OnSortEvent);
            m_Tweener.onFinished.RemoveListener(this.OnTweenAlphaFinish);
            for (int i = 0; i < m_Root.childCount; i++)
                m_Root.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        }
        #region 事件
        void OnSortEvent()
        {
            if (m_State != State.Show)
            {
                m_Tweener.gameObject.SetActive(true);
                m_State = State.TweenAlpha;
                m_Tweener.tweenFactor = 0.0f;
                m_Tweener.PlayForward();

                m_SortImage.eulerAngles = new Vector3(0, 0, 180);
            }
            else
            {
                m_State = State.TweenAlpha;
                m_Tweener.PlayReverse();

                m_SortImage.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        void OnSortTypeEvent(int index)
        {
            int[] nType = new int[] {
                (int)PetType.ALL,
                (int)PetType.NORMAL,
                (int)PetType.HIGHT_NORMAL,
                (int)PetType.PRECIOUS,
                (int)PetType.THERION,};
            //m_Panel.Event.FireEvent<int>(EventID.Pets_HB_Sort, index);
            App.my.eventSet.FireEvent<int>(EventID.Pets_HB_Sort, index);

            m_SortBtn.GetComponentInChildren<Text>().text = m_Root.GetChild(index).GetComponentInChildren<Text>().text;
            if (m_State == State.Show)
            {
                m_State = State.TweenAlpha;
                m_Tweener.PlayReverse();

                m_SortImage.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        #endregion

        void OnTweenAlphaFinish()
        {
            if (m_Tweener.tweenFactor > 0.9999f)
            {
                m_State = State.Show;

                if (m_ClickHandlerId != 0)
                    EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
                m_ClickHandlerId = EventHandler.pointerClickHandler.Add(this.OnGlobalClick);
            }
            else
            {
                m_State = State.Hide;
                EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
                m_ClickHandlerId = 0;
                m_Tweener.gameObject.SetActive(false);
            }
        }

        protected bool OnGlobalClick(GameObject go, BaseEventData bed)
        {
            if (go == null || !go.transform.IsChildOf(this.m_Transform))
            {
                if (m_State == State.Show)
                {
                    m_State = State.TweenAlpha;
                    m_Tweener.PlayReverse();

                    m_SortImage.eulerAngles = new Vector3(0, 0, 0);
                }

                return false;
            }

            return true;
        }
    }
}



#endif
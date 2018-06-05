#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class EventMask : MonoBehaviour,
            IPointerEnterHandler,
            IPointerExitHandler,
            IPointerDownHandler,
            IPointerUpHandler,
            IPointerClickHandler,
            IInitializePotentialDragHandler,
            IBeginDragHandler,
            IDragHandler,
            IEndDragHandler,
            IDropHandler,
            IScrollHandler,
            IUpdateSelectedHandler,
            ISelectHandler,
            IDeselectHandler,
            IMoveHandler,
            ISubmitHandler,
            ICancelHandler
    {

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
        }

        public virtual void OnMove(AxisEventData eventData)
        {
        }

        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
        }
    }
}

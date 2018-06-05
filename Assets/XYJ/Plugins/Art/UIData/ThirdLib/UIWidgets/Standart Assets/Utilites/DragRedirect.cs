using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIWidgets {
	/// <summary>
	/// Drag redirect.
	/// </summary>
	public class DragRedirect : UIBehaviour, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
	{
		[SerializeField]
		public ScrollRect RedirectTo;

		public void OnBeginDrag(PointerEventData eventData)
		{
			RedirectTo.OnBeginDrag(eventData);
		}

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			RedirectTo.OnInitializePotentialDrag(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			RedirectTo.OnDrag(eventData);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			RedirectTo.OnEndDrag(eventData);
		}

		public void OnScroll(PointerEventData eventData)
		{
			RedirectTo.OnScroll(eventData);
		}
	}
}


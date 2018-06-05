using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.UI
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class OnClickHidePanel : EmptyGraphic, IPointerClickHandler
    {
        UIPanelBase panelBase;

        BoxCollider2D boxCollider2D;

        protected override void Awake()
        {
            base.Awake();

            panelBase = GetComponent<UIPanelBase>();
            boxCollider2D = GetComponent<BoxCollider2D>();

            if (boxCollider2D == null)
            {
                boxCollider2D = panelBase.cachedGameObject.AddComponent<BoxCollider2D>();
            }

            OnRectTransformDimensionsChange();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (panelBase == null)
            {
                return;
            }

            Vector2 size = panelBase.cachedTransform.rect.size;
            Vector2 pivot = panelBase.cachedTransform.pivot - (Vector2.one * 0.5f);

            boxCollider2D.offset = new Vector2(-size.x * pivot.x, -size.y * pivot.y);
            boxCollider2D.size = size;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerEnter != eventData.pointerPress)
                return;

            if (panelBase != null)
                panelBase.Hide(true);
        }
    }
}
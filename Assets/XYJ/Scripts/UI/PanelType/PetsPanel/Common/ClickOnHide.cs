using UnityEngine;
using System.Collections;
using UI;
using UnityEngine.EventSystems;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
namespace xys.UI
{
    public class ClickOnHide : MonoBehaviour
    {

        int onlyId;
        void OnEnable()
        {
            onlyId = EventHandler.pointerClickHandler.Add(OnPress);
        }

        bool OnPress(GameObject obj, BaseEventData eventData)
        {
            gameObject.SetActive(false);
            return false;
        }

        void OnDisable()
        {
            EventHandler.pointerClickHandler.Remove(onlyId);
        }
    }
}


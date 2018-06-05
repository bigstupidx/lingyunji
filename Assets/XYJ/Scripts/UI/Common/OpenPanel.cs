using UnityEngine;
using UnityEngine.EventSystems;

namespace xys.UI
{
    public class OpenPanel : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        string panelName; // 面板名

        // 点击事件
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            App.my.uiSystem.ShowPanel(panelName);
        }
    }
}

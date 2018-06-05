using Config;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    namespace ItemObtainShow
    {
        public class UIAwardTipsItem : MonoBehaviour
        {
            [SerializeField]
            Image itemIcon;

            public void OnShow(int itemId)
            {
                Helper.SetSprite(itemIcon, ItemBaseAll.Get(itemId).icon);
            }
        }
    }
}
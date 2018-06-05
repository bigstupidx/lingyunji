#if !USE_HOT
using Config;
using NetProto;
using NetProto.Hot;
using System.Collections.Generic;
using UnityEngine;

namespace xys.hot.UI
{
    [AutoILMono]
    class WelfareItemScrollView
    {
        [SerializeField]
        Transform m_Grid;
        [SerializeField]
        Transform m_SignItem;
        [SerializeField]
        Transform m_LvItem;
        [SerializeField]
        Transform m_DayOLItem;
        [SerializeField]
        Transform m_OLItem;


        public void HideChild(WelfarePageType type)
        {
            switch (type)
            {
                case WelfarePageType.TYPE_SIGN:
                    break;
                case WelfarePageType.TYPE_LV:
                    m_LvItem.gameObject.SetActive(false);
                    break;
                case WelfarePageType.TYPE_DAYOL:
                    m_DayOLItem.gameObject.SetActive(false);
                    break;
                case WelfarePageType.TYPE_OL:
                    m_OLItem.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

    }
}

#endif
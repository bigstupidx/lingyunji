#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    
    class FunTipsData : BaseTipsData
    {
        public bool isShowGetType;
    }

    [System.Serializable]
    class FunTipsItem : BaseTipsItem
    {
        public GameObject getTypeRoot; // 获取途径

        public void OnInit()
        {
            root.SetActive(false);
        }

        public void Set(FunTipsData ftd)
        {
            base.Set(ftd);
            getTypeRoot.SetActive(ftd.isShowGetType);

            root.SetActive(true);
        }
    }
}
#endif
#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    
    class BaseTipsData
    {
        public string name; // 道具名
        public string icon; // icon
        public bool isBind; // 是否绑定
        public string quality; // 品质
        public string type; // 类型
        public string desc; // 描述
        public string levelLimit; // 等级限制
        public string funIntro; // 功能描述
    }

    [System.Serializable]
    class BaseTipsItem
    {
        public GameObject root; // 根结点

        // 装备头部件
        public Text name; // 物品名
        public Image icon; // ICON
        public Image bind; // 是否绑定
        public Image quality; // 品质

        public Text type; // 类型
        public Text desc; // 物品描述

        public Text levelLimit; // 等级限制
        public Text funIntro; // 功能描述

        public void Set(BaseTipsData ftd)
        {
            xys.UI.Helper.SetSprite(icon, ftd.icon);
            xys.UI.Helper.SetSprite(quality, ftd.quality);

            if (bind != null)
                bind.gameObject.SetActive(ftd.isBind);
            if (name != null)
                name.text = ftd.name;

            if (type != null)
                type.text = ftd.type;

            if (desc != null)
                desc.text = GlobalSymbol.ToUT(ftd.desc);

            if (levelLimit != null)
                levelLimit.text = ftd.levelLimit;

            if (funIntro != null)
                funIntro.text = GlobalSymbol.ToUT(ftd.funIntro);
        }

        public bool isVisable
        {
            get { return root.activeSelf; }
            set { root.SetActive(value); }
        }
    }
}
#endif
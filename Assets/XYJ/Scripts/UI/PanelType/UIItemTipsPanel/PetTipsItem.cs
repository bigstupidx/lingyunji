#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    class PetTipsData : BaseTipsData
    {
        public int score; // 评分
        public string grade; // 级别
    }

    // 元兽灵魂珠
    [System.Serializable]
    class PetTipsItem : BaseTipsItem
    {
        public Text score; // 评分
        public Text grade; // 灵魂级别

        public void OnInit()
        {
            root.SetActive(false);
        }

        public void Set(PetTipsData ptd)
        {
            base.Set(ptd);
            score.text = ptd.score.ToString();
            grade.text = ptd.grade;

            root.SetActive(true);
        }
    }
}
#endif
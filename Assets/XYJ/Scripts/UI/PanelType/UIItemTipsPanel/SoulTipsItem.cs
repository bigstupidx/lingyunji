#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    class SoulAtt
    {
        public string name;
        public string value;
    }

    class SoulTipsData : BaseTipsData
    {
        public int strengLevel; // 魂魄的强化等级

        public List<SoulAtt> atts = new List<SoulAtt>(); // 属性列表
    }

    [System.Serializable]
    class SoulTipsItem : BaseTipsItem
    {
        public Text strengLevel; // 强化等级

        public GameObject attTemplate; // 属性项模版

        class Att
        {
            public Att(GameObject r)
            {
                root = r;
                name = r.transform.Find("Name").GetComponent<Text>();
                att = r.transform.Find("Att").GetComponent<Text>();
            }

            public GameObject root;
            public Text name; // 名称项
            public Text att; // 属性描述项

            public void Set(SoulAtt sa)
            {
                root.SetActive(true);
                name.text = sa.name;
                att.text = sa.value;
            }
        }

        List<Att> attLists = new List<Att>();

        public void OnInit()
        {
            root.SetActive(false);
            attLists.Add(new Att(attTemplate));
        }

        public void Set(SoulTipsData ftd)
        {
            base.Set(ftd);

            while (true)
            {
                if (attLists.Count < ftd.atts.Count)
                {
                    var newItem = UITools.AddChild(attTemplate.transform.parent.gameObject, attTemplate);
                    attLists.Add(new Att(newItem));
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < ftd.atts.Count; ++i)
            {
                attLists[i].Set(ftd.atts[i]);
            }

            for (int i = ftd.atts.Count; i < attLists.Count; ++i)
                attLists[i].root.SetActive(false);

            root.SetActive(true);
        }
    }
}
#endif
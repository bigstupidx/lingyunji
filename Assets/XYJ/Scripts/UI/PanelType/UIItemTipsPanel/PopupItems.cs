#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    class PopupData
    {
        public string name;
        public System.Action action;
    }

    // 法宝
    [System.Serializable]
    class PopupItems
    {
        public GameObject root;
        public GameObject template;

        class Item
        {
            public GameObject root;
            public Button btn;
            public Text text;

            public Item(GameObject r)
            {
                root = r;
                var rt = root.transform;
                btn = root.GetComponent<Button>();
                text = rt.Find("Name").GetComponent<Text>();

                btn.onClick.RemoveAllListeners();
            }

            public void Set(PopupData pd)
            {
                btn.onClick.SetListener(() => { pd.action(); });
                text.text = pd.name;
                root.SetActive(true);
            }

            public void Release()
            {
                btn.onClick.RemoveAllListeners();
                root.SetActive(false);
            }
        }

        List<Item> currentList = new List<Item>();

        public void OnInit()
        {
            currentList.Add(new Item(template));

            root.SetActive(false);
        }

        public void Set(List<PopupData> datas)
        {
            while (currentList.Count < datas.Count)
            {
                currentList.Add(new Item(UITools.AddChild(template.transform.parent.gameObject, template)));
            }

            for (int i = 0; i < datas.Count; ++i)
                currentList[i].Set(datas[i]);

            for (int i = datas.Count; i < currentList.Count; ++i)
                currentList[i].Release();

            root.SetActive(true);
        }
    }
}
#endif
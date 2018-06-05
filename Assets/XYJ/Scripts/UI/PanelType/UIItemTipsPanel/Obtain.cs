#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    class ObtainGrid
    {
        public string m_ItemIcon; // 图标
        public string m_ItemName; // 图标名
    }

    // 获取途径
    [System.Serializable]
    class ObtainTips : BaseTipsItem
    {
        public Text obtainDes;
        public GridLayoutGroup gridCom;
        public GameObject itemGrid;

        public void OnInit()
        {
            root.SetActive(false);
        }

        public void Set(List<ObtainGrid> gridList)
        {
            foreach (var itor in gridList)
            {
                GameObject obj = GameObject.Instantiate(itemGrid) as GameObject;
                obj.transform.SetParent(gridCom.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;

                Image icon = obj.transform.Find("Icon").GetComponent<Image>();
                xys.UI.Helper.SetSprite(icon, itor.m_ItemIcon);
                Text name = obj.transform.Find("Name").GetComponent<Text>();
                name.text = itor.m_ItemName;
            }

            root.SetActive(true);
        }

        public void SetDes(string des)
        {
            obtainDes.text = des;
        }
    }
}
#endif
#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys;
using xys.battle;

namespace xys.hot.UI
{
    [System.Serializable]
    class UIPetsAssignScrollView
    {
        public GameObject m_ItemPrefab;
        public Transform m_ItemGrid;

        public ScrollRect m_ScrollRect;

        Dictionary<int, PetAttributeItem> m_Items = new Dictionary<int, PetAttributeItem>();
        public void OnInit()
        {
            //更多属性面板
            List<Config.AttributeDefine> attrPropList = new List<Config.AttributeDefine>(Config.AttributeDefine.GetAll().Values);
            List<Config.AttributeDefine> tempGroud = new List<Config.AttributeDefine>();
            for (int i = 0; i < attrPropList.Count; i++)
            {
                if (attrPropList[i].petAddPointOrder > 0)
                    tempGroud.Add(attrPropList[i]);
            }
            tempGroud.Sort((a, b) => a.petAddPointOrder.CompareTo(b.petAddPointOrder));
            for (int i = 0; i < tempGroud.Count; i++)
            {
                GameObject obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null)
                    continue;

                obj.SetActive(true);
                obj.transform.SetParent(m_ItemGrid, false);
                obj.transform.localScale = Vector3.one;

                if (!m_Items.ContainsKey(tempGroud[i].petAddPointOrder))
                {
                    PetAttributeItem item = new PetAttributeItem(tempGroud[i].id, obj.GetComponent<Button>(), null);
                    m_Items.Add(tempGroud[i].petAddPointOrder, item);
                }
            }
        }

        public void RefreshItem(PetObj petAttribute, BattleAttri tempAttribute)
        {
            foreach (PetAttributeItem item in m_Items.Values)
            {
                item.RefreshNum(petAttribute, tempAttribute);
            }
        }
    }
}

#endif
#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class EquipMaterialItem : MaterialItem
    {
        Image m_EquipedSp;
        public int index
        {
            get;
            protected set;
        }
        public EquipMaterialItem(Transform trans) : base(trans)
        {
            m_EquipedSp = trans.Find("EquipedSp").GetComponent<Image>();
        }
        public void SetEquipedActive(bool active)
        {
            m_EquipedSp.gameObject.SetActive(active);
        }
        public void SetData(int itemID, int index)
        {
            this.index = index;
            base.SetData(itemID, 0, false);
            base.SetTextActive(false);
        }
        public void SetOnClickListener(Action<int> action)
        {
            base.m_Btn.onClick.RemoveAllListeners();
            base.m_Btn.onClick.AddListener(() => { action(index); });
        }
    }
}
#endif
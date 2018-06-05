#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using NetProto.Hot;
using Config;

namespace xys.hot.UI
{
    using NetProto;
    using System;
    using System.Linq;
    using xys.UI.State;

    [AutoILMono]
    class UIEquipSoulItem
    {
        [SerializeField]
        ILMonoBehaviour m_ILEquipIcon;
        EquipIcon m_EquipIcon;
        [SerializeField]
        Transform m_SoulGridTrans;
        [SerializeField]
        Transform m_Focus;
        [SerializeField]
        Text m_Text;
        [SerializeField]
        Button m_Btn;

        SoulGrids m_SoulGrids;
        int m_Subtype;
        void Awake()
        {
            if(m_SoulGridTrans!=null)
                m_SoulGrids = new SoulGrids(m_SoulGridTrans);
            if (m_ILEquipIcon != null)
                m_EquipIcon = (EquipIcon)m_ILEquipIcon.GetObject();
        }

        public void SetData(int subType, NetProto.SoulGrids data, Action<int> onClickCallBack)
        {
            m_Subtype = subType;
            EquipMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr as EquipMgr;
            var itemData = equipMgr.GetEquipData(subType);
            if (itemData != null)
                m_EquipIcon.SetData(itemData, Config.EquipPrototype.Get(itemData.id));
            else
                m_EquipIcon.Reset();
            if (data != null)
                m_SoulGrids.SetData(data);
            m_Text.text = EquipCfgMgr.GetEquipTypeName((Config.EquipPartsType)subType) +" · 部位";

            if (m_Btn != null)
                m_Btn.onClick.AddListener(()=> { onClickCallBack(m_Subtype); });
        }

        public NetProto.SoulGrids GetData()
        {
            return m_SoulGrids.data;
        }
        public void SetFocus(bool active)
        {
            m_Focus.gameObject.SetActive(active);
        }
    }
}
#endif

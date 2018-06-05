#if !USE_HOT

using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;
using UnityEngine.Events;

namespace xys.hot.UI
{
    using Config;
    using xys.UI;
    [AutoILMono]
    class EquipIcon
    {
        [SerializeField]
        Image m_EquipIconSp;
        [SerializeField]
        Image m_EquipQualitySp;
        [SerializeField]
        Button m_Btn;

        const string DefaultEquipIcon = "i11113_unequip";
        const string DefaultQualityIcon = "ui_Common_Quality_White";
        ItemData m_ItemData = new ItemData();
        public void Awake()
        {
            if (null != m_Btn)
                m_Btn.onClick.AddListener(() =>
                {
                    if (Config.EquipPrototype.Get(m_ItemData.id) != null)
                    {
                        InitItemTipsData param = new InitItemTipsData();
                        NetProto.ItemGrid itemGrid = new NetProto.ItemGrid();
                        itemGrid.data = m_ItemData;
                        param.itemData = itemGrid;
                        param.type = InitItemTipsData.Type.CommonEquipTips;
                        App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
                    }
                });
        }

        public void SetData(ItemData itemData,EquipPrototype cfg)
        {
            m_ItemData = itemData;
            Helper.SetSprite(m_EquipIconSp, cfg.icon);
            Helper.SetSprite(m_EquipQualitySp, Config.QualitySourceConfig.Get(cfg.quality).icon);
        }

        public void Reset()
        {
            Helper.SetSprite(m_EquipIconSp, DefaultEquipIcon);
            Helper.SetSprite(m_EquipQualitySp, DefaultQualityIcon);
        }
        public void SetOnClickListener(UnityAction action)
        {
            if (m_Btn!=null)
            {
                m_Btn.onClick.RemoveAllListeners();
                m_Btn.onClick.AddListener(action);
            }
        }
    }
}
#endif

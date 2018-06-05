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
using xys.UI;
using Config;

namespace xys.hot.UI
{ 
    class UISoulIcon
    {
        Image m_AddSp;
        Image m_Icon;
        Image m_LockSp;
        Image m_Focus;
        Text m_PlayerLvText;
        Text m_SoulLvText;
        Transform m_Trans;
        StateRoot m_StateRoot;
        SoulGrids.State m_CurrentState;
        int m_Index;
        Action<int> m_CallBackAction;
        public UISoulIcon(Transform trans, int index, Action<int> callBack)
        {
            m_Trans = trans;
            m_Index = index;
            m_CallBackAction = callBack;
            m_AddSp = trans.Find("Add").GetComponent<Image>();
            m_Icon = trans.Find("Icon").GetComponent<Image>();
            m_LockSp = trans.Find("Lock").GetComponent<Image>();
            m_PlayerLvText = trans.Find("PlayerLv").GetComponent<Text>();
            m_SoulLvText = trans.Find("SoulLv").GetComponent<Text>();
            m_StateRoot = trans.GetComponent<StateRoot>();
            m_StateRoot.uCurrentButton.onClick.AddListener(()=> {
                callBack(index);
            });
        }
        public void SetData(int soulID)
        {
            SetState(SoulGrids.State.State_Attached);
            var cfg = Config.SoulAttributePrototype.Get(soulID);
            if (cfg !=null)
            {
                m_SoulLvText.text = cfg.inforceLevel.ToString() + "级";
                Helper.SetSprite(m_Icon, Config.Item.Get(soulID).icon);
            }
        }
        public void SetState(SoulGrids.State state)
        {
            m_CurrentState = state;
            m_StateRoot.SetCurrentState((int)state,false);
            if (state == SoulGrids.State.State_Disable)
            {
                int requireLv = 0;
                m_PlayerLvText.text = requireLv.ToString();
            }
                
        }
        public void SetFocus(bool isActive)
        {
            m_Focus.gameObject.SetActive(isActive);
        }
    }
}
#endif

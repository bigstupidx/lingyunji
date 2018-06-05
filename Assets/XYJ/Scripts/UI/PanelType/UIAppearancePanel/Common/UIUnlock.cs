#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
namespace xys.hot.UI
{
    [AutoILMono]
    class UIUnlock
    {
        [SerializeField]
        StateRoot m_UnLockSR;
        [SerializeField]
        Text m_MaterialNeed;
        [SerializeField]
        Text m_NumNeed;
        private System.Action m_clickEvent = null;
        void Awake()
        {
            if(m_UnLockSR!=null)
            {
                m_UnLockSR.onClick.AddListener(OnClick);
            }
        }
        public void Set(int state,int materialId,int curOwn, int needNum, System.Action clickEvent)
        {
            m_UnLockSR.SetCurrentState(state, false);
            Config.Item tempItem = Config.Item.Get(materialId);
            if(tempItem!=null)
            {
                m_MaterialNeed.text = tempItem.name;
            }
            
            if(curOwn < needNum)
            {
                m_NumNeed.color = Color.red;
                m_NumNeed.text = curOwn.ToString() + "/" + needNum.ToString();
            }
            else
            {
                m_NumNeed.color = Color.green;
                m_NumNeed.text = curOwn.ToString() + "/" + needNum.ToString();
            }
            m_clickEvent = null;
            m_clickEvent = clickEvent;

        }
        void OnClick()
        {
            if(m_clickEvent!=null)
            {
                m_clickEvent();
            }
        }

    }
}
#endif
#if !USE_HOT
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using UnityEngine;
using xys.UI.State;
using Config;
using xys.UI;
using UnityEngine.Events;
using xys.hot.Event;

namespace xys.hot.UI
{
    [AutoILMono]
    class WelfareRwdColumn
    {
        public enum ColumnType
        {
            TYPE_LVRWD,
            TYPE_OLRWD,
            TYPE_FESTIVALRWD,
            TYPE_DAYOLRWD
        }
        public enum BtnState
        {
            STATE_ENABLE,
            STATE_RECEIVED,
            STATE_DISABLE
        }
        [SerializeField]
        GameObject m_AwardItem;
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Text m_BtnText;
        [SerializeField]
        Text m_InfoText;
        [SerializeField]
        Button m_AwardBtn;
        [SerializeField]
        Transform m_Grid;

        public Transform transform { get { return m_Transform; } }

        List<WelfareRwdItem> m_ItemInstanceList = new List<WelfareRwdItem>();
        HotObjectEventAgent m_Event;
        bool rwdShowed = false;
        int m_Index = -1;
        protected void Awake()
        {
            SetReceiveBtnDisable();
        }
        
        public void OnEnable()
        {
            if (m_Event!=null)
            {
                m_Event.Release();
                m_Event = null;
            }
            m_Event = new HotObjectEventAgent(App.my.localPlayer.eventSet);
        }
        public void OnDisable()
        {
            if (m_Event != null)
            {
                m_Event.Release();
                m_Event = null;
            } 
        }
        public void SetData(ColumnType type,int index, ItemCount[] itemList)
        {
            m_Index = index;
            for (int i = 0; i < itemList.Length; i++)
            {
                GameObject obj = GameObject.Instantiate(m_AwardItem);
                obj.SetActive(true);
                obj.transform.SetParent(m_Transform.Find("Scroll View").Find("Grid"));
                obj.transform.localScale = Vector3.one;
                WelfareRwdItem item = (WelfareRwdItem)obj.GetComponent<ILMonoBehaviour>().GetObject();
                item.SetData(i, itemList[i]);
                m_ItemInstanceList.Add(item);
            }

            EventID myEvent; 
            switch (type)
            {
                case ColumnType.TYPE_DAYOLRWD:
                    myEvent = EventID.Welfare_GetSevendayRwd; break;
                case ColumnType.TYPE_OLRWD:
                    myEvent = EventID.Welfare_GetOnlineRwd; break;
                case ColumnType.TYPE_LVRWD:
                    myEvent = EventID.Welfare_GetLevelRwd; break;
                default:
                    myEvent = EventID.Welfare_GetLevelRwd; break;
            }
            m_AwardBtn.onClick.AddListener(() =>
            {
                if (-1!=m_Index)
                {
                    m_Event.FireEvent<int>(myEvent, m_Index + 1);
                }
            }
            );
        }
        //设置按钮文字
        public void SetBtnText(string str)
        {
            m_BtnText.text = str;
        }
        //设置信息文字
        public void SetInfoText(string str)
        {
            m_InfoText.text = str;
        }
        //激活可领取状态
        public void SetReceiveBtnEnable()
        {
            m_AwardBtn.GetComponent<StateRoot>().SetCurrentState((int)BtnState.STATE_ENABLE, true);
            m_BtnText.text = "可领取";
            m_AwardBtn.enabled = true;
        }
        //激活不可领取状态
        public void SetReceiveBtnDisable()
        {
            m_AwardBtn.GetComponent<StateRoot>().SetCurrentState((int)BtnState.STATE_DISABLE, true);
            m_BtnText.text = "领取";
            m_AwardBtn.enabled = false;
        }
        //激活已领取状态
        public void SetReceiveBtnReceived()
        {
            m_AwardBtn.GetComponent<StateRoot>().SetCurrentState((int)BtnState.STATE_RECEIVED, true);
            m_AwardBtn.enabled = false;
            m_BtnText.text = "已领取";
        }
        //获取当前按钮状态
        public int GetReceiveBtnStatus()
        {
            return m_AwardBtn.GetComponent<StateRoot>().CurrentState;
        }

        public void SetClickListener(UnityAction action)
        {
            m_AwardBtn.onClick.AddListener(action);
        }
    }
}

#endif
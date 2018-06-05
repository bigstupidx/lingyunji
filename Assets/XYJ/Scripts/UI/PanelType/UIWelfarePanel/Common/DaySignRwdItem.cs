#if !USE_HOT
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Config;
using xys.UI;
using xys.hot.Event;

namespace xys.hot.UI
{
    [AutoILMono]
    class DaySignRwdItem : WelfareRwdItem
    {
        [SerializeField]
        Image m_SignedSp;
        [SerializeField]
        Image m_HighLightSp;
        int m_Index;
        //初始化
        void Awake()
        {
            m_ItemBtn.onClick.AddListener(OnItemClick);
        }
        //设置数据
        public void SetData(int index,ItemCount[] items)
        {
            m_Index = index;
            
            if (items.Length>1)
            {
                Debug.Log("SignRwd Cfg Error");
            }
            else
            {
                // 设置Item SP。数量等
                base.SetData(index,items[0]);
            }
        }
        //设置已签到状态
        public  void SetSigned(bool signed)
        {
            m_SignedSp.gameObject.SetActive(signed);
            //m_HighLightSp.gameObject.SetActive(!signed);
        }
        //点击回调
        void OnItemClick()
        {
            m_Event.FireEvent<int>(EventID.Welfare_OnSignItem, m_Index);
        }
        //设置高亮状态
        public void SetHighLight(bool isActive)
        {
            m_HighLightSp.gameObject.SetActive(isActive);
        }

        //public void ShowObtainItem()
        //{
        //    List<Obtain> obainList = new List<xys.UI.Obtain>();
        //    Obtain obtain = new Obtain(base.itemID, base.count);
        //    obainList.Add(obtain);
        //    ObtainItemShowMgr.ShowObtain(obainList);
        //}
    }


}

#endif
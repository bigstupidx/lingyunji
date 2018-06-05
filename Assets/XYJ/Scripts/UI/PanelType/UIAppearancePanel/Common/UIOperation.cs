#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using NetProto;
namespace xys.hot.UI
{
    [AutoILMono]
    class UIOperation
    {
        [SerializeField]
        ILMonoBehaviour m_ilColorMake;
        UIColorMake m_colorMake;
        [SerializeField]
        ILMonoBehaviour m_ilClothContent;
        UIClothContent m_clothContent;

        [SerializeField]
        Button m_paint;
        [SerializeField]
        StateRoot m_wear;

        [SerializeField]
        Transform m_colorRoot;

        HotAppearanceModule module;
        AppearanceMgr m_appearanceMgr ;
        ClothHandle m_clothHandle ;
        RoleDisguiseHandle m_disguiseHandle;

        private List<UIColorBtn> m_colorBtnList = new List<UIColorBtn>();
        public ClothItem m_clothItem = null;
  
        public bool m_isChangeRole = false;

        void Awake()
        {
            if(m_ilClothContent!=null)
            {
                m_clothContent = m_ilClothContent.GetObject() as UIClothContent;
            }
            if(m_ilColorMake!=null)
            {
                m_colorMake = m_ilColorMake.GetObject() as UIColorMake;
            }
            if (m_colorRoot != null)
            {
                int count = m_colorRoot.childCount;
                for (int i = 0; i < count; i++)
                {
                    Transform tempTran = m_colorRoot.GetChild(i);
                    if (tempTran == null) return;                  
                    ILMonoBehaviour tempILObj = tempTran.GetComponent<ILMonoBehaviour>();
                    if (tempILObj == null) return;
                    UIColorBtn tempItem = tempILObj.GetObject() as UIColorBtn;
                    m_colorBtnList.Add(tempItem);

                }
            }
            if(m_paint!=null)
            {
                m_paint.onClick.AddListener(OnClickPaint);
            }
            if(m_wear!=null)
            {
                m_wear.onClick.AddListener(OnClickWear);
            }

            module=hotApp.my.GetModule<HotAppearanceModule>();
            m_appearanceMgr = module.GetMgr();
            m_clothHandle = m_appearanceMgr.GetClothHandle();
            m_disguiseHandle = m_appearanceMgr.GetDisguiHandle();
        }
        public void Set()
        {
            if(m_clothItem != null)
            {
                int curCount = m_clothItem.GetColorList().Count;
                for(int i=0;i< curCount;i++)
                {
                    m_colorBtnList[i].Set(i, UIColorBtn.State.Unlock_Unselected, m_clothItem.GetColorList()[i], OnClick, null);
                }
                for(int i=curCount;i<m_colorBtnList.Count;i++)
                {
                    m_colorBtnList[i].Set(i,UIColorBtn.State.Null_Color);
                }

                if (m_clothItem.m_id == m_clothHandle.m_roleClothData.m_clothId&&
                    m_clothItem.m_curColor == m_clothHandle.m_roleClothData.m_curColor)
                {
                    m_wear.SetCurrentState(1, false);
                }
                else
                {
                    m_wear.SetCurrentState(0, false);
                }  

                if(m_clothItem.m_curColor>=0&& m_clothItem.m_curColor< m_colorBtnList.Count)
                {
                    m_colorBtnList[m_clothItem.m_curColor].SetState(UIColorBtn.State.Unlock_Selected, false);
                }
                else
                {
                    m_colorBtnList[0].SetState(UIColorBtn.State.Unlock_Selected, false);
                }
                
            }
        }
        void OnClick(UIColorBtn item)
        {
            if(m_clothItem.m_curColor == item.m_index)
            {
                return;
            }
            else
            {
                m_colorBtnList[m_clothItem.m_curColor].SetState(UIColorBtn.State.Unlock_Unselected, true);
                m_clothItem.m_curColor = item.m_index;
                item.SetState(UIColorBtn.State.Unlock_Selected, true);

                if (m_clothItem.m_id!=m_clothHandle.m_roleClothData.m_clothId||
                    m_clothItem.m_curColor != m_clothHandle.m_roleClothData.m_curColor)
                {
                    m_wear.SetCurrentState(0, false);
                }
                else
                {
                    m_wear.SetCurrentState(1, false);
                }

                Color temp = m_clothItem.GetColorList()[item.m_index];
                m_disguiseHandle.SetClothColor(temp);
                //改变角色服装颜色
            }
        }
        //染色
        void OnClickPaint()
        {            
            m_clothContent.CloseContent();
            m_colorMake.Open(m_clothItem);
        }
        //装备，卸下
        void OnClickWear()
        {
            if(m_wear.CurrentState==0)
            {
                WearCloth();
            }
            else
            {
                RemoveCloth();
            }
        }
        void WearCloth()
        {
            //更新预览
            //改变handle中roledata  
            //切换页面时，更新服务器数据

            m_clothHandle.m_roleClothData.m_clothId = m_clothItem.m_id;
            m_clothHandle.m_roleClothData.m_curColor = m_clothItem.m_curColor;
            m_isChangeRole = true;
            m_wear.SetCurrentState(1, false);
        }
        void RemoveCloth()
        {
            //更新预览
            //改变handle中roledata 
            m_disguiseHandle.SetDefaultCloth();
            m_isChangeRole = true;            
            m_wear.SetCurrentState(0, false);
            
        }



    }
}
#endif
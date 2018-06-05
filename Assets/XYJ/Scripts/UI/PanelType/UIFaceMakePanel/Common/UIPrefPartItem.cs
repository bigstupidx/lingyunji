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
    class UIPrefPartItem
    {
        [SerializeField]
        public StateRoot m_StateRoot;

        [SerializeField]
        Transform m_NomalRoot;
        [SerializeField]
        Transform m_ColorItemRoot;


        [SerializeField]
        Transform m_TexItemRoot;
        [SerializeField]
        GameObject m_ContentGO;


        GameObject m_ItemPref = null;
        Transform m_ItemSource = null;

        [SerializeField]
        Text m_TitleText;
        public int m_PartIndex = 0;
        public int m_curTabIndex = 0;
        public int m_curTexItem = 0;
        private List<UITextureItem> m_texItemList = new List<UITextureItem>();
        private System.Action<UIPrefPartItem> m_ClickEvent = null;
        private System.Action<UIPrefPartItem> m_StateChangeEvent = null;
        protected UIFaceMakePanel m_FaceMakeModule=null;
      
        void Awake()
        {
            m_StateRoot.onClick.AddListener(OnClick);
            m_StateRoot.onStateChange.AddListener(OnStateChange);
        }
        public void Set(int partIndex,string name,System.Action<UIPrefPartItem> clickEvent, System.Action<UIPrefPartItem> stateChangeEvent)
        {
            InitPart();
            m_TitleText.text = name;
            m_PartIndex = partIndex;
            m_ClickEvent = clickEvent;
            m_StateChangeEvent = stateChangeEvent;
        }
        //初始化类数据
        void InitPart()
        {
            m_TitleText.text = "";
            m_PartIndex = 0;
            m_curTabIndex = 0;
            m_curTexItem = 0;
            m_ClickEvent = null;
            m_StateChangeEvent = null;
            m_FaceMakeModule = null;          
            m_StateRoot.SetCurrentState(0, false);
            
        }
        //打开分部位内容
        public void OpenPart(int tabIndex, UIFaceMakePanel faceMakeModule)
        {
            m_FaceMakeModule = faceMakeModule;
            RoleDisguiseHandle prefHandle= m_FaceMakeModule.GetDisguiseHandle();
            m_curTabIndex = tabIndex;
            
            if(m_ContentGO!=null)
            {
                m_ContentGO.SetActive(true);
            }
            if(m_TexItemRoot!=null)
            {
                m_TexItemRoot.gameObject.SetActive(true);
            }

            int[] tabsKeys = UIWholePage.s_tabKeys[m_curTabIndex];
            var type = tabsKeys[m_PartIndex];

            SetCurtexItemIndex(type, prefHandle);

            RoleDisguiseCareer config = prefHandle.GetOverallConfig();
            RoleDisguiseType partData = config.GetType(type);
            
            int partItemCount = partData.items.Count;
            for(int i=0;i<partItemCount;i++)
            {
                if (m_TexItemRoot == null) return;

                if (type==2||type==3)
                {
                    m_ItemSource = m_ColorItemRoot;
                }
                else
                {
                    m_ItemSource = m_NomalRoot;
                }
                if (m_ItemSource == null) return;
                int curChildCount = m_ItemSource.childCount;
                if(curChildCount>0)
                {
                    m_ItemPref = m_ItemSource.GetChild(0).gameObject;
                }
                if (m_ItemPref == null) return;
                GameObject tempObj = null;
                if (i < curChildCount)
                {
                    tempObj = m_ItemSource.GetChild(i).gameObject;
                }
                else
                {
                    tempObj = GameObject.Instantiate(m_ItemPref);
                }
                if (tempObj == null) return;
                tempObj.SetActive(true);
                tempObj.transform.SetParent(m_TexItemRoot, false);
                tempObj.transform.localScale = Vector3.one;
                     

                if (tempObj == null) return;
                ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                if (ILObj == null) return;
                UITextureItem item = (UITextureItem)ILObj.GetObject();
                var style = partData.items[i];
                if (string.IsNullOrEmpty(style.iconName))
                {
                     item.Set(i,  style.color.RBGFloatColor, OnClickItem, OnItemStateChange);   
                }
                else
                {
                    item.Set(i, 0,OnClickItem, OnItemStateChange, style.iconName);
                }
                
                m_texItemList.Add(item);
            }
            if(m_texItemList.Count>0)
            {
                if(m_curTexItem>=0&&m_curTexItem<m_texItemList.Count)
                {
                    m_texItemList[m_curTexItem].m_StateRoot.SetCurrentState(1, false);
                }
            }
        }
        void SetCurtexItemIndex(int type,RoleDisguiseHandle prefHandle)
        {
            int index = 0;
            switch(type)
            {
                case 1:
                    index = prefHandle.GetDisguiseItemIndex(type,prefHandle.GetOverallData().hairStyle);
                    break;
                case 2:
                    index = prefHandle.GetDisguiseItemIndex(type, prefHandle.GetOverallData().hairColorIdx);
                    break;
                case 3:
                    index = prefHandle.GetDisguiseItemIndex(type, prefHandle.GetOverallData().skinColorIdx); 
                    break;
                case 4:
                    index = -1;
                    break;
                default:
                    break;
            }

            m_curTexItem = index;
        
        }
        //初始化游戏对象状态
        public void ClosePart()
        {
            m_texItemList.Clear();           
            if (m_StateRoot!=null)
            {
                m_StateRoot.gameObject.SetActive(true);
            }
            if (m_ContentGO != null)
            {
                m_ContentGO.SetActive(false);
                int childCount = m_ContentGO.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    m_ContentGO.transform.GetChild(i).gameObject.SetActive(false);
                }
                if (m_TexItemRoot != null)
                {
                    while(m_TexItemRoot.childCount!=0)
                    {
                        m_TexItemRoot.GetChild(0).SetParent(m_ItemSource);
                    }
                }
            }
          
           
        }
        public void ResetUI()
        {
            //刷新界面
            if (m_StateRoot.CurrentState==1)
            {
                int[] tabsKeys = UIWholePage.s_tabKeys[m_curTabIndex];
                var type = tabsKeys[m_PartIndex];
                m_texItemList[m_curTexItem].m_StateRoot.SetCurrentState(0, false);
                SetCurtexItemIndex(type, m_FaceMakeModule.GetDisguiseHandle());
                if (m_curTexItem < 0 || m_curTexItem >= m_texItemList.Count)
                {
                    Debug.Log("当前 Type:" + type + "当前 index" + m_curTexItem);
                    m_curTexItem = -1;
                }
                if(m_curTexItem>=0)
                {
                    m_texItemList[m_curTexItem].m_StateRoot.SetCurrentState(1, false);
                }
                
            }
        }
        void OnClickItem(UITextureItem item)
        {
            if(m_curTabIndex==1)
            {
                if(UIFaceMakePanel.m_NeedComfirm)
                {
                    //显示二次确认框
                    xys.UI.Dialog.TwoBtn m_TwoBtn = new xys.UI.Dialog.TwoBtn();
                    m_TwoBtn = xys.UI.Dialog.TwoBtn.Show("", "重新选择脸型/默认方案，将重置之前对脸型的妆扮及修容操作，是否确认？",
                    "取消", () => 
                    {
                        if (m_curTexItem == item.m_Index)
                        {
                            item.m_StateRoot.SetCurrentState(1, false);
                        }
                        else
                        {
                            if(m_curTexItem>=0)
                            {
                                m_texItemList[m_curTexItem].m_StateRoot.SetCurrentState(0, false);
                            }
                            m_curTexItem = item.m_Index;
                            item.m_StateRoot.SetCurrentState(1, false);
                        }
                        return false;
                    } ,
                    "确定", () =>
                    {
                        if (m_curTexItem == item.m_Index)
                        {
                            item.m_StateRoot.SetCurrentState(1, true);
                        }
                        else
                        {
                            if (m_curTexItem >= 0)
                            {
                                m_texItemList[m_curTexItem].m_StateRoot.SetCurrentState(0, false);
                            }
                            m_curTexItem = item.m_Index;
                            item.m_StateRoot.SetCurrentState(1, true);
                        }
                        UIFaceMakePanel.m_NeedComfirm = false;
                        return false;
                    }, true, true, () => { m_TwoBtn = null; });
                    return;
                }

            }

            if (m_curTexItem == item.m_Index)
            {
                return;
            }
            else
            {
                if (m_curTexItem >= 0)
                {
                    m_texItemList[m_curTexItem].m_StateRoot.SetCurrentState(0, true);
                }
                m_curTexItem = item.m_Index;
                item.m_StateRoot.SetCurrentState(1, true);
            }



        }
        void OnItemStateChange(UITextureItem item)
        {          
            if (item.m_StateRoot.CurrentState==0)
            {
                return;
            }
            else
            {
                int[] tabsKeys = UIWholePage.s_tabKeys[m_curTabIndex];
                var type = tabsKeys[m_PartIndex];
                
                RoleDisguiseHandle prefHandle = m_FaceMakeModule.GetDisguiseHandle();
                prefHandle.SetStyle(type,item.m_Index);
            }

        }
        void OnClick()
        {
            if(m_ClickEvent!=null)
            {
                m_ClickEvent(this);
            }
        }
        void OnStateChange()
        {
            if(m_StateChangeEvent!=null)
            {
                m_StateChangeEvent(this);
            }
        }

    }
}

#endif
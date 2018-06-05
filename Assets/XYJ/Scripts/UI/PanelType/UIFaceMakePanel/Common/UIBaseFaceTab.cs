#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using Config;
namespace xys.hot.UI
{
    [AutoILMono]
    class UIBaseFaceTab
    {
        [SerializeField]
        Transform m_BaseFaceRoot;
        [SerializeField]
        GameObject m_BaseFaceItemPref;

        [SerializeField]
        Transform m_ManItemRoot;
        [SerializeField]
        Transform m_WomenItemRoot;


        Transform m_ItemSource = null;

        [SerializeField]
        Transform m_PresettingRoot;
        [SerializeField]
        Transform m_PresettingSource;




        [SerializeField]
        GameObject m_Tips;
        [SerializeField]
        Button m_deletePresetBtn;
        [SerializeField]
        Button m_sharePresetBtn;

        protected List<UITextureItem> m_BaseFaceItemList = new List<UITextureItem>();
        protected List<UITextureItem> m_PresetItemList = new List<UITextureItem>();

        UIFaceMakePanel m_FaceMakeModule = null;
        RTTModelPart m_RttModelPart = null;
        public int m_curBaseItemIndex = 0;
        public int m_curPresetItemIndex = 0;
        public static int m_ildlePresetCount = 5;
        int m_activeCount = 0;
        void Awake()
        {
            if(m_deletePresetBtn != null)
            {
                m_deletePresetBtn.onClick.AddListener(OnClickDelete);
            }
            if(m_sharePresetBtn != null)
            {
                m_sharePresetBtn.onClick.AddListener(OnClickShare);
            }
        }
        public void OpenTab(UIFaceMakePanel faceMakeModule)
        {
            InitTab();
            m_FaceMakeModule = faceMakeModule;

            if (m_FaceMakeModule != null)
            {
                m_RttModelPart = m_FaceMakeModule.GetRttModlePart();
                if (m_RttModelPart != null)
                {
                    m_RttModelPart.SetCamState(0, false);
                }
            }

            SetCurBaseFace(m_FaceMakeModule.GetOverallData());

            int baseFaceCount = 2;
            for (int i=0;i< baseFaceCount;i++)
            {
                if (m_BaseFaceRoot == null) return;

                if (m_FaceMakeModule.GetDisguiseHandle().m_sex==1)
                {
                    m_ItemSource = m_ManItemRoot;
                }
                else
                {
                    m_ItemSource = m_WomenItemRoot;
                }
                if (m_ItemSource == null) return;
                GameObject tempObj = null;
                if(m_ItemSource.childCount>0)
                {
                    tempObj = m_ItemSource.GetChild(0).gameObject;
                }
                
                if (tempObj == null) return;
                tempObj.SetActive(true);
                tempObj.transform.SetParent(m_BaseFaceRoot, false);
                tempObj.transform.localScale = Vector3.one;
                ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                UITextureItem item = (UITextureItem)ILObj.GetObject();
                item.Set(i,0, OnClickBaseItem, OnBaseItemStateChange);
                m_BaseFaceItemList.Add(item);
            }
            m_BaseFaceItemList[m_curBaseItemIndex].m_StateRoot.SetCurrentState(1, false);
            OpenPresettingPart();   
        }
        void OpenPresettingPart()
        {
            InitContent();
            SetContent();
            OnEndSetContent();
        }
        void InitContent()
        {
            RoleDisguiseHandle disguiseHandle = m_FaceMakeModule.GetDisguiseHandle();
            RoleDisguiseCareer config = disguiseHandle.GetOverallConfig();
            int[] tabsKeys = UIWholePage.s_tabKeys[0];
            var type = tabsKeys[0];
            RoleDisguiseType partData = config.GetType(type);
            if (partData == null)
            {
                Debuger.ErrorLog("partData == null");
                return;
            }

            if (partData.items == null)
            {
                Debuger.ErrorLog("partData.items == null");
                return;
            }
            if(m_ildlePresetCount!=0)
            {
                if (m_curBaseItemIndex == 0)
                {
                    kvCommon limitNumItem = kvCommon.Get("FaceTypeHoldNumber0");
                    int limitNum = 0;
                    int.TryParse(limitNumItem.value, out limitNum);
                    m_ildlePresetCount = limitNum;
                }
                else
                {
                    kvCommon limitNumItem = kvCommon.Get("FaceTypeHoldNumber1");
                    int limitNum = 0;
                    int.TryParse(limitNumItem.value, out limitNum);
                    m_ildlePresetCount = limitNum;
                }
            }           
            int presetItemCount = partData.items.Count + m_ildlePresetCount;
            UITextureItem.AddNullItem(presetItemCount, m_PresettingRoot, m_PresettingSource);
        }
        void SetContent()
        {
            m_PresetItemList.Clear();
            m_curPresetItemIndex = 0;
            m_activeCount = 0;
            RoleDisguiseHandle disguiseHandle = m_FaceMakeModule.GetDisguiseHandle();
            RoleDisguiseCareer config = disguiseHandle.GetOverallConfig();
            int[] tabsKeys = UIWholePage.s_tabKeys[0];
            var type = tabsKeys[0];
            RoleDisguiseType partData = config.GetType(type);

            for (int i = 0; i < m_PresettingRoot.childCount; i++)
            {
                GameObject tempObj = null;
                tempObj = m_PresettingRoot.GetChild(i).gameObject;
                if (tempObj == null) return;
                ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                UITextureItem item = (UITextureItem)ILObj.GetObject();

                if(m_ildlePresetCount==0)
                {
                    string name = partData.items[i].iconName;
                    if (name != null && name != "")
                    {
                        item.Set(i, 0,OnClickPresetItem, OnPresetItemStateChange, partData.items[i].iconName);
                    }
                }
                else
                { 
                    if (i < partData.items.Count)
                    {
                        string name = partData.items[i].iconName;
                        if (name != null && name != "")
                        {
                            item.Set(i,0, OnClickPresetItem, OnPresetItemStateChange, partData.items[i].iconName);
                        }
                    }
                    else if(i>= partData.items.Count&&i< GetActiveCount())
                    {

                        item.Set(i, 3 , OnClickPresetItem, OnPresetItemStateChange);//自定义的预设
                    }
                    else
                    {
                        item.Set(i, 2 ,OnClickPresetItem, OnPresetItemStateChange);
                    }
                }
                m_PresetItemList.Add(item);
            }
        }
        void OnEndSetContent()
        {
            if (m_PresetItemList.Count > 0)
            {
                if(m_ildlePresetCount==0)
                {
                    if (m_curPresetItemIndex >= 0 && m_curPresetItemIndex < m_PresetItemList.Count)
                    {
                        m_PresetItemList[m_curPresetItemIndex].m_StateRoot.SetNextStateWithLoop(false);
                    }
                }
                else
                {
                    if (m_curPresetItemIndex >= 0 && m_curPresetItemIndex < GetActiveCount())
                    {
                        m_PresetItemList[m_curPresetItemIndex].m_StateRoot.SetNextStateWithLoop(false);
                    }
                        
                }
            }
        }
        void ClosePressettingPart()
        {
            UITextureItem.RecycleItem(m_PresettingRoot, m_PresettingSource);
            m_Tips.SetActive(false);     
        }
        void InitTab()
        {
            m_BaseFaceItemList.Clear();
            m_PresetItemList.Clear();
        }
        void SetCurBaseFace(RoleDisguiseOverallData overallData)
        {
            m_curBaseItemIndex = overallData.faceType;
        }

        public void CloseTab()
        {
            if(m_BaseFaceRoot!=null)
            {
                while(m_BaseFaceRoot.childCount>0)
                {
                    m_BaseFaceRoot.GetChild(0).SetParent(m_ItemSource);
                }
            }

            ClosePressettingPart();
        }
        void OnClickBaseItem(UITextureItem item)
        {
            if (UIFaceMakePanel.m_NeedComfirm)
            {
                //显示再次确认框
                xys.UI.Dialog.TwoBtn m_TwoBtn = new xys.UI.Dialog.TwoBtn();
                m_TwoBtn = xys.UI.Dialog.TwoBtn.Show("", "重新选择脸型/默认方案，将重置之前对脸型的妆扮及修容操作，是否确认？",
                "取消", () =>
                {
                    return false;
                },
                "确定", () =>
                {
                    if (m_curBaseItemIndex != item.m_Index)
                    {
                        m_BaseFaceItemList[m_curBaseItemIndex].m_StateRoot.FrontState();
                        m_curBaseItemIndex = item.m_Index;
                        item.m_StateRoot.NextState();
                    }
                    UIFaceMakePanel.m_NeedComfirm = false;
                    return false;
                }, true, true, () => { m_TwoBtn = null; });

                return;
            }
            if (item.m_Index==m_curBaseItemIndex)
            {
                return;
            }
            else
            {
                m_BaseFaceItemList[m_curBaseItemIndex].m_StateRoot.SetCurrentState(0, true);
                item.m_StateRoot.SetCurrentState(1, true);
            }
        }
        void OnBaseItemStateChange(UITextureItem item)
        {
            if(item.m_StateRoot.CurrentState==0)
            {
                return;
            }
            else
            {
                m_curBaseItemIndex = item.m_Index;

                RoleDisguiseHandle disguiseHandle = m_FaceMakeModule.GetDisguiseHandle();
                if (m_RttModelPart==null)
                    m_RttModelPart = m_FaceMakeModule.GetRttModlePart();
                m_RttModelPart.ResetCamScale();
                disguiseHandle.ChangeFaceStyle(item.m_Index);

                ClosePressettingPart();
                OpenPresettingPart();
            }
        }
        void OnClickPresetItem(UITextureItem item)
        {
            if (UIFaceMakePanel.m_NeedComfirm)
            {
                //显示再次确认框
                //显示二次确认框
                xys.UI.Dialog.TwoBtn m_TwoBtn = new xys.UI.Dialog.TwoBtn();
                m_TwoBtn = xys.UI.Dialog.TwoBtn.Show("", "重新选择脸型/默认方案，将重置之前对脸型的妆扮及修容操作，是否确认？",
                "取消", () =>
                {
                    if (m_curPresetItemIndex != item.m_Index)
                    {
                        if(m_curPresetItemIndex>=0)
                        {
                            m_PresetItemList[m_curPresetItemIndex].m_StateRoot.SetFrontStateWithLoop(false);
                        }      
                        m_curPresetItemIndex = item.m_Index;
                       
                        item.m_StateRoot.SetNextStateWithLoop(false);
                    }
                    return false;
                },
                "确定", () =>
                {
                    if (m_curPresetItemIndex == item.m_Index)
                    {
                        item.m_StateRoot.SetNextStateWithLoop(true);
                    }
                    else
                    {
                        if(m_curPresetItemIndex >= 0)
                        {
                            
                            m_PresetItemList[m_curPresetItemIndex].m_StateRoot.SetFrontStateWithLoop(false);
                        }
                        
                        m_curPresetItemIndex = item.m_Index;
                       
                        item.m_StateRoot.SetNextStateWithLoop(true);
                    }
                    UIFaceMakePanel.m_NeedComfirm = false;
                    return false;
                }, true, true, () => { m_TwoBtn = null; });

                return;
            }

            if (m_ildlePresetCount != 0)
            {
                RoleDisguiseHandle disguiseHandle = m_FaceMakeModule.GetDisguiseHandle();
                RoleDisguiseCareer config = disguiseHandle.GetOverallConfig();
                int[] tabsKeys = UIWholePage.s_tabKeys[0];
                var type = tabsKeys[0];
                RoleDisguiseType partData = config.GetType(type);

                int activeItemCount = GetActiveCount();
                if (item.m_Index >= partData.items.Count && item.m_Index < activeItemCount)
                {
                    if (m_Tips != null)
                    {
                        m_Tips.SetActive(true);
                        m_Tips.GetComponent<RectTransform>().anchoredPosition = item.m_rectTran.anchoredPosition + new Vector2(-334, 255);
                    }
                }
                else
                {
                    if (m_Tips != null)
                    {
                        m_Tips.SetActive(false);
                    }
                }
                if(item.m_Index< activeItemCount)
                {
                    if (m_curPresetItemIndex == item.m_Index)
                    {
                        return;
                    }
                    else
                    {
                        if(m_curPresetItemIndex>=0)
                        {
                            m_PresetItemList[m_curPresetItemIndex].m_StateRoot.FrontState();
                        }
                        item.m_StateRoot.NextState();
                    }
                }          
            }
            else
            {
                if (m_curPresetItemIndex == item.m_Index)
                {
                    return;
                }
                else
                {
                    if(m_curPresetItemIndex >= 0)
                    {
                        m_PresetItemList[m_curPresetItemIndex].m_StateRoot.FrontState(); 
                    }
                    item.m_StateRoot.NextState();
                }
            }
            
        }
        void OnPresetItemStateChange(UITextureItem item)
        {
            if(item.m_StateRoot.CurrentState==0)
            {
                return;
            }
            else
            {
                m_curPresetItemIndex = item.m_Index;
                
                RoleDisguiseHandle disguiseHandle = m_FaceMakeModule.GetDisguiseHandle();
                RoleDisguiseCareer config = disguiseHandle.GetOverallConfig();
                int[] tabsKeys = UIWholePage.s_tabKeys[0];
                var type = tabsKeys[0];
                RoleDisguiseType partData = config.GetType(type);
                if(item.m_Index<partData.items.Count)
                {
                    disguiseHandle.SetStyle(type, item.m_Index);
                }
                else
                {
                    HotAppearanceModule module = hotApp.my.GetModule<HotAppearanceModule>();
                    NetProto.AppearanceData appearanceData = module.GetAppearanceData();
                    int index = item.m_Index - partData.items.Count;
                    string presetting = null;
                    if(m_curBaseItemIndex==0)
                    {
                        presetting = appearanceData.presettings_0[index].persetting;
                    }
                    else
                    {
                        presetting = appearanceData.presettings_1[index].persetting;
                    }
                    if (presetting != null )
                    {
                        disguiseHandle.SetByPresetting(presetting);
                    }
                    
                }   
            }
        }
        void OnClickDelete()
        {
            RoleDisguiseHandle disguiseHandle = m_FaceMakeModule.GetDisguiseHandle();
            RoleDisguiseCareer config = disguiseHandle.GetOverallConfig();
            int[] tabsKeys = UIWholePage.s_tabKeys[0];
            var type = tabsKeys[0];
            RoleDisguiseType partData = config.GetType(type);

            HotAppearanceModule module = hotApp.my.GetModule<HotAppearanceModule>();
            if(module!=null)
            {
                NetProto.PresettingDelReq request = new NetProto.PresettingDelReq();
                request.faceType = m_curBaseItemIndex;
                request.index = m_curPresetItemIndex- partData.items.Count;
                module.Event.FireEvent(EventID.Ap_DeletePreset, request);
            }
            Debug.Log("删除预设");
            m_Tips.SetActive(false);
        }
        void OnClickShare()
        {
            Debug.Log("分享预设");
            m_Tips.SetActive(false);
        }

        public void RefreshUI()
        {
            SetContent();
            OnEndSetContent();
        }

        int GetActiveCount()
        {
            if(m_activeCount==0)
            {
                
                RoleDisguiseHandle disguiseHandle = m_FaceMakeModule.GetDisguiseHandle();
                RoleDisguiseCareer config = disguiseHandle.GetOverallConfig();
                int[] tabsKeys = UIWholePage.s_tabKeys[0];
                var type = tabsKeys[0];
                RoleDisguiseType partData = config.GetType(type);
                HotAppearanceModule module = hotApp.my.GetModule<HotAppearanceModule>();
                NetProto.AppearanceData appearanceData = module.GetAppearanceData();

                m_activeCount = 0;
                if (m_curBaseItemIndex == 0)
                {
                    m_activeCount = partData.items.Count + appearanceData.presettings_0.Count;
                }
                else
                {
                    m_activeCount = partData.items.Count + appearanceData.presettings_1.Count;
                }
            }            
            return m_activeCount;
        }
    }
}
#endif

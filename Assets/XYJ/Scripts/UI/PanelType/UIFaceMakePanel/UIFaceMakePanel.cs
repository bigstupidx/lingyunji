#if !USE_HOT
using xys.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Config;
using xys.UI.State;
namespace xys.hot.UI
{
    class UIFaceMakePanel : HotTablePanelBase
    {
        public static bool m_NeedComfirm = false;
        public enum Page
        {
            Whole=0,   //整体
            Makeup=1,  //妆扮
            Modify=2,  //修容
        }

        public UIFaceMakePanel() : base(null) { }
        public UIFaceMakePanel(HotTablePanel _self) : base(_self)
        {

        }
        [SerializeField]
        Button m_BackBtn;
        [SerializeField]
        StateRoot m_ApplyBtnState;
        [SerializeField]
        ILMonoBehaviour m_ILNameFiled;
        UINameField m_UINameFiled;
        [SerializeField]
        ILMonoBehaviour m_ILApplyTab;
        UIApplyTab m_UIApplyTab;
      

        #region 数据部分

        public RectTransform m_rttUIObject;


        RTTModelPartHandler m_rttHandler=null;
        RoleDisguiseHandle m_disguiseHandle;
        RoleSkinHandle m_skinMerge;
        RoleShapeHandle m_shapeMgr;


        public RTTModelPart GetRttModlePart()
        {
            if(m_rttHandler!=null)
            {
                return m_rttHandler.GetRTT();
            }
            else
            {
                return null;
            }
            
        }
        public RoleDisguiseOverallData GetOverallData()
        {
            return m_disguiseHandle.GetOverallData();
        }
        public RoleDisguiseHandle GetDisguiseHandle()
        {
            return m_disguiseHandle;
        }
        public RoleSkinPart GetSkinPartConfig()
        {
            return m_skinMerge.GetConfig();
        }
        public RoleSkinHandle GetRoleSkinHandle()
        {
            return m_skinMerge;
        }
        public RoleShapeConfig GetShapeConfig()
        {
            return m_shapeMgr.GetConfig();
        }
        public RoleShapeHandle GetShapeHandle()
        {
            return m_shapeMgr;
        }
        #endregion


        protected override void OnInit()
        {           
            if(m_BackBtn!=null)
            {
                m_BackBtn.onClick.AddListener(OnClickBackBtn);
            }
            if(m_ApplyBtnState != null)
            {
                m_ApplyBtnState = m_ApplyBtnState.GetComponent<StateRoot>();
                m_ApplyBtnState.onClick.AddListener(OnClickApplyBtn);
            }
            if(m_ILNameFiled!=null)
            {
                m_UINameFiled = (UINameField)m_ILNameFiled.GetObject();
                if(m_UINameFiled.m_CloseBtn!=null)
                {
                    m_UINameFiled.m_CloseBtn.onClick.AddListener(() => 
                    {
                        m_ILNameFiled.gameObject.SetActive(false);
                    });
                }  
            }
            if(m_ILApplyTab!=null)
            {
                m_UIApplyTab = m_ILApplyTab.GetObject() as UIApplyTab;
            }

            RoleSkinConfig.Cache();
            if (m_rttHandler == null)
            {
                m_rttHandler = new RTTModelPartHandler("RTT_Avatar", m_rttUIObject, true, Vector3.up*100, () =>
                {
                    GameObject.DontDestroyOnLoad(m_rttHandler.GetRTT().gameObject);
                });
            }
        }


        protected override void OnShow(object arg)
        {
            if (arg is xys.CreateCharVo)
            {
                xys.CreateCharVo vo = arg as xys.CreateCharVo;
                m_disguiseHandle = new RoleDisguiseHandle();
                m_disguiseHandle.SetRoleAppearance(vo.roleId, vo.jobId, vo.sex, vo.appearance);
                m_ApplyBtnState.SetCurrentState(0, false);
                UIBaseFaceTab.m_ildlePresetCount = 0;//不留预设空位
            }
            else 
            {
                HotAppearanceModule module = hotApp.my.GetModule<HotAppearanceModule>();
                AppearanceMgr appearanceMgr = module.GetMgr();
                m_disguiseHandle = appearanceMgr.GetDisguiHandle();
                m_ApplyBtnState.SetCurrentState(1, false);
                UIBaseFaceTab.m_ildlePresetCount = 1;//有预设空位
                tableParent.isExclusive = false;
            }
            
            m_skinMerge = m_disguiseHandle.m_skinHandle;
            m_shapeMgr = m_disguiseHandle.m_shapeHandle;

            m_rttHandler.LoadModelWithAppearence(m_disguiseHandle);
            ShowPageByName("UIWholePage");
        }

        protected override bool OnPreChange(HotTablePage page)
        {

            if(m_disguiseHandle.m_sex==1&&(page.pageType=="UIMakeupPage"||page.pageType=="UIModifyPage"))
            {
                SystemHintMgr.ShowHint("此功能暂未开放！");
                return false;
            }

            ShowPageByName(page.pageType);
            return true;
        }
        void ShowPageByName(string pageType)
        {
            object args = null;
            args = GetPanelInfo();
            tableParent.ShowType(pageType, args);
        }

        void OnClickBackBtn()
        {
            

            //判断当前是否在登录状态
            if(App.my.appStateMgr.curState == AppStateType.CreateCharacter)
            {
                if (parent != null)
                {
                    parent.Hide(false);
                }
                App.my.eventSet.FireEvent<object>(EventID.Login_Disguise2Login, new object[] { m_disguiseHandle.m_career, m_disguiseHandle.m_sex });
            }
            //游戏中进入外观_易容界面
            if(m_ApplyBtnState.CurrentState==1)
            {
                if (!IsExist())
                {
                    //显示外观界面；
                    xys.UI.Dialog.TwoBtn m_TwoBtn = new xys.UI.Dialog.TwoBtn();
                    m_TwoBtn = xys.UI.Dialog.TwoBtn.Show("", "你还未保存新设置的形象，是否保存？",
                    "取消", () =>
                    {
                        if (parent != null)
                        {
                            parent.Hide(false);
                        }
                        App.my.uiSystem.ShowPanel(PanelType.UIAppearancePanel, null);
                        return false;
                    },
                    "确定", () =>
                    {
                        m_ILApplyTab.gameObject.SetActive(true);
                        m_UIApplyTab.Set(OnClickSave, OnClickApply);
                        return false;
                    }, true, true, () => { m_TwoBtn = null; });
                }
                else
                {
                    if (parent != null)
                    {
                        parent.Hide(false);
                    }
                    App.my.uiSystem.ShowPanel(PanelType.UIAppearancePanel, null);
                }
                
            }
        }
        void OnClickApplyBtn()
        {
            if(m_ApplyBtnState.CurrentState==0)
            {          
                m_UINameFiled.m_disguiseHandle = this.m_disguiseHandle;
                if (m_UINameFiled.m_disguiseHandle.m_sex == 0)
                {
                    m_UINameFiled.m_IsMale = false;
                }
                else
                {
                    m_UINameFiled.m_IsMale = true;
                }
                m_ILNameFiled.gameObject.SetActive(true);
            }
            else
            {
                if(IsExist())
                {
                    //直接应用
                    kvCommon itemNeed = kvCommon.Get("FaceChange");
                    int id = 0;
                    int num = 0;
                    ClothItem.StrToTwoInt(itemNeed.value, out id, out num);
                    int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(id);
                    Item tempItem = Item.Get(id);
                    string tempDes = "确定消耗" + tempItem.name + "保存新形象？";
                    UICommon.ShowTipsWithItem(tempDes, id, num, (IsSave) => 
                    {
                        if(IsSave)
                        {
                            ApplyNewDisguise();
                        }
                        else
                        {
                            Debug.Log("不保存");
                        }
                    });
                }
                else
                {
                    m_ILApplyTab.gameObject.SetActive(true);
                    m_UIApplyTab.Set(OnClickSave, OnClickApply);
                }
                
            }
        }
        void OnClickSave()
        {
            SaveNewPreset();
        }
        void OnClickApply()
        {
            if(m_UIApplyTab.m_saveAsPreset.CurrentState==0)
            {
                ApplyNewDisguise();
            }
            else
            {
                SaveNewPreset();
                ApplyNewDisguise();
            }
        }
        void SaveNewPreset()
        {
            HotAppearanceModule appearanceModule = hotApp.my.GetModule<HotAppearanceModule>();
            if (m_disguiseHandle.GetOverallData().faceType==0)
            {
                kvCommon limitNumItem = kvCommon.Get("FaceTypeHoldNumber0");
                int limitNum = 0;
                int.TryParse(limitNumItem.value, out limitNum);
                if(appearanceModule.GetAppearanceData().presettings_0.Count>= limitNum)
                {
                    SystemHintMgr.ShowTipsHint(7608);
                    m_ILApplyTab.gameObject.SetActive(false);
                    return;
                }
            }
            else
            {
                kvCommon limitNumItem = kvCommon.Get("FaceTypeHoldNumber1");
                int limitNum = 0;
                int.TryParse(limitNumItem.value, out limitNum);
                if (appearanceModule.GetAppearanceData().presettings_1.Count >= limitNum)
                {
                    SystemHintMgr.ShowTipsHint(7608);
                    m_ILApplyTab.gameObject.SetActive(false);
                    return;
                }
            }
            
            
            NetProto.PresettingSaveReq request = new NetProto.PresettingSaveReq();
            request.faceType = m_disguiseHandle.GetOverallData().faceType;
            request.persetting.persetting = m_disguiseHandle.GetOverallData().ToJson();

            appearanceModule.Event.FireEvent<NetProto.PresettingSaveReq>(EventID.Ap_SaveNewFace, request);
            m_NeedComfirm = false;
            SystemHintMgr.ShowTipsHint(7606);
            m_ILApplyTab.gameObject.SetActive(false);
            return;
        }
        void ApplyNewDisguise()
        {
            kvCommon item = kvCommon.Get("FaceChange");
            int id = 0;
            int num = 0;
            ClothItem.StrToTwoInt(item.value, out id, out num);
            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(id);

            if(curOwn<num)
            {
                ItemTipsPanel.Param param = new ItemTipsPanel.Param();
                param.itemId = id;
                App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
            }
            else
            {               
                App.my.localPlayer.GetModule<PackageModule>().UseItemById(id, num);

                NetProto.DisguiseSetReq request = new NetProto.DisguiseSetReq();
                request.faceType = m_disguiseHandle.GetOverallData().faceType;
                request.hairStyleId = m_disguiseHandle.GetOverallData().hairStyle;
                request.hairColorId = m_disguiseHandle.GetOverallData().hairColorIdx;
                request.skinColorId = m_disguiseHandle.GetOverallData().skinColorIdx;
                request.faceJson = m_disguiseHandle.GetOverallData().ToFaceJson();

                HotAppearanceModule module = hotApp.my.GetModule<HotAppearanceModule>();
                module.Event.FireEvent<NetProto.DisguiseSetReq>(EventID.Ap_ApplyNewFace, request);

                SystemHintMgr.ShowTipsHint(7607);
                m_ILApplyTab.gameObject.SetActive(false);
                return;
            }
        }
        protected override void OnHide()
        {
            if(m_ApplyBtnState.CurrentState == 1)
            {
                hotApp.my.GetModule<HotAppearanceModule>().GetMgr().RefreshDisguisehandle();//刷新外观模型handle
            }
            m_NeedComfirm = false;
            if (m_rttHandler != null)
                m_rttHandler.DestoryModel();
        }

        protected override void OnDestroy()
        {
            if(m_rttHandler!=null)
            {
                m_rttHandler.Destroy();
                m_rttHandler = null;
            }
        }

        //当前预设已存在
        bool IsExist()
        {
            RoleDisguiseOverallData curData = m_disguiseHandle.GetOverallData();
            string curString = curData.ToJson();

            int count = m_disguiseHandle.GetOverallDataCount();
            for(int i=0;i<count;i++)
            {
                string temp = m_disguiseHandle.GetOverallDataConfigByIndex(i);

                if (curString.Equals(temp))
                {
                    return true;
                }
            }
            HotAppearanceModule appearanceModule = hotApp.my.GetModule<HotAppearanceModule>();
            NetProto.AppearanceData data = appearanceModule.GetAppearanceData();
            if(curData.faceType==0)
            {
                for(int i=0;i<data.presettings_0.Count;i++)
                {
                    
                    if (curString.Equals(data.presettings_0[i].persetting))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < data.presettings_1.Count; i++)
                {
                    if (curString.Equals(data.presettings_1[i].persetting))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        PanelInfo GetPanelInfo()
        {
            PanelInfo info = new PanelInfo();
            info.m_Panel = this;
            return info;
        }
    }

    class PanelInfo
    {
        public UIFaceMakePanel m_Panel=null;
    }

}
#endif
#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;
using xys;
using xys.UI;
using NetProto.Hot;

namespace xys.hot.UI
{
    [System.Serializable]
    class UIPetsInfomation
    {
        [SerializeField]
        Transform m_Transform;
        public Transform transform { get { return m_Transform; } }

        [SerializeField]
        protected Text m_PetName;
        [SerializeField]
        protected Text m_PetLevel;
        [SerializeField]
        protected StateRoot m_PetQuality;
        [SerializeField]
        protected Text m_Score;
        [SerializeField]
        protected GameObject m_VariationIcon;
        [Header("Exp")]
        [SerializeField]
        protected Image m_ExpBar;
        [SerializeField]
        protected Text m_ExpText;
        [Header("Button")]
        [SerializeField]
        protected Text m_PlayButtonText;
        [SerializeField]
        protected Button m_TrainBtn;
        [SerializeField]
        protected Button m_AssianBtn;

        [Header("RenderTexture")]
        [SerializeField]
        protected RawImage m_TrumpRawIcon;
        [Header("RootPanel")]
        [SerializeField]
        string m_FightTxt = "出 战";
        [SerializeField]
        string m_RestTxt = "下 战";
        [SerializeField]
        Button m_ReleasePetBtn;
        [SerializeField]
        Button m_RecyclingPetBtn;
        [SerializeField]
        Button m_FightPetBtn;

        RTTModelPartHandler m_Rtt;

        xys.UI.Dialog.TwoBtn m_Screen;

        PetsPanel m_Panel;
        public PetsPanel panel { set { m_Panel = value; } }
        int m_AttributeIndex = -1;

        public void OnInit()
        {
            this.OnCreateModel();

            m_ReleasePetBtn.onClick.AddListener(this.OnReleasePet);
            m_RecyclingPetBtn.onClick.AddListener(this.OnRecyclingPet);
            m_FightPetBtn.onClick.AddListener(this.OnPetPlay);
            m_TrainBtn.onClick.AddListener(() => { m_Panel.OnTrainPage(m_Panel); });
            m_AssianBtn.onClick.AddListener(() => { xys.App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIPetsAssginPanel, this.m_Panel, true); });
        }

        public void OnDestroy()
        {
            this.OnDelectModel();

            m_ReleasePetBtn.onClick.RemoveAllListeners();
            m_RecyclingPetBtn.onClick.RemoveAllListeners();
            m_FightPetBtn.onClick.RemoveAllListeners();
            m_TrainBtn.onClick.RemoveAllListeners();
            m_AssianBtn.onClick.RemoveAllListeners();
        }


        public void OnShow()
        {
            this.m_Transform.gameObject.SetActive(true);
            if (m_Rtt == null) this.OnCreateModel();
            this.ActiveModel();
        }

        public void OnHide()
        {
            this.m_Transform.gameObject.SetActive(false);
            this.UnActiveModel();
        }

        void PetsNickNameEvent(int attributeIndex, string oldVale, string newVale)
        {
            if (m_AttributeIndex == attributeIndex)
            {
                m_PetName.text = newVale;
            }
        }

        public void ResetData(int attributeIndex)
        {
            PetsModule petsModule = App.my.localPlayer.GetModule<PetsModule>();
            PetsMgr petsMgr = m_Panel.petsMgr;
            if (!petsMgr.CheckIndex(attributeIndex))
                return;
            NetProto.PetsAttribute attribute = petsMgr.m_PetsTable.attribute[attributeIndex];
            if (attribute != null && Config.PetAttribute.Get(attribute.id) != null)
            {
                if (m_Panel.selectedPetObj == null)
                    return;

                Config.PetAttribute property = Config.PetAttribute.Get(attribute.id);
                m_PetName.text = attribute.nick_name;
                m_PetLevel.text = attribute.lv.ToString();
                m_VariationIcon.SetActive(property.variation == 1);
                m_PetQuality.gameObject.SetActive(true);
                m_PetQuality.CurrentState = property.type;

                SetPetScore(100);

                Config.RoleExp upgradeExp = Config.RoleExp.Get(attribute.lv);
                if (upgradeExp == null)
                    m_ExpBar.fillAmount = 0.0f;
                else
                    m_ExpBar.fillAmount = Mathf.Clamp01(1.0f * attribute.exp / upgradeExp.pet_exp);

                m_ExpText.text = attribute.exp + "/" + upgradeExp.pet_exp;

                int fightIndex = petsMgr.GetFightPetID();
                if (fightIndex != -1)
                    m_PlayButtonText.text = attributeIndex == fightIndex ? m_RestTxt : m_FightTxt;
                else
                    m_PlayButtonText.text = m_FightTxt;
                #region 模型
                if (Config.RoleDefine.GetAll().ContainsKey(property.identity))
                {
                    if (m_Rtt.modelId != property.identity)
                    {
                        this.ActiveModel();
                        m_Rtt.SetModel(property.identity, (go) => { m_Rtt.SetCameraState(property.camView, new Vector3(property.camPos[0], property.camPos[1], property.camPos[2])); });
                    }
                }
                else
                {
                    this.UnActiveModel();
                }
                #endregion
            }
            else
            {
                m_PetName.text = "";
                m_PetLevel.text = "";
                m_PetQuality.gameObject.SetActive(false);
                m_ExpBar.fillAmount = 0.0f;

                this.UnActiveModel();
            }

            m_AttributeIndex = attributeIndex;
        }
        //放生宠物
        public void OnReleasePet()
        {
            if (m_Screen != null)
                return;
            PetsMgr petMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petMgr == null || !petMgr.CheckIndex(m_AttributeIndex))
                return;
            if (petMgr.m_PetsTable.PlayPetID == m_AttributeIndex)
            {
                SystemHintMgr.ShowHint("出战的灵兽无法放生");
                return;
            }
            NetProto.PetsAttribute attribute = petMgr.m_PetsTable.attribute[m_AttributeIndex];
            if (Config.PetAttribute.Get(attribute.id) == null)
                return;

            Config.PetAttribute data = Config.PetAttribute.Get(attribute.id);
            if (data == null)
                return;
            int quality = data.type;
            m_Screen = xys.UI.Dialog.TwoBtn.Show(
                "",
                string.Format("灵兽放生后将无法还原，确定要放生这只#c{1}{0}#n？", data.name, "#c" + Config.QualitySourceConfig.Get((Config.ItemQuality)data.type).color),
                "取消", () => false,
                "确定", () =>
                {
                    App.my.eventSet.FireEvent<int>(EventID.Pets_Delect, m_AttributeIndex);
                    return false;
                }, true, true, () => m_Screen = null);

        }

        void OnRecyclingPet()
        {
            if (m_Screen != null)
                return;
            PetsMgr petsMgr = m_Panel.petsMgr;

            if (petsMgr.m_PetsTable.PlayPetID == m_AttributeIndex)
            {
                //UIHintManage.Instance.ShowHint("出战的灵兽无法回收");
                SystemHintMgr.ShowHint("出战的灵兽无法放生");
                return;
            }

            NetProto.PetsAttribute attribute = petsMgr.m_PetsTable.attribute[m_AttributeIndex];
            if (Config.PetAttribute.Get(attribute.id) == null)
                return;


            Config.PetAttribute data = Config.PetAttribute.Get(attribute.id);
            m_Screen = xys.UI.Dialog.TwoBtn.Show(
              "",
              string.Format("确定将#c{1}{0}#n收回至包裹？", data.name, "#c" + Config.QualitySourceConfig.Get((Config.ItemQuality)data.type).color),
              "取消", () =>
              {
                  return false;
              },
              "确定", () =>
              {
                  //Utils.EventDispatcher.Instance.TriggerEvent<int>(PetsSystem.Event.SetItemToPet, m_PetUuid);

                  App.my.eventSet.FireEvent<int>(EventID.Pets_2Items, m_AttributeIndex);
                  return false;
              }, true, true, () => { m_Screen = null; });
        }

        public void OnShowEnhancedPage()
        {
            //m_Panel.ShowTips();
        }

        //出战/下战宠物
        void OnPetPlay()
        {
            PetsMgr petsMgr = m_Panel.petsMgr;
            if (petsMgr.m_PetsTable.PlayPetID == m_AttributeIndex)
            {
                m_Panel.selectedPetPlay = -1;
            }
            else
            {
                NetProto.PetsAttribute attribute = petsMgr.m_PetsTable.attribute[m_AttributeIndex];
                if (attribute != null)
                {
                    bool death = App.my.srvTimer.GetTime.GetCurrentTime() - attribute.last_die_time < float.Parse(Config.kvCommon.Get("petReliveCD").value);

                    //看看宠物的是否处于死亡待复活的状态，如果是，就不能修改这个值
                    if (death)
                    {
                        //UIHintManage.Instance.ShowHint("已死亡的灵兽，在复活之前无法出战");
                        SystemHintMgr.ShowHint("出战的灵兽无法放生");
                        return;
                    }
                }

                m_Panel.selectedPetPlay = m_AttributeIndex;
            }
        }

        string GetQualityBg(int quality)
        {
            string bg = "ui_Common_Quality_White";
            switch (quality)
            {
                case 1:
                    bg = "ui_Common_Quality_White";
                    break;
                case 2:
                    bg = "ui_Common_Quality_Green";
                    break;
                case 3:
                    bg = "ui_Common_Quality_Blue";
                    break;
                case 4:
                    bg = "ui_Common_Quality_Purple";
                    break;
                case 5:
                    bg = "ui_Common_Quality_Orange";
                    break;
                case 6:
                    bg = "ui_Common_Quality_Red";
                    break;
            }
            return bg;
        }
        void SetPetScore(int score)
        {
            m_Score.text = score.ToString();
        }
        public void UnComplete()
        {
            //UIHintManage.Instance.ShowHint("功能未完成");
            SystemHintMgr.ShowHint("功能未完成");
        }
        void HandlePetAttribute(int key, int oldVal, int newVal)
        {
            this.ResetData(m_AttributeIndex);
        }
        void OnCreateModel()
        {
            m_Rtt = new RTTModelPartHandler("RTTModelPart", m_TrumpRawIcon.GetComponent<RectTransform>(), "", true, new Vector3(1000, 1000, 0),
               () =>
               {
                   m_Rtt.SetModelRotate(new Vector3(0.0f, 150.0f, 0.0f));
               });
        }
        void OnDelectModel()
        {
            if (m_Rtt != null) m_Rtt.Destroy();
            m_Rtt = null;
            m_AttributeIndex = -1;
        }
        void ActiveModel()
        {
            m_Rtt.SetRenderActive(true);
        }
        void UnActiveModel()
        {
            m_Rtt.SetRenderActive(false);
            m_AttributeIndex = -1;
        }
    }

}
#endif
#if !USE_HOT
using Config;
using NetProto;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIRoleAttriRoleInfo
    {
        [SerializeField]
        Image job;
        [SerializeField]
        StateRoot fiveElement;
        [SerializeField]
        Text roleTitle;
        [SerializeField]
        Text name;
        [SerializeField]
        Text level;
        [SerializeField]
        Transform ModelImage;

        [SerializeField]
        Button roleTitleBtn;

        RTTModelPartHandler m_rtt;      //角色显示
        ObjectEventAgent m_events;

        void Awake()
        {
            m_rtt = new RTTModelPartHandler("RTTModelPart", ModelImage.GetComponent<RectTransform>(), "", true, new Vector3(500, 0, 0));
        }

        void OnEnable()
        {
            m_events = new ObjectEventAgent(App.my.localPlayer.eventSet);
            m_events.Subscribe(AttType.AT_Level, (args) => { level.text = App.my.localPlayer.levelValue + "级"; });

            App.my.eventSet.Subscribe(EventID.Title_Change, ShowRoleTitle);

            ShowRoleInfo();
        }

        private void OnDisable()
        {
            m_events.Release();
            m_events = null;
        }

        public void ShowRoleInfo()
        {
            name.text = App.my.localPlayer.name;
            level.text = App.my.localPlayer.levelValue + "级";

            ShowJob();
            ShowFiveElement();
            ShowModel();
            ShowRoleTitle();
        }

        void ShowJob()
        {
            Helper.SetSprite(job, RoleJob.Get(App.my.localPlayer.carrerValue).attrPanelIcon);
        }

        void ShowFiveElement()
        {
            fiveElement.CurrentState = RoleJob.Get(App.my.localPlayer.carrerValue).attributeType - 1;
        }

        void ShowModel()
        {
            RoleJob jobCfg = RoleJob.Get(App.my.localPlayer.carrerValue);
            if (null == jobCfg) return;

            ModelImage.gameObject.SetActive(true);
            m_rtt.SetModel(App.my.localPlayer.sexValue == 1 ? jobCfg.maleId : jobCfg.felmaleId);
        }

        void ShowRoleTitle()
        {
            TitleList titleData = App.my.localPlayer.GetModule<TitleModule>().m_TitleListData as TitleList;
            RoleTitle titleConfig = RoleTitle.Get(titleData.currTitle);
            if (titleConfig == null)
                roleTitle.text = "无称号";
            else
            {
                QualitySourceConfig qualityConfig = QualitySourceConfig.Get(titleConfig.quality);
                if (titleConfig.specialEqulity != "")
                    roleTitle.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", titleConfig.specialEqulity, titleConfig.name));
                else if (qualityConfig != null)
                    roleTitle.text = string.Format("<color=#{0}>{1}</color>", qualityConfig.color, titleConfig.name);
            }

            roleTitleBtn.transform.Find("RedTips").gameObject.SetActive(false);
        }
    }
}
#endif
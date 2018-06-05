#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using xys.UI;
using UnityEngine;
using UnityEngine.UI;
using Config;
using NetProto.Hot;

namespace xys.hot.UI
{
    class UIActivityIntroductionPanel : HotPanelBase
    {
        public class UIActivityIconItem
        {
            #region 下方奖励对象
            public Transform m_ItemTransform;

            private Item itemConfigData;

            public void SetData(int itemId, int num = 1)
            {
                itemConfigData = Item.Get(itemId);

                QualitySourceConfig qualitConfig = QualitySourceConfig.Get(itemConfigData.quality);
                if (qualitConfig == null)
                    return;

                Helper.SetSprite(m_ItemTransform.Find("Icon").GetComponent<Image>(), itemConfigData.icon);
                Helper.SetSprite(m_ItemTransform.Find("Quality").GetComponent<Image>(), qualitConfig.icon);

                Button selfButton = m_ItemTransform.GetComponentInChildren<Button>();
                if (selfButton != null)
                {
                    selfButton.onClick.RemoveAllListeners();
                    selfButton.onClick.AddListener(this.OnClickShowDesc);
                }
            }

            private void OnClickShowDesc()
            {
                UICommon.ShowItemTips(itemConfigData.id);
            }
            #endregion
        }

        [SerializeField]
        private Transform m_Transform;
        [SerializeField]
        private Transform m_BookTrans;
        [SerializeField]
        private Transform m_LimitTrans;
        [SerializeField]
        private Transform m_Grid;

        [SerializeField]
        private GameObject m_ItemObj;

        [SerializeField]
        private Text m_NameText;
        [SerializeField]
        private Text m_NumberText;
        [SerializeField]
        private Text m_RewardText;
        [SerializeField]
        private Text m_RewardTitle;
        [SerializeField]
        private Text m_DescText;
        [SerializeField]
        private Text m_ShowText1;
        [SerializeField]
        private Text m_ShowText2;
        [SerializeField]
        private Text m_ShowText3;

        [SerializeField]
        private Image m_IconImage;

        public UIActivityIntroductionPanel() : base(null) { }
        public UIActivityIntroductionPanel(UIHotPanel parent) : base(parent)
        {
        }

        protected override void OnInit()
        {

        }

        protected override void OnShow(object args)
        {
            if (args == null)
                return;

            ActivityData activityData = (ActivityData)args;
            ActivityDefine acticityConf = ActivityDefine.Get(activityData.activityId);

            Helper.SetSprite(m_IconImage, acticityConf.icon);

            m_NameText.text = acticityConf.name;

            if (acticityConf.maxNum == 0)
                m_NumberText.text = "次数: 无限制";
            else
                m_NumberText.text = activityData.activityNum + "/" + acticityConf.maxNum;

            if (acticityConf.activeness == 0)
                m_RewardText.text = "";
            else
            {
                float maxActive = (activityData.activityNum * acticityConf.activeness > acticityConf.maxActiveness ? acticityConf.maxActiveness : activityData.activityNum * acticityConf.activeness);
                m_RewardText.text = maxActive + "/" + acticityConf.maxActiveness;
            }
            m_RewardTitle.gameObject.SetActive(acticityConf.activeness != 0);

            m_DescText.text = acticityConf.desc;

            m_ShowText1.text = acticityConf.openDateDesc + " " + ActivityMgr.GetTimeStr(activityData, acticityConf);
            m_ShowText2.text = acticityConf.requireLv.ToString();
            m_ShowText3.text = (acticityConf.pepoleLimit == 1 ? "单人" : "组队");

            DestroyAllItems(m_Grid);

            for (int i = 0; i < acticityConf.prize.Length; i++) // 奖励
            {
                GameObject go = GameObject.Instantiate(m_ItemObj);
                go.SetActive(true);
                go.transform.SetParent(m_Grid);
                go.transform.localScale = Vector3.one;

                UIActivityIconItem activityItem = new UIActivityIconItem();
                activityItem.m_ItemTransform = go.transform;
                activityItem.SetData(acticityConf.prize[i]);
            }
        }

        private void DestroyAllItems(Transform parent)
        {
            if (parent.childCount == 0)
                return;

            var children = new GameObject[parent.childCount];
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                children[i] = child.gameObject;
            }

            for (int i = 0; i < children.Length; i++)
            {
                if (Application.isPlaying)
                    GameObject.Destroy(children[i]);
                else
                    GameObject.DestroyImmediate(children[i]);
            }
        }
    }
}
#endif
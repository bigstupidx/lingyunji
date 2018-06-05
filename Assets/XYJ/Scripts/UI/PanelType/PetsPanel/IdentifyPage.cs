#if !USE_HOT
using Config;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class IdentifyPage : HotTablePageBase
    {
        #region 属性
        [SerializeField]
        Transform m_PetInformationRoot;
        [SerializeField]
        Transform m_BasePropertyRoot;
        [SerializeField]
        Transform m_FirstPropertyRoot;
        [SerializeField]
        Transform m_SkillRoot;
        [SerializeField]
        Button m_VarationBtn;
        [SerializeField]
        Button m_CatchBtn;
        [SerializeField]
        Button m_DemonPlotBtn;
        [SerializeField]
        GameObject m_BG2;
        [SerializeField]
        GameObject m_VariationIcon;
        [SerializeField]
        StateRoot m_Grade;

        [SerializeField]
        protected UnityEngine.UI.RawImage m_TrumpRawIcon;
        RTTModelPartHandler m_Rtt;
        //string m_PetModel;
        //是否查看变异标示
        bool isShowVariation = false;
        //UI脚本相关
        [SerializeField]
        ILMonoBehaviour m_ILSort;
        UIPetsHandBookSort m_Sort;
        [SerializeField]
        ILMonoBehaviour m_ILSkillTips;
        UIPetsSKillTips m_SkillTips;
        [SerializeField]
        ILMonoBehaviour m_ILScrollView;
        UIPetsHandBookScrollView m_ScrollView;

        //UIPetsInfomation m_Infos;

        List<UIPetsHandBookSkillItem> m_UISkillList = new List<UIPetsHandBookSkillItem>();

        MethodInfo OnShowSkillTips = null;
        MethodInfo OnScrollViewEventSet = null;
        MethodInfo OnSortInit = null;

        HotTablePage m_Page;
        #endregion
        IdentifyPage() : base(null) { }
        IdentifyPage(HotTablePage page) : base(page)
        {
            m_Page = page;
        }

        protected override void OnInit()
        {
            m_VarationBtn.onClick.AddListener(() => { this.OnVariationEvent(); });
            m_CatchBtn.onClick.AddListener(() => { this.OnCatchEvent(); });
            m_DemonPlotBtn.onClick.AddListener(() => { this.OnDemonPotEvent(); });

            for (int i = 0; i < m_SkillRoot.childCount; i++)
            {
                int index = i;
                m_SkillRoot.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.ShowSkillTips(index);
                });

                ILMonoBehaviour il = m_SkillRoot.GetChild(i).GetComponent<ILMonoBehaviour>();
                UIPetsHandBookSkillItem skillItem = (UIPetsHandBookSkillItem)il.GetObject();
                m_UISkillList.Add(skillItem);
            }
        }

        #region 初始化
        void InitScrollView()
        {
            if (m_ILScrollView != null)
            {
                object obj = m_ILScrollView.GetObject();
                m_ScrollView = (UIPetsHandBookScrollView)obj;
                if (m_ScrollView == null)
                    return;
                m_ScrollView.SetEvent(Event);
                m_ScrollView.selectedCallback = this.ResetPage;
            }
        }
        void InitSort()
        {
            if (m_ILSort != null)
            {
//                 object obj = m_ILSort.GetObject();
//                 m_Sort = (UIPetsHandBookSort)obj;
//                 if (m_Sort == null)
//                     return;
//                 m_Sort.SetEvent(Event);
            }
        }
        void InitSkillTips()
        {
            if (m_ILSkillTips != null)
            {
                object obj = m_ILSkillTips.GetObject();
                m_SkillTips = (UIPetsSKillTips)obj;
            }
        }
        
        #endregion
        //
        protected override void OnShow(object args)
        {
            //初始化IL脚本
            this.InitScrollView();
            this.InitSort();
            this.InitSkillTips();

            this.OnCreateModel();
            this.parent.parent.Event.Subscribe(EventID.Pets_RefreshUI, this.ResetPage);

            this.parent.parent.StartCoroutine(m_ScrollView.Show());
        }
        //
        protected override void OnHide()
        {
            m_ScrollView.Hide();
            this.OnDelectModel();
        }

        #region 内部
        void ResetPage()
        {
            if (m_ScrollView.selected == 0)
                return;
            isShowVariation = false;
            m_VarationBtn.GetComponentInChildren<Text>().text = "查看变异";

            PetAttribute property = PetAttribute.Get(m_ScrollView.selected);
            if (property == null)
                return;
            //加载模型
            if (Config.RoleDefine.GetAll().ContainsKey(property.identity))
            {
                m_Rtt.SetModel(property.identity, (go) => { m_Rtt.SetCameraState(property.camView, new Vector3(property.camPos[0], property.camPos[1], property.camPos[2])); });
            }
            else
            {
                this.UnActiveModel();
            }

            this.parent.transform.Find("bg/Common/Level").GetComponent<Text>().text = "参战等级 " + property.carry_level + "级";
            m_PetInformationRoot.Find("Name").GetComponent<Text>().text = property.name;


            m_VariationIcon.SetActive(property.variation == 1);
            m_Grade.CurrentState = property.type;

            m_PetInformationRoot.Find("Des").GetComponent<Text>().text = property.des;
            //
            int petId = property.identity;
            int attackId = 0;// RoleDefine.Get(property.identity).defaultSkills[0];
            //m_BasePropertyRoot.Find("Attack").GetComponent<Text>().text = SkillPrototypeManage.instance.Getproperty(attackId).range.ToString();
            m_BasePropertyRoot.Find("AttackCD").GetComponent<Text>().text = PetparametersDefine.Get(m_ScrollView.selected).attackInterval.ToString();

            m_BasePropertyRoot.Find("Icon4").GetComponent<StateRoot>().CurrentState = property.fiveElements - 1;
            //
            int index = 0;
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
                "成长值", property.grow_rate_min, property.grow_rate_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
                AttributeDefine.Get(AttributeDefine.iStrength).name + "资质", property.grow_pow_min, property.grow_pow_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
               AttributeDefine.Get(AttributeDefine.iIntelligence).name + "资质", property.grow_intelligence_min, property.grow_intelligence_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
                AttributeDefine.Get(AttributeDefine.iBone).name + "资质", property.grow_bone_min, property.grow_bone_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
                AttributeDefine.Get(AttributeDefine.iPhysique).name + "资质", property.grow_bodies_min, property.grow_bodies_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
                AttributeDefine.Get(AttributeDefine.iAgility).name + "资质", property.grow_agile_min, property.grow_agile_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
                AttributeDefine.Get(AttributeDefine.iBodyway).name + "资质", property.grow_bodyposition_min, property.grow_bodyposition_max, ref index);
            #region 技能
            index = 0;
            m_UISkillList[index].SetData(property.defaultSkillId, UIPetsSkillItem_Type.PassivasSlot, ref index, true);
            m_UISkillList[index].SetData(property.trickSkillsID, UIPetsSkillItem_Type.TrickSlot, ref index);
            //
            m_UISkillList[index].SetData(property.trickSkillsID, UIPetsSkillItem_Type.TalentSlot, ref index);
            //
            for(int i = 0;i<property.skills.Length;i++)
            {
                m_UISkillList[index].SetData(property.skills[i], UIPetsSkillItem_Type.PassivasSlot, ref index);
            }
            for (; index < m_SkillRoot.childCount; index++)
                m_SkillRoot.GetChild(index).gameObject.SetActive(false);
            #endregion
            //显示变异按钮
            m_VarationBtn.gameObject.SetActive(property.variation_id == 0 ? false : true);
            m_BG2.gameObject.SetActive(property.variation_id == 0 ? true : false);
        }

        void ShowVariationUI()
        {
            if (m_ScrollView.selected == 0)
                return;

            PetAttribute property = PetAttribute.Get(m_ScrollView.selected);
            if (property == null)
                return;
            if (property.variation_id == 0)
                return;

            int varationID = property.variation_id;
            property = PetAttribute.Get(varationID);
            if (property == null)
            {
                Debuger.LogError("找不到变异宠物ID：" + varationID);
                return;
            }

            isShowVariation = true;
            m_VarationBtn.GetComponentInChildren<Text>().text = "查看普通";
            //
            this.parent.transform.Find("bg/Common/Level").GetComponent<Text>().text = "参战等级 " + property.carry_level + "级";
            m_PetInformationRoot.Find("Name").GetComponent<Text>().text = property.name;
            m_PetInformationRoot.Find("Des").GetComponent<Text>().text = property.des;
            //   
            int petId = property.identity;
            int attackId = 0;// RoleDefine.Get(property.identity).defaultSkills[0];
            m_BasePropertyRoot.Find("Attack").GetComponent<Text>().text = "0";// SkillPrototypeManage.instance.Getproperty(attackId).range.ToString();
            m_BasePropertyRoot.Find("AttackCD").GetComponent<Text>().text = PetparametersDefine.Get(m_ScrollView.selected).attackInterval.ToString();

            m_BasePropertyRoot.Find("Icon4").GetComponent<StateRoot>().CurrentState = property.fiveElements - 1;

            m_VariationIcon.SetActive(property.variation == 1);
            m_Grade.CurrentState = property.type;

            if (Config.RoleDefine.GetAll().ContainsKey(varationID))
            {
                m_Rtt.SetModel(varationID, (go) => { m_Rtt.SetCameraState(property.camView, new Vector3(property.camPos[0], property.camPos[1], property.camPos[2])); });
            }
            else
            {
                this.UnActiveModel();
            }

            //
            int index = 0;
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
                "成长值", property.grow_rate_min, property.grow_rate_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
                AttributeDefine.Get(AttributeDefine.iStrength).name + "资质", property.grow_pow_min, property.grow_pow_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
               AttributeDefine.Get(AttributeDefine.iIntelligence).name + "资质", property.grow_intelligence_min, property.grow_intelligence_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
               AttributeDefine.Get(AttributeDefine.iBone).name + "资质", property.grow_bone_min, property.grow_bone_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
               AttributeDefine.Get(AttributeDefine.iPhysique).name + "资质", property.grow_bodies_min, property.grow_bodies_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
               AttributeDefine.Get(AttributeDefine.iAgility).name + "资质", property.grow_agile_min, property.grow_agile_max, ref index);
            SetAttribute(m_FirstPropertyRoot.GetChild(index),
               AttributeDefine.Get(AttributeDefine.iBodyway).name + "资质", property.grow_bodyposition_min, property.grow_bodyposition_max, ref index);
            #region 技能
            index = 0;
            m_UISkillList[index].SetData(property.defaultSkillId, UIPetsSkillItem_Type.PassivasSlot, ref index, true);
            m_UISkillList[index].SetData(property.trickSkillsID, UIPetsSkillItem_Type.TrickSlot, ref index);
            //
            m_UISkillList[index].SetData(property.trickSkillsID, UIPetsSkillItem_Type.TalentSlot, ref index);
            //
            for (int i = 0; i < property.skills.Length; i++)
            {
                m_UISkillList[index].SetData(property.skills[i], UIPetsSkillItem_Type.PassivasSlot, ref index);
            }
            for (; index < m_SkillRoot.childCount; index++)
            {
                m_SkillRoot.GetChild(index).gameObject.SetActive(false);
            }
            #endregion
        }

        void SetAttribute(Transform nRoot, string attr, float minvalue, float maxvalue, ref int index)
        {
            nRoot.GetComponent<Text>().text = minvalue + "-" + maxvalue;
            nRoot.Find("Attack").GetComponent<Text>().text = attr;
            index++;
        }

        void OnCreateModel()
        {
            m_Rtt = new RTTModelPartHandler("RTTModelPart", m_TrumpRawIcon.GetComponent<RectTransform>(), "", true, new Vector3(1000, -1000, 0),
               () =>
               {
                   m_Rtt.SetModelRotate(new Vector3(0.0f, 150.0f, 0.0f));
               });
        }
        void OnDelectModel()
        {
            if (m_Rtt != null) m_Rtt.Destroy();
            m_Rtt = null;
        }
        void  ActiveModel()
        {
            m_Rtt.SetRenderActive(true);
        }
       void UnActiveModel()
        {
            m_Rtt.SetRenderActive(false);
        }
        #endregion

        #region 事件
        public void OnVariationEvent()
        {
            if (!isShowVariation)
            {
                ShowVariationUI();
            }
            else
            {
                ResetPage();
            }
        }
        public void OnBuyEvent()
        {
            //UIHintManage.Instance.ShowHint("功能未开放，稍后补充");
        }

        public void OnDemonPotEvent()
        {
            //UIHintManage.Instance.ShowHint("功能未开放，稍后补充");
        }
        public void OnCatchEvent()
        {
            //UIHintManage.Instance.ShowHint("功能未开放，稍后补充");
        }

        protected void ShowSkillTips(int index)
        {
            Vector2 pos = new Vector2(-173.0f, 0);
            ILMonoBehaviour il = m_SkillRoot.GetChild(index).GetComponent<ILMonoBehaviour>();
            UIPetsHandBookSkillItem item = il.GetObject() as UIPetsHandBookSkillItem;
            m_SkillTips.Set(item.id);//, 0, pos);
        }
        #endregion
    }

}
#endif
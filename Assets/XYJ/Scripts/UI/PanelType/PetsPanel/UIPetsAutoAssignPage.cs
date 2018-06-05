#if !USE_HOT
using NetProto;
using NetProto.Hot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.Dialog;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIPetsAutoAssignPage : HotTablePageBase
    {
        string sAddBtn = "AddBtn";
        string sReduction = "ReductionBtn";
        string sSlider = "Slider";
        int m_TotalAssignNum = 5;
        [SerializeField]
        Transform m_Grid;
        [SerializeField]
        Button m_BgBtn;
        int m_SelectedIndex = -1;
        int[] m_TempAssignNum;

        PetsPanel m_Panel;

        UIPetsAutoAssignPage() : base(null) { }
        UIPetsAutoAssignPage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            for (int i = 0; i < m_Grid.childCount; i++)
            {
                int index = i;
                Transform item = m_Grid.GetChild(i);
                item.Find(sAddBtn).GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.AddPotentialPoint(index);
                });
                item.Find(sReduction).GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.ReducePotentialPoint(index);
                });
            }
            m_BgBtn.onClick.AddListener(() => {
                this.parent.parent.ShowType(this.parent.parent.GetPageList()[(int)UIPetsAssginPanel.page.AssignPage].Get().pageType, this.m_Panel);
            });
        }
        void SetRightProperty(Transform nRoot, string attr, int value, ref int index)
        {
            nRoot.GetChild(index).Find("Name").GetComponent<Text>().text = attr;
            nRoot.GetChild(index).Find("num").GetComponent<Text>().text = value.ToString();
            if (m_TempAssignNum[index] > 0)
            {
                nRoot.GetChild(index).Find("num").GetComponent<Text>().color = ColorUT.ToColor("61e171");// ColorUtls.Instance.ToColor("61e171");
            }
            else
            {
                nRoot.GetChild(index).Find("num").GetComponent<Text>().color = Color.white;
            }
            index++;
        }
        protected override void OnHide()
        {
            if (!this.SameSliderValue())
            {
                SetPetPotentialSliderRequest request = new SetPetPotentialSliderRequest();

                request.index = this.m_SelectedIndex;
                request.power = m_TempAssignNum[0];
                request.intelligence = m_TempAssignNum[1];
                request.root_bone = m_TempAssignNum[2];
                request.bodies = m_TempAssignNum[3];
                request.agile = m_TempAssignNum[4];
                request.body_position = m_TempAssignNum[5];

                App.my.eventSet.FireEvent(EventID.Pets_Slider, request);
            }
        }
        protected override void OnShow(object args)
        {
            m_Panel = args as PetsPanel;
            if(m_Panel == null)
            {
                Debuger.LogError("Panel null");
                this.parent.gameObject.SetActive(false);
                return;
            }
            m_TempAssignNum = new int[6];
            m_SelectedIndex = m_Panel.selected;
            ResetUI();
            RefleshData();
        }
        #region event
        void OnValueChange(float value, int index)
        {
            int tempValue = (int)value;
            if (tempValue == 0)
            {
                SetSeletBtnState(sReduction, index, 0);
            }
            else if (tempValue == m_TotalAssignNum)
            {
                SetSeletBtnState(sAddBtn, index, 0);
            }
            else
            {
                SetSeletBtnState(sReduction, index, 1);
                SetSeletBtnState(sAddBtn, index, 1);
            }
            tempValue = m_TempAssignNum[index];
            m_TempAssignNum[index] = (int)value;
            if (GetTempPotentialSliderPoint() > m_TotalAssignNum)
            {
                m_TempAssignNum[index] = tempValue;
            }
            else if (GetTempPotentialSliderPoint() == m_TotalAssignNum)
            {
                //SetSliderValue(index, m_TempAssignNum[index]);
                SetAllBtnState(sAddBtn, 0);
            }
            else
            {
               // SetSliderValue(index, m_TempAssignNum[index]);
                SetAllBtnState(sAddBtn, 1);
            }
        }
        void AddPotentialPoint(int index)
        {
            if (index >= m_TempAssignNum.Length)
                return;
            if (m_TotalAssignNum <= GetTempPotentialSliderPoint())
                return;
            m_TempAssignNum[index] += 1;
            if (this.GetTempPotentialSliderPoint() < m_TotalAssignNum)
            {
                SetSeletBtnState(sAddBtn, index, 1);
            }
            else
            {
                SetSeletBtnState(sAddBtn, index, 0);
            }
            SetSeletBtnState(sReduction, index, 1);

            if (GetTempPotentialSliderPoint() == m_TotalAssignNum)
            {
                SetAllBtnState(sAddBtn, 0);
            }
            RefleshData();
        }
        void ReducePotentialPoint(int index)
        {
            if (index >= m_TempAssignNum.Length)
                return;
            m_TempAssignNum[index] -= 1;
            if (m_TempAssignNum[index] <= 0)
            {
                m_TempAssignNum[index] = 0;
                SetSeletBtnState(sReduction, index, 0);
            }
            else
            {
                SetSeletBtnState(sReduction, index, 1);
            }
            SetSeletBtnState(sAddBtn, index, 1);

            int tempNum = GetTempPotentialSliderPoint();
            if (tempNum == 0)
            {
                SetAllBtnState(sReduction, 0);
            }
            else if (tempNum != m_TotalAssignNum)
            {
                SetAllBtnState(sAddBtn, 1);
            }
            RefleshData();
        }
        #endregion
        #region 内部UI逻辑
        public void ResetUI()
        {
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null)
                return;
            PetObj attribute = new PetObj();
            if (!attribute.Load(pm.m_PetsTable.attribute[m_SelectedIndex]))
                return;
            int index = 0;
            SetRightProperty(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iStrength).name, attribute.power_slider_point, ref index);
            SetRightProperty(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iIntelligence).name, attribute.intelligence_slider_point, ref index);
            SetRightProperty(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iBone).name, attribute.root_bone_slider_point, ref index);
            SetRightProperty(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iPhysique).name, attribute.bodies_slider_point, ref index);
            SetRightProperty(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iAgility).name, attribute.agile_slider_point, ref index);
            SetRightProperty(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iBodyway).name, attribute.body_position_slider_point, ref index);

            //设置分配点信息
            m_TempAssignNum = new int[6];
            m_TempAssignNum[0] = attribute.power_slider_point;
            m_TempAssignNum[1] = attribute.intelligence_slider_point;
            m_TempAssignNum[2] = attribute.root_bone_slider_point;
            m_TempAssignNum[3] = attribute.bodies_slider_point;
            m_TempAssignNum[4] = attribute.agile_slider_point;
            m_TempAssignNum[5] = attribute.body_position_slider_point;

            index = 0;
            ChangeSliderValue(index++, m_TempAssignNum[0]);
            ChangeSliderValue(index++, m_TempAssignNum[1]);
            ChangeSliderValue(index++, m_TempAssignNum[2]);
            ChangeSliderValue(index++, m_TempAssignNum[3]);
            ChangeSliderValue(index++, m_TempAssignNum[4]);
            ChangeSliderValue(index++, m_TempAssignNum[5]);
        }
        public void RefleshData()
        {
            this.parent.transform.Find("PromptNum").GetComponent<Text>().text = "" + this.GetReadyPotentialSliderPoint();
            int index = 0;
            SetSliderValue(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iStrength).name, m_TempAssignNum[0], ref index);
            SetSliderValue(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iIntelligence).name, m_TempAssignNum[1], ref index);
            SetSliderValue(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iBone).name, m_TempAssignNum[2], ref index);
            SetSliderValue(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iPhysique).name, m_TempAssignNum[3], ref index);
            SetSliderValue(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iAgility).name, m_TempAssignNum[4], ref index);
            SetSliderValue(m_Grid,  Config.AttributeDefine.Get(Config.AttributeDefine.iBodyway).name, m_TempAssignNum[5], ref index);
        }
        void SetSliderValue(Transform nRoot, string attr, int value, ref int index)
        {
            this.parent.transform.Find("PromptNum").GetComponent<Text>().text = "" + this.GetTempPotentialSliderPoint();
            nRoot.GetChild(index).Find("num").GetComponent<Text>().text = value.ToString();
            index++;
        }
        void SetSeletBtnState(string btnName, int index, int state)
        {
            m_Grid.GetChild(index).Find(btnName).GetComponent<StateRoot>().CurrentState = state;
        }
        void SetAllBtnState(string btnName, int state)
        {
            for (int i = 0; i < m_Grid.childCount; i++)
            {
                m_Grid.GetChild(i).Find(btnName).GetComponent<StateRoot>().CurrentState = state;
            }
        }
        void ChangeSliderValue(int index, int value)
        {
            int sliderPoint = GetTempPotentialSliderPoint();
            if (sliderPoint == m_TotalAssignNum)
            {
                m_Grid.GetChild(index).Find(sAddBtn).GetComponent<StateRoot>().CurrentState = 0;
                m_Grid.GetChild(index).Find(sReduction).GetComponent<StateRoot>().CurrentState = value == 0 ? 0 : 1;
            }
            else
            {
                m_Grid.GetChild(index).Find(sAddBtn).GetComponent<StateRoot>().CurrentState = 1;
                m_Grid.GetChild(index).Find(sReduction).GetComponent<StateRoot>().CurrentState = value == 0 ? 0 : 1;
            }
        }
        int GetTempPotentialSliderPoint()
        {
            int num = 0;
            for (int i = 0; i < m_TempAssignNum.Length; i++)
            {
                num += m_TempAssignNum[i];
            }
            return num;
        }
        int GetReadyPotentialSliderPoint()
        {
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null)
                return 0;
            PetsAttribute attribute = pm.m_PetsTable.attribute[m_SelectedIndex];
            return attribute.sliderpointAtt[0]
                + attribute.sliderpointAtt[1]
                + attribute.sliderpointAtt[2]
                + attribute.sliderpointAtt[3]
                + attribute.sliderpointAtt[4]
                + attribute.sliderpointAtt[5];
        }
        bool SameSliderValue()
        {
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null)
                return false;
            PetsAttribute attribute = pm.m_PetsTable.attribute[m_SelectedIndex];
            if (attribute.sliderpointAtt[0] == m_TempAssignNum[0]
                && attribute.sliderpointAtt[1] == m_TempAssignNum[1]
                && attribute.sliderpointAtt[2] == m_TempAssignNum[2]
                && attribute.sliderpointAtt[3] == m_TempAssignNum[3]
                && attribute.sliderpointAtt[4] == m_TempAssignNum[4]
                && attribute.sliderpointAtt[5] == m_TempAssignNum[5])
                return true;
            return false;
        }
        #endregion
    }
}

#endif
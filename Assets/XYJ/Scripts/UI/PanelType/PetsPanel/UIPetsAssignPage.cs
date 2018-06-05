#if !USE_HOT
using Config;
using NetProto.Hot;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;
using xys.UI;
using xys.UI.Dialog;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIPetsAssignPage : HotTablePageBase
    {
        protected string sAddBtn = "Add";
        protected string sReduction = "Reduction";

        [SerializeField]
        Button m_AssignSureBtn;
        [SerializeField]
        Button m_AssignCancelBtn;
        [SerializeField]
        Button m_AssignTipsBtn;

        [SerializeField]
        UIPetsAssignScrollView m_ScrollView;
        [SerializeField]
        Transform m_RightRoot;
        [SerializeField]
        Text m_PetsNickName;


        protected int m_PowerPoint;
        protected int m_IntelligencePoint;
        protected int m_RootBonePoint;
        protected int m_BodiesPoint;
        protected int m_AgilePoint;
        protected int m_BodyPositionPoint;

        int[] m_AddTeampPotential = new int[6];

        PetsPanel m_Panel;

        TwoBtn m_Screen;

        UIPetsAssignPage() : base(null) { }
        UIPetsAssignPage(HotTablePage page) : base(page)
        {
        }

        protected override void OnInit()
        {
            this.m_ScrollView.OnInit();

            m_AssignSureBtn.onClick.AddListener(this.AssignSureEvent);
            m_AssignCancelBtn.onClick.AddListener(this.AssignCancelEvent);

            for (int i = 0; i < m_RightRoot.childCount; i++)
            {
                int index = i;
                Transform item = m_RightRoot.GetChild(i);
                item.Find(sAddBtn).GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.AddPotentialPoint(index);
                });
                item.Find(sAddBtn).GetComponent<DPOnPointer>().onLongPress.AddListener(() =>
                {
                    this.AddPotentialPoint(index);
                });
                item.Find(sReduction).GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.ReducePotentialPoint(index);
                });
                item.Find(sReduction).GetComponent<DPOnPointer>().onLongPress.AddListener(() =>
                {
                    this.ReducePotentialPoint(index);
                });
            }
        }
        protected override void OnShow(object args)
        {
            this.m_Panel = args as PetsPanel;
            if (m_Panel == null)
            {
                Debuger.LogError("Panel null");
                this.parent.gameObject.SetActive(false);
                return;
            }

            Event.Subscribe(EventID.Pets_DataRefresh, this.Refresh);
        }

        protected override IEnumerator OnShowSync(object args)
        {
            this.m_ScrollView.m_ScrollRect.content.localPosition = Vector2.zero;
            yield return 0;
            this.m_ScrollView.m_ScrollRect.content.GetComponent<xys.UI.ContentSizeFitter>().SetDirty();
            //
            this.m_AddTeampPotential = new int[6];
            RefleshData();
            InitAllBtn();
        }
        #region 事件
        void AssignSureEvent()
        {
            int[] point = new int[6];
            point[0] = m_AddTeampPotential[0];
            point[1] = m_AddTeampPotential[1];
            point[2] = m_AddTeampPotential[2];
            point[3] = m_AddTeampPotential[3];
            point[4] = m_AddTeampPotential[4];
            point[5] = m_AddTeampPotential[5];
            if (m_AddTeampPotential[0] != 0
                || m_AddTeampPotential[1] != 0
                 || m_AddTeampPotential[2] != 0
                 || m_AddTeampPotential[3] != 0
                 || m_AddTeampPotential[4] != 0
                 || m_AddTeampPotential[5] != 0)
            {
                SetPetPotentialPointRequest request = new SetPetPotentialPointRequest();
                request.index = m_Panel.selected;
                request.power = point[0];
                request.intelligence = point[1];
                request.root_bone = point[2];
                request.bodies = point[3];
                request.agile = point[4];
                request.body_position = point[5];

                App.my.eventSet.FireEvent(EventID.Pets_SetPotential, request);
            }
            else
            {
                SystemHintMgr.ShowHint("没有潜能点可确定");
            }
        }
        void AssignCancelEvent()
        {
            if (m_Screen != null) return;
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null)
                return;
            PetsAttribute petAttribute = pm.m_PetsTable.attribute[m_Panel.selected];
            Config.Item itempro = Config.Item.Get(int.Parse(Config.kvCommon.Get("PetsResetPointItem").value));
            if (itempro == null)
                return;
            if (petAttribute.pointAtt[0] == 0 &&
                petAttribute.pointAtt[1] == 0 &&
                petAttribute.pointAtt[2] == 0 &&
                petAttribute.pointAtt[3] == 0 &&
                petAttribute.pointAtt[4] == 0 &&
                petAttribute.pointAtt[5] == 0)
            {
                SystemHintMgr.ShowHint("没有潜能点可重置");
                return;
            }
            int ncopper = m_Panel.selectedPetObj.lv * 5 * 1000;
            int acopper = (int)App.my.localPlayer.silverShellValue;
            string color = ncopper < acopper ? "G" : "R";
            string copper = (ncopper).ToString();
            string copperStr = string.Format("<sprite n={0} w=24 h=24>", "item_silver");
            int resetTime = petAttribute.reset_times;
            string des = string.Empty;
//             if (resetTime > 0)
//                 des = string.Format("每只灵兽前3次重置加点操作可免费进行 当前为第{0}次 确认重置吗", 3 - resetTime);
//             else
            {
                des = string.Format("<color={0}>{1}</color>", Config.QualitySourceConfig.Get(itempro.quality).color, itempro.name);
                des = string.Format("重置当前灵兽加点需要消耗一枚{0} 确认重置吗", des);
            }
            //if (resetTime > 0)
            {
                m_Screen = TwoBtn.Show(
                "", des, "取消", () => { return false; },
                "确定", () =>
                {
                    if (m_Panel != null && m_Panel.selectedPetObj.total_potential_point != 0)

                    {
                        PetItemRequest request = new PetItemRequest();
                        request.index = m_Panel.selected;
                        App.my.eventSet.FireEvent(EventID.Pets_ResetPotential, request);
                    }
                    else
                        SystemHintMgr.ShowHint("重置失败");
                    return false;
                }, true, true, () => { m_Screen = null; });
            }
            //else
            {
                //                 CommonTipsParam para = new CommonTipsParam();
                //                 para.itemId = ConfigCommon.instance.petResetPointItemId;
                //                 para.itemNum = 1;// data.needMaterical;
                //                 para.text2 = des;
                //                 para.rightAction = () =>
                //                 {
                //                     if (1 > ItemManage.Instance.GetNumByItemID(ConfigCommon.instance.petResetPointItemId))
                //                     {
                //                         SystemHintMgr.ShowHint("材料不足");
                //                         return;
                //                     }
                // 
                //                     if (m_Panel != null && m_Panel.selectedPetObj.total_potential_point != 0)
                //                         Utils.EventDispatcher.Instance.TriggerEvent<int>(PetsSystem.Event.ResetPotentialPoint, m_Panel.selectedPet);
                //                     else
                //                     SystemHintMgr.ShowHint("重置失败");
                //                         
                //                 };
                //                 UICommonPannel.ShowCommonTip(para);

                return;
            }
        }
        #endregion

        #region 内部UI逻辑
        void Refresh()
        {
            this.ResetUI();
            this.RefleshData();
        }
        void ResetUI()
        {
            m_AddTeampPotential = new int[6];
            SetAllBtnState(sAddBtn, GetReadyPotentialPoint() == 0 ? 0 : 1);
            SetAllBtnState(sReduction, 0);
        }
        void RefleshData()
        {
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null || this.m_Panel.selectedPetObj == null)
                return;

            m_PetsNickName.text = m_Panel.selectedPetObj.nickName;
            //设置潜能点
            int potentialPoint = GetReadyPotentialPoint() - GetAllTeampPotentialPoint();
            this.parent.transform.Find("Potential").GetComponent<Text>().text = potentialPoint.ToString();

            int index = 0,attributeIndex = 0;
            BattleAttri tempAtttribute = m_Panel.selectedPetObj.GetNextPotentialAttribute(m_AddTeampPotential);
            Transform nRoot = m_ScrollView.m_ScrollRect.content.transform.GetChild(0);
            if (m_ScrollView != null)
                m_ScrollView.RefreshItem(m_Panel.selectedPetObj, tempAtttribute);

            index = 0;
            attributeIndex = AttributeDefine.iStrength;
            SetRightProperty(m_RightRoot, AttributeDefine.Get(attributeIndex).name, (int)m_Panel.selectedPetObj.power_point + m_AddTeampPotential[attributeIndex - 1], ref index);
            attributeIndex = AttributeDefine.iIntelligence;
            SetRightProperty(m_RightRoot, AttributeDefine.Get(attributeIndex).name, (int)m_Panel.selectedPetObj.intelligence_point + m_AddTeampPotential[attributeIndex - 1], ref index);
            attributeIndex = AttributeDefine.iBone;
            SetRightProperty(m_RightRoot, AttributeDefine.Get(attributeIndex).name, (int)m_Panel.selectedPetObj.root_bone_point + m_AddTeampPotential[attributeIndex - 1], ref index);
            attributeIndex = AttributeDefine.iPhysique;
            SetRightProperty(m_RightRoot, AttributeDefine.Get(attributeIndex).name, (int)m_Panel.selectedPetObj.bodies_point + m_AddTeampPotential[attributeIndex - 1], ref index);
            attributeIndex = AttributeDefine.iAgility;
            SetRightProperty(m_RightRoot, AttributeDefine.Get(attributeIndex).name, (int)m_Panel.selectedPetObj.agile_point + m_AddTeampPotential[attributeIndex - 1], ref index);
            attributeIndex = AttributeDefine.iBodyway;
            SetRightProperty(m_RightRoot, AttributeDefine.Get(attributeIndex).name, (int)m_Panel.selectedPetObj.body_position_point + m_AddTeampPotential[attributeIndex - 1], ref index);
        }

        void InitAllBtn()
        {
            SetAllBtnState(sAddBtn, GetReadyPotentialPoint() == 0 ? 0 : 1);
            SetAllBtnState(sReduction, 0);
        }
        void SetLeftProperty(Transform nRoot, string attr, int value, int addValue, ref int index)
        {
            nRoot.GetChild(index).Find("attr").GetComponent<Text>().text = attr;
            nRoot.GetChild(index).Find("num").GetComponent<Text>().text = value.ToString();
            nRoot.GetChild(index).Find("added").GetComponent<Text>().text = (addValue - value) == 0 ? "" : "+" + (addValue - value).ToString();
            index++;
        }

        void SetRightProperty(Transform nRoot, string attr, int value, ref int index)
        {
            nRoot.GetChild(index).Find("attr").GetComponent<Text>().text = attr;
            nRoot.GetChild(index).Find("num").GetComponent<Text>().text = value.ToString();
            //
            if (m_AddTeampPotential[index] > 0)
            {
                nRoot.GetChild(index).Find("num").GetComponent<Text>().color = XTools.Utility.ParseColor("61e171", 0);
            }
            else
            {
                nRoot.GetChild(index).Find("num").GetComponent<Text>().color = Color.white;
            }
            index++;
        }

        void AddPotentialPoint(int index)
        {
            if (index < m_AddTeampPotential.Length)
            {
                int tempNum = GetAllTeampPotentialPoint();
                int potentialPoint = GetReadyPotentialPoint();
                if (tempNum >= potentialPoint)
                    return;

                m_AddTeampPotential[index] += 1;
                if (GetAllTeampPotentialPoint() < potentialPoint)
                {
                    SetSeletBtnState(sAddBtn, index, 1);
                }
                else
                {
                    SetSeletBtnState(sAddBtn, index, 0);
                }
                //
                SetSeletBtnState(sReduction, index, 1);
                //天赋点为0
                if (GetAllTeampPotentialPoint() == potentialPoint)
                    SetAllBtnState(sAddBtn, 0);

                RefleshData();
            }
        }

        void ReducePotentialPoint(int index)
        {
            if (index < m_AddTeampPotential.Length)
            {
                int tempNum = GetAllTeampPotentialPoint();
                if (tempNum <= 0)
                    return;
                int potentialPoint = GetReadyPotentialPoint();
                //
                m_AddTeampPotential[index] -= 1;
                if (m_AddTeampPotential[index] <= 0)
                {
                    m_AddTeampPotential[index] = 0;
                    SetSeletBtnState(sReduction, index, 0);
                }
                else
                {
                    SetSeletBtnState(sReduction, index, 1);
                }
                //
                SetSeletBtnState(sAddBtn, index, 1);
                //
                tempNum = GetAllTeampPotentialPoint();
                if (tempNum == 0)
                    SetAllBtnState(sReduction, 0);
                else if (tempNum != potentialPoint)
                    SetAllBtnState(sAddBtn, 1);
                RefleshData();
            }
        }
        void SetSeletBtnState(string btnName, int index, int state)
        {
            m_RightRoot.GetChild(index).Find(btnName).GetComponent<StateRoot>().CurrentState = state;
        }
        void SetAllBtnState(string btnName, int state)
        {
            for (int i = 0; i < m_RightRoot.childCount; i++)
            {
                m_RightRoot.GetChild(i).Find(btnName).GetComponent<StateRoot>().CurrentState = state;
            }
        }

        /// <summary>
        /// 获得面板点击后的潜能点
        /// </summary>
        /// <returns></returns>
        int GetAllTeampPotentialPoint()
        {
            int num = 0;
            for (int i = 0; i < m_AddTeampPotential.Length; i++)
            {
                num += m_AddTeampPotential[i];
            }
            return num;
        }
        /// <summary>
        /// 获得剩余潜能点
        /// </summary>
        /// <returns></returns>
        int GetReadyPotentialPoint()
        {
            if (m_Panel.selectedPetObj == null)
                return 0;
            return m_Panel.selectedPetObj.total_potential_point - (m_Panel.selectedPetObj.power_point + m_Panel.selectedPetObj.intelligence_point + m_Panel.selectedPetObj.root_bone_point + m_Panel.selectedPetObj.bodies_point + m_Panel.selectedPetObj.agile_point + m_Panel.selectedPetObj.body_position_point);
        }
        #endregion
    }
}

#endif
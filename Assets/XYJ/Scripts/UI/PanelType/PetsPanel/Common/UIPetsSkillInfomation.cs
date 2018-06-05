#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using NetProto;
using NetProto.Hot;

namespace xys.hot.UI
{
    [System.Serializable]
    class UIPetsSkillInfomation
    {
        readonly string[] c_SkillQualityDefine = new string[] { "绝技", "天赋", "主动", "被动" };
        [SerializeField]
        Image m_Icon;
        [SerializeField]
        Text m_PetName;
        [SerializeField]
        Text m_Score;
        [SerializeField]
        StateRoot m_QualitySR;
        [SerializeField]
        Transform m_SkillRoot;
        [SerializeField]
        Text m_SkillName;
        [SerializeField]
        Text m_SkillLockCountText;
        [SerializeField]
        Text m_SkillLockText;
        [SerializeField]
        Text m_SkillDesr;
        [SerializeField]
        Text m_SkillQuality;
        [SerializeField]
        Image m_MaterialImage;
        [SerializeField]
        Text m_MaterialText;
        [SerializeField]
        Button m_MaterialBtn;
        [SerializeField]
        StateRoot m_LockStateInfo;

        System.Action<int> m_Action = null;

        int m_SelectedIndex;
        int m_SkillID;

        PetsPanel m_Panel;
        public PetsPanel panel { set { m_Panel = value; } }
        public void OnShow()
        {
            this.SetSkillInfos(-1, null);
            m_MaterialBtn.onClick.AddListener(this.OnShowMaterialTips);
        }

        public void OnHide()
        {
            m_MaterialBtn.onClick.RemoveListener(this.OnShowMaterialTips);
        }

        public void Reset(System.Action<int> action = null)
        {
            PetsMgr petsMgr = m_Panel.petsMgr;

            if (m_Panel.selectedPetObj != null)
            {
                PetObj petobj = m_Panel.selectedPetObj;
                Config.PetAttribute property = Config.PetAttribute.Get(petobj.id);
                if (property == null)
                    return;
                m_SelectedIndex = -1;
                //设置头像
                Helper.SetSprite(m_Icon, property.icon);
                m_PetName.text = petobj.nickName;
                m_Score.text = "" + 0;
                m_SkillLockCountText.text = "" + petobj.lockNumSKills + "/" + petsMgr.MaxSkillLock;

                int[] teapQuality = new int[] { 0, 0, 0, 1, 2, 3 };
                m_QualitySR.CurrentState = teapQuality[property.type - 1];

                for (int i = 0; i < m_SkillRoot.childCount; i++)
                {
                    m_SkillRoot.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    m_SkillRoot.GetChild(i).Find("lock").gameObject.SetActive(false);
                }

                PetSkillData skillInfo = null;
                int childIndex = 0;

                if (petobj.trickSkills.id != 0)
                {
                    skillInfo = petobj.trickSkills;
                    Transform item = m_SkillRoot.GetChild(childIndex);
                    this.SetSkillIcon(skillInfo, item);
                    childIndex +=1;
                }
                if (petobj.talentSkills.id != 0)
                {
                    skillInfo = petobj.talentSkills;
                    Transform item = m_SkillRoot.GetChild(childIndex);
                    this.SetSkillIcon(skillInfo, item);
                    childIndex += 1;
                }
                for(int i = 0; i < petobj.passiveSkills.Count;i++)
                {
                    if (petobj.passiveSkills[i].id != 0)
                    {
                        Transform item = m_SkillRoot.GetChild(childIndex);
                        this.SetSkillIcon(petobj.passiveSkills[i], item);
                        childIndex += 1;
                    }
                }

                for (int i = childIndex; i < m_SkillRoot.childCount; i++)
                    m_SkillRoot.GetChild(i).GetComponent<StateRoot>().CurrentState = 2;
            }
        }

        void SetSkillInfos(int lockNumSKills, PetSkillData skillInfo = null)
        {
            if (skillInfo == null)
            {
                m_SkillQuality.text = string.Empty;
                m_SkillLockText.text = string.Empty;
                m_SkillName.text = string.Empty;
                m_SkillDesr.text = string.Empty;
                m_LockStateInfo.CurrentState = 2;
                return;
            }
            PetsMgr petsMgr = m_Panel.petsMgr;
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;

            string skillQuality = string.Empty;

            if (Config.SkillConfig.GetAll().ContainsKey(skillInfo.id))
            {
                Config.SkillConfig data = Config.SkillConfig.Get(skillInfo.id);
                if (data.isPetStunt)
                    skillQuality = c_SkillQualityDefine[0];
                else
                    skillQuality = c_SkillQualityDefine[2];
                m_SkillDesr.text = GlobalSymbol.ToUT(data.des);
                m_SkillName.text = data.name;
            }

            if (Config.PassiveSkills.GetAll().ContainsKey(skillInfo.id))
            {
                Config.PassiveSkills pData = Config.PassiveSkills.Get(skillInfo.id);
                if (pData.type == (int)Config.PassiveSkills.Type.Talent)
                    skillQuality = c_SkillQualityDefine[1];
                else if (pData.type == (int)Config.PassiveSkills.Type.Pets)
                    skillQuality = c_SkillQualityDefine[3];
                m_SkillName.text = pData.name;
                m_SkillDesr.text = GlobalSymbol.ToUT(pData.des);
            }

            m_SkillQuality.text = "【" + skillQuality + "】";
            m_SkillLockText.text = skillInfo.islock == 0 ? "锁定" : "解锁";
            //
            if (lockNumSKills >= petsMgr.MaxSkillLock)
            {
                m_LockStateInfo.CurrentState = 1;
                m_LockStateInfo.transform.Find("LockDes").GetComponent<Text>().text = "可锁定技能个数已达上限";
            }
            else
            {
                m_LockStateInfo.CurrentState = skillInfo.islock == 0 ? 0 : 1;
                m_LockStateInfo.transform.Find("LockDes").GetComponent<Text>().text = "该技能已锁定";
            }
            //
            if (lockNumSKills < petsMgr.MaxSkillLock)
            {
                int materialID = Config.kvCommon.petLockInfo[lockNumSKills][0];
                int count = Config.kvCommon.petLockInfo[lockNumSKills][1];
                int hCount = packageMgr.GetItemCount(materialID);
                if (count <= hCount)
                    m_MaterialText.text = hCount + "/" + count;
                else
                    m_MaterialText.text = string.Format("<color=#{0}>", "ef3c49") + hCount + "/" + count + "</color>";
                Helper.SetSprite(m_MaterialImage, Config.Item.Get(materialID).icon);
            }
        }

        void SetSkillIcon(PetSkillData data, Transform item)
        {
            item.GetComponent<Button>().onClick.AddListener(() => { this.OnShowSkillEvent(item, data); });
            //
            item.GetComponent<StateRoot>().CurrentState = 0;
            item.Find("lock").gameObject.SetActive(data.islock == 1);

            int curIndex = 0;
            if (Config.SkillConfig.GetAll().ContainsKey(data.id))
            {
                Config.SkillConfig skillData = Config.SkillConfig.Get(data.id);
                curIndex = 1;
                //                 if (!skillData.isPetStunt)
                //                     curIndex = 1;
                //                 else 
                //                     curIndex = 0;
            }
            if (Config.PassiveSkills.GetAll().ContainsKey(data.id))
            {
                Config.PassiveSkills pSkillData = Config.PassiveSkills.Get(data.id);
                if (pSkillData.type == (int)Config.PassiveSkills.Type.Talent)
                    curIndex = 3;
                else if (pSkillData.type == (int)Config.PassiveSkills.Type.Pets)
                    curIndex = 2;
            }
            if(curIndex == 0)
            {
                item.GetComponent<StateRoot>().CurrentState = 2;
                return;
            }

            if (Config.SkillIconConfig.GetAll().ContainsKey(data.id))
                Helper.SetSprite(item.Find("Icon").GetComponent<Image>(), Config.SkillIconConfig.Get(data.id).icon);


            item.Find("Tag").gameObject.SetActive(true);
            if (curIndex == 1 || curIndex == 2)
                item.Find("Tag").gameObject.SetActive(false);
            else
                item.Find("Tag").GetComponent<StateRoot>().CurrentState = curIndex;
        }

        void OnShowSkillEvent(Transform item, PetSkillData data)
        {
            if (m_SelectedIndex == item.GetSiblingIndex())
                return;

            if (m_SelectedIndex != -1)
                m_SkillRoot.GetChild(m_SelectedIndex).GetComponent<StateRoot>().CurrentState = 0;

            if (item != null)
            {
                m_SelectedIndex = item.GetSiblingIndex();
                m_SkillRoot.GetChild(m_SelectedIndex).GetComponent<StateRoot>().CurrentState = 1;
                m_SkillID = data.id;
                this.SetSkillInfos(m_Panel.selectedPetObj.lockNumSKills, data);
            }
        }

        void OnShowMaterialTips()
        {
            if (m_Panel.selectedPetObj == null)
                return;

            PetObj petAttribute = m_Panel.selectedPetObj;
            int materialID = Config.kvCommon.petLockInfo[petAttribute.lockNumSKills][0];
            //             AnyArgs args = new AnyArgs();
            //             args.Add("id", materialID);
            //             args.Add("isClickThrough", false);
            //             args.Add("isShowObtain", true);
            //             UISystem.Instance.ShowPanel("UIItemTipsPanel", args, true);

            //App.my.uiSystem.ShowPanel("UIIItemTipsPanel", new object { });
        }

        public void Refresh()
        {
            this.SetSkillInfos(-1, null);
        }
        public int skillID { get { return m_SkillID; } }
    }
}

#endif
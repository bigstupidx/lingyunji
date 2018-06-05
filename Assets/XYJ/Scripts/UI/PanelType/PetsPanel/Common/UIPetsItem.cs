#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using xys.UI.State;
using xys.UI;
using NetProto.Hot;
using NetProto;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{
    [AutoILMono]
    class UIPetsItem
    {
        [SerializeField]
        Transform m_Transform;
        public Transform transform { get { return m_Transform; } }
        [SerializeField]
        Image m_Background;
        [SerializeField]
        Image m_Icon;
        [SerializeField]
        StateRoot m_Tag;
        [SerializeField]
        Image m_Mask;

        protected PetsAttribute m_Attribute = null;
        protected System.Action<UIPetsItem> m_ClickEvent = null;

        float m_IsDeath = 0.0f;
        // int m_TimerID;
        int m_AttributeIndex = 0;
        void Awake()
        {
            UnityEngine.UI.Button selfButton = this.m_Transform.GetComponent<UnityEngine.UI.Button>();
            if (selfButton != null) selfButton.onClick.AddListener(this.OnClick);
        }
        //设置子控件信息
        public void Set(int index, PetsAttribute attribute,bool isLockState = false, System.Action<UIPetsItem> click_event = null)
        {
            PetsMgr petMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petMgr == null)
            {
                this.m_Transform.gameObject.SetActive(false);
                return;
            }
            m_AttributeIndex = index;
            m_Attribute = attribute;
            m_ClickEvent = click_event;
            if(m_Attribute != null)
            {
                this.CheckPetState();
                //设置状态
                this.m_Transform.GetComponent<StateRoot>().CurrentState = 2;
                //设置头像
                Helper.SetSprite(m_Icon, Config.PetAttribute.Get(m_Attribute.id).icon);
            }
            else
            {
                m_Tag.CurrentState = 2;
                this.m_Transform.GetComponent<StateRoot>().CurrentState = isLockState ? 0 : 1;
            }
        }

        void Timer_DeathEvent(double beginTime)
        {
        }

        void CheckPetState()
        {
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return;
            //判断上次死亡时间
            bool death = (float)App.my.srvTimer.GetTime.GetCurrentTime() - (float)m_Attribute.last_die_time < float.Parse(Config.kvCommon.Get("petReliveCD").value);

            float petReliveCD = float.Parse(Config.kvCommon.Get("petReliveCD").value);
            float lastDeadTime = (float)m_Attribute.last_die_time;
            float srvTime = (float)App.my.srvTimer.GetTime.GetCurrentTime();
            //出战
            int fightPetData = petsMgr.GetFightPetID() ;
            if (fightPetData != -1)
                if (m_AttributeIndex == fightPetData)
                    m_Tag.CurrentState = death ?1 : 0;
            else
                    m_Tag.CurrentState = death? 3 : 2;

            if (death)
            {
                m_IsDeath = petReliveCD - srvTime - lastDeadTime;
                m_Mask.fillAmount = m_IsDeath / petReliveCD;
            }   
        }

        //更新子控件信息
        public void EnablePlay(bool isPlay)
        {
            bool death = (float)App.my.srvTimer.GetTime.GetCurrentTime() - (float)m_Attribute.last_die_time < float.Parse(Config.kvCommon.Get("petReliveCD").value);
            if (isPlay)
                m_Tag.CurrentState = death ? 1 : 0;
            else
                m_Tag.CurrentState = death ? 3 : 2;
        }

        void OnClick()
        {
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return;
            int index = this.m_Transform.GetSiblingIndex();
            if (m_Attribute == null&& !petsMgr.GetPetsHoleState(index,Config.PetsHoleDefine.GetAll().Count))
            {
                this.ShowOpenHoldEvent();
            }
            else
            {
                if (m_ClickEvent != null)
                    m_ClickEvent(this);
            }
        }

        void Update()
        {
            if (m_IsDeath > 0)
            {
                m_Mask.fillAmount = m_IsDeath / float.Parse(Config.kvCommon.Get("petReliveCD").value);
                m_IsDeath -= Time.deltaTime;// MainTime.realDeltaTime;
                if (m_IsDeath <= 0)
                    this.CheckPetState();
            }
        }
        xys.UI.Dialog.TwoBtn m_Screen;
        void ShowOpenHoldEvent()
        {
            PackageModule packageModule = App.my.localPlayer.GetModule<PackageModule>();
            if (packageModule == null)
                return;
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return;
            Config.PetsHoleDefine data = Config.PetsHoleDefine.Get(petsMgr.GetHoles() + 1);
            if (data == null)
                return;

            int itemId = data.itemID;
            Config.Item itemData = Config.Item.Get(itemId);
            string des = string.Format(Config.TipsContent.Get(5001).des, data.Count, itemData.name);

            //             CommonTipsParam para = new CommonTipsParam();
            //             para.itemId = itemId;
            //             para.itemNum = data.needMaterical;
            //             para.text2 = des;
            //             para.rightAction = () => 
            //             {
            //                 if (data.needMaterical > ItemManage.Instance.GetNumByItemID(data.needMatericalIdentity))
            //                 {
            //                     SystemHintMgr.ShowHint("材料不足");
            //                     return;
            //                 }
            //                 Utils.EventDispatcher.Instance.TriggerEvent(PetsSystem.Event.SetHoles);
            //             };
            //             UICommonPannel.ShowCommonTip(para);

            //测试
            if (data.Count > packageModule.GetItemCount(data.itemID))
            {
                return;
            }
            PetItemRequest pir = new PetItemRequest();
            pir.index = m_AttributeIndex;
            pir.itemId = data.itemID;
            pir.itemCount = data.Count;
            App.my.eventSet.FireEvent<PetItemRequest>(EventID.Pets_OpenHoles, pir);
        }

        public int attributeIndex { get { return m_AttributeIndex; } }
        public PetsAttribute element { get { return m_Attribute; } }

        public Color color { set { m_Background.color = value; } }

        public UnityEngine.UI.Image icon { get { return m_Icon; } }
    }
}

#endif
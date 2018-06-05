#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Config;
using xys.battle;
using xys.UI;

namespace xys.hot.UI
{
    public partial class UIRoleInfoMgr
    {
        #region 玩家
        UIHpProgressBar m_meHp;
        UIHpProgressBar m_meHuti;
        UIMpProgressBar m_meZhenqi;
        #endregion

        #region 目标
        //目标事件
        xys.hot.Event.HotObjectEventAgent m_targetEvent;
        UIHpProgressBar m_targetHp;
        UIHpProgressBar m_targetHuti;
        Text m_targetName;
        Transform m_targetRoot;
        #endregion

        public UIRoleInfoMgr(Transform root)
        {
            root = root.Find("Offset/RoleInfo/Offset");
            m_meHp = new UIHpProgressBar(Helper.FindComnpnent<Image>(root, "roleIcon/offset/hpMask/hp"), Helper.FindComnpnent<Text>(root, "roleIcon/offset/hpMask/Text"));
            m_meZhenqi = new UIMpProgressBar(root.Find("mp"));
            m_meHuti = new UIHpProgressBar(Helper.FindComnpnent<Image>(root, "defend/value"), null);

            m_targetRoot = root.Find("player");
            m_targetHp = new UIHpProgressBar(Helper.FindComnpnent<Image>(m_targetRoot, "hpMask/hp"), null);
            m_targetHuti = new UIHpProgressBar(Helper.FindComnpnent<Image>(m_targetRoot, "defendMask/defend"), null);
            m_targetName = Helper.FindComnpnent<Text>(m_targetRoot, "name");
        }

        public void OnShow(xys.hot.Event.HotObjectEventAgent Event)
        {
            Event.Subscribe(NetProto.AttType.AT_HP, (p) => { m_meHp.SetCurValue(p.currentValue.intValue); });
            Event.Subscribe(NetProto.AttType.AT_MAX_HP, (p) => { m_meHp.SetMaxValue(p.currentValue.intValue); });
            Event.Subscribe(NetProto.AttType.AT_HuTi, (p) => { m_meHuti.SetCurValue(p.currentValue.intValue); });
            Event.Subscribe(NetProto.AttType.AT_Max_HuTi, (p) => { m_meHuti.SetMaxValue(p.currentValue.intValue); });
            Event.Subscribe(NetProto.AttType.AT_ZhenQi, (p) => { m_meZhenqi.SetCurValue(p.currentValue.intValue); });


            Event.Subscribe<IObject>(EventID.MainPanel_SetTarget, OnSetTarget);

            //已经在游戏中了，打开界面先初始化一次
            if (App.my.appStateMgr.curState == AppStateType.GameIn)
            {
                m_meHp.Init(App.my.localPlayer.hpValue, (int)App.my.localPlayer.maxHpValue);
                m_meHuti.Init(App.my.localPlayer.huTiValue, (int)App.my.localPlayer.maxHuTiValue);
                m_meZhenqi.SetCurValue(App.my.localPlayer.zhenQiValue);

                OnSetTarget(App.my.localPlayer.GetUIChooseTarget());
            }
        }

        public void OnHide()
        {
            OnSetTarget(null);
        }

        void OnSetTarget(IObject obj)
        {
            m_targetRoot.gameObject.SetActive(obj != null);
            if (m_targetEvent != null)
                m_targetEvent.Release();
            if (obj == null)
                m_targetEvent = null;
            else
            {
                m_targetEvent = new Event.HotObjectEventAgent(obj.eventSet);
                m_targetEvent.Subscribe(NetProto.AttType.AT_HP, (p) => { m_targetHp.SetCurValue(p.currentValue.intValue); });
                m_targetEvent.Subscribe(NetProto.AttType.AT_MAX_HP, (p) => { m_targetHp.SetMaxValue(p.currentValue.intValue); });
                m_targetEvent.Subscribe(NetProto.AttType.AT_HuTi, (p) => { m_targetHuti.SetCurValue(p.currentValue.intValue); });
                m_targetEvent.Subscribe(NetProto.AttType.AT_Max_HuTi, (p) => { m_targetHuti.SetMaxValue(p.currentValue.intValue); });

                //需要初始化目标数据
                m_targetHp.Init(obj.hpValue, (int)obj.maxHpValue);
                m_targetHuti.Init(obj.huTiValue, (int)obj.maxHuTiValue);                    
                m_targetName.text = obj.cfgInfo.name;//临时使用配置表的名字
            }
        }
    }
}
#endif
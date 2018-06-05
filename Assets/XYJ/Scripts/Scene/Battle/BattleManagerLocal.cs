using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;

namespace xys.battle
{
    public class BattleManagerLocal : BattleManagerBase
    {
        public AutoChooseTarget m_autoChooseTarget { get; private set; }
        EventAgent m_event = new EventAgent();

        public BattleManagerLocal(IObject obj)
            : base(obj)
        {
            m_event.Subscribe<UIBattleFunType>(EventID.MainPanel_BattleFuntion,OnUIBattleFuntion);
            m_event.Subscribe<UIPanelBase>(EID.OpenPanel, "UIMainPanel", OnOpenMainPannel);

            m_autoChooseTarget = AddCompoment(new AutoChooseTarget());
            m_isAiByLocal = true;
            
        }


        public override void Destroy()
        {
            base.Destroy();
            m_event.Release();
        }


        //打开主面板需要初始化技能区域
        void OnOpenMainPannel(UIPanelBase p)
        {
            ((SkillManagerLocal)m_skillMgr).Slot_InitUI();
        }

        public override IObject GetTarget()
        {
            IObject obj = m_targetMgr.target;
            //如果没有目标，则使用准心目标
            if (obj == null)
                obj = m_autoChooseTarget.chooseTarget;
            return obj;

        }


        void OnUIBattleFuntion(UIBattleFunType type)
        {
            //切换目标
            if(type == UIBattleFunType.SwitchTarget)
            {
                ((BattleManagerLocal)m_obj.battle).m_autoChooseTarget.SwitchTarget();
            }
        }
    }
}

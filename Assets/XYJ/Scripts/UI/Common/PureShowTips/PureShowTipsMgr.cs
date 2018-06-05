using Config;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;
using xys.UI.State;

namespace xys.UI
{
    public enum PureShowType
    {
        PST_UnlockTitle,                //称号解锁
        PST_PersonalityChangeValue,     //个性值变化
    }

    public class PureShowTip
    {
        public PureShowType m_type;

        public PersonalityChangeValue m_personality;
        public TitleUnlock m_title;

        public PureShowTip(PureShowType type, PersonalityChangeValue personality)
        {
            m_type = type;
            m_personality = personality;
        }
        public PureShowTip(PureShowType type, TitleUnlock title)
        {
            m_type = type;
            m_title = title;
        }
    }

    public class PersonalityChangeValue
    {
        public int m_id;         //个性Id
        public int m_addValue;   //个性增长值

        public PersonalityChangeValue(int id, int addValue)
        {
            m_id = id;
            m_addValue = addValue;
        }
    }

    public class TitleUnlock
    {
        public int m_id;         //称号Id

        public TitleUnlock(int id)
        {
            m_id = id;
        }
    }

    public class PureShowTipsMgr : MonoBehaviour
    {
        [SerializeField]
        GameObject m_unlockTitleTip;
        [SerializeField]
        GameObject m_personalityTip;

        Queue<PureShowTip> tipQue;

        private float m_lastTriggerTime;
        [SerializeField]
        private const float m_minCutTime = 2.8f;

        public void Init()
        {
            tipQue = new Queue<UI.PureShowTip>();

            m_unlockTitleTip.SetActive(false);
            m_personalityTip.SetActive(false);

            App.my.eventSet.Subscribe(EventID.FinishLoadScene, this.OnFinishLoadScene);

            App.my.eventSet.Subscribe<PersonalityChangeValue>(EventID.Personality_ChangeValue, this.NewPureShow);       //个性
            App.my.eventSet.Subscribe<TitleUnlock>(EventID.Title_Unlock, this.NewPureShow);                             //称号
        }

        private void OnFinishLoadScene()
        {
            m_lastTriggerTime = BattleHelp.timePass;
        }

        void NewPureShow(PersonalityChangeValue show)
        {
            if (show == null) return;
            tipQue.Enqueue(new PureShowTip(PureShowType.PST_PersonalityChangeValue, show));
            TriggerHint();
        }

        void NewPureShow(TitleUnlock title)
        {
            if (title == null) return;
            tipQue.Enqueue(new PureShowTip(PureShowType.PST_UnlockTitle, title));
            TriggerHint();
        }

        public void TriggerHint()
        {
            float interval = BattleHelp.timePass - m_lastTriggerTime - m_minCutTime;
            if (interval < 0)
            {
                App.my.mainTimer.Register(-interval, 1, TriggerHint);
                return;
            }

            PureShowTip[] tips = tipQue.ToArray();
            if (tips.Length <= 0) return;
            m_lastTriggerTime = BattleHelp.timePass;

            ShowTips(tipQue.Dequeue());

            tips = tipQue.ToArray();
            if (tips.Length > 0)
                App.my.mainTimer.Register(m_minCutTime, 1, TriggerHint);
        }

        void ShowTips(PureShowTip tip)
        {
            switch (tip.m_type)
            {
                case PureShowType.PST_UnlockTitle:
                    ShowUnlockTitleTip(tip.m_title);
                    break;

                case PureShowType.PST_PersonalityChangeValue:
                    ShowPersonalityChangeValueTip(tip.m_personality);
                    break;
            }
        }

        void ShowUnlockTitleTip(TitleUnlock show)
        {
            m_unlockTitleTip.SetActive(false);
            Text title = m_unlockTitleTip.transform.Find("1/Text1").GetComponent<Text>();
            RoleTitle roleTitle = RoleTitle.Get(show.m_id);
            title.text = string.Format("<color=#{0}>{1}</color>", QualitySourceConfig.Get(roleTitle.quality).color, roleTitle.name);
            m_unlockTitleTip.SetActive(true);
        }

        void ShowPersonalityChangeValueTip(PersonalityChangeValue show)
        {
            m_personalityTip.SetActive(false);
            m_personalityTip.GetComponent<StateRoot>().CurrentState = show.m_id - 1;
            m_personalityTip.transform.Find("Text").GetComponent<Text>().text = show.m_addValue > 0 ? "+" + show.m_addValue : show.m_addValue.ToString();
            m_personalityTip.SetActive(true);
        }
    }
}
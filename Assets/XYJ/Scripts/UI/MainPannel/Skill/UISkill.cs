#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using xys.UI;
using xys.battle;
using Config;
using WXB;

namespace xys.hot.UI
{
    public class UISkill
    {
        int m_slotid;
        SkillConfig m_cfg;

        Image m_icon;   //图标
        Image m_lock;   //解锁
        Image m_forbit; //禁止施放

        Text m_testLabel;//测试表现

        UISkillManager m_skillMgr;
        ImageFillAmount m_cd;
        GameObject m_go;

        //出现特效
        PrefabsLoadReference m_switchEffect = new PrefabsLoadReference(true);
        //循环特效
        PrefabsLoadReference m_switchLoopEffect = new PrefabsLoadReference(true);


        public UISkill(Transform trans, int slot,UISkillManager skillMgr)
        {
            m_skillMgr = skillMgr;
            m_go = trans.gameObject;
            m_slotid = slot;

            if (m_slotid == (int)Slot.Skill6)
                trans = trans.Find("Offset");

            m_icon = Helper.FindComnpnent<Image>(trans, "icon");
            m_testLabel = Helper.FindComnpnent<Text>(trans, "label",false);
            m_lock = Helper.FindComnpnent<Image>(trans, "Lock", false);
            m_forbit = Helper.FindComnpnent<Image>(trans, "Disable", false);

            m_cd = trans.AddComponentIfNoExist<ImageFillAmount>();
            m_cd.Init(Helper.FindComnpnent<Text>(trans, "cd"), Helper.FindComnpnent<Image>(trans, "cdIcon"));

            //暂时屏蔽掉不用的效果
            SetGoActive(m_lock, false);
        }



        void SetGoActive<T>(T com,bool active) where T:Component
        {
            if (com != null)
                com.gameObject.SetActive(active);
        }


        public void SetSlot(SkillManager.SkillInfo info)
        {
            SkillConfig cfg = info == null ? null : info.cfg;

            //切换特效
            SetSwithEffect(cfg, m_cfg);
            m_cfg = cfg;

            SetIcon(info);
        }

        public void SetSlotState(SkillManagerLocal.UISkillState state)
        {
            SetGoActive(m_lock, false);
            SetGoActive(m_forbit, false);
            m_icon.material = null;

            //真气不足等条件
            if (state == SkillManagerLocal.UISkillState.ConditionFail)
            {
                SetGoActive(m_forbit, true);
                m_icon.material = Resources.Load("UIGray") as Material;
            }
            //状态禁止施放
            else if (state == SkillManagerLocal.UISkillState.Forbit)
            {
                SetGoActive(m_forbit, true);          
            }
            //未解锁
            else if (state == SkillManagerLocal.UISkillState.Lock)
            {
                SetGoActive(m_lock, true);
            }
        }


        // 设置图标
        void SetIcon(xys.battle.SkillManager.SkillInfo info)
        {
            bool setActive = info != null;
           
            if (info == null)
            {
                m_go.SetActive(false);
                m_cd.Stop();
            }
            else
            {
                m_go.SetActive(true);
                SkillConfig cfg = info.cfg;
                //设置图标
                xys.UI.Helper.SetSprite(m_icon, SkillIconConfig.GetIcon(cfg.id));
                m_testLabel.font = Resources.Load("msyh") as Font;
                m_testLabel.text = cfg.id.ToString();

                //设置cd
                m_cd.PlayCD(info.canPlayTime, cfg.cd);
            }
        }

        //技能切换特效
        void SetSwithEffect(SkillConfig newCfg, SkillConfig oldCfg)
        {
            string fxName = "";
            string loopFxName = "";

            //非战斗状态不播放特效
            if (m_skillMgr.GetBattleState())
            {
                //非常规技能消失获取切换到常规技能
                if (oldCfg != null && oldCfg.switchAttribute != SkillConfig.SkillSwitchAttribute.Default && (newCfg==null||newCfg.switchAttribute == SkillConfig.SkillSwitchAttribute.Default))
                {
                    fxName = "fx_ui_skill_disappear_01";
                }
                //常规切换到常规
                else if (oldCfg != null && newCfg != null && newCfg.switchAttribute == SkillConfig.SkillSwitchAttribute.Default && oldCfg.switchAttribute == SkillConfig.SkillSwitchAttribute.Default)
                {
                    fxName = "fx_ui_skill_qiehuan_01";
                }
                //切换到非常规动画
                else if (newCfg != null && newCfg.switchAttribute != SkillConfig.SkillSwitchAttribute.Default)
                {
                    //原本没有技能的时候，播发触发特效
                    if (oldCfg == null)
                        fxName = "fx_ui_skill_chufa_01";

                    //循环特效
                    if (newCfg.switchAttribute == SkillConfig.SkillSwitchAttribute.NextSkill)
                        loopFxName = "fx_ui_skill_exist_01";
                    else if (newCfg.switchAttribute == SkillConfig.SkillSwitchAttribute.SwitchSkill)
                        loopFxName = "fx_ui_skill_exist_02";
                }
            }


            //播放出现效果
            PlayEffect(m_switchEffect, fxName);
            //播放循环特效
            PlayEffect(m_switchLoopEffect, loopFxName);
        }

        //
        void PlayEffect(PrefabsLoadReference effect, string fxname)
        {
            effect.SetDestroy();
            if (string.IsNullOrEmpty(fxname))
                return;
            effect.Load(fxname, (go, para) =>
            {
                UITools.AddToChild(m_go.transform.parent.gameObject, go);
                go.transform.position = m_go.transform.position;

            });
        }
    }
}
#endif
using UnityEngine;
using System.Collections;
using xys.battle;
using UI;
using System.Collections.Generic;
using xys;
using Config;
using UnityEngine.UI;

/// <summary>
/// 准心
/// </summary>
namespace xys.battle
{
    public class AutoChooseTarget : IBattleComponent,IBattleUpdate
    {
        //是否有选择目标
        bool isChooseTarget { get { return chooseTarget != null && chooseTarget.isAlive; } }
        //检查敌人间隔
        CheckInterval m_checkAllEnemy = new CheckInterval();

        //选中目标表现
        UIChooseTarget m_uiChoose = new UIChooseTarget();

        //准心选择目标
        public IObject chooseTarget { get { return m_choosetarget; } }

        //找怪的判断开始点(目前是玩家坐标)
        Transform sourceTrans { get { return m_obj.root; } }

        Camera mainCamera { get { return App.my.cameraMgr.m_mainCamera; } }
        ObjectEventAgent m_targetEvent;
        IObject m_choosetarget;
        IObject m_obj;

        public void OnAwake(IObject obj)
        {
            m_obj = obj;
        }
        public void OnStart()
        {
        }
        public void OnDestroy()
        {
            m_obj = null;
        }
        public void OnEnterScene()
        {

        }
        public void OnExitScene()
        {
            SetChooseImpl(null);
        }

        public void OnUpdate()
        {
            if (!m_obj.isAlive || sourceTrans==null)
                return;
            //有选择目标
            if (isChooseTarget)
            {
                float distance = BattleHelp.GetAttackDistance(sourceTrans.position, chooseTarget);
                //更新ui
                m_uiChoose.SetDistanceTip(distance);

                //如果玩家当前没有仇恨目标，则可以在重新瞄准新目标
                if (m_obj.battle.m_targetMgr.target == null)
                {
                    //如果怪物距离大于30米，解除锁定
                    if (distance >= kvBattle.lockTargetMaxDistance)
                    {
                        SetChooseImpl(null);
                        return;
                    }

                    //目标夹角不对
                    if (!CheckTargetLegal(chooseTarget))
                        SetChooseImpl(null);
                }
                //仇恨目标死了
                else if (!m_obj.battle.m_targetMgr.target.isAlive)
                {
                    m_obj.battle.m_targetMgr.SetTarget(null);
                }
            }
            else
                CheckAllTarget();
        }

        //镜头选择一个目标,不修改仇恨目标
        public void SetChooseTarget(IObject target)
        {
            if (target == chooseTarget)
                return;

            //目标不能选择,设置目标为null
            if (!IsCanSelect(target))
            {
                SetChooseImpl(null);
            }
            else
            {

                SetChooseImpl(target);
            }
        }

        //按顺序记录选过的目标，最多记录10个,用来实现切换目标的时候不重复
        const int MaxSwitchCnt = 10;
        List<int> m_switchTarget = new List<int>();
        //切换一个目标,仇恨目标也会切换
        public void SwitchTarget()
        {
            //当前没有目标
            if(chooseTarget == null)
                return;

            IObject target = null;
            int orderId = int.MaxValue;
            foreach (var pair in App.my.sceneMgr.GetObjs())
            {
                IObject p = pair.Value;
                if (!IsCanSelect(p))
                    continue;
                if (chooseTarget == p)
                    continue;

                //距离合适,并且最久没被选中
                float dis = BattleHelp.GetAttackDistance(sourceTrans.position, p);
                if (CheckTargetLegal(p) && dis < kvBattle.lockTargetMaxDistance && BattleHelp.IsEnemy(m_obj, p))
                {
                    //选最久没被选中的
                    int temOrderId = -1;
                    for(int i=m_switchTarget.Count-1;i>=0;i--)
                    {
                        if(m_switchTarget[i]==p.charSceneId)
                        {
                            temOrderId = i;
                            break;
                        }
                    }
                    if (target == null || temOrderId < orderId)
                    {
                        orderId = temOrderId;
                        target = p;
                    }

                }
            }

            if (target != null)
            {
                //记录开始切换时的目标
                if (m_switchTarget.Count == 0 || m_switchTarget[m_switchTarget.Count - 1] != chooseTarget.charSceneId)
                    m_switchTarget.Add(chooseTarget.charSceneId);
                //保证上限
                if (m_switchTarget.Count > MaxSwitchCnt)
                    m_switchTarget.RemoveAt(0);
                m_switchTarget.Add(target.charSceneId);


                m_obj.battle.m_targetMgr.SetTarget(target);
            }

        }

        //目标能否选择
        bool IsCanSelect(IObject obj)
        {
            return obj != null && obj.isAlive && obj.battle.IsCanSelect() && obj != m_obj && obj.battle.actor.m_partManager.IsNormal;
        }


        void SetChooseImpl(IObject target)
        {
            if (m_choosetarget == target)
                return;
            m_choosetarget = target;

            App.my.eventSet.FireEvent<IObject>(EventID.MainPanel_SetTarget, target);

            if (target != null)
            {
                //填写小于零则不显示选中圈
                if (target.cfgInfo.selectScaleRate >= 0)
                    m_uiChoose.Show(target);
                else
                    m_uiChoose.Hide();
            }
            else
                m_uiChoose.Hide();
        }

        //开始选择目标
        void CheckAllTarget()
        {
            //避免频繁调用
            if (!m_checkAllEnemy.Check(0.2f))
                return;
            float targetDistance = float.MaxValue;
            IObject target = null;
            foreach (var pair in App.my.sceneMgr.GetObjs())
            {
                IObject p = pair.Value;
                if (!IsCanSelect(p))
                    continue;
                //找一个距离最近的
                float dis = BattleHelp.GetDistance(sourceTrans.position, p.position);
                if (dis < 0)
                    dis = 0;
                if (CheckTargetLegal(p) && dis < targetDistance && BattleHelp.IsEnemy(m_obj, p))
                {
                    target = p;
                    targetDistance = dis;
                }
            }

            if (target != null && targetDistance < kvBattle.lockTargetMaxDistance)
            {
                SetChooseImpl(target);
            }
            else
            {
                SetChooseImpl(null);
            }
        }

        //测试目标是否合法
        bool CheckTargetLegal(IObject target)
        {
            if (mainCamera == null)
                return false;

            if (IsCanSelect(target))
            {
                Vector3 pos = mainCamera.WorldToViewportPoint(target.position);
                return pos.x > 0 && pos.x < 1.0f && pos.y > 0 && pos.y < 1;
            }
            return false;
        }

    }


    class UIChooseTarget
    {
        //选中目标特效
        PrefabsLoadReference m_selectEffectObj = new PrefabsLoadReference();
        //准心资源 
        PrefabsLoadReference m_aimPointObj = new PrefabsLoadReference();
        //准心文字
        UnityEngine.UI.Text m_aimPointTxt;
        //准心动画
        Animator m_aimAnimator;


        public void Show(IObject target)
        {
            //播放选中动画
            m_aimPointObj.Show("aim_Scale_Grp", OnLoadAimPointFinsh, target);

            if (target.type == NetProto.ObjectType.Npc)
                m_selectEffectObj.Show("fx_xuanzhongnpc", OnLoadSelectEffectComplete, target);

            else
                m_selectEffectObj.Show("fx_xuanzhongmubiao", OnLoadSelectEffectComplete, target);
        }

        //隐藏选中特效
        public void Hide()
        {
            m_aimPointObj.Hide();
            m_selectEffectObj.Hide();
        }

        public void SetDistanceTip(float distance)
        {
            if (m_aimPointObj.IsLoad() && m_aimPointTxt != null)
            {
                ScalePointTxt();
                m_aimPointTxt.text = string.Format("{0}米", distance.ToString("f1"));
            }
        }

        //选择准心加载完
        void OnLoadAimPointFinsh(GameObject go, object info)
        {
            IObject target = (IObject)info;
            Camera camera = App.my.cameraMgr.m_mainCamera;

            if (m_aimPointTxt == null)
            {
                m_aimPointTxt = go.transform.Find("offset/Label").GetComponent<UnityEngine.UI.Text>();
                m_aimAnimator = go.GetComponentInChildren<Animator>();
            }

            //修正坐标
            if (target.cfgInfo.selectTargetScaleRate == 0)
            {
                go.SetActive(false);
            }
            else
            {
                go.SetActive(true);
                go.transform.localScale = Vector3.one * target.cfgInfo.selectTargetScaleRate;

                Vector3 direction = camera.transform.position - target.position;
                direction.Normalize();
                Vector3 offset = Vector3.zero; //direction * m_chooseTarget.m_modelManage.m_colDistance;
                ScalePointTxt();
                m_aimPointTxt.transform.localPosition = new Vector3(90f, 40f, 0f) + new Vector3((m_aimPointTxt.transform.localScale.x - 1) * 100 / 6, 0f, 0f);
                EffectFollowBone.AddFollow(m_aimPointObj.m_go, target.battle.actor.m_lookAtTrans, false, offset, true);

                //播放动画
                if (m_aimAnimator != null)
                    m_aimAnimator.Play("aim_start", 0, 0);
            }

        }
        //选择框加载完后需要设置大小
        void OnLoadSelectEffectComplete(GameObject go, object info)
        {
            IObject role = (IObject)info;
            if (role.isAlive)
            {
                go.transform.localScale = new Vector3(role.cfgInfo.selectScaleRate, 1, role.cfgInfo.selectScaleRate);

                EffectFollowBone.AddFollow(go, role.root, false, Vector3.zero, false);
            }
        }

        void ScalePointTxt()
        {
            Vector3 targetPos = m_aimPointTxt.transform.position;
            Camera camera = App.my.cameraMgr.m_mainCamera;
            float distance = Vector3.Distance(camera.transform.position, targetPos);
            m_aimPointTxt.transform.localScale = 150 * Vector3.one * (2 * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad)) / Screen.width;//120是调试数值
        }
    }

}

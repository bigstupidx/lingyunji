using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using xys;
using xys.battle;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

/// <summary>
/// 动作特效
/// </summary>
public partial class AnimationEffectManage : MonoBehaviour, IEffectManage
{
    List<AnimationEffectManageData.AnimationManage> m_effectDataList;

    //当前动画的特效列表
    List<BaseEffectTriggerLogic> m_curTriggerList = new List<BaseEffectTriggerLogic>();
    //记录没有清除的特效
    List<PrefabTriggerLogic> m_needDestroyFx = new List<PrefabTriggerLogic>();
    public BoneManage m_boneManage { get; protected set; }
    // 动画控制器
    AnimationWrap m_aniWrap;
    //动画长度
    float m_stLength;
    //动画名字
    public string m_stName { get; private set; }
    float m_endNormalizedTime;
    //动画帧率
    float m_stFrameRate;

    //是否本地玩家
    public bool m_isMe { get; private set; }
    //内部数据
    public TriggerDataImpl triggerDataImpl { get; private set; }

    //动画播放完后间隔一点才销毁，因为下段动画可能是相同动画后续帧数，立马销毁会导致特效也被销毁了
    float m_finishTime = 0;
    bool m_aniFinish;

    //消息管理
    //public EventDispatcher m_eventManage { get; private set; }

    public void SetEffectData(List<AnimationEffectManageData.AnimationManage> effectData)
    {
        m_effectDataList = effectData;
    }

    void Awake()
    {
        triggerDataImpl = new TriggerDataImpl();
        m_boneManage = new BoneManage(gameObject);
        //m_eventManage = new EventDispatcher();
#if SCENE_DEBUG
        m_isMe = true;
#endif
    }

    void OnDestroy()
    {
        for (int i = m_needDestroyFx.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(m_needDestroyFx[i].m_obj);
        }
        ClearPosture();
        //m_eventManage.Clear();
        m_needDestroyFx.Clear();
        m_curTriggerList.Clear();
        m_role = null;
    }

    //设置动画
    public void SetAni(AnimationWrap aniWrap, bool isMe = false)
    {
        m_aniWrap = aniWrap;
        m_isMe = isMe;
#if SCENE_DEBUG
        m_isMe = true;
#endif
    }

#if SCENE_DEBUG

    public bool IsAniFinish()
    {
        if (m_aniWrap == null)
            return false;

        return m_aniWrap.IsFinish();
    }

#endif

    public IObject m_role;


    //播放特效,需要先播放动作
    public void PlayAni(string name, float normalizedTime = 0, float endNormalizedTime = 1.0f)
    {
        if (m_aniWrap == null)
            return;

        m_aniFinish = false;
        //如果是同一段动画后续的帧数,不需要销毁
        if (m_stName == name && normalizedTime !=0)
        {
            //动作太快，要播放完没播的特效
            float timelen = m_endNormalizedTime * m_stLength-0.01f;
            if (timelen > m_stLength)
                timelen = m_stLength;
            TriggerUpdate(timelen);
            m_endNormalizedTime = endNormalizedTime;
            return;
        }

        m_endNormalizedTime = endNormalizedTime;
        FinishEffect();

        for (int i = 0; i < m_effectDataList.Count; i++)
        {
            AnimationEffectManageData.AnimationManage t = m_effectDataList[i];
            if (t.m_animationName == name)
            {
                this.enabled = true;
                m_stLength = m_aniWrap.GetLength(name);
                m_stFrameRate = m_aniWrap.GetFrameRate(name);

                int aniBeginFrame = (int)(normalizedTime * m_stLength * m_stFrameRate);
                m_stName = name;
                m_curTriggerList.Clear();
                AddTriggerList(t.m_eventList);

                for (int j = 0; j < t.m_effectList.Length; j++)
                {
                    if (t.m_effectList[j].m_beginFrame >= aniBeginFrame)
                    {
                        AddTrigger(t.m_effectList[j]);
                    }
                }

                AddTriggerList(t.m_soundList);
                break;
            }
        }
        Update();
    }

    //记录需要删除的特效
    public void SetEffectNeedDestroy(PrefabTriggerLogic go)
    {
        m_needDestroyFx.Add(go);
    }

    //该特效是否存在
    public bool IsEffectExist(string name)
    {
        for (int i = m_needDestroyFx.Count - 1; i >= 0; i--)
        {
            if (m_needDestroyFx[i].m_obj != null && m_needDestroyFx[i].m_obj.name == name)
                return true;
        }
        return false;
    }

    //根据名字删除特效
    public void DestroyEffectByName(string name)
    {
        for (int i = m_needDestroyFx.Count - 1; i >= 0; i--)
        {
            if (m_needDestroyFx[i].m_obj != null && m_needDestroyFx[i].m_obj.name == name)
            {
                GameObject.Destroy(m_needDestroyFx[i].m_obj);
                m_needDestroyFx.RemoveAt(i);
            }
        }
    }

    //技能结束需要关闭的特效
    public void FinishSkill()
    {
        for (int i = m_needDestroyFx.Count - 1; i >= 0; i--)
        {
            if (m_needDestroyFx[i].m_data.m_destroyBySkill)
            {
                GameObject.Destroy(m_needDestroyFx[i].m_obj);
                m_needDestroyFx.RemoveAt(i);
            }
        }
    }

    //结束
    void FinishEffect()
    {
        for (int i = 0; i < m_curTriggerList.Count; i++)
        {
            m_curTriggerList[i].Finish(this);
        }

        m_curTriggerList.Clear();

#if !SCENE_DEBUG
        this.enabled = false;
#endif
    }

    void AddTriggerList(BaseEffectTrigger[] list)
    {
        if (list == null)
            return;

        for (int j = 0; j < list.Length; j++)
        {
            AddTrigger(list[j]);
        }
    }

    void AddTrigger(BaseEffectTrigger p)
    {
        BaseEffectTriggerLogic logic;
        if (p is PrefabTrigger)
            logic = new PrefabTriggerLogic((PrefabTrigger)p);
        else if (p is SoundTrigger)
            logic = new SoundTriggerLogic((SoundTrigger)p);
        else
            logic = new EventTriggerLogic((EventTrigger)p);

        m_curTriggerList.Add(logic);
        logic.Play(this);
    }

    void OnDisable()
    {
        FinishEffect();
    }

    void Update()
    {
        if (m_aniWrap == null)
            return;
        //need add
        if (!m_aniWrap.IsFinish() || m_aniWrap.IsLoop())
        {
            TriggerUpdate(m_aniWrap.GetCurFrameTime());
        }
        else
        {
            if(!m_aniFinish)
            {
                m_aniFinish = true;
                m_finishTime = BattleHelp.timePass + 0.5f;
            }

            else if (BattleHelp.timePass > m_finishTime)
            {
                FinishEffect();
            }
            
        }
    }

    void TriggerUpdate(float time)
    {
        int frame;
        if(time == m_stLength)
            frame = (int)(time * m_stFrameRate);
        else
            frame = (int)((time % m_stLength) * m_stFrameRate);
        int loopCnt = (int)(time / m_stLength);
        if (time > m_stLength)
        {
            loopCnt = (int)(time / m_stLength);
        }

        for (int i = 0; i < m_curTriggerList.Count; i++)
        {
            m_curTriggerList[i].Update(this, frame, loopCnt, m_stName);
        }
    }

    //特效父节点
    private static Transform effectRoot;
    static public Transform GetEffectRoot()
    {
        if (effectRoot == null)
        {
            effectRoot = (new GameObject("[EffectRoot]")).transform;
            effectRoot.position = new Vector3(0, 0, 0);
        }
        return effectRoot;
    }

    //特效父节点
    private static Transform uiEffectRoot;
    static public Transform GetUIEffectRoot()
    {
        if (uiEffectRoot == null)
        {
            uiEffectRoot = (new GameObject("[UIEffectRoot]")).transform;
            uiEffectRoot.position = new Vector3(0, 0, 0);
        }
        return uiEffectRoot;
    }

    //内部数据
    public class TriggerDataImpl
    {
        //已经创建的对象
        public List<GameObject> m_goList = new List<GameObject>();
        //放缩
        public float m_scale = 1;
    }

}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

/// <summary>
/// 动作特效
/// </summary>
public class AnimationEffectManageData : MonoBehaviour
{
    //记录所有预制体的动作特效配置，当做是静态表,避免每个实例都存在一份，浪费内存
    static Dictionary<string, List<AnimationManage>> s_aniTriggerMap = new Dictionary<string, List<AnimationManage>>();

#if SCENE_DEBUG
    public string m_testPlayAnimation;
    public bool m_isTestLoop;
#endif
    [SerializeField]
    public List<AnimationManage> m_aniTriggers;

    //动作效果
    [System.Serializable]
    public class AnimationManage
    {
        public string m_animationName;
        public EventTrigger[] m_eventList;
        public PrefabTrigger[] m_effectList;
        public SoundTrigger[] m_soundList;
    }

    AnimationEffectManage m_effectManage;

#if UNITY_EDITOR
    public HashSet<string> GetBones()
    {
        HashSet<string> list = new HashSet<string>();
        foreach (var am in m_aniTriggers)
        {
            foreach (var pt in am.m_effectList)
                list.Add(pt.m_boneName);
        }

        return list;
    }
#endif

    void Awake()
    {
        List<AnimationManage> list;
        if (!s_aniTriggerMap.TryGetValue(gameObject.name,out list))
        {
            list = this.m_aniTriggers;
            s_aniTriggerMap.Add(gameObject.name, list);
        }
        m_effectManage = this.gameObject.AddMissingComponent<AnimationEffectManage>();
        m_effectManage.SetEffectData(list);
#if !SCENE_DEBUG
        GameObject.Destroy(this);
#endif
    }

#if SCENE_DEBUG

    
    ModelPartManage m_modelManage;
    void CheckModelPartManage()
    {
        if (m_modelManage == null)
        {
            ModelPartSetting1 setting = gameObject.GetComponent<ModelPartSetting1>();
            if (setting != null && setting.RootObject != null)
            {
                m_modelManage = setting.GetModelPartManage();
            }
        }
    }

    public string[] GetAnimList()
    {
        if (m_aniTriggers == null || m_aniTriggers.Count == 0)
            return null;

        List<string> animList = new List<string>();
        foreach (var item in m_aniTriggers)
        {
            if (string.IsNullOrEmpty(item.m_animationName))
                continue;
            animList.Add(item.m_animationName);
        }

        return animList.ToArray();
    }

    public void PlayTestAnim ()
    {
        if (!Application.isPlaying)
            return;

        CheckModelPartManage();
        if (m_modelManage != null)
        {
            m_modelManage.PlayAnim(m_testPlayAnimation);
        }
        else
        {
            AnimationWrap aniWrap = AnimationWrap.Create(this.gameObject);
            m_effectManage.SetAni(aniWrap, false);
            aniWrap.PlayAni(m_testPlayAnimation);

            m_effectManage.PlayAni(m_testPlayAnimation);
        }
    }

    void UpdateLoopAnim ()
    {
        if (!m_isTestLoop)
            return;

        if (m_effectManage!=null && m_effectManage.IsAniFinish())
        {
            PlayTestAnim();
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayTestAnim();
        }       
    }

    void LateUpdate()
    {
        if (!Application.isPlaying)
            return;

        // 处理动画循环
        UpdateLoopAnim();
    }

#endif


}


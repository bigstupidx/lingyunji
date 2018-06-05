using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTModel : RTTObject
{

    // 加载后的对象
    protected Transform m_modelObject;
    protected Animator m_animator;

    Dictionary<string, AnimationClip> m_clips = new Dictionary<string, AnimationClip>();

    public override void LoadModel(string modelName, System.Action<GameObject> loadEnd)
    {
        base.LoadModel(modelName, loadEnd);
        if (string.IsNullOrEmpty(m_objectName))
            return;
        ArtResLoad.LoadRes(m_objectName, LoadModelEnd);
    }
    void LoadModelEnd(GameObject go, object param)
    {
        SetObjectParent(go.transform);
        SetRenderActive(true);
        SetObjectLayer(go);

        m_modelObject = go.transform;
        m_animator = go.GetComponent<Animator>();

        if (m_loadModelEndEvent != null)
            m_loadModelEndEvent(go);
    }

    public override void DestroyModel()
    {
        base.DestroyModel();

        if (m_modelObject != null)
        {
            GameObject.Destroy(m_modelObject.gameObject);
            m_modelObject = null;
            m_animator = null;
        }
    }

    public override void PlayAnim(string name, float speed = 1, bool isLoop = true)
    {
        if (m_animator!=null)
        {
            m_animator.speed = speed;
            m_animator.Play(name);

#if UNITY_EDITOR
            AnimationClip clip = GetClip(name);
            if (isLoop && clip!=null && !clip.isLooping)
            {
                Debuger.LogWarning(" 模型" + m_animator.transform.parent.name + "  动画 " + name + " 没有勾选循环设置");
            }
#endif
        }
    }

    public override float GetAnimLen(string name)
    {
        AnimationClip clip = GetClip(name);
        if (clip == null)
            return 0.0f;
        else
            return clip.length;
    }

    AnimationClip GetClip(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        if (null != m_animator && m_animator.layerCount >= 1)
        {
            AnimationClip clip;
            if (m_clips.TryGetValue(name, out clip))
                return clip;

            AnimationClip[] info = m_animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].name == name)
                {
                    m_clips.Add(name, info[i]);
                    return info[i];
                }
            }
        }
        return null;
    }

    // Use this for initialization
    //void Start()
    //{

    //    SetRenderLayer();
    //    SetRenderTexture(m_uiObject);

    //    LoadModel(m_objectName);

    //    SetModelAutoRotate(true);
    //    SetModelDrag(true);
    //}

    // Update is called once per frame
    //void Update () {

    //}
}

public class RTTModelHandler
{
    RTTModel m_rttObject;

    public class Cxt
    {
        public string m_rttName;
        public string m_modelName;
        public RectTransform m_uiObject = null;
        public bool m_canDrag = false;
        public Vector3 m_pos = Vector3.zero;

        public System.Action m_loadRTTEndEvent = null;
        public System.Action<GameObject> m_loadModelEndEvent = null;
        
    }

    Cxt m_cxt = null;

    public RTTModelHandler(string rttName) : this(rttName, null, false, Vector3.zero, null) { }

    public RTTModelHandler(string rttName, RectTransform uiObject) : this(rttName, uiObject, false, Vector3.zero, null) { }

    /// <summary>
    /// 不用考虑预制体是否加载完成
    /// </summary>
    /// <param name="rttName"></param>
    /// <param name="uiObject"></param>
    /// <param name="canDrag"></param>
    /// <param name="pos"></param>
    /// <param name="loadRTTEnd"></param>
    public RTTModelHandler(string rttName, RectTransform uiObject, bool canDrag, Vector3 pos, System.Action loadRTTEnd)
    {
        m_cxt = new Cxt();

        m_cxt.m_rttName = rttName;
        m_cxt.m_uiObject = uiObject;
        m_cxt.m_canDrag = canDrag;
        m_cxt.m_pos = pos;
        m_cxt.m_loadRTTEndEvent = loadRTTEnd;

        LoadRTT();
    }

    void LoadRTT()
    {
        ArtResLoad.LoadRes(m_cxt.m_rttName, LoadRTTEnd);
    }
    void LoadRTTEnd(GameObject go, object param)
    {
        m_rttObject = go.GetComponent<RTTModel>();
        if (m_rttObject == null)
            Debuger.LogError(string.Format("RTT对象={0}，没有挂组件：RTTModel", m_cxt.m_rttName));

        m_rttObject.SetPosition(m_cxt.m_pos);
        if (m_cxt.m_uiObject != null)
            m_rttObject.Init(m_cxt.m_uiObject, m_cxt.m_canDrag);

        m_rttObject.LoadModel(m_cxt.m_modelName, m_cxt.m_loadModelEndEvent);
    }

    /// <summary>
    /// 加载或替换模型
    /// 不用考虑预制体是否加载完成
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="loadModelEnd"></param>
    public void ReplaceModel(string modelName, System.Action<GameObject> loadModelEnd)
    {
        m_cxt.m_modelName = modelName;
        m_cxt.m_loadModelEndEvent = loadModelEnd;

        if (m_rttObject != null)
        {
            m_rttObject.DestroyModel();
            m_rttObject.LoadModel(modelName, loadModelEnd);
        }
    }

    public void Destroy ()
    {
        if (m_rttObject!=null)
        {
            m_rttObject.Release();

            GameObject.Destroy(m_rttObject.gameObject);
            m_rttObject = null;
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTModelPart : RTTObject
{
    ModelPartManage m_modelManager = new ModelPartManage();

    RTTCameraScale m_camScale;

    public ModelPartManage GetModelManger()
    {
        return m_modelManager;
    }

    public void LoadModelWithAppearance(RoleDisguiseHandle disguiseHandle, Action<GameObject> loadEnd = null)
    {
        if (m_camScale != null)
            m_camScale.Reset();
        base.LoadModel(disguiseHandle.ModelName, loadEnd);
        if (string.IsNullOrEmpty(m_objectName))
            return;
        m_modelManager.LoadModelWithAppearance(disguiseHandle, LoadModelEnd);
    }

    public override void LoadModel(string modelName, Action<GameObject> loadEnd)
    {
        if (m_camScale != null)
            m_camScale.Reset();
        base.LoadModel(modelName, loadEnd);
        if (string.IsNullOrEmpty(m_objectName))
            return;
        m_modelManager.LoadModel(m_objectName, LoadModelEnd);
    }
    void LoadModelEnd(GameObject go)
    {
        SetObjectParent(go.transform);
        SetRenderActive(true);

        m_modelManager.SetRenderLayer(ComLayer.Layer_RTT);

        if (m_loadModelEndEvent != null)
            m_loadModelEndEvent(go);
    }

    public override void DestroyModel()
    {
        base.DestroyModel();
        m_modelManager.Destroy();
    }

    public override void PlayAnim(string name, float speed = 1, bool isLoop = true)
    {
        m_modelManager.PlayAnim(name, speed, 0, isLoop);
    }

    public override float GetAnimLen(string name)
    {
        return m_modelManager.GetAnimLength(name);
    }

    protected override void UdatePlayAnims()
    {
        if (!m_modelManager.IsNormal)
            return;
        base.UdatePlayAnims();
    }

    /// <summary>
    /// 设置摄像机状态，0全身，1半身，2脸部
    /// </summary>
    /// <param name="index"></param>
    public void SetCamState(int index, bool immediately)
    {
        if (m_camScale!=null)
        {
            m_camScale.SetState(index, immediately);
        }
    }

    public void ResetCamScale()
    {
        if (m_camScale!=null)
        {
            m_camScale.Reset();
        }
    }

    /// <summary>
    /// 镜头放大
    /// </summary>
    public void AddCamState()
    {
        if (m_camScale != null)
        {
            m_camScale.AddState();
        }
    }

    /// <summary>
    /// 镜头缩小
    /// </summary>
    public void ReduceCamState()
    {
        if (m_camScale!=null)
        {
            m_camScale.ReduceState();
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_camScale = this.GetComponent<RTTCameraScale>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (m_camScale!=null && !m_camScale.m_hasInitScale && m_modelManager.IsNormal)
        {
            ModelPart facePart;
            m_modelManager.GetModelPart(ModelPartType.Face, out facePart);
            if(facePart!=null)
            {
                m_camScale.SetFaceObject(m_objectRoot, facePart.gameObject.transform);
            }
            
        }
    }

}

public class RTTModelPartHandler
{
    RTTModelPart m_rttObject;

    public class Cxt
    {
        public string m_rttName;
        public RectTransform m_uiObject;
        public string m_modelName;

        // 外观相关
        public RoleDisguiseHandle m_disguiseHandle = null;

        // 其他信息
        public bool m_canDrag = false;
        public Vector3 m_rttPos = Vector3.zero;// 整体RTT位置

        // 动画相关
        public string m_idleAnim = null;
        public string[] m_relaxAnims = null;

        public System.Action m_loadRTTEndEvent = null;// 加载完Rtt对象的回调函数
        public System.Action<GameObject> m_loadModelEndEvent = null;
    }

    Cxt m_cxt = null;

    public RTTModelPartHandler(string rttName, RectTransform uiObject) : this(rttName, uiObject, "", true, Vector3.zero, null) { }

    public RTTModelPartHandler(string rttName, RectTransform uiObject, bool canDrag, Vector3 pos, System.Action loadRTTEnd) :
        this(rttName, uiObject, "", canDrag, pos, loadRTTEnd)
    { }

    /// <summary>
    /// 不用考虑预制体是否加载完成
    /// </summary>
    /// <param name="rttName"></param>
    /// <param name="uiObject"></param>
    /// <param name="modelName"></param>
    /// <param name="canDrag"></param>
    /// <param name="rttPos"></param>
    /// <param name="loadRTTEnd"></param>
    public RTTModelPartHandler(string rttName, RectTransform uiObject, string modelName, bool canDrag, Vector3 rttPos, System.Action loadRTTEnd=null)
    {
        m_cxt = new Cxt();

        m_cxt.m_rttName = rttName;
        m_cxt.m_uiObject = uiObject;
        m_cxt.m_modelName = modelName;
        m_cxt.m_canDrag = canDrag;
        m_cxt.m_rttPos = rttPos;
        m_cxt.m_loadRTTEndEvent = loadRTTEnd;

        LoadRTT();
    }

    void LoadRTT()
    {
        ArtResLoad.LoadRes(m_cxt.m_rttName, LoadRTTEnd);
    }
    void LoadRTTEnd(GameObject go, object param)
    {
        m_rttObject = go.GetComponent<RTTModelPart>();
        if (m_rttObject == null)
        {
            Debuger.LogError(string.Format("RTT对象={0}，没有挂组件：RTTModelPart", m_cxt.m_rttName));
            return;
        }

        if (m_cxt.m_uiObject != null)
            m_rttObject.Init(m_cxt.m_uiObject, m_cxt.m_canDrag);

        if (m_cxt.m_disguiseHandle == null)
            m_rttObject.LoadModel(m_cxt.m_modelName, m_cxt.m_loadModelEndEvent);
        else
            m_rttObject.LoadModelWithAppearance(m_cxt.m_disguiseHandle, m_cxt.m_loadModelEndEvent);

        m_rttObject.SetPosition(m_cxt.m_rttPos);
        m_rttObject.SetAnims(m_cxt.m_idleAnim, m_cxt.m_relaxAnims);

        if (m_cxt.m_loadRTTEndEvent != null)
            m_cxt.m_loadRTTEndEvent();
    }

    public RTTModelPart GetRTT()
    {
        return m_rttObject;
    }
    
    /// <summary>
    /// 带易容加载
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="career"></param>
    /// <param name="sex"></param>
    /// <param name="faceType"></param>
    /// <param name="overallData"></param>
    /// <param name="loadModelEnd"></param>
    public void LoadModelWithAppearence(RoleDisguiseHandle disguiseHandle, Action<GameObject> loadModelEnd = null)
    {
        m_cxt.m_disguiseHandle = disguiseHandle;
        if (m_rttObject == null)
        {
            //加载完成
            //Debuger.LogError("RTT对象未加载出来，不能执行该方法");
            return;
        }

        m_rttObject.DestroyModel();
        m_rttObject.LoadModelWithAppearance(disguiseHandle, loadModelEnd);
    }

    /// <summary>
    /// 加载或更换模型用
    /// （不用考虑预制体是否加载完成）
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="loadModelEnd"></param>
    public void ReplaceModel(string modelName, Action<GameObject> loadModelEnd=null)
    {
        m_cxt.m_modelName = modelName;
        m_cxt.m_loadModelEndEvent = loadModelEnd;

        if (m_rttObject == null) return;

        m_rttObject.DestroyModel();
        m_rttObject.LoadModel(modelName, loadModelEnd);
    }

    /// <summary>
    /// 该方法会通过角色id设置模型和动画
    /// （不用考虑预制体是否加载完成）
    /// </summary>
    /// <param name="objId"></param>
    /// <param name="loadModelEnd"></param>
    public void SetModel(int objId, Action<GameObject> loadModelEnd=null)
    {
        Config.RoleDefine config = Config.RoleDefine.Get(objId);
        if (null != config)
        {
            modelId = objId;
            ReplaceModel(config.model, loadModelEnd);
            Config.PostureConfig p = Config.PostureConfig.Get(config.posture);
            SetPlayAnims(p);
        }
    }

    public int modelId { get; private set; }

    /// <summary>
    /// 设置播放的动画
    /// </summary>
    /// <param name="p"></param>
    public void SetPlayAnims(Config.PostureConfig p)
    {
        if (p != null)
        {
            string[] relaxAnis = null;
            if (!string.IsNullOrEmpty(p.relaxIdle))
                relaxAnis = p.relaxIdle.Split('|');

            m_cxt.m_idleAnim = p.normalIdle;
            m_cxt.m_relaxAnims = relaxAnis;
            if (m_rttObject != null)
                m_rttObject.SetAnims(m_cxt.m_idleAnim, m_cxt.m_relaxAnims);
        }
    }


    public void Destroy()
    {
        if (m_rttObject != null)
        {
            m_rttObject.Release();

            GameObject.Destroy(m_rttObject.gameObject);
            m_rttObject = null;
        }
    }

    public void DestoryModel()
    {
        if (m_rttObject!=null)
        {
            m_rttObject.DestroyModel();
        }
    }

    public void SetRenderActive(bool active)
    {
        if (m_rttObject != null)
            m_rttObject.SetRenderActive(active);
    }

    public void SetCameraState(int camView, Vector3 camPos)
    {
        m_rttObject.SetCameraFov(camView);
        m_rttObject.SetCameraPositon(camPos);
    }

    public void SetCamareClipPlane(float far, float near)
    {
        m_rttObject.SetCameraClipPlane(far, near);
    }
    public void SetModelRotate(Vector3 rotate)
    {
        m_rttObject.SetObjectRotation(rotate);
    }

    public void SetRTTParent(Transform parent)
    {
        m_rttObject.transform.parent = parent;
    }

    public bool IsActive()
    {
        if (m_rttObject != null)
            return m_rttObject.IsRenderActive();
        else
            return false;
    }
}

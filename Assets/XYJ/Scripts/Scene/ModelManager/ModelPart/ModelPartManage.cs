using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityChan;

//角色模型管理,提供播放动画接口,自动播放特效
[System.Serializable]
public class ModelPartManage
{

    /// <summary>
    /// 模型状态
    /// </summary>
    public enum ModelState
    {
        none,//还没有初始化
        init,//已初始化setting对象
        loadingPart,//加载部位的模型预制体
        normal,//已经加载完
    }

    #region 成员变量

    public GameObject m_modelObject = null;// 模型对象

    public ModelPartSetting1 m_setting = null;// 中间对象组件

    Transform m_titleObject = null;// Title挂点对象
    public Transform TitleObject
    {
        get
        {
            if (m_titleObject == null)
            {
                if (m_modelObject != null)
                    return m_modelObject.transform;
                else
                    return null;
            }

            return m_titleObject;
        }
    }

    [SerializeField]
    List<ModelPart> m_modelParts = new List<ModelPart>();// 模型部件列表
    ModelPart m_rootPart = null;
    public ModelPart RootPart// 主部件
    {
        get
        {
            if (m_rootPart == null)
            {
                for (int i = 0; i < m_modelParts.Count; ++i)
                {
                    if (m_modelParts[i].m_type == ModelPartType.Body)
                    {
                        m_rootPart = m_modelParts[i];
                        break;
                    }
                }
            }
            if (m_rootPart == null)
            {
                m_rootPart = new ModelPart(ModelPartType.Body);
                m_modelParts.Add(m_rootPart);
            }

            return m_rootPart;
        }
    }

    /// <summary>
    /// 坐骑跟其他部件分开管理
    /// </summary>
    [SerializeField]
    ModelPart m_ridePart = null;
    public ModelPart RidePart// 当前使用的坐骑部件
    {
        get
        {
            if (m_ridePart == null)
            {
                m_ridePart = new ModelPart(ModelPartType.Ride);
            }

            return m_ridePart;
        }
    }
    private bool m_isUpRide = false;// 是否上了坐骑
    public bool IsUpRide
    {
        get { return m_isUpRide; }
    }
    private PlayAniCxt m_rideAniCxt = null;


    // 模型易容数据处理
    public RoleDisguiseHandle m_disguiseHandle = null;//如果为空，即没有易容数据
    // 其他外观数据处理

    // 运行时才需要
    public AnimationEffectManage m_aniEffect = null;// 动画特效管理

    // 运行时才需要
    private SpringManager m_springManager = null;// spring bone manager


    private bool m_isLocalPlayer = false;//是否本地玩家
    // 需要缓存的数据
    private string m_LoadModelName = string.Empty;//加载的模型名
    private float m_modelScale = 1.0f;//模型缩放值
    private Action<GameObject> m_loadModelEnd;//加载回调
    // anim
    private PlayAniCxt m_aniCxt = null;// 播放动画数据
    // renderer

    /// <summary>
    /// 整个模型是否激活
    /// </summary>
    public bool modelActive { get; private set; }

    // 模型状态
    [SerializeField]
    private ModelState m_modelState = ModelState.none;
    /// <summary>
    /// true表示加载完成
    /// </summary>
    public bool IsNormal
    {
        get { return (m_modelState == ModelState.normal); }
    }
    public bool IsModelLoading//是否加载状态
    {
        get
        {
            return (m_modelState == ModelState.loadingPart);
        }
    }

    #endregion

    /// <summary>
    /// 使用setting对象创建初始化ModelPartManage
    /// </summary>
    /// <param name="settingObject"></param>
    /// <returns></returns>
    public static ModelPartManage Create(GameObject settingObject)
    {
        ModelPartManage manage = new ModelPartManage();
        ModelPartSetting1 tmpSetting = settingObject.GetComponent<ModelPartSetting1>();
        if (tmpSetting == null)
        {
            manage.InitNormalModel(settingObject);
        }
        else
        {
            manage.InitSettingModel(settingObject);
        }

        return manage;
    }

    public ModelPartManage(bool islocalPlayer = false)
    {
        m_isLocalPlayer = islocalPlayer;
        ResetModelState();
    }


    #region 模型加载方法

    /// <summary>
    /// 加载模型并带外观数据
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="loadFinish"></param>
    public void LoadModelWithAppearance(RoleDisguiseHandle disguiseHandle, Action<GameObject> loadFinish)
    {
        if (IsNormal || IsModelLoading)
        {
            Destroy();
        }
        m_disguiseHandle = disguiseHandle;
        m_loadModelEnd = loadFinish;
        ArtResLoad.LoadRes(m_disguiseHandle.ModelName, OnLoadModelEnd, 1.0f, false);
    }

    /// <summary>
    /// 加载模型, 一般游戏运行调用
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="loadFinish"></param>
    /// <param name="scale"></param>
    public void LoadModel(string modelName, Action<GameObject> loadFinish, float scale = 1f)
    {
        if (IsNormal || IsModelLoading)
        {
            Destroy();
        }
        m_LoadModelName = modelName;
        m_loadModelEnd = loadFinish;
        ArtResLoad.LoadRes(modelName, OnLoadModelEnd, scale, false);
    }
    void OnLoadModelEnd(GameObject go, object param)
    {
        if (go != null)
        {
            try
            {
                GameObject tmp = GameObject.Instantiate(go);
                float modelScale = (float)param;
                // 处理普通模型
                ModelPartSetting1 tmpSetting = tmp.GetComponent<ModelPartSetting1>();
                if (tmpSetting == null)
                {
                    InitNormalModel(tmp, modelScale);
                }
                else
                {
                    InitSettingModel(tmp, modelScale);
                }
            }
            catch (System.Exception ex)
            {
                Debuger.LogException(ex);
            }
        }
    }

    /// <summary>
    /// 单纯初始话模型用
    /// </summary>
    /// <param name="normalObject"></param>
    /// <param name="modelScale"></param>
    public void InitNormalModel(GameObject normalObject, float modelScale=1.0f)
    {
        m_modelObject = normalObject;
        SetModelScale(modelScale);
        // 创建Title
        m_titleObject = m_modelObject.transform.Find("Title");
        if (m_titleObject == null)
        {
            m_titleObject = new GameObject("Title").transform;
            m_titleObject.parent = m_modelObject.transform;
            m_titleObject.localPosition = Vector3.zero;
            m_titleObject.localRotation = Quaternion.identity;
            m_titleObject.localScale = Vector3.one;
        }

        m_aniEffect = null;
        m_springManager = null;

        // 获取动画
        // 获取Renderer

        m_modelState = ModelState.normal;
        if (m_loadModelEnd != null)
            m_loadModelEnd(m_modelObject);
    }

    /// <summary>
    /// 初始化带ModelPartSetting1的对象
    /// </summary>
    /// <param name="settingObject"></param>
    /// <param name="modelScale"></param>
    public void InitSettingModel(GameObject settingObject, float modelScale=1.0f)
    {
        ResetModelState();
        ModelPartSetting1 tmpSetting = settingObject.GetComponent<ModelPartSetting1>();
        if (tmpSetting == null)
        {
            Debug.LogError("没有ModelPartSetting1组件，对象：" + settingObject.name);
            return;
        }
        m_setting = tmpSetting;
        m_modelObject = settingObject;
        m_modelState = ModelState.init;

        // 模型缩放
        if (m_disguiseHandle != null)
            SetModelScale(m_disguiseHandle.GetModelScale());
        else
            SetModelScale(modelScale);

        // 创建Title
        m_titleObject = settingObject.transform.Find("Title");
        if (m_titleObject == null)
        {
            m_titleObject = new GameObject("Title").transform;
            m_titleObject.parent = settingObject.transform;
            m_titleObject.localPosition = Vector3.zero;
            m_titleObject.localRotation = Quaternion.identity;
            m_titleObject.localScale = Vector3.one;
        }

        // 动画特效
        if (Application.isPlaying)
        {
            InitAniEffect();
        }

        // -------------------------------------
        // 初始化模型对象配置
        List<ModelPartConfig> partConfigs = m_setting.GetParts();
        if (partConfigs != null && partConfigs.Count > 0)
        {
            for (int i = 0; i < partConfigs.Count; ++i)
            {
                SetPartData(partConfigs[i]);
            }
        }

        // begin model Loading...
        LoadAllParts();
    }

    public void Destroy()
    {
        if (m_modelState == ModelState.none)
        {
            return;
        }

        // 先卸载子部位，再卸载根部位
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].Destroy();
        }
        m_modelParts.Clear();
        m_rootPart = null;

        if (m_modelObject != null)
        {
            GameObject.Destroy(m_modelObject);
            m_modelObject = null;
            m_setting = null;
        }

        ResetModelState();

        m_disguiseHandle = null;
        m_springManager = null;
        m_aniEffect = null;
    }

    /// <summary>
    /// 设置模型缩放
    /// </summary>
    /// <param name="scale"></param>
    public void SetModelScale(float scale)
    {
        if (scale <= 0.0f)
            m_modelScale = 1.0f;
        else
            m_modelScale = scale;
        if (m_modelObject != null)
        {
            m_modelObject.transform.localScale = Vector3.one * m_modelScale;
        }
    }

    /// <summary>
    /// 获取模型部件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public bool GetModelPart(ModelPartType type, out ModelPart part)
    {
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            if (type == m_modelParts[i].m_type)
            {
                part = m_modelParts[i];
                return true;
            }
        }
        Debuger.LogError("模型没有部件：" + type);
        part = null;
        return false;
    }

    /// <summary>
    /// 更换附加部位
    /// </summary>
    /// <param name="type"></param>
    public void ReplacePart(ModelPartType type, string newName, bool withLoad = true)
    {
        ModelPart part;
        if (GetModelPart(type, out part))
        {
            part.m_modelName = newName;

            // 卸载重新加载
            if (withLoad)
                LoadAllParts();
        }
    }

    /// <summary>
    /// 重新刷新整体对象，会根据易容重新修改部位信息
    /// </summary>
    public void RefreshSettingObject()
    {
        InitSettingModel(this.m_modelObject);
    }

    /// <summary>
    /// 只重新刷模型部分，部件通过外部修改，如易容修改模型部件调用
    /// </summary>
    public void RefreshModel()
    {
        LoadAllParts();
    }

    #region 坐骑相关

    const string RideSitBoneName = "sit_point";

    /// <summary>
    /// 播放坐骑动画
    /// </summary>
    /// <param name="name"></param>
    /// <param name="speed"></param>
    public void PlayRideAnim(string name, float speed=1.0f)
    {
        if (string.IsNullOrEmpty(name))
            return;
        m_rideAniCxt = new PlayAniCxt(name, speed);
        if (m_ridePart != null && m_ridePart.IsLoaded)
        {
            m_ridePart.PlayAnim(m_rideAniCxt);

            // 主角播放动画
            if (name.Contains("drive_run"))
                PlayAnim("drive_run_1");
            else
                PlayAnim("drive_idle_1");
        }
    }

    /// <summary>
    /// 上坐骑
    /// </summary>
    public void UpRide()
    {
        m_isUpRide = true;
        RootPart.m_boneName = RideSitBoneName;
        if (IsNormal)
        {
            LoadRideModel();
        }
    }

    /// <summary>
    /// 下坐骑
    /// </summary>
    public void DownRide()
    {
        m_isUpRide = false;
        RootPart.m_boneName = string.Empty;
        m_rideAniCxt = null;
        if (IsNormal)
        {
            UnLoadRideModel();
        }
    }

    /// <summary>
    /// 设置坐骑
    /// </summary>
    /// <param name="model"></param>
    public void SetRidePart(string model)
    {
        ArtResLoad.LoadRes(model, LoadRideObject, null, false);
    }
    void LoadRideObject(GameObject go, object param)
    {
        if (go!=null)
        {
            GameObject tmp = GameObject.Instantiate(go);
            string modelName;
            RuntimeAnimatorController controller;
            
            // 处理普通模型
            ModelPartSetting1 tmpSetting = tmp.GetComponent<ModelPartSetting1>();
            if (tmpSetting == null)
            {
                Animator rideAni = tmp.GetComponent<Animator>();
                controller = rideAni.runtimeAnimatorController;
                modelName = go.name;
            }
            else
            {
                controller = tmpSetting.GetPartController(ModelPartType.Body);
                modelName = tmpSetting.GetPartName(ModelPartType.Body);
            }
            
            SetRidePartData(modelName, controller);
            GameObject.Destroy(tmp);
        }
        
    }

    /// <summary>
    /// 设置坐骑信息
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="controller"></param>
    public void SetRidePartData(string modelName, RuntimeAnimatorController controller)
    {
        if (m_ridePart==null)
            m_ridePart = new ModelPart (ModelPartType.Ride);

        m_ridePart.SetConfigData(modelName, "", controller);
        if (m_isUpRide && IsNormal)
        {
            UnLoadRideModel();
            LoadRideModel();
        }
    }

    void UnLoadRideModel()
    {
        if (m_ridePart != null && m_ridePart.IsLoaded)
        {
            m_ridePart.Unload();

            if (IsNormal)
            {
                RootPart.SetParentObject(m_setting.RootObject);
                PlayAnim("idle_1");
            }
        }
    }

    void LoadRideModel()
    {
        if (m_ridePart == null)
            return;

        m_ridePart.Load(LoadRideModelEnd);
    }
    void LoadRideModelEnd(ModelPart part)
    {
        // 把角色的父节点设置在坐骑上
        RootPart.SetParentObject(part.gameObject.transform);

        // 坐骑
        part.SetParentObject(m_setting.RootObject);

        part.OverrideAniController();
        PlayRideAnim("drive_idle_1");
        
        // Anim Effect

        // SpringManager
        
    }

    #endregion

    /// <summary>
    /// 对每个部位隐藏或显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="active"></param>
    public void SetPartActive(ModelPartType type, bool active)
    {
        ModelPart part = null;
        if (GetModelPart(type, out part))
        {
            part.SetActive(active);
        }
    }

    /// <summary>
    /// 整个模型显示隐藏
    /// </summary>
    /// <param name="active"></param>
    public void SetModelActive(bool active)
    {
        this.modelActive = active;
        if (m_modelObject!=null)
        {
            m_modelObject.SetActive(active);
        }
    }

    #endregion


    #region 模型加载内部方法

    void ResetModelState()
    {
        m_modelState = ModelState.none;
        modelActive = true;

        m_LoadModelName = string.Empty;
        m_modelScale = 1.0f;

        m_aniCxt = null;
    }

    /// <summary>
    /// 删除部件数据，模型也一起删掉
    /// </summary>
    /// <param name="type"></param>
    void DestroyPartData(ModelPartType type)
    {
        ModelPart part;
        if (GetModelPart(type, out part))
        {
            part.Destroy();
            m_modelParts.Remove(part);
            if (type == ModelPartType.Body)
                m_rootPart = null;
        }
    }

    /// <summary>
    /// 添加或设置部件数据
    /// </summary>
    /// <param name="config"></param>
    void SetPartData(ModelPartConfig config)
    {
        // 获取部件
        ModelPart part = null;
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            if (m_modelParts[i].m_type == config.m_type)
            {
                part = m_modelParts[i];
                break;
            }
        }
        if (part == null)
        {
            part = new ModelPart(config.m_type);
            m_modelParts.Add(part);
        }
        if (config.m_type == ModelPartType.Body)
            m_rootPart = part;

        // 处理外观模块需要修改模型部件
        string modelName = config.m_modelName;
        if (m_disguiseHandle != null)
        {
            string tmpName = string.Empty;
            if (config.m_type == ModelPartType.Hair)
            {
                tmpName = m_disguiseHandle.GetHairModelName();
            }
            else if (config.m_type == ModelPartType.Body)
            {
                tmpName = m_disguiseHandle.GetClothModelName();
            }
            else if (config.m_type == ModelPartType.Weapon_L)
            {
                tmpName = m_disguiseHandle.GetWeaponModelName0();
            }
            else if (config.m_type == ModelPartType.Weapon_R)
            {
                tmpName = m_disguiseHandle.GetWeaponModelName1();
            }

            if (!string.IsNullOrEmpty(tmpName))
                modelName = tmpName;
        }

        part.SetConfigData(modelName, config.m_boneName, config.m_controller);
    }

    // 卸载所有部件
    void UnloadAllParts()
    {
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].Unload();
        }

        // 动画特效
        if (m_aniEffect != null)
            m_aniEffect.SetAni(null, m_isLocalPlayer);
    }

    // 加载所有部件
    void LoadAllParts()
    {
        m_modelState = ModelState.loadingPart;
        UnloadAllParts();
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].Load(OnLoadPartEnd);
        }

    }
    void OnLoadPartEnd(ModelPart part)
    {
        // 判断整体是否全部加载完成
        m_modelState = ModelState.normal;
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            if (!m_modelParts[i].IsLoaded)
            {
                m_modelState = ModelState.loadingPart;
                break;
            }
        }

        // 如果所有部件加载完处理一些事情
        if (IsNormal)
        {
            LoadModelEnd();
        }
    }

    void LoadModelEnd()
    {
        if (RootPart.gameObject==null)
        {
            if (m_loadModelEnd != null)
                m_loadModelEnd(this.m_modelObject);
            Debuger.LogError("模型没有主部位！");
            return;
        }
        // 处理模型父子节点关系，并激活显示
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            ModelPart part = m_modelParts[i];
            if (part.m_type == ModelPartType.Body)
                continue;

            part.SetParentObject(RootPart.gameObject.transform);
        }
        if (m_setting.RootObject == null)
            m_setting.CreateRootObject(this);
        RootPart.SetParentObject(m_setting.RootObject);

        // Spring Manager
        if (Application.isPlaying)
        {
            m_springManager = RootPart.gameObject.GetComponent<SpringManager>();
            if (m_springManager != null && !m_springManager.enabled)
                m_springManager = null;
            InitSpringManager(RootPart.gameObject, m_springManager);
        }

        // 设置一下特效动画
        InitAniEffect();

        // 播放默认动作
        RootPart.OverrideAniController();
        if (m_disguiseHandle != null)
        {
            m_disguiseHandle.InitModel(this);
            if (m_aniCxt == null)
                m_aniCxt = new PlayAniCxt("idle_1");
        }
        else
        {
            if (m_aniCxt == null)
                m_aniCxt = new PlayAniCxt("idle_1");
        }
        PlayAnim(m_aniCxt);

        if (m_loadModelEnd != null)
            m_loadModelEnd(this.m_modelObject);
        
    }

    void InitSpringManager(GameObject go, SpringManager sm)
    {
        if (go == null || sm == null)
            return;

        // 处理spring manager的bones
        List<SpringBone> springBones = new List<SpringBone>();
        go.GetComponentsInChildren(springBones);
        if (springBones.Count > 0)
        {
            SpringCollider[] colliders = go.GetComponentsInChildren<SpringCollider>();
            if (colliders != null && colliders.Length > 0)
            {
                for (int i = 0; i < springBones.Count; ++i)
                {
                    SpringBone sb = springBones[i];
                    sb.colliders = colliders;
                }
            }

            sm.springBones = springBones.ToArray();
        }
    }

    #endregion

    #region Renderer Methods

    /// <summary>
    /// 设置渲染动画层
    /// </summary>
    /// <param name="layer"></param>
    public void SetRenderLayer(int layer)
    {
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].SetRenderLayer(layer);
        }
    }

    /// <summary>
    /// 叠加材质
    /// </summary>
    /// <param name="mat"></param>
    public void AddMaterial(Material mat)
    {
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].AddMaterial(mat);
        }
    }

    /// <summary>
    /// 替换材质
    /// </summary>
    /// <param name="mat"></param>
    public void ChangeMaterial(Material mat)
    {
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].ChangeMaterial(mat);
        }
    }

    /// <summary>
    /// 重置材质
    /// </summary>
    public void ResetMaterial()
    {
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].ResetMaterial();
        }
    }

    public void SetRenderQueue(int queue, bool isSetCache = true)
    {
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].SetRenderQueue(queue, isSetCache);
        }
    }

    /// <summary>
    /// 设置默认颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetDefaultColor(Color color, bool isSetCache = true)
    {
        for (int i = 0; i < m_modelParts.Count; ++i)
        {
            m_modelParts[i].SetDefaultColor(color, isSetCache);
        }
    }

    /// <summary>
    /// 设置肤色
    /// </summary>
    /// <param name="color"></param>
    public void SetSkinColor(Color color)
    {
        // 皮肤包括脸和身体
        RootPart.SetDefaultColor(color);
        // 脸
        ModelPart face;
        if (GetModelPart(ModelPartType.Face, out face))
        {
            face.SetDefaultColor(color);
        }
    }

    /// <summary>
    /// 设置发色
    /// </summary>
    /// <param name="color"></param>
    public void SetHairColor(Color color)
    {
        // 头发
        ModelPart hair;
        if (GetModelPart(ModelPartType.Hair, out hair))
        {
            hair.SetHairColor(color);
        }
    }

    /// <summary>
    /// 设置服色
    /// </summary>
    /// <param name="color"></param>
    public void SetClothColor(Color color)
    {
        // 衣服颜色
        RootPart.SetClothColor(color);
    }

    #endregion

    #region 模型动画方法

    /// <summary>
    /// 播放动画，不必等模型创建即有效
    /// </summary>
    /// <param name="name"></param>
    /// <param name="speed"></param>
    /// <param name="normalizedTime"></param>
    /// <param name="isLoop"></param>
    /// <param name="crossFade"></param>
    /// <param name="layer"></param>
    /// <param name="crossFadeNormalizedTime"></param>
    public void PlayAnim(string name, float speed = 1.0f, float normalizedTime = 0, bool isLoop = false, bool crossFade = false, int layer = 0, float crossFadeNormalizedTime = -1, float endNormalizedTime = 1.0f)
    {
        if (string.IsNullOrEmpty(name))
            return;
        m_aniCxt = new PlayAniCxt(name, speed, normalizedTime, isLoop, crossFade, layer, crossFadeNormalizedTime);
        m_aniCxt.endNormalizedTime = endNormalizedTime;
        if (!IsNormal)
            return;

        PlayAnim(m_aniCxt);
    }

    public void PlayAnim(PlayAniCxt cxt)
    {
        if (cxt == null)
            return;

        if (m_aniCxt != cxt)
            m_aniCxt = cxt;
        RootPart.PlayAnim(cxt);

        // Aniamtion Effect
        if (Application.isPlaying)
        {
            if (m_aniEffect == null)
            {
                InitAniEffect();
            }

            if (m_aniEffect != null)
                m_aniEffect.PlayAni(cxt.name, cxt.normalizedTime, cxt.endNormalizedTime);
        }
    }

    public bool IsAniFinish()
    {
        if (!IsNormal)
            return true;

        if (RootPart.ani == null)
            return true;

        return RootPart.ani.IsFinish();
    }

    /// <summary>
    /// 模型的主动画控制器
    /// </summary>
    /// <returns></returns>
    public AnimationWrap GetRootAni()
    {
        return RootPart.ani;
    }

    /// <summary>
    /// 获取动画长度
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public float GetAnimLength(string name)
    {
        if (!IsNormal)
            return 0.0f;

        if (RootPart.ani == null)
            return 0.0f;

        return RootPart.ani.GetLength(name);
    }

    public int GetCurTrueAniFrame()
    {
        if (!IsNormal)
            return int.MaxValue;

        if (RootPart.ani == null)
            return int.MaxValue;

        return RootPart.ani.GetCurTrueAniFrame();
    }

    public RuntimeAnimatorController GetRootController()
    {
        return RootPart.m_controller;
    }

    /// <summary>
    /// 修改root对象控制器
    /// </summary>
    /// <param name="controller"></param>
    public void SetRootController(RuntimeAnimatorController controller)
    {
        RootPart.SetAniController(controller);
    }

    public void SetAnimCullingMode(bool isAlwaysAnim)
    {
        RootPart.SetAnimCullingMode(isAlwaysAnim);
    }

    /// <summary>
    /// 设置动画速度
    /// </summary>
    /// <param name="speed"></param>
    public void SetAnimSpeed(float speed)
    {
        RootPart.SetAnimSpeed(speed);
    }

    /// <summary>
    /// 初始动画效果
    /// </summary>
    void InitAniEffect()
    {
        m_aniEffect = m_modelObject.GetComponent<AnimationEffectManage>();
        if (m_aniEffect != null && RootPart.ani!=null)
            m_aniEffect.SetAni(RootPart.ani, m_isLocalPlayer);
    }

    #endregion

}

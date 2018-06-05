using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[System.Serializable]
public class ModelPart
{
    public enum LoadState
    {
        none,
        loading,// 加载中
        loaded,// 加载结束
    }

    // Config Data Values
    #region Config Data Values

    public ModelPartType m_type;// 部位类型

    public string m_modelName;// 部位模型名
    public string m_boneName;// 父节点骨骼名，如果没有就放在root节点下

    public RuntimeAnimatorController m_controller;// 动画的控制器

    #endregion

    // 外部回调事件
    System.Action<ModelPart> OnLoadEnd;

    // Object
    [SerializeField]
    Transform _parent;
    public Transform parentObject { get { return _parent; } private set { _parent = value; } }// 部件的父对象，如果没有就放在角色的Root对象下

    // 部件的模型对象
    public GameObject gameObject;// 模型渲染层对象
    public GameObject aniGameObject;// 模型的骨骼层对象，（从原对象分离出来的，现在只有脸部和头部需要这样处理）

    // 部件的动画
    [SerializeField]
    AnimationWrap _ani;
    public AnimationWrap ani {
        get {
            if (_ani==null && gameObject!=null)
                _ani = AnimationWrap.Create(gameObject);
            return _ani;
        }
        private set { _ani = value; }
    } // 动画组件
    public Animator animator = null;// 动画的Animator组件

    // object layer, tag ...
    public int RenderLayer { get; private set; }

    // part anim values
    public float AnimSpeed { get; private set; }
    public bool IsAlwaysAnim { get; private set; }

    // gameObject active state value
    public bool active { get; private set; }// 部件的激活状态

    // 加载状态
    // Part Load State
    [SerializeField]
    LoadState m_state = LoadState.none;
    /// <summary>
    /// 是否加载完
    /// </summary>
    public bool IsLoaded
    {
        get { return (m_state == LoadState.loaded); }
    }

    /// <summary>
    /// 是否闲置的
    /// </summary>
    public bool IsIdle
    {
        get { return (m_state == LoadState.none); }
    }

    public ModelPart(ModelPartType type)
    {
        m_type = type;

        ResetInternalData();
    }

    /// <summary>
    /// 重置内部数据
    /// </summary>
    void ResetInternalData()
    {
        active = true;
        m_state = LoadState.none;

        RenderLayer = ComLayer.Layer_ProjectorRender;
        AnimSpeed = 1.0f;
        IsAlwaysAnim = false;
    }

    /// <summary>
    /// 设置部件配置数据
    /// </summary>
    public void SetConfigData(string modelName, string boneName, RuntimeAnimatorController controller)
    {
        m_modelName = modelName;
        m_boneName = boneName;
        m_controller = controller;
    }


    // 动画相关方法
    #region Animation Methods

    /// <summary>
    /// 修改动画控制器
    /// </summary>
    /// <param name="controller"></param>
    public void SetAniController(RuntimeAnimatorController controller)
    {
        m_controller = controller;

        if (m_controller == null)
            return;
        if (animator != null)
            animator.runtimeAnimatorController = m_controller;
        else if (ani is NewAnimation)
        {
            animator = gameObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = m_controller;
        }
    }

    /// <summary>
    /// 重设runtimeAnimatorController
    /// </summary>
    public void OverrideAniController()
    {
        if (animator != null)
        {
            animator.runtimeAnimatorController = null;
            animator.runtimeAnimatorController = m_controller;
        }
    }
    
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="cxt"></param>
    public void PlayAnim(PlayAniCxt cxt)
    {
        if (ani != null)
        {
            ani.PlayAni(cxt.name, cxt.speed, cxt.normalizedTime, cxt.isLoop, cxt.crossFade, cxt.layer, cxt.crossFadeNormalizedTime);
        }
    }

    public void SetAnimSpeed(float speed)
    {
        AnimSpeed = speed;
        if (ani != null)
        {
            if (ani.GetSpeed() != speed)
            {
                ani.SetCurSpeed(speed);
            }
        }
    }

    public void SetAnimCullingMode(bool isAlwaysAnim)
    {
        IsAlwaysAnim = isAlwaysAnim;
        if (ani != null)
        {
            ani.SetCullingMode(isAlwaysAnim);
        }
    }

    #endregion


    // 添加或修改材质，修改材质颜色
    #region Renderer Methods


    // 材质相关
    // 材质字段名
    const string MatDefaultColorName = "_Color";
    const string MatHairColorName = "_EmissionColor";
    const string MatClothColorName = "_ColorStd";

    // 部位的渲染材质缓存
    Dictionary<Renderer, List<Material>> m_cachePartMats = new Dictionary<Renderer, List<Material>>();
    Dictionary<int, Color> m_cacheDefalutColors = new Dictionary<int, Color>();
    Dictionary<int, int> m_cacheRenderQueues = new Dictionary<int, int>();

    private Material m_addMat = null;
    private Material m_changeMat = null;
    // 暂时颜色和渲染队列不做缓存
    private Color m_defaultColor;
    private int m_renderQueue = 0;

    public void AddMaterial(Material mat)
    {
        m_addMat = mat;
        if (m_cachePartMats.Count == 0)
            return;

        foreach (var item in m_cachePartMats)
        {
            List<Material> matList = new List<Material>();
            matList.AddRange(item.Value);
            matList.Add(mat);
            item.Key.materials = matList.ToArray();
        }
    }

    public void ChangeMaterial(Material mat)
    {
        m_changeMat = mat;
        if (m_cachePartMats.Count == 0)
            return;

        foreach (var item in m_cachePartMats)
        {
            item.Key.material = mat;
        }
    }

    public void ResetMaterial()
    {
        m_changeMat = null;
        m_addMat = null;
        if (m_cachePartMats.Count == 0)
            return;

        foreach (var item in m_cachePartMats)
        {
            item.Key.materials = item.Value.ToArray();
        }
    }

    /// <summary>
    /// 设置部位的渲染Layer
    /// </summary>
    /// <param name="layer"></param>
    public void SetRenderLayer(int layer)
    {
        RenderLayer = layer;
        if (gameObject != null)
        {
            var renders = gameObject.GetComponentsInChildren(m_type == ModelPartType.Body ? typeof(SkinnedMeshRenderer) : typeof(Renderer), true);
            if (renders != null)
            {
                for (int i = 0; i < renders.Length; i++)
                {
                    renders[i].gameObject.layer = layer;
                }
            }
            //gameObject.layer = layer;
        }
    }

    /// <summary>
    /// 设置渲染队列
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="isSetCache"></param>
    public void SetRenderQueue(int queue, bool isSetCache = true)
    {
        foreach (var item in m_cachePartMats)
        {
            if (item.Key is SkinnedMeshRenderer)
            {
                if (isSetCache)
                {
                    foreach (var mat in item.Value)
                    {
                        mat.renderQueue = queue;
                    }
                }
                else
                {
                    item.Key.material.renderQueue = queue;
                }
            }
        }
    }

    public void SetDefaultColor(Color color, bool isSetCache = true)
    {
        SetMatColor(color, MatDefaultColorName, isSetCache);
    }

    public void SetHairColor(Color color)
    {
        SetMatColor(color, MatHairColorName);
    }

    public void SetClothColor(Color color)
    {
        SetMatColor(color, MatClothColorName);
    }

    void InitRendererMaterials()
    {
        if (gameObject != null)
        {
            Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renders)
            {
                if (r is SkinnedMeshRenderer || r is MeshRenderer)
                {
                    m_cachePartMats.Add(r, new List<Material>());
                    foreach (var m in r.materials)
                    {
                        if (m != null)
                        {
                            m_cachePartMats[r].Add(m);
                            int id = m.GetInstanceID();
                            m_cacheRenderQueues.Add(id, m.renderQueue);
                            if (m.HasProperty(MatDefaultColorName))
                                m_cacheDefalutColors.Add(id, m.GetColor(MatDefaultColorName));
                        }
                    }
                }
            }
        }

        if (m_changeMat != null)
            ChangeMaterial(m_changeMat);
        else if (m_addMat != null)
            AddMaterial(m_addMat);
    }

    void SetMatColor(Color color, string pName, bool isSetCache=true)
    {
        foreach (var item in m_cachePartMats)
        {
            if (item.Key is SkinnedMeshRenderer)
            {
                if (isSetCache)
                {
                    foreach (var mat in item.Value)
                    {
                        if (mat.HasProperty(pName))
                            mat.SetColor(pName, color);
                    }
                }
                else
                {
                    if (item.Key.material.HasProperty(pName))
                        item.Key.material.SetColor(pName, color);
                }
            }
        }
    }

    #endregion

    #region Model Methods

    /// <summary>
    /// 模型激活
    /// </summary>
    /// <param name="active"></param>
    public void SetActive(bool active)
    {
        this.active = active;
        if (gameObject != null)
        {
            gameObject.SetActive(this.active);
        }
    }

    /// <summary>
    /// 销毁部位
    /// </summary>
    public void Destroy()
    {
        Unload();

        m_modelName = string.Empty;
        m_boneName = string.Empty;
        m_controller = null;
    }

    /// <summary>
    /// 获取父节点
    /// </summary>
    /// <param name="rootObject"></param>
    public void SetParentObject(Transform rootObject)
    {
        // 获取父节点
        if (rootObject != null)
        {
            // 根据root找父节点bone对象
            if (string.IsNullOrEmpty(m_boneName))
            {
                // 没有bone，就跟root
                parentObject = rootObject;
            }
            else
            {
                // 找bone对象
                parentObject = BoneManage.GetBone(rootObject, m_boneName);
                if (parentObject == null)
                {
                    Debug.LogError(string.Format("ModelPart={0}, 没有找到bone:{1}", m_modelName, m_boneName));
                }
            }
        }
        else
        {
            parentObject = null;
            Debuger.LogError("部位设置的父节点为空，部位：" + m_type);
        }

        // 设置对象层级
        if (gameObject != null)
        {
            gameObject.transform.parent = parentObject;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;

            // 处理动画层对象的层级结构
            if (parentObject != null && (m_type == ModelPartType.Face || m_type == ModelPartType.Hair))
            {
                for (int i = 0; i < gameObject.transform.childCount; ++i)
                {
                    Transform child = gameObject.transform.GetChild(i);
                    if (child.name != "Mod")
                    {
                        child.parent = gameObject.transform.parent;
                        aniGameObject = child.gameObject;
                        break;
                    }
                }
            }
        }

        if (active && gameObject != null)
            gameObject.SetActive(true);
    }

    /// <summary>
    /// 卸载部位模型
    /// </summary>
    public void Unload()
    {
        m_state = LoadState.none;

        m_cachePartMats.Clear();
        m_cacheDefalutColors.Clear();
        m_cacheRenderQueues.Clear();

        if (gameObject != null)
        {
            if (Application.isPlaying)
                GameObject.Destroy(gameObject);
            else
                GameObject.DestroyImmediate(gameObject);
            gameObject = null;
        }

        if (aniGameObject != null)
        {
            if (Application.isPlaying)
                GameObject.Destroy(aniGameObject);
            else
                GameObject.DestroyImmediate(aniGameObject);
            aniGameObject = null;
        }

        ani = null;
        animator = null;

        ResetInternalData();
    }

    /// <summary>
    /// 预先加载部位模型资源
    /// 当所有部位加载完成，对每个部位设置父节点，动画控制器和播动画等
    /// </summary>
    /// <returns>返回是否能加载</returns>
    public bool Load(System.Action<ModelPart> loadEnd)
    {
        if (string.IsNullOrEmpty(m_modelName))
        {
            Unload();
            m_state = LoadState.loaded;
            OnLoadEnd = loadEnd;
            if (OnLoadEnd != null)
                OnLoadEnd(this);
            return false;
        }

        OnLoadEnd = loadEnd;
        m_state = LoadState.loading;
#if SCENE_DEBUG
        ArtResLoad.LoadResSync(m_modelName, OnLoadFinished, null, false);
#else
        ArtResLoad.LoadRes(m_modelName, OnLoadFinished, null, false);
#endif
        return true;
    }
    void OnLoadFinished(GameObject go, object param)
    {
        // set object data
        if (go != null)
        {
            // 如果已有模型对象，先卸载
            if (gameObject != null)
                Unload();

            // 隐藏
            GameObject tmp = GameObject.Instantiate(go);
            tmp.name = go.name;

            // 设置对象数据
            SetGameObject(tmp);

            tmp.SetActive(false);

            // Change state
            m_state = LoadState.loaded;
            if (OnLoadEnd != null)
                OnLoadEnd(this);
        }
        else
        {
            Debug.LogError("ModelPart加载对象失败，模型名=" + m_modelName);
            // 如果加载失败，如果有对象状态为加载完成
            if (gameObject == null)
                m_state = LoadState.none;
            else
                m_state = LoadState.loaded;
            // 加载部件模型失败
            if (OnLoadEnd != null)
                OnLoadEnd(null);
        }
    }

    /// <summary>
    /// 加载对象后设置对象数据
    /// </summary>
    /// <param name="go"></param>
    void SetGameObject(GameObject go)
    {
        gameObject = go;
        aniGameObject = null;

        // 设置动画数据和配置
        ani = AnimationWrap.Create(go);
        if (ani != null)
        {
            if (ani is NewAnimation && m_controller != null)
            {
                animator = go.GetComponent<Animator>();
            }

            if (AnimSpeed != ani.GetSpeed())
                ani.SetCurSpeed(AnimSpeed);
            ani.SetCullingMode(IsAlwaysAnim);
        }
        else
        {
            if (m_type == ModelPartType.Body)
                Debuger.LogError("模型没有动画:"+m_modelName);
        }

        if (Application.isPlaying)
            InitRendererMaterials();
        SetRenderLayer(RenderLayer);
    }

    #endregion

}
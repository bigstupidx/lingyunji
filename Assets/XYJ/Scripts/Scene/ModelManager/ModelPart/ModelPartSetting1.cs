

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 部位类型，每个部位不能重复
/// </summary>
public enum ModelPartType
{
    Body = 0,// 主部件Root
    Hair,
    Weapon_R,
    Weapon_L,
    Face,

    // 额外部件
    Ride,//坐骑
    Wing,//翅膀
}

/// <summary>
/// 角色部件配置,根据字符串创建对象
/// </summary>
#if SCENE_DEBUG
[ExecuteInEditMode]
#endif
public class ModelPartSetting1 : MonoBehaviour
{

    // ========================================================
    // Config Datas

    [Header("配置顺序为身体、头、右武器、左武器、脸")]
    public string[] m_bones;

    [PackTool.Pack]
    public RuntimeAnimatorController[] m_controllers;

    // 部件模型名称
    public string[] prefabsNames;

    // ========================================================

    public void CopyConfigDatas(ModelPartSetting1 source)
    {
        m_controllers = new RuntimeAnimatorController[source.m_controllers.Length];
        m_bones = new string[source.m_bones.Length];
        prefabsNames = new string[source.prefabsNames.Length];

        for (int i = 0; i < source.m_controllers.Length; i++)
            m_controllers[i] = source.m_controllers[i];

        for (int i = 0; i < source.m_bones.Length; i++)
            m_bones[i] = source.m_bones[i];

        for (int i = 0; i < source.prefabsNames.Length; i++)
            m_bones[i] = source.prefabsNames[i];
    }

    //[SerializeField]
    //Transform _rootObject = null;
    /// <summary>
    /// 模型部件的根目录对象
    /// </summary>
    public Transform RootObject = null;

    /// <summary>
    /// 获取ModelPartManage
    /// </summary>
    /// <returns></returns>
    public ModelPartManage GetModelPartManage()
    {
#if SCENE_DEBUG
        if (RootObject == null)
        {
            CreateRootObject();
        }
        ModelPartTempObject modelObject = RootObject.GetComponent<ModelPartTempObject>();
        if (modelObject != null)
            return modelObject.m_manager;
#endif
        return null;
    }

    /// <summary>
    /// 编辑模式的临时对象根目录
    /// </summary>
    static Transform _tempModelsRoot;
    public static Transform TempModelsRoot
    {
        get
        {
            if (_tempModelsRoot == null)
            {
                GameObject root = GameObject.Find("[TempModelsRoot]");
                if (root == null)
                {
                    root = new GameObject("[TempModelsRoot]");
                }
                _tempModelsRoot = root.transform;
                _tempModelsRoot.position = Vector3.zero;
            }
            return _tempModelsRoot;
        }
    }

    /// <summary>
    /// 创建模型跟目录
    /// </summary>
    public void CreateRootObject(ModelPartManage manager = null)
    {
#if SCENE_DEBUG
        if (RootObject == null)
        {
            GameObject go = new GameObject(string.Format("Root_{0}", this.gameObject.name));
            RootObject = go.transform;
            ModelPartTempObject tmp = go.AddComponent<ModelPartTempObject>();
            if (manager == null)
                tmp.m_manager.InitSettingModel(this.gameObject);
            else
                tmp.m_manager = manager;
        }
        RootObject.parent = TempModelsRoot;
#else
        if (RootObject == null)
        {
            GameObject go = new GameObject("PartRoot");
            RootObject = go.transform;
        }
        RootObject.parent = this.transform;
#endif
        ResetTransform();
    }
    void ResetTransform()
    {
        if (RootObject != null)
        {
#if SCENE_DEBUG
            RootObject.transform.position = this.transform.position;
            RootObject.transform.rotation = this.transform.rotation;
            RootObject.transform.localScale = Vector3.one;
#else
            RootObject.transform.localPosition = Vector3.zero;
            RootObject.transform.localEulerAngles = Vector3.zero;
            RootObject.transform.localScale = Vector3.one;
#endif
        }
    }

#region Public Methods

    /// <summary>
    /// 获取部位数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public ModelPartConfig GetPart(ModelPartType type)
    {
        string modelName = GetPartName(type);
        if (string.IsNullOrEmpty(modelName))
            return null;

        ModelPartConfig config = new ModelPartConfig(type);
        config.m_modelName = modelName;
        config.m_boneName = GetPartBone(type);
        config.m_controller = GetPartController(type);
        return config;
    }

    public List<ModelPartConfig> GetParts()
    {
        if (prefabsNames != null && prefabsNames.Length > 0)
        {
            List<ModelPartConfig> partConfigs = new List<ModelPartConfig>();
            for (int i = 0; i < prefabsNames.Length; ++i)
            {
                ModelPartConfig tmp = GetPart((ModelPartType)i);
                if (tmp != null)
                    partConfigs.Add(tmp);
            }
            return partConfigs;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 获取附加部位数据
    /// </summary>
    /// <returns></returns>
    public List<ModelPartConfig> GetChildParts()
    {
        if(prefabsNames!=null && prefabsNames.Length>1)
        {
            List<ModelPartConfig> childConfigs = new List<ModelPartConfig>();
            for (int i=1; i<prefabsNames.Length; ++i)
            {
                ModelPartConfig tmp = GetPart((ModelPartType)i);
                if (tmp != null)
                    childConfigs.Add(tmp);
            }
            return childConfigs;
        }
        else
        {
            return null;
        }
    }

    public string GetPartName(ModelPartType type)
    {
        if (prefabsNames == null || prefabsNames.Length == 0)
            return string.Empty;

        if (prefabsNames.Length > (int)type)
            return prefabsNames[(int)type];
        else
            return string.Empty;
    }

    public string GetPartBone(ModelPartType type)
    {
        if (m_bones.Length > (int)type)
            return m_bones[(int)type];
        else
            return string.Empty;
    }

    public RuntimeAnimatorController GetPartController(ModelPartType type)
    {
        if (m_controllers.Length > (int)type)
            return m_controllers[(int)type];
        else
            return null;
    }

    #endregion

    private void OnEnable()
    {
#if SCENE_DEBUG
        CreateRootObject();
        //if (RootObject != null)
        //{
        //    if (this.gameObject.activeSelf != RootObject.gameObject.activeSelf)
        //        RootObject.gameObject.SetActive(this.gameObject.activeSelf);
        //}
#endif
    }

    private void OnDisable()
    {
#if SCENE_DEBUG
        if (RootObject != null)
        {
            //if (this.gameObject.activeSelf != RootObject.gameObject.activeSelf)
            //    RootObject.gameObject.SetActive(this.gameObject.activeSelf);
            GameObject.DestroyImmediate(RootObject.gameObject);
        }
#endif
    }

    private void OnDestroy()
    {
#if SCENE_DEBUG
        if (RootObject != null)
        {
            GameObject.DestroyImmediate(RootObject.gameObject);
        }
#endif
    }
    // Use this for initialization
    void Start()
    {
#if SCENE_DEBUG
        if (RootObject == null)
        {
            CreateRootObject();
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if SCENE_DEBUG
        if (RootObject==null)
        {
            CreateRootObject();
        }
#endif
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetProto;
using xys;
namespace xys
{
    public class CreateCharVo
    {
        //职业id
        public int jobId = 1;
        //名字
        public string name = "";
        //性别
        public int sex;
        //配置表id
        public int roleId;
        // 外观数据
        public AppearanceData appearance;
    }
}


/// <summary>
/// 角色易容整体处理，该模块包含服饰，武饰和坐骑数据
/// </summary>
public class RoleDisguiseHandle
{
    // 1发型，2发色，3肤色，4换脸，5整体
    public enum DisguiseType : int
    {
        HairStyle = 1,
        HairColor = 2,
        SkinColor = 3,
        FacePerfab = 4,
        OverallPerfab = 5,
    }

    public RoleSkinHandle m_skinHandle;// 化妆处理
    public RoleShapeHandle m_shapeHandle;// 外形处理
    public ClothHandle m_clothHandle;//服装
    public HairHandle m_hairHandle;//发饰
    public WeaponHandle m_weaponHandle;//武饰
    public RideHandle m_rideHandle;//坐骑

    private int m_defaultClothId = 0;
    private int m_defaultWeaponId = 0;


    ModelPartManage m_modelManager;// 角色模型
    SkinnedMeshRenderer m_faceMesh;// 脸部

    ModelPartManage m_RideManager;//坐骑模型

    //public RoleDisguisePart m_overallConfig;// 整体配置
    RoleDisguiseCareer m_overallConfig
    {
        get;
        set;
    }

    RoleDisguiseOverallData m_overallData;// 易容的所有自定义数据
    RoleDisguiseOverallData m_defaultOverallData;// 根据基准脸的默认数据

    public RoleDisguiseCareer GetOverallConfig()
    {
        return m_overallConfig;
    }

    public RoleDisguiseOverallData GetOverallData()
    {
        m_shapeHandle.GetData();
        return m_overallData;
    }

    // 职业
    public int m_career = 1;
    // 性别
    public int m_sex = 0;// 0女1男

    // 角色模型名称
    int m_roleId = 0;
    string m_modelName;
    public string ModelName
    {
        get { return m_modelName; }
    }

    /// <summary>
    /// 创角的数据
    /// </summary>
    /// <returns></returns>
    public xys.CreateCharVo GetCreateData()
    {
        xys.CreateCharVo vo = new xys.CreateCharVo();// 1男2女
        vo.jobId = m_career;
        vo.sex = (m_sex == 1 ? 1 : 2);

        // 外观数据
        vo.appearance = new AppearanceData();
        vo.appearance.faceType = m_overallData.faceType;
        vo.appearance.hairStyleId = m_overallData.hairStyle;
        vo.appearance.hairColorId = m_overallData.hairColorIdx;
        vo.appearance.skinColorId = m_overallData.skinColorIdx;
        vo.appearance.faceJson = m_overallData.ToFaceJson();

        vo.appearance.clothStyleId = m_defaultClothId;
        vo.appearance.clothColorIdx = 0;
        vo.appearance.hairDressId = -1;
        vo.appearance.weaponStyleId = m_defaultWeaponId;
        vo.appearance.weaponEffectIdx = 0;
        vo.appearance.rideStyleId = -1;
        vo.appearance.rideColorIdx = 0;

        return vo;
    }

    /// <summary>
    /// 根据职业，性别和基准脸获取配置内容
    /// </summary>
    /// <param name="model"></param>
    /// <param name="career"></param>
    /// <param name="sex"></param>
    /// <param name="faceStyle"></param>
    /// <param name="resetData"></param>
    void SetRole(string model, int career, int sex, int faceStyle)
    {
        // 根据职业，性别和基准脸获取配置内容
        Config.RoleDisguiseDefault faceCfg = GetDefaultConfig(career, sex, faceStyle);
        if (faceCfg == null)
            return;

        m_modelName = model;
        m_career = career;
        m_sex = sex;

        // 服装和武饰       
        m_defaultClothId = faceCfg.clothId;
        m_defaultWeaponId = faceCfg.weaponId;
       
        if (m_defaultOverallData == null)// 默认数据值，用来做重置用，根据基准脸获取
        {
            m_defaultOverallData = new RoleDisguiseOverallData();
        }
        m_defaultOverallData.Clone(RoleDisguisePrefab.GetPrefabData(faceCfg.prefabName));

        // 玩家自定义数据
        if (m_overallData == null)
        {
            m_overallData = new RoleDisguiseOverallData();
        }
        m_overallData.Clone(m_defaultOverallData);

        m_overallData.faceType = faceStyle;// 记录一下基准脸类型

        // 获取和初始化配置
        if (m_skinHandle == null)
            m_skinHandle = new RoleSkinHandle();
        if (m_shapeHandle == null)
            m_shapeHandle = new RoleShapeHandle();

        // 化妆配置
        RoleSkinPart skinPartConfig = RoleSkinConfig.instance.Get(faceCfg.skinConfig);
        m_skinHandle.InitInfo(skinPartConfig, m_overallData.skinData, m_defaultOverallData.skinData);

        // 捏脸配置
        RoleShapeConfig shapeConfig;
        RoleShapeConfig.TryGet(faceCfg.shapeConfig, out shapeConfig);
        m_shapeHandle.InitInfo(shapeConfig, m_overallData.shapeData, m_defaultOverallData.shapeData);

        // 整体配置
        m_overallConfig = RoleDisguiseConfig.Get(career, sex, faceStyle);
    }


    public int GetOverallDataCount()
    {
        RoleDisguiseType style = m_overallConfig.GetType((int)DisguiseType.OverallPerfab);
        return style.items.Count;
    }
    public string GetOverallDataConfigByIndex(int index)
    {
        RoleDisguiseType style = m_overallConfig.GetType((int)DisguiseType.OverallPerfab);
        RoleDisguisePrefab prefab;
        if (RoleDisguisePrefab.TryGet(style.items[index].perfabName, out prefab))
        {

            return prefab.m_data.ToJson();
        }
        return null;
    }
    /// <summary>
    /// 设为默认服饰
    /// </summary>
    public void SetDefaultCloth()
    {
        m_clothHandle.m_roleClothData.m_clothId = m_defaultClothId;
        m_clothHandle.m_roleClothData.m_curColor = 0;
    }
    /// <summary>
    /// 设为默认发饰
    /// </summary>
    public void SetDefaultHairId()
    {
        m_hairHandle.m_roleHairData.m_hairDressId= -1;
    }
    /// <summary>
    /// 设为默认武器
    /// </summary>
    public void  SetDefaultWeapon()
    {
        m_weaponHandle.m_roleWeaponData.m_weaponId = m_defaultWeaponId;
        m_weaponHandle.m_roleWeaponData.m_curEffect = 0;

    }

    /// <summary>
    /// 设置角色外观
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="jobId"></param>
    /// <param name="sexId"></param>
    /// <param name="appearance"></param>
    public void SetRoleAppearance(int roleId, int jobId, int sexId, AppearanceData appearance)
    {
        // 模型
        m_roleId = roleId;
        string model = string.Empty;
        Config.RoleDefine rd = Config.RoleDefine.Get(m_roleId);
        if (rd != null)
        {
            model = rd.model;
        }

        if (appearance==null)
        {
            SetRole(model, jobId, (sexId == 1 ? 1 : 0), 0);
        }
        else
        {
            SetRole(model, jobId, (sexId == 1 ? 1 : 0), appearance.faceType);

            RoleDisguiseOverallData tmpData = new RoleDisguiseOverallData();
            tmpData.faceType = appearance.faceType;
            tmpData.hairStyle = appearance.hairDressId==-1?appearance.hairStyleId:appearance.hairDressId;
            tmpData.hairColorIdx = appearance.hairColorId;
            tmpData.skinColorIdx = appearance.skinColorId;
            tmpData.FromFaceJson(appearance.faceJson);

            m_overallData.Clone(tmpData);
            m_skinHandle.SetData(m_overallData.skinData, false);
            m_shapeHandle.SetData(m_overallData.shapeData, false);

            m_clothHandle = new ClothHandle(jobId, (sexId == 1 ? 1 : 0), appearance);
            m_hairHandle = new HairHandle(jobId, (sexId == 1 ? 1 : 0), appearance);
            m_weaponHandle = new WeaponHandle(jobId, appearance);
            m_rideHandle = new RideHandle(appearance);
        }
    }

    /// <summary>
    /// 直接设置具体的配置
    /// </summary>
    //[System.Obsolete("废弃方法，只demo编辑用，最终游戏用SetRole和InitModel替换")]
    public void InitConfig(string modelName, RoleDisguiseCareer overallCfg, RoleSkinPart skinCfg, RoleShapeConfig shapCfg)
    {
        m_modelName = modelName;
        m_overallConfig = overallCfg;

        if (m_overallData == null)// 玩家自定义数据
        {
            m_overallData = new RoleDisguiseOverallData();
        }
        if (m_defaultOverallData == null)// 默认数据值，用来做重置用，根据基准脸获取
        {
            m_defaultOverallData = new RoleDisguiseOverallData();
        }

        RoleDisguiseType style = m_overallConfig.GetType((int)DisguiseType.OverallPerfab);
        RoleDisguisePrefab prefab;
        if (RoleDisguisePrefab.TryGet(style.items[0].perfabName, out prefab))
        {
            m_defaultOverallData.Clone(prefab.m_data);// 把预设方案设成默认方案
            m_overallData.Clone(prefab.m_data);
        }

        if (m_skinHandle == null)
            m_skinHandle = new RoleSkinHandle();
        m_skinHandle.InitInfo(skinCfg, m_overallData.skinData, m_defaultOverallData.skinData);

        if (m_shapeHandle == null)
            m_shapeHandle = new RoleShapeHandle();
        m_shapeHandle.InitInfo(shapCfg, m_overallData.shapeData, m_defaultOverallData.shapeData);

        // 设置基准脸
        m_shapeHandle.SetBaseFace(m_overallConfig.faceStyle);
    }

    public void SetClothById(int id,int colorIndex)
    {
        ClothItem temp = m_clothHandle.m_clothConfig.Get(id);
        if(temp!=null)
        {
            m_modelManager.ReplacePart(ModelPartType.Body, temp.GetModName());
            if(colorIndex >= 0 && colorIndex < temp.GetColorList().Count)
            {
                m_modelManager.SetClothColor(temp.GetColorList()[colorIndex]);
            }
            
        }         
    }
    public void SetClothColor(Color color)
    {
        m_modelManager.SetClothColor(color);
    }
    public void SetClothColorByIndex(int id,int index)
    {
        ClothItem temp = m_clothHandle.m_clothConfig.Get(id);
        if(temp!=null)
        {
            if (index >= 0 && index < temp.GetColorList().Count)
            {
                m_modelManager.SetClothColor(temp.GetColorList()[index]);
            }
        } 
    }

    public string GetClothModelName()
    {
        if(m_clothHandle!=null)
        {
            int id = m_clothHandle.m_roleClothData.m_clothId;
            ClothItem temp = m_clothHandle.m_clothConfig.Get(id);
            if(temp!=null)
            return temp.GetModName();
        }
        return string.Empty;
    }

    public void SetWeaponById(int id)
    {
        WeaponItem temp = m_weaponHandle.m_weaponConfig.Get(id);
        if (temp != null)
        {
            m_modelManager.ReplacePart(ModelPartType.Weapon_L, temp.GetModNameL(1), false);
            m_modelManager.ReplacePart(ModelPartType.Weapon_R, temp.GetModNameR(1), false);
            m_modelManager.RefreshModel();
        }
    }

    public void SetWeaponModelEffect0(int level)
    {

    }

    public void SetWeaponModelEffect1(int level)
    {

    }

    public string GetWeaponModelName0()
    {
        if(m_weaponHandle!=null)
        {
            int id = m_weaponHandle.m_roleWeaponData.m_weaponId;
            WeaponItem temp = m_weaponHandle.m_weaponConfig.Get(id);
            if (temp != null)
                return temp.GetModNameL(1);
        }
        return string.Empty;
    }

    public string GetWeaponModelEffect0()
    {
        return string.Empty;
    }

    public string GetWeaponModelName1()
    {
        if (m_weaponHandle != null)
        {
            int id = m_weaponHandle.m_roleWeaponData.m_weaponId;
            WeaponItem temp = m_weaponHandle.m_weaponConfig.Get(id);
            if (temp != null)
                return temp.GetModNameR(1);
        }
        return string.Empty;
    }

    public string GetWeaponModelEffect1()
    {
        return string.Empty;
    }

    /// <summary>
    /// 修改基准脸
    /// </summary>
    /// <param name="faceStyle"></param>
    public void ChangeFaceStyle(int faceStyle)
    {
        SetRole(m_modelName, m_career, m_sex, faceStyle);

        SetOverallData(m_defaultOverallData);

        if (m_modelManager != null)
            m_modelManager.SetModelScale(GetModelScale());
    }

    /// <summary>
    /// 只是模型加载完后调用
    /// </summary>
    /// <param name="model"></param>
    /// <param name="faceMesh"></param>
    public void InitModel(ModelPartManage model, SkinnedMeshRenderer faceMesh=null)
    {
        m_modelManager = model;
        m_faceMesh = faceMesh;
        if (m_faceMesh == null && m_modelManager != null && m_modelManager.IsNormal)
        {
            // 获取脸部meshrenderer
            ModelPart part = null;
            m_modelManager.GetModelPart(ModelPartType.Face, out part);
            if (part != null && part.gameObject != null)
            {
                m_faceMesh = part.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }

        // 设置换脸
        m_skinHandle.InitModel(m_faceMesh);
        m_shapeHandle.InitModel(m_faceMesh, m_modelManager);
        
        // 设置基准脸
        m_shapeHandle.SetBaseFace(m_overallConfig.faceStyle);

        // 设置发色
        SetHairColorById(m_overallData.hairColorIdx);

        // 设置肤色
        SetSkinColorById(m_overallData.skinColorIdx);

        //设置服饰颜色
        //SetClothColorByIndex(m_clothHandle.m_roleClothData.m_clothId,m_clothHandle.m_roleClothData.m_curColor);

        //设置武器特效

    }

    /// <summary>
    /// 重置当前所有设置
    /// </summary>
    public void Reset()
    {
        SetOverallData(m_defaultOverallData);
    }

    /// <summary>
    /// 默认想类型重置
    /// </summary>
    /// <param name="type"></param>
    public void ResetType(int type)
    {
        int index = m_defaultOverallData.GetType(type);

        SetStyle(type, index);
    }

    /// <summary>
    /// 根据基准类型获取模型缩放
    /// </summary>
    /// <returns></returns>
    public float GetModelScale()
    {
        return m_shapeHandle.GetModelScale(m_overallConfig.faceStyle);
    }

    /// <summary>
    /// 获取当前发型模型名
    /// </summary>
    /// <returns></returns>
    public string GetHairModelName()
    {
        RoleDisguiseItem item = GetDisguiseItemById((int)DisguiseType.HairStyle, m_overallData.hairStyle);
        if (item == null)
            return string.Empty;
        return item.modelName;
    }

    /// <summary>
    /// 根据类型和编号获取item
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public RoleDisguiseItem GetDisguiseItemById(int type, int id)
    {
        RoleDisguiseType style = m_overallConfig.GetType(type);
        if (style == null)
            return null;

        for (int i = 0; i < style.items.Count; ++i)
        {
            if (style.items[i].itemId == id)
            {
                return style.items[i];
            }
        }
        Debuger.LogError(string.Format("易容型：{0}, 没有找到资源编号：{1}", type, id));
        return null;
    }

    /// <summary>
    /// 根据类型和编号获取item下标
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetDisguiseItemIndex(int type, int id)
    {
        RoleDisguiseType style = m_overallConfig.GetType(type);
        if (style == null)
            return -1;

        for (int i = 0; i < style.items.Count; ++i)
        {
            if (style.items[i].itemId == id)
            {
                return i;
            }
        }
        Debuger.LogError(string.Format("易容类型：{0}, 没有找到资源编号：{1}", type, id));
        return -1;
    }

    /// <summary>
    /// 1发型，2发色，3肤色，4换脸，5整体
    /// 直接用下标设置默写数据,给UI界面用
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public void SetStyle(int type, int index)
    {
        RoleDisguiseType style = m_overallConfig.GetType(type);
        if (style == null)
            return;

        if (index >= style.items.Count || style.items[index] == null)
        {
            Debuger.LogError(string.Format("易容类型：{0}, 没有下标：{1}", type, index));
            return;
        }

        if (type == (int)DisguiseType.HairStyle)
        {
            m_modelManager.ReplacePart(ModelPartType.Hair, style.items[index].modelName);
            m_overallData.hairStyle = style.items[index].itemId;// 设置数据
        }
        else if (type == (int)DisguiseType.HairColor)
        {
            m_modelManager.SetHairColor(style.items[index].color.RBGFloatColor);
            m_overallData.hairColorIdx = style.items[index].itemId;// 设置数据
        }
        else if (type == (int)DisguiseType.SkinColor)
        {
            m_modelManager.SetSkinColor(style.items[index].color.RBGFloatColor);
            m_overallData.skinColorIdx = style.items[index].itemId;// 设置数据
        }
        else if (type == (int)DisguiseType.FacePerfab)
        {
            RoleFaceDisguisePrefab prefab;
            if (RoleFaceDisguisePrefab.TryGet(style.items[index].perfabName, out prefab))
            {
                m_overallData.skinData.Clone(prefab.skinData);
                m_overallData.shapeData.Clone(prefab.shapeData);
                m_skinHandle.SetData(m_overallData.skinData);
                m_shapeHandle.SetData(m_overallData.shapeData);
            }
        }
        else if (type == (int)DisguiseType.OverallPerfab)
        {
            RoleDisguisePrefab prefab;
            if (RoleDisguisePrefab.TryGet(style.items[index].perfabName, out prefab))
            {
                m_defaultOverallData.Clone(prefab.m_data);// 把预设方案设成默认方案
                SetOverallData(prefab.m_data);
            }
        }
    }

    public void SetByPresetting(string presetting)
    {
        if(!string.IsNullOrEmpty(presetting))
        {
            RoleDisguiseOverallData data = new RoleDisguiseOverallData();
            data.FromJson(presetting);
            m_defaultOverallData.Clone(data);
            SetOverallData(data);
        }
    }


    #region Internal Methods

    public bool SetHairStyleById(int id)
    {
        RoleDisguiseItem item = GetDisguiseItemById((int)DisguiseType.HairStyle, id);
        if (item != null)
        {
            m_modelManager.ReplacePart(ModelPartType.Hair, item.modelName);
            return true;
        }
        // 发型在两个地方找id
        HairItem temp = m_hairHandle.m_hairConfig.Get(id);
     
        if (temp != null)
        {
            m_modelManager.ReplacePart(ModelPartType.Hair, temp.GetModName());
            return true;
        }

        return false;
    }

    void SetHairColorById(int id)
    {
        RoleDisguiseItem item = GetDisguiseItemById((int)DisguiseType.HairColor, id);
        if (item != null)
        {
            m_modelManager.SetHairColor(item.color.RBGFloatColor);
        }
    }

    void SetSkinColorById(int id)
    {
        RoleDisguiseItem item = GetDisguiseItemById((int)DisguiseType.SkinColor, id);
        if (item != null)
        {
            m_modelManager.SetSkinColor(item.color.RBGFloatColor);
        }
    }


    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="data"></param>
    void SetOverallData(RoleDisguiseOverallData data)
    {
        m_overallData.Clone(data);

        // 设置发型
        if (!SetHairStyleById(data.hairStyle))
        {
            // 如果发型设置失败需要手动设置

            // 设置基准脸
            m_shapeHandle.SetBaseFace(m_overallConfig.faceStyle, true);

            // 设置发色
            SetHairColorById(m_overallData.hairColorIdx);

            // 设置肤色
            SetSkinColorById(m_overallData.skinColorIdx);

            // 设置换脸
            m_skinHandle.SetData(m_overallData.skinData);
            m_shapeHandle.SetData(m_overallData.shapeData);
        }
    }


    /// <summary>
    /// 获取默认配置
    /// </summary>
    /// <param name="career"></param>
    /// <param name="sex"></param>
    /// <param name="faceStyle"></param>
    /// <returns></returns>
    Config.RoleDisguiseDefault GetDefaultConfig(int career, int sex, int faceStyle)
    {
        Dictionary<int, Config.RoleDisguiseDefault> all = Config.RoleDisguiseDefault.GetAll();
        foreach (var item in all)
        {
            if (item.Value.career == career && item.Value.sex == sex && item.Value.faceType == faceStyle)
                return item.Value;
        }

        Debuger.LogError(string.Format("RoleDisguiseHandle.GetDefaultConfig找不到对应配置，职业：{0}，性别：{1}，基准脸：{2}", career, sex, faceStyle));
        return null;
    }

    #endregion


    #region Demo 使用的接口

    public void SetEditorOverallData(RoleDisguiseOverallData data)
    {
        SetOverallData(data);
    }

    //[System.Obsolete("Demo 使用的接口,游戏内不用")]
    public void SetSelectedItem(int type)
    {
        if (type == (int)DisguiseType.HairColor)
        {
            SetHairColorById(m_overallData.hairColorIdx);
        }
        else if (type == (int)DisguiseType.SkinColor)
        {
            SetSkinColorById(m_overallData.skinColorIdx);
        }
    }

    //[System.Obsolete("Demo 使用的接口,游戏内不用")]
    public RoleDisguiseItem GetSelectedItemConfig(int type)
    {
        RoleDisguiseType style = m_overallConfig.GetType(type);
        int id = 0;
        if (type == (int)DisguiseType.HairColor)
        {
            id = m_overallData.hairColorIdx;
        }
        else if (type == (int)DisguiseType.SkinColor)
        {
            id = m_overallData.skinColorIdx;
        }
        return GetDisguiseItemById(type, id);
    }

    #endregion

}

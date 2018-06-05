using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.json;
using xys.GameStory;

/// <summary>
/// Json Config Manager
/// </summary>
public class JsonConfigMgr
{

    public static JsonDictionary<RoleShapeConfig> RoleShapeConfigs = new JsonDictionary<RoleShapeConfig>("Data/Config/Edit/Role/RoleShapeConfig/");
    public static JsonDictionary<RoleShapeDataPrefab> RoleShapeDataPerfabs = new JsonDictionary<RoleShapeDataPrefab>("Data/Config/Edit/Role/RoleShapeDataPrefab/");
    public static JsonDictionary<RoleSkinDataPrefab> RoleSkinDataPerfabs = new JsonDictionary<RoleSkinDataPrefab>("Data/Config/Edit/Role/RoleSkinDataPrefab/");

    public static JsonDictionary<RoleDisguisePrefab> RoleDisguisePrefabs = new JsonDictionary<RoleDisguisePrefab>("Data/Config/Edit/Role/RoleDisguisePrefab/");
    public static JsonDictionary<RoleFaceDisguisePrefab> RoleFaceDisguisePrefabs = new JsonDictionary<RoleFaceDisguisePrefab>("Data/Config/Edit/Role/RoleFaceDisguisePrefab/");

    public static JsonDictionary<StoryConfig> StoryConfigs = new JsonDictionary<StoryConfig>("Data/Config/Edit/GameStory/Story/");

    public static void Release()
    {
        RoleShapeConfigs.Release();
        RoleShapeDataPerfabs.Release();

        StoryConfigs.Release();
    }
}

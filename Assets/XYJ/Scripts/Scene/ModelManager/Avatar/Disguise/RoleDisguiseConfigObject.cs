using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleDisguiseConfigObject : MonoBehaviour
{
    public bool m_useThis = false;
    [Header("职业：1天剑，2墨家，3鬼谷，4七曜；性别：0女1男；部位：1发型，2发色，3肤色，4换脸，5整体")]
    public List<RoleDisguiseCareer> careers = new List<RoleDisguiseCareer>();


    void Awake()
    {
        if (m_useThis)
            RoleDisguiseConfig.Init(careers);
        else
        {
            CsvCommon.CsvLoadKey gameLoadKey = new CsvCommon.CsvLoadKey("Data/Config/Auto/Game/");
            Config.RoleDisguisePrefabs.Load(gameLoadKey);

            gameLoadKey = new CsvCommon.CsvLoadKey("Data/Config/Auto/Game/");
            Config.RoleDisguiseAsset.Load(gameLoadKey);

            RoleDisguiseConfig.Init();
        }
    }

    [ContextMenu("LoadConfig")]
    void LoadConfig()
    {
        CsvCommon.CsvLoadKey gameLoadKey = new CsvCommon.CsvLoadKey("Data/Config/Auto/Game/");
        Config.RoleDisguisePrefabs.Load(gameLoadKey);

        gameLoadKey = new CsvCommon.CsvLoadKey("Data/Config/Auto/Game/");
        Config.RoleDisguiseAsset.Load(gameLoadKey);

        RoleDisguiseConfig.Init();

        careers.Clear();
        careers.AddRange(RoleDisguiseConfig.GetAll());
    }
}

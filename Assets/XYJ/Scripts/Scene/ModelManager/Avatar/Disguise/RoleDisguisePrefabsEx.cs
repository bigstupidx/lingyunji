using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public partial class RoleDisguisePrefabs
    {
        // 加载完当前配置
        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }

        static void OnLoadAllCsv()
        {
            // 处理编号
            RoleDisguiseConfig.Init();
        }
    }
}

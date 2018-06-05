using System;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public class CsvLoadAdapter
    {
        static List<Action> s_callAfterAllLoad = new List<Action>();

#if !COM_SERVER
        static string gameCfgPath = "Data/Config/Auto/Game/";
        static string worldCfgPath = "Data/Config/Auto/World/";
#else
        static string gameCfgPath = "../ServerConfig/Game/Config/Auto/Game/";
        static string worldCfgPath = "Data/Config/Auto/World/";
#endif


        static public void All()
        {
            //重置全局数据
            s_callAfterAllLoad.Clear();
            ActionManager.Clear();

            //加载配置表
#if USE_RESOURCESEXPORT || USER_ALLRESOURCES
            CsvCommon.CsvLoadKey gameLoadKey = new PackTool.CsvLoad(gameCfgPath);
            CsvCommon.CsvLoadKey worldLoadKey = new PackTool.CsvLoad(worldCfgPath);
#else
            CsvCommon.CsvLoadKey gameLoadKey = new CsvCommon.CsvLoadKey(gameCfgPath);
            CsvCommon.CsvLoadKey worldLoadKey = new CsvCommon.CsvLoadKey(worldCfgPath);
#endif

            //优先加载的配置表
            AttributeDefine.Load(gameLoadKey);
            //自动加载的配置表
            CsvLoad.All(gameLoadKey);

#if !COM_SERVER
            WorldCsvLoad.All(worldLoadKey);
#endif

#if !COM_SERVER
            xys.App.my.hotAgent.CsvLoad(gameLoadKey);
#endif
            //所有配置表加载完以后回调
            foreach (var p in s_callAfterAllLoad)
                p();
        }

        public static void AddCallAfterAllLoad(Action call)
        {
            s_callAfterAllLoad.Add(call);
        }

    }
}

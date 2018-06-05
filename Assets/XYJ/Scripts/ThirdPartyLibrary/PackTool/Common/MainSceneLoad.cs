#if USE_RESOURCESEXPORT || USE_ABL
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    public class MainSceneLoad : ASyncOperation
    {
        public MainSceneLoad()
        {
            MagicThread.Instance.StartCoroutine(Load());
        }

        IEnumerator Load()
        {
#if RESOURCES_DEBUG
            OnceStep step = new OnceStep();
#endif

#if RESOURCES_DEBUG
            yield return step.Next("加载精灵索引!");//不导出中文
#endif

#if UNITY_IOS
			SpritesLoad.isAstc = DeviceModel.isHight;
#endif
            SpritesLoad.Init();
            FmodInit.Init();

#if RESOURCES_DEBUG
            yield return step.Next("加载主场景!");//不导出中文
#endif
            //PrefabLoad.Load("TaskTitle", null, null, true);

            //SceneLoad load = SceneLoad.Load("Level_zhucheng", null);
            //SceneLoad load = SceneLoad.Load("Level_Zhuqueta", null);
            //SceneLoad load = SceneLoad.Load("level_renwujiemian", null);
            SceneLoad load = SceneLoad.Load("main", null);
            while (!load.isDone)
            {
                progress = 0.5f * load.progress;
                yield return 0;
            }

#if USE_RESOURCESEXPORT || USE_ABL
            load.AddRef();
#endif
            progress = 1.0f;
            isDone = true;
        }
    }
}
#endif
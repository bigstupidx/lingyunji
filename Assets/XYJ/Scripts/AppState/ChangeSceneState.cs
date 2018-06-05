namespace xys
{
    using NetProto;
    using Network;
    using PackTool;
    using System.Collections;
    using UnityEngine;
    using System.Collections.Generic;

    class ChangeSceneState : AppStateBase
    {
        public ChangeSceneState() : base(AppStateType.ChangeScene)
        {
        }

        protected override void OnEnter(object p)
        {
            ChangeSceneData csd = p as ChangeSceneData;
            App.my.main.StartCoroutine(BeginLoadScene(csd));
        }

        protected override void OnLevel()
        {
            base.OnLevel();
        }

        IEnumerator BeginLoadScene(ChangeSceneData csd)
        {
            NetProto.ZoneType zt; int auid; ushort serverid; ushort mapid;
            xys.Common.Utility.Zone(csd.zoneId, out zt, out auid, out serverid, out mapid);

            bool isSeamless = IsSeamless(mapid);
            App.my.eventSet.FireEvent<bool>(EventID.BeginLoadScene, isSeamless);

            Config.LevelDefine levelDefine = Config.LevelDefine.Get(mapid);
            if (levelDefine == null)
            {
                Debuger.ErrorLog("mapid:{0} not find!", mapid);
                yield break;
            }

            var config = levelDefine.jsonConfig;
            if (config == null)
            {
                Debuger.ErrorLog("mapid:{0} logic config not find!", levelDefine.configId);
                yield break;
            }

            var logic = config.GetLogic(csd.logic);
            if (logic == null)
            {
                Debuger.ErrorLog("mapid:{0} logic:{1} config not find!", levelDefine.configId, csd.logic);
                yield break;
            }

            App.my.uiSystem.DestroyPanel("UILoginPanel");
            App.my.uiSystem.DestroyPanel("UIFaceMakePanel");

            //判断是否是定制副本
            if (isSeamless)
            {
                yield return App.my.main.StartCoroutine(SeamlessChange());
            }
            else
            {
                yield return App.my.main.StartCoroutine(NormalChange(logic));
            }
            yield return 0;

            // 场景加载成功了，可以让服务器把玩家添加到场景内了
            App.my.socket.SendGame(Protoid.C2A_ChangeScene_End);
            Level();
            App.my.eventSet.fireEvent(EventID.FinishLoadScene);
            App.my.eventSet.FireEvent<bool>(EventID.FinishLoadSceneParam, isSeamless);
            App.my.appStateMgr.Enter(AppStateType.GameIn, csd);

            BackgroundMusic.Play(levelDefine.bgMusic);
        }

        /// <summary>
        /// 判定是否是无缝切换，切换进定制副本以及从定制副本出来
        /// </summary>
        /// <returns></returns>
        bool IsSeamless(int targetLevelId)
        {
            Config.LevelDefine targetLevelDefine = Config.LevelDefine.Get(targetLevelId);
            int curLevelId = App.my.localPlayer.GetModule<LevelModule>().levelId;
            Config.LevelDefine curLevelDefine = Config.LevelDefine.Get(curLevelId);

            if (null != targetLevelDefine && null != curLevelDefine &&
                targetLevelDefine.jsonConfig.StartLogic.m_scene == curLevelDefine.jsonConfig.StartLogic.m_scene)
            {
                if (targetLevelDefine.levelType == Config.LevelType.StroyDuplicate)
                {
                    //目标关卡是定制副本
                    return true;
                }
                if (targetLevelDefine.levelType == Config.LevelType.MainCity && curLevelDefine.levelType == Config.LevelType.StroyDuplicate)
                {
                    //从定制副本返回主城
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 普通切换
        /// </summary>
        /// <returns></returns>
        IEnumerator NormalChange(LevelDesignConfig.LevelLogicData logic)
        {
            BackgroundMusic.Release();
            var loadingMgr = App.my.uiSystem.loadingMgr;
            loadingMgr.Show();

            SceneLoad sl = SceneLoad.Load(logic.m_scene, null);
            while (!sl.isDone)
            {
                loadingMgr.progress = sl.progress;
                yield return 0;
            }
        }

        /// <summary>
        /// 无缝切换
        /// </summary>
        /// <returns></returns>
        IEnumerator SeamlessChange()
        {
            //加载无缝切换的特效
            RALoad.LoadPrefab("fx_cam_effect_jinrujuqing", null, null);
            yield return new WaitForSeconds(1.5f);
            PackTool.AssetsUnLoad.UnloadUnusedAssets(false);
            while (!PackTool.AssetsUnLoad.isDone)
                yield return 0;
        }
    }
}
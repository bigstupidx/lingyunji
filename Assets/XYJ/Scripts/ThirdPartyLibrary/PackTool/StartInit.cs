#if USE_RESOURCESEXPORT || USE_ABL
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using PackTool;
using UnityEngine;
using System.Collections;

public class StartInit : MonoBehaviour
{
    //List<string> Scenes = new List<string>();
#if !UNITY_EDITOR
    private void LateUpdate()
    {
        if (xys.App.my == null)
        {
            XTools.TimerMgrObj.Instance.LateUpdate();
        }
    }

    private void Update()
    {
        if (xys.App.my == null)
        {
            XTools.TimerMgrObj.Instance.Update();
        }
    }
#endif

    IEnumerator Start ()
    {
#if ASSET_DEBUG
        TimeTrackMgr.CreateInstance();
        TimeTrack.get_file_size = ResourcesPack.FindFileSize;
#endif
        Logger.CreateInstance();
        ConsoleSelf.CreateInstance();
        ConsoleSelf.SetEnabled(false);
#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidResolution.SetWindows();
        yield return new WaitForSeconds(0.2f);
#endif

#if USER_UWASDK
        GameObject go = new GameObject();
        go.SetActive(false);
        UWA.GUIWrapper wrapper = go.AddComponent<UWA.GUIWrapper>();
        go.SetActive(true);
#endif

#if UNITY_EDITOR
        MonoQuit.CreateInstance();
#endif
        DontDestroyOnLoad(gameObject);
        //Layer.Init();
        GUITextShow.CreateInstance();
        XTools.TimerMgrObj.CreateInstance();
        MagicThread.CreateInstance();
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if COM_DEBUG
        string extended_path = ProgramConfig.Default.inside_patchList_patch;
        if (!string.IsNullOrEmpty(extended_path))
        {
            extended_path = extended_path.Substring(0, extended_path.LastIndexOf('/'));
            extended_path = string.Format("{0}/{1}", extended_path, XTools.AssetZip.extendedname);

            string down_file = ResourcesPath.LocalTempPath + XTools.AssetZip.extendedname;
            if (System.IO.File.Exists(down_file))
                System.IO.File.Delete(down_file);
            HttpDown http = HttpDown.Down(extended_path, down_file, 0.5f);
            while (!http.isDone)
                yield return 0;
        }
#endif

        //ResourcesPack.InitExterPath(ResourcesPath.LocalDataPath);
        {
            yield return ResourcesPack.InitUnite();
            //FileFirstLoad ffl = new FileFirstLoad();
            //GUITextShow.TextInfo info = GUITextShow.AddText("解压资源中");
            //while (!ffl.isDone)
            //{
            //    info.text = string.Format("正在解压资源，当前进度:{0} {1}/{2} 用时:{3}秒", ffl.progress.ToString("0.00"), ffl.current, ffl.total, ffl.totaltime.ToString("0.0"));
            //    yield return 0;
            //}
            //info.isCannel = true;
        }

        //yield return StartCoroutine((new ResourcesCheck()).Begin());

        ResourcesCheck rc = new ResourcesCheck();
        while (!rc.isDone)
            yield return 0;

        Logger.LogDebug("ProgramConfig:{0} resources:{1}", ProgramConfig.Default.svn, ResourcesPack.Current.config.svn);

#if RESOURCES_DEBUG
        OnceStep step = new OnceStep();
        yield return step.Next("初始化基础资源!");
#endif

#if USE_ABL
        ABL.Dependencies.Init();
#else
        BuiltinResource.Init();
        while (!BuiltinResource.isDone)
            yield return 0;
#endif
        MainSceneLoad msl = new MainSceneLoad();
        while (!msl.isDone)
            yield return 0;

#if UNITY_EDITOR
        MonoQuit.AddQuit(() => { ResourcesPack.Release(); });
#endif
        Destroy(gameObject);
    }
}
#endif
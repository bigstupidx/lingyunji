#if USE_RESOURCESEXPORT
using PackTool;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourcesCheck : ASyncOperation
{
    public ResourcesCheck()
    {
        MagicThread.Instance.StartCoroutine(CheckUpdate());
    }

    Dictionary<string, GUITextShow.TextInfo> Dic = new Dictionary<string, GUITextShow.TextInfo>();

    GUITextShow.TextInfo GetByKey(string key)
    {
        GUITextShow.TextInfo info;
        if (Dic.TryGetValue(key, out info))
            return info;

        info = guiList.AddText("");
        Dic.Add(key, info);
        return info;
    }
    GUITextShowList guiList = new GUITextShowList();

    void OnShowGUI(VersionUpdate vu)
    {
        switch (vu.CurrentStep)
        {
        case VersionUpdate.Step.Null:
            {
                GUITextShow.TextInfo info = GetByKey("normal");
                info.text = "正在处理当中...";
            }
            break;
        case VersionUpdate.Step.DownPathlisting:
            {
                GUITextShow.TextInfo info = GetByKey("DownPathlisting");
                info.text = string.Format("正在下载更新列表!{0}%", (vu.progress * 100f).ToString("0.00"));
            }
            break;
        case VersionUpdate.Step.DownPrograming:
            {
                GUITextShow.TextInfo info = GetByKey("DownPrograming");
                info.text = string.Format("正在下载安装包列表!{0}%", (vu.progress * 100f).ToString("0.00"));
            }
            break;
        case VersionUpdate.Step.ShowProgramUrl:
            {
                GUITextShow.TextInfo info = GetByKey("ShowProgramUrl");
                if (info.click == null)
                {
                    info.isbutton = true;
                    info.click = (object p) => { Application.OpenURL((string)p); };
                    info.clickp = vu.program_down_url;
                }

                info.text = "点击开始安装程序包!";
            }
            break;

        case VersionUpdate.Step.DownPatching:
            {
                GUITextShow.TextInfo info = GetByKey("DownPatching");
                if (string.IsNullOrEmpty(info.text))
                {
                    info.isbutton = true;
                    info.text = "是否下载补丁?大小:" + XTools.Utility.ToMb(vu.downsize);
                    info.click = (object p) => 
                    {
                        info.isbutton = false;
                        vu.isContinue = true;
                        info.num = int.MaxValue;
                        info.text = string.Format("正在下载补丁包中!{0}%", (vu.progress * 100f).ToString("0.00"));
                    };
                }

                if (vu.isContinue)
                {
                    info.text = string.Format("正在下载补丁包中!{0}/{1}", XTools.Utility.ToMb(vu.currentsize), XTools.Utility.ToMb(vu.downsize));
                }
            }
            break;
        case VersionUpdate.Step.InstallPatching:
            {
                GUITextShow.TextInfo info = GetByKey("InstallPatching");
                info.text = string.Format("正在安装补丁包中!{0}%", (vu.progress * 100f).ToString("0.00"));
            }
            break;
        }
    }

    IEnumerator CheckUpdate()
    {
        while (!ResourcesPack.isInitEnd)
        {
            yield return 0;
        }

        if (!ResourcesPack.IsVaild)
        {
            ResourcesPack.InitEmpty();
        }

        // 网络环境检测
        NetDownloadCheck ndc = new NetDownloadCheck();
        MagicThread.Instance.StartCoroutine(ndc.BeginCheckGUI(guiList));
        while (!ndc.isDone)
            yield return 0;
        guiList.Release();

        // 开启更新版本的任务
        DTATask updateTask = new DTATask(
            "版本检测",
            () =>
            {
                return new VersionUpdate(
                    ProgramConfig.Default.inside_patchList_program,
                    ProgramConfig.Default.inside_patchList_patch,
                    ProgramConfig.Default.client_channel);
            },
            (ASyncOperation task) => { OnShowGUI((VersionUpdate)task); },
            null);

        MagicThread.StartForeground(updateTask.Begin());
        while (!updateTask.isDone)
            yield return 0;

        guiList.Release();
        isDone = true;
        Debug.Log("更新完成!");
    }
}
#endif
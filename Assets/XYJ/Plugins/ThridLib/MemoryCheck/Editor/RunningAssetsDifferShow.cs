#if MEMORY_CHECK
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using XTools;

public class RunningAssetsDifferShow
{
    ParamList mParamList = new ParamList();

    public void Release()
    {
        mParamList.ReleaseAll();
    }

    public EditorPageBtn mEditorPageBtn = new EditorPageBtn();
    public RunningAssetsDiffer mRunningAssetsDiffer = new RunningAssetsDiffer();

    enum TagType
    {
        //
        增加的,
        减少的,
        一样的
    }

    TagType mTagType = TagType.增加的;

    public void OnGUI()
    {
        mTagType = (TagType)EditorGUILayout.EnumPopup("类型", mTagType);
        if (mRunningAssetsDiffer == null || mRunningAssetsDiffer.current == null || mRunningAssetsDiffer.before == null)
            return;

        switch (mTagType)
        {
        case TagType.增加的:
            {
                GUILayout.Label("增加的资源:" + mRunningAssetsDiffer.addInfo.Text);
                var add = mParamList.Get<RunningAssetsShow>("addinfo"); add.mInfo = mRunningAssetsDiffer.addInfo; add.OnGUI();
            }
            break;
        case TagType.减少的:
            {
                GUILayout.Label("减少的资源:" + mRunningAssetsDiffer.subInfo.Text);
                var add = mParamList.Get<RunningAssetsShow>("subInfo"); add.mInfo = mRunningAssetsDiffer.subInfo; add.OnGUI();
            }
            break;
        case TagType.一样的:
            {
                GUILayout.Label("一样的资源:" + mRunningAssetsDiffer.sameInfo.Text);
                var add = mParamList.Get<RunningAssetsShow>("sameInfo"); add.mInfo = mRunningAssetsDiffer.sameInfo; add.OnGUI();
            }
            break;
        }
    }
}
#endif
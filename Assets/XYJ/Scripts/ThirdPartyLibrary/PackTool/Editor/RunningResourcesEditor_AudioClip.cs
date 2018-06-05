#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PackTool;
using GUIEditor;
using UnityEngine.UI;

partial class RunningResourcesEditor : BaseEditorWindow
{
    class AudioClipShow
    {
        string search_key = "";

        Vector2 ScrollPosition = Vector2.zero;

        public void Show(ParamList paramList)
        {
            search_key = EditorGUILayout.TextField("搜索key", search_key);

            // 查看下纹理的使用情况
            List<AudioClipLoad> audios = new List<AudioClipLoad>();
            {
                foreach (var itor in AudioClipLoad.GetAllList())
                {
                    if (!string.IsNullOrEmpty(search_key) && !itor.Key.Contains(search_key))
                        continue;

                    audios.Add(itor.Value as AudioClipLoad);
                }
            }

            EditorGUILayout.LabelField("音频");//不导出中文
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            GuiTools.ObjectFieldList<AudioClipLoad>(
                paramList,
                audios,
                (AudioClipLoad pl) => { return pl.asset; },
                false,
                null,
                (AudioClipLoad pl) =>
                {

                },
                (AudioClipLoad pl) =>
                {
                    //ParamList ppp = paramList.Get<ParamList>(pl.url);
                });
            EditorGUILayout.EndScrollView();
        }
    }

    AudioClipShow mAudioClipShow = new AudioClipShow();

    void ShowAudioClip()
    {
        mAudioClipShow.Show(mParamList.Get("AudioClip", ()=> { return new ParamList(); }));
    }
}
#endif
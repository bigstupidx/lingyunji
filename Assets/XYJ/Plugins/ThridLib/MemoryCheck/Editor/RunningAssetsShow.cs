#if MEMORY_CHECK
using PackTool;
using GUIEditor;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class RunningAssetsShow
{
    public AssetSyncInfo mInfo;
    string showType = "";
    ParamList mParamList = new ParamList();

    class AssetsShow<T>
    {
        string search_key = "";

        Vector2 ScrollPosition = Vector2.zero;

        public List<T> assets = null;

        public System.Func<T, Object> switchfun;
        public System.Func<T, string> infoText;

        public System.Action<List<T>, ParamList> beginList = null;

        protected Object DefaultSwitch(T o)
        {
            string path = o.ToString();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/" + path);
            if (obj == null)
                obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/__copy__/" + path);

            return obj;
        }

        List<T> Temps = new List<T>();

        public void Show(ParamList paramList, string text, List<T> lists)
        {
            if (switchfun == null)
            {
                switchfun = DefaultSwitch;
            }

            search_key = EditorGUILayout.TextField("搜索key", search_key);
            Temps.Clear();
            if (string.IsNullOrEmpty(search_key))
                Temps.AddRange(lists);
            else
            {
                for (int i = 0; i < lists.Count; ++i)
                {
                    string key = lists[i].ToString();
                    if (key.ToLower().Contains(search_key.ToLower()))
                    {
                        Temps.Add(lists[i]);
                    }
                }
            }

            EditorGUILayout.LabelField(text);
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

            GuiTools.ObjectFieldList<T>(
                paramList,
                Temps,
                switchfun,
                0,
                beginList,
                (T pl) =>
                {
                    Object obj = switchfun(pl);
                    if (obj != null && infoText != null)
                    {
                        EditorGUILayout.LabelField(infoText(pl));
                        return;
                    }

                    EditorGUILayout.LabelField(string.Format("index:{0} key:{1}", Temps.IndexOf(pl), pl.ToString()));
                },
                (T pl) =>
                {

                });

            EditorGUILayout.EndScrollView();
        }
    }

    AssetsShow<string> mTextures = new AssetsShow<string>();
    AssetsShow<string> mMeshs = new AssetsShow<string>();
    AssetsShow<string> mControls = new AssetsShow<string>();
    AssetsShow<string> mAudioClips = new AssetsShow<string>();
    AssetsShow<string> mAvatars = new AssetsShow<string>();
    AssetsShow<string> mMaterials = new AssetsShow<string>();
    PrefabShow mPrefabs = new PrefabShow();

    public void OnGUI()
    {
        if (mInfo == null)
            return;

        string[] keys = new string[] { "预置体", "贴图", "材质", "图集", "字体", "动画控制器", "网格", "音频", "字库", "Avatar" };
        showType = GuiTools.StringPopup(0, "资源类型", showType, new List<string>(keys));

        switch (showType)
        {
        case "预置体":
            {
                mPrefabs.Show(mParamList.Get<ParamList>("Prefab"), "贴图", mInfo.mPrefabs);
            }
            break;
        case "贴图":
            mTextures.Show(mParamList.Get<ParamList>("Texture"), "贴图", mInfo.textures);
            break;
        case "网格":
            mMeshs.Show(mParamList.Get<ParamList>("Mesh"), "网格", mInfo.meshs);
            break;
        case "动画控制器":
            mControls.Show(mParamList.Get<ParamList>("RuntimeAnimatorController"), "动画控制器", mInfo.controls);
            break;
        case "音频":
            mAudioClips.Show(mParamList.Get<ParamList>("AudioClip"), "音频", mInfo.audioclips);
            break;
        case "Avatar":
            mAvatars.Show(mParamList.Get<ParamList>("Avatar"), "Avatar", mInfo.avatars);
            break;
        case "材质":
            mMaterials.Show(mParamList.Get<ParamList>("Material"), "材质", mInfo.materials);
            break;
        }
    }
}
#endif
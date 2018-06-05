using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PackTool;

namespace PackTool
{
    // 显示音频信息
    public class HelpShowEditor
    {
        public delegate void OnFindRes<T>(string path, List<T> list, List<string> paths);

        public class AssetList<T> where T : Object
        {
            public List<T> mList;
            public List<string> mPaths;
            public Vector2 mPosition = Vector2.zero;
            public bool mIsPack = false;
            public bool mIsShow = false; // 是否显示

            public List<T> GetAssetList()
            {
                return mList;
            }
        }

        // 显示资源信息
        public static void ShowResourcesInfo<T>(string path, OnFindRes<T> fun, string resName, bool bPackAll, AssetList<T> asset) where T : Object
        {
            asset.mIsPack = false;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打包" + resName, GUILayout.Width(100), GUILayout.Height(100)) || bPackAll == true)
            {
                asset.mList = new List<T>();
                asset.mPaths = new List<string>();
                asset.mIsPack = true;
                fun(path, asset.mList, asset.mPaths);
            }

            if (!asset.mIsShow)
            {
                if (GUILayout.Button("显示" + resName + "信息", GUILayout.Width(100), GUILayout.Height(100)))
                {
                    if (asset.mList == null)
                    {
                        asset.mList = new List<T>();
                    }
                    if (asset.mPaths == null)
                        asset.mPaths = new List<string>();

                    asset.mList.Clear();
                    asset.mPaths.Clear();
                    fun(path, asset.mList, asset.mPaths);

                    asset.mIsShow = true;
                }
            }
            else
            {
                if (GUILayout.Button("隐藏" + resName + "信息", GUILayout.Width(100), GUILayout.Height(100)))
                {
                    asset.mIsShow = false;
                    asset.mList = null;
                }
            }

            if (asset.mIsShow && asset.mList != null)
            {
                GUILayout.Label("要打包的" + resName + "个数:" + asset.mList.Count);
                asset.mPosition = GUILayout.BeginScrollView(asset.mPosition, GUILayout.MaxHeight(300f));
                foreach (T obj in asset.mList)
                {
                    EditorGUILayout.ObjectField(obj, typeof(T), true);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }
    }
}
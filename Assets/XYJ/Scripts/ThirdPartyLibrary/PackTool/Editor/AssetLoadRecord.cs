#if UNITY_EDITOR && ASSET_DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PackTool;
using GUIEditor;

class AssetLoadRecord : BaseEditorWindow
{
    [MenuItem("PackTool/资源加载查看", false, 9)]
    [MenuItem("Assets/PackTool/资源加载查看", false, 0)]
    static public void OpenRunningResourcesEditor()
    {
        TimeTrackMgr.CreateInstance();
        EditorWindow.GetWindow<AssetLoadRecord>(false, "AssetLoadRecord", true);
    }

    // 排序
    enum SortType
    {
        [Enum2Type.EnumValue("执行次数")]
        ExeNum, // 
        [Enum2Type.EnumValue("用时")]
        Time, // 
        [Enum2Type.EnumValue("URL路径")]
        Url, // 存活个数
        [Enum2Type.EnumValue("调用时间顺序")]
        UseTime, // 调用时间顺序
        [Enum2Type.EnumValue("文件大小")]
        FileSize, // 调用时间顺序
    }

    ParamList mParamList = new ParamList();

    static string GetKey(string s, int lenght)
    {
        if (lenght == s.Length)
            return s;

        if (s.Length > lenght)
            return s.Substring(0, lenght);

        while (s.Length < lenght)
            s += " ";

        return s;
    }

    void OnShowList(Dictionary<string, TimeTrack> list)
    {
        List<string> keys = new List<string>(list.Count);
        List<string> show_keys = new List<string>(list.Count);
        float totaltime = 0f;

        {
            List<KeyValuePair<string, TimeTrack>> ls = new List<KeyValuePair<string, TimeTrack>>();
            foreach (KeyValuePair<string, TimeTrack> itor in list)
            {
                if (itor.Value.count != 0)
                    ls.Add(itor);
            }

            ls.Sort((KeyValuePair<string, TimeTrack> x, KeyValuePair<string, TimeTrack> y) => { return y.Value.Totaltime.CompareTo(x.Value.Totaltime); });

            string searchKey = mParamList.GetString("searchKey");
            foreach (KeyValuePair<string, TimeTrack> itor in ls)
            {
                if (string.IsNullOrEmpty(searchKey) || itor.Key.ToLower().Contains(searchKey.ToLower()))
                {
                    keys.Add(itor.Key);
                    show_keys.Add(string.Format("{0} total:{1} count:{2} 平均:{3}", itor.Key, itor.Value.Totaltime.ToString("0.000"), itor.Value.count, (itor.Value.Totaltime / itor.Value.count).ToString("0.000")));//不导出中文
                    totaltime += itor.Value.Totaltime;
                }
            }
        }

        string key = mParamList.GetString("key");
        GuiTools.HorizontalField(false,
            () =>
            {
                string searchKey = mParamList.GetString("searchKey");
                EditorGUILayout.LabelField("搜索关键字:", GUILayout.Width(80f));
                searchKey = EditorGUILayout.TextField(searchKey, GUILayout.Width(100f));
                mParamList.Set("searchKey", searchKey);

                EditorGUILayout.LabelField("数据类型:" + totaltime.ToString("0.00"), GUILayout.Width(100f));
                key = GuiTools.StringPopup(false, key, show_keys);
                mParamList.Set("key", key);

                if (GUILayout.Button("清除所有", GUILayout.Width(50f)))
                {
                    foreach (KeyValuePair<string, TimeTrack> itor in list)
                        itor.Value.Reset();
                }

                if (GUILayout.Button("保存", GUILayout.Width(50f)))
                {
                    TimeTrackMgr.Instance.Save();
                }
            });

        SortType sortType = SortType.Time;
        bool isDasc = false;

        TimeTrack tt = null;
        XTools.Map<string, List<TimeTrack.Data>> suffix_keys = null;
        List<TimeTrack.Data> datas = new List<TimeTrack.Data>();
        string keyfind = mParamList.GetString("keyfind", "");
        GuiTools.HorizontalField(false,
            () =>
            {
                sortType = mParamList.Get<SortType>("sortType", sortType);
                EditorGUILayout.LabelField("排序类型", GUILayout.Width(50f));
                sortType = (SortType)GuiTools.EnumPopup(false, sortType, GUILayout.Width(150f));
                mParamList.Set("sortType", sortType);
                EditorGUILayout.Space();
                isDasc = mParamList.Get<bool>("isDasc", isDasc);
                EditorGUILayout.LabelField("升序", GUILayout.Width(30f));
                isDasc = EditorGUILayout.Toggle(isDasc, GUILayout.Width(20f));
                mParamList.Set("isDasc", isDasc);

                if (show_keys.Count != 0)
                {
                    list.TryGetValue(keys[show_keys.IndexOf(key)], out tt);
                    if (tt == null)
                        return;
                }

                datas = new List<TimeTrack.Data>();
                if (tt != null)
                {
                    foreach (KeyValuePair<string, TimeTrack.Data> itor in tt.GetAll())
                    {
                        datas.Add(itor.Value);
                    }
                }

                switch (sortType)
                {
                case SortType.ExeNum:
                    datas.Sort((TimeTrack.Data x, TimeTrack.Data y) => { return x.times.Count.CompareTo(y.times.Count); });
                    break;
                case SortType.Time:
                    datas.Sort((TimeTrack.Data x, TimeTrack.Data y) => { return x.total.CompareTo(y.total); });
                    break;
                case SortType.UseTime:
                    datas.Sort((TimeTrack.Data x, TimeTrack.Data y) => { return x.starttime.CompareTo(y.starttime); });
                    break;
                case SortType.Url:
                    break;
                case SortType.FileSize:
                    datas.Sort((TimeTrack.Data x, TimeTrack.Data y) => { return x.file_size.CompareTo(y.file_size); });
                    break;
                }

                if (!isDasc)
                {
                    datas.Reverse();
                }

                suffix_keys = new XTools.Map<string, List<TimeTrack.Data>>();
                suffix_keys.Add("all", datas);
                foreach (TimeTrack.Data data in datas)
                {
                    suffix_keys[System.IO.Path.GetExtension(data.key)].Add(data);
                }

                keyfind = mParamList.GetString("keyfind", "");
                EditorGUILayout.LabelField("关键字搜索:", GUILayout.Width(80f));
                keyfind = EditorGUILayout.TextField(keyfind, GUILayout.Width(80f));
                mParamList.Set("keyfind", keyfind);

                EditorGUILayout.LabelField("  后缀名过滤:", GUILayout.Width(80f));
                string selected = mParamList.GetString("selected", "all");
                selected = GuiTools.MapStringPopup<List<TimeTrack.Data>>("", selected, suffix_keys,
                    (List<KeyValuePair<string, List<TimeTrack.Data>>> ls) =>
                    {
                        ls.Sort(
                            (KeyValuePair<string, List<TimeTrack.Data>> y, KeyValuePair<string, List<TimeTrack.Data>> x) =>
                            {
                                float xt = 0f;
                                float yt = 0f;
                                foreach (TimeTrack.Data d in x.Value)
                                    xt += d.total;

                                foreach (TimeTrack.Data d in y.Value)
                                    yt += d.total;

                                return xt.CompareTo(yt);
                            });
                    },
                    (KeyValuePair<string, List<TimeTrack.Data>> v) =>
                    {
                        float total = 0f;
                        foreach (TimeTrack.Data d in v.Value)
                            total += d.total;

                        return string.Format("{0} num:{1} totaltime:{2} 平均:{3}", GetKey(v.Key, 10), v.Value.Count, total.ToString("0.000"), (total / v.Value.Count).ToString("0.000"));//不导出中文
                    },
                    null,
                    GUILayout.Width(250f));

                if (string.IsNullOrEmpty(selected))
                    return;

                mParamList.Set("selected", selected);
                datas = suffix_keys[selected];

                if (!string.IsNullOrEmpty(keyfind))
                {
                    List<TimeTrack.Data> tmpdatas = new List<TimeTrack.Data>();
                    foreach (TimeTrack.Data d in datas)
                    {
                        if (d.key.ToLower().Contains(keyfind.ToLower()))
                            tmpdatas.Add(d);
                    }

                    datas = tmpdatas;
                }

                float time = 0f;
                foreach (TimeTrack.Data td in datas)
                    time += td.total;

                EditorGUILayout.LabelField("total:" + time, GUILayout.Width(100f));
                if (GUILayout.Button("清除当前数据", GUILayout.Width(100f)))
                    tt.Reset();
            });

        EditorPageBtn page = mParamList.Get<EditorPageBtn>("page");
        page.total = datas.Count;
        page.pageNum = 30;
        page.OnRender();

        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
        for (int i = page.beginIndex; i < page.endIndex; ++i)
        {
            TimeTrack.Data data = datas[i];
            GuiTools.HorizontalField(true,
                (System.Action)(() =>
                {
                    EditorGUILayout.TextField(string.Format(
                        "总用时:{0}({1}桢) 起始:{2}({3}) 结束:{4}({5}) 请求次数:{6} url:{7} 大小:{8}",
                        data.total.ToString("0.00"),
                        data.end_frame - data.begin_frame,
                        data.starttime_text,
                        data.begin_frame,
                        data.lasttime_text,
                        data.end_frame,
                        data.times.Count,
                        data.key,
                        XTools.Utility.ToMb(data.file_size)));

                    if (data.times.Count >= 2)
                    {
                        bool value = mParamList.Get<bool>(data.key, false);
                        value = EditorGUILayout.Toggle(value, GUILayout.ExpandWidth(false));
                        mParamList.Set(data.key, value);
                        EditorGUILayout.EndHorizontal();

                        if (value)
                        {
                            for (int m = 0; m < data.times.Count; ++m)
                                GuiTools.TextFieldFun(true, string.Format("({0}) usedtime:{1} time:{2}", m, data.times[m].usetime.ToString("0.000"), data.times[m].usetime_text));
                        }

                        EditorGUILayout.BeginHorizontal();
                    }
                }));
        }

        GUILayout.EndScrollView();
    }

    Vector2 ScrollPosition;

    TextAsset textAsset;

    void OnGUI()
    {
        TimeTrackMgr.CreateInstance();
        textAsset = EditorGUILayout.ObjectField("文本", textAsset, typeof(TextAsset), false) as TextAsset;
        if (GUILayout.Button("文本初始数据") && textAsset != null)
        {
            TimeTrackMgr.Instance.InitByJson(new JSONObject(textAsset.text));
        }
        if (GUILayout.Button("加载文本文件"))
        {
            string filename = EditorPrefs.GetString("AssetLoadRecord.OnGUI", Application.dataPath + "/../");
            filename = EditorUtility.OpenFilePanel("文件", filename, "txt");
            if (!string.IsNullOrEmpty(filename))
            {
                EditorPrefs.SetString("AssetLoadRecord.OnGUI", filename);
                TimeTrackMgr.Instance.InitByJson(new JSONObject(File.ReadAllText(filename)));
            }
        }


        Dictionary<string, TimeTrack> list = TimeTrackMgr.Instance.GetAll();
        OnShowList(list);
    }
}

#endif
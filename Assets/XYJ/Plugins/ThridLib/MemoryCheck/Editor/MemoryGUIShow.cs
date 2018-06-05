#if MEMORY_CHECK
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using XTools;

using MemoryPair = System.Collections.Generic.KeyValuePair<string, MemoryInfo.TypeData>;

public class MemoryGUIShow
{
    ParamList mParamList = new ParamList();

    public void Release()
    {
        mParamList.ReleaseAll();
    }

    public EditorPageBtn mEditorPageBtn = new EditorPageBtn();
    public MemoryInfo mMemoryInfo = new MemoryInfo();

    public string select = null;

    // 排序类型
    public enum SortType
    {
        //
        所有,
        存在个数,
        历史最高,
        最后创建时间
    }

    public SortType mSortType = SortType.所有;
    public bool mIsReverse = false;
    string search_key = "";

    protected virtual string Text
    {
        get
        {
            return MemoryObjectMgr.Text();
        }
    }

    List<MemoryInfo.TypeData> mTypeDatas = new List<MemoryInfo.TypeData>();
    string tag_type;

    static void AddTypeToList(HashSet<string> list, Type type)
    {
        if (!string.IsNullOrEmpty(type.Namespace))
        {
            list.Add(type.Namespace);
        }

        list.Add(type.Name);
        if (type.BaseType != null)
            AddTypeToList(list, type.BaseType);
    }

    static public bool IsContain(Type type, string tag)
    {
        if (type.Name.ToLower().Contains(tag))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(type.Namespace))
        {
            if (type.Namespace.ToLower().Contains(tag))
                return true;
        }

        if (type.BaseType != null)
            return IsContain(type.BaseType, tag);

        return false;
    }

    static public bool IsType(Type type, string tag)
    {
        if (!string.IsNullOrEmpty(type.Namespace))
        {
            if (tag.Contains(type.Namespace))
                return true;
        }

        if (tag.Contains(type.Name))
            return true;

        if (type.BaseType != null)
            return IsType(type.BaseType, tag);

        return false;
    }

    void OnShowTag()
    {
        List<string> basetypes = new List<string>();
        List<MemoryInfo.TypeData> tds = new List<MemoryInfo.TypeData>(mMemoryInfo.List);
        tds.Sort((MemoryInfo.TypeData x, MemoryInfo.TypeData y) => { return y.total.CompareTo(x.total); });

        basetypes.Add("All");
        for (int i = 0; i < tds.Count; ++i)
        {
            MemoryInfo.TypeData td = tds[i];
            basetypes.Add(string.Format("{2}({0}@{1}) ", td.total, td.historyTotal, Utility.GetTypeNameFull(td.fullName)));
        }

        tag_type = GUIEditor.GuiTools.StringPopup("Tag", tag_type, basetypes);
    }

    public void OnGUI(bool isforce = false)
    {
        if (mMemoryInfo == null)
            return;

        EditorGUILayout.LabelField(string.Format("概要:{0}", mMemoryInfo.ToString()));
        GUI.changed = false;
        search_key = EditorGUILayout.TextField("搜索key", search_key);
        EditorGUILayout.BeginHorizontal();
        mSortType = (SortType)EditorGUILayout.EnumPopup("排序", mSortType);
        mIsReverse = EditorGUILayout.Toggle("倒序", mIsReverse);
        EditorGUILayout.EndHorizontal();
        //OnShowTag();

        if (isforce || GUI.changed || mTypeDatas.Count == 0)
        {
            mTypeDatas.Clear();
            if (string.IsNullOrEmpty(search_key))
                mTypeDatas.AddRange(mMemoryInfo.List);
            else
            {
                mTypeDatas.Clear();
                int count = mMemoryInfo.List.Count;
                for (int i = 0; i < count; ++i)
                {
                    MemoryInfo.TypeData td = mMemoryInfo.List[i];
                    if (IsContain(td.type, search_key.ToLower()))
                        mTypeDatas.Add(td);
                }
            }
        }

        int total = 0;
        int histTotal = 0;
        for (int i = 0; i < mTypeDatas.Count; ++i)
        {
            MemoryInfo.TypeData td = mTypeDatas[i];
            total += td.total;
            histTotal += td.historyTotal;
        }

        EditorGUILayout.TextField(string.Format("num:{0}/{1}", total, histTotal));

        //if (tag_type != "All")
        //{
        //    int count = mTypeDatas.Count;
        //    List<MemoryInfo.TypeData> temps = new List<MemoryInfo.TypeData>();
        //    for (int i = 0; i < count; ++i)
        //    {
        //        MemoryInfo.TypeData td = mMemoryInfo.List[i];
        //        if (IsType(td.type, tag_type))
        //            temps.Add(td);
        //    }

        //    mTypeDatas.Clear();
        //    mTypeDatas.AddRange(temps);
        //}

        mEditorPageBtn.total = mTypeDatas.Count;
        mEditorPageBtn.pageNum = 30;

        switch (mSortType)
        {
        case SortType.所有:
            break;
        case SortType.存在个数:
            {
                mTypeDatas.Sort((MemoryInfo.TypeData x, MemoryInfo.TypeData y)=> { return x.fullName.CompareTo(y.fullName); });
                mTypeDatas.Sort((MemoryInfo.TypeData x, MemoryInfo.TypeData y) =>
                {
                    return x.total.CompareTo(y.total);
                });
            }
            break;
        case SortType.历史最高:
            {
                mTypeDatas.Sort((MemoryInfo.TypeData x, MemoryInfo.TypeData y)=> { return x.fullName.CompareTo(y.fullName); });
                mTypeDatas.Sort((MemoryInfo.TypeData x, MemoryInfo.TypeData y) =>
                {
                    return x.historyTotal.CompareTo(y.historyTotal);
                });
            }
            break;
        case SortType.最后创建时间:
            {
                mTypeDatas.Sort((MemoryInfo.TypeData x, MemoryInfo.TypeData y)=> { return x.fullName.CompareTo(y.fullName); });
                mTypeDatas.Sort((MemoryInfo.TypeData x, MemoryInfo.TypeData y) =>
                {
                    return x.last_time.CompareTo(y.last_time);
                });
            }
            break;
        }

        if (mIsReverse)
            mTypeDatas.Reverse();

        mEditorPageBtn.OnRender();
        mEditorPageBtn.ForEach((int index) => 
        {
            MemoryInfo.TypeData td = mTypeDatas[index];
            string text = string.Format("{2}({0}@{1}) ", td.total, td.historyTotal, Utility.GetTypeNameFull(td.fullName));
            EditorGUILayout.TextField(text);
        }, 
        true);
    }
}
#endif
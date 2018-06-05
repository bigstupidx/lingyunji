#if MEMORY_CHECK
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using XTools;

using MemoryPair = System.Collections.Generic.KeyValuePair<string, MemoryInfo.TypeData>;

public class MemoryDifferShow
{
    ParamList mParamList = new ParamList();

    public void Release()
    {
        mParamList.ReleaseAll();
    }

    public EditorPageBtn mEditorPageBtn = new EditorPageBtn();
    public MemoryDiffer mMemoryDiffer = new MemoryDiffer();

    public string select = null;

    // 排序类型
    public enum SortType
    {
        //
        所有,
        变化数量,
        存在个数,
        历史最高,
        最后创建时间
    }

    public SortType mSortType = SortType.所有;
    public bool mIsReverse = false;
    string search_key = "";

    List<MemoryDiffer.Data> mTypeDatas = new List<MemoryDiffer.Data>();
    string tag_type;
    bool isShowDiff = false;

    public void OnGUI(bool isforce = false)
    {
        if (mMemoryDiffer == null)
            return;

        EditorGUILayout.LabelField(string.Format("概要:{0}", mMemoryDiffer.ToString()));
        GUI.changed = false;
        search_key = EditorGUILayout.TextField("搜索key", search_key);
        EditorGUILayout.BeginHorizontal();
        mSortType = (SortType)EditorGUILayout.EnumPopup("排序", mSortType);
        mIsReverse = EditorGUILayout.Toggle("倒序", mIsReverse);
        isShowDiff = EditorGUILayout.Toggle("只显示差异", isShowDiff);
        EditorGUILayout.EndHorizontal();

        if (isforce || GUI.changed || mTypeDatas.Count == 0)
        {
            mTypeDatas.Clear();
            if (string.IsNullOrEmpty(search_key))
            {
                if (isShowDiff)
                {
                    int count = mMemoryDiffer.List.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        if (mMemoryDiffer.List[i].subCount != 0)
                            mTypeDatas.Add(mMemoryDiffer.List[i]);
                    }
                }
                else
                {
                    mTypeDatas.AddRange(mMemoryDiffer.List);
                }
            }
            else
            {
                mTypeDatas.Clear();
                int count = mMemoryDiffer.List.Count;
                for (int i = 0; i < count; ++i)
                {
                    MemoryDiffer.Data td = mMemoryDiffer.List[i];
                    if (isShowDiff && td.subCount == 0)
                        continue;

                    if (MemoryGUIShow.IsContain(td.type, search_key.ToLower()))
                        mTypeDatas.Add(td);
                }
            }
        }

        int total = 0;
        int histTotal = 0;
        for (int i = 0; i < mTypeDatas.Count; ++i)
        {
            MemoryDiffer.Data td = mTypeDatas[i];
            total += td.subCount;
            histTotal += td.subHistoryCount;
        }

        EditorGUILayout.TextField(string.Format("num:{0}/{1}", total, histTotal));

        mEditorPageBtn.total = mTypeDatas.Count;
        mEditorPageBtn.pageNum = 30;

        switch (mSortType)
        {
        case SortType.所有:
            break;
        case SortType.变化数量:
            {
                mTypeDatas.Sort((MemoryDiffer.Data x, MemoryDiffer.Data y) => { return x.fullName.CompareTo(y.fullName); });
                mTypeDatas.Sort((MemoryDiffer.Data x, MemoryDiffer.Data y) =>
                {
                    return x.subCount.CompareTo(y.subCount);
                });
            }
            break;
        case SortType.存在个数:
            {
                mTypeDatas.Sort((MemoryDiffer.Data x, MemoryDiffer.Data y)=> { return x.fullName.CompareTo(y.fullName); });
                mTypeDatas.Sort((MemoryDiffer.Data x, MemoryDiffer.Data y) =>
                {
                    return x.total.CompareTo(y.total);
                });
            }
            break;
        case SortType.历史最高:
            {
                mTypeDatas.Sort((MemoryDiffer.Data x, MemoryDiffer.Data y)=> { return x.fullName.CompareTo(y.fullName); });
                mTypeDatas.Sort((MemoryDiffer.Data x, MemoryDiffer.Data y) =>
                {
                    return x.historyTotal.CompareTo(y.historyTotal);
                });
            }
            break;
        case SortType.最后创建时间:
            {
                mTypeDatas.Sort((MemoryDiffer.Data x, MemoryDiffer.Data y)=> { return x.fullName.CompareTo(y.fullName); });
                mTypeDatas.Sort((MemoryDiffer.Data x, MemoryDiffer.Data y) =>
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
            MemoryDiffer.Data td = mTypeDatas[index];
            string text = Utility.GetTypeNameFull(td.fullName);
            int subCount = td.subCount;
            if (subCount == 0)
            {
                text += string.Format("无差异!总数:{0}/{1}", td.total, td.historyTotal);
            }
            else if (subCount > 0)
            {
                text += string.Format("增加数量:{0}! 总数:{1}/{2}", subCount, td.total, td.historyTotal);
            }
            else
            {
                text +=  string.Format("减少数量:{0}! 总数:{1}/{2}", subCount, td.total, td.historyTotal);
            }

            EditorGUILayout.TextField(text);
        }, 
        true);
    }
}
#endif
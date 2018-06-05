using PackTool;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if MEMORY_CHECK
abstract class NetDebugBaseEditor<T> : BaseEditorWindow where T : new()
{
    protected static DebugProtocol protocol;

    protected static System.Func<Network.BitStream, T> ReadStream;
    protected static System.Func<T, T, bool> Compare;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (NetDebugEditor.Channel != null)
            NetDebugEditor.Channel.Reg(protocol, OnMessage);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (NetDebugEditor.Channel != null)
            NetDebugEditor.Channel.UnReg(protocol);
    }

    static List<T> Datas = new List<T>();

    static void OnMessage(Network.SocketClient socket, Network.BitStream stream)
    {
        T info = ReadStream(stream);
        lock (Datas)
        {
            Datas.Add(info);
        }
    }

    // 接收到的总数据
    protected List<T> mCurrents = new List<T>();

    protected override void OnUpdate()
    {
        base.OnUpdate();
        lock (Datas)
        {
            mCurrents.AddRange(Datas);
            Datas.Clear();
        }
    }

    protected T CurrentInfo;

    protected EditorPageBtn mEditorPageBtn = new EditorPageBtn();

    protected ParamList mParamList = new ParamList();

    protected enum ShowType
    {
        ShowAll,
        ShowCurrent,
        ShowPause,
    }

    ShowType mShowType = ShowType.ShowAll;
    bool isForce = false;

    int compare_index = -1;

    protected abstract void BeginCompare(T x, T y);
    protected abstract void Show(T x, ParamList pl, bool isForce);

    protected virtual void OnGUI()
    {
        mShowType = (ShowType)EditorGUILayout.EnumPopup("显示类型", mShowType);
        switch (mShowType)
        {
        case ShowType.ShowAll:
            {
                mEditorPageBtn.total = mCurrents.Count;
                mEditorPageBtn.pageNum = 30;
                mEditorPageBtn.OnRender();

                if (compare_index >= mEditorPageBtn.total)
                    compare_index = -1;

                mEditorPageBtn.ForEach((int index) =>
                {
                    Color color = GUI.color;
                    GUI.color = compare_index == index ? Color.yellow : color;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(mCurrents[index].ToString());
                    bool isShow = mParamList.Get<bool>(index.ToString(), false);
                    if (GUILayout.Button(isShow ? "隐藏" : "显示"))
                        isShow = !isShow;

                    if (compare_index == index)
                    {
                        if (GUILayout.Button("取消对比"))
                        {
                            compare_index = -1;
                        }
                    }
                    else
                    {
                        if (compare_index != -1)
                        {
                            if (GUILayout.Button("开始比较"))
                            {
                                BeginCompare(mCurrents[compare_index], mCurrents[index]);
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("比较"))
                            {
                                compare_index = index;
                            }
                        }
                    }

                    GUI.color = color;
                    EditorGUILayout.EndHorizontal();
                    mParamList.Set(index.ToString(), isShow);
                    if (isShow)
                    {
                        Color c = GUI.color;
                        GUI.color = Color.yellow;
                        Show(mCurrents[index], mParamList.Get<ParamList>("GUIShow-" + index.ToString()), true);
                        GUI.color = c;
                    }
                },
                true);

                return;
            }
        case ShowType.ShowCurrent:
            {
                T newinfo = default(T);
                if (mCurrents.Count != 0)
                {
                    newinfo = mCurrents[mCurrents.Count - 1];
                }

                if (!Compare(CurrentInfo, newinfo))
                {
                    CurrentInfo = newinfo;
                    isForce = true;
                }
            }
            break;
        case ShowType.ShowPause:
            break;
        }

        Show(CurrentInfo, mParamList.Get<ParamList>("Curren"), isForce);
    }
}
#endif
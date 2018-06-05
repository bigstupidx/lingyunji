using System;
using XTools;
using UnityEngine;
using UnityEditor;
using FMOD.Studio;
using System.Collections.Generic;

namespace FMODUnity
{
    public class SoundRecordEditor : BaseEditorWindow
    {
        [MenuItem("FMOD/声音播放记录", false, 9)]
        static public void OpenSoundRecordEditor()
        {
            GetWindow<SoundRecordEditor>(false, "声音播放记录", true).ShowPopup();
        }

        enum State
        {
            Begin, // 刚开始
            Playing, // 播放中
            End, // 已结束
        }

        [Serializable]
        class Data
        {
            public Guid guid;
            public int onlyid; // 实例唯一ID
            [FMODUnity.EventRef]
            public string path;
            public DateTime startTime; // 起始播放时间
            public DateTime endTime; // 结束播放时间

            public static string To(DateTime t)
            {
                return string.Format("{0}:{1} {2}", t.Hour, t.Minute, t.Second);
            }

            public State state = State.Begin;

            public void OnGUI()
            {
                GUILayout.Label(System.IO.Path.GetFileNameWithoutExtension(path), GUILayout.Width(120f));
                GUILayout.Label(To(startTime), GUILayout.Width(120f));

                switch (state)
                {
                case State.Begin: GUILayout.Label("开始", GUILayout.Width(60f)); break;
                case State.Playing: GUILayout.Label("播放中", GUILayout.Width(60f)); break;
                case State.End: GUILayout.Label("结束", GUILayout.Width(60f)); break;
                }

                switch (state)
                {
                case State.Begin: GUILayout.Label("0.00", GUILayout.Width(60f)); break;
                case State.Playing: GUILayout.Label(((DateTime.Now - startTime).TotalMilliseconds / 1000f).ToString("0.00"), GUILayout.Width(60f)); break;
                case State.End: GUILayout.Label(((endTime - startTime).TotalMilliseconds / 1000f).ToString("0.00"), GUILayout.Width(60f)); break;
                }
            }
        }

        // 播放的数据
        Dictionary<int, Data> Plays = new Dictionary<int, Data>();

        [SerializeField]
        List<Data> PlayList = new List<Data>();

        SerializedObject serializedObject;

        void OnGUI()
        {
            if (serializedObject == null)
                serializedObject = new SerializedObject(this);

            var dic = RuntimeManager.CachedDescriptions;
            if (dic == null)
            {
                GUILayout.Label("当前没有正在播放的音频！");
                return;
            }

            Dictionary<int, Data> newPlays = new Dictionary<int, Data>();
            foreach (var itor in dic)
            {
                EventDescription desc = itor.Value;
                EventInstance[] eis = null;
                if (desc.getInstanceList(out eis) == FMOD.RESULT.OK)
                {
                    foreach (var ei in eis)
                    {
                        Data d = new Data();
                        d.guid = itor.Key;
                        d.onlyid = ei.GetHashCode();
                        desc.getPath(out d.path);

                        newPlays.Add(d.onlyid, d);
                    }
                }
            }

            var currentTime = DateTime.Now;
            newPlays.Compare(Plays,
                (KeyValuePair<int, Data> kv1, KeyValuePair<int, Data> kv2) =>
                {
                    // 都有的，说明还在播放当中，先不处理
                    kv2.Value.state = State.Playing;
                },
                (KeyValuePair<int, Data> kv) => 
                {
                    // 新添加的，正在播放的
                    kv.Value.startTime = currentTime;
                    kv.Value.endTime = new DateTime(0);
                    kv.Value.state = State.Begin;

                    Plays.Add(kv.Key, kv.Value);
                },
                (KeyValuePair<int, Data> kv) => 
                {
                    // 播放已结束的
                    if (kv.Value.state != State.End)
                    {
                        kv.Value.state = State.End;
                        kv.Value.endTime = currentTime;
                    }
                });

            PlayList.Clear();
            foreach (var itor in Plays)
                PlayList.Add(itor.Value);

            PlayList.Sort((Data x, Data y)=> { return y.startTime.CompareTo(x.startTime); });

            if (GUILayout.Button("清除播放结束的", GUILayout.Width(100), GUILayout.Height(50f)))
            {
                for (int i = 0; i < PlayList.Count; ++i)
                {
                    if (PlayList[i].state == State.End)
                    {
                        Plays.Remove(PlayList[i].onlyid);
                    }
                }
            }

            for (int i = 0; i < PlayList.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("{0})", i), GUILayout.Width(60f));
                PlayList[i].OnGUI();
                GUILayout.EndHorizontal();
            }
        }
    }
}